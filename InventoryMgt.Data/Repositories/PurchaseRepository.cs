using InventoryMgt.Data.Models;

public interface IPurchaseRepository
{
    Task<Purchase> AddPurchase(Purchase purchase);
    Task<Purchase> UpdatePurchase(Purchase purchase);
    Task<Purchase> RemovePurchase(int id);
    Task<Purchase> GetPurchase(int id);
    Task<IEnumerable<Purchase>> GetPurchases();
}

class PurchaseRepository : IPurchaseRepository
{
    public Task<Purchase> AddPurchase(Purchase purchase)
    {
        throw new NotImplementedException();
    }

    public Task<Purchase> GetPurchase(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Purchase>> GetPurchases()
    {
        throw new NotImplementedException();
    }

    public Task<Purchase> RemovePurchase(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Purchase> UpdatePurchase(Purchase purchase)
    {
        throw new NotImplementedException();
    }
}