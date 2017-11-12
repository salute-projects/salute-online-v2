import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { GlobalState } from "../services/global.state";
import { LoginResultDto, SignUpResultDto } from "../dto/dto";
import { Observable, Observer } from 'rxjs';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { apiSettings } from "../configuration/constants";

@Injectable()
export class AuthService {
    constructor(private readonly router: Router, private readonly state: GlobalState, private readonly http: HttpClient) {}

    urls: any = {
        userExists: '/account/userExists/',
        signUp: 'account/signUp/',
        login: 'account/login/',
        refreshToken: 'account/refreshToken/',
        forgotPassword: 'account/forgotPassword/'
    }

    login(email: string, password: string): Observable<LoginResultDto> {
        return Observable.create((observer: Observer<LoginResultDto>) => {
            const headers = new HttpHeaders().set('Access-Control-Allow-Origin', '*')
                .set('Access-Control-Allow-Methods', 'GET, POST, PATCH, PUT, DELETE, OPTIONS')
                .set('Access-Control-Allow-Headers', 'Origin, Content-Type, X-Auth-Token');
            const body = {
                email: email,
                password: password
            }
            return this.http.post<LoginResultDto>(apiSettings.baseUrl + this.urls.login, body, { headers: headers }).subscribe(result => {
                    const expiresAt = JSON.stringify((result.expiresIn * 1000) + new Date().getTime());
                    sessionStorage.setItem('token', result.token);
                    sessionStorage.setItem('expires_at', expiresAt);
                    localStorage.setItem('refresh_token', result.refreshToken);
                    this.state.notifyDataChanged(this.state.events.global.logged, true);
                    localStorage.setItem('avatar', `data:image/jpg;base64,${result.avatar}`);
                    this.state.notifyDataChanged(this.state.events.user.avatarChanged, '');
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
            const body = {
                email: email,
                password: password
            }
            return this.http.post<SignUpResultDto>(apiSettings.baseUrl + this.urls.signUp, body).subscribe(result => {
                const expiresAt = JSON.stringify((result.expiresIn * 1000) + new Date().getTime());
                sessionStorage.setItem('token', result.token);
                sessionStorage.setItem('expires_at', expiresAt);
                localStorage.setItem('refresh_token', result.refreshToken);
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
        sessionStorage.removeItem('token');
        sessionStorage.removeItem('expires_at');
        localStorage.removeItem('refresh_token');
        this.state.notifyDataChanged(this.state.events.global.logged, false);
        this.router.navigate(['/']);
    }

    refreshToken(): Observable<LoginResultDto> {
        return Observable.create((observer: Observer<LoginResultDto>) => {
            const refreshToken = localStorage.getItem("refresh_token");
            if (!refreshToken || refreshToken === 'undefined') {
                return Observable.create(() => {
                    observer.error('Refresh token not found');
                    observer.complete();
                });
            }
            return this.http.get<LoginResultDto>(apiSettings.baseUrl + this.urls.refreshToken + refreshToken).subscribe(result => {
                if (!result.token || !result.expiresIn) {
                    this.state.notifyDataChanged(this.state.events.global.logged, false);
                    observer.error('Not logged');
                    observer.complete();
                } else {
                    const expiresAt = JSON.stringify((result.expiresIn * 1000) + new Date().getTime());
                    sessionStorage.setItem('token', result.token);
                    sessionStorage.setItem('expires_at', expiresAt);
                    this.state.notifyDataChanged(this.state.events.global.logged, true);
                    observer.next(result);
                    observer.complete();   
                }
            },
                error => {
                    this.state.notifyDataChanged(this.state.events.global.logged, false);
                    observer.error(error);
                    observer.complete();
                });
        });
    }

    userExists(email: string): Observable<boolean> {
        return this.http.get<boolean>(apiSettings.baseUrl + this.urls.userExists + email);
    }

    forgotPassword(email: string): Observable<string> {
        return Observable.create((observer: Observer<string>) => {
            this.http.get(apiSettings.baseUrl + this.urls.forgotPassword + email, { responseType: 'text' }).subscribe(data => {
                observer.next(data);
                observer.complete();
            }, error => {
                observer.error(error);
                observer.complete();
            });
        });
    }

    isAuthenticated(): boolean {
        const token = sessionStorage.getItem('token');
        const expires = sessionStorage.getItem('expires_at');
        if (!expires || !token)
            return false;
        const expiresAt = JSON.parse(expires);
        return new Date().getTime() < expiresAt;
    }

    tryGetToken(): string | undefined {
        if (!this.isAuthenticated()) {
            if (localStorage.getItem('refresh_token')) {
                this.refreshToken().subscribe(() => {
                    return `Bearer ${sessionStorage.getItem('token')}`;
                }, () => {
                    return undefined;
                });
            }
            return undefined;
        }
        return `Bearer ${sessionStorage.getItem('token')}`;
    }
}