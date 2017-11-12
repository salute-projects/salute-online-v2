var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { apiSettings } from "../../configuration/constants";
var ClubsApi = /** @class */ (function () {
    function ClubsApi(http) {
        this.http = http;
        this.urls = {
            createClub: 'clubs',
            getList: 'clubs/list',
            getClubInfoAggregation: 'clubs',
            getClubInfo: 'clubs/',
            getClubAdministrators: 'clubs/admins',
            getClubMembers: 'clubs/members',
            addClubMember: 'clubs/addClubMember',
            addMembershipRequest: 'clubs/addMembershipRequest',
            getMembershipRequests: 'clubs/getMembershipRequests',
            handleMembershipRequest: 'clubs/handleMembershipRequest'
        };
    }
    ClubsApi.prototype.createClub = function (dto) {
        return this.http.post(apiSettings.baseUrl + this.urls.createClub, dto, { responseType: "text" });
    };
    ClubsApi.prototype.getList = function (dto) {
        return this.http.post(apiSettings.baseUrl + this.urls.getList, dto);
    };
    ClubsApi.prototype.getClubInfoAggregation = function () {
        return this.http.get(apiSettings.baseUrl + this.urls.getClubInfoAggregation);
    };
    ClubsApi.prototype.getClubInfo = function (id) {
        return this.http.get(apiSettings.baseUrl + this.urls.getClubInfo + id);
    };
    ClubsApi.prototype.getClubAdministrators = function (filter) {
        return this.http.post(apiSettings.baseUrl + this.urls.getClubAdministrators, filter);
    };
    ClubsApi.prototype.getClubMembers = function (filter) {
        return this.http.post(apiSettings.baseUrl + this.urls.getClubMembers, filter);
    };
    ClubsApi.prototype.addClubMember = function (member) {
        return this.http.post(apiSettings.baseUrl + this.urls.addClubMember, member, { responseType: "text" });
    };
    ClubsApi.prototype.addMembershipRequest = function (dto) {
        return this.http.post(apiSettings.baseUrl + this.urls.addMembershipRequest, dto, { responseType: "text" });
    };
    ClubsApi.prototype.getMembershipRequests = function (filter) {
        return this.http.post(apiSettings.baseUrl + this.urls.getMembershipRequests, filter);
    };
    ClubsApi.prototype.handleMembershipRequest = function (dto) {
        return this.http.post(apiSettings.baseUrl + this.urls.handleMembershipRequest, dto, { responseType: "text" });
    };
    ClubsApi = __decorate([
        Injectable(),
        __metadata("design:paramtypes", [HttpClient])
    ], ClubsApi);
    return ClubsApi;
}());
export { ClubsApi };
//# sourceMappingURL=clubsApi.js.map