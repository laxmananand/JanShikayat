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
    { value: 'BranchOfficer', label: 'Branch Officer (registers complaints)' },
    { value: 'DepartmentHead', label: 'Department Head (reviews & forwards)' },
    { value: 'CompetentAuthority', label: 'Competent Authority (field enquiry)' },
    { value: 'SuperAdmin', label: 'Super Admin' }
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
        this.message = `Account created for ${this.form.value.email}.`;
        this.form.reset({ role: 'BranchOfficer' });
      },
      error: (err) => {
        this.submitting = false;
        this.errorMessage = err.error?.errors?.join(', ') || err.error?.message || 'Could not create account.';
      }
    });
  }
}
