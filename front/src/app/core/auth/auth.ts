import { HttpClient } from '@angular/common/http';
import { computed, Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

export interface Credentials {
    email: string;
    password: string;
}

interface UserInfo {
    email: string;
}

@Injectable({providedIn: 'root'})
export class Auth {

    private readonly http = inject(HttpClient);
    private readonly currentUser = signal<UserInfo | null>(null);

    readonly user = this.currentUser.asReadonly();
    readonly isAuthenticated = computed(() => this.currentUser() !== null);

    // Get XSRF token and search from a previous valid one
    async initialize(): Promise<void> {
        await this.refreshXsrfToken();

        try {
            this.currentUser.set(await firstValueFrom(this.http.get<UserInfo>('/api/auth/manage/info')));
        } catch {
            this.currentUser.set(null);
        }
    }

    // Register User and logIn immediately after
    async register(credentials: Credentials): Promise<void> {
        await firstValueFrom(this.http.post('/api/auth/register', credentials));
        await this.login(credentials);
    }

    // LogIn
    async login(credentials: Credentials): Promise<void> {
        await firstValueFrom(this.http.post('/api/auth/login?useCookies=true', credentials));
        await this.refreshXsrfToken();
        this.currentUser.set(await firstValueFrom(this.http.get<UserInfo>('/api/auth/manage/info')));
    }

    // LogOut
    async logout(): Promise<void> {
        await firstValueFrom(this.http.post('/api/auth/logout', {}));
        await this.afterSignOut();
    }

    // Delete account
    async deleteAccount(): Promise<void> {
        await firstValueFrom(this.http.delete('/api/auth/account'));
        await this.afterSignOut();
    }

    // Called by Interceptor in case of API respond 401 (expired token)
    clear(): void {
        this.currentUser.set(null);
    }

    // CurrentUser set to null and refresh XSRF Token
    private async afterSignOut(): Promise<void> {
        this.currentUser.set(null);
        await this.refreshXsrfToken();
    }

    // Refresh XSRF Token from API
    private async refreshXsrfToken(): Promise<void> {
        await firstValueFrom(this.http.get('/api/antiforgery/token'));
    }
}
