
import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {

  email = '';
  password = '';

  errorMessage = '';
  isLoading = false;

  private authService = inject(Auth);
  private router = inject(Router);

  login(): void {

    this.errorMessage = '';

    if (
      !this.email.trim() ||
      !this.password.trim()
    ) {

      this.errorMessage =
        'Please enter your email and password.';

      return;
    }

    this.isLoading = true;

    const loginData = {
      email: this.email,
      password: this.password
    };

    this.authService.login(loginData).subscribe({

      next: (response) => {

        console.log(
          'LOGIN SUCCESS:',
          response
        );

        localStorage.setItem(
          'token',
          response.token
        );

        this.isLoading = false;

        this.router.navigate([
          '/dashboard'
        ]);

      },

      error: (err) => {

        console.error(
          'LOGIN ERROR:',
          err
        );

        this.isLoading = false;

        this.errorMessage =
          'Login failed. Please check your email and password.';

      }

    });

  }

}

