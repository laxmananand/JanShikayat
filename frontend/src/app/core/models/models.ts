export type AppRole = 'SuperAdmin' | 'BranchOfficer' | 'DepartmentHead' | 'CompetentAuthority';

export interface LoginResponse {
  token: string;
  expiresAt: string;
  email: string;
  fullName: string;
  role: AppRole;
  branchId: number | null;
  departmentId: number | null;
}

export interface CurrentUser {
  email: string;
  fullName: string;
  role: AppRole;
  branchId: number | null;
  departmentId: number | null;
}

export interface Branch {
  id: number;
  code: string;
  name: string;
  nameHindi?: string;
}

export interface Department {
  id: number;
  code: string;
  name: string;
  nameHindi?: string;
}

export interface CompetentAuthorityDesignation {
  id: number;
  code: string;
  title: string;
}

export type ComplaintStatus =
  | 'Pending'
  | 'UnderReview'
  | 'ForwardedToDepartment'
  | 'ForwardedForFieldEnquiry'
  | 'ActionTaken'
  | 'Disposed';

export interface ComplaintListItem {
  id: number;
  complaintNumber: string;
  applicantName: string;
  subject: string;
  complaintDate: string;
  status: ComplaintStatus;
  branchName: string;
  departmentName: string | null;
}

export interface ComplaintDocument {
  id: number;
  fileName: string;
  documentType: string;
  uploadedAt: string;
  downloadUrl: string;
}

export interface ComplaintRemark {
  id: number;
  remarkType: string;
  text: string;
  createdByName: string;
  createdAt: string;
}

export interface ComplaintHistoryEntry {
  fromStatus: string;
  toStatus: string;
  reason: string | null;
  actionByName: string;
  actionAt: string;
}

export interface ComplaintDetail extends ComplaintListItem {
  guardianName: string;
  mobileNumber: string;
  address: string;
  district: string;
  block: string;
  policeStation?: string;
  oppositeParty?: string;
  category: string;
  subCategory?: string;
  description: string;
  source: string;
  closureRemarks?: string;
  closedAt?: string;
  documents: ComplaintDocument[];
  remarks: ComplaintRemark[];
  history: ComplaintHistoryEntry[];
}

export interface CreateComplaintPayload {
  applicantName: string;
  guardianName: string;
  mobileNumber: string;
  address: string;
  district: string;
  block: string;
  policeStation?: string;
  area?: string;
  email?: string;
  oppositeParty?: string;
  subject: string;
  category: string;
  subCategory?: string;
  description: string;
  complaintDate?: string;
  source: string;
  branchId: number;
  departmentId?: number;
}

export interface DashboardSummary {
  totalComplaints: number;
  pending: number;
  underReview: number;
  forwarded: number;
  underFieldEnquiry: number;
  disposed: number;
}
