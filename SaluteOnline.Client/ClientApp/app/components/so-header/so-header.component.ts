import { Component, ViewEncapsulation, Inject } from "@angular/core";
import { Router } from "@angular/router";
import { GlobalState } from "../../services/global.state";
import { AuthService } from "../../services/auth";
import { MatDialog, MatDialogRef, MatDialogConfig, MAT_DIALOG_DATA } from "@angular/material";

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
            debugger;
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
            var x = result;
        });
    }

    login() {
        this.authService.login();
    }

    logout() {
        this.authService.logout();
    }

    toggleMenu() {
        this.isMenuCollapsed = !this.isMenuCollapsed;
        this.state.notifyDataChanged('menu.isCollapsed', this.isMenuCollapsed);
    }
}

@Component({
    selector: "login-dialog",
    templateUrl: "./login-dialog.html"
})

export class LoginDialog {
    email: string;
    password: string;
    confirmPassword: string;
    private logo = require('../../assets/logo.png');

    constructor(public dialogRef: MatDialogRef<LoginDialog>, @Inject(MAT_DIALOG_DATA) public data: any) {
    }

    onNoClick(): void {
        this.dialogRef.close();
    }
}