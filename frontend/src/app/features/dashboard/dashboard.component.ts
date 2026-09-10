import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ComplaintService } from '../../core/services/complaint.service';
import { AuthService } from '../../core/services/auth.service';
import { DashboardSummary } from '../../core/models/models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  summary: DashboardSummary | null = null;
  loading = true;

  constructor(private complaintService: ComplaintService, public auth: AuthService) {}

  ngOnInit(): void {
    this.complaintService.dashboardSummary().subscribe({
      next: (s) => {
        this.summary = s;
        this.loading = false;
      },
      error: () => (this.loading = false)
    });
  }

  get canRegisterComplaint(): boolean {
    return this.auth.hasRole('BranchOfficer', 'SuperAdmin');
  }
}
