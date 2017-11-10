﻿import { Component, ViewEncapsulation, Output  } from "@angular/core";
import { ActivatedRoute  } from "@angular/router";
import { Context } from "../../services/context/context";
import { GlobalState } from "../../services/global.state";
import { SoSnackService } from "../../services/snack.service";
import { Observable } from 'rxjs/Observable';
import { MatDialog, MatDialogConfig, PageEvent } from "@angular/material";
import { ClubDto, Country, Page, ClubMemberSummary, ClubMemberFilter } from "../../dto/dto";
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { DataSource } from '@angular/cdk/collections';
import { CustomDataSource } from "../../services/datatable.service";
import { AddClubMemberDialog } from "../so-add-club-member/so-add-club-member";

@Component({
    selector: 'so-clubs-edit',
    styleUrls: ['./so-club-edit.scss'],
    templateUrl: './so-club-edit.html',
    encapsulation: ViewEncapsulation.None
})

export class SoClubsEdit {

    //#region Props

    id: number;
    clubInfo: ClubDto;
    allCountries: Country[];
    filteredCountries: Observable<Country[]>;
    countryInputValue: string;

    // admins tab
    clubAdminsDataSet: DataSource<ClubMemberSummary>;
    clubAdmins = new Page<ClubMemberSummary>();
    clubAdminsFilter: ClubMemberFilter;

    // members tab
    clubMembersDataSet: DataSource<ClubMemberSummary>;
    clubMembers = new Page<ClubMemberSummary>();
    clubMembersFilter: ClubMemberFilter;

    displayedColumns = [ 'nickname', 'firstName', 'lastName', 'registered', 'actions'];

    //#endregion

    constructor(private readonly context: Context, private readonly snackService: SoSnackService, private readonly state: GlobalState,
        private readonly addMemberDialog: MatDialog, private readonly route: ActivatedRoute, ) {
        this.clubInfo = new ClubDto();
        this.route.params.subscribe(params => {
            this.id = +params['id'];
            this.context.clubsApi.getClubInfo(this.id).subscribe(result => {
                this.clubInfo = result;
                this.clubInfo.logo = `data:image/jpg;base64,${this.clubInfo.logo}`;
                this.countryInputValue = result.country;
                this.clubAdminsFilter = new ClubMemberFilter(this.id);
                this.clubMembersFilter = new ClubMemberFilter(this.id);
            }, error => {
                this.snackService.showError(error.error, 'OK');
            });
            this.context.commonApi.getCountries().subscribe(result => {
                this.allCountries = result;
            });
        });
    }

    onAvatarChange(event: any) {
        
    }

    updateMainClubInfo() {
        
    }

    //#region Countries

    countryOnChange() {
        this.filteredCountries = Observable.of(this.countryInputValue ? this.filterCountries(this.countryInputValue) : this.allCountries.slice());
    }

    countrySelected(value: any) {
        this.clubInfo.country = this.countryInputValue;
    }

    private filterCountries(country: string) {
        return this.allCountries.filter(state => state.name.toLowerCase().indexOf(country.toLowerCase()) === 0);
    }

    //#endregion

    tabChanged(event: any) {
        switch (event.index) {
        case 1:
            this.getClubAdmins();
            break;
        case 2:
            this.getClubMembers();
            break;
        }
    }

    getClubAdmins() {
        this.context.clubsApi.getClubAdministrators(this.clubAdminsFilter).subscribe(result => {
            this.clubAdmins = result;
            this.clubAdminsDataSet = new CustomDataSource<ClubMemberSummary>(result.items);
        }, error => {
            this.snackService.showError(error.error, 'OK');
        });
    }

    getClubMembers() {
        this.context.clubsApi.getClubMembers(this.clubMembersFilter).subscribe(result => {
            this.clubMembers = result;
            this.clubMembersDataSet = new CustomDataSource<ClubMemberSummary>(result.items);
        }, error => {
            this.snackService.showError(error.error, 'OK');
        });
    }

    getLength<T>(page: Page<T>) {
        return page.items ? page.items.length : 0;
    }

    adminsPaginationEvent(event: any) {
        debugger;
    }

    usersPaginationEvent(event: any) {
        debugger;
    }

    addMember() {
        const config: MatDialogConfig = {
            width: '400px',
            panelClass: 'custom-dialog'
        };
        const dialogRef = this.addMemberDialog.open(AddClubMemberDialog, config);
        dialogRef.afterClosed().subscribe(result => {
            debugger;
        }, error => {
            this.snackService.showError(error.error, "OK");
        });
    }
}