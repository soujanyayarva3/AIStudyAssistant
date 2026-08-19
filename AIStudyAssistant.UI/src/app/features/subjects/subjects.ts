import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { SubjectService } from '../../core/services/subject';
import { Subject } from '../../core/models/subject';

@Component({
  selector: 'app-subjects',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './subjects.html',
  styleUrl: './subjects.scss'
})
export class Subjects implements OnInit {

  private subjectService = inject(SubjectService);
  private cdr = inject(ChangeDetectorRef);

  subjects: Subject[] = [];

  // Search
  searchText = '';

  // Add/Edit modal
  showAddModal = false;
  selectedSubjectId = 0;

  // Delete confirmation
  showDeleteConfirm = false;
  subjectToDelete: Subject | null = null;

  // Form
  newSubject = {
    subjectName: '',
    description: ''
  };

  ngOnInit(): void {
    this.loadSubjects();
  }

  loadSubjects(): void {

    this.subjectService.getSubjects().subscribe({

      next: (data: Subject[]) => {

        console.log('SUBJECTS:', data);

        this.subjects = data;

        this.cdr.detectChanges();
      },

      error: (err) => {

        console.error('SUBJECT LOAD ERROR:', err);

      }

    });

  }

  // Filter subjects according to search text
  get filteredSubjects(): Subject[] {

    const search = this.searchText.trim().toLowerCase();

    if (!search) {
      return this.subjects;
    }

    return this.subjects.filter(subject =>
      subject.subjectName.toLowerCase().includes(search) ||
      subject.description.toLowerCase().includes(search)
    );

  }

  // Open Add Subject form
  openAddForm(): void {

    this.selectedSubjectId = 0;

    this.newSubject = {
      subjectName: '',
      description: ''
    };

    this.showAddModal = true;
  }

  // Add or Update
  saveSubject(): void {

    if (!this.newSubject.subjectName.trim()) {
      return;
    }

    // UPDATE
    if (this.selectedSubjectId > 0) {

      this.subjectService.updateSubject(
        this.selectedSubjectId,
        this.newSubject
      ).subscribe({

        next: () => {

          this.closeForm();

          this.loadSubjects();

        },

        error: (err) => {

          console.error('UPDATE SUBJECT ERROR:', err);

        }

      });

      return;
    }

    // CREATE
    this.subjectService.createSubject(this.newSubject).subscribe({

      next: () => {

        this.closeForm();

        this.loadSubjects();

      },

      error: (err) => {

        console.error('CREATE SUBJECT ERROR:', err);

      }

    });

  }

  // Edit
  editSubject(subject: Subject): void {

    this.selectedSubjectId = subject.subjectId;

    this.newSubject = {
      subjectName: subject.subjectName,
      description: subject.description
    };

    this.showAddModal = true;
  }

  // Open delete confirmation
  confirmDelete(subject: Subject): void {

    this.subjectToDelete = subject;

    this.showDeleteConfirm = true;

  }

  // Actually delete
  deleteSubject(): void {

    if (!this.subjectToDelete) {
      return;
    }

    const id = this.subjectToDelete.subjectId;

    this.subjectService.deleteSubject(id).subscribe({

      next: () => {

        this.showDeleteConfirm = false;
        this.subjectToDelete = null;

        this.loadSubjects();

      },

      error: (err) => {

        console.error('DELETE SUBJECT ERROR:', err);

      }

    });

  }

  // Cancel delete
  cancelDelete(): void {

    this.showDeleteConfirm = false;
    this.subjectToDelete = null;

  }

  // Close Add/Edit form
  closeForm(): void {

    this.showAddModal = false;

    this.selectedSubjectId = 0;

    this.newSubject = {
      subjectName: '',
      description: ''
    };

  }

}