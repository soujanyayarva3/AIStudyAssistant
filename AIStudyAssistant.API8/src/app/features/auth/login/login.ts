import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {

  email = '';
  password = '';

  private authService = inject(Auth);
  private router = inject(Router);

  login() {

    const loginData = {
      email: this.email,
      password: this.password
    };

    this.authService.login(loginData).subscribe({

      next: (response: any) => {

  console.log("LOGIN RESPONSE:", response);

  localStorage.setItem('token', response.token);

  alert('Login Successful');

  this.router.navigate(['/dashboard']);

},
      error: (err) => {

        console.error(err);

        alert('Invalid Email or Password');

      }

    });

  }

}