import { CommonModule } from '@angular/common';

import {
  Component,
  inject,
  OnInit,
  ViewChild,
  ElementRef,
  AfterViewChecked,
  ChangeDetectorRef
} from '@angular/core';

import { FormsModule } from '@angular/forms';

import { AIChatService } from '../../core/services/ai-chat.service';
import { ConversationService } from '../../core/services/conversation.service';

@Component({
  selector: 'app-ai-chat',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './ai-chat.html',
  styleUrl: './ai-chat.scss'
})
export class AIChat
  implements OnInit, AfterViewChecked {

  // =====================================================
  // SERVICES
  // =====================================================

  private aiChatService =
    inject(AIChatService);

  private conversationService =
    inject(ConversationService);

  private cdr =
    inject(ChangeDetectorRef);


  // =====================================================
  // CHAT CONTAINER
  // =====================================================

  @ViewChild('chatContainer')
  chatContainer!: ElementRef;


  // =====================================================
  // SIDEBAR
  // =====================================================

  conversations: any[] = [];

  filteredConversations: any[] = [];

  conversationSearch = '';


  // =====================================================
  // CURRENT CONVERSATION
  // =====================================================

  currentConversationId = 0;


  // =====================================================
  // MESSAGES
  // =====================================================

  messages: any[] = [];


  // =====================================================
  // INPUT
  // =====================================================

  question = '';


  // =====================================================
  // LOADING
  // =====================================================

  isLoading = false;


  // =====================================================
  // INIT
  // =====================================================

  ngOnInit(): void {

    console.log(
      'AI CHAT INITIALIZED'
    );

    this.loadHistory();

  }


  // =====================================================
  // AFTER VIEW CHECKED
  // =====================================================

  ngAfterViewChecked(): void {

    this.scrollToBottom();

  }


  // =====================================================
  // SCROLL TO BOTTOM
  // =====================================================

  private scrollToBottom(): void {

    try {

      if (this.chatContainer) {

        this.chatContainer.nativeElement.scrollTop =
          this.chatContainer.nativeElement.scrollHeight;

      }

    } catch {

      // Ignore scroll errors

    }

  }


  // =====================================================
  // LOAD AI SETTINGS
  // =====================================================

  private getAISettings(): {
    responseStyle: string;
    showExamples: boolean;
  } {

    const savedSettings =
      localStorage.getItem('aiStudySettings');

    if (!savedSettings) {

      return {
        responseStyle: 'Balanced',
        showExamples: true
      };

    }

    try {

      const settings =
        JSON.parse(savedSettings);

      return {

        responseStyle:
          settings.responseStyle ??
          'Balanced',

        showExamples:
          settings.showExamples ??
          true

      };

    } catch {

      console.error(
        'Unable to read AI settings.'
      );

      return {
        responseStyle: 'Balanced',
        showExamples: true
      };

    }
  }


  // =====================================================
  // LOAD CHAT HISTORY
  // =====================================================

  loadHistory(): void {

    console.log(
      'Loading conversation history...'
    );

    this.conversationService
      .getHistory()
      .subscribe({

        next: (data) => {

          console.log(
            'CONVERSATION HISTORY:',
            data
          );

          this.conversations =
            data || [];

          this.filterConversations();

          this.cdr.detectChanges();

        },

        error: (err) => {

          console.error(
            'HISTORY LOAD ERROR:',
            err
          );

          this.cdr.detectChanges();

        }

      });

  }


  // =====================================================
  // SEARCH CONVERSATIONS
  // =====================================================

  filterConversations(): void {

    const search =
      this.conversationSearch
        .trim()
        .toLowerCase();


    if (!search) {

      this.filteredConversations = [
        ...this.conversations
      ];

      this.cdr.detectChanges();

      return;

    }


    this.filteredConversations =
      this.conversations.filter(
        item => {

          const title =
            (item.title || '')
              .toString()
              .toLowerCase();

          return title.includes(search);

        }
      );


    this.cdr.detectChanges();

  }


  // =====================================================
  // CLEAR CHAT SEARCH
  // =====================================================

  clearConversationSearch(): void {

    this.conversationSearch = '';

    this.filterConversations();

  }


  // =====================================================
  // LOAD EXISTING CONVERSATION
  // =====================================================

  loadConversation(
    conversationId: number
  ): void {

    console.log(
      'Loading conversation:',
      conversationId
    );

    this.currentConversationId =
      conversationId;

    this.isLoading = true;

    this.cdr.detectChanges();


    this.aiChatService
      .getConversationMessages(
        conversationId
      )
      .subscribe({

        next: (data) => {

          console.log(
            'CONVERSATION MESSAGES:',
            data
          );

          this.messages =
            data || [];

          this.isLoading = false;

          this.cdr.detectChanges();

        },

        error: (err) => {

          console.error(
            'CONVERSATION LOAD ERROR:',
            err
          );

          this.isLoading = false;

          this.cdr.detectChanges();

        }

      });

  }


  // =====================================================
  // NEW CHAT
  // =====================================================

  newChat(): void {

    console.log(
      'STARTING NEW CHAT'
    );

    this.currentConversationId = 0;

    this.messages = [];

    this.question = '';

    this.isLoading = false;

    this.cdr.detectChanges();

  }


  // =====================================================
  // ASK AI
  // =====================================================

  askAI(): void {

    // Prevent empty question
    if (!this.question.trim()) {

      return;

    }


    // Prevent duplicate requests
    if (this.isLoading) {

      console.log(
        'AI REQUEST ALREADY IN PROGRESS'
      );

      return;

    }


    // =================================================
    // SAVE QUESTION
    // =================================================

    const userQuestion =
      this.question.trim();


    // =================================================
    // CLEAR INPUT
    // =================================================

    this.question = '';


    // =================================================
    // START LOADING
    // =================================================

    this.isLoading = true;

    this.cdr.detectChanges();


    console.log(
      'AI LOADING STARTED'
    );

    console.log(
      'QUESTION:',
      userQuestion
    );


    // =================================================
    // READ SETTINGS
    // =================================================

    const aiSettings =
      this.getAISettings();


    console.log(
      'AI SETTINGS:',
      aiSettings
    );


    // =================================================
    // SEND REQUEST
    // =================================================

    this.aiChatService
      .sendMessage({

        conversationId:
          this.currentConversationId,

        question:
          userQuestion,

        response: '',

        userId: 0,

        // Settings
        responseStyle:
          aiSettings.responseStyle,

        showExamples:
          aiSettings.showExamples

      })
      .subscribe({

        // =================================================
        // SUCCESS
        // =================================================

        next: (chat) => {

          console.log(
            'AI RESPONSE RECEIVED:',
            chat
          );


          // =================================================
          // CHECK RESPONSE
          // =================================================

          if (!chat) {

            console.error(
              'AI RESPONSE IS EMPTY'
            );

            this.isLoading = false;

            this.cdr.detectChanges();

            return;

          }


          console.log(
            'AI ANSWER:',
            chat.response
          );


          // =================================================
          // SET NEW CONVERSATION ID
          // =================================================

          if (
            this.currentConversationId === 0
          ) {

            this.currentConversationId =
              chat.conversationId;

            console.log(
              'NEW CONVERSATION ID:',
              this.currentConversationId
            );

          }


          // =================================================
          // ADD MESSAGE
          // =================================================

          this.messages = [
            ...this.messages,
            chat
          ];


          // =================================================
          // STOP LOADING
          // =================================================

          this.isLoading = false;


          // =================================================
          // REFRESH HISTORY
          // =================================================

          this.loadHistory();


          // =================================================
          // FORCE UI UPDATE
          // =================================================

          this.cdr.detectChanges();


          console.log(
            'AI LOADING STOPPED'
          );

          console.log(
            'MESSAGES:',
            this.messages
          );

        },


        // =================================================
        // ERROR
        // =================================================

        error: (err) => {

          console.error(
            'AI CHAT ERROR:',
            err
          );

          this.isLoading = false;

          this.cdr.detectChanges();

          alert(
            'Unable to get AI response. Please try again.'
          );

        },


        // =================================================
        // COMPLETE
        // =================================================

        complete: () => {

          console.log(
            'AI HTTP REQUEST COMPLETED'
          );

          this.isLoading = false;

          this.cdr.detectChanges();

        }

      });

  }


  // =====================================================
  // SEND MESSAGE WITH ENTER
  // =====================================================

  sendMessage(
    event: Event
  ): void {

    const keyboardEvent =
      event as KeyboardEvent;


    // Shift + Enter = new line
    if (
      keyboardEvent.shiftKey
    ) {

      return;

    }


    // Enter = send
    keyboardEvent.preventDefault();

    this.askAI();

  }

}