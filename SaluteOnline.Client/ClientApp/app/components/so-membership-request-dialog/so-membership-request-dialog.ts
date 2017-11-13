import { Component, Inject, ViewEncapsulation } from "@angular/core";
import { Context } from "../../services/context/context";
import { GlobalState } from "../../services/global.state";
import { SoSnackService } from "../../services/snack.service";
import { FormBuilder, FormGroup, Validators, FormControl } from "@angular/forms";
import { MatDialog, MatDialogRef, MatDialogConfig, MAT_DIALOG_DATA } from "@angular/material";
import { ClubMemberSummary, ClubMemberFilter, UserDto, Page, MembershipRequestCreateDto } from "../../dto/dto";
import { Helpers } from "../../services/helpers";
import { Observable } from 'rxjs/Observable';
import { forkJoin } from "rxjs/observable/forkJoin"
import { AtLeastOneValidation } from "../../services/validators";

@Component({
    selector: "so-membership-request-dialog",
    templateUrl: "./so-membership-request-dialog.html",
    styleUrls: ["./so-membership-request-dialog.scss"],
    encapsulation: ViewEncapsulation.None,
    providers: [SoSnackService]
})

export class MembershipRequestDialog {
    private membershipRequestForm: FormGroup;
    private clubMembers: ClubMemberSummary[];
    private filteredMembers: Observable<ClubMemberSummary[]>;
    private readonly clubMembersFilter: ClubMemberFilter;
    private readonly id: number;

    constructor(public dialogRef: MatDialogRef<MembershipRequestDialog>, @Inject(MAT_DIALOG_DATA) public data: any, private readonly fb: FormBuilder, private readonly snackService: SoSnackService,
        private readonly context: Context, private readonly helpers: Helpers) {
        this.createForm();
        this.id = data.clubId;
        this.clubMembersFilter = new ClubMemberFilter(this.id);
        const getClubMembers = this.context.clubsApi.getClubMembers(this.clubMembersFilter);
        const getCurrentUser = this.context.userApi.getUserInfo();
        forkJoin([getClubMembers, getCurrentUser]).subscribe((results: any) => {
            this.clubMembers = (results[0] as Page<ClubMemberSummary>).items;
            const userInfo = results[1] as UserDto;
            this.membershipRequestForm.controls['nicknameManual'].setValue(userInfo.nickname);
            this.filteredMembers = (this.membershipRequestForm.controls['nickname'] as FormControl).valueChanges
                .startWith(null)
                .map((nickname: string) => nickname ? this.filterMembers(nickname) : this.clubMembers.slice());
        });

        this.context.clubsApi.getClubMembers(this.clubMembersFilter).subscribe(result => {
            this.clubMembers = result.items;
            this.filteredMembers = (this.membershipRequestForm.controls['nickname'] as FormControl).valueChanges.startWith(null)
                .map((nickname: string) => nickname ? this.filterMembers(nickname) : this.clubMembers.slice());
        }, error => {
            this.filteredMembers = Observable.from([]);
            this.snackService.showError(error.error, 'OK');
        });
    }

    private createForm() {
        this.membershipRequestForm = this.fb.group({
            nickname: '',
            nicknameManual: ''
        }, { validator: AtLeastOneValidation });
        this.membershipRequestForm.controls['nicknameManual'].valueChanges.subscribe(value => {
            if (value) {
                this.membershipRequestForm.controls['nickname'].setValue('');
            }
        });
    }

    private filterMembers(nick: string) {
        return this.clubMembers.filter(member => member.nickname.toLowerCase().indexOf(nick.toLowerCase()) === 0);
    }

    nicknameSelected(event: any) {
        const existing = this.membershipRequestForm.get('nickname');
        const manual = this.membershipRequestForm.get('nicknameManual');
        if (existing && existing.value && manual)
            manual.setValue('');
    }

    sendMembershipRequest() {
        const args = new MembershipRequestCreateDto();
        args.clubId = this.id;
        const nicknameControl = this.membershipRequestForm.get('nickname');
        const nicknameManualControl = this.membershipRequestForm.get('nicknameManual');
        args.selectedFromExisting = nicknameControl && nicknameControl.value ? true : false;
        args.nickname = nicknameManualControl && nicknameManualControl.value
            ? nicknameManualControl.value
            : nicknameControl ? nicknameControl.value : '';
        this.context.clubsApi.addMembershipRequest(args).subscribe(result => {
            this.snackService.showGeneral('Request successfully sent', 'OK');
            this.dialogRef.close(result);
        }, error => {
            this.snackService.showError(error.error, 'OK');
        });
    }
}