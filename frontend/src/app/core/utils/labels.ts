export const STATUS_LABELS: Record<string, string> = {
  Pending: 'लंबित',
  UnderReview: 'समीक्षाधीन',
  ForwardedToDepartment: 'विभाग को अग्रेषित',
  ForwardedForFieldEnquiry: 'क्षेत्रीय जांच हेतु अग्रेषित',
  ActionTaken: 'कार्रवाई की गई',
  Disposed: 'निष्पादित'
};

export const ROLE_LABELS: Record<string, string> = {
  SuperAdmin: 'सुपर एडमिन',
  BranchOfficer: 'शाखा अधिकारी',
  DepartmentHead: 'विभागाध्यक्ष',
  CompetentAuthority: 'सक्षम प्राधिकारी'
};

export const SOURCE_LABELS: Record<string, string> = {
  Offline: 'ऑफ़लाइन',
  CPGRAMS: 'सीपीग्राम्स',
  CMJaibodha: 'सीएम जय बोध',
  PMOGrievancePortal: 'पीएमओ शिकायत पोर्टल',
  Other: 'अन्य'
};

export const REMARK_TYPE_LABELS: Record<string, string> = {
  Enquiry: 'जांच टिप्पणी',
  Official: 'कार्यालयी / कार्रवाई टिप्पणी'
};

export function statusLabel(status: string | null | undefined): string {
  if (!status) return '—';
  return STATUS_LABELS[status] ?? status;
}

export function roleLabel(role: string | null | undefined): string {
  if (!role) return '—';
  return ROLE_LABELS[role] ?? role;
}

export function sourceLabel(source: string | null | undefined): string {
  if (!source) return '—';
  return SOURCE_LABELS[source] ?? source;
}

export function remarkTypeLabel(type: string | null | undefined): string {
  if (!type) return '—';
  return REMARK_TYPE_LABELS[type] ?? type;
}
