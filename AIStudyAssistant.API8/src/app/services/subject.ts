import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class SubjectService {

  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/Subjects`;

  getSubjects(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }
}