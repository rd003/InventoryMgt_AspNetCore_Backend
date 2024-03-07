using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace InventoryMgt.Data.Models;
public class Category : BaseSchema
{
    [NotNull]
    [MaxLength(50)]
    public string? CategoryName { get; set; }
    public int? CategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
}