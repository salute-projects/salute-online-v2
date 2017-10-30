import { Injector, Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { apiSettings } from "../../configuration/constants";
import { AuthService } from "../../services/auth";
import { Observable, Observer } from 'rxjs';
import { CreateClubDto } from "../../dto/dto";

@Injectable()
export class ClubsApi {
    urls: any = {
        createClub: 'clubs'
    }

    constructor(private readonly http: HttpClient, private readonly auth: AuthService) {
    }

    createClub(dto: CreateClubDto): Observable<string> {
        const headers = this.auth.tryGetAuth();
        if (headers === undefined)
            return Observable.create((observer: Observer<any>) => {
                observer.error("Not authenticated");
                observer.complete();
            });
        return this.http.post(apiSettings.baseUrl + this.urls.createClub, dto, { headers: headers, responseType: "text" });
    }
}