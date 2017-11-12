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
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { apiSettings } from "../../configuration/constants";
var UserApi = /** @class */ (function () {
    function UserApi(http) {
        this.http = http;
        this.urls = {
            getUserInfo: 'user',
            updateUserInfo: 'user',
            updateMainUserInfo: 'user',
            updatePersonalUserInfo: 'user',
            uploadAvatar: 'user/uploadAvatar'
        };
    }
    UserApi.prototype.getUserInfo = function () {
        return this.http.get(apiSettings.baseUrl + this.urls.getUserInfo);
    };
    UserApi.prototype.updateUserInfo = function (dto) {
        return this.http.post(apiSettings.baseUrl + this.urls.updateUserInfo, dto, { responseType: "text" });
    };
    UserApi.prototype.updateMainUserInfo = function (dto) {
        return this.http.put(apiSettings.baseUrl + this.urls.updateMainUserInfo, dto, { responseType: "text" });
    };
    UserApi.prototype.updatePersonalUserInfo = function (dto) {
        return this.http.patch(apiSettings.baseUrl + this.urls.updateMainUserInfo, dto, { responseType: "text" });
    };
    UserApi.prototype.uploadAvatar = function (avatar) {
        var headers = new HttpHeaders();
        headers.append('Content-Type', 'multipart/form-data');
        var formData = new FormData();
        formData.append('avatar', avatar, avatar.name);
        return this.http.post(apiSettings.baseUrl + this.urls.uploadAvatar, formData, { headers: headers, responseType: 'text' });
    };
    UserApi = __decorate([
        Injectable(),
        __metadata("design:paramtypes", [HttpClient])
    ], UserApi);
    return UserApi;
}());
export { UserApi };
//# sourceMappingURL=userApi.js.map