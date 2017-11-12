var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Injectable } from "@angular/core";
import { MatSnackBar } from '@angular/material';
var SoSnackService = /** @class */ (function () {
    function SoSnackService(snackBar) {
        this.snackBar = snackBar;
    }
    SoSnackService.prototype.showGeneral = function (message, action) {
        return this.core(message, action, 3000, []);
    };
    SoSnackService.prototype.showError = function (message, action, duration) {
        if (duration === void 0) { duration = 5000; }
        return this.core(message, action, duration, ['so-error-snackbar']);
    };
    SoSnackService.prototype.core = function (message, action, duration, extraClasses) {
        if (action === void 0) { action = "OK"; }
        if (duration === void 0) { duration = 3000; }
        var config = {
            duration: duration,
            extraClasses: extraClasses
        };
        var ref = this.snackBar.open(message, action, config);
        return ref.afterDismissed();
    };
    SoSnackService = __decorate([
        Injectable(),
        __metadata("design:paramtypes", [MatSnackBar])
    ], SoSnackService);
    return SoSnackService;
}());
export { SoSnackService };
//# sourceMappingURL=snack.service.js.map