import { Component, ViewEncapsulation } from "@angular/core";
import { Context } from "../../services/context/context";
import { GlobalState } from "../../services/global.state";
import { SoSnackService } from "../../services/snack.service";
import { Observable } from 'rxjs/Observable';

@Component({
    selector: 'so-clubs-list',
    styleUrls: ['./so-clubs-list.component.scss'],
    templateUrl: './so-clubs-list.component.html',
    encapsulation: ViewEncapsulation.None
})

export class SoClubsList {
    constructor(private readonly context: Context, private readonly snackService: SoSnackService, private readonly state: GlobalState) {
        
    }
}