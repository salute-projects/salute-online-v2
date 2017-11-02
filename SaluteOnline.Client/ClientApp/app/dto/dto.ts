// User controller

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

// common

export class Page<TEntity> {
    currentPage: number;
    pageSize: number;
    total: number;
    totalPages: number;
    totalItems: number;
    items: Array<TEntity>;
}

export class Country {
    name: string;
    code: string;
}

export class BaseFilter {
    pageSize: number | null;
    page: number;
    asc: boolean;
}

// clubs
export enum ClubStatus {
    None = 0,
    Active = 1,
    PendingActivation = 2,
    Blocked = 3,
    Deleted = 4,
    ActiveAndPending = 5
}

export class ClubDto {
    id: number;
    title: string;
    country: string;
    description: string;
    registered: string;
    lastUpdate: string;
    isFiim: boolean | null;
    isActive: boolean | null;
    creatorId: number;
    status: ClubStatus;
    logo: string;
}

export class CreateClubDto {
    constructor() {
        this.title = '';
        this.country = '';
        this.city = '';
        this.description = '';
    }

    title: string;
    country: string;
    city: string;
    description: string;
}

export class ClubFilter extends BaseFilter {

    constructor() {
        super();
        this.title = "";
        this.asc = false;
        this.city = "";
        this.isFiim = null;
        this.isActive = true;
        this.creatorId = null;
        this.status = ClubStatus.ActiveAndPending;
        this.page = 1;
        this.pageSize = 25;
    }

    title: string | null;
    country: string | null;
    city: string | null;
    isFiim: boolean | null;
    isActive: boolean | null;
    creatorId: number | null;
    status: ClubStatus;
}