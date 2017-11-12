var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var UserDto = /** @class */ (function () {
    function UserDto() {
    }
    return UserDto;
}());
export { UserDto };
var UserMainInfoDto = /** @class */ (function () {
    function UserMainInfoDto(dto) {
        this.id = dto.id;
        this.firstName = dto.firstName;
        this.lastName = dto.lastName;
        this.nickname = dto.nickname;
        this.dateOfBirth = dto.dateOfBirth;
    }
    return UserMainInfoDto;
}());
export { UserMainInfoDto };
var UserPersonalInfoDto = /** @class */ (function () {
    function UserPersonalInfoDto(dto) {
        this.id = dto.id;
        this.phone = dto.phone;
        this.country = dto.country;
        this.city = dto.city;
        this.address = dto.address;
        this.alternativeEmail = dto.alternativeEmail;
        this.facebook = dto.facebook;
        this.twitter = dto.twitter;
        this.vk = dto.vk;
        this.instagram = dto.instagram;
        this.skype = dto.skype;
    }
    return UserPersonalInfoDto;
}());
export { UserPersonalInfoDto };
var LoginResultDto = /** @class */ (function () {
    function LoginResultDto() {
    }
    return LoginResultDto;
}());
export { LoginResultDto };
var SignUpResultDto = /** @class */ (function (_super) {
    __extends(SignUpResultDto, _super);
    function SignUpResultDto() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    return SignUpResultDto;
}(LoginResultDto));
export { SignUpResultDto };
//# sourceMappingURL=user.js.map