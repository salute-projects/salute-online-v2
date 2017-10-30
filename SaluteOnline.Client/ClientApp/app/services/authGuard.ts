import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";
import { AuthService } from "./auth";

@Injectable()
export class AuthGuard implements CanActivate {
    constructor(private readonly auth: AuthService, private readonly router: Router) { }

    canActivate(route: Object, state: Object) {
        if (this.auth.isAuthenticated()) {
            return true;
        } else {
            //this.router.navigateByUrl('/home');
            return false;
        }
    }
}