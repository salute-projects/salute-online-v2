import { Injectable } from "@angular/core";
import { HttpHeaders } from '@angular/common/http';

@Injectable()
export class TokenService {
    isAuthenticated(): boolean {
        const expires = localStorage.getItem('expires_at');
        if (!expires)
            return false;
        const expiresAt = JSON.parse(expires);
        return new Date().getTime() < expiresAt;
    }

    tryGetAuth(): HttpHeaders | undefined {
        if (!this.isAuthenticated())
            return undefined;
        const token = localStorage.getItem('token');
        return new HttpHeaders().set('Authorization', `Bearer ${token}`);
    }
}