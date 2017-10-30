import { Component, ViewEncapsulation } from '@angular/core';
import { GlobalState } from "../../services/global.state";
import { AuthService } from "../../services/auth";
import { SoSnackService } from "../../services/snack.service";

@Component({
    selector: 'app',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    providers: [ SoSnackService],
    encapsulation: ViewEncapsulation.None
})
export class AppComponent {
    isMenuCollapsed = false;

    constructor(private readonly state: GlobalState) {
        this.state.subscribe('menu.isCollapsed', (isCollapsed: boolean) => {
            this.isMenuCollapsed = isCollapsed;
        });
    }
}
