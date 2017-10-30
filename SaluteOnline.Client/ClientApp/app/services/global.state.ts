import { Injectable } from "@angular/core";
import { Subject } from "rxjs/Subject";
import { BehaviorSubject } from "rxjs/BehaviorSubject";

@Injectable()
export class GlobalState {
    private currentState: Array<{ event: string, data: any }> = [];
    private data = new Subject<Object>();
    private dataStream = this.data.asObservable();
    private subscriptions = new Map<string, Array<Function>>();

    constructor() {
        this.dataStream.subscribe(data => {
            this.onEvent(data);
        });
    }

    notifyDataChanged(event: string, value: any) {
        const current = this.currentState.find(item => { return item.event === event; });
        if (!current || current.data !== value) {
            const newValue = {
                event: event,
                data: value
            }
            if (current) {
                const index = this.currentState.findIndex(item => { return item.event === event; });
                this.currentState[index] = newValue;
            } else {
                this.currentState.push(newValue);
            }
            this.data.next(newValue);
        }
    }

    subscribe(event: string, callback: Function) {
        const subscribers = this.subscriptions.get(event) || [];
        subscribers.push(callback);
        this.subscriptions.set(event, subscribers);
    }

    onEvent(data: any) {
        const subscribers = this.subscriptions.get(data['event']) || [];
        subscribers.forEach((callback) => {
            callback.call(null, data['data']);
        });
    }

    events = {
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
    }
}