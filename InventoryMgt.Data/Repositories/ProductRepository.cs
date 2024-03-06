using System.Data;
using Dapper;
using InventoryMgt.Data.Models;
using InventoryMgt.Data.Models.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace InventoryMgt.Data.Repositories;
public interface IProductRepository
{
    Task<ProductDisplay> AddProduct(Product product);
    Task UpdatProduct(Product product);
    Task DeleteProduct(int id);
    Task<IEnumerable<ProductDisplay>> GetProducts();
    Task<ProductDisplay?> GetProduct(int id);
}
public class ProductRepository : IProductRepository
{
    private readonly IConfiguration _config;
    private readonly string _constr;
    public ProductRepository(IConfiguration config)
    {
        _config = config;
        _constr = _config.GetConnectionString("default");
    }
    public async Task<ProductDisplay> AddProduct(Product product)
    {
        using IDbConnection connection = new SqlConnection(_constr);
        ProductDisplay createdProduct = await connection.QueryFirstAsync<ProductDisplay>("Usp_AddProduct", new
        {
            ProductName = product.ProductName,
            CategoryId = product.CategoryId,
            Price = product.Price
        }, commandType: CommandType.StoredProcedure);

        return createdProduct;
    }

    public async Task UpdatProduct(Product product)
    {
        using IDbConnection connection = new SqlConnection(_constr);
        string sql = @"update Product set
         UpdateDate=getdate(), 
         ProductName=@ProductName,
         CategoryId=@CategoryId,
         Price=@Price
         where Id=@Id";
        await connection.ExecuteAsync(sql, new
        {
            ProductName = product.ProductName,
            CategoryId = product.CategoryId,
            Price = product.Price,
            Id = product.Id
        });
    }

    public async Task DeleteProduct(int id)
    {
        using IDbConnection connection = new SqlConnection(_constr);
        string sql = "update Product set IsDeleted=1 where Id=@id";
        await connection.ExecuteAsync(sql, new { id });
    }

    public async Task<ProductDisplay?> GetProduct(int id)
    {
        using IDbConnection connection = new SqlConnection(_constr);
        string sql = @"select p.*, c.CategoryName from Product p join Category c
         on p.CategoryId=c.Id where p.IsDeleted=0 and c.IsDeleted=0 and p.Id=@id";
        var product = await connection.QueryFirstOrDefaultAsync<ProductDisplay>(sql, new { id });
        return product;
    }

    public async Task<IEnumerable<ProductDisplay>> GetProducts()
    {
        using IDbConnection connection = new SqlConnection(_constr);
        string sql = @"select p.*, c.CategoryName from Product p 
        join Category c on p.CategoryId=c.Id where p.IsDeleted=0 and c.IsDeleted=0";
        return await connection.QueryAsync<ProductDisplay>(sql);
    }


}