import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SummaryService {

  private http = inject(HttpClient);

  // Docker backend
  private apiUrl =
    `${environment.apiUrl}/Summaries`;

  // =====================================================
  // CREATE TEXT SUMMARY
  // POST: /api/Summaries
  // =====================================================

  generateSummary(
    data: any
  ): Observable<any> {

    console.log('==========================================');
    console.log('POST TEXT SUMMARY');
    console.log('REQUEST:', data);
    console.log('==========================================');

    return this.http.post<any>(
      this.apiUrl,
      data
    );

  }

  // =====================================================
  // IMAGE SUMMARY
  // POST: /api/Summaries/image
  // =====================================================

  uploadImage(
    file: File,
    title: string,
    summaryStyle: string
  ): Observable<any> {

    const formData = new FormData();

    formData.append(
      'image',
      file
    );

    formData.append(
      'title',
      title
    );

    formData.append(
      'summaryStyle',
      summaryStyle
    );

    console.log('==========================================');
    console.log('POST IMAGE SUMMARY');
    console.log('FILE:', file.name);
    console.log('TITLE:', title);
    console.log('STYLE:', summaryStyle);
    console.log('==========================================');

    return this.http.post<any>(
      `${this.apiUrl}/image`,
      formData
    );

  }

  // =====================================================
  // PDF SUMMARY
  // POST: /api/Summaries/pdf
  // =====================================================

  uploadPdf(
    file: File,
    title: string,
    summaryStyle: string
  ): Observable<any> {

    const formData = new FormData();

    formData.append(
      'pdf',
      file
    );

    formData.append(
      'title',
      title
    );

    formData.append(
      'summaryStyle',
      summaryStyle
    );

    console.log('==========================================');
    console.log('POST PDF SUMMARY');
    console.log('FILE:', file.name);
    console.log('TITLE:', title);
    console.log('STYLE:', summaryStyle);
    console.log('==========================================');

    return this.http.post<any>(
      `${this.apiUrl}/pdf`,
      formData
    );

  }

  // =====================================================
  // GET ALL REPORTS
  // GET: /api/Summaries
  // =====================================================

  getSummaries(): Observable<any[]> {

    console.log(
      'GET ALL SUMMARIES:',
      this.apiUrl
    );

    return this.http.get<any[]>(
      this.apiUrl
    );

  }

  // =====================================================
  // GET ONE REPORT
  // GET: /api/Summaries/{id}
  // =====================================================

  getSummary(
    id: number
  ): Observable<any> {

    const summaryId =
      Number(id);

    return this.http.get<any>(
      `${this.apiUrl}/${summaryId}`
    );

  }

  // =====================================================
  // DELETE REPORT
  // DELETE: /api/Summaries/{id}
  // =====================================================

  deleteSummary(
    id: number
  ): Observable<void> {

    const summaryId =
      Number(id);

    console.log(
      'DELETE SUMMARY:',
      summaryId
    );

    return this.http.delete<void>(
      `${this.apiUrl}/${summaryId}`
    );

  }

  // =====================================================
  // DOWNLOAD REPORT PDF
  // GET: /api/Summaries/download/{id}
  // =====================================================

  downloadSummary(
    id: number
  ): Observable<Blob> {

    const summaryId =
      Number(id);

    console.log(
      'DOWNLOAD SUMMARY PDF:',
      summaryId
    );

    return this.http.get(
      `${this.apiUrl}/download/${summaryId}`,
      {
        responseType: 'blob'
      }
    );

  }

}