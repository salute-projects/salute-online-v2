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
    orderBy: string;
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
    constructor() {
        this.id = -1;
        this.title = '';
        this.country = '';
        this.registered = '';
        this.lastUpdate = '';
        this.isFiim = false;
        this.isActive = false;
        this.creatorId = -1;
        this.status = ClubStatus.None;
        this.logo = '';
        this.canBeEdited = false;
    }

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
    canBeEdited: boolean;
}

export class ClubSummaryDto {
    id: number;
    title: string;
    country: string;
    city: string;
    description: string;
    logo: string;
    canBeEdited: boolean;
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
        this.orderBy = "";
    }

    title: string | null;
    country: string | null;
    city: string | null;
    isFiim: boolean | null;
    isActive: boolean | null;
    creatorId: number | null;
    status: ClubStatus;
}

export class ClubMemberFilter extends BaseFilter {
    constructor(clubId: number) {
        super();
        this.pageSize = 25;
        this.asc = false;
        this.clubId = clubId;
        this.page = 1;
        this.search = '';
    }

    search: string;
    clubId: number;
}

export class ClubInfoAggregation {
    count: number;
    isFiim: number;
    byStatus: Map<ClubStatus, number>;
    geography: Map<string, Array<string>>;
}

export class ClubMemberSummary {
    playerId: number | null;
    userId: number | null;
    firstName: string;
    lastName: string;
    email: string;
    country: string;
    city: string;
    isActive: boolean;
    nickname: string;
    registered: Date;
    avatar: string;
}

export class CreateClubMemberDto {
    nickname: string;
}