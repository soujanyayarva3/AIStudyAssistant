
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class Auth {

  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/Auth`;

  // LOGIN
  login(data: any): Observable<any> {
    return this.http.post<any>(
      `${this.apiUrl}/login`,
      data
    );
  }

  // REGISTER
  register(data: any): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/register`,
      data,
      { responseType: 'text' }
    );
  }

  // FORGOT PASSWORD
  forgotPassword(email: string): Observable<any> {
    return this.http.post<any>(
      `${this.apiUrl}/forgot-password`,
      {
        email: email
      }
    );
  }

  // RESET PASSWORD
  resetPassword(
    email: string,
    token: string,
    newPassword: string
  ): Observable<any> {

    return this.http.post(
      `${this.apiUrl}/reset-password`,
      {
        email: email,
        token: token,
        newPassword: newPassword
      },
      {
        responseType: 'text'
      }
    );
  }
}

