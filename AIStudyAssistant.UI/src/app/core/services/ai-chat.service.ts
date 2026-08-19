import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

export interface AIChatResponse {
  chatId: number;
  conversationId: number;
  question: string;
  response: string;
  userId: number;
  createdDate: string;
}

@Injectable({
  providedIn: 'root'
})
export class AIChatService {

  private http = inject(HttpClient);

  private api = `${environment.apiUrl}/AIChats`;

  // =====================================================
  // SEND AI CHAT MESSAGE
  // =====================================================

  sendMessage(data: {
    conversationId: number;
    question: string;
    response: string;
    userId: number;

    // AI preferences
    responseStyle: string;
    showExamples: boolean;

  }): Observable<AIChatResponse> {

    console.log(
      'Sending AI request:',
      data
    );

    return this.http.post<AIChatResponse>(
      this.api,
      data
    );
  }

  // =====================================================
  // GET CONVERSATION MESSAGES
  // =====================================================

  getConversationMessages(
    id: number
  ): Observable<AIChatResponse[]> {

    return this.http.get<AIChatResponse[]>(
      `${this.api}/conversation/${id}`
    );
  }

  // =====================================================
  // GET ALL AI CHATS
  // =====================================================

  getAIChats(): Observable<AIChatResponse[]> {

    console.log(
      'Loading AI Chats...'
    );

    return this.http.get<AIChatResponse[]>(
      this.api
    );
  }
}