import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { LayoutComponent } from './shared/layout/layout.component';
import { LoginComponent } from './features/auth/login/login.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { ComplaintListComponent } from './features/complaints/complaint-list/complaint-list.component';
import { ComplaintEntryComponent } from './features/complaints/complaint-entry/complaint-entry.component';
import { ComplaintDetailComponent } from './features/complaints/complaint-detail/complaint-detail.component';
import { UserManagementComponent } from './features/admin/user-management/user-management.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'complaints', component: ComplaintListComponent },
      {
        path: 'complaints/new',
        component: ComplaintEntryComponent,
        canActivate: [roleGuard(['BranchOfficer', 'SuperAdmin'])]
      },
      { path: 'complaints/:id', component: ComplaintDetailComponent },
      {
        path: 'admin/users',
        component: UserManagementComponent,
        canActivate: [roleGuard(['SuperAdmin'])]
      }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
