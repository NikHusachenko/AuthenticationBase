import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { NavbarItemComponent } from "../navbar-item/navbar-item.component";

type NavigationItem = {
  label: string,
  route: string
}

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, NavbarItemComponent],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})

export class HeaderComponent {
  protected links: NavigationItem[] = [
    { label: 'Home', route: '/' },
    { label: 'List', route: 'user/user-list' },
    { label: 'About project', route: 'home/about' },
    { label: 'Account information', route: 'user/user-information' }
  ]
}