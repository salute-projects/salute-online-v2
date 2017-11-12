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
var Page = /** @class */ (function () {
    function Page() {
    }
    return Page;
}());
export { Page };
var Country = /** @class */ (function () {
    function Country() {
    }
    return Country;
}());
export { Country };
var BaseFilter = /** @class */ (function () {
    function BaseFilter() {
    }
    return BaseFilter;
}());
export { BaseFilter };
var EntityFilter = /** @class */ (function (_super) {
    __extends(EntityFilter, _super);
    function EntityFilter(id) {
        var _this = _super.call(this) || this;
        _this.entityId = id;
        _this.asc = false;
        _this.searchBy = '';
        _this.page = 1;
        _this.pageSize = 25;
        _this.orderBy = "";
        return _this;
    }
    return EntityFilter;
}(BaseFilter));
export { EntityFilter };
//# sourceMappingURL=common.js.map