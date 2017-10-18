import { AbstractControl } from "@angular/forms";

export class EqualityValidation {
    static checkEquality(control: AbstractControl) {
        const password = control.get('password');
        const confirm = control.get('confirmPassword');
        if (!password || !confirm)
            return null;
        if (password.value !== confirm.value) {
            confirm.setErrors({ checkEquality: true });
        } else {
            return null;
        }
    }
}