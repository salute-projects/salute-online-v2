import { Component, ViewEncapsulation } from "@angular/core";
import { Context } from "../../services/context/context";
import { SoSnackService } from "../../services/snack.service";
import { ClubSummaryDto, ServiceProps, Protocol, PlayerEntry, Roles, Role } from "../../dto/dto";
import { ActivatedRoute } from "@angular/router";
import { DataSource } from '@angular/cdk/collections';
import { CustomDataSource } from "../../services/datatable.service";

@Component({
    selector: 'so-protocol',
    templateUrl: './so-protocol.html',
    styleUrls: ['./so-protocol.scss'],
    encapsulation: ViewEncapsulation.None
})

export class SoProtocol {
    myClubs: ClubSummaryDto[] = [];
    selectedClub: number;
    settings: ServiceProps;
    protocol: Protocol;

    constructor(private readonly context: Context, private readonly snackService: SoSnackService, private readonly route: ActivatedRoute) {
        this.route.params.subscribe(params => {
            const targetId = +params['id'];
            this.context.clubsApi.getMyClubs().subscribe(result => {
                this.myClubs = result;
                if (result.some(t => t.id === targetId)) {
                    this.selectedClub = targetId;
                } else {
                    this.selectedClub = 0;
                }
            }, error => {
                this.snackService.showError(error.error, 'OK');
            });
        });
        this.initialSet();
    }

    private initialSet() {
        this.protocol = new Protocol();
        for (let i = 0; i < 10; i++) {
            this.protocol.players.push(new PlayerEntry(i));
        }
    }
}