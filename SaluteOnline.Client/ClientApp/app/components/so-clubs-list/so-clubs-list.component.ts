import { Component, ViewEncapsulation } from "@angular/core";
import { Context } from "../../services/context/context";
import { GlobalState } from "../../services/global.state";
import { SoSnackService } from "../../services/snack.service";
import { Observable } from 'rxjs/Observable';
import { MatDialog, MatDialogConfig } from "@angular/material";
import { CreateClubDialog } from "../so-create-club-dialog/so-create-club-dialog";
import { Page, ClubDto, ClubFilter, ClubInfoAggregation } from "../../dto/dto";

@Component({
    selector: 'so-clubs-list',
    styleUrls: ['./so-clubs-list.component.scss'],
    templateUrl: './so-clubs-list.component.html',
    encapsulation: ViewEncapsulation.None
})

export class SoClubsList {
    clubInfoAggregation: ClubInfoAggregation;
    clubsFilter: ClubFilter;
    clubs: Page<ClubDto>;

    constructor(private readonly context: Context, private readonly snackService: SoSnackService, private readonly state: GlobalState, private readonly loginDialog: MatDialog) {
        this.clubs = new Page<ClubDto>();
        this.clubsFilter = new ClubFilter();
        this.refreshClubsList();
    }

    private refreshClubsList() {
        this.context.clubsApi.getList(this.clubsFilter).subscribe((result: Page<ClubDto>) => {
            this.clubs = result;
        }, error => {
            this.snackService.showError(error.error, "OK");
            });
        this.context.clubsApi.getClubInfoAggregation().subscribe((result: ClubInfoAggregation) => {
            debugger;
            this.clubInfoAggregation = result;
        }, error => {
            this.snackService.showError(error.error, "OK");
        });
    }

    createClub() : void {
        const config: MatDialogConfig = {
            width: '400px',
            panelClass: 'login-dialog-panel'
        };
        const dialogRef = this.loginDialog.open(CreateClubDialog, config);
        dialogRef.afterClosed().subscribe(result => {
            this.refreshClubsList();
        }, error => {
            this.snackService.showError(error.error, "OK");
        });
    }

    getClubAvatar(base64: string) {
        return `data:image/jpg;base64,${base64}`;
    }
}