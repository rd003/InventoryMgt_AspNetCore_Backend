namespace InventoryMgt.Data.Models.DTOs;
public class PagedProduct
{
    public IEnumerable<ProductDisplay> Products { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
}

public class ProductCount
{
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
}