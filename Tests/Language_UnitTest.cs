using Argo_Books.Classes;
using Guna.UI2.WinForms;
using Newtonsoft.Json;
using Argo_Books.Language;
using System.Windows.Forms;

namespace Tests
{
    [TestClass]
    public class LanguageManager_Tests
    {
        private Form testForm;
        private Label testLabel;
        private Guna2Button testButton;
        private LinkLabel testLinkLabel;
        private string tempCacheDir;

        [TestInitialize]
        public void Setup()
        {
            // Create a temporary directory for cache files
            tempCacheDir = Path.Combine(Path.GetTempPath(), "TestCache_" + Guid.NewGuid());
            Directory.CreateDirectory(tempCacheDir);

            // Set up test directories
            Directories.Cache_dir = tempCacheDir;
            Directories.Translations_file = Path.Combine(tempCacheDir, "translations.json");
            Directories.English_file = Path.Combine(tempCacheDir, "english_texts.json");

            // Initialize test controls
            testForm = new Form
            {
                Name = "TestForm",
                Text = "Test Form"
            };

            testLabel = new Label
            {
                Name = "TestLabel",
                Text = "Test Label",
                AutoSize = true
            };

            testButton = new Guna2Button
            {
                Name = "TestButton",
                Text = "Test Button",
                AutoSize = true,
                Width = 150,
                Height = 40
            };

            testLinkLabel = new LinkLabel
            {
                Name = "TestLinkLabel",
                Text = "Click here to test",
                AutoSize = true,
                LinkArea = new LinkArea(5, 5)
            };

            testForm.Controls.Add(testLabel);
            testForm.Controls.Add(testButton);
            testForm.Controls.Add(testLinkLabel);

            // Pre-populate translation cache with test data
            SetupTestTranslations();

            // Initialize the manager
            LanguageManager.InitLanguageManager();
        }

        private static void SetupTestTranslations()
        {
            // Create and save test English cache
            Dictionary<string, string> englishCache = new()
            {
                { "TestForm", "Test Form" },
                { "TestForm.TestLabel", "Test Label" },
                { "TestForm.TestButton", "Test Button" },
                { "TestForm.TestLinkLabel_before", "Click" },
                { "TestForm.TestLinkLabel_link", "here" },
                { "TestForm.TestLinkLabel_after", "to test" }
            };
            string englishJson = JsonConvert.SerializeObject(englishCache, Formatting.Indented);
            File.WriteAllText(Directories.English_file, englishJson);

            // Create and save test translations cache
            Dictionary<string, Dictionary<string, string>> translationCache = new()
            {
                {
                    "fr", new Dictionary<string, string>
                    {
                        { "TestForm", "Formulaire de test" },
                        { "TestForm.TestLabel", "Étiquette de test" },
                        { "TestForm.TestButton", "Bouton de test" },
                        { "TestForm.TestLinkLabel_before", "Cliquez" },
                        { "TestForm.TestLinkLabel_link", "ici" },
                        { "TestForm.TestLinkLabel_after", "pour tester" }
                    }
                }
            };
            string translationsJson = JsonConvert.SerializeObject(translationCache, Formatting.Indented);
            File.WriteAllText(Directories.Translations_file, translationsJson);
        }

        [TestMethod]
        public void TestEnglishTextCaching()
        {
            // Act
            LanguageManager.UpdateLanguageForControl(testForm);

            // Assert
            string cachedEnglish = File.ReadAllText(Directories.English_file);
            Dictionary<string, string>? englishCache = JsonConvert.DeserializeObject<Dictionary<string, string>>(cachedEnglish);

            // Verify form text was cached
            Assert.IsTrue(englishCache.ContainsKey("TestForm"));
            Assert.AreEqual("Test Form", englishCache["TestForm"]);

            // Verify label text was cached
            Assert.IsTrue(englishCache.ContainsKey("TestForm.TestLabel"));
            Assert.AreEqual("Test Label", englishCache["TestForm.TestLabel"]);

            // Verify button text was cached
            Assert.IsTrue(englishCache.ContainsKey("TestForm.TestButton"));
            Assert.AreEqual("Test Button", englishCache["TestForm.TestButton"]);
        }

        [TestMethod]
        public void TestLinkLabelCachingAndFormatting()
        {
            // Act
            LanguageManager.UpdateLanguageForControl(testLinkLabel);

            // Assert
            string cachedEnglish = File.ReadAllText(Directories.English_file);
            Dictionary<string, string>? englishCache = JsonConvert.DeserializeObject<Dictionary<string, string>>(cachedEnglish);

            // Verify the link parts were cached correctly
            Assert.IsTrue(englishCache.ContainsKey("TestForm.TestLinkLabel_before"));
            Assert.IsTrue(englishCache.ContainsKey("TestForm.TestLinkLabel_link"));
            Assert.IsTrue(englishCache.ContainsKey("TestForm.TestLinkLabel_after"));

            Assert.AreEqual("Click", englishCache["TestForm.TestLinkLabel_before"]);
            Assert.AreEqual("here", englishCache["TestForm.TestLinkLabel_link"]);
            Assert.AreEqual("to test", englishCache["TestForm.TestLinkLabel_after"]);

            // Verify link area is preserved
            Assert.AreEqual(1, testLinkLabel.Links.Count);
            Assert.IsTrue(testLinkLabel.Links[0].Length > 0);
        }

        [TestMethod]
        public void TestButtonSizeAdjustment()
        {
            // Arrange
            float originalFontSize = testButton.Font.Size;
            testButton.Width = 80;  // Make button smaller to force text adjustment

            // Act
            string longText = "A very long button text that should not fit";
            LanguageManager.AdjustButtonFontSize(testButton, longText);

            // Assert
            Assert.IsTrue(testButton.Font.Size < originalFontSize,
                "Font size should be reduced for longer text");
            Assert.IsTrue(testButton.Width >= testButton.PreferredSize.Width,
                "Text should fit within button width");
        }

        [TestMethod]
        public void TestLanguageSelection()
        {
            // Act
            List<KeyValuePair<string, string>> languages = LanguageManager.GetLanguages();
            List<string> languageNames = [.. languages.Select(kvp => kvp.Key)];

            // Assert
            Assert.IsTrue(languages.Count > 0);

            // Verify essential languages are present
            Assert.IsTrue(languages.Any(l => l.Key == "English" && l.Value == "en"));
            Assert.IsTrue(languages.Any(l => l.Key == "French" && l.Value == "fr"));

            // Verify language names match the full list
            Assert.AreEqual(languages.Count, languageNames.Count);
        }

        [TestMethod]
        public void TestTranslateString_ReturnsOriginalIfNoTranslation()
        {
            // Arrange
            Argo_Books.Properties.Settings.Default.Language = "French";
            string messageWithoutTranslation = "This message has no translation available";

            // Act
            string result = LanguageManager.TranslateString(messageWithoutTranslation);

            // Assert
            Assert.AreEqual(messageWithoutTranslation, result,
                "Should return original text when no translation is available");
        }

        [TestMethod]
        public void TestTranslateString_ReturnsOriginalForEnglish()
        {
            // Arrange
            Argo_Books.Properties.Settings.Default.Language = "English";
            string originalMessage = "Test message in English";

            // Act
            string result = LanguageManager.TranslateString(originalMessage);

            // Assert
            Assert.AreEqual(originalMessage, result,
                "Should return original text for English language");
        }


        [TestCleanup]
        public void Cleanup()
        {
            testForm.Dispose();

            // Clean up temporary cache directory
            if (Directory.Exists(tempCacheDir))
            {
                Directory.Delete(tempCacheDir, true);
            }
        }
    }
}