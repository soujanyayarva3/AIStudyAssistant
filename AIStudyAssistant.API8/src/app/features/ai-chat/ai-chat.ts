import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AIChatService } from '../../core/services/ai-chat';
import { AIChatModel } from '../../core/models/ai-chat';

@Component({
  selector: 'app-ai-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ai-chat.html',
  styleUrl: './ai-chat.scss'
})
export class AIChat implements OnInit {

  private aiService = inject(AIChatService);

  chats: AIChatModel[] = [];

  question = '';

  ngOnInit(): void {
    this.loadChats();
  }

  loadChats() {
    this.aiService.getChats().subscribe({
      next: data => this.chats = data,
      error: err => console.error(err)
    });
  }

  askAI() {

    if(!this.question.trim()) return;

    const request = {

      question: this.question,

      response: "",

      userId: 12

    };

    this.aiService.sendQuestion(request).subscribe({

      next: () => {

        this.question = '';

        this.loadChats();

      },

      error: err => console.error(err)

    });

  }

}