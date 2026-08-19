import {
  Component,
  OnInit,
  inject,
  ChangeDetectorRef
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { NotesService } from '../../core/services/notes';
import { Note } from '../../core/models/note';

import { Subject } from '../../core/models/subject';
import { SubjectService } from '../../core/services/subject';

@Component({
  selector: 'app-notes',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './notes.html',
  styleUrl: './notes.scss'
})
export class Notes implements OnInit {

  private notesService = inject(NotesService);
  private subjectService = inject(SubjectService);
  private cdr = inject(ChangeDetectorRef);

  notes: Note[] = [];

  subjects: Subject[] = [];

  showAddForm = false;

  selectedNoteId = 0;

  searchText = '';

  showDeleteConfirm = false;

  noteToDelete: Note | null = null;

  newNote = {
    title: '',
    content: '',
    subjectId: 0
  };

  // =====================================================
  // INIT
  // =====================================================

  ngOnInit(): void {

    this.loadNotes();

    this.loadSubjects();

  }

  // =====================================================
  // LOAD NOTES
  // =====================================================

  loadNotes(): void {

    console.log('Loading notes...');

    this.notesService
      .getNotes()
      .subscribe({

        next: (data: Note[]) => {

          console.log('NOTES:', data);

          this.notes = [...data];

          this.cdr.detectChanges();

        },

        error: (err) => {

          console.error(
            'NOTES LOAD ERROR:',
            err
          );

        }

      });

  }

  // =====================================================
  // LOAD SUBJECTS
  // =====================================================

  loadSubjects(): void {

    console.log('Loading subjects...');

    this.subjectService
      .getSubjects()
      .subscribe({

        next: (data: Subject[]) => {

          console.log(
            'SUBJECTS:',
            data
          );

          this.subjects = data;

          this.cdr.detectChanges();

        },

        error: (err) => {

          console.error(
            'SUBJECT LOAD ERROR:',
            err
          );

        }

      });

  }

  // =====================================================
  // SEARCH
  // =====================================================

  get filteredNotes(): Note[] {

    const search =
      this.searchText
        .trim()
        .toLowerCase();

    if (!search) {

      return this.notes;

    }

    return this.notes.filter(note =>

      String(note.title || '')
        .toLowerCase()
        .includes(search)

      ||

      String(note.content || '')
        .toLowerCase()
        .includes(search)

      ||

      this.getSubjectName(
        note.subjectId
      )
        .toLowerCase()
        .includes(search)

    );

  }

  // =====================================================
  // GET SUBJECT NAME
  // =====================================================

  getSubjectName(
    subjectId: number
  ): string {

    const subject =
      this.subjects.find(
        s =>
          Number(s.subjectId) ===
          Number(subjectId)
      );

    return subject
      ? subject.subjectName
      : 'Unknown Subject';

  }

  // =====================================================
  // OPEN ADD FORM
  // =====================================================

  openAddForm(): void {

    this.selectedNoteId = 0;

    this.newNote = {

      title: '',

      content: '',

      subjectId: 0

    };

    this.showAddForm = true;

  }

  // =====================================================
  // SAVE NOTE
  // =====================================================

  saveNote(): void {

    if (
      !this.newNote.title.trim()
      ||
      !this.newNote.content.trim()
      ||
      Number(this.newNote.subjectId) <= 0
    ) {

      alert(
        'Please enter title, content and select a subject.'
      );

      return;

    }

    const note = {

      title:
        this.newNote.title.trim(),

      content:
        this.newNote.content.trim(),

      subjectId:
        Number(this.newNote.subjectId)

    };

    console.log(
      'NOTE TO SAVE:',
      note
    );

    // =================================================
    // CREATE
    // =================================================

    if (this.selectedNoteId === 0) {

      this.notesService
        .createNote(note)
        .subscribe({

          next: (response) => {

            console.log(
              'NOTE CREATED:',
              response
            );

            this.resetForm();

            this.loadNotes();

          },

          error: (err) => {

            console.error(
              'CREATE NOTE ERROR:',
              err
            );

            console.error(
              'CREATE NOTE RESPONSE:',
              err?.error
            );

            alert(
              'Failed to create note.'
            );

          }

        });

      return;

    }

    // =================================================
    // UPDATE
    // =================================================

    this.notesService
      .updateNote(
        this.selectedNoteId,
        note
      )
      .subscribe({

        next: () => {

          console.log(
            'NOTE UPDATED'
          );

          this.resetForm();

          this.loadNotes();

        },

        error: (err) => {

          console.error(
            'UPDATE NOTE ERROR:',
            err
          );

          alert(
            'Failed to update note.'
          );

        }

      });

  }

  // =====================================================
  // EDIT NOTE
  // =====================================================

  editNote(
    note: Note
  ): void {

    this.selectedNoteId =
      note.noteId;

    this.newNote = {

      title:
        note.title,

      content:
        note.content,

      subjectId:
        Number(note.subjectId)

    };

    this.showAddForm = true;

  }

  // =====================================================
  // CONFIRM DELETE
  // =====================================================

  confirmDelete(
    note: Note
  ): void {

    this.noteToDelete = note;

    this.showDeleteConfirm = true;

  }

  // =====================================================
  // DELETE NOTE
  // =====================================================

  deleteNote(): void {

    if (!this.noteToDelete) {

      return;

    }

    const id =
      this.noteToDelete.noteId;

    this.notesService
      .deleteNote(id)
      .subscribe({

        next: () => {

          console.log(
            'NOTE DELETED:',
            id
          );

          this.showDeleteConfirm = false;

          this.noteToDelete = null;

          this.loadNotes();

        },

        error: (err) => {

          console.error(
            'DELETE NOTE ERROR:',
            err
          );

          alert(
            'Failed to delete note.'
          );

        }

      });

  }

  // =====================================================
  // CANCEL DELETE
  // =====================================================

  cancelDelete(): void {

    this.showDeleteConfirm = false;

    this.noteToDelete = null;

  }

  // =====================================================
  // RESET FORM
  // =====================================================

  resetForm(): void {

    this.selectedNoteId = 0;

    this.showAddForm = false;

    this.newNote = {

      title: '',

      content: '',

      subjectId: 0

    };

  }

}