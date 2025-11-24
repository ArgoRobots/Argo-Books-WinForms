using Argo_Books.Classes;
using Argo_Books.Encryption;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace Tests
{
    [TestClass]
    public class GoogleAPI_UnitTest
    {
        private static string? _testSpreadsheetId;

        [ClassInitialize]
        public static void ClassSetup(TestContext _)
        {
            Directories.SetUniversalDirectories();
            ArgoCompany.InitCacheFiles();
            EncryptionManager.Initialize();
            DotEnv.Load();
        }

        [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
        public static async Task ClassCleanup()
        {
            if (!string.IsNullOrEmpty(_testSpreadsheetId))
            {
                await DeleteTestSpreadsheet(_testSpreadsheetId);
            }
        }

        [TestMethod]
        public async Task TestCredentialsLoading()
        {
            // Test passes if credentials load without throwing an exception
            UserCredential credential = await GoogleCredentialsManager.GetUserCredentialAsync();

            // Verify we can create a service with the credentials
            _ = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Argo Books"
            });
        }

        [TestMethod]
        public async Task TestGoogleSheetsAccess()
        {
            UserCredential credential = await GoogleCredentialsManager.GetUserCredentialAsync();

            SheetsService service = new(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Argo Books"
            });

            Spreadsheet spreadsheet = new()
            {
                Properties = new SpreadsheetProperties { Title = "Test Access" }
            };

            Spreadsheet result = await service.Spreadsheets.Create(spreadsheet).ExecuteAsync();
            _testSpreadsheetId = result.SpreadsheetId;

            Assert.IsNotNull(result.SpreadsheetId, "Spreadsheet should be created");
        }

        [TestMethod]
        public async Task TestSpreadsheetCreation()
        {
            UserCredential credential = await GoogleCredentialsManager.GetUserCredentialAsync();

            SheetsService service = new(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Argo Books"
            });

            Spreadsheet spreadsheet = new()
            {
                Properties = new SpreadsheetProperties
                {
                    Title = $"Unit Test - {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
                }
            };

            Spreadsheet result = await service.Spreadsheets.Create(spreadsheet).ExecuteAsync();
            _testSpreadsheetId = result.SpreadsheetId;

            Assert.IsNotNull(result.SpreadsheetId);
            Assert.IsNotNull(result.Properties);
            Assert.IsTrue(result.Properties.Title.Contains("Unit Test"));
        }

        [TestMethod]
        public async Task TestWriteData()
        {
            UserCredential credential = await GoogleCredentialsManager.GetUserCredentialAsync();

            SheetsService service = new(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Argo Books"
            });

            // Create spreadsheet
            Spreadsheet spreadsheet = new()
            {
                Properties = new SpreadsheetProperties { Title = "Test Write Data" }
            };

            Spreadsheet result = await service.Spreadsheets.Create(spreadsheet).ExecuteAsync();
            _testSpreadsheetId = result.SpreadsheetId;

            // Write data
            ValueRange valueRange = new()
            {
                Values =
                [
                    ["Header1", "Header2"],
                    ["Value1", "Value2"]
                ]
            };

            SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest =
                service.Spreadsheets.Values.Update(valueRange, result.SpreadsheetId, "A1:B2");
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

            UpdateValuesResponse updateResponse = await updateRequest.ExecuteAsync();

            Assert.AreEqual(2, updateResponse.UpdatedRows);
        }

        [TestMethod]
        public async Task TestReadData()
        {
            UserCredential credential = await GoogleCredentialsManager.GetUserCredentialAsync();

            SheetsService service = new(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Argo Books"
            });

            // Create and write data
            Spreadsheet spreadsheet = new()
            {
                Properties = new SpreadsheetProperties { Title = "Test Read Data" }
            };

            Spreadsheet result = await service.Spreadsheets.Create(spreadsheet).ExecuteAsync();
            _testSpreadsheetId = result.SpreadsheetId;

            ValueRange valueRange = new()
            {
                Values =
                [
                    ["Test", "Data"]
                ]
            };

            SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest =
                service.Spreadsheets.Values.Update(valueRange, result.SpreadsheetId, "A1:B1");
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            await updateRequest.ExecuteAsync();

            // Read data
            SpreadsheetsResource.ValuesResource.GetRequest getRequest =
                service.Spreadsheets.Values.Get(result.SpreadsheetId, "A1:B1");

            ValueRange readResponse = await getRequest.ExecuteAsync();

            Assert.IsNotNull(readResponse.Values);
            Assert.AreEqual(1, readResponse.Values.Count);
            Assert.AreEqual("Test", readResponse.Values[0][0]);
            Assert.AreEqual("Data", readResponse.Values[0][1]);
        }

        [TestMethod]
        public async Task TestMultipleSheets()
        {
            UserCredential credential = await GoogleCredentialsManager.GetUserCredentialAsync();

            SheetsService service = new(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Argo Books"
            });

            // Create spreadsheet with multiple sheets
            Spreadsheet spreadsheet = new()
            {
                Properties = new SpreadsheetProperties { Title = "Test Multiple Sheets" },
                Sheets =
                [
                    new() { Properties = new SheetProperties { Title = "Sheet1" } },
                    new() { Properties = new SheetProperties { Title = "Sheet2" } }
                ]
            };

            Spreadsheet result = await service.Spreadsheets.Create(spreadsheet).ExecuteAsync();
            _testSpreadsheetId = result.SpreadsheetId;

            Assert.IsNotNull(result.Sheets);
            Assert.AreEqual(2, result.Sheets.Count);
            Assert.AreEqual("Sheet1", result.Sheets[0].Properties.Title);
            Assert.AreEqual("Sheet2", result.Sheets[1].Properties.Title);
        }

        [TestMethod]
        public async Task TestBatchUpdate()
        {
            UserCredential credential = await GoogleCredentialsManager.GetUserCredentialAsync();

            SheetsService service = new(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Argo Books"
            });

            // Create spreadsheet
            Spreadsheet spreadsheet = new()
            {
                Properties = new SpreadsheetProperties { Title = "Test Batch Update" }
            };

            Spreadsheet result = await service.Spreadsheets.Create(spreadsheet).ExecuteAsync();
            _testSpreadsheetId = result.SpreadsheetId;

            // Batch update - format header row
            BatchUpdateSpreadsheetRequest batchRequest = new()
            {
                Requests =
                [
                    new()
                    {
                        RepeatCell = new RepeatCellRequest
                        {
                            Range = new GridRange
                            {
                                SheetId = 0,
                                StartRowIndex = 0,
                                EndRowIndex = 1,
                                StartColumnIndex = 0,
                                EndColumnIndex = 2
                            },
                            Cell = new CellData
                            {
                                UserEnteredFormat = new CellFormat
                                {
                                    BackgroundColor = new Color { Red = 0.8f, Green = 0.8f, Blue = 0.8f },
                                    TextFormat = new TextFormat { Bold = true }
                                }
                            },
                            Fields = "userEnteredFormat(backgroundColor,textFormat)"
                        }
                    }
                ]
            };

            BatchUpdateSpreadsheetResponse batchResponse = await service.Spreadsheets
                .BatchUpdate(batchRequest, result.SpreadsheetId)
                .ExecuteAsync();

            Assert.IsNotNull(batchResponse.Replies);
            Assert.AreEqual(1, batchResponse.Replies.Count);
        }

        private static async Task DeleteTestSpreadsheet(string spreadsheetId)
        {
            try
            {
                UserCredential credential = await GoogleCredentialsManager.GetUserCredentialAsync();

                DriveService driveService = new(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Argo Books"
                });

                await driveService.Files.Delete(spreadsheetId).ExecuteAsync();
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}