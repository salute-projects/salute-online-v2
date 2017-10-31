// system
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { AppComponent } from './components/app/app.component';
import { CanActivate } from '@angular/router';
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";

// third party

import {
    MatButtonModule, MatIconModule, MatMenuModule, MatDialogModule, MatTabsModule, MatInputModule, MatFormFieldModule, MatSnackBarModule, MatCardModule, MatDatepickerModule,
    MatNativeDateModule, MatGridListModule, MatAutocompleteModule, MatExpansionModule, MATERIAL_SANITY_CHECKS, MAT_DATE_LOCALE, MAT_NATIVE_DATE_FORMATS, MAT_DATE_FORMATS
} from "@angular/material";


// old

import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
import { CounterComponent } from './components/counter/counter.component';

// components

import { SoSidebar } from "./components/so-sidebar/so-sidebar.component";
import { SoHeader } from "./components/so-header/so-header.component";
import { LoginDialog } from "./components/so-login-dialog/so-login-dialog";
import { SoMenu } from "./components/so-menu/so-menu.component";
import { SoMenuItem } from "./components/so-menu-item/so-menu-item.component";
import { SoUserProfile } from "./components/so-user-profile/so-user-profile.component";
import { SoClubsList } from "./components/so-clubs-list/so-clubs-list.component";
import { CreateClubDialog } from "./components/so-create-club-dialog/so-create-club-dialog";

// services

import { AuthGuard } from "./services/authGuard";
import { GlobalState } from "./services/global.state";
import { AuthService } from "./services/auth";
import { Helpers } from "./services/helpers";

// context


import { ClubsApi } from "./services/context/clubsApi";
import { CommonApi } from "./services/context/commonApi";
import { UserApi } from "./services/context/userApi";
import { Context } from "./services/context/context";

@
NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        CounterComponent,
        FetchDataComponent,
        HomeComponent,
        SoSidebar,
        LoginDialog,
        SoHeader,
        SoMenu,
        SoMenuItem,
        SoUserProfile,
        SoClubsList,
        CreateClubDialog
    ],
    entryComponents: [LoginDialog, CreateClubDialog],
    providers: [ 
        Helpers,
        GlobalState,
        AuthService,
        CommonApi,
        UserApi,
        ClubsApi,
        Context,
        { provide: MAT_DATE_LOCALE, useValue: 'en-GB' },
        { provide: MAT_DATE_FORMATS, useValue: MAT_NATIVE_DATE_FORMATS }
    ],
    imports: [
        FormsModule,
        ReactiveFormsModule,
        MatMenuModule,
        MatIconModule,
        MatButtonModule,
        MatDialogModule,
        MatTabsModule,
        MatInputModule,
        MatFormFieldModule,
        MatSnackBarModule,
        MatCardModule,
        MatDatepickerModule,
        MatNativeDateModule,
        MatGridListModule,
        MatAutocompleteModule,
        MatExpansionModule,
        CommonModule,
        HttpModule,
        FormsModule,
        BrowserAnimationsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'counter', component: CounterComponent },
            { path: 'fetch-data', component: FetchDataComponent },
            { path: 'so-user-profile', component: SoUserProfile },
            { path: 'so-clubs-list', component: SoClubsList },
            { path: '**', redirectTo: 'home' }
        ])
    ]
})
export class AppModuleShared {
}
