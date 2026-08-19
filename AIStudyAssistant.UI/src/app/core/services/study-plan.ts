import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { StudyPlan } from '../models/study-plan';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class StudyPlanService {

  private http = inject(HttpClient);

  private apiUrl =
    `${environment.apiUrl}/StudyPlans`;

  // =====================================================
  // GET ALL STUDY PLANS
  // =====================================================

  getStudyPlans(): Observable<StudyPlan[]> {

    return this.http.get<StudyPlan[]>(
      this.apiUrl
    );

  }

  // =====================================================
  // CREATE STUDY PLAN
  // =====================================================

  createStudyPlan(
    plan: any
  ): Observable<any> {

    return this.http.post<any>(
      this.apiUrl,
      plan
    );

  }

  // =====================================================
  // UPDATE STUDY PLAN
  // =====================================================

  updateStudyPlan(
    id: number,
    plan: any
  ): Observable<any> {

    return this.http.put<any>(
      `${this.apiUrl}/${id}`,
      plan
    );

  }

  // =====================================================
  // DELETE STUDY PLAN
  // =====================================================

  deleteStudyPlan(
    id: number
  ): Observable<any> {

    return this.http.delete<any>(
      `${this.apiUrl}/${id}`
    );

  }

}