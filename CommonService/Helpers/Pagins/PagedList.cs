namespace CommonService.Helpers.Pagins
{
    public class PagedList<TEntity> : List<TEntity>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }

        public bool HasPrevious
        {
            get
            {
                return CurrentPage > 1;
            }
        }
        public bool HasNext
        {
            get
            {
                return CurrentPage < TotalPages;
            }
        }

        public PagedList(List<TEntity> items, int currentPage, int itemsPerPage, int totalItems)
        {
            TotalItems = totalItems;
            ItemsPerPage = itemsPerPage;
            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling(totalItems / (double)itemsPerPage);
            this.AddRange(items);
        }
    }
}
