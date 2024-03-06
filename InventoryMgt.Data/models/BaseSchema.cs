namespace InventoryMgt.Data.Models;
public class BaseSchema
{
    public int Id { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }
}