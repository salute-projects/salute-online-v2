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
import { BaseFilter, EntityFilter } from './common';
export var ClubStatus;
(function (ClubStatus) {
    ClubStatus[ClubStatus["None"] = 0] = "None";
    ClubStatus[ClubStatus["Active"] = 1] = "Active";
    ClubStatus[ClubStatus["PendingActivation"] = 2] = "PendingActivation";
    ClubStatus[ClubStatus["Blocked"] = 3] = "Blocked";
    ClubStatus[ClubStatus["Deleted"] = 4] = "Deleted";
    ClubStatus[ClubStatus["ActiveAndPending"] = 5] = "ActiveAndPending";
})(ClubStatus || (ClubStatus = {}));
var ClubDto = /** @class */ (function () {
    function ClubDto() {
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
    return ClubDto;
}());
export { ClubDto };
var ClubSummaryDto = /** @class */ (function () {
    function ClubSummaryDto() {
    }
    return ClubSummaryDto;
}());
export { ClubSummaryDto };
var CreateClubDto = /** @class */ (function () {
    function CreateClubDto() {
        this.title = '';
        this.country = '';
        this.city = '';
        this.description = '';
    }
    return CreateClubDto;
}());
export { CreateClubDto };
var ClubFilter = /** @class */ (function (_super) {
    __extends(ClubFilter, _super);
    function ClubFilter() {
        var _this = _super.call(this) || this;
        _this.title = "";
        _this.asc = false;
        _this.city = "";
        _this.isFiim = null;
        _this.isActive = true;
        _this.creatorId = null;
        _this.status = ClubStatus.ActiveAndPending;
        _this.page = 1;
        _this.pageSize = 25;
        _this.orderBy = "";
        return _this;
    }
    return ClubFilter;
}(BaseFilter));
export { ClubFilter };
var ClubMemberFilter = /** @class */ (function (_super) {
    __extends(ClubMemberFilter, _super);
    function ClubMemberFilter(clubId) {
        var _this = _super.call(this) || this;
        _this.pageSize = 25;
        _this.asc = false;
        _this.clubId = clubId;
        _this.page = 1;
        _this.search = '';
        return _this;
    }
    return ClubMemberFilter;
}(BaseFilter));
export { ClubMemberFilter };
var ClubInfoAggregation = /** @class */ (function () {
    function ClubInfoAggregation() {
    }
    return ClubInfoAggregation;
}());
export { ClubInfoAggregation };
var ClubMemberSummary = /** @class */ (function () {
    function ClubMemberSummary() {
    }
    return ClubMemberSummary;
}());
export { ClubMemberSummary };
var CreateClubMemberDto = /** @class */ (function () {
    function CreateClubMemberDto() {
    }
    return CreateClubMemberDto;
}());
export { CreateClubMemberDto };
var MembershipRequestCreateDto = /** @class */ (function () {
    function MembershipRequestCreateDto() {
    }
    return MembershipRequestCreateDto;
}());
export { MembershipRequestCreateDto };
var MembershipRequestDto = /** @class */ (function () {
    function MembershipRequestDto() {
    }
    return MembershipRequestDto;
}());
export { MembershipRequestDto };
export var MembershipRequestStatus;
(function (MembershipRequestStatus) {
    MembershipRequestStatus[MembershipRequestStatus["None"] = 0] = "None";
    MembershipRequestStatus[MembershipRequestStatus["Pending"] = 1] = "Pending";
    MembershipRequestStatus[MembershipRequestStatus["Accepted"] = 2] = "Accepted";
    MembershipRequestStatus[MembershipRequestStatus["Declined"] = 3] = "Declined";
})(MembershipRequestStatus || (MembershipRequestStatus = {}));
var HandleMembershipRequestDto = /** @class */ (function () {
    function HandleMembershipRequestDto() {
    }
    return HandleMembershipRequestDto;
}());
export { HandleMembershipRequestDto };
var MembershipRequestFilter = /** @class */ (function (_super) {
    __extends(MembershipRequestFilter, _super);
    function MembershipRequestFilter(id) {
        var _this = _super.call(this, id) || this;
        _this.entityId = id;
        _this.asc = false;
        _this.searchBy = '';
        _this.page = 1;
        _this.pageSize = 25;
        _this.orderBy = "";
        _this.status = MembershipRequestStatus.Pending;
        return _this;
    }
    return MembershipRequestFilter;
}(EntityFilter));
export { MembershipRequestFilter };
//# sourceMappingURL=club.js.map