import {
  Component,
  OnInit,
  inject
} from '@angular/core';

import { FormsModule } from '@angular/forms';

import {
  SettingsService,
  StudySettings
} from '../../core/services/settings.service';


@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './settings.html',
  styleUrl: './settings.scss'
})
export class Settings implements OnInit {

  private settingsService =
    inject(SettingsService);


  // ==========================================
  // SETTINGS
  // ==========================================

  theme = 'Light';

  responseStyle = 'Balanced';

  showExamples = true;

  dailyStudyGoal = '2 hours';

  quizDifficulty = 'Medium';

  studyReminders = true;

  quizReminders = false;


  // ==========================================
  // INIT
  // ==========================================

  ngOnInit(): void {

    this.loadSettings();

    this.applyTheme();

  }


  // ==========================================
  // SAVE
  // ==========================================

  saveSettings(): void {

    const settings: StudySettings = {

      theme:
        this.theme,

      responseStyle:
        this.responseStyle,

      showExamples:
        this.showExamples,

      dailyStudyGoal:
        this.dailyStudyGoal,

      quizDifficulty:
        this.quizDifficulty,

      studyReminders:
        this.studyReminders,

      quizReminders:
        this.quizReminders

    };


    this.settingsService
      .saveSettings(settings);


    this.applyTheme();


    alert(
      'Settings saved successfully!'
    );

  }


  // ==========================================
  // APPLY THEME
  // ==========================================

  applyTheme(): void {

    const root =
      document.documentElement;

    let darkMode = false;


    if (this.theme === 'Dark') {

      darkMode = true;

    }

    else if (
      this.theme === 'System Default'
    ) {

      darkMode =
        window.matchMedia(
          '(prefers-color-scheme: dark)'
        ).matches;

    }


    if (darkMode) {

      root.style.setProperty(
        '--app-bg',
        '#0f172a'
      );

      root.style.setProperty(
        '--app-card',
        '#1e293b'
      );

      root.style.setProperty(
        '--app-text',
        '#e2e8f0'
      );

      root.style.setProperty(
        '--app-muted',
        '#94a3b8'
      );

      root.style.setProperty(
        '--app-border',
        '#475569'
      );

      root.style.setProperty(
        '--app-input',
        '#1e293b'
      );


      document.body.style.backgroundColor =
        '#0f172a';

      document.body.style.color =
        '#e2e8f0';

    }

    else {

      root.style.setProperty(
        '--app-bg',
        '#f8fafc'
      );

      root.style.setProperty(
        '--app-card',
        '#ffffff'
      );

      root.style.setProperty(
        '--app-text',
        '#1e293b'
      );

      root.style.setProperty(
        '--app-muted',
        '#64748b'
      );

      root.style.setProperty(
        '--app-border',
        '#e2e8f0'
      );

      root.style.setProperty(
        '--app-input',
        '#ffffff'
      );


      document.body.style.backgroundColor =
        '#f8fafc';

      document.body.style.color =
        '#1e293b';

    }

  }


  // ==========================================
  // LOAD SETTINGS
  // ==========================================

  loadSettings(): void {

    const settings =
      this.settingsService
        .getSettings();


    this.theme =
      settings.theme;

    this.responseStyle =
      settings.responseStyle;

    this.showExamples =
      settings.showExamples;

    this.dailyStudyGoal =
      settings.dailyStudyGoal;

    this.quizDifficulty =
      settings.quizDifficulty;

    this.studyReminders =
      settings.studyReminders;

    this.quizReminders =
      settings.quizReminders;

  }

}