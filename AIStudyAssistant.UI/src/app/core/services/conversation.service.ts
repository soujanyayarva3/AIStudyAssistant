import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConversationService {

  private http = inject(HttpClient);

  private api = 'http://172.17.16.126:8080/api/Conversations';

  getHistory(): Observable<any[]> {
    return this.http.get<any[]>(`${this.api}/history`);
  }

}