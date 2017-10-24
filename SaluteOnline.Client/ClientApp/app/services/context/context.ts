import { Injectable } from "@angular/core";
import { AccountApi } from "./accountApi";
import { UserApi } from "./userApi";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class Context {
    accountApi: AccountApi;
    userApi: UserApi;

    constructor(private readonly http: HttpClient) {
        this.accountApi = new AccountApi(this.http);
        this.userApi = new UserApi(this.http);
    }
}