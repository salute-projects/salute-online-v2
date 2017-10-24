import { Component, ViewEncapsulation, ElementRef, AfterViewInit, OnInit, HostListener } from "@angular/core";
import { menuList } from "../../configuration/menu-list";
import { layoutSizes } from "../../configuration/constants";
import { GlobalState } from "../../services/global.state";
import * as _ from "lodash";

@Component({
    selector: "so-sidebar",
    styleUrls: ["./so-sidebar.component.scss"],
    templateUrl: "./so-sidebar.component.html",
    encapsulation: ViewEncapsulation.None
})

export class SoSidebar implements AfterViewInit, OnInit {
    routes = _.cloneDeep(menuList);

    isMenuCollapsed = false;
    isSidebarShouldCollapse = false;
    menuHeight: number;

    constructor(private readonly elementRef: ElementRef, private readonly state: GlobalState) {
        this.state.subscribe('menu.isCollapsed', (isCollapsed: boolean) => {
            this.isMenuCollapsed = isCollapsed;
        });
    }

    ngAfterViewInit(): void {
        setTimeout(() => this.updateHeight());
    }

    ngOnInit(): void {
        if (this.shouldSidebarCollapse()) {
            this.sidebarCollapse();
        }
    }

    updateHeight(): void {
        this.menuHeight = this.elementRef.nativeElement.childNodes[0].clientHeight - 84;
    }

    sidebarCollapseStatusChange(collapse: boolean): void {
        this.isMenuCollapsed = collapse;
        this.state.notifyDataChanged(this.state.events.menu.isCollapsed, this.isMenuCollapsed);
    }

    sidebarExpand(): void {
        this.sidebarCollapseStatusChange(false);
    }

    sidebarCollapse(): void {
        this.sidebarCollapseStatusChange(true);
    }

    @HostListener('window:resize')
    onWindowResize(): void {
        const isSidebarMustCollapse = this.shouldSidebarCollapse();
        this.isSidebarShouldCollapse = isSidebarMustCollapse;
        this.updateHeight();
    }

    private shouldSidebarCollapse(): boolean {
        return window.innerWidth <= layoutSizes.resolutionCollapseSidebar;
    }
}