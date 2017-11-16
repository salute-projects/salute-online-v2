import { Component, ViewEncapsulation } from "@angular/core";
import { Context } from "../../services/context/context";
import { InnerMessageDto, InnerMessagesFilter, EntityType, MessageStatus, Page } from "../../dto/dto";
import { SoSnackService } from "../../services/snack.service";
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { systemAvatar } from "../../configuration/constants";

@Component({
    selector: "so-messages",
    styleUrls: ["./so-messages.scss"],
    templateUrl: "./so-messages.html",
    encapsulation: ViewEncapsulation.None
})

export class SoMessages {
    messages = new Page<InnerMessageDto>();

    constructor(private readonly context: Context, private readonly snackService: SoSnackService, private readonly sanitizer: DomSanitizer) {
        const filter = new InnerMessagesFilter(EntityType.User, null, MessageStatus.Pending);
        this.context.innerMessageApi.getMessages(filter).subscribe(result => {
            this.messages = result;
        }, error => {
            this.snackService.showError(error.error, 'OK');
        });
    }

    getAvatar(avatar: string) {
        return this.sanitizer.bypassSecurityTrustResourceUrl(avatar ? `data:image/jpg;base64,${avatar}` : systemAvatar);
    }
}