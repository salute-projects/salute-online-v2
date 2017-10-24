import { Component, Input, Output, OnInit, OnDestroy, EventEmitter, ViewEncapsulation } from "@angular/core";
import { Routes, Router, NavigationEnd } from "@angular/router";
import { SoMenuService } from "./so-menu.service";
import { Subscription } from "rxjs/Rx";
import { GlobalState } from "../../services/global.state";

@Component({
    selector: "so-menu",
    styleUrls: ['./so-menu.component.scss'],
    templateUrl: './so-menu.component.html',
    providers: [SoMenuService],
    encapsulation: ViewEncapsulation.None
})

export class SoMenu implements OnInit, OnDestroy {
    
    @Input() menuRoutes: Routes = [];
    @Input() sidebarCollapsed = false;
    @Input() menuHeight: number;

    @Output() expandMenu = new EventEmitter<any>();

    menuItems: any[];
    showHoverElem: boolean;
    hoverElemHeight: number;
    hoverElemTop: number;
    protected onRouteChange: Subscription;

    constructor(private readonly router: Router, private readonly service: SoMenuService, private readonly state: GlobalState) {
        this.onRouteChange = this.router.events.subscribe(event => {
            if (event instanceof NavigationEnd) {
                if (this.menuItems) {
                    this.selectMenuAndNotify();
                } else {
                    setTimeout(() => this.selectMenuAndNotify());
                }
            }
        });
    }

    selectMenuAndNotify(): void {
        if (this.menuItems) {
            this.menuItems = this.service.selectMenuItem(this.menuItems);
            this.state.notifyDataChanged(this.state.events.menu.activeLink, this.service.getCurrentItem());
        }
    }

    ngOnInit(): void {
        this.menuItems = this.service.convertRoutesToMenus(this.menuRoutes);
    }

    ngOnDestroy(): void {
        this.onRouteChange.unsubscribe();
    }

    hoverItem($event: any): void {
        this.showHoverElem = true;
        this.hoverElemHeight = $event.currentTarget.clientHeight;
        this.hoverElemTop = $event.currentTarget.getBoundingClientRect().top - 66;
    }

    toggleSubMenu($event: any): boolean {
        if (this.sidebarCollapsed) {
            this.expandMenu.emit(null);
            if (!$event.item.expanded) {
                $event.item.expanded = true;
            }
        } else {
            $event.item.expanded = !$event.item.expanded;
        }
        return false;
    }
}