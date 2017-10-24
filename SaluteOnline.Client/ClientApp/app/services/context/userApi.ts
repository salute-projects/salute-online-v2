import { HttpClient, HttpHeaders } from '@angular/common/http';
import { apiSettings } from "../../configuration/constants";
import { Observable, Observer } from 'rxjs';

export class UserApi {
    urls: any = {
        getUserInfo: 'user'
    }

    constructor(private readonly http: HttpClient) { }

    getUserInfo(): Observable<any> {
        return this.http.get(apiSettings.baseUrl + this.urls.getUserInfo);
    }
}