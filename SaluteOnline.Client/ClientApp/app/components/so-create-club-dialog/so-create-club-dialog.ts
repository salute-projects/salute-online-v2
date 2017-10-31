import { Component, Inject, ViewEncapsulation } from "@angular/core";
import { Context } from "../../services/context/context";
import { GlobalState } from "../../services/global.state";
import { SoSnackService } from "../../services/snack.service";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatDialog, MatDialogRef, MatDialogConfig, MAT_DIALOG_DATA } from "@angular/material";
import { CreateClubDto } from "../../dto/dto";
import { Helpers } from "../../services/helpers";

@Component({
    selector: "so-create-club-dialog",
    templateUrl: "./so-create-club-dialog.html",
    styleUrls: ["./so-create-club-dialog.scss"],
    encapsulation: ViewEncapsulation.None,
    providers: [SoSnackService]
})

export class CreateClubDialog {
    private createClubForm: FormGroup;

    constructor(public dialogRef: MatDialogRef<CreateClubDialog>, @Inject(MAT_DIALOG_DATA) public data: any, private readonly fb: FormBuilder, private readonly snackService: SoSnackService,
        private readonly context: Context, private readonly helpers: Helpers) {
        this.createForm();
    }

    private createForm() {
        this.createClubForm = this.fb.group({
            title: ['', Validators.required],
            country: ['', Validators.required],
            city: ['', Validators.required],
            description: ''
        });
    }

    createClub() {
        const args = this.helpers.formToObject(this.createClubForm, new CreateClubDto());
        this.context.clubsApi.createClub(args).subscribe((result: any) => {
            debugger;
            this.dialogRef.close(result);
        }, error => {
            debugger;
            this.snackService.showError(error.error, 'OK', undefined);
        });
    }
}