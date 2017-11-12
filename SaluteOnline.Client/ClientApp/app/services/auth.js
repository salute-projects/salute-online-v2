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
import { Router } from "@angular/router";
import { GlobalState } from "../services/global.state";
import { Observable } from 'rxjs';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { apiSettings } from "../configuration/constants";
var AuthService = /** @class */ (function () {
    function AuthService(router, state, http) {
        this.router = router;
        this.state = state;
        this.http = http;
        this.urls = {
            userExists: '/account/userExists/',
            signUp: 'account/signUp/',
            login: 'account/login/',
            refreshToken: 'account/refreshToken/',
            forgotPassword: 'account/forgotPassword/'
        };
    }
    AuthService.prototype.login = function (email, password) {
        var _this = this;
        return Observable.create(function (observer) {
            var headers = new HttpHeaders().set('Access-Control-Allow-Origin', '*')
                .set('Access-Control-Allow-Methods', 'GET, POST, PATCH, PUT, DELETE, OPTIONS')
                .set('Access-Control-Allow-Headers', 'Origin, Content-Type, X-Auth-Token');
            var body = {
                email: email,
                password: password
            };
            return _this.http.post(apiSettings.baseUrl + _this.urls.login, body, { headers: headers }).subscribe(function (result) {
                var expiresAt = JSON.stringify((result.expiresIn * 1000) + new Date().getTime());
                sessionStorage.setItem('token', result.token);
                sessionStorage.setItem('expires_at', expiresAt);
                localStorage.setItem('refresh_token', result.refreshToken);
                _this.state.notifyDataChanged(_this.state.events.global.logged, true);
                localStorage.setItem('avatar', "data:image/jpg;base64," + result.avatar);
                _this.state.notifyDataChanged(_this.state.events.user.avatarChanged, '');
                observer.next(result);
                observer.complete();
            }, function (error) {
                _this.state.notifyDataChanged(_this.state.events.global.logged, false);
                observer.error(error);
                observer.complete();
            });
        });
    };
    AuthService.prototype.signUp = function (email, password) {
        var _this = this;
        return Observable.create(function (observer) {
            var body = {
                email: email,
                password: password
            };
            return _this.http.post(apiSettings.baseUrl + _this.urls.signUp, body).subscribe(function (result) {
                var expiresAt = JSON.stringify((result.expiresIn * 1000) + new Date().getTime());
                sessionStorage.setItem('token', result.token);
                sessionStorage.setItem('expires_at', expiresAt);
                localStorage.setItem('refresh_token', result.refreshToken);
                _this.state.notifyDataChanged(_this.state.events.global.logged, true);
                observer.next(result);
                observer.complete();
            }, function (error) {
                _this.state.notifyDataChanged(_this.state.events.global.logged, false);
                observer.error(error);
                observer.complete();
            });
        });
    };
    AuthService.prototype.logout = function () {
        sessionStorage.removeItem('token');
        sessionStorage.removeItem('expires_at');
        localStorage.removeItem('refresh_token');
        this.state.notifyDataChanged(this.state.events.global.logged, false);
        this.router.navigate(['/']);
    };
    AuthService.prototype.refreshToken = function () {
        var _this = this;
        return Observable.create(function (observer) {
            var refreshToken = localStorage.getItem("refresh_token");
            if (!refreshToken || refreshToken === 'undefined') {
                return Observable.create(function () {
                    observer.error('Refresh token not found');
                    observer.complete();
                });
            }
            return _this.http.get(apiSettings.baseUrl + _this.urls.refreshToken + refreshToken).subscribe(function (result) {
                if (!result.token || !result.expiresIn) {
                    _this.state.notifyDataChanged(_this.state.events.global.logged, false);
                    observer.error('Not logged');
                    observer.complete();
                }
                else {
                    var expiresAt = JSON.stringify((result.expiresIn * 1000) + new Date().getTime());
                    sessionStorage.setItem('token', result.token);
                    sessionStorage.setItem('expires_at', expiresAt);
                    _this.state.notifyDataChanged(_this.state.events.global.logged, true);
                    observer.next(result);
                    observer.complete();
                }
            }, function (error) {
                _this.state.notifyDataChanged(_this.state.events.global.logged, false);
                observer.error(error);
                observer.complete();
            });
        });
    };
    AuthService.prototype.userExists = function (email) {
        return this.http.get(apiSettings.baseUrl + this.urls.userExists + email);
    };
    AuthService.prototype.forgotPassword = function (email) {
        var _this = this;
        return Observable.create(function (observer) {
            _this.http.get(apiSettings.baseUrl + _this.urls.forgotPassword + email, { responseType: 'text' }).subscribe(function (data) {
                observer.next(data);
                observer.complete();
            }, function (error) {
                observer.error(error);
                observer.complete();
            });
        });
    };
    AuthService.prototype.isAuthenticated = function () {
        var token = sessionStorage.getItem('token');
        var expires = sessionStorage.getItem('expires_at');
        if (!expires || !token)
            return false;
        var expiresAt = JSON.parse(expires);
        return new Date().getTime() < expiresAt;
    };
    AuthService.prototype.tryGetToken = function () {
        if (!this.isAuthenticated()) {
            if (localStorage.getItem('refresh_token')) {
                this.refreshToken().subscribe(function () {
                    return "Bearer " + sessionStorage.getItem('token');
                }, function () {
                    return undefined;
                });
            }
            return undefined;
        }
        return "Bearer " + sessionStorage.getItem('token');
    };
    AuthService = __decorate([
        Injectable(),
        __metadata("design:paramtypes", [Router, GlobalState, HttpClient])
    ], AuthService);
    return AuthService;
}());
export { AuthService };
//# sourceMappingURL=auth.js.map