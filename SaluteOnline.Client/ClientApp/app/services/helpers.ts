import { Injectable } from "@angular/core";
import { Subject } from "rxjs/Subject";
import { BehaviorSubject } from "rxjs/BehaviorSubject";
import { FormGroup } from "@angular/forms";

@Injectable()
export class Helpers {
    formToObject(form: FormGroup, target: any): any {
        for (let key in target) {
            if (target.hasOwnProperty(key)) {
                const control = form.get(key);
                target[key] = control == null ? null : control.value;
            }
        }
        return target;
    }
}