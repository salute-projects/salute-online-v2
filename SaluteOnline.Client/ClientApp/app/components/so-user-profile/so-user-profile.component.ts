import { Component, ViewEncapsulation } from "@angular/core";
import { Context } from "../../services/context/context";

@Component({
    selector: 'so-user-profile',
    styleUrls: ['./so-user-profile.component.scss'],
    templateUrl: './so-user-profile.component.html',
    encapsulation: ViewEncapsulation.None
})

export class SoUserProfile {
    private logo = require('../../assets/logo.png');

    constructor(private readonly context: Context) {
        this.context.userApi.getUserInfo().subscribe(result => {
            debugger;
        }, error => {
            
        });
    }
}