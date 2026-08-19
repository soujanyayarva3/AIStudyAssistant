import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { AIChatModel } from '../models/ai-chat';
import { Conversation } from '../models/conversation';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AIChatService {

  private http = inject(HttpClient);

  // Docker backend
  private chatUrl =
    `${environment.apiUrl}/AIChats`;

  private conversationUrl =
    `${environment.apiUrl}/Conversations/12`;


  // =====================================================
  // GET ALL CONVERSATIONS
  // =====================================================

  getConversations(): Observable<Conversation[]> {

    return this.http.get<Conversation[]>(
      this.conversationUrl
    );

  }


  // =====================================================
  // GET CHATS OF ONE CONVERSATION
  // =====================================================

  getConversationChats(
    conversationId: number
  ): Observable<AIChatModel[]> {

    return this.http.get<AIChatModel[]>(
      `${this.chatUrl}/conversation/${conversationId}`
    );

  }


  // =====================================================
  // ASK AI
  // =====================================================

  sendQuestion(data: any): Observable<any> {

    return this.http.post(
      this.chatUrl,
      data
    );

  }

}