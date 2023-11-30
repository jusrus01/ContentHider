import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { ADMINISTRATOR_ROLE } from '../constants';
import { LoginResponse } from '../models/login/login-response.model';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private _cachedToken: string | null = null;
  private _cachedRefreshToken: string | null = null;

  private readonly TOKEN_KEY = 'token';
  private readonly REFRESH_KEY = 'refreshToken';

  public readonly isLoggedIn$: Subject<boolean>;

  constructor() {
    this.isLoggedIn$ = new Subject<boolean>();
  }

  public login(loginResponse: LoginResponse): void {
    this.setToken(loginResponse.token);
    this.setRefreshToken(loginResponse.refreshToken);
    this.isLoggedIn$.next(true);
  }

  public logout(): void {
    this.removeToken();
    this.removeRefreshToken();
    this.isLoggedIn$.next(false);
  }

  public getAuthUserId(): string {
    // const userInfo = this.getUserInfo();
    // return userInfo?.userId ?? '';
    console.error('not implemented');
    return '';
  }

  public isLoggedIn(): boolean {
    return this.getCreds() !== null;
  }

  public hasRole(role: string): boolean {
    const token = this.getToken();
    if (token === null) {
      return false;
    }

    const decodedToken = jwtDecode(token) as any;
    return decodedToken.role === role;
  }

  public isAdminOrOwner(userId: any): boolean {
    return (
      (this.hasRole(ADMINISTRATOR_ROLE) || this.getAuthUserId() == userId) &&
      this.isLoggedIn()
    );
  }

  public isOwner(userId: any): boolean {
    return this.getAuthUserId() == userId;
  }

  private getUserInfo(): LoginResponse | null {
    // try {
    //   const parsedUserInfo = JSON.parse(
    //     this.cookieService.get(this.TOKEN_COOKIE_PARAM)
    //   );
    //   return parsedUserInfo as LoginResponse;
    // } catch {
    //   return null;
    // }
    console.error('not implemented');
    return null;
  }

  public getCreds(): any {
    if (this._cachedToken === null) {
      console.log('reloading token from storage');
      this._cachedToken = this.getToken();
    }

    if (this._cachedRefreshToken === null) {
      console.log('reloading refresh token from storage');
      this._cachedRefreshToken = this.getRefreshToken();
    }

    if (this._cachedToken === null || this._cachedRefreshToken === null) {
      return null;
    }

    return { token: this._cachedToken, refreshToken: this._cachedRefreshToken };
  }

  public setCreds(creds: any): void {
    if (creds == null) {
      throw new Error('Credentials are required.');
    }

    this.setToken(creds.token);
    this.setRefreshToken(creds.refreshToken);
  }

  private setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
    this._cachedToken = token;
  }

  private getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  private removeToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this._cachedToken = null;
  }

  private setRefreshToken(token: string): void {
    localStorage.setItem(this.REFRESH_KEY, token);
    this._cachedRefreshToken = token;
  }

  private getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_KEY);
  }

  private removeRefreshToken(): void {
    localStorage.removeItem(this.REFRESH_KEY);
    this._cachedRefreshToken = null;
  }
}
