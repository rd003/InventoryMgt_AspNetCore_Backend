using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace InventoryMgt.Data.Models;
public class Product : BaseSchema
{
    [NotNull, MaxLength(50)]
    public string? ProductName { get; set; }
    [NotNull]
    public int CategoryId { get; set; }
    [NotNull]
    public double Price { get; set; }
}