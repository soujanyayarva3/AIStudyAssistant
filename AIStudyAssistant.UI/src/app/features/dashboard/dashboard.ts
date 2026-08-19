
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import {
  Component,
  OnInit,
  inject,
  ChangeDetectorRef
} from '@angular/core';

import { SubjectService } from '../../core/services/subject';
import { StudyPlanService } from '../../core/services/study-plan';
import { NotesService } from '../../core/services/notes';
import { AIChatService } from '../../core/services/ai-chat.service';

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
  private aiChatService = inject(AIChatService);

  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  totalSubjects = 0;
  totalStudyPlans = 0;
  totalNotes = 0;
  totalAIChats = 0;

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {

    // =========================
    // SUBJECTS
    // =========================

    this.subjectService.getSubjects().subscribe({

      next: (data) => {

        console.log('Subjects:', data);

        this.totalSubjects = data.length;

        this.cdr.detectChanges();
      },

      error: (error) => {

        console.error(
          'Subjects error:',
          error
        );

      }

    });


    // =========================
    // STUDY PLANS
    // =========================

    this.studyPlanService.getStudyPlans().subscribe({

      next: (data) => {

        console.log(
          'Study Plans:',
          data
        );

        this.totalStudyPlans = data.length;

        this.cdr.detectChanges();
      },

      error: (error) => {

        console.error(
          'Study Plans error:',
          error
        );

      }

    });


    // =========================
    // NOTES
    // =========================

    this.notesService.getNotes().subscribe({

      next: (data) => {

        console.log(
          'Notes:',
          data
        );

        this.totalNotes = data.length;

        this.cdr.detectChanges();
      },

      error: (error) => {

        console.error(
          'Notes error:',
          error
        );

      }

    });


    // =========================
    // AI CHATS
    // =========================

    this.aiChatService.getAIChats().subscribe({

      next: (data) => {

        console.log(
          'AI Chats:',
          data
        );

        this.totalAIChats = data.length;

        this.cdr.detectChanges();
      },

      error: (error) => {

        console.error(
          'AI Chats error:',
          error
        );

      }

    });

  }


  // =========================
  // NAVIGATION
  // =========================

  goToSubjects(): void {

    this.router.navigate([
      '/subjects'
    ]);

  }


  goToNotes(): void {

    this.router.navigate([
      '/notes'
    ]);

  }


  goToStudyPlans(): void {

    this.router.navigate([
      '/study-plans'
    ]);

  }


  goToAIChats(): void {

    // Correct route:
    // ai-chat (NOT ai-chats)

    this.router.navigate([
      '/ai-chat'
    ]);

  }

}

