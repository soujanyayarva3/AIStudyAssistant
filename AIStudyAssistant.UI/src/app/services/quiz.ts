import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

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

  private apiUrl =
    'http://172.17.16.126:8080/api/Quizzes'


  // =====================================================
  // GENERATE QUIZ
  // =====================================================

  generateQuiz(
    topic: string,
    difficulty: string = 'Medium'
  ): Observable<Quiz[]> {

    console.log(
      'Generating quiz:',
      {
        topic: topic,
        difficulty: difficulty
      }
    );

    return this.http.post<Quiz[]>(
      this.apiUrl,
      {
        topic: topic,
        difficulty: difficulty,
        score: 1
      }
    );

  }


  // =====================================================
  // GET ALL QUIZZES
  // =====================================================

  getQuizzes(): Observable<Quiz[]> {

    return this.http.get<Quiz[]>(
      this.apiUrl
    );

  }

}