import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from '../constants';

@Injectable({
  providedIn: 'root',
})
export class RegisterResource {
  constructor(private httpClient: HttpClient) {}

  register(data: any): Observable<any> {
    return this.httpClient.post(`${API_URL}/account/register`, {
      firstName: data.firstName,
      lastName: data.lastName,
      email: data.email,
      password: data.password,
    });
  }
}
  