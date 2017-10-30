import { Injector, Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { apiSettings } from "../../configuration/constants";
import { AuthService } from "../../services/auth";
import { Observable, Observer } from 'rxjs';
import { UserDto, UserMainInfoDto, UserPersonalInfoDto } from "../../dto/dto";

@Injectable()
export class UserApi {
    urls: any = {
        getUserInfo: 'user',
        updateUserInfo: 'user',
        updateMainUserInfo: 'user',
        updatePersonalUserInfo: 'user',
        uploadAvatar: 'user/uploadAvatar'
    }

    constructor(private readonly http: HttpClient, private readonly auth: AuthService) {
    }

    getUserInfo(): Observable<UserDto> {
        const headers = this.auth.tryGetAuth();
        if (headers === undefined)
            return Observable.create((observer: Observer<UserDto>) => {
                observer.error("Not authenticated");
                observer.complete();
            });
        return this.http.get<UserDto>(apiSettings.baseUrl + this.urls.getUserInfo, { headers: headers });
    }

    updateUserInfo(dto: UserDto): Observable<any> {
        const headers = this.auth.tryGetAuth();
        if (headers === undefined)
            return Observable.create((observer: Observer<any>) => {
                observer.error("Not authenticated");
                observer.complete();
            });
        return this.http.post(apiSettings.baseUrl + this.urls.updateUserInfo, dto, { headers: headers, responseType: "text" });
    }

    updateMainUserInfo(dto: UserMainInfoDto): Observable<any> {
        const headers = this.auth.tryGetAuth();
        if (headers === undefined)
            return Observable.create((observer: Observer<any>) => {
                observer.error("Not authenticated");
                observer.complete();
            });
        return this.http.put(apiSettings.baseUrl + this.urls.updateMainUserInfo, dto, { headers: headers, responseType: "text" });
    }

    updatePersonalUserInfo(dto: UserPersonalInfoDto): Observable<any> {
        const headers = this.auth.tryGetAuth();
        if (headers === undefined)
            return Observable.create((observer: Observer<any>) => {
                observer.error("Not authenticated");
                observer.complete();
            });
        return this.http.patch(apiSettings.baseUrl + this.urls.updateMainUserInfo, dto, { headers: headers, responseType: "text" });
    }

    uploadAvatar(avatar: File): Observable<any> {
        const headers = this.auth.tryGetAuth();
        if (headers === undefined || headers === null)
            return Observable.create((observer: Observer<any>) => {
                observer.error("Not authenticated");
                observer.complete();
            });
        headers.append('Content-Type', 'multipart/form-data');
        const formData = new FormData();
        formData.append('avatar', avatar, avatar.name);
        return this.http.post(apiSettings.baseUrl + this.urls.uploadAvatar, formData, { headers: headers, responseType: 'text' });
    }
}