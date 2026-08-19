import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

export interface ProgressData {
  userId: number;
  totalSubjects: number;
  totalNotes: number;
  completedStudyPlans: number;
  totalStudyPlans: number;
  totalQuizzes: number;
  averageQuizScore: number;
  progressPercentage: number;
  lastUpdated: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProgressService {

  private http = inject(HttpClient);

  // Docker backend
  private apiUrl =
    `${environment.apiUrl}/Progress`;

  getProgress(): Observable<ProgressData> {

    return this.http.get<ProgressData>(
      this.apiUrl
    );

  }

}