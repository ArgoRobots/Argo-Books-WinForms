using Argo_Books.AISearch;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using FieldInfo = System.Reflection.FieldInfo;

namespace Tests
{
    [TestClass]
    public class AIQueryTranslator_UnitTest
    {
        private Mock<HttpMessageHandler> _mockHttpHandler;
        private HttpClient _httpClient;
        private AIQueryTranslator _translator;
        private const string TestApiKey = "test-api-key";

        [TestInitialize]
        public void Setup()
        {
            // Set up mock HTTP handler to simulate API responses
            _mockHttpHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpHandler.Object);

            // Use reflection to create AIQueryTranslator with our mocked HttpClient
            _translator = new AIQueryTranslator(TestApiKey);

            // Use reflection to replace the private _httpClient field
            FieldInfo? httpClientField = typeof(AIQueryTranslator).GetField("_httpClient",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            httpClientField.SetValue(_translator, _httpClient);
        }

        [TestMethod]
        public async Task TranslateQueryAsync_ValidQuery_ReturnsFormattedQuery()
        {
            // Arrange
            string naturalLanguageQuery = "products from usa with high discount";
            string expectedTranslation = "+Country of origin:United States +Discount:>5";

            SetupMockApiResponse(expectedTranslation);

            // Act
            string result = await _translator.TranslateQueryAsync(naturalLanguageQuery);

            // Assert
            Assert.AreEqual(expectedTranslation, result);
        }

        [TestMethod]
        public async Task TranslateQueryAsync_EmptyQuery_ReturnsEmptyString()
        {
            // Arrange
            string emptyQuery = "";

            // Act
            string result = await _translator.TranslateQueryAsync(emptyQuery);

            // Assert
            Assert.AreEqual("", result);

            // Verify that the API request was not sent
            _mockHttpHandler.Protected().Verify(
                "SendAsync",
                Times.Never(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        public async Task TranslateQueryAsync_RegionalQuery_TranslatesToCountryList()
        {
            // Arrange
            string naturalLanguageQuery = "orders from europe";
            string expectedTranslation = "+Country of origin:France|Germany|United Kingdom|Italy|Spain|Netherlands|Sweden|Switzerland|Norway|Finland|Belgium|Denmark|Austria|Portugal|Greece|Ireland";

            SetupMockApiResponse(expectedTranslation);

            // Act
            string result = await _translator.TranslateQueryAsync(naturalLanguageQuery);

            // Assert
            Assert.AreEqual(expectedTranslation, result);
        }

        [TestMethod]
        public async Task TranslateQueryAsync_QualitativeTerms_TranslatesToNumericalValues()
        {
            // Arrange
            string naturalLanguageQuery = "expensive purchases";
            string expectedTranslation = "+Total:>200";

            SetupMockApiResponse(expectedTranslation);

            // Act
            string result = await _translator.TranslateQueryAsync(naturalLanguageQuery);

            // Assert
            Assert.AreEqual(expectedTranslation, result);
        }

        [TestMethod]
        public async Task TranslateQueryAsync_CountryAliases_MapsToCorrectCountry()
        {
            // Arrange
            string naturalLanguageQuery = "sales to UK";
            string expectedTranslation = "+Country of destination:United Kingdom";

            SetupMockApiResponse(expectedTranslation);

            // Act
            string result = await _translator.TranslateQueryAsync(naturalLanguageQuery);

            // Assert
            Assert.AreEqual(expectedTranslation, result);
        }

        private void SetupMockApiResponse(string translation)
        {
            var responseObject = new
            {
                choices = new[]
                {
                    new
                    {
                        message = new
                        {
                            content = translation
                        }
                    }
                }
            };

            HttpResponseMessage response = new()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(responseObject))
            };

            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
        }
    }
}