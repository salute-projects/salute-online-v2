export enum Teams {
    None = 0,
    Red = 1,
    Black = 2
}

export enum Roles {
    Sheriff = 1,
    Don = 2,
    Mafia = 3,
    Red = 4
}

export enum BestPlayers {
    None = 0,
    Best1 = 1,
    Best2 = 2,
    Best3 = 3
}

export class Role {
    constructor(role: Roles, label: string) {
        this.role = role;
        this.label = label;
    }
    role: Roles;
    label: string;
}

export class BestPlayer {
    constructor(value: BestPlayers, label: string, enabled: boolean) {
        this.value = value;
        this.label = label;
        this.enabled = enabled;
    }
    value: BestPlayers;
    label: string;
    enabled: boolean;
}

export class PlayerEntry {
    constructor(index: number) {
        this.index = index;
        this.nickname = '';
        this.role = null;
        this.foul = null;
        this.bestPlayer = null;
        this.result = null;
        this.mainScore = null;
        this.additionalScore = null;
        this.positionInKillQueue = null;
        this.killedAtDay = false;
        this.killedAtNight = false;
        this.checkedAtNight = false;
        this.halfBestWay = false;
        this.fullBestWay = false;
        this.rolesAvailable = [new Role(Roles.Sheriff, Roles[1]), new Role(Roles.Don, Roles[2]), new Role(Roles.Mafia, Roles[3]), new Role(Roles.Red, Roles[4])];
        this.bestPlayersAvailable = [new BestPlayer(BestPlayers.None, '', true), new BestPlayer(BestPlayers.Best1, 'Best 1', true),
            new BestPlayer(BestPlayers.Best2, 'Best 2', true), new BestPlayer(BestPlayers.Best3, 'Best 3', true)];
    }

    index: number | null;
    nickname: string;
    role: Role | null;
    foul: number | null;
    bestPlayer: BestPlayer | null;
    result: number | null;
    mainScore: number | null;
    additionalScore: number | null;
    positionInKillQueue: number | null;
    killedAtDay: boolean;
    killedAtNight: boolean;
    checkedAtNight: boolean;
    halfBestWay: boolean;
    fullBestWay: boolean;
    rolesAvailable: Role[];
    bestPlayersAvailable: BestPlayer[];
}

export class Protocol {
    constructor() {
        this.winner = Teams.None;
        this.game = null;
        this.table = null;
        this.killedAtDay = [];
        this.killedAtNight = [];
        this.bestWay = [];
        this.donCheck = null;
        this.sheriffCheck = null;
        this.threeCheck = null;
        this.techRed = false;
        this.techBlack = false;
        this.ugadayka = [];
        this.ugadaykaEnabled = false;
        this.falseSheriff = null;
        this.sheriffFirstKilled = false;
        this.players = [];
    }

    winner: Teams;
    game: number | null;
    table: number | null;
    killedAtDay: number[];
    killedAtNight: number[];
    bestWay: number[];
    donCheck: number | null;
    sheriffCheck: number | null;
    threeCheck: number | null;
    techRed: boolean;
    techBlack: boolean;
    ugadayka: number[];
    ugadaykaEnabled: boolean;
    falseSheriff: number | null;
    sheriffFirstKilled: boolean;
    players: PlayerEntry[];
}

export class ServiceProps {
    constructor() {
        this.night = true;
        this.notOnVote = Array.apply(null, { length: 10 }).map((value: any, index: number) => index + 1);
        this.onVote = [];
        this.killQueue = 1;
        this.miskills = 0;
        this.canFillRedRoles = false;
        this.canClearRoles = false;
        this.rolesValid = false;
        this.nicksValid = false;
        this.checkVisibility = false;
        this.checkSuccess = null;
        this.checkTypeIsDon = null;
        this.currentCheckIndex = null;
    }

    night: boolean;
    onVote: number[];
    notOnVote: number[];
    killQueue: number;
    miskills: number;
    canFillRedRoles: boolean;
    canClearRoles: boolean;
    rolesValid: boolean;
    nicksValid: boolean;
    checkVisibility: boolean;
    checkSuccess: boolean | null;
    checkTypeIsDon: boolean | null;
    currentCheckIndex: number | null;
}