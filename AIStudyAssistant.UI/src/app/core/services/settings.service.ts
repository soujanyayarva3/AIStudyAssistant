import { Injectable } from '@angular/core';

export interface StudySettings {
  theme: string;
  responseStyle: string;
  showExamples: boolean;
  dailyStudyGoal: string;
  quizDifficulty: string;
  studyReminders: boolean;
  quizReminders: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class SettingsService {

  private readonly storageKey = 'aiStudySettings';

  private defaultSettings: StudySettings = {
    theme: 'Light',
    responseStyle: 'Balanced',
    showExamples: true,
    dailyStudyGoal: '2 hours',
    quizDifficulty: 'Medium',
    studyReminders: true,
    quizReminders: false
  };

  // ==========================================
  // GET SETTINGS
  // ==========================================

  getSettings(): StudySettings {

    const saved =
      localStorage.getItem(this.storageKey);

    if (!saved) {
      return { ...this.defaultSettings };
    }

    try {

      const parsed =
        JSON.parse(saved);

      return {
        ...this.defaultSettings,
        ...parsed
      };

    } catch {

      console.error(
        'Invalid study settings found.'
      );

      return {
        ...this.defaultSettings
      };
    }
  }


  // ==========================================
  // SAVE SETTINGS
  // ==========================================

  saveSettings(
    settings: StudySettings
  ): void {

    localStorage.setItem(
      this.storageKey,
      JSON.stringify(settings)
    );
  }


  // ==========================================
  // INDIVIDUAL SETTINGS
  // ==========================================

  getDailyStudyGoal(): string {

    return this.getSettings()
      .dailyStudyGoal;

  }


  getQuizDifficulty(): string {

    return this.getSettings()
      .quizDifficulty;

  }


  getResponseStyle(): string {

    return this.getSettings()
      .responseStyle;

  }


  getShowExamples(): boolean {

    return this.getSettings()
      .showExamples;

  }


  getStudyReminders(): boolean {

    return this.getSettings()
      .studyReminders;

  }


  getQuizReminders(): boolean {

    return this.getSettings()
      .quizReminders;

  }


  // ==========================================
  // THEME
  // ==========================================

  getTheme(): string {

    return this.getSettings()
      .theme;

  }
}