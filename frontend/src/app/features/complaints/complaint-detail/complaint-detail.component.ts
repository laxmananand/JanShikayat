import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ComplaintService } from '../../../core/services/complaint.service';
import { LookupService } from '../../../core/services/lookup.service';
import { AuthService } from '../../../core/services/auth.service';
import { CompetentAuthorityDesignation, ComplaintDetail, Department } from '../../../core/models/models';

@Component({
  selector: 'app-complaint-detail',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './complaint-detail.component.html',
  styleUrl: './complaint-detail.component.scss'
})
export class ComplaintDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private complaintService = inject(ComplaintService);
  private lookupService = inject(LookupService);
  public auth = inject(AuthService);
  private fb = inject(FormBuilder);

  complaint: ComplaintDetail | null = null;
  loading = true;
  complaintId!: number;

  departments: Department[] = [];
  authorities: CompetentAuthorityDesignation[] = [];

  busy = false;
  message = '';
  errorMessage = '';
  reportFile: File | null = null;

  remarkForm = this.fb.group({
    remarkType: ['Enquiry', Validators.required],
    text: ['', [Validators.required, Validators.maxLength(3000)]]
  });

  forwardForm = this.fb.group({
    targetType: ['department', Validators.required], // 'department' | 'authority'
    departmentId: [null as number | null],
    competentAuthorityDesignationId: [null as number | null],
    reason: ['', Validators.required]
  });

  actionForm = this.fb.group({
    text: ['', [Validators.required, Validators.maxLength(3000)]]
  });

  closeForm = this.fb.group({
    closureRemarks: ['', [Validators.required, Validators.maxLength(1000)]]
  });

  ngOnInit(): void {
    this.complaintId = Number(this.route.snapshot.paramMap.get('id'));
    this.load();
    this.lookupService.getDepartments().subscribe((d) => (this.departments = d));
    this.lookupService.getCompetentAuthorities().subscribe((a) => (this.authorities = a));
  }

  load(): void {
    this.loading = true;
    this.complaintService.getById(this.complaintId).subscribe({
      next: (c) => {
        this.complaint = c;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.errorMessage = 'Complaint not found or you do not have access to it.';
      }
    });
  }

  get canAddRemarkOrForward(): boolean {
    return this.auth.hasRole('DepartmentHead', 'CompetentAuthority', 'SuperAdmin');
  }

  get canForward(): boolean {
    return this.auth.hasRole('DepartmentHead', 'SuperAdmin');
  }

  get canMarkActionTaken(): boolean {
    return this.auth.hasRole('CompetentAuthority', 'DepartmentHead', 'SuperAdmin');
  }

  get canClose(): boolean {
    return this.auth.hasRole('DepartmentHead', 'CompetentAuthority', 'SuperAdmin');
  }

  get isClosed(): boolean {
    return this.complaint?.status === 'Disposed';
  }

  onReportFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      if (file.type !== 'application/pdf') {
        this.errorMessage = 'Only PDF files can be uploaded.';
        return;
      }
      this.reportFile = file;
      this.errorMessage = '';
    }
  }

  viewDocument(documentId: number): void {
    this.complaintService.downloadDocumentBlob(this.complaintId, documentId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        window.open(url, '_blank');
      },
      error: () => (this.errorMessage = 'Failed to open document.')
    });
  }

  uploadReport(): void {
    if (!this.reportFile) return;
    this.busy = true;
    this.complaintService.uploadDocument(this.complaintId, this.reportFile, 'EnquiryReport').subscribe({
      next: () => {
        this.busy = false;
        this.reportFile = null;
        this.message = 'Report uploaded successfully.';
        this.load();
      },
      error: () => {
        this.busy = false;
        this.errorMessage = 'Failed to upload report.';
      }
    });
  }

  submitRemark(): void {
    if (this.remarkForm.invalid) {
      this.remarkForm.markAllAsTouched();
      return;
    }
    this.busy = true;
    const { remarkType, text } = this.remarkForm.value;
    this.complaintService.addRemark(this.complaintId, remarkType!, text!).subscribe({
      next: () => {
        this.busy = false;
        this.remarkForm.reset({ remarkType: 'Enquiry', text: '' });
        this.message = 'Remark added.';
        this.load();
      },
      error: () => {
        this.busy = false;
        this.errorMessage = 'Failed to add remark.';
      }
    });
  }

  submitForward(): void {
    if (this.forwardForm.invalid) {
      this.forwardForm.markAllAsTouched();
      return;
    }
    const { targetType, departmentId, competentAuthorityDesignationId, reason } = this.forwardForm.value;

    if (targetType === 'department' && !departmentId) {
      this.errorMessage = 'Select a department to forward to.';
      return;
    }
    if (targetType === 'authority' && !competentAuthorityDesignationId) {
      this.errorMessage = 'Select a competent authority to forward to.';
      return;
    }

    this.busy = true;
    this.complaintService
      .forward(this.complaintId, {
        departmentId: targetType === 'department' ? departmentId! : undefined,
        competentAuthorityDesignationId: targetType === 'authority' ? competentAuthorityDesignationId! : undefined,
        reason: reason!
      })
      .subscribe({
        next: () => {
          this.busy = false;
          this.forwardForm.reset({ targetType: 'department', reason: '' });
          this.message = 'Complaint forwarded.';
          this.load();
        },
        error: () => {
          this.busy = false;
          this.errorMessage = 'Failed to forward complaint.';
        }
      });
  }

  submitActionTaken(): void {
    if (this.actionForm.invalid) {
      this.actionForm.markAllAsTouched();
      return;
    }
    this.busy = true;
    this.complaintService.markActionTaken(this.complaintId, this.actionForm.value.text!).subscribe({
      next: () => {
        this.busy = false;
        this.actionForm.reset();
        this.message = 'Action recorded.';
        this.load();
      },
      error: () => {
        this.busy = false;
        this.errorMessage = 'Failed to record action.';
      }
    });
  }

  submitClose(): void {
    if (this.closeForm.invalid) {
      this.closeForm.markAllAsTouched();
      return;
    }
    this.busy = true;
    this.complaintService.close(this.complaintId, this.closeForm.value.closureRemarks!).subscribe({
      next: () => {
        this.busy = false;
        this.message = 'Case closed.';
        this.load();
      },
      error: () => {
        this.busy = false;
        this.errorMessage = 'Failed to close case.';
      }
    });
  }
}
