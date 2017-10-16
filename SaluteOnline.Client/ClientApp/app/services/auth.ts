import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import {} from ""
import * as auth0 from "auth0-angular2";
import { GlobalState } from "../services/global.state";

@Injectable()
export class AuthService {
    constructor(private readonly router: Router, private readonly state: GlobalState) {}

    login() {
        const service = new auth0.AuthService('3s-MDoybe9b6jDbMq6jvZAhMsRFhHTE7', 'saluteonline.eu.auth0.com', '/home', '/callback', this.router);
        service.login();
    }

    logout(): void {
        localStorage.removeItem('access_token');
        localStorage.removeItem('id_token');
        localStorage.removeItem('expires_at');
        this.state.notifyDataChanged("global.loggedIn", false);
        this.router.navigate(['/']);
    }

    isAuthenticated(): boolean {
        const expires = localStorage.getItem('expires_at');
        if (!expires)
            return false;
        const expiresAt = JSON.parse(expires);
        return new Date().getTime() < expiresAt;
    }
}