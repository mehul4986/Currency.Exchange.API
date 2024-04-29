using Currency.Exchange.API.Models;
using Currency.Exchange.API.Repositories.Interfaces;
using Dapper;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace Currency.Exchange.API.Repositories
{
    public class ExchangeRepository : IExchangeRepository
    {
        /// <summary>
        /// This method will return the external API's call history from the SQL database.
        /// </summary>
        /// <param name="FromDate"></param>
        /// <returns></returns>
        public async Task<List<CurrencyExchangeRates>> GetExchangeRates(DateTime FromDate)
        {
            using (IDbConnection dbConnection = new SqlConnection(System.Environment.GetEnvironmentVariable("ExchangeRateConnStr")))
            {
                return await dbConnection.QueryAsync<CurrencyExchangeRates, string, CurrencyExchangeRates>("usp_get_conversion_rate",
                   (currencyExchangeRates, ExchangeRate) =>
                   {
                       currencyExchangeRates.rates = JsonConvert.DeserializeObject<CurrencyCode>(ExchangeRate);
                       return currencyExchangeRates;
                   },
                   splitOn: "ExchangeRate", param: new { TimeStamp = FromDate }, commandType: CommandType.StoredProcedure) as List<CurrencyExchangeRates>;
            }
        }

        /// <summary>
        /// This method will return the list of Convert API results from the database.
        /// </summary>
        /// <param name="FromDate"></param>
        /// <returns></returns>
        public async Task<List<CurrencyExchangeHistory>> GetExchangeLogs(DateTime FromDate)
        {
            using (IDbConnection dbConnection = new SqlConnection(System.Environment.GetEnvironmentVariable("ExchangeRateConnStr")))
            {
                var parameter = new DynamicParameters();

                parameter.Add("@TimeStamp", value: FromDate, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                return await dbConnection.QueryAsync<CurrencyExchangeHistory>("dbo.usp_get_conversion_log", parameter, commandType: CommandType.StoredProcedure) as List<CurrencyExchangeHistory>;
            }
        }


        /// <summary>
        /// This method will save the external api's response in the database.
        /// </summary>
        /// <param name="dBExchangeRates"></param>
        /// <returns></returns>
        public async Task<bool> SaveExchangeRates(DBExchangeRates dBExchangeRates)
        {
            bool inserted = false;
            using (IDbConnection dbConnection = new SqlConnection(System.Environment.GetEnvironmentVariable("ExchangeRateConnStr")))
            {
                var parameter = new DynamicParameters();

                parameter.Add("@TimeStamp", value: dBExchangeRates.TimeStamp, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                parameter.Add("@BaseCurrency", value: dBExchangeRates.BaseCurrency, dbType: DbType.String, direction: ParameterDirection.Input);
                parameter.Add("@ExchangeRate", value: dBExchangeRates.ExchangeRate, dbType: DbType.String, direction: ParameterDirection.Input);
                int rowCount = await dbConnection.QueryFirstOrDefaultAsync<int>("dbo.usp_insert_conversion_rate", parameter, commandType: CommandType.StoredProcedure);

                if (rowCount > 0)
                {
                    inserted = true;
                }
                return inserted;
            }
        }

        /// <summary>
        /// This method will save the Convert API's response in the database.
        /// </summary>
        /// <param name="currencyExchangeHistory"></param>
        /// <returns></returns>
        public async Task<bool> SaveExchangeLog(CurrencyExchangeHistory currencyExchangeHistory)
        {
            bool inserted = false;
            using (IDbConnection dbConnection = new SqlConnection(System.Environment.GetEnvironmentVariable("ExchangeRateConnStr")))
            {
                var parameter = new DynamicParameters();

                parameter.Add("@TimeStamp", value: currencyExchangeHistory.TimeStamp, dbType: DbType.DateTime, direction: ParameterDirection.Input);
                parameter.Add("@BaseCurrency", value: currencyExchangeHistory.BaseCurrency, dbType: DbType.String, direction: ParameterDirection.Input);
                parameter.Add("@BaseAmount", value: currencyExchangeHistory.BaseAmount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                parameter.Add("@ToCurrency", value: currencyExchangeHistory.ToCurrency, dbType: DbType.String, direction: ParameterDirection.Input);
                parameter.Add("@ToAmount", value: currencyExchangeHistory.ToAmount, dbType: DbType.Decimal, direction: ParameterDirection.Input);
                int rowCount = await dbConnection.QueryFirstOrDefaultAsync<int>("dbo.usp_insert_conversion_log", parameter, commandType: CommandType.StoredProcedure);

                if (rowCount > 0)
                {
                    inserted = true;
                }
                return inserted;
            }
        }
    }
}
