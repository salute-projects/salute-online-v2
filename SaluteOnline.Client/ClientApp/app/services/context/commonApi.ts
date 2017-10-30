import { Injector, Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { apiSettings } from "../../configuration/constants";
import { AuthService } from "../../services/auth";
import { Observable, Observer } from 'rxjs';
import { Country } from "../../dto/dto";

@Injectable()
export class CommonApi {
    urls: any = {
        getCountries: 'common/countries'
    }

    constructor(private readonly http: HttpClient) {
    }

    getCountries(): Observable<Country[]> {
        return this.http.get<Country[]>(apiSettings.baseUrl + this.urls.getCountries);
    }
}