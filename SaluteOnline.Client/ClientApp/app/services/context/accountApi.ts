import { HttpClient, HttpHeaders } from '@angular/common/http';
import { apiSettings } from "../../configuration/constants";
import { Observable } from 'rxjs';

export class LoginResultDto {
    token: string;
    expiresIn: number;
}

export class SignUpResultDto extends LoginResultDto {
    id: number;
}

export class AccountApi {
    urls: any = {
        userExists: '/account/userExists/',
        signUp: 'account/signUp/',
        login: 'account/login/'
    }

    constructor(private readonly http: HttpClient) { }

    userExists(email: string): Observable<boolean> {
        return this.http.get<boolean>(apiSettings.baseUrl + this.urls.userExists + email);
    }

    login(email: string, password: string): Observable<LoginResultDto> {
        const headers = new HttpHeaders().set('Access-Control-Allow-Origin', '*')
            .set('Access-Control-Allow-Methods', 'GET, POST, PATCH, PUT, DELETE, OPTIONS')
            .set('Access-Control-Allow-Headers', 'Origin, Content-Type, X-Auth-Token');
        const body = {
            email: email,
            password: password
        }
        return this.http.post<LoginResultDto>(apiSettings.baseUrl + this.urls.login, body, { headers: headers });
    }

    signUp(email: string, password: string): Observable<SignUpResultDto> {
        const body = {
            email: email,
            password: password
        }
        return this.http.post<SignUpResultDto>(apiSettings.baseUrl + this.urls.signUp, body);
    }
}