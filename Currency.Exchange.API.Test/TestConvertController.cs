using Currency.Exchange.API.Controllers;
using Currency.Exchange.API.Repositories.Interfaces;
using Currency.Exchange.API.Services;
using Currency.Exchange.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Currency.Exchange.API.Test
{
    [TestClass]
    public class TestConvertController
    {
        private readonly Mock<ILogger<ConvertController>> _logger;
        private readonly Mock<IExchangeService> _exchangeService;
        private readonly Mock<IExchangeRepository> _exchangeRepository;
        private readonly Mock<ICacheService> _cacheService;

        public TestConvertController()
        {
            _exchangeService = new Mock<IExchangeService>();
            _exchangeRepository = new Mock<IExchangeRepository>();
            _cacheService = new Mock<ICacheService>();
            _logger = new Mock<ILogger<ConvertController>>();
        }

        [TestMethod]
        public void GetExchangeRateTest()
        {
            var ctr = new ConvertController(_logger.Object, _exchangeService.Object);
            var data = ctr.ConvertExchangeRate("USD","INR",5);
            Assert.IsNotNull(data);
        }

        [TestMethod]
        public void GetConversionHistoryTest()
        {
            var ctr = new ConvertController(_logger.Object, _exchangeService.Object);
            var data = ctr.GetConversionHistory("2024-01-01");
            Assert.IsNotNull(data);
        }


        [TestMethod]
        public void GetConversionAPIHistoryTest()
        {
            var ctr = new ConvertController(_logger.Object, _exchangeService.Object);
            var data = ctr.GetConversionAPIHistory("2024-01-01");
            Assert.IsNotNull(data);

        }


    }
}