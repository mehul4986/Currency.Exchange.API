using Currency.Exchange.API.Helper;
using Currency.Exchange.API.Models;
using Currency.Exchange.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Currency.Exchange.API.Controllers
{
    public class ConvertController : ControllerBase
    {
        private readonly ILogger<ConvertController> _logger;
        private IExchangeService _exchangeService;

        public ConvertController(ILogger<ConvertController> logger, IExchangeService exchangeService)
        {
            _logger = logger;
            _exchangeService = exchangeService;
        }

        /// <summary>
        /// This API will return the exchanged value of the amount requested.
        /// </summary>
        /// <param name="basecode"></param>
        /// <param name="targetcode"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]")]        
        public async Task<ResponseModel> ConvertExchangeRate(string basecode, string targetcode, decimal amount)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                _logger.LogInformation("Retriving Exchange Rate - Validating Input.");

                if (
                        (string.IsNullOrEmpty(basecode) || !typeof(CurrencyCode).HasProperty(basecode)) ||
                        (string.IsNullOrEmpty(targetcode) || !typeof(CurrencyCode).HasProperty(targetcode))
                    )
                {
                    response.StatusCode = 400;
                    response.Status = "bad request";
                    response.Error = "Request data is invalid";
                }
                else
                {


                    _logger.LogInformation("Retriving and Converting Exchange Rate.");
                    var convertedRate = await _exchangeService.GetExchangeRates(basecode.ToUpper(), targetcode.ToUpper(), amount);
                    response.Result = convertedRate;
                }

            }
            catch (Exception ex)
            {
                response.StatusCode = 400;
                response.Status = "bad request - " + ex.Message.ToString();
                response.Error = "Error in retriving exchange rate. Please try again.";

                _logger.LogError(ex, string.Concat("Error in retriving exchange rate from ", basecode, " ERROR: ", ex.Message));
            }
            return response;
        }

        /// <summary>
        /// This API will return the list of Convert API results from the database.
        /// </summary>
        /// <param name="FromDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task<ResponseModel> GetConversionHistory(string? FromDate)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                _logger.LogInformation("Retriving conversion history - Validating Input.");

                if (string.IsNullOrEmpty(FromDate)) FromDate = "1900-01-01";

                if (!DateTime.TryParse(FromDate, out _))
                {
                    response.StatusCode = 400;
                    response.Status = "bad request";
                    response.Error = "From date is invalid.";
                }
                else
                {
                    _logger.LogInformation("Retriving conversion history.");
                    var saveCaseStatusResponse = await _exchangeService.GetExchangeLogs(Convert.ToDateTime(FromDate));
                    response.Result = saveCaseStatusResponse;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 400;
                response.Status = "bad request - " + ex.Message.ToString();
                response.Error = "Error in retriving conversion history. Please try again.";

                _logger.LogError(ex, string.Concat("Error in retriving Conversion History.", " ERROR: ", ex.Message));
            }
            return response;
        }

        /// <summary>
        /// This API will return the external API's call history from the SQL database.
        /// </summary>
        /// <param name="FromDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task<ResponseModel> GetConversionAPIHistory(string? FromDate)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                _logger.LogInformation("Retriving conversion api history - Validating Input.");

                if (string.IsNullOrEmpty(FromDate)) FromDate = "1900-01-01";

                if (!DateTime.TryParse(FromDate, out _))
                {
                    response.StatusCode = 400;
                    response.Status = "bad request";
                    response.Error = "From date is invalid.";
                }
                else
                {
                    _logger.LogInformation("Retriving conversion api history.");
                    var saveCaseStatusResponse = await _exchangeService.GetExchangeRatesHistory(Convert.ToDateTime(FromDate));
                    response.Result = saveCaseStatusResponse;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 400;
                response.Status = "bad request - " + ex.Message.ToString();
                response.Error = "Error in retriving conversion api history. Please try again.";

                _logger.LogError(ex, string.Concat("Error in retriving Conversion API History.", " ERROR: ", ex.Message));
            }
            return response;
        }
    }
}
