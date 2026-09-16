import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ComplaintService } from '../../../core/services/complaint.service';
import { ComplaintListItem } from '../../../core/models/models';
import { statusLabel } from '../../../core/utils/labels';

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
    { value: '', label: 'सभी स्थितियां' },
    { value: 'Pending', label: 'लंबित' },
    { value: 'UnderReview', label: 'समीक्षाधीन' },
    { value: 'ForwardedToDepartment', label: 'विभाग को अग्रेषित' },
    { value: 'ForwardedForFieldEnquiry', label: 'क्षेत्रीय जांच हेतु अग्रेषित' },
    { value: 'ActionTaken', label: 'कार्रवाई की गई' },
    { value: 'Disposed', label: 'निष्पादित' }
  ];

  statusLabel = statusLabel;

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
