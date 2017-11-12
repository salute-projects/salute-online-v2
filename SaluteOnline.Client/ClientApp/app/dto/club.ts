import { BaseFilter, EntityFilter } from './common';
import { UserMainInfoDto } from "./user";

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
    clubId: number;
}

export class MembershipRequestCreateDto {
    clubId: number;
    nickname: string;
    selectedFromExisting: boolean;
}

export class MembershipRequestDto {
    id: number;
    nickname: string;
    selectedFromExisting: boolean;
    created: Date;
    lastActivity: Date;
    status: MembershipRequestStatus;
    userInfo: UserMainInfoDto;
}

export enum MembershipRequestStatus {
    None = 0,
    Pending = 1,
    Accepted = 2,
    Declined = 3
}

export class HandleMembershipRequestDto {
    clubId: number;
    requestId: number;
    status: MembershipRequestStatus;
}

export class MembershipRequestFilter extends EntityFilter {
    constructor(id: number) {
        super(id);
        this.entityId = id;
        this.asc = false;
        this.searchBy = '';
        this.page = 1;
        this.pageSize = 25;
        this.orderBy = "";
        this.status = MembershipRequestStatus.Pending;
    }

    entityId: number | null;
    searchBy: string;
    status: MembershipRequestStatus;
}