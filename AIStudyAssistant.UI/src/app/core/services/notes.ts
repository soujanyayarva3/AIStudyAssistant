import {
  Injectable,
  inject
} from '@angular/core';

import {
  HttpClient
} from '@angular/common/http';

import {
  Observable
} from 'rxjs';

import {
  Note
} from '../models/note';

import {
  environment
} from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class NotesService {

  private http = inject(HttpClient);

  private baseUrl =
    `${environment.apiUrl}/Notes`;

  // =====================================================
  // GET ALL NOTES
  // =====================================================

  getNotes(): Observable<Note[]> {

    return this.http.get<Note[]>(
      this.baseUrl
    );

  }

  // =====================================================
  // GET SINGLE NOTE
  // =====================================================

  getNote(
    id: number
  ): Observable<Note> {

    return this.http.get<Note>(
      `${this.baseUrl}/${id}`
    );

  }

  // =====================================================
  // CREATE NOTE
  // =====================================================

  createNote(
    note: {
      title: string;
      content: string;
      subjectId: number;
    }
  ): Observable<Note> {

    console.log(
      'CREATING NOTE:',
      note
    );

    return this.http.post<Note>(
      this.baseUrl,
      note
    );

  }

  // =====================================================
  // UPDATE NOTE
  // =====================================================

  updateNote(
    id: number,
    note: {
      title: string;
      content: string;
      subjectId: number;
    }
  ): Observable<void> {

    return this.http.put<void>(
      `${this.baseUrl}/${id}`,
      note
    );

  }

  // =====================================================
  // DELETE NOTE
  // =====================================================

  deleteNote(
    id: number
  ): Observable<void> {

    console.log(
      'DELETING NOTE:',
      id
    );

    return this.http.delete<void>(
      `${this.baseUrl}/${id}`
    );

  }

}