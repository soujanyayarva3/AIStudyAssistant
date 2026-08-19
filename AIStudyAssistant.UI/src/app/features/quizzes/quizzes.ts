import {
  Component,
  inject,
  ChangeDetectorRef
} from '@angular/core';

import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import {
  QuizService,
  Quiz
} from '../../services/quiz';

@Component({
  selector: 'app-quizzes',
  standalone: true,

  imports: [
    FormsModule,
    CommonModule
  ],

  templateUrl: './quizzes.html',
  styleUrl: './quizzes.scss'
})
export class Quizzes {

  private quizService = inject(QuizService);
  private cdr = inject(ChangeDetectorRef);


  // =====================================================
  // QUIZ SETTINGS
  // =====================================================

  topic = '';

  quizDifficulty = 'Medium';

  responseStyle = 'Balanced';

  showExamples = true;


  // =====================================================
  // QUIZ DATA
  // =====================================================

  questions: Quiz[] = [];

  userAnswers: string[] = [];

  currentQuestionIndex = 0;

  loading = false;

  quizStarted = false;

  quizCompleted = false;

  score = 0;

  showGenerator = true;

  totalQuestions = 5;


  // =====================================================
  // INIT
  // =====================================================

  constructor() {

    this.loadStudyPreferences();

  }


  // =====================================================
  // LOAD STUDY PREFERENCES
  // =====================================================

  loadStudyPreferences(): void {

    const savedSettings =
      localStorage.getItem('aiStudySettings');

    if (!savedSettings) {

      console.log(
        'No saved study preferences found. Using defaults.'
      );

      return;

    }

    try {

      const settings =
        JSON.parse(savedSettings);

      this.quizDifficulty =
        settings.quizDifficulty ??
        'Medium';

      this.responseStyle =
        settings.responseStyle ??
        'Balanced';

      this.showExamples =
        settings.showExamples ??
        true;


      console.log(
        '========== QUIZ SETTINGS =========='
      );

      console.log(
        'Quiz Difficulty:',
        this.quizDifficulty
      );

      console.log(
        'Response Style:',
        this.responseStyle
      );

      console.log(
        'Show Examples:',
        this.showExamples
      );

    } catch (error) {

      console.error(
        'Unable to load quiz preferences:',
        error
      );

      this.quizDifficulty = 'Medium';

      this.responseStyle = 'Balanced';

      this.showExamples = true;

    }

  }


  // =====================================================
  // GENERATE QUIZ
  // =====================================================

  generateQuiz(): void {

    if (!this.topic.trim()) {

      alert(
        'Please enter a topic.'
      );

      return;

    }


    // Reload settings before generating
    // so changes made in Settings are reflected.

    this.loadStudyPreferences();


    // =================================================
    // RESET PREVIOUS QUIZ
    // =================================================

    this.loading = true;

    this.questions = [];

    this.userAnswers = [];

    this.currentQuestionIndex = 0;

    this.quizCompleted = false;

    this.quizStarted = false;

    this.score = 0;

    this.showGenerator = true;


    this.cdr.detectChanges();


    console.log(
      '========== GENERATING QUIZ =========='
    );

    console.log(
      'Topic:',
      this.topic
    );

    console.log(
      'Difficulty:',
      this.quizDifficulty
    );

    console.log(
      'Response Style:',
      this.responseStyle
    );

    console.log(
      'Show Examples:',
      this.showExamples
    );


    // =================================================
    // BACKEND REQUEST
    // =================================================

    /*
      We pass the difficulty if QuizService supports
      the second parameter.

      If your current QuizService only accepts:

          generateQuiz(topic)

      use the fallback call below.
    */

    this.quizService
      .generateQuiz(
        this.topic,
        this.quizDifficulty
      )
      .subscribe({

        next: (response: Quiz[]) => {

          console.log(
            '========== QUIZ GENERATED =========='
          );

          console.log(
            'Questions received:',
            response
          );


          // =================================================
          // EMPTY RESPONSE
          // =================================================

          if (
            !response ||
            response.length === 0
          ) {

            this.loading = false;

            alert(
              'No questions were generated.'
            );

            this.cdr.detectChanges();

            return;

          }


          // =================================================
          // TAKE MAXIMUM 5 QUESTIONS
          // =================================================

          this.questions =
            response.slice(
              0,
              this.totalQuestions
            );


          // =================================================
          // CREATE ANSWER SLOTS
          // =================================================

          this.userAnswers =
            new Array(
              this.questions.length
            ).fill('');


          this.currentQuestionIndex = 0;

          this.loading = false;

          this.quizStarted = true;

          this.quizCompleted = false;

          this.showGenerator = false;


          this.cdr.detectChanges();


          console.log(
            '========== QUIZ READY =========='
          );

          console.log(
            'Total questions:',
            this.questions.length
          );

        },


        error: (error: unknown) => {

          console.error(
            'QUIZ ERROR:',
            error
          );


          this.loading = false;

          this.quizStarted = false;

          this.quizCompleted = false;

          this.showGenerator = true;


          this.cdr.detectChanges();


          alert(
            'Unable to generate the quiz. Please make sure the backend and Ollama are running.'
          );

        }

      });

  }


