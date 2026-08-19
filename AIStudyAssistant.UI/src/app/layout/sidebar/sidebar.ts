import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
selector: 'app-sidebar',
standalone: true,
imports: [RouterLink, RouterLinkActive],
templateUrl: './sidebar.html',
styleUrl: './sidebar.scss'
})
export class Sidebar {

constructor(private router: Router) {}

logout(): void {
localStorage.removeItem('token');
this.router.navigate(['/login']);
}

}
