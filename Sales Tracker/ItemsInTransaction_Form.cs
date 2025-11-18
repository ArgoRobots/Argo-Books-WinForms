using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.GridView;
using Sales_Tracker.Language;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;
using System.Text;

namespace Sales_Tracker
{
    /// <summary>
    /// Form for viewing and managing items within a transaction.
    /// </summary>
    public partial class ItemsInTransaction_Form : BaseForm
    {
        // Properties
        private readonly List<MainMenu_Form.Column> _columnsToLoad = [
            MainMenu_Form.Column.Product,
            MainMenu_Form.Column.Category,
            MainMenu_Form.Column.Country,
            MainMenu_Form.Column.Company,
            MainMenu_Form.Column.TotalItems,
            MainMenu_Form.Column.PricePerUnit
        ];
        private bool _hasChanges = false;

        // Init.
        public ItemsInTransaction_Form() : this(null) { }  // This is needed for TranslationGenerator.GenerateAllLanguageTranslationFiles()
        public ItemsInTransaction_Form(DataGridViewRow row)
        {
            InitializeComponent();
            if (row == null) { return; }

            DataGridViewManager.SelectedRowInMainMenu = row;

            SetTitle();

            ThemeManager.SetThemeForForm(this);
            LanguageManager.UpdateLanguageForControl(this);

            if (row.Tag is (List<string> itemList, TagData))
            {
                SetDataGridView(itemList);
            }

            // Attach event handlers to detect changes in DataGridView
            Items_DataGridView.CellValueChanged += (_, _) => _hasChanges = true;
            Items_DataGridView.RowsAdded += (_, _) => _hasChanges = true;
            Items_DataGridView.RowsRemoved += (_, _) => _hasChanges = true;

            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void SetTitle()
        {
            Title_Label.Text = LanguageManager.TranslateString(
                MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.Purchases ? "Items in purchase" : "Items in sale"
            );
        }

        // Form event handlers
        private void ItemsInTransaction_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainMenu_Form.IsProgramLoading = true;

            if (_hasChanges)
            {
                if (Items_DataGridView.Rows.Count == 0)
                {
                    Items_DataGridView.Rows.Remove(DataGridViewManager.SelectedRowInMainMenu);
                }
                else
                {
                    UpdateMainMenuRowTag();
                    DataGridViewManager.UpdateChargedDifferenceInRowWithMultipleItems(DataGridViewManager.SelectedRowInMainMenu);
                }

                DataGridViewManager.DataGridViewRowChanged(Items_DataGridView);
                MainMenu_Form.Instance.RefreshDataGridViewAndCharts();
            }
            MainMenu_Form.IsProgramLoading = false;
        }
        private void ItemsInTransaction_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
        }

        // Methods
        private void UpdateMainMenuRowTag()
        {
            List<string> items = [];

            if (MainMenu_Form.Instance.SelectedDataGridView.SelectedRows[0].Tag is (List<string> existingItems, TagData tagData))
            {
                string lastItem = null;
                if (existingItems.Last().StartsWith(ReadOnlyVariables.Receipt_text))
                {
                    lastItem = existingItems.Last();
                }

                foreach (DataGridViewRow row in Items_DataGridView.Rows)
                {
                    StringBuilder itemBuilder = new();
                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        if (i > 0) { itemBuilder.Append(','); }
                        itemBuilder.Append(row.Cells[i].Value);
                    }
                    items.Add(itemBuilder.ToString());
                }

                // Add the receipt file path if it exists
                if (lastItem != null)
                {
                    items.Add(lastItem);
                }

                MainMenu_Form.Instance.SelectedDataGridView.SelectedRows[0].Tag = (items, tagData);
            }
        }
        private void SetDataGridView(List<string> itemList)
        {
            Dictionary<MainMenu_Form.Column, string> columnHeaders;
            columnHeaders = (MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.Purchases)
                ? MainMenu_Form.Instance.PurchaseColumnHeaders
                : MainMenu_Form.Instance.SalesColumnHeaders;

            DataGridViewManager.InitializeDataGridView(Items_DataGridView, "Items_DataGridView", columnHeaders, _columnsToLoad, this);
            Items_DataGridView.RowsRemoved -= DataGridViewManager.DataGridView_RowsRemoved;
            Items_DataGridView.UserDeletingRow -= DataGridViewManager.DataGridView_UserDeletingRow;
            Items_DataGridView.Tag = MainMenu_Form.DataGridViewTag.ItemsInPurchase;

            LoadAllItemsInDataGridView(itemList);
            RefreshItemReturnStatus();
        }
        private void LoadAllItemsInDataGridView(List<string> itemList)
        {
            string defaultCurrencyType = DataFileManager.GetValue(AppDataSettings.DefaultCurrencyType);
            string receiptFilePath = null;
            int index = 0;

            // Check if the last item is the receipt file path
            if (itemList.Count > 0 && itemList[^1].StartsWith(ReadOnlyVariables.Receipt_text))
            {
                receiptFilePath = itemList[^1];
                index = 1;
            }

            // Add rows to DataGridView, and convert money values
            for (int i = 0; i < itemList.Count - index; i++)
            {
                string[] values = itemList[i].Split(',');

                // Check all values and replace null/empty with "-"
                for (int j = 0; j < values.Length; j++)
                {
                    if (string.IsNullOrEmpty(values[j]))
                    {
                        values[j] = ReadOnlyVariables.EmptyCell;
                    }
                }

                int sourceIndex = (defaultCurrencyType == "USD") ? 6 : 5;

                // Only format as decimal if it's not "-" and is a valid decimal
                if (values[sourceIndex] != ReadOnlyVariables.EmptyCell && decimal.TryParse(values[sourceIndex], out decimal result))
                {
                    values[5] = result.ToString("N2");
                }
                else
                {
                    values[5] = ReadOnlyVariables.EmptyCell;
                }

                int rowIndex = Items_DataGridView.Rows.Add(values);
                Items_DataGridView.Rows[rowIndex].Tag = receiptFilePath;
            }
        }
        public void RefreshItemReturnStatus()
        {
            DataGridViewManager.UpdateItemRowAppearance(Items_DataGridView, DataGridViewManager.SelectedRowInMainMenu);
        }
    }
}