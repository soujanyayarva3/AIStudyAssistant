import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Subject } from '../models/subject';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SubjectService {

  private http = inject(HttpClient);

  private apiUrl =
    `${environment.apiUrl}/Subjects`;

  // =====================================================
  // GET ALL SUBJECTS
  // =====================================================

  getSubjects(): Observable<Subject[]> {

    return this.http.get<Subject[]>(
      this.apiUrl
    );

  }

  // =====================================================
  // GET SUBJECT BY ID
  // =====================================================

  getSubjectById(
    id: number
  ): Observable<Subject> {

    return this.http.get<Subject>(
      `${this.apiUrl}/${id}`
    );

  }

  // =====================================================
  // CREATE SUBJECT
  // =====================================================

  createSubject(
    subject: any
  ): Observable<any> {

    return this.http.post<any>(
      this.apiUrl,
      subject
    );

  }

  // =====================================================
  // UPDATE SUBJECT
  // =====================================================

  updateSubject(
    id: number,
    subject: any
  ): Observable<any> {

    return this.http.put<any>(
      `${this.apiUrl}/${id}`,
      subject
    );

  }

  // =====================================================
  // DELETE SUBJECT
  // =====================================================

  deleteSubject(
    id: number
  ): Observable<any> {

    return this.http.delete<any>(
      `${this.apiUrl}/${id}`
    );

  }

}