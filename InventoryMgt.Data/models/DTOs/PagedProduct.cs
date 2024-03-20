namespace InventoryMgt.Data.Models.DTOs;
public class PagedProduct : PaginationBase
{
    public IEnumerable<ProductDisplay> Products { get; set; }

}

public class ProductCount
{
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
}