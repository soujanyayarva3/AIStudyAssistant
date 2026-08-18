import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { StudyPlan } from '../models/study-plan';

@Injectable({
  providedIn: 'root'
})
export class StudyPlanService {

  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7241/api/StudyPlans';

  getStudyPlans(): Observable<StudyPlan[]> {
    return this.http.get<StudyPlan[]>(this.apiUrl);
  }

  createStudyPlan(plan: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, plan);
  }

  updateStudyPlan(id: number, plan: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, plan);
  }

  deleteStudyPlan(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

}