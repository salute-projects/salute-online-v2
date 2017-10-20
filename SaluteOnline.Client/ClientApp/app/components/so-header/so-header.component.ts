import { Component, ViewEncapsulation, Inject } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { GlobalState } from "../../services/global.state";
import { Context } from "../../services/context/context";
import { SoSnackService } from "../../services/snack.service";
import { AuthService } from "../../services/auth";
import { MatDialog, MatDialogRef, MatDialogConfig, MAT_DIALOG_DATA } from "@angular/material";
import { EqualityValidation } from "../../services/validators/equality-validator";

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
            debugger;
            var x = result;
        });
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
    templateUrl: "./login-dialog.html",
    providers: [Context, SoSnackService]
})

export class LoginDialog {
    email: string;
    password: string;
    confirmPassword: string;
    currentTab: string;

    loginForm: FormGroup;
    signupForm: FormGroup;

    private logo = require('../../assets/logo.png');

    constructor(public dialogRef: MatDialogRef<LoginDialog>, @Inject(MAT_DIALOG_DATA) public data: any, private readonly fb: FormBuilder, private readonly context: Context,
        private readonly snackService: SoSnackService) {
        this.currentTab = "login";
        this.createLoginForm();
        this.createSignupForm();
    }

    private createLoginForm() {
        this.loginForm = this.fb.group({
            email: ['', [Validators.required, Validators.email]],
            password: ['', Validators.required]
        });
    }

    private createSignupForm() {
        this.signupForm = this.fb.group({
            email: ['', [Validators.required, Validators.email]],
            password: ['', Validators.required],
            confirmPassword: ['', Validators.required]
        }, { validator: EqualityValidation.checkEquality });
    }

    tabChanged(tabIndex: number): void {
        switch (tabIndex) {
            case 0:
                this.currentTab = "login";
                break;
            case 1:
                this.currentTab = "signup";
                break;
            default:
        }
    }

    buttonText(): string {
        switch (this.currentTab) {
        case "signup":
            return "SIGN UP >";
        default:
                return "LOG IN >";    
        }
    }

    submitDisabled(): boolean {
        if (this.currentTab === "login") {
            return this.loginForm.invalid;
        } else if (this.currentTab === "signup") {
            return this.signupForm.invalid;
        }
        return true;
    }

    submit() {
        if (this.currentTab === "login") {
            const email = this.loginForm.get('email');
            const password = this.loginForm.get('password');
            if (!email || !password)
                return;
            this.email = email.value;
            this.password = password.value;
            this.login();
        } else {
            const email = this.signupForm.get('email');
            const password = this.signupForm.get('password');
            if (!email || !password)
                return;
            this.email = email.value;
            this.password = password.value;
            this.signUp();
        }
    }

    login() {
        this.context.accountApi.login(this.email, this.password).subscribe(result => {
            const expiresAt = JSON.stringify((result.expiresIn * 1000) + new Date().getTime());
            localStorage.setItem('token', result.token);
            localStorage.setItem('expires_at', expiresAt);
            this.dialogRef.close(result);
        }, error => {
            this.snackService.showError(error, 'OK', undefined);
        });
    }

    signUp() {
        this.context.accountApi.signUp(this.email, this.password).subscribe(result => {
            const expiresAt = JSON.stringify((result.expiresIn * 1000) + new Date().getTime());
            localStorage.setItem('token', result.token);
            localStorage.setItem('expires_at', expiresAt);
            this.dialogRef.close(result);
        }, error => {
            this.snackService.showError(error, 'OK', undefined);
        });
    }

    onNoClick(): void {
        this.dialogRef.close();
    }
}