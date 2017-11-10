import { Injector, Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { apiSettings } from "../../configuration/constants";
import { AuthService } from "../../services/auth";
import { Observable, Observer } from 'rxjs';
import { CreateClubDto, ClubFilter, Page, ClubDto, ClubSummaryDto, ClubInfoAggregation, ClubMemberFilter, ClubMemberSummary, CreateClubMemberDto } from "../../dto/dto";

@Injectable()
export class ClubsApi {
    urls: any = {
        createClub: 'clubs',
        getList: 'clubs/list',
        getClubInfoAggregation: 'clubs',
        getClubInfo: 'clubs/',
        getClubAdministrators: 'clubs/admins',
        getClubMembers: 'clubs/members',
        addClubMember: 'clubs/addClubMember'
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

    getList(dto: ClubFilter): Observable<Page<ClubSummaryDto>> {
        const headers = this.auth.tryGetAuth();
        if (headers === undefined)
            return Observable.create((observer: Observer<any>) => {
                observer.error("Not authenticated");
                observer.complete();
            });
        return this.http.post<Page<ClubSummaryDto>>(apiSettings.baseUrl + this.urls.getList, dto, { headers: headers});
    }

    getClubInfoAggregation(): Observable<ClubInfoAggregation> {
        const headers = this.auth.tryGetAuth();
        if (headers === undefined)
            return Observable.create((observer: Observer<any>) => {
                observer.error("Not authenticated");
                observer.complete();
            });
        return this.http.get<ClubInfoAggregation>(apiSettings.baseUrl + this.urls.getClubInfoAggregation, { headers: headers });
    }

    getClubInfo(id: number): Observable<ClubDto> {
        const headers = this.auth.tryGetAuth();
        if (headers === undefined)
            return Observable.create((observer: Observer<any>) => {
                observer.error("Not authenticated");
                observer.complete();
            });
        return this.http.get<ClubDto>(apiSettings.baseUrl + this.urls.getClubInfo + id, { headers: headers });
    }

    getClubAdministrators(filter: ClubMemberFilter): Observable<Page<ClubMemberSummary>> {
        const headers = this.auth.tryGetAuth();
        if (headers === undefined)
            return Observable.create((observer: Observer<any>) => {
                observer.error("Not authenticated");
                observer.complete();
            });
        return this.http.post<Page<ClubMemberSummary>>(apiSettings.baseUrl + this.urls.getClubAdministrators, filter, { headers: headers });
    }

    getClubMembers(filter: ClubMemberFilter): Observable<Page<ClubMemberSummary>> {
        const headers = this.auth.tryGetAuth();
        if (headers === undefined)
            return Observable.create((observer: Observer<any>) => {
                observer.error("Not authenticated");
                observer.complete();
            });
        return this.http.post<Page<ClubMemberSummary>>(apiSettings.baseUrl + this.urls.getClubMembers, filter, { headers: headers });
    }

    addClubMember(member: CreateClubMemberDto): Observable<string> {
        const headers = this.auth.tryGetAuth();
        if (headers === undefined)
            return Observable.create((observer: Observer<any>) => {
                observer.error("Not authenticated");
                observer.complete();
            });
        return this.http.post(apiSettings.baseUrl + this.urls.addClubMember, member, { headers: headers, responseType: "text" });
    }
}