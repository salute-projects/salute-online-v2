var EqualityValidation = /** @class */ (function () {
    function EqualityValidation() {
    }
    EqualityValidation.checkEquality = function (control) {
        var password = control.get('password');
        var confirm = control.get('confirmPassword');
        if (!password || !confirm)
            return null;
        if (password.value !== confirm.value) {
            confirm.setErrors({ checkEquality: true });
        }
        else {
            return null;
        }
    };
    return EqualityValidation;
}());
export { EqualityValidation };
var AtLeastOneValidation = /** @class */ (function () {
    function AtLeastOneValidation() {
    }
    AtLeastOneValidation.checkAtLeastOne = function (control) {
        var password = control.get('nickname');
        var confirm = control.get('nicknameManual');
        if (!password || !confirm)
            return null;
        if (!password.value && !confirm.value) {
            confirm.setErrors({ checkAtLeastOne: true });
        }
        else {
            return null;
        }
    };
    return AtLeastOneValidation;
}());
export { AtLeastOneValidation };
//# sourceMappingURL=validators.js.map