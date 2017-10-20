import { Injectable } from "@angular/core";
import { AccountApi } from "./accountApi";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class Context {
    accountApi: AccountApi;

    constructor(private readonly http: HttpClient) {
        this.accountApi = new AccountApi(this.http);
    }
}