namespace ship_convenient.Core.CoreModel
{
    public static class IEnumerablePagedListExtension
    {
        public static PaginatedList<T> ToPagedList<T>(this IEnumerable<T> source, int totalCount, int pageIndex, int pageSize) => new PaginatedList<T>(source.ToList(), totalCount, pageIndex, pageSize);

        public static Task<PaginatedList<T>> ToPaginatedListAsync<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            if (pageIndex < 0 || pageSize < 0)
            {
                throw new ArgumentOutOfRangeException($"pageIndex and pageSize greater than one");
            }
            int totalCount = source.ToList().Count;
            List<T> items = source.Skip((pageIndex) * pageSize).Take(pageSize).ToList();
            PaginatedList<T> paginatedList = new PaginatedList<T>(items, totalCount, pageIndex, pageSize);
            return Task.FromResult(paginatedList);
        }
    }
}
