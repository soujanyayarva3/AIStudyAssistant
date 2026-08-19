import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';

import {
  ProgressService,
  ProgressData
} from '../../core/services/progress.service';

@Component({
  selector: 'app-progress',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './progress.html',
  styleUrl: './progress.scss'
})
export class Progress implements OnInit {

  private progressService = inject(ProgressService);

  progress?: ProgressData;

  loading = true;

  error = '';

  ngOnInit(): void {
    this.loadProgress();
  }

  loadProgress(): void {

    this.loading = true;
    this.error = '';

    this.progressService.getProgress().subscribe({

      next: (data: ProgressData) => {

        console.log('PROGRESS RESPONSE:', data);

        this.progress = data;

        this.loading = false;
      },

      error: (error: any) => {

        console.error('PROGRESS ERROR:', error);

        this.error = 'Unable to load progress.';

        this.loading = false;
      }

    });
  }
}