import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AIChatModel } from '../models/ai-chat';

@Injectable({
  providedIn: 'root'
})
export class AIChatService {

  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7241/api/AIChats';

  getChats(): Observable<AIChatModel[]> {
    return this.http.get<AIChatModel[]>(this.apiUrl);
  }

  sendQuestion(data: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, data);
  }
}