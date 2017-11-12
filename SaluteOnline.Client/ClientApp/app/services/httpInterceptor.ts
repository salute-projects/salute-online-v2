import { Injectable, Injector } from "@angular/core";
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from "@angular/common/http";
import { AuthService } from "./auth";
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(private readonly injector: Injector) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const auth = this.injector.get(AuthService);
        if (req.url.indexOf('account/') > -1)
            return next.handle(req);
        const token = auth.tryGetToken();
        if (!token)
            return next.handle(req);
        const authReq = req.clone({ headers: req.headers.set('Authorization', token) });
        return next.handle(authReq);
    }
}