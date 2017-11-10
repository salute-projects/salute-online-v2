import { Injectable } from "@angular/core";
import { Subject } from "rxjs/Subject";
import { DataSource } from '@angular/cdk/collections';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class CustomDataSource<T> extends DataSource<T> {
    dataChange = new BehaviorSubject<T[]>([]);

    constructor(dataSet: T[]) {
        super();
        this.dataChange.next(dataSet);
    }

    connect(): Observable<T[]> {
        return this.dataChange;
    }

    disconnect(collectionViewer: Object): void { }
}