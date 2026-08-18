import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../../services/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class Register {

  fullName = '';
  email = '';
  password = '';
  confirmPassword = '';

  private authService = inject(Auth);
  private router = inject(Router);

  register() {

    if (this.password !== this.confirmPassword) {
      alert('Passwords do not match');
      return;
    }

    const registerData = {
      fullName: this.fullName,
      email: this.email,
      password: this.password
    };

    this.authService.register(registerData).subscribe({

      next: (response: any) => {

        alert(response);

        this.router.navigate(['/']);

      },

      error: (err) => {

        console.error(err);

        alert('Registration Failed');

      }

    });

  }

}