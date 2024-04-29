using Currency.Exchange.API.Models;

namespace Currency.Exchange.API.Services.Interfaces
{
    public interface IExchangeService
    {
        Task<Decimal> GetExchangeRates(string basecode, string targetcode, decimal amount);
        Task<List<CurrencyExchangeRates>> GetExchangeRatesHistory(DateTime FromDate);
        Task<List<CurrencyExchangeHistory>> GetExchangeLogs(DateTime FromDate);
    }
}
