using System.Data;
using Dapper;
using InventoryMgt.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public interface IPurchaseRepository
{
    Task<Purchase> AddPurchase(Purchase purchase);
    Task<Purchase> UpdatePurchase(Purchase purchase);
    Task RemovePurchase(int id);
    Task<Purchase?> GetPurchase(int id);
    Task<PaginatedPurchase> GetPurchases(int page = 1, int limit = 4, string? productName = null, DateTime? dateFrom = null, DateTime? dateTo = null, string? sortColumn = null, string? sortDirection = null);
}

public class PurchaseRepository : IPurchaseRepository
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    public PurchaseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = this._configuration.GetConnectionString("default") ?? "";
    }
    public async Task<Purchase> AddPurchase(Purchase purchase)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        Purchase createdPurchase = await connection.QuerySingleAsync<Purchase>("usp_AddPurchase", new
        {
            purchase.PurchaseDate,
            purchase.ProductId,
            purchase.Description,
            purchase.Quantity,
            purchase.Price
        }, commandType: CommandType.StoredProcedure);
        return createdPurchase;
    }

    public async Task<Purchase?> GetPurchase(int id)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        var purchase = await connection.QueryFirstOrDefaultAsync<Purchase>("usp_GetPurchaseById", new { Id = id }, commandType: CommandType.StoredProcedure);
        return purchase;
    }

    public async Task<PaginatedPurchase> GetPurchases(int page = 1, int limit = 4, string? productName = null, DateTime? dateFrom = null, DateTime? dateTo = null, string? sortColumn = null, string? sortDirection = null)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        var param = new
        {
            page,
            limit,
            productName,
            dateFrom,
            dateTo,
            sortColumn,
            sortDirection
        };
        var multipleResult = await connection.QueryMultipleAsync("usp_getPurchases", param, commandType: CommandType.StoredProcedure);
        var purchases = multipleResult.Read<Purchase>();
        var paginationData = multipleResult.ReadFirst<PaginationBase>();
        paginationData.Page = page;
        paginationData.Limit = limit;
        return new PaginatedPurchase { Purchases = purchases, Pagination = paginationData };
    }

    public async Task RemovePurchase(int id)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync("usp_DeletePurchase", new
        {
            Id = id
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<Purchase> UpdatePurchase(Purchase purchase)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        Purchase updatedPurchase = await connection.QuerySingleAsync<Purchase>("usp_UpdatePurchase", new
        {
            purchase.Id,
            purchase.PurchaseDate,
            purchase.ProductId,
            purchase.Description,
            purchase.Quantity,
            purchase.Price
        }, commandType: CommandType.StoredProcedure);
        return updatedPurchase;
    }
}