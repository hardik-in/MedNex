namespace MedNex_Backend.API.DTOs.Common
{
    // Query parameters the client sends:
    // GET /api/patients?page=1&pageSize=10
    public class PagedRequest
    {
        private int _page = 1;
        private int _pageSize = 10;

        // Page number is 1-based (page 1 = first page, not page 0)
        public int Page
        {
            get => _page;
            set => _page = value < 1 ? 1 : value; // clamp minimum to 1
        }

        // Items per page — clamped between 1 and 50 to prevent
        // clients requesting 10,000 records in a single call.
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 1 : value > 50 ? 50 : value;
        }

        // Optional search term — applied in repo where supported
        public string? Search { get; set; }
    }
}