import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class Auth {

  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/Auth`;

  login(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, data);
  }

  
  register(data: any): Observable<any> {
  return this.http.post(
    `${this.apiUrl}/register`,
    data,
    { responseType: 'text' }
  );
}
}