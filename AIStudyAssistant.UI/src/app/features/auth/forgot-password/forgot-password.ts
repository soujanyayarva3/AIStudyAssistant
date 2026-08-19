import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink
  ],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.scss'
})
export class ForgotPassword implements OnInit {

  email = '';
  token = '';
  newPassword = '';
  confirmPassword = '';

  message = '';
  errorMessage = '';

  isLoading = false;
  resetMode = false;

  private http = inject(HttpClient);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  private apiUrl = `${environment.apiUrl}/Auth`;

  ngOnInit(): void {

    this.route.queryParamMap.subscribe(params => {

      const email = params.get('email');
      const token = params.get('token');

      if (email && token) {

        this.email = email;
        this.token = token;

        this.resetMode = true;

        this.message =
          'Enter your new password below.';
      }
    });
  }

  forgotPassword(): void {

    this.message = '';
    this.errorMessage = '';

    if (!this.email.trim()) {
      this.errorMessage = 'Please enter your email.';
      return;
    }

    this.isLoading = true;

    this.http.post(
      `${this.apiUrl}/forgot-password`,
      {
        email: this.email
      },
      {
        responseType: 'text'
      }
    ).subscribe({

      next: (response) => {

        this.isLoading = false;

        this.message =
          'If an account with this email exists, a password reset link has been sent.';
      },

      error: (err) => {

        console.error(
          'FORGOT PASSWORD ERROR:',
          err
        );

        this.isLoading = false;

        this.errorMessage =
          err?.error ||
          'Unable to send password reset email.';
      }
    });
  }

  resetPassword(): void {

    this.message = '';
    this.errorMessage = '';

    if (!this.email.trim()) {
      this.errorMessage = 'Invalid password reset link.';
      return;
    }

    if (!this.token.trim()) {
      this.errorMessage = 'Invalid or missing reset token.';
      return;
    }

    if (!this.newPassword.trim()) {
      this.errorMessage = 'Please enter a new password.';
      return;
    }

    if (this.newPassword !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match.';
      return;
    }

    this.isLoading = true;

    this.http.post(
      `${this.apiUrl}/reset-password`,
      {
        email: this.email,
        token: this.token,
        newPassword: this.newPassword
      },
      {
        responseType: 'text'
      }
    ).subscribe({

      next: (response) => {

        this.isLoading = false;

        this.message =
          'Password reset successfully. Redirecting to login...';

        this.newPassword = '';
        this.confirmPassword = '';

        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 1500);
      },

      error: (err) => {

        console.error(
          'RESET PASSWORD ERROR:',
          err
        );

        this.isLoading = false;

        this.errorMessage =
          err?.error ||
          'Unable to reset password.';
      }
    });
  }
}