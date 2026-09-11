import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss'
})
export class LayoutComponent {
  sidebarCollapsed = false;
  sidebarMobileOpen = false;
  userMenuOpen = false;

  constructor(public auth: AuthService, private router: Router) {}

  toggleSidebar(): void {
    if (window.innerWidth <= 768) {
      this.sidebarMobileOpen = !this.sidebarMobileOpen;
    } else {
      this.sidebarCollapsed = !this.sidebarCollapsed;
    }
  }

  closeMobileSidebar(): void {
    this.sidebarMobileOpen = false;
  }

  toggleUserMenu(): void {
    this.userMenuOpen = !this.userMenuOpen;
  }

  closeUserMenu(): void {
    this.userMenuOpen = false;
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  get canRegisterComplaint(): boolean {
    return this.auth.hasRole('BranchOfficer', 'SuperAdmin');
  }

  get canManageUsers(): boolean {
    return this.auth.hasRole('SuperAdmin');
  }

  getUserInitials(): string {
    const user = this.auth.currentUser();
    if (!user) return '?';
    const parts = user.fullName.split(' ');
    if (parts.length >= 2) {
      return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
    }
    return user.fullName.substring(0, 2).toUpperCase();
  }
}
