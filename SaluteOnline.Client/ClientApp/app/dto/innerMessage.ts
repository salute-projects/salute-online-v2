import { BaseFilter } from "./common";

export class InnerMessageDto {
    id: number;
    senderId: number | null;
    senderType: EntityType;
    receiverId: number | null;
    receiver: EntityType;
    status: MessageStatus;
    title: string;
    body: string;
    created: Date;
    lastActivity: Date;
    sentBySystem: boolean;
    oneResponseForAll: boolean;
    avatar: string;
    senderName: string;
}

export class InnerMessagesFilter extends BaseFilter {
    constructor(type: EntityType, id: number | null, status: MessageStatus) {
        super();
        this.receiverType = type;
        this.receiverId = id;
        this.status = status;
    }

    receiverId: number | null;
    receiverType: EntityType;
    status: MessageStatus;
}

export enum MessageStatus {
    None = 0,
    Pending = 1,
    Read = 2,
    Closed = 3,
    All = 4
}

export enum EntityType {
    System = 0,
    User = 1,
    Club = 2
}