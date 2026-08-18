
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { SubjectService } from '../../core/services/subject';
import { StudyPlanService } from '../../core/services/study-plan';
import { NotesService } from '../../core/services/notes';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {

  private subjectService = inject(SubjectService);
  private studyPlanService = inject(StudyPlanService);
  private notesService = inject(NotesService);
  private router = inject(Router);
private cdr = inject(ChangeDetectorRef);
  totalSubjects = 0;
  totalStudyPlans = 0;
  totalNotes = 0;
  totalAIChats = 0;

  ngOnInit(): void {

    this.loadDashboard();

  }

  loadDashboard() {

  this.subjectService.getSubjects().subscribe({
    next: (data) => {
      console.log("Subjects:", data);
      this.totalSubjects = data.length;
      this.cdr.detectChanges();
    }
  });

  this.studyPlanService.getStudyPlans().subscribe({
    next: (data) => {
      console.log("Study Plans:", data);
      this.totalStudyPlans = data.length;
      this.cdr.detectChanges();
    }
  });

  this.notesService.getNotes().subscribe({
    next: (data) => {
      console.log("Notes:", data);
      this.totalNotes = data.length;
      this.cdr.detectChanges();
    }
  });

}

  goToSubjects() {
    this.router.navigate(['/subjects']);
  }

  goToNotes() {
    this.router.navigate(['/notes']);
  }

  goToStudyPlans() {
    this.router.navigate(['/study-plans']);
  }

  goToAIChats() {
    this.router.navigate(['/ai-chats']);
  }

}