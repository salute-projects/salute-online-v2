import { Injectable, Injector } from "@angular/core";
import { AccountApi } from "./accountApi";
import { UserApi } from "./userApi";
import { HttpClient } from '@angular/common/http';
import { TokenService } from "../../services/token.service";

@Injectable()
export class Context {
    accountApi: AccountApi;
    userApi: UserApi;

    constructor(private readonly http: HttpClient, private readonly tokenService: TokenService) {
        this.accountApi = new AccountApi(this.http);
        this.userApi = new UserApi(this.http, this.tokenService);
    }
}