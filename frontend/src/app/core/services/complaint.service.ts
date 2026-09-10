import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ComplaintDetail,
  ComplaintListItem,
  CreateComplaintPayload,
  DashboardSummary
} from '../models/models';

@Injectable({ providedIn: 'root' })
export class ComplaintService {
  private base = `${environment.apiUrl}/complaints`;

  constructor(private http: HttpClient) {}

  list(params?: { status?: string; search?: string }): Observable<ComplaintListItem[]> {
    let query = '';
    if (params?.status) query += `status=${params.status}&`;
    if (params?.search) query += `search=${encodeURIComponent(params.search)}&`;
    return this.http.get<ComplaintListItem[]>(`${this.base}?${query}`);
  }

  getById(id: number): Observable<ComplaintDetail> {
    return this.http.get<ComplaintDetail>(`${this.base}/${id}`);
  }

  create(payload: CreateComplaintPayload): Observable<ComplaintDetail> {
    return this.http.post<ComplaintDetail>(this.base, payload);
  }

  uploadDocument(complaintId: number, file: File, documentType: string): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('documentType', documentType);
    return this.http.post(`${this.base}/${complaintId}/documents`, formData);
  }

  addRemark(complaintId: number, remarkType: string, text: string): Observable<any> {
    return this.http.post(`${this.base}/${complaintId}/remarks`, { remarkType, text });
  }

  forward(complaintId: number, payload: { departmentId?: number; competentAuthorityDesignationId?: number; reason: string }): Observable<any> {
    return this.http.post(`${this.base}/${complaintId}/forward`, payload);
  }

  markActionTaken(complaintId: number, text: string): Observable<any> {
    return this.http.post(`${this.base}/${complaintId}/action-taken`, { remarkType: 'Official', text });
  }

  close(complaintId: number, closureRemarks: string): Observable<any> {
    return this.http.post(`${this.base}/${complaintId}/close`, { closureRemarks });
  }

  downloadUrl(complaintId: number, documentId: number): string {
    return `${this.base}/${complaintId}/documents/${documentId}/download`;
  }

  downloadDocumentBlob(complaintId: number, documentId: number): Observable<Blob> {
    return this.http.get(this.downloadUrl(complaintId, documentId), { responseType: 'blob' });
  }

  dashboardSummary(): Observable<DashboardSummary> {
    return this.http.get<DashboardSummary>(`${environment.apiUrl}/dashboard/summary`);
  }
}