  // =====================================================
  // CURRENT QUESTION
  // =====================================================

  get currentQuestion(): Quiz | undefined {

    return this.questions[
      this.currentQuestionIndex
    ];

  }


  // =====================================================
  // SELECTED ANSWER
  // =====================================================

  get selectedAnswer(): string {

    return (
      this.userAnswers[
        this.currentQuestionIndex
      ] || ''
    );

  }


  set selectedAnswer(
    value: string
  ) {

    this.userAnswers[
      this.currentQuestionIndex
    ] = value;

  }


  // =====================================================
  // PROGRESS
  // =====================================================

  get progressPercentage(): number {

    if (
      this.questions.length === 0
    ) {

      return 0;

    }


    return (
      (
        (this.currentQuestionIndex + 1) /
        this.questions.length
      ) * 100
    );

  }


  // =====================================================
  // NEXT QUESTION
  // =====================================================

  nextQuestion(): void {

    if (!this.selectedAnswer) {

      alert(
        'Please select an answer before continuing.'
      );

      return;

    }


    this.userAnswers[
      this.currentQuestionIndex
    ] = this.selectedAnswer;


    // =================================================
    // LAST QUESTION
    // =================================================

    if (
      this.currentQuestionIndex ===
      this.questions.length - 1
    ) {

      this.submitQuiz();

      return;

    }


    this.currentQuestionIndex++;

  }


  // =====================================================
  // PREVIOUS QUESTION
  // =====================================================

  previousQuestion(): void {

    if (
      this.currentQuestionIndex > 0
    ) {

      this.currentQuestionIndex--;

    }

  }


  // =====================================================
  // SUBMIT QUIZ
  // =====================================================

  submitQuiz(): void {

    this.score = 0;


    this.questions.forEach(
      (question, index) => {

        if (
          this.userAnswers[index] ===
          question.correctAnswer
        ) {

          this.score++;

        }

      }
    );


    this.quizCompleted = true;

    this.quizStarted = false;


    console.log(
      '========== QUIZ COMPLETED =========='
    );

    console.log(
      'Score:',
      this.score
    );

    console.log(
      'Total:',
      this.questions.length
    );

    console.log(
      'Percentage:',
      this.scorePercentage
    );


    this.cdr.detectChanges();

  }


  // =====================================================
  // RESTART
  // =====================================================

  restartQuiz(): void {

    this.questions = [];

    this.userAnswers = [];

    this.currentQuestionIndex = 0;

    this.quizCompleted = false;

    this.quizStarted = false;

    this.score = 0;

    this.showGenerator = true;

    this.loading = false;


    // Reload latest settings
    this.loadStudyPreferences();


    this.cdr.detectChanges();

  }


  // =====================================================
  // SCORE PERCENTAGE
  // =====================================================

  get scorePercentage(): number {

    if (
      this.questions.length === 0
    ) {

      return 0;

    }


    return Math.round(
      (
        this.score /
        this.questions.length
      ) * 100
    );

  }


  // =====================================================
  // RESULT MESSAGE
  // =====================================================

  get resultMessage(): string {

    if (
      this.scorePercentage >= 80
    ) {

      return 'Excellent work! 🎉';

    }


    if (
      this.scorePercentage >= 60
    ) {

      return 'Good job! Keep improving. 👍';

    }


    if (
      this.scorePercentage >= 40
    ) {

      return 'Nice attempt! Review the answers and try again. 📚';

    }


    return 'Keep practicing. You can improve! 💪';

  }

}