namespace MedNex_Backend.API.DTOs.Common
{
    // Generic wrapper returned by every paginated endpoint.
    // T is the DTO type (PatientDto, AppointmentDto, etc.)
    //
    // Example response:
    // {
    //   "items": [...],
    //   "totalCount": 148,
    //   "page": 1,
    //   "pageSize": 10,
    //   "totalPages": 15,
    //   "hasNextPage": true,
    //   "hasPreviousPage": false
    // }
    public class PagedResult<T>
    {
        // The actual data for this page
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        // Total records in the full dataset (not just this page)
        // Frontend uses this to render "Showing 1-10 of 148 results"
        public int TotalCount { get; set; }

        // Current page number (1-based)
        public int Page { get; set; }

        // Records per page
        public int PageSize { get; set; }

        // Total number of pages — calculated server-side so frontend
        // doesn't need to do the math
        public int TotalPages { get; set; }

        // Convenience flags — frontend uses these to enable/disable
        // Next / Previous buttons without any calculation
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        // Static factory — creates a PagedResult from a full list and request params.
        // All pagination math lives here in one place.
        public static PagedResult<T> Create(IEnumerable<T> items, int totalCount, PagedRequest request)
        {
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                HasNextPage = request.Page < totalPages,
                HasPreviousPage = request.Page > 1
            };
        }
    }
}