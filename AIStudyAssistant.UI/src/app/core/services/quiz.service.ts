import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

export interface Quiz {
  quizId: number;
  title: string;
  question: string;
  optionA: string;
  optionB: string;
  optionC: string;
  optionD: string;
  correctAnswer: string;
  score: number;
  createdDate: string;
}

@Injectable({
  providedIn: 'root'
})
export class QuizService {

  private http = inject(HttpClient);

  // Docker backend
  private apiUrl =
    `${environment.apiUrl}/Quizzes`;

  generateQuiz(
    topic: string
  ): Observable<any> {

    return this.http.post(
      this.apiUrl,
      {
        topic: topic,
        score: 10
      }
    );

  }

}