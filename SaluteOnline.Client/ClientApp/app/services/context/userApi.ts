import { Injector } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { apiSettings } from "../../configuration/constants";
import { TokenService } from "../../services/token.service";
import { Observable, Observer } from 'rxjs';

export class UserDto {
    id: number; 
    firstName: string;
    lastName: string;
    email: string;
    phone: string;
    dateOfBirth: string;
    country: string;
    city: string;
    address: string;
    alternativeEmail: string;
    facebook: string;
    twitter: string;
    vk: string;
    instagram: string;
    skype: string;
    isActive: boolean;
    role: number;
    registered: string;
    lastActivity: string;
}

export class UserApi {

    urls: any = {
        getUserInfo: 'user'
    }

    constructor(private readonly http: HttpClient, private readonly tokenService: TokenService) {
    }

    getUserInfo(): Observable<UserDto> {
        debugger;
        const headers = this.tokenService.tryGetAuth();
        if (headers === undefined)
            return Observable.create((observer: Observer<UserDto>) => {
                observer.error("Not authenticated");
                observer.complete();
            });
        return this.http.get<UserDto>(apiSettings.baseUrl + this.urls.getUserInfo, { headers: headers });
    }
}