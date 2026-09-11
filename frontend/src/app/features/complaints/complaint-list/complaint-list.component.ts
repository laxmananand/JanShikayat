import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ComplaintService } from '../../../core/services/complaint.service';
import { ComplaintListItem } from '../../../core/models/models';

@Component({
  selector: 'app-complaint-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './complaint-list.component.html',
  styleUrl: './complaint-list.component.scss'
})
export class ComplaintListComponent implements OnInit {
  complaints: ComplaintListItem[] = [];
  loading = true;
  statusFilter = '';
  searchTerm = '';

  statuses = [
    { value: '', label: 'All statuses' },
    { value: 'Pending', label: 'Pending' },
    { value: 'UnderReview', label: 'Under Review' },
    { value: 'ForwardedToDepartment', label: 'Forwarded to Department' },
    { value: 'ForwardedForFieldEnquiry', label: 'Forwarded for Field Enquiry' },
    { value: 'ActionTaken', label: 'Action Taken' },
    { value: 'Disposed', label: 'Disposed' }
  ];

  constructor(
    private complaintService: ComplaintService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    // Read status query param from URL (set when clicking dashboard cards)
    this.route.queryParams.subscribe(params => {
      if (params['status']) {
        this.statusFilter = params['status'];
      }
      this.load();
    });
  }

  load(): void {
    this.loading = true;
    this.complaintService.list({ status: this.statusFilter, search: this.searchTerm }).subscribe({
      next: (data) => {
        this.complaints = data;
        this.loading = false;
      },
      error: () => (this.loading = false)
    });
  }

  openComplaint(id: number): void {
    this.router.navigate(['/complaints', id]);
  }
}
