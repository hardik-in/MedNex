// ── Pagination ────────────────────────────────────────────────────────────
// Matches backend PagedResult<T> shape returned by all list endpoints
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// ── API Error ─────────────────────────────────────────────────────────────
// Consistent shape for error responses from the .NET backend
export interface ApiError {
  message: string;
  errors?: Record<string, string[]>; // validation errors (field → messages)
  statusCode?: number;
}