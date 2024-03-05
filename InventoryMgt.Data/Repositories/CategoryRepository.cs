using System.Data;
using Dapper;
using InventoryMgt.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace InventoryMgt.Data.Repositories;

public interface ICategoryRepository
{
    Task<Category> AddCategory(Category category);
    Task UpdateCategory(Category category);
    Task DeleteCategory(int id);
    Task<Category> GetCategory(int id);
    Task<IEnumerable<Category>> GetCategories();
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
        string sql = "insert into Category(CreateDate,UpdateDate,IsDeleted,CategoryName,CategoryId) values(@CreateDate,@UpdateDate,@IsDeleted,@CategoryName,@CategoryId);select scope_indentity()";
        int addedCategoryId = await connection.ExecuteScalarAsync<int>(sql, new
        {
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            IsDeleted = false,
            CategoryName = category.CategoryName,
            CategoryId = category.CategoryId
        });
        category.Id = addedCategoryId;
        return category;
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

    public async Task<IEnumerable<Category>> GetCategories()
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        string sql = "select * from Category where IsDeleted=0";
        return await connection.QueryAsync<Category>(sql);

    }

    public async Task<Category> GetCategory(int id)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        string sql = "select * from Category where IsDeleted=0 and Id=@Id";
        return await connection.QueryFirstAsync<Category>(sql, new { Id = id });
    }

    public async Task UpdateCategory(Category category)
    {
        category.UpdateDate = DateTime.UtcNow;
        using IDbConnection connection = new SqlConnection(_connectionString);
        string sql = @"Update Category
          set
          UpdateDate=@UpdateDate,
          CategoryName=@CategoryName,
          CategoryId=@CategoryId where Id=@Id";
        await connection.ExecuteAsync(sql, new { Id = category.Id, UpdateDate = category.UpdateDate, CategoryName = category.CategoryName, CategoryId = category.CategoryName });
    }
}