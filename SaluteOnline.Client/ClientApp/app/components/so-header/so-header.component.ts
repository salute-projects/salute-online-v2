import { Component, ViewEncapsulation } from "@angular/core";
import { Router } from "@angular/router";
import { GlobalState } from "../../services/global.state";
import { AuthService } from "../../services/auth";
import { MatDialog, MatDialogConfig} from "@angular/material";
import { LoginDialog } from "../so-login-dialog/so-login-dialog";

@Component({
    selector: "so-header",
    styleUrls: ["./so-header.component.scss"],
    templateUrl: "./so-header.component.html",
    encapsulation: ViewEncapsulation.None
})

export class SoHeader {
    isMenuCollapsed = false;

    logged = false;

    email: string;
    password: string;

    constructor(private readonly state: GlobalState, private readonly router: Router, private readonly authService: AuthService, public loginDialog: MatDialog) {
        this.state.subscribe('menu.isCollapsed', (isCollapsed: boolean) => {
            this.isMenuCollapsed = isCollapsed;
        });
        this.state.subscribe('global.loggedIn', (logged: boolean) => {
            this.logged = logged;
        });
    }

    openDialog(): void {
        const config: MatDialogConfig = {
            width: '400px',
            panelClass: 'login-dialog-panel',
            data: { email: this.email, password: this.password }
        };
        const dialogRef = this.loginDialog.open(LoginDialog, config);
        dialogRef.afterClosed().subscribe(result => {
            this.logged = result ? true : false;
        }, error => {
            this.logged = false;
        });
    }

    logout() {
        this.authService.logout();
    }

    toggleMenu() {
        this.isMenuCollapsed = !this.isMenuCollapsed;
        this.state.notifyDataChanged(this.state.events.menu.isCollapsed, this.isMenuCollapsed);
    }

    gotoUserProfile() {
        this.router.navigateByUrl('/so-user-profile');
    }
}