﻿import { Injectable, Component } from "@angular/core";
import { MatSnackBar, MatSnackBarConfig } from '@angular/material';
import { Observable } from 'rxjs';

@Injectable()
export class SoSnackService {
    constructor(public snackBar: MatSnackBar) { }

    showGeneral(message: string, action: string | undefined): Observable<void> {
        return this.core(message, action, 1000, []);
    }

    showError(message: string, action: string | undefined, duration: number = 2000): Observable<void> {
        return this.core(message, action, 2000, ['so-error-snackbar']);
    }

    core(message: string, action: string = "OK", duration: number = 1000, extraClasses: string[]) {
        const config: MatSnackBarConfig = {
            duration: duration,
            extraClasses: extraClasses
        };
        const ref = this.snackBar.open(message, action, config);
        return ref.afterDismissed();
    }
}