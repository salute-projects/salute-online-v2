import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
import { CounterComponent } from './components/counter/counter.component';
import { SoSidebar } from "./components/so-sidebar/so-sidebar.component";
import { SoHeader, LoginDialog } from "./components/so-header/so-header.component";
import { SoMenu } from "./components/so-menu/so-menu.component";
import { SoMenuItem } from "./components/so-menu-item/so-menu-item.component";
import { SoCallback } from "./components/callback/callback.component";

import { provideAuthService } from 'auth0-angular2';

import { BrowserAnimationsModule } from "@angular/platform-browser/animations";

import { MatButtonModule, MatIconModule, MatMenuModule, MatDialogModule, MatTabsModule, MatInputModule, MatFormFieldModule, MATERIAL_SANITY_CHECKS } from "@angular/material";

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
        SoCallback
    ],
    entryComponents: [LoginDialog],
    providers: [
        provideAuthService('3s-MDoybe9b6jDbMq6jvZAhMsRFhHTE7', 'saluteonline.eu.auth0.com', '/home', '/callback'), {
            provide: MATERIAL_SANITY_CHECKS,
            useValue: false
    }],
    imports: [
        MatMenuModule,
        MatIconModule,
        MatButtonModule,
        MatDialogModule,
        MatTabsModule,
        MatInputModule,
        MatFormFieldModule,
        CommonModule,
        HttpModule,
        FormsModule,
        BrowserAnimationsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'counter', component: CounterComponent },
            { path: 'fetch-data', component: FetchDataComponent },
            { path: 'callback', component: SoCallback },
            { path: '**', redirectTo: 'home' }
        ])
    ]
})
export class AppModuleShared {
}
