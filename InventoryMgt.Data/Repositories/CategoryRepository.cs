using System.Data;
using Dapper;
using InventoryMgt.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace InventoryMgt.Data.Repositories;

public interface ICategoryRepository
{
    Task<Category> AddCategory(Category category);
    Task<Category> UpdateCategory(Category category);
    Task DeleteCategory(int id);
    Task<Category?> GetCategory(int id);
    Task<IEnumerable<Category>> GetCategories(string searchTerm = "");
}
public class CategoryRepository : ICategoryRepository
{
    private readonly string? _connectionString;
    private readonly IConfiguration _config;
    public CategoryRepository(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("default");
    }
    public async Task<Category> AddCategory(Category category)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        var createdCategory = await connection.QueryFirstAsync<Category>("usp_AddCategory", new
        {
            CategoryName = category.CategoryName,
            CategoryId = category.CategoryId
        }, commandType: CommandType.StoredProcedure);
        return createdCategory;
    }

    public async Task DeleteCategory(int id)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        string sql = "update Category set IsDeleted=@IsDeleted where Id=@Id";
        await connection.ExecuteAsync(sql, new
        {
            IsDeleted = true,
            Id = id
        });
    }

    public async Task<IEnumerable<Category>> GetCategories(string searchTerm = "")
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<Category>("usp_getCategories", new { searchTerm }, commandType: CommandType.StoredProcedure);

    }

    public async Task<Category?> GetCategory(int id)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        string sql = "select * from Category where IsDeleted=0 and Id=@Id";
        return await connection.QueryFirstOrDefaultAsync<Category>(sql, new { Id = id });
    }

    public async Task<Category> UpdateCategory(Category category)
    {
        category.UpdateDate = DateTime.UtcNow;
        using IDbConnection connection = new SqlConnection(_connectionString);
        var updatedCategory = await connection.QueryFirstAsync<Category>("usp_UpdateCategory", new
        {
            Id = category.Id,
            CategoryName = category.CategoryName,
            CategoryId = category.CategoryId
        });
        return updatedCategory;
    }
}