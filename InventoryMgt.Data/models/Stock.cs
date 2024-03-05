using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace InventoryMgt.Data.Models;
public class Stock : BaseSchema
{
    [NotNull]
    public int ProductId { get; set; }
    [NotNull, MinLength(0)]
    public double Quantity { get; set; }
}