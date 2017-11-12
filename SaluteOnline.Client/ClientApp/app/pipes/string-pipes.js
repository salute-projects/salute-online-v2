var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { Pipe } from "@angular/core";
var TruncatePipe = /** @class */ (function () {
    function TruncatePipe() {
    }
    TruncatePipe.prototype.transform = function (value, limit, trail) {
        if (limit === void 0) { limit = 10; }
        if (trail === void 0) { trail = '...'; }
        return value.length > limit ? value.substring(0, limit) + trail : value;
    };
    TruncatePipe = __decorate([
        Pipe({
            name: 'trancate'
        })
    ], TruncatePipe);
    return TruncatePipe;
}());
export { TruncatePipe };
//# sourceMappingURL=string-pipes.js.map