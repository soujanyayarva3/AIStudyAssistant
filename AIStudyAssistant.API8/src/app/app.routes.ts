import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { AIChat } from './features/ai-chat/ai-chat';
import { Dashboard } from './features/dashboard/dashboard';
import { Subjects } from './features/subjects/subjects';
import { Notes } from './features/notes/notes';
import { StudyPlans } from './features/study-plans/study-plans';

import { Quizzes } from './features/quizzes/quizzes';
import { Summaries } from './features/summaries/summaries';
import { Progress } from './features/progress/progress';

import { MainLayout } from './layout/main-layout/main-layout';

export const routes: Routes = [

  // Authentication
  {
    path: '',
    component: Login
  },

  {
    path: 'register',
    component: Register
  },

  // Main Layout
  {
    path: '',
    component: MainLayout,
    children: [
      {
        path: 'dashboard',
        component: Dashboard
      },
      {
        path: 'subjects',
        component: Subjects
      },
      {
        path: 'notes',
        component: Notes
      },
      {
        path: 'study-plans',
        component: StudyPlans
      },
      {
        path: 'ai-chat',
        component: AIChat
      },
      {
        path: 'quizzes',
        component: Quizzes
      },
      {
        path: 'summaries',
        component: Summaries
      },
      {
        path: 'progress',
        component: Progress
      }
    ]
  },

  // Redirect unknown URLs
  {
    path: '**',
    redirectTo: ''
  }

];