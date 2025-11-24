using Argo_Books.Classes;
using Argo_Books.Encryption;
using Newtonsoft.Json;
using Argo_Books.Language;

namespace Tests
{
    [TestClass]
    public class TranslationKeyGeneration_Tests
    {
        private string? tempCacheDir;

        [TestInitialize]
        public void Setup()
        {
            // Create a temporary directory for cache files
            tempCacheDir = Path.Combine(Path.GetTempPath(), "TranslationTestCache_" + Guid.NewGuid());
            Directory.CreateDirectory(tempCacheDir);

            // Set up test directories
            Directories.Cache_dir = tempCacheDir;
            Directories.Translations_file = Path.Combine(tempCacheDir, "translations.json");
            Directories.English_file = Path.Combine(tempCacheDir, "english_texts.json");

            SetupTestTranslationCache();
            LanguageManager.InitLanguageManager();
            EncryptionManager.Initialize();
            Directories.SetUniversalDirectories();
            DotEnv.Load();
        }
        private static void SetupTestTranslationCache()
        {
            Dictionary<string, Dictionary<string, string>> translationCache = new()
            {
                {
                    "fr", new Dictionary<string, string>
                    {
                        { "str_thepurchase{0}alreadyexistswouldyouliketoaddthispu", "Le {0} d'achat existe déjà. Souhaitez-vous quand même ajouter cet achat ?" },
                        { "str_amountcharged{0}{1}{2}isnotequaltothetotalpriceoft", "Le montant facturé ({0}{1} {2}) n'est pas égal au prix total de l'achat ({3}{4} {5}). La différence sera prise en compte." },
                    }
                }
            };

            string jsonContent = JsonConvert.SerializeObject(translationCache, Formatting.Indented);
            File.WriteAllText(Directories.Translations_file, jsonContent);
        }

        [TestMethod]
        public void TestKeyGeneration()
        {
            // Arrange
            Dictionary<string, string> messageVariants = new()
            {
                { "The purchase #{0} already exists. Would you like to add this purchase anyways?",
                  "str_thepurchase{0}alreadyexistswouldyouliketoaddthispu" },
                { "The sale #{0} already exists. Would you like to add this sale anyways?",
                  "str_thesale{0}alreadyexistswouldyouliketoaddthissalean" }
            };

            // Act & Assert
            foreach (KeyValuePair<string, string> variant in messageVariants)
            {
                string key = LanguageManager.GetStringKey(variant.Key);

                Assert.AreEqual(variant.Value, key,
                    $"Key generation failed for: '{variant.Key}'");
            }
        }

        [TestMethod]
        public void TestKeyGenerationWithFormats()
        {
            // Arrange
            Dictionary<string, string?> testCases = new()
            {
                { "Simple message", "str_simplemessage" },
                { "Message with spaces", "str_messagewithspaces" },
                { "Message with # punctuation!", "str_messagewithpunctuation" },
                { "message with MIXED case", "str_messagewithmixedcase" }
            };

            // Act & Assert
            foreach (KeyValuePair<string, string?> testCase in testCases)
            {
                string result = LanguageManager.GetStringKey(testCase.Key);
                Assert.AreEqual(testCase.Value, result,
                    $"Key generation failed for: '{testCase.Key}'");
            }
        }

        [TestMethod]
        public void TestTranslateString()
        {
            // Arrange
            Argo_Books.Properties.Settings.Default.Language = "French";  // Set to French for testing

            // Test messages with variables that should find translations
            Dictionary<string, string> testCases = new()
            {
                { "The purchase #{0} already exists. Would you like to add this purchase anyways?",
                  "Le {0} d'achat existe déjà. Souhaitez-vous quand même ajouter cet achat ?" },
                { "Amount charged ({0}{1} {2}) is not equal to the total price of the purchase ({3}{4} {5}). The difference will be accounted for.",
                  "Le montant facturé ({0}{1} {2}) n'est pas égal au prix total de l'achat ({3}{4} {5}). La différence sera prise en compte." }
            };

            // Act & Assert
            foreach (KeyValuePair<string, string> testCase in testCases)
            {
                // Try to translate the message
                string translation = LanguageManager.TranslateString(testCase.Key);

                Assert.AreEqual(testCase.Value, translation,
                    $"Translation failed for: '{testCase.Key}'");
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Reset language to English
            Argo_Books.Properties.Settings.Default.Language = "English";

            // Clean up temporary cache directory
            if (Directory.Exists(tempCacheDir))
            {
                Directory.Delete(tempCacheDir, true);
            }
        }
    }
}