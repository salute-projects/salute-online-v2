import { Injectable, Injector } from "@angular/core";
import { UserApi } from "./userApi";
import { CommonApi } from "./commonApi";
import { ClubsApi } from "./clubsApi";
import { HttpClient } from '@angular/common/http';
import { AuthService } from "../../services/auth";

@Injectable()
export class Context {
    userApi: UserApi;
    commonApi: CommonApi;
    clubsApi: ClubsApi;

    constructor(private readonly userApiWrapper: UserApi, private readonly commonApiWrapper: CommonApi, private readonly clubsApiWrapper: ClubsApi) {
        this.userApi = userApiWrapper;
        this.commonApi = commonApiWrapper;
        this.clubsApi = clubsApiWrapper;
    }
}