using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace InventoryMgt.Data.Models;
public class Purchase : BaseSchema
{
    [NotNull]
    public int ProductId { get; set; }
    [NotNull, MinLength(0)]
    public double Quantity { get; set; }
    [NotNull]
    public DateTime PurchaseDate { get; set; }
    [MaxLength(100)]
    public string? Description { get; set; }
}