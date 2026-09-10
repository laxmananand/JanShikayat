import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Branch, CompetentAuthorityDesignation, Department } from '../models/models';

@Injectable({ providedIn: 'root' })
export class LookupService {
  constructor(private http: HttpClient) {}

  getBranches(): Observable<Branch[]> {
    return this.http.get<Branch[]>(`${environment.apiUrl}/branches`);
  }

  getDepartments(): Observable<Department[]> {
    return this.http.get<Department[]>(`${environment.apiUrl}/departments`);
  }

  getCompetentAuthorities(): Observable<CompetentAuthorityDesignation[]> {
    return this.http.get<CompetentAuthorityDesignation[]>(`${environment.apiUrl}/competent-authorities`);
  }
}
