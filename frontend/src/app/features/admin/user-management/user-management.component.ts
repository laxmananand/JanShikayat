import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { environment } from '../../../../environments/environment';
import { LookupService } from '../../../core/services/lookup.service';
import { Branch, Department } from '../../../core/models/models';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.scss'
})
export class UserManagementComponent implements OnInit {
  private fb = inject(FormBuilder);
  private http = inject(HttpClient);
  private lookupService = inject(LookupService);

  branches: Branch[] = [];
  departments: Department[] = [];
  submitting = false;
  message = '';
  errorMessage = '';

  roles = [
    { value: 'BranchOfficer', label: 'शाखा अधिकारी (शिकायत दर्ज करता है)' },
    { value: 'DepartmentHead', label: 'विभागाध्यक्ष (समीक्षा व अग्रेषण करता है)' },
    { value: 'CompetentAuthority', label: 'सक्षम प्राधिकारी (क्षेत्रीय जांच)' },
    { value: 'SuperAdmin', label: 'सुपर एडमिन' }
  ];

  form = this.fb.group({
    fullName: ['', Validators.required],
    designation: [''],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    role: ['BranchOfficer', Validators.required],
    branchId: [null as number | null],
    departmentId: [null as number | null]
  });

  ngOnInit(): void {
    this.lookupService.getBranches().subscribe((b) => (this.branches = b));
    this.lookupService.getDepartments().subscribe((d) => (this.departments = d));
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.submitting = true;
    this.message = '';
    this.errorMessage = '';

    this.http.post(`${environment.apiUrl}/auth/register`, this.form.value).subscribe({
      next: () => {
        this.submitting = false;
        this.message = `${this.form.value.email} के लिए खाता बनाया गया।`;
        this.form.reset({ role: 'BranchOfficer' });
      },
      error: (err) => {
        this.submitting = false;
        this.errorMessage = err.error?.errors?.join(', ') || err.error?.message || 'खाता नहीं बनाया जा सका।';
      }
    });
  }
}
