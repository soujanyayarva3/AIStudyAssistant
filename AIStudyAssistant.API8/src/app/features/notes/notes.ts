import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { NotesService } from '../../core/services/notes';
import { Note } from '../../core/models/note';

import { Subject } from '../../core/models/subject';
import { SubjectService } from '../../core/services/subject';

@Component({
  selector: 'app-notes',
  standalone: true,
  imports: [CommonModule, FormsModule],
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

  newNote = {
    title: '',
    content: '',
    subjectId: 0
  };

  ngOnInit(): void {
    this.loadNotes();
    this.loadSubjects();
  }

  loadNotes() {
    this.notesService.getNotes().subscribe({
      next: (data) => {
        this.notes = [...data];
        this.cdr.detectChanges();
      },
      error: (err) => console.error(err)
    });
  }

  loadSubjects() {
    this.subjectService.getSubjects().subscribe({
      next: (data) => {
        this.subjects = data;
      },
      error: (err) => console.error(err)
    });
  }

  saveNote() {

    const note = {
      title: this.newNote.title,
      content: this.newNote.content,
      subjectId: this.newNote.subjectId
    };

    if (this.selectedNoteId === 0) {

      this.notesService.createNote(note).subscribe({
        next: () => {
          alert('Note Added Successfully');
          this.resetForm();
          this.loadNotes();
        },
        error: (err) => {
          console.error(err);
          alert('Failed to add note');
        }
      });

    } else {

      this.notesService.updateNote(this.selectedNoteId, note).subscribe({
        next: () => {
          alert('Note Updated Successfully');
          this.resetForm();
          this.loadNotes();
        },
        error: (err) => {
          console.error(err);
          alert('Failed to update note');
        }
      });

    }

  }

  editNote(note: Note) {

    this.selectedNoteId = note.noteId;

    this.newNote = {
      title: note.title,
      content: note.content,
      subjectId: note.subjectId
    };

    this.showAddForm = true;

  }

  deleteNote(id: number) {

    if (!confirm('Are you sure you want to delete this note?')) {
      return;
    }

    this.notesService.deleteNote(id).subscribe({
      next: () => {
        alert('Note Deleted Successfully');
        this.loadNotes();
      },
      error: (err) => {
        console.error(err);
        alert('Failed to delete note');
      }
    });

  }

  resetForm() {

    this.selectedNoteId = 0;

    this.showAddForm = false;

    this.newNote = {
      title: '',
      content: '',
      subjectId: 0
    };

  }

}