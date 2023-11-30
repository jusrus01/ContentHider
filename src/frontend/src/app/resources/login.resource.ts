import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { LoginResponse } from '../models/login/login-response.model';
import { API_URL } from '../constants';

@Injectable({
  providedIn: 'root',
})
export class LoginResource {
  constructor(private httpClient: HttpClient) {}

  login(data: any): Observable<any> {
    return this.httpClient.post<LoginResponse>(`${API_URL}/auth/login`, {
      email: data.email,
      password: data.password,
    });
  }

  refresh(data: any): Observable<any> {
    return this.httpClient.post<LoginResponse>(
      `${API_URL}/auth/refresh-login`,
      {
        token: data.token,
        refreshToken: data.refreshToken,
      }
    );
  }
}
