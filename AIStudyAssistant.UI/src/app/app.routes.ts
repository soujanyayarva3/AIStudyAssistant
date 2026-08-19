import { Routes } from '@angular/router';

import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { ForgotPassword } from './features/auth/forgot-password/forgot-password';

import { AIChat } from './features/ai-chat/ai-chat';
import { Dashboard } from './features/dashboard/dashboard';
import { Subjects } from './features/subjects/subjects';
import { Notes } from './features/notes/notes';
import { StudyPlans } from './features/study-plans/study-plans';
import { Quizzes } from './features/quizzes/quizzes';
import { Summaries } from './features/summaries/summaries';
import { Progress } from './features/progress/progress';
import { About } from './features/about/about';
import { Settings } from './features/settings/settings';

import { MainLayout } from './layout/main-layout/main-layout';

import { authGuard } from './gaurds/auth.guard';

export const routes: Routes = [

  // =========================
  // PUBLIC ROUTES
  // =========================

  {
    path: '',
    component: Login
  },

  {
    path: 'register',
    component: Register
  },

  {
    path: 'forgot-password',
    component: ForgotPassword
  },

  // =========================
  // PROTECTED ROUTES
  // =========================

  {
    path: '',
    component: MainLayout,
    canActivate: [authGuard],

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
      },

      // About page
      {
        path: 'about',
        component: About
      },

      // Settings page
      {
        path: 'settings',
        component: Settings
      }

    ]
  },

  // =========================
  // UNKNOWN ROUTES
  // =========================

  {
    path: '**',
    redirectTo: ''
  }

];