import { Component, Input, Output, EventEmitter, ViewEncapsulation } from "@angular/core";

@Component({
    selector: "so-menu-item",
    styleUrls: ['./so-menu-item.component.scss'],
    templateUrl: './so-menu-item.component.html',
    encapsulation: ViewEncapsulation.None
})

export class SoMenuItem {
    @Input() menuItem: any;
    @Input() child = false;

    @Output() itemHover = new EventEmitter<any>();
    @Output() toggleSubMenu = new EventEmitter<any>();

    onHoverItem($event: Object): void {
        this.itemHover.emit($event);
    }

    onToggleSubMenu($event: any, item: any): boolean {
        $event.item = item;
        this.toggleSubMenu.emit($event);
        return false;
    }
}