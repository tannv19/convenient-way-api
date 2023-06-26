namespace ship_convenient.Core.CoreModel
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public int TotalPage { get; }
        public PaginatedList(IList<T> items, int totalCount, int pageIndex, int pageSize)
        {
            TotalCount = totalCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalPage = (int)Math.Ceiling(TotalCount / (double)pageSize);
            AddRange(items);
        }
    }
}
