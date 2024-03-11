namespace linc.Utility
{
    public abstract class PagedViewModel
    {
        public int PageIndex { get; }

        public int PageSize { get; }

        public int TotalRecords { get; }

        protected PagedViewModel(int totalRecords, int pageIndex, int pageSize)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
            TotalRecords = totalRecords;
        }

        public bool HasPreviousPage => (PageIndex > 1);

        public int TotalPages => (int) Math.Ceiling(d: (decimal) TotalRecords / PageSize);

        public bool HasNextPage => (PageIndex < TotalPages);
    }
}
