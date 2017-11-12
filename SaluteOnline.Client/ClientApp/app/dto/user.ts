export class UserDto {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    phone: string;
    dateOfBirth: string;
    country: string;
    city: string;
    address: string;
    alternativeEmail: string;
    facebook: string;
    twitter: string;
    vk: string;
    instagram: string;
    skype: string;
    isActive: boolean;
    role: number;
    registered: string;
    lastActivity: string;
    nickname: string;
    avatar: string;
}

export class UserMainInfoDto {

    constructor(dto: UserDto) {
        this.id = dto.id;
        this.firstName = dto.firstName;
        this.lastName = dto.lastName;
        this.nickname = dto.nickname;
        this.dateOfBirth = dto.dateOfBirth;
    }

    id: number;
    firstName: string;
    lastName: string;
    dateOfBirth: string;
    nickname: string;
}

export class UserPersonalInfoDto {

    constructor(dto: UserDto) {
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

    id: number;
    phone: string;
    country: string;
    city: string;
    address: string;
    alternativeEmail: string;
    facebook: string;
    twitter: string;
    vk: string;
    instagram: string;
    skype: string;
}

export class LoginResultDto {
    token: string;
    expiresIn: number;
    refreshToken: string;
    avatar: string;
}

export class SignUpResultDto extends LoginResultDto {
    id: number;
}