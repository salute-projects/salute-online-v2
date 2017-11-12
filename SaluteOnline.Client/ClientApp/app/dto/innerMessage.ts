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
}

export class InnerMessagesFilter {
    constructor(type: EntityType, id: number | null) {
        this.receiverType = type;
        this.receiverId = id;
    }

    receiverId: number | null;
    receiverType: EntityType;
}

export enum MessageStatus {
    None = 0,
    Pending = 1,
    Closed = 2
}

export enum EntityType {
    System = 0,
    User = 1,
    Club = 2
}