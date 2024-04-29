using Currency.Exchange.API.Models;

namespace Currency.Exchange.API.Repositories.Interfaces
{
    public interface IExchangeRepository
    {
        Task<bool> SaveExchangeRates(DBExchangeRates currencyExchangeRates);
        Task<List<CurrencyExchangeRates>> GetExchangeRates(DateTime FromDate);
        Task<bool> SaveExchangeLog(CurrencyExchangeHistory currencyExchangeHistory);
        Task<List<CurrencyExchangeHistory>> GetExchangeLogs(DateTime FromDate);
    }
}
