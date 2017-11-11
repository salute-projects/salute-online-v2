import { Component, Inject, ViewEncapsulation } from "@angular/core";
import { Context } from "../../services/context/context";
import { GlobalState } from "../../services/global.state";
import { AuthService } from "../../services/auth";
import { SoSnackService } from "../../services/snack.service";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatDialog, MatDialogRef, MatDialogConfig, MAT_DIALOG_DATA } from "@angular/material";
import { EqualityValidation } from "../../services/validators";

@Component({
    selector: "login-dialog",
    templateUrl: "./so-login-dialog.html",
    styleUrls: ["./so-login-dialog.scss"],
    encapsulation: ViewEncapsulation.None,
    providers: [ SoSnackService ]
})

export class LoginDialog {
    email: string;
    password: string;

    loginForm: FormGroup;
    signupForm: FormGroup;

    private logo = require('../../assets/logo.png');

    constructor(public dialogRef: MatDialogRef<LoginDialog>, @Inject(MAT_DIALOG_DATA) public data: any, private readonly fb: FormBuilder, private readonly snackService: SoSnackService,
        private readonly auth: AuthService, private readonly context: Context) {
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

    submitLogin() {
        const email = this.loginForm.get('email');
        const password = this.loginForm.get('password');
        if (!email || !password)
            return;
        this.email = email.value;
        this.password = password.value;
        this.login();
    }

    submitSignup() {
        const email = this.signupForm.get('email');
        const password = this.signupForm.get('password');
        if (!email || !password)
            return;
        this.email = email.value;
        this.password = password.value;
        this.signUp();
    }

    login() {
        this.auth.login(this.email, this.password).subscribe(result => {
            this.dialogRef.close(result);
        },
        error => {
            this.snackService.showError(error.error, 'OK', undefined);
        });
    }

    signUp() {
        this.auth.signUp(this.email, this.password).subscribe(result => {
            this.dialogRef.close(result);
        }, error => {
            this.snackService.showError(error.error, 'OK', undefined);
        });
    }

    forgotPassword() {
        const email = this.loginForm.get('email');
        if (!email)
            return;
        this.email = email.value;
        this.auth.forgotPassword(this.email).subscribe(result => {
            this.snackService.showGeneral(result, 'OK');
            this.dialogRef.close();
        }, error => {
            this.snackService.showError(error.error, 'OK', undefined);
        });
    }

    forgotPasswordDisabled() {
        const email = this.loginForm.get('email');
        return !(email && email.valid);
    }
}