import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { AskAI } from '../models/ask-ai';
import { AIResponse } from '../models/ai-response';

@Injectable({
  providedIn: 'root'
})
export class AIService {

  private http = inject(HttpClient);

  private apiUrl = 'http://172.17.16.126:8080/api/AI';
  askAI(request: AskAI): Observable<AIResponse> {

    return this.http.post<AIResponse>(
      `${this.apiUrl}/ask`,
      request
    );

  }

}