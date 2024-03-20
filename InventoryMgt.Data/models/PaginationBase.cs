namespace InventoryMgt.Data.Models;
public class PaginationBase
{
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
}