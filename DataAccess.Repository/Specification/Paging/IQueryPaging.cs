namespace DataAccess.Repository.Specification.Paging
{
    public interface IQueryPaging
    {
        int Offset { get; }
        int PageNumber { get; set; }
        int PageSize { get; set; }
    }
}
