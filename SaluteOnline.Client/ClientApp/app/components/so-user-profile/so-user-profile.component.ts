import { Component, ViewEncapsulation } from "@angular/core";
import { Context } from "../../services/context/context";
import { GlobalState } from "../../services/global.state";
import { UserDto, UserMainInfoDto, UserPersonalInfoDto, Country } from "../../dto/dto";
import { SoSnackService } from "../../services/snack.service";
import { FormControl } from '@angular/forms';
import { Observable } from 'rxjs/Observable';

@Component({
    selector: 'so-user-profile',
    styleUrls: ['./so-user-profile.component.scss'],
    templateUrl: './so-user-profile.component.html',
    encapsulation: ViewEncapsulation.None
})

export class SoUserProfile {
    countryCtrl: FormControl;
    allCountries: Country[];
    filteredCountries: Observable<Country[]>;

    userProfile = new UserDto();
    pickerOptions = {
        maxDate: new Date(),
        minDate: new Date(1950, 0, 1)
    }

    constructor(private readonly context: Context, private readonly snackService: SoSnackService, private readonly state: GlobalState) {
        this.countryCtrl = new FormControl();
        this.context.userApi.getUserInfo().subscribe((result: UserDto) => {
            this.userProfile = result;
            this.userProfile.avatar = `data:image/jpg;base64,${this.userProfile.avatar}`;
        }, error => {
            this.snackService.showError(error.error, 'OK', undefined);
        });
        this.context.commonApi.getCountries().subscribe(result => {
            this.allCountries = result;
            this.filteredCountries = this.countryCtrl.valueChanges.startWith(null)
                .map((country: string) => country ? this.filterCountries(country) : this.allCountries.slice());
        }, () => {
            this.filteredCountries = Observable.from([]);
        });
    }

    private filterCountries(country: string) {
        return this.allCountries.filter(state => state.name.toLowerCase().indexOf(country.toLowerCase()) === 0);
    }

    updateInfo() : void {
        this.context.userApi.updateUserInfo(this.userProfile).subscribe(() => {
            this.snackService.showGeneral("Successfully saved", 'OK');
        }, error => {
            this.snackService.showError(error.error, 'OK', undefined);
        });
    }

    updateMainUserInfo(): void {
        const args = new UserMainInfoDto(this.userProfile);
        this.context.userApi.updateMainUserInfo(args).subscribe(() => {
            this.snackService.showGeneral("Successfully saved", 'OK');
        }, error => {
            this.snackService.showError(error.error, 'OK', undefined);
        });
    }

    updatePersonalUserInfo(): void {
        const args = new UserPersonalInfoDto(this.userProfile);
        args.country = this.countryCtrl.value;
        this.context.userApi.updatePersonalUserInfo(args).subscribe(() => {
            this.snackService.showGeneral("Successfully saved", 'OK');
        }, error => {
            this.snackService.showError(error.error, 'OK', undefined);
        });
    }

    onAvatarChange(event: any) {
        const newAvatar: File = event.target.files[0];
        this.context.userApi.uploadAvatar(newAvatar).subscribe(result => {
            this.userProfile.avatar = `data:image/jpg;base64,${result}`;
            localStorage.setItem('avatar', `data:image/jpg;base64,${result}`);
            this.state.notifyDataChanged(this.state.events.user.avatarChanged, '');
        }, error => {
            this.snackService.showError(error.error, 'OK', undefined);
        });
    }
}