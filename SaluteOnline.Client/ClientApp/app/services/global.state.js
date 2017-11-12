var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Injectable } from "@angular/core";
import { Subject } from "rxjs/Subject";
var GlobalState = /** @class */ (function () {
    function GlobalState() {
        var _this = this;
        this.currentState = [];
        this.data = new Subject();
        this.dataStream = this.data.asObservable();
        this.subscriptions = new Map();
        this.events = {
            global: {
                logged: "global.logged"
            },
            menu: {
                isCollapsed: "menu.isCollapsed",
                activeLink: "menu.activeLink"
            },
            user: {
                avatarChanged: "user.avatarChanged"
            }
        };
        this.dataStream.subscribe(function (data) {
            _this.onEvent(data);
        });
    }
    GlobalState.prototype.notifyDataChanged = function (event, value) {
        var current = this.currentState.find(function (item) { return item.event === event; });
        if (!current || current.data !== value) {
            var newValue = {
                event: event,
                data: value
            };
            if (current) {
                var index = this.currentState.findIndex(function (item) { return item.event === event; });
                this.currentState[index] = newValue;
            }
            else {
                this.currentState.push(newValue);
            }
            this.data.next(newValue);
        }
    };
    GlobalState.prototype.subscribe = function (event, callback) {
        var subscribers = this.subscriptions.get(event) || [];
        subscribers.push(callback);
        this.subscriptions.set(event, subscribers);
    };
    GlobalState.prototype.onEvent = function (data) {
        var subscribers = this.subscriptions.get(data['event']) || [];
        subscribers.forEach(function (callback) {
            callback.call(null, data['data']);
        });
    };
    GlobalState = __decorate([
        Injectable(),
        __metadata("design:paramtypes", [])
    ], GlobalState);
    return GlobalState;
}());
export { GlobalState };
//# sourceMappingURL=global.state.js.map