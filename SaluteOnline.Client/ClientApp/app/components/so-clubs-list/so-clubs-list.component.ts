import { Component, ViewEncapsulation } from "@angular/core";
import { Router } from "@angular/router";
import { Context } from "../../services/context/context";
import { GlobalState } from "../../services/global.state";
import { SoSnackService } from "../../services/snack.service";
import { Observable } from 'rxjs/Observable';
import { MatDialog, MatDialogConfig } from "@angular/material";
import { CreateClubDialog } from "../so-create-club-dialog/so-create-club-dialog";
import { MembershipRequestDialog } from "../so-membership-request-dialog/so-membership-request-dialog";
import { Page, ClubDto, ClubFilter, ClubInfoAggregation, ClubSummaryDto } from "../../dto/dto";
import { TreeNode } from 'primeng/primeng';

@Component({
    selector: 'so-clubs-list',
    styleUrls: ['./so-clubs-list.component.scss'],
    templateUrl: './so-clubs-list.component.html',
    encapsulation: ViewEncapsulation.None
})

export class SoClubsList {
    geographyTreeNodes: TreeNode[];
    selectedCountry: TreeNode;
    clubInfoAggregation: ClubInfoAggregation;
    clubsFilter: ClubFilter;
    clubs: Page<ClubSummaryDto>;

    constructor(private readonly context: Context, private readonly snackService: SoSnackService, private readonly state: GlobalState,
        private readonly loginDialog: MatDialog, private readonly router: Router) {
        this.clubs = new Page<ClubSummaryDto>();
        this.clubsFilter = new ClubFilter();
        this.refreshClubsList();
    }

    private refreshClubsList() {
        this.context.clubsApi.getList(this.clubsFilter).subscribe((result: Page<ClubSummaryDto>) => {
            this.clubs = result;
        }, error => {
            this.snackService.showError(error.error, "OK");
            });
        this.context.clubsApi.getClubInfoAggregation().subscribe((result: ClubInfoAggregation) => {
            this.clubInfoAggregation = result;
            this.geographyTreeNodes = this.convertToTreeNodes(result.geography);
        }, error => {
            this.snackService.showError(error.error, "OK");
        });
    }

    createClub() : void {
        const config: MatDialogConfig = {
            width: '400px',
            panelClass: 'custom-dialog'
        };
        const dialogRef = this.loginDialog.open(CreateClubDialog, config);
        dialogRef.afterClosed().subscribe(result => {
            this.refreshClubsList();
        }, error => {
            this.snackService.showError(error.error, "OK");
        });
    }

    getClubAvatar(base64: string) {
        return `data:image/jpg;base64,${base64}`;
    }

    countrySelected(event: any) {
        const node = event.node;
        if (!node.parent) {
            this.clubsFilter.country = node.data || '';
            this.clubsFilter.city = '';
        } else {
            this.clubsFilter.city = node.data || '';
            this.clubsFilter.country = (node.parent ? node.parent.data : '') || '';
        }
        this.refreshClubsList();
    }

    private convertToTreeNodes(data: Map<string, Array<string>>) : TreeNode[] {
        if (!data || !data)
            return new Array<TreeNode>();
        const result = new Array<any>();
        data.forEach((item: any, key: string) => {
            result.push({
                label: item.key + " (" + item.value.length + ")",
                data: item.key,
                expanded: true,
                expandedIcon: 'fa-map-marker',
                collapsedIcon: 'fa-map-marker',
                children: item.value.map((subitem: string) => {
                    return {
                        label: subitem,
                        data: subitem,
                        expandedIcon: "fa-location-arrow",
                        collapsedIcon: "fa-location-arrow"
                    }
                })
            });
        });
        return result as TreeNode[];
    }

    edit(id: number) {
        this.router.navigate(['so-club-edit', id]);
    }

    sendMembershipRequest(id: number) {
        const config: MatDialogConfig = {
            width: '400px',
            panelClass: 'custom-dialog',
            data: { clubId: id }
        };
        const dialogRef = this.loginDialog.open(MembershipRequestDialog, config);
        dialogRef.afterClosed().subscribe(result => {
            debugger;
        }, error => {
            this.snackService.showError(error.error, "OK");
        });
    }
}