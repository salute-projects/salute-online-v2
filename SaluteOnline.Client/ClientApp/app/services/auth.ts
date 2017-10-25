import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { GlobalState } from "../services/global.state";
import { Context } from "../services/context/context";
import { LoginResultDto, SignUpResultDto } from "../services/context/accountApi";
import { Observable, Observer } from 'rxjs';
import { HttpHeaders } from '@angular/common/http';

@Injectable()
export class AuthService {
    constructor(private readonly router: Router, private readonly state: GlobalState, private readonly context: Context) {}

    login(email: string, password: string): Observable<LoginResultDto> {
        return Observable.create((observer: Observer<LoginResultDto>) => {
            this.context.accountApi.login(email, password).subscribe(result => {
                    const expiresAt = JSON.stringify((result.expiresIn * 1000) + new Date().getTime());
                    localStorage.setItem('token', result.token);
                    localStorage.setItem('expires_at', expiresAt);
                    this.state.notifyDataChanged(this.state.events.global.logged, true);
                    observer.next(result);
                    observer.complete();
                },
                error => {
                    this.state.notifyDataChanged(this.state.events.global.logged, false);
                    observer.error(error);
                    observer.complete();
                });
        });
    }

    signUp(email: string, password: string): Observable<SignUpResultDto> {
        return Observable.create((observer: Observer<SignUpResultDto>) => {
            this.context.accountApi.signUp(email, password).subscribe(result => {
                const expiresAt = JSON.stringify((result.expiresIn * 1000) + new Date().getTime());
                localStorage.setItem('token', result.token);
                localStorage.setItem('expires_at', expiresAt);
                this.state.notifyDataChanged(this.state.events.global.logged, true);
                observer.next(result);
                observer.complete();
            }, error => {
                this.state.notifyDataChanged(this.state.events.global.logged, false);
                observer.error(error);
                observer.complete();
            });
        });
    }

    logout(): void {
        localStorage.removeItem('token');
        localStorage.removeItem('expires_at');
        this.state.notifyDataChanged(this.state.events.global.logged, false);
        this.router.navigate(['/']);
    }
            
    isAuthenticated(): boolean {
        const expires = localStorage.getItem('expires_at');
        if (!expires)
            return false;
        const expiresAt = JSON.parse(expires);
        return new Date().getTime() < expiresAt;
    }

    tryGetAuth(): HttpHeaders|undefined {
        if (!this.isAuthenticated())
            return undefined;
        const token = localStorage.getItem('token');
        return new HttpHeaders().set('Authorization', `Bearer ${token}`);
    }
}