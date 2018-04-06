namespace DataAccess.Core.Specification.Paging
{
    public class QueryPaging : IQueryPaging
    {       
        public QueryPaging() : this(20, 0)
        {}

        public QueryPaging(int pageSize, int pageNumber)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
        }

        
        private int _pageNumber;

        /// <summary>
        /// Номер страницы
        /// </summary>
        public int PageNumber
        {
            get { return _pageNumber; }
            set { _pageNumber = value <= 0 ? 0 : value - 1; }
        }

        /// <summary>
        /// Размер страницы
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Номер последнего просматриваемого элемента
        /// </summary>
        public int Offset => PageSize*PageNumber;

        public static QueryPaging SingleItem 
            => new QueryPaging(1, 1);
    }
}
