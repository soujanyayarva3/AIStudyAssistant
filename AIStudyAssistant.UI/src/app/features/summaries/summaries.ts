
import {
  Component,
  ChangeDetectorRef,
  inject,
  OnInit
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { SummaryService } from '../../core/services/summary.service';
import { NotesService } from '../../core/services/notes';

import { Subject } from '../../core/models/subject';
import { SubjectService } from '../../core/services/subject';

@Component({
  selector: 'app-summaries',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './summaries.html',
  styleUrl: './summaries.scss'
})
export class Summaries implements OnInit {

  // =====================================================
  // FORM DATA
  // =====================================================

  title = '';

  originalText = '';

  selectedImage: File | null = null;

  imagePreview: string | null = null;

  selectedPdf: File | null = null;

  selectedSource = 'text';

  summaryStyle = 'revision';

  // =====================================================
  // SUMMARY RESULT
  // =====================================================

  summaryResult: any = null;

  loading = false;

  // =====================================================
  // SAVE TO NOTES
  // =====================================================

  showSaveNotesModal = false;

  savingNote = false;

  subjects: Subject[] = [];

  selectedSubjectId = 0;

  // =====================================================
  // SAVED REPORTS
  // =====================================================

  savedReports: any[] = [];

  filteredReports: any[] = [];

  reportSearchText = '';

  // =====================================================
  // SERVICES
  // =====================================================

  private summaryService = inject(SummaryService);

  private notesService = inject(NotesService);

  private subjectService = inject(SubjectService);

  private cdr = inject(ChangeDetectorRef);

  // =====================================================
  // INIT
  // =====================================================

  ngOnInit(): void {

    console.log('==========================================');
    console.log('SUMMARIES COMPONENT LOADED');
    console.log('==========================================');

    this.loadReports();

    this.loadSubjects();
  }

  // =====================================================
  // LOAD SAVED REPORTS
  // =====================================================

  loadReports(): void {

    console.log('==========================================');
    console.log('LOADING SAVED REPORTS...');
    console.log('==========================================');

    this.summaryService
      .getSummaries()
      .subscribe({

        next: (data: any) => {

          console.log('RAW REPORT RESPONSE:', data);

          if (Array.isArray(data)) {

            this.savedReports = data;

          } else {

            this.savedReports = [];

          }

          console.log(
            'REPORT COUNT:',
            this.savedReports.length
          );

          this.savedReports.forEach(
            (report, index) => {

              console.log(
                `REPORT ${index + 1}:`,
                report
              );

              console.log(
                'SummaryId:',
                report?.summaryId
              );

              console.log(
                'Title:',
                report?.title
              );

              console.log(
                'Subject:',
                report?.subject
              );

            }
          );

          this.filterReports();

          this.cdr.detectChanges();

        },

        error: (err) => {

          console.error(
            '=========================================='
          );

          console.error(
            'REPORT LOAD ERROR'
          );

          console.error(
            'STATUS:',
            err?.status
          );

          console.error(
            'ERROR:',
            err
          );

          console.error(
            'SERVER RESPONSE:',
            err?.error
          );

          console.error(
            '=========================================='
          );

          this.savedReports = [];

          this.filteredReports = [];

          this.cdr.detectChanges();

        }

      });
  }

  // =====================================================
  // LOAD SUBJECTS
  // =====================================================

  loadSubjects(): void {

    console.log(
      'Loading subjects...'
    );

    this.subjectService
      .getSubjects()
      .subscribe({

        next: (data: Subject[]) => {

          console.log(
            'SUBJECTS FOR SUMMARY:',
            data
          );

          this.subjects =
            Array.isArray(data)
              ? data
              : [];

          this.cdr.detectChanges();

        },

        error: (err) => {

          console.error(
            'SUBJECT LOAD ERROR:',
            err
          );

          this.subjects = [];

          this.cdr.detectChanges();

        }

      });
  }

  // =====================================================
  // SEARCH REPORTS
  // =====================================================

  filterReports(): void {

    const search =
      this.reportSearchText
        .trim()
        .toLowerCase();

    if (!search) {

      this.filteredReports = [
        ...this.savedReports
      ];

      return;
    }

    this.filteredReports =
      this.savedReports.filter(
        report => {

          const title =
            String(
              report?.title || ''
            ).toLowerCase();

          const style =
            String(
              report?.summaryStyle || ''
            ).toLowerCase();

          const fileName =
            String(
              report?.fileName || ''
            ).toLowerCase();

          const subjectName =
            String(
              report?.subject?.subjectName ||
              report?.subjectName ||
              ''
            ).toLowerCase();

          return (
            title.includes(search) ||
            style.includes(search) ||
            fileName.includes(search) ||
            subjectName.includes(search)
          );

        }
      );
  }

  // =====================================================
  // IMAGE SELECT
  // =====================================================

  onImageSelected(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    if (!input.files?.length) {

      return;
    }

    this.selectedImage =
      input.files[0];

    console.log(
      'IMAGE SELECTED:',
      this.selectedImage.name
    );

    const reader =
      new FileReader();

    reader.onload = () => {

      this.imagePreview =
        reader.result as string;

      this.cdr.detectChanges();

    };

    reader.readAsDataURL(
      this.selectedImage
    );
  }

  // =====================================================
  // PDF SELECT
  // =====================================================

  onPdfSelected(
    event: Event
  ): void {

    const input =
      event.target as HTMLInputElement;

    if (!input.files?.length) {

      return;
    }

    this.selectedPdf =
      input.files[0];

    console.log(
      'PDF SELECTED:',
      this.selectedPdf.name
    );

    this.cdr.detectChanges();
  }

  // =====================================================
  // PARSE AI ARRAY
  // =====================================================

  private parseArray(
    data: any
  ): any[] {

    if (!data) {

      return [];
    }

    if (Array.isArray(data)) {

      return data;
    }

    if (typeof data === 'string') {

      try {

        const parsed =
          JSON.parse(data);

        if (Array.isArray(parsed)) {

          return parsed;
        }

      } catch {

        console.warn(
          'Unable to parse array:',
          data
        );

      }

    }

    return [];
  }

  // =====================================================
  // GENERATE SUMMARY
  // =====================================================

  generateSummary(): void {

    // ---------------------------------------------------
    // VALIDATE TITLE
    // ---------------------------------------------------

    if (!this.title.trim()) {

      alert(
        'Please enter a title for this summary.'
      );

      return;
    }

    // ---------------------------------------------------
    // VALIDATE SOURCE
    // ---------------------------------------------------

    if (
      this.selectedSource === 'text' &&
      !this.originalText.trim()
    ) {

      alert(
        'Please paste some study material.'
      );

      return;
    }

    if (
      this.selectedSource === 'image' &&
      !this.selectedImage
    ) {

      alert(
        'Please select an image.'
      );

      return;
    }

    if (
      this.selectedSource === 'pdf' &&
      !this.selectedPdf
    ) {

      alert(
        'Please select a PDF.'
      );

      return;
    }

    // ---------------------------------------------------
    // START LOADING
    // ---------------------------------------------------

    this.loading = true;

    this.summaryResult = null;

    // ===================================================
    // TEXT SUMMARY
    // ===================================================

    if (
      this.selectedSource === 'text'
    ) {

      console.log(
        'GENERATING TEXT SUMMARY...'
      );

      /*
       * SubjectId is not required to generate
       * the summary. It can be assigned when
       * saving the summary to Notes.
       */

      const request = {

        title:
          this.title.trim(),

        summaryStyle:
          this.summaryStyle,

        originalText:
          this.originalText,

        summaryText:
          ''

      };

      console.log(
        'TEXT SUMMARY REQUEST:',
        request
      );

      this.summaryService
        .generateSummary(request)
        .subscribe({

          next: (response) => {

            console.log(
              'TEXT SUMMARY RESPONSE:',
              response
            );

            this.setSummaryResult(
              response
            );

            this.loading = false;

            /*
             * Reload saved reports because
             * the backend creates the Summary
             * during this request.
             */

            this.loadReports();

            this.cdr.detectChanges();

          },

          error: (err) => {

            console.error(
              'TEXT SUMMARY ERROR:',
              err
            );

            console.error(
              'SERVER RESPONSE:',
              err?.error
            );

            this.loading = false;

            alert(
              err?.error?.message ||
              'Unable to generate summary.'
            );

            this.cdr.detectChanges();

          }

        });

      return;
    }

    // ===================================================
    // IMAGE SUMMARY
    // ===================================================

    if (
      this.selectedSource === 'image'
    ) {

      if (!this.selectedImage) {

        this.loading = false;

        return;
      }

      console.log(
        'GENERATING IMAGE SUMMARY...'
      );

      this.summaryService
        .uploadImage(

          this.selectedImage,

          this.title.trim(),

          this.summaryStyle

        )
        .subscribe({

          next: (response) => {

            console.log(
              'IMAGE SUMMARY RESPONSE:',
              response
            );

            this.setSummaryResult(
              response
            );

            this.loading = false;

            this.loadReports();

            this.cdr.detectChanges();

          },

          error: (err) => {

            console.error(
              'IMAGE SUMMARY ERROR:',
              err
            );

            console.error(
              'SERVER RESPONSE:',
              err?.error
            );

            this.loading = false;

            alert(
              err?.error?.message ||
              'Unable to summarize the image.'
            );

            this.cdr.detectChanges();

          }

        });

      return;
    }

    // ===================================================
    // PDF SUMMARY
    // ===================================================

    if (
      this.selectedSource === 'pdf'
    ) {

      if (!this.selectedPdf) {

        this.loading = false;

        return;
      }

      console.log(
        'GENERATING PDF SUMMARY...'
      );

      this.summaryService
        .uploadPdf(

          this.selectedPdf,

          this.title.trim(),

          this.summaryStyle

        )
        .subscribe({

          next: (response) => {

            console.log(
              'PDF SUMMARY RESPONSE:',
              response
            );

            this.setSummaryResult(
              response
            );

            this.loading = false;

            this.loadReports();

            this.cdr.detectChanges();

          },

          error: (err) => {

            console.error(
              'PDF SUMMARY ERROR:',
              err
            );

            console.error(
              'SERVER RESPONSE:',
              err?.error
            );

            this.loading = false;

            alert(
              err?.error?.message ||
              'Unable to summarize the PDF.'
            );

            this.cdr.detectChanges();

          }

        });
    }
  }

  // =====================================================
  // SET SUMMARY RESULT
  // =====================================================

  private setSummaryResult(
    response: any
  ): void {

    console.log(
      '=========================================='
    );

    console.log(
      'RAW SUMMARY RESPONSE:',
      response
    );

    console.log(
      '=========================================='
    );

    const keywords =
      this.parseArray(
        response?.keywords
      );

    const questions =
      this.parseArray(
        response?.questions
      );

    this.summaryResult = {

      summary:
        response?.summaryText ??
        response?.summary ??
        'No summary generated.',

      keywords:
        keywords,

      questions:
        questions

    };

    /*
     * Keep SummaryId in the result.
     * This is useful if the generated summary
     * needs to be downloaded immediately.
     */

    this.summaryResult.summaryId =
      response?.summaryId ??
      response?.id ??
      null;

    console.log(
      'FINAL SUMMARY RESULT:',
      this.summaryResult
    );

    console.log(
      'SummaryId:',
      this.summaryResult.summaryId
    );

    console.log(
      'Keywords:',
      this.summaryResult.keywords
    );

    console.log(
      'Questions:',
      this.summaryResult.questions
    );

    console.log(
      '=========================================='
    );

    this.cdr.detectChanges();
  }

  // =====================================================
  // OPEN SAVE NOTES
  // =====================================================

  openSaveNotes(): void {

    if (!this.summaryResult) {

      alert(
        'Generate a summary first.'
      );

      return;
    }

    if (!this.subjects.length) {

      alert(
        'No subjects are available. Please create a subject first.'
      );

      return;
    }

    this.selectedSubjectId = 0;

    this.showSaveNotesModal = true;

    this.cdr.detectChanges();
  }

  // =====================================================
  // CLOSE SAVE NOTES
  // =====================================================

  closeSaveNotes(): void {

    if (this.savingNote) {

      return;
    }

    this.showSaveNotesModal = false;

    this.selectedSubjectId = 0;

    this.cdr.detectChanges();
  }

  // =====================================================
  // SAVE SUMMARY TO NOTES
  // =====================================================

  saveToNotes(): void {

    if (!this.summaryResult) {

      alert(
        'Generate a summary first.'
      );

      return;
    }

    const subjectId =
      Number(
        this.selectedSubjectId
      );

    if (
      !subjectId ||
      subjectId <= 0
    ) {

      alert(
        'Please select a subject.'
      );

      return;
    }

    const selectedSubject =
      this.subjects.find(
        subject =>
          Number(
            subject.subjectId
          ) === subjectId
      );

    if (!selectedSubject) {

      alert(
        'The selected subject is invalid.'
      );

      return;
    }

    // ---------------------------------------------------
    // BUILD NOTE CONTENT
    // ---------------------------------------------------

    const keywords =
      this.summaryResult.keywords?.length
        ? this.summaryResult.keywords.join(', ')
        : 'No keywords generated.';

    const questions =
      this.summaryResult.questions?.length
        ? this.summaryResult.questions
            .map(
              (q: string, i: number) =>
                `${i + 1}. ${q}`
            )
            .join('\n')
        : 'No viva questions generated.';

    const note = {

      title:
        this.title.trim() ||
        `${this.summaryStyle} Summary`,

      content:
`AI Study Summary

Title:
${this.title.trim()}

Summary:
${this.summaryResult.summary}

Key Concepts:
${keywords}

Viva Questions:
${questions}`,

      subjectId:
        subjectId

    };

    console.log(
      '=========================================='
    );

    console.log(
      'SAVING SUMMARY TO NOTES'
    );

    console.log(
      'SUBJECT:',
      selectedSubject.subjectName
    );

    console.log(
      'SUBJECT ID:',
      subjectId
    );

    console.log(
      'NOTE:',
      note
    );

    console.log(
      '=========================================='
    );

    this.savingNote = true;

    this.cdr.detectChanges();

    this.notesService
      .createNote(note)
      .subscribe({

        next: (response) => {

          console.log(
            'NOTE SAVED SUCCESSFULLY:',
            response
          );

          this.savingNote = false;

          this.showSaveNotesModal = false;

          this.selectedSubjectId = 0;

          this.cdr.detectChanges();

          alert(
            'Summary saved to Notes successfully.'
          );

        },

        error: (err) => {

          console.error(
            'SAVE NOTE ERROR:',
            err
          );

          console.error(
            'SERVER RESPONSE:',
            err?.error
          );

          this.savingNote = false;

          this.cdr.detectChanges();

          if (
            err?.status === 400
          ) {

            alert(
              err?.error?.message ||
              'Invalid subject selected.'
            );

          } else if (
            err?.status === 500
          ) {

            alert(
              'The selected subject does not exist in the database.'
            );

          } else {

            alert(
              'Failed to save summary to Notes.'
            );

          }

        }

      });
  }

  // =====================================================
  // DELETE REPORT
  // =====================================================

  deleteReport(
    id: number
  ): void {

    const summaryId =
      Number(id);

    if (
      !summaryId ||
      summaryId <= 0
    ) {

      console.error(
        'Invalid report ID:',
        id
      );

      alert(
        'Unable to delete this report.'
      );

      return;
    }

    const confirmed =
      confirm(
        'Are you sure you want to delete this report?'
      );

    if (!confirmed) {

      return;
    }

    console.log(
      'DELETING REPORT:',
      summaryId
    );

    this.summaryService
      .deleteSummary(summaryId)
      .subscribe({

        next: () => {

          console.log(
            'REPORT DELETED:',
            summaryId
          );

          /*
           * Remove immediately from UI.
           */

          this.savedReports =
            this.savedReports.filter(
              report =>
                Number(
                  report?.summaryId
                ) !== summaryId
            );

          this.filterReports();

          this.cdr.detectChanges();

          /*
           * Reload from database to confirm
           * actual server state.
           */

          this.loadReports();

          alert(
            'Report deleted successfully.'
          );

        },

        error: (err) => {

          console.error(
            'DELETE REPORT ERROR:',
            err
          );

          console.error(
            'DELETE SERVER RESPONSE:',
            err?.error
          );

          alert(
            err?.error?.message ||
            'Failed to delete report.'
          );

        }

      });
  }

  // =====================================================
  // DOWNLOAD REPORT AS PDF
  // =====================================================

  downloadReport(
    report: any
  ): void {

    console.log(
      '=========================================='
    );

    console.log(
      'DOWNLOAD REPORT REQUEST'
    );

    console.log(
      'REPORT:',
      report
    );

    console.log(
      '=========================================='
    );

    /*
     * Backend returns SummaryId.
     */

    const summaryId =
      Number(
        report?.summaryId
      );

    if (
      !summaryId ||
      summaryId <= 0
    ) {

      console.error(
        'INVALID SUMMARY ID:',
        report?.summaryId
      );

      alert(
        'Unable to download this report because its ID is missing.'
      );

      return;
    }

    console.log(
      'DOWNLOADING SUMMARY ID:',
      summaryId
    );

    this.summaryService
      .downloadSummary(summaryId)
      .subscribe({

        next: (blob: Blob) => {

          console.log(
            'PDF RESPONSE:',
            blob
          );

          console.log(
            'PDF SIZE:',
            blob?.size
          );

          console.log(
            'PDF TYPE:',
            blob?.type
          );

          if (
            !blob ||
            blob.size === 0
          ) {

            alert(
              'The generated PDF is empty.'
            );

            return;
          }

          /*
           * Create PDF blob.
           */

          const pdfBlob =
            new Blob(
              [blob],
              {
                type:
                  'application/pdf'
              }
            );

          const url =
            window.URL.createObjectURL(
              pdfBlob
            );

          /*
           * Create temporary download link.
           */

          const anchor =
            document.createElement(
              'a'
            );

          anchor.href = url;

          /*
           * Create safe filename.
           */

          let safeTitle =
            String(
              report?.title ||
              'AI-Study-Summary'
            )
              .replace(
                /[<>:"/\\|?*]/g,
                ''
              )
              .trim();

          if (!safeTitle) {

            safeTitle =
              'AI-Study-Summary';
          }

          anchor.download =
            `${safeTitle}.pdf`;

          /*
           * Some browsers require the link
           * to actually exist in the document.
           */

          anchor.style.display =
            'none';

          document.body.appendChild(
            anchor
          );

          anchor.click();

          document.body.removeChild(
            anchor
          );

          /*
           * Release browser memory.
           */

          setTimeout(() => {

            window.URL.revokeObjectURL(
              url
            );

          }, 1500);

          console.log(
            'PDF DOWNLOAD COMPLETE'
          );

        },

        error: (err) => {

          console.error(
            '=========================================='
          );

          console.error(
            'PDF DOWNLOAD ERROR'
          );

          console.error(
            'STATUS:',
            err?.status
          );

          console.error(
            'ERROR:',
            err
          );

          console.error(
            'SERVER RESPONSE:',
            err?.error
          );

          console.error(
            '=========================================='
          );

          if (
            err?.status === 404
          ) {

            alert(
              'Report not found. Please refresh the page and try again.'
            );

          } else if (
            err?.status === 401
          ) {

            alert(
              'Your login session has expired. Please login again.'
            );

          } else if (
            err?.status === 500
          ) {

            alert(
              'The server could not generate the PDF. Check the backend console.'
            );

          } else {

            alert(
              'Download failed.'
            );

          }

        }

      });
  }

  // =====================================================
  // RESET FORM
  // =====================================================

  resetForm(): void {

    this.title = '';

    this.originalText = '';

    this.selectedImage = null;

    this.imagePreview = null;

    this.selectedPdf = null;

    this.selectedSource = 'text';

    this.summaryStyle = 'revision';

    this.summaryResult = null;

    this.loading = false;

    this.selectedSubjectId = 0;

    this.showSaveNotesModal = false;

    this.savingNote = false;

    this.cdr.detectChanges();
  }

}

