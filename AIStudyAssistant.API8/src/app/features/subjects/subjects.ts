import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChangeDetectorRef } from '@angular/core';
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

  showAddModal = false;

  selectedSubjectId = 0;

  newSubject = {
    subjectName: '',
    description: ''
  };

  ngOnInit(): void {
    this.loadSubjects();
  }

  loadSubjects() {
    this.subjectService.getSubjects().subscribe({
      next: (data) => {
        this.subjects = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err);
      }
    });
  }

  addSubject() {

    // UPDATE
    if (this.selectedSubjectId > 0) {

      this.subjectService.updateSubject(
        this.selectedSubjectId,
        this.newSubject
      ).subscribe({

        next: () => {

          alert('Subject Updated Successfully');

          this.resetForm();

        },

        error: (err) => {

          console.error(err);

          alert('Update Failed');

        }

      });

      return;
    }

    // CREATE
    this.subjectService.createSubject(this.newSubject).subscribe({

      next: () => {

        alert('Subject Added Successfully');

        this.resetForm();

      },

      error: (err) => {

        console.error(err);

        alert('Failed to add subject');

      }

    });

  }

  editSubject(subject: Subject) {

    this.selectedSubjectId = subject.subjectId;

    this.newSubject = {
      subjectName: subject.subjectName,
      description: subject.description
    };

    this.showAddModal = true;

  }

  deleteSubject(id: number) {

    if (!confirm('Are you sure you want to delete this subject?')) {
      return;
    }

    this.subjectService.deleteSubject(id).subscribe({

      next: () => {

        alert('Subject Deleted Successfully');

        this.loadSubjects();

      },

      error: (err) => {

        console.error(err);

        alert('Delete Failed');

      }

    });

  }

  resetForm() {

    this.showAddModal = false;

    this.selectedSubjectId = 0;

    this.newSubject = {
      subjectName: '',
      description: ''
    };

    this.loadSubjects();

  }

}