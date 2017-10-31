import { Component, ViewEncapsulation } from "@angular/core";
import { Context } from "../../services/context/context";
import { GlobalState } from "../../services/global.state";
import { SoSnackService } from "../../services/snack.service";
import { Observable } from 'rxjs/Observable';
import { MatDialog, MatDialogConfig } from "@angular/material";
import { CreateClubDialog } from "../so-create-club-dialog/so-create-club-dialog";

@Component({
    selector: 'so-clubs-list',
    styleUrls: ['./so-clubs-list.component.scss'],
    templateUrl: './so-clubs-list.component.html',
    encapsulation: ViewEncapsulation.None
})

export class SoClubsList {
    constructor(private readonly context: Context, private readonly snackService: SoSnackService, private readonly state: GlobalState, private readonly loginDialog: MatDialog) {
        
    }

    createClub() : void {
        const config: MatDialogConfig = {
            width: '400px',
            panelClass: 'login-dialog-panel'
        };
        const dialogRef = this.loginDialog.open(CreateClubDialog, config);
        dialogRef.afterClosed().subscribe(result => {
            debugger;
        }, () => {
            debugger;
        });
    }
}