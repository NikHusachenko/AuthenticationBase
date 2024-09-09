import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navbar-item',
  standalone: true,
  imports: [],
  templateUrl: './navbar-item.component.html',
  styleUrl: './navbar-item.component.css',
})
export class NavbarItemComponent {
  @Input() label: string | undefined;
  @Input() route: string | undefined;

  constructor(private router: Router) { }

  protected navigate() {
    if (this.route) {
      this.router.navigate([this.route]);
    }
  }
}