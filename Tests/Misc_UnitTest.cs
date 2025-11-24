using Argo_Books.UI;
using System.Windows.Forms;

namespace Tests
{
    [TestClass]
    public class Misc_UnitTest
    {
        [TestMethod]
        public void TestBlankLoadingPanel()
        {
            Form testForm = new();
            LoadingPanel.InitBlankLoadingPanel();

            // Show the loading panel
            LoadingPanel.ShowBlankLoadingPanel(testForm);
            Assert.IsTrue(testForm.Controls.Contains(LoadingPanel.BlankLoadingPanelInstance));

            // Hide the loading panel
            LoadingPanel.HideBlankLoadingPanel(testForm);
            Assert.IsFalse(testForm.Controls.Contains(LoadingPanel.BlankLoadingPanelInstance));
        }

        [TestMethod]
        public void TestLoadingPanel()
        {
            Form testForm = new();
            LoadingPanel.InitLoadingPanel();

            // Show the loading panel
            LoadingPanel.ShowLoadingScreen(testForm, "Loading...");
            Assert.IsTrue(testForm.Controls.Contains(LoadingPanel.LoadingPanelInstance));

            // Hide the loading panel
            LoadingPanel.HideLoadingScreen(testForm);
            Assert.IsFalse(testForm.Controls.Contains(LoadingPanel.LoadingPanelInstance));
        }
    }
}