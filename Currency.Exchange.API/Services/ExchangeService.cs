using Currency.Exchange.API.Models;
using Currency.Exchange.API.Repositories.Interfaces;
using Currency.Exchange.API.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Authentication;
using Currency.Exchange.API.Helper;

namespace Currency.Exchange.API.Services
{
    public class ExchangeService : IExchangeService
    {
        private readonly IExchangeRepository _exchangeRepository;
        private readonly ICacheService _cacheService;

        private static HttpClientHandler clientHandler = new HttpClientHandler()
        {
            SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12
        };

        public ExchangeService(IExchangeRepository exchangeRepository, ICacheService cacheService)
        {
            _exchangeRepository = exchangeRepository;
            _cacheService = cacheService;
        }

        public string APIKey => System.Environment.GetEnvironmentVariable("openexchangeratesapikey");

        public string CacheExpirationMinutes => System.Environment.GetEnvironmentVariable("CacheExpirationMinutes");

        public string APIUrl => System.Environment.GetEnvironmentVariable("openexchangeratesapi");

        /// <summary>
        /// This method will return the exchanged value of the amount requested.
        /// This method will first fetch the result from cache. If cache is empty then it will call the external api.
        /// On successfull response from external api, it will set the result into cache, store the result into database and converted value in database.
        /// </summary>
        /// <param name="basecode"></param>
        /// <param name="targetcode"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<decimal> GetExchangeRates(string basecode, string targetcode, decimal amount)
        {
            try
            {
                // _cacheService.RemoveData("exchangerate");
                decimal ReturnAmt = 0;
                DBExchangeRates dBExchangeRates = new();
                CurrencyExchangeHistory currencyExchangeHistory = new();
                DateTime currentDateTime = DateTime.Now;

                //Fetch the Exchange Rates from the cache
                var cacheRates = _cacheService.GetData<Dictionary<string, decimal>>("exchangerate");

                //If cahce Exchange Rates is not null then just calculate the return amount
                if (cacheRates != null)
                {
                    var BaseRate = basecode == "USD" ? 1 : cacheRates[basecode];
                    var TargetRate = targetcode == "USD" ? 1 : cacheRates[targetcode];

                    var ExRate = TargetRate / BaseRate;
                    ReturnAmt = (amount * ExRate).GetDecimalPoint();
                }
                //If cahce Exchange Rates is null then call the external api and store the result
                else
                {
                    string ExchangeApiUrl = APIUrl + APIKey;
                    HttpClient httpClient = new HttpClient(clientHandler, false);
                    httpClient.BaseAddress = new Uri(ExchangeApiUrl);
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await httpClient.GetAsync(ExchangeApiUrl);

                    if (!response.IsSuccessStatusCode)
                    {
                        string msg = $"StatusCode {response.StatusCode}: Failed with HttpClient.GetAsync call to: {ExchangeApiUrl}";

                        throw new HttpRequestException(msg);
                    }

                    using (HttpContent content = response.Content)
                    {
                        CurrencyExchangeRates? apiResponse = JsonConvert.DeserializeObject<CurrencyExchangeRates>(response.Content.ReadAsStringAsync().Result);

                        if (apiResponse != null)
                        {
                            dBExchangeRates.TimeStamp = currentDateTime;
                            dBExchangeRates.BaseCurrency = apiResponse.@base;
                            dBExchangeRates.ExchangeRate = JsonConvert.SerializeObject(apiResponse.rates);

                            var rate = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(dBExchangeRates.ExchangeRate);

                            bool isSavedER = await _exchangeRepository.SaveExchangeRates(dBExchangeRates);
                            if (isSavedER)
                            {
                                var BaseRate = basecode == "USD" ? 1 : rate[basecode];
                                var TargetRate = targetcode == "USD" ? 1 : rate[targetcode];

                                var ExRate = TargetRate / BaseRate;
                                ReturnAmt = (amount * ExRate).GetDecimalPoint();

                                var expirationTime = DateTimeOffset.Now.AddMinutes(Convert.ToDouble(CacheExpirationMinutes));
                                _cacheService.SetData<Dictionary<string, decimal>>("exchangerate", rate, expirationTime);
                            }
                        }
                    }
                }

                //Insert the request into database
                currencyExchangeHistory.TimeStamp = currentDateTime;
                currencyExchangeHistory.BaseCurrency = basecode;
                currencyExchangeHistory.BaseAmount = amount;
                currencyExchangeHistory.ToCurrency = targetcode;
                currencyExchangeHistory.ToAmount = ReturnAmt;

                bool isSavedEL = await _exchangeRepository.SaveExchangeLog(currencyExchangeHistory);

                return ReturnAmt;
            }
            catch (System.Exception ex)
            {
                throw new HttpRequestException(ex.Message.ToString());
            }
        }

        /// <summary>
        /// This Method will return the list of Convert API results from the database. 
        /// </summary>
        /// <param name="FromDate"></param>
        /// <returns></returns>
        public async Task<List<CurrencyExchangeRates>> GetExchangeRatesHistory(DateTime FromDate)
        {
            return await _exchangeRepository.GetExchangeRates(FromDate);
        }

        /// <summary>
        /// This Method will return the external API's call history from the SQL database.
        /// </summary>
        /// <param name="FromDate"></param>
        /// <returns></returns>
        public async Task<List<CurrencyExchangeHistory>> GetExchangeLogs(DateTime FromDate)
        {
            return await _exchangeRepository.GetExchangeLogs(FromDate);
        }
    }
}
