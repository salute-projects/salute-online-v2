import { Component } from "@angular/core";
import { Router, NavigationStart, NavigationEnd, NavigationError } from "@angular/router";
import { GlobalState } from "../../services/global.state";

@Component({
    selector: 'so-callback-component',
    templateUrl: 'callback.component.html'
})

export class SoCallback {
    token: any;

    constructor(private readonly router: Router, private readonly state: GlobalState) {
        try {
            const auth: any = {
                access_token: '',
                id_token: '',
                scope: '',
                expires_in: '',
                token_type: ''
            };
            const hash = router.url.substr(1).replace("callback#", "");
            hash.split('&').forEach((item: string) => {
                const parts = item.split('=');
                auth[parts[0]] = parts[1];
            });
            if (auth.access_token) {
                window.location.hash = '';
                this.setSession(auth);
                this.state.notifyDataChanged("global.loggedIn", true);
                debugger;
                this.router.navigate(['/home']);
            }
        } catch (e) {
            this.router.navigate(['/home']);
            this.state.notifyDataChanged("global.loggedIn", false);
        } 
    }

    private setSession(auth: any): void {
        if (!auth)
            return;
        const expiresAt = JSON.stringify((auth.expires_in * 1000) + new Date().getTime());
        localStorage.setItem('access_token', auth.access_token);
        localStorage.setItem('id_token', auth.id_token);
        localStorage.setItem('expires_at', expiresAt);
    }
}