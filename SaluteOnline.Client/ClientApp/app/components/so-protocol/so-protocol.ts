import { Component, ViewEncapsulation } from "@angular/core";
import { Context } from "../../services/context/context";
import { SoSnackService } from "../../services/snack.service";
import { ClubSummaryDto, ServiceProps, Protocol, PlayerEntry, Roles, Role } from "../../dto/dto";
import { ActivatedRoute } from "@angular/router";
import { DataSource } from '@angular/cdk/collections';
import { CustomDataSource } from "../../services/datatable.service";
import { Observable } from 'rxjs/Observable';

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
    filteredNicknames: Observable<string[]>;
    allNicknames: string[];

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
                this.allNicknames = ['don', 'me', 'test'];
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

    nicknameSelected(player: PlayerEntry) {
        debugger;
    }

    nicknameOnChange(player: PlayerEntry) {
        this.filteredNicknames = Observable.of(player.nickname ? this.filterNicknames(player.nickname) : this.allNicknames.slice());
    }

    private filterNicknames(search: string) {
        return this.allNicknames.filter(nickname => nickname.toLowerCase().indexOf(search.toLowerCase()) === 0);
    }
}