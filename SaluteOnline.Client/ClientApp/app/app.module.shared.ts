import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
import { CounterComponent } from './components/counter/counter.component';
import { SoSidebar } from "./components/so-sidebar/so-sidebar.component";
import { SoHeader } from "./components/so-header/so-header.component";
import { LoginDialog } from "./components/so-login-dialog/so-login-dialog";
import { SoMenu } from "./components/so-menu/so-menu.component";
import { SoMenuItem } from "./components/so-menu-item/so-menu-item.component";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { SoUserProfile } from "./components/so-user-profile/so-user-profile.component";
import { MatButtonModule, MatIconModule, MatMenuModule, MatDialogModule, MatTabsModule, MatInputModule, MatFormFieldModule, MatSnackBarModule, MatCardModule, MATERIAL_SANITY_CHECKS }
    from "@angular/material";

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
        SoUserProfile
    ],
    entryComponents: [LoginDialog],
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
            { path: '**', redirectTo: 'home' }
        ])
    ]
})
export class AppModuleShared {
}
