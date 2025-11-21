using Guna.UI2.WinForms;
using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.Language;
using Sales_Tracker.LostProduct;
using Sales_Tracker.Rentals;
using Sales_Tracker.ReturnProduct;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;
using System.ComponentModel;

namespace Sales_Tracker.GridView
{
    /// <summary>
    /// Manages the setup, interactions, and event handling for DataGridView components.
    /// </summary>
    public class DataGridViewManager
    {
        // Properties
        private static DataGridViewRow _removedRow;
        private static readonly string _deleteAction = LanguageManager.TranslateString("deleted");
        private static DataGridViewCell _currentlyHoveredNoteCell;
        private static int RowHeight => (int)(35 * DpiHelper.GetRelativeDpiScale());
        private static int ColumnHeaderHeight => (int)(60 * DpiHelper.GetRelativeDpiScale());

        // Getters
        public static bool DoNotDeleteRows { get; set; }
        public static DataGridViewRow SelectedRowInMainMenu { get; set; }

        // Construct DataGridView
        public static void InitializeDataGridView<TEnum>(Guna2DataGridView dataGridView, string name, Dictionary<TEnum, string> columnHeaders, List<TEnum>? columnsToLoad, Control? parent) where TEnum : Enum
        {
            dataGridView.Name = name;
            dataGridView.ReadOnly = true;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.ColumnHeadersHeight = ColumnHeaderHeight;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView.RowTemplate.Height = RowHeight;
            dataGridView.RowTemplate.DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            dataGridView.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            dataGridView.DefaultCellStyle.Font = new Font("Segoe UI", 12);
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = CustomColors.Text;
            dataGridView.Theme = CustomColors.DataGridViewTheme;
            dataGridView.BackgroundColor = CustomColors.ControlBack;
            dataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView.ScrollBars = ScrollBars.Vertical;

            if (parent == MainMenu_Form.Instance)
            {
                dataGridView.ColumnWidthChanged += DataGridView_ColumnWidthChanged;
            }

            dataGridView.UserDeletingRow += DataGridView_UserDeletingRow;
            dataGridView.RowsRemoved += DataGridView_RowsRemoved;
            dataGridView.MouseUp += DataGridView_MouseUp;
            dataGridView.KeyDown += DataGridView_KeyDown;
            dataGridView.CellMouseClick += DataGridView_CellMouseClick;
            dataGridView.CellMouseMove += DataGridView_CellMouseMove;
            dataGridView.CellMouseLeave += DataGridView_CellMouseLeave;
            dataGridView.ColumnHeaderMouseClick += DataGridView_ColumnHeaderMouseClick;
            dataGridView.SortCompare += DataGridView_SortCompare;

            LoadColumns(dataGridView, columnHeaders, columnsToLoad);
            ThemeManager.UpdateDataGridViewHeaderTheme(dataGridView);

            parent?.Controls.Add(dataGridView);
        }

        // DataGridView event handlers
        public static void DataGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (MainMenu_Form.IsProgramLoading) { return; }
            MainMenu_Form.Instance.AlignTotalLabels();
        }
        public static void DataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MainMenu_Form.IsProgramLoading) { return; }

            if (DoNotDeleteRows)
            {
                e.Cancel = true;
                return;
            }

            _removedRow = e.Row;

            switch (MainMenu_Form.Instance.Selected)
            {
                case MainMenu_Form.SelectedOption.Purchases:
                    HandlePurchasesDeletion(e);
                    break;

                case MainMenu_Form.SelectedOption.Sales:
                    HandleSalesDeletion(e);
                    break;

                case MainMenu_Form.SelectedOption.Rentals:
                    HandleRentalsDeletion(e);
                    break;

                case MainMenu_Form.SelectedOption.ProductPurchases:
                case MainMenu_Form.SelectedOption.ProductSales:
                case MainMenu_Form.SelectedOption.ProductRentals:
                    HandleProductDeletion(e);
                    break;

                case MainMenu_Form.SelectedOption.CategoryPurchases:
                case MainMenu_Form.SelectedOption.CategorySales:
                case MainMenu_Form.SelectedOption.CategoryRentals:
                    HandleCategoryDeletion(e);
                    break;

                case MainMenu_Form.SelectedOption.Accountants:
                    HandleAccountantsDeletion(e);
                    break;

                case MainMenu_Form.SelectedOption.Companies:
                    HandleCompaniesDeletion(e);
                    break;

                case MainMenu_Form.SelectedOption.ItemsInPurchase:
                case MainMenu_Form.SelectedOption.ItemsInSale:
                    HandleItemsDeletion(sender, e);
                    break;
            }
        }
        public static void DataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (MainMenu_Form.IsProgramLoading) { return; }

            DataGridViewRowChanged((Guna2DataGridView)sender);

            // Remove receipt from file
            if (MainMenu_Form.Instance.Selected is MainMenu_Form.SelectedOption.Purchases
                or MainMenu_Form.SelectedOption.Sales
                or MainMenu_Form.SelectedOption.Rentals
                && _removedRow?.Tag != null)

            {
                string tagValue = "";

                if (_removedRow.Tag is (List<string> tagList, TagData))
                {
                    tagValue = ReceiptManager.ProcessReceiptTextFromRowTag(tagList[^1]);
                }
                else if (_removedRow.Tag is (string tagString, TagData))
                {
                    tagValue = ReceiptManager.ProcessReceiptTextFromRowTag(tagString);
                }

                if (tagValue != "")
                {
                    Directories.DeleteFile(tagValue);
                }

                _removedRow = null;
            }
        }
        public static void DataGridViewRowChanged(Guna2DataGridView dataGridView)
        {
            MainMenu_Form.SelectedOption selected = MainMenu_Form.Instance.Selected;

            if (selected is MainMenu_Form.SelectedOption.Purchases
                or MainMenu_Form.SelectedOption.Sales)
            {
                MainMenu_Form.Instance.UpdateTotalLabels();
                MainMenu_Form.Instance.LoadOrRefreshMainCharts();
                MainMenu_Form.SaveDataGridViewToFileAsJson(dataGridView, selected);
            }
            else if (selected is MainMenu_Form.SelectedOption.Rentals)
            {
                // Rentals are stored in RentalInventory.txt (single source of truth)
                // Don't save to rentals.txt
                MainMenu_Form.Instance.UpdateTotalLabels();
                MainMenu_Form.Instance.LoadOrRefreshMainCharts();
            }
            else if (selected is MainMenu_Form.SelectedOption.CategoryPurchases or
                MainMenu_Form.SelectedOption.CategorySales or
                MainMenu_Form.SelectedOption.CategoryRentals or
                MainMenu_Form.SelectedOption.ProductPurchases or
                MainMenu_Form.SelectedOption.ProductSales or
                MainMenu_Form.SelectedOption.ProductRentals)
            {
                MainMenu_Form.Instance.SaveCategoriesToFile(selected);
            }
            else if (selected is MainMenu_Form.SelectedOption.Customers)
            {
                MainMenu_Form.Instance.SaveCustomersToFile();
            }
            else if (selected is MainMenu_Form.SelectedOption.Accountants
                or MainMenu_Form.SelectedOption.Companies
                or MainMenu_Form.SelectedOption.Customers)
            {
                MainMenu_Form.SaveDataGridViewToFile(dataGridView, selected);
            }
        }
        private static void DataGridView_MouseUp(object sender, MouseEventArgs e)
        {
            Guna2DataGridView grid = (Guna2DataGridView)sender;

            SelectRowAndDeselectAllOthers(grid, e);

            if (!IsValidRightClick(grid, e))
            {
                return;
            }

            DataGridView.HitTestInfo info = grid.HitTest(e.X, e.Y);
            if (info.Type != DataGridViewHitTestType.Cell)
            {
                return;
            }

            // First hide other right click panels
            MainMenu_Form.CloseRightClickPanels();

            ConfigureRightClickDataGridViewMenuButtons(grid);
            PositionRightClickDataGridViewMenu(grid, e, info);
        }
        private static void DataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                Guna2DataGridView grid = (Guna2DataGridView)sender;
                CustomMessageBoxResult result;

                if (grid.SelectedRows.Count == 1)
                {
                    result = CustomMessageBox.Show("Delete row", "Are you sure you want to delete this row?",
                       CustomMessageBoxIcon.Exclamation, CustomMessageBoxButtons.OkCancel);
                }
                else
                {
                    result = CustomMessageBox.Show("Delete rows", "Are you sure you want to delete the selected rows?",
                        CustomMessageBoxIcon.Exclamation, CustomMessageBoxButtons.OkCancel);
                }


                if (result != CustomMessageBoxResult.Ok)
                {
                    DoNotDeleteRows = true;
                    UnselectAllRowsInDataGridView(grid);
                }
            }
        }
        private static void DataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView dataGridView = (DataGridView)sender;

            if (IsNoteCell(e, dataGridView))
            {
                DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                string idCell = dataGridView.Rows[e.RowIndex].Cells[ReadOnlyVariables.ID_column].Value.ToString();

                if (IsClickOnText(cell, out Rectangle hitbox))
                {
                    Point mousePos = dataGridView.PointToClient(Cursor.Position);
                    if (hitbox.Contains(mousePos))
                    {
                        AddUnderlineToCell(cell);
                        string type = MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.Purchases ? "purchase" : "sale";

                        CustomMessageBox.ShowWithFormat("Note for {0} {1}",
                            cell.Tag?.ToString(),
                            CustomMessageBoxIcon.Info, CustomMessageBoxButtons.Ok,
                            type, idCell);
                    }
                }
            }
        }
        private static void DataGridView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView dataGridView = (DataGridView)sender;

            if (IsNoteCell(e, dataGridView))
            {
                DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                _currentlyHoveredNoteCell = cell;  // Track the current note cell

                if (IsClickOnText(cell, out Rectangle hitbox))
                {
                    // Draw a rectangle around the hitbox for debugging
                    //using (Graphics g = dataGridView.CreateGraphics())
                    //{
                    //    g.DrawRectangle(Pens.Red, hitbox);
                    //}

                    Point mousePos = dataGridView.PointToClient(Cursor.Position);
                    UpdateCellStyleBasedOnMousePosition(cell, hitbox, mousePos);
                }
            }
            else
            {
                // If we're not in a note cell, clear any previously hovered note cell
                if (_currentlyHoveredNoteCell != null)
                {
                    RestoreNoteCellUnderline(_currentlyHoveredNoteCell);
                    _currentlyHoveredNoteCell = null;
                }
            }
        }
        private static void DataGridView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (_currentlyHoveredNoteCell != null)
            {
                RestoreNoteCellUnderline(_currentlyHoveredNoteCell);
                _currentlyHoveredNoteCell = null;
            }
        }
        private static void DataGridView_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            // Show right click panel
            if (e.Button == MouseButtons.Right && e.RowIndex == -1 && e.ColumnIndex >= 0)
            {
                ColumnVisibilityPanel.Show(sender as Guna2DataGridView, e);
            }

            UpdateRowColors((DataGridView)sender);
        }
        private static void DataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            // Check if this is a money column
            if (MoneyColumns.Contains(((Guna2DataGridView)sender).Columns[e.Column.Index].Name))
            {
                // Parse values as decimals for proper numeric comparison
                if (decimal.TryParse(e.CellValue1?.ToString().Replace(MainMenu_Form.CurrencySymbol, "").Replace(",", ""), out decimal num1) &&
                    decimal.TryParse(e.CellValue2?.ToString().Replace(MainMenu_Form.CurrencySymbol, "").Replace(",", ""), out decimal num2))
                {
                    e.SortResult = num1.CompareTo(num2);
                    e.Handled = true;
                }
            }
        }
        private static readonly HashSet<string> MoneyColumns =
        [
            ReadOnlyVariables.PricePerUnit_column,
            ReadOnlyVariables.Shipping_column,
            ReadOnlyVariables.Tax_column,
            ReadOnlyVariables.Fee_column,
            ReadOnlyVariables.Discount_column,
            ReadOnlyVariables.ChargedDifference_column,
            ReadOnlyVariables.Total_column
        ];

        // Methods for DataGridView_UserDeletingRow
        private static void HandlePurchasesDeletion(DataGridViewRowCancelEventArgs e)
        {
            string columnName = ReadOnlyVariables.ID_column;
            string name = e.Row.Cells[columnName].Value?.ToString();

            string message = $"Deleted purchase '{name}'";
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 2, message);
        }
        private static void HandleSalesDeletion(DataGridViewRowCancelEventArgs e)
        {
            string columnName = ReadOnlyVariables.ID_column;
            string name = e.Row.Cells[columnName].Value?.ToString();

            string message = $"Deleted sale '{name}'";
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 2, message);
        }
        private static void HandleRentalsDeletion(DataGridViewRowCancelEventArgs e)
        {
            string rentalItemIDColumn = Rentals_Form.Column.RentalItemID.ToString();
            string rentalItemID = e.Row.Cells[rentalItemIDColumn].Value?.ToString();

            string productNameColumn = Rentals_Form.Column.ProductName.ToString();
            string name = e.Row.Cells[productNameColumn].Value?.ToString();

            // Remove the item from the rental inventory
            if (!string.IsNullOrEmpty(rentalItemID))
            {
                RentalInventoryManager.RemoveRentalItem(rentalItemID);
            }

            string message = $"Deleted rental '{name}'";
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 2, message);
        }
        private static void HandleProductDeletion(DataGridViewRowCancelEventArgs e)
        {
            string type = "product";
            string columnName = Products_Form.Column.ProductName.ToString();
            string valueBeingRemoved = e.Row.Cells[columnName].Value?.ToString();

            List<Category>? categoryList = GetCategoryList();

            if (categoryList == null)
            {
                CustomMessageBox.Show(
                   "Cannot delete category",
                   "Failed to delete the product",
                   CustomMessageBoxIcon.Error,
                   CustomMessageBoxButtons.Ok);
                e.Cancel = true;
                return;
            }

            if (IsThisBeingUsedByDataGridView(type, ReadOnlyVariables.Product_column, valueBeingRemoved, _deleteAction))
            {
                e.Cancel = true;
                return;
            }

            // Remove product from list
            categoryList.ForEach(c => c.ProductList.Remove(c.ProductList.FirstOrDefault(p => p.Name == valueBeingRemoved)));

            // In case the product name that is being deleted is in the TextBox
            Products_Form.Instance.ValidateProductNameTextBox();

            string message = $"Deleted {type} '{valueBeingRemoved}'";
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 2, message);
        }
        private static void HandleCategoryDeletion(DataGridViewRowCancelEventArgs e)
        {
            string columnName = Categories_Form.Column.CategoryName.ToString();
            string valueBeingRemoved = e.Row.Cells[columnName].Value?.ToString();

            List<Category>? categoryList = GetCategoryList();

            if (categoryList == null)
            {
                CustomMessageBox.Show(
                   "Cannot delete category",
                   "Failed to delete the category",
                   CustomMessageBoxIcon.Error,
                   CustomMessageBoxButtons.Ok);
                e.Cancel = true;
                return;
            }

            if (!CanCategoryBeMovedOrDeleted(valueBeingRemoved, categoryList, _deleteAction))
            {
                e.Cancel = true;
                return;
            }

            // Remove category from list
            categoryList.Remove(categoryList.FirstOrDefault(c => c.Name == valueBeingRemoved));

            // In case the category name that is being deleted is in the TextBox
            Categories_Form.Instance.VaidateCategoryTextBox();

            string message = $"Deleted category '{valueBeingRemoved}'";
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 2, message);
        }
        private static void HandleAccountantsDeletion(DataGridViewRowCancelEventArgs e)
        {
            string type = "accountant";
            string columnName = Accountants_Form.Column.AccountantName.ToString();
            string valueBeingRemoved = e.Row.Cells[columnName].Value?.ToString();

            if (IsThisBeingUsedByDataGridView(type, ReadOnlyVariables.Accountant_column, valueBeingRemoved, _deleteAction))
            {
                e.Cancel = true;
                return;
            }

            if (MainMenu_Form.SelectedAccountant == valueBeingRemoved)
            {
                CustomMessageBox.Show(
                    "Cannot delete accountant",
                    "You cannot delete the accountant you are currently signed in as.",
                    CustomMessageBoxIcon.Info,
                    CustomMessageBoxButtons.Ok);

                e.Cancel = true;
                return;
            }

            if (MainMenu_Form.Instance.AccountantList.Count == 1)
            {
                CustomMessageBox.Show(
                    "Cannot delete accountant",
                    "You must have at least one accountant.",
                    CustomMessageBoxIcon.Info,
                    CustomMessageBoxButtons.Ok);

                e.Cancel = true;
                return;
            }

            // Remove accountant from list
            MainMenu_Form.Instance.AccountantList.Remove(valueBeingRemoved);

            // In case the accountant name that is being deleted is in the TextBox
            Accountants_Form.Instance.VaidateAccountantTextBox();

            string message = $"Deleted {type} '{valueBeingRemoved}'";
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 2, message);
        }
        private static void HandleCompaniesDeletion(DataGridViewRowCancelEventArgs e)
        {
            string type = "company";
            string columnName = Companies_Form.Column.Company.ToString();
            string valueBeingRemoved = e.Row.Cells[columnName].Value?.ToString();

            if (IsThisBeingUsedByDataGridView(type, ReadOnlyVariables.Company_column, valueBeingRemoved, _deleteAction))
            {
                e.Cancel = true;
                return;
            }

            // Remove company from list
            MainMenu_Form.Instance.CompanyList.Remove(valueBeingRemoved);

            // In case the company name that is being deleted is in the TextBox
            Companies_Form.Instance.ValidateCompanyTextBox();

            string message = $"Deleted {type} '{valueBeingRemoved}'";
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 2, message);
        }
        private static void HandleItemsDeletion(object sender, DataGridViewRowCancelEventArgs e)
        {
            string columnName = ReadOnlyVariables.Product_column;
            string productName = e.Row.Cells[columnName].Value?.ToString();

            if (!(SelectedRowInMainMenu.Tag is (List<string> itemList, TagData)))
            {
                return;
            }

            string transactionType = MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.ItemsInPurchase
                ? "purchase" : "sale";

            if (e.Row.DataGridView.Rows.Count == 1)
            {
                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                    "Delete the {0}",
                    "Deleting the last item will also delete the {0}.",
                    CustomMessageBoxIcon.Info,
                    CustomMessageBoxButtons.OkCancel,
                    transactionType);

                if (result != CustomMessageBoxResult.Ok)
                {
                    e.Cancel = true;
                    return;
                }

                Tools.CloseOpenForm<ItemsInTransaction_Form>();
                Guna2DataGridView grid = (Guna2DataGridView)sender;
                grid.Rows.Remove(SelectedRowInMainMenu);
            }
            else
            {
                // Remove the row from the tag
                itemList.RemoveAt(e.Row.Index);
            }

            string message = $"Deleted item '{productName}' in {transactionType}";
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 2, message);
        }
        private static List<Category>? GetCategoryList()
        {
            return MainMenu_Form.Instance.Selected switch
            {
                MainMenu_Form.SelectedOption.CategoryPurchases => MainMenu_Form.Instance.CategoryPurchaseList,
                MainMenu_Form.SelectedOption.CategorySales => MainMenu_Form.Instance.CategorySaleList,
                MainMenu_Form.SelectedOption.CategoryRentals => MainMenu_Form.Instance.CategoryRentalList,
                MainMenu_Form.SelectedOption.ProductPurchases => MainMenu_Form.Instance.CategoryPurchaseList,
                MainMenu_Form.SelectedOption.ProductSales => MainMenu_Form.Instance.CategorySaleList,
                MainMenu_Form.SelectedOption.ProductRentals => MainMenu_Form.Instance.CategoryRentalList,
                _ => null
            };
        }

        // Methods for DataGridView for event handlers
        private static void RestoreNoteCellUnderline(DataGridViewCell cell)
        {
            if (cell != null && cell.Value != null && cell.Value.ToString() == ReadOnlyVariables.Show_text)
            {
                AddUnderlineToCell(cell);
            }
        }
        private static bool IsNoteCell(DataGridViewCellMouseEventArgs e, DataGridView dataGridView)
        {
            if (e?.RowIndex < 0) { return false; }

            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

            return cell.Value?.ToString() == ReadOnlyVariables.Show_text && cell.Tag is string;
        }
        private static bool IsClickOnText(DataGridViewCell cell, out Rectangle hitbox)
        {
            hitbox = Rectangle.Empty;

            if (cell.Value != null && cell.Value.ToString() == ReadOnlyVariables.Show_text)
            {
                Size textSize = TextRenderer.MeasureText(cell.Value.ToString(), cell.DataGridView.Font);
                byte padding = 3;
                textSize.Width += 10 + padding + padding;
                textSize.Height += padding + padding;

                Rectangle cellRect = cell.DataGridView.GetCellDisplayRectangle(cell.ColumnIndex, cell.RowIndex, false);
                int relativeX = cellRect.X + cell.InheritedStyle.Padding.Left - padding;
                int relativeY = cellRect.Y + (cell.Size.Height - textSize.Height) / 2;
                hitbox = new Rectangle(relativeX, relativeY, textSize.Width, textSize.Height);

                return true;
            }
            return false;
        }
        private static void UpdateCellStyleBasedOnMousePosition(DataGridViewCell cell, Rectangle hitbox, Point mousePos)
        {
            if (hitbox.Contains(mousePos))
            {
                RemoveUnderlineFromCell(cell);
            }
            else
            {
                AddUnderlineToCell(cell);
            }
        }

        // Methods for DataGridView for MouseUp
        private static bool IsValidRightClick(Guna2DataGridView grid, MouseEventArgs e)
        {
            return e.Button == MouseButtons.Right && grid.Rows.Count > 0;
        }
        private static void SelectRowAndDeselectAllOthers(Guna2DataGridView grid, MouseEventArgs e)
        {
            DataGridView.HitTestInfo info = grid.HitTest(e.X, e.Y);
            if (info.RowIndex == -1)
            {
                return;
            }
            if (!grid.Rows[info.RowIndex].Selected)
            {
                UnselectAllRowsInDataGridView(grid);
            }

            // Select current row
            grid.Rows[info.RowIndex].Selected = true;
            grid.Focus();
        }
        private static void ConfigureRightClickDataGridViewMenuButtons(Guna2DataGridView grid)
        {
            FlowLayoutPanel flowPanel = RightClickDataGridViewRowMenu.Panel.Controls.OfType<FlowLayoutPanel>().FirstOrDefault();
            RightClickDataGridViewRowMenu.Panel.Tag = grid;  // This is used in the button event handlers

            // First, hide all buttons
            foreach (Control control in flowPanel.Controls)
            {
                control.Visible = false;
            }

            MainMenu_Form.SelectedOption selectedOption = MainMenu_Form.Instance.Selected;
            bool isSingleRowSelected = grid.SelectedRows.Count == 1;
            bool isPurchasesOrSales = selectedOption == MainMenu_Form.SelectedOption.Purchases ||
                                      selectedOption == MainMenu_Form.SelectedOption.Sales;
            bool isRentals = selectedOption == MainMenu_Form.SelectedOption.Rentals;
            bool isTransactionView = selectedOption == MainMenu_Form.SelectedOption.ItemsInPurchase ||
                                     selectedOption == MainMenu_Form.SelectedOption.ItemsInSale;
            int currentIndex = 0;

            bool isRentalInventory = grid.Tag?.ToString() == MainMenu_Form.DataGridViewTag.RentalInventory.ToString();

            // Add buttons for Rental Inventory
            if (isRentalInventory)
            {
                if (isSingleRowSelected)
                {
                    if (grid.SelectedRows[0].Tag is RentalItem rentalItem)
                    {
                        // Check if item has available quantity for renting out
                        if (rentalItem.QuantityAvailable > 0)
                        {
                            RightClickDataGridViewRowMenu.RentOut_Button.Visible = true;
                            flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.RentOut_Button, currentIndex++);
                        }

                        // Check if item has rented quantity for returning
                        if (rentalItem.QuantityRented > 0)
                        {
                            RightClickDataGridViewRowMenu.ReturnRental_Button.Visible = true;
                            flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.ReturnRental_Button, currentIndex++);
                        }
                    }

                    RightClickDataGridViewRowMenu.Modify_Button.Visible = true;
                    flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.Modify_Button, currentIndex++);
                }

                RightClickDataGridViewRowMenu.Delete_Button.Visible = true;
                flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.Delete_Button, currentIndex++);

                return;  // Don't add any more buttons
            }

            // Add buttons for Rental Transactions
            if (isRentals && isSingleRowSelected)
            {
                DataGridViewRow selectedRow = grid.SelectedRows[0];

                // Check if this is a rental transaction (has TagData) and not already returned
                if (selectedRow.Tag is TagData tagData && !tagData.IsReturned)
                {
                    RightClickDataGridViewRowMenu.ReturnRental_Button.Visible = true;
                    flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.ReturnRental_Button, currentIndex++);
                }
            }

            // Add buttons for Receipts_Form
            if (selectedOption == MainMenu_Form.SelectedOption.Receipts)
            {
                if (isSingleRowSelected)
                {
                    RightClickDataGridViewRowMenu.ViewReceipt_Button.Visible = true;
                    flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.ViewReceipt_Button, currentIndex++);

                    RightClickDataGridViewRowMenu.ExportReceipt_Button.Visible = true;
                    flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.ExportReceipt_Button, currentIndex++);
                }

                return;  // Don't add any more buttons
            }

            // Add Modify_Button
            if (isSingleRowSelected)
            {
                RightClickDataGridViewRowMenu.Modify_Button.Visible = true;
                flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.Modify_Button, currentIndex++);
            }

            // Add Move_Button and Move2_Button for categories
            if (selectedOption == MainMenu_Form.SelectedOption.CategoryPurchases)
            {
                RightClickDataGridViewRowMenu.Move_Button.Visible = true;
                RightClickDataGridViewRowMenu.Move_Button.Text = LanguageManager.TranslateString("Move category to sales");
                flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.Move_Button, currentIndex++);

                RightClickDataGridViewRowMenu.Move2_Button.Visible = true;
                RightClickDataGridViewRowMenu.Move2_Button.Text = LanguageManager.TranslateString("Move category to rentals");
                flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.Move2_Button, currentIndex++);
            }
            else if (selectedOption == MainMenu_Form.SelectedOption.CategorySales)
            {
                RightClickDataGridViewRowMenu.Move_Button.Visible = true;
                RightClickDataGridViewRowMenu.Move_Button.Text = LanguageManager.TranslateString("Move category to purchases");
                flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.Move_Button, currentIndex++);

                RightClickDataGridViewRowMenu.Move2_Button.Visible = true;
                RightClickDataGridViewRowMenu.Move2_Button.Text = LanguageManager.TranslateString("Move category to rentals");
                flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.Move2_Button, currentIndex++);
            }
            else if (selectedOption == MainMenu_Form.SelectedOption.CategoryRentals)
            {
                RightClickDataGridViewRowMenu.Move_Button.Visible = true;
                RightClickDataGridViewRowMenu.Move_Button.Text = LanguageManager.TranslateString("Move category to purchases");
                flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.Move_Button, currentIndex++);

                RightClickDataGridViewRowMenu.Move2_Button.Visible = true;
                RightClickDataGridViewRowMenu.Move2_Button.Text = LanguageManager.TranslateString("Move category to sales");
                flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.Move2_Button, currentIndex++);
            }

            if (isPurchasesOrSales)
            {
                // Add ShowItems_Button
                if (isSingleRowSelected && grid.SelectedRows[0].Tag is (List<string>, TagData))
                {
                    RightClickDataGridViewRowMenu.ShowItems_Button.Visible = true;
                    flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.ShowItems_Button, currentIndex++);
                }

                // Add ViewReceipt_Button and ExportReceipt_Button
                if (AnySelectedRowHasReceipt(grid))
                {
                    if (isSingleRowSelected)
                    {
                        RightClickDataGridViewRowMenu.ViewReceipt_Button.Visible = true;
                        flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.ViewReceipt_Button, currentIndex++);
                    }

                    RightClickDataGridViewRowMenu.ExportReceipt_Button.Visible = true;
                    flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.ExportReceipt_Button, currentIndex++);
                }
            }

            // Add view details and action buttons based on current status
            if (isSingleRowSelected && (isPurchasesOrSales || isTransactionView))
            {
                DataGridViewRow selectedRow;

                // Check if we're in the items view of a transaction
                if (selectedOption == MainMenu_Form.SelectedOption.ItemsInPurchase ||
                    selectedOption == MainMenu_Form.SelectedOption.ItemsInSale)
                {
                    // Use the main transaction row for status checks
                    selectedRow = SelectedRowInMainMenu;
                }
                else
                {
                    // Normal case - we're in the main purchases/sales view
                    selectedRow = grid.SelectedRows[0];
                }

                // Check return status
                bool isFullyReturned = ReturnManager.IsTransactionFullyReturned(selectedRow);
                bool isPartiallyReturned = ReturnManager.IsTransactionPartiallyReturned(selectedRow);
                bool hasAnyReturns = isFullyReturned || isPartiallyReturned;

                // Check loss status
                bool isFullyLost = LostManager.IsTransactionFullyLost(selectedRow);
                bool isPartiallyLost = LostManager.IsTransactionPartiallyLost(selectedRow);
                bool hasAnyLoss = isFullyLost || isPartiallyLost;

                // View Details buttons - show when there's information to view
                if (hasAnyReturns)
                {
                    RightClickDataGridViewRowMenu.ViewReturnDetails_Button.Visible = true;
                    flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.ViewReturnDetails_Button, currentIndex++);
                }

                if (hasAnyLoss)
                {
                    RightClickDataGridViewRowMenu.ViewLossDetails_Button.Visible = true;
                    flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.ViewLossDetails_Button, currentIndex++);
                }

                // Return action buttons
                if (isFullyReturned)
                {
                    // Fully returned - only show undo button
                    RightClickDataGridViewRowMenu.UndoReturn_Button.Visible = true;
                    RightClickDataGridViewRowMenu.UndoReturn_Button.Text = LanguageManager.TranslateString("Undo return");
                    flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.UndoReturn_Button, currentIndex++);
                }
                else if (isPartiallyReturned)
                {
                    // Partially returned - show both buttons
                    RightClickDataGridViewRowMenu.Return_Button.Visible = true;
                    RightClickDataGridViewRowMenu.Return_Button.Text = LanguageManager.TranslateString("Return more items");
                    flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.Return_Button, currentIndex++);

                    RightClickDataGridViewRowMenu.UndoReturn_Button.Visible = true;
                    RightClickDataGridViewRowMenu.UndoReturn_Button.Text = LanguageManager.TranslateString("Undo partial return");
                    flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.UndoReturn_Button, currentIndex++);
                }
                else if (!isFullyLost && !isPartiallyLost)
                {
                    // Not returned and not lost - show return button
                    RightClickDataGridViewRowMenu.Return_Button.Visible = true;
                    RightClickDataGridViewRowMenu.Return_Button.Text = LanguageManager.TranslateString("Return product");
                    flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.Return_Button, currentIndex++);
                }

                // Loss action buttons
                if (isFullyLost)
                {
                    // Fully lost - only show undo loss button
                    RightClickDataGridViewRowMenu.UndoLoss_Button.Visible = true;
                    RightClickDataGridViewRowMenu.UndoLoss_Button.Text = LanguageManager.TranslateString("Undo loss");
                    flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.UndoLoss_Button, currentIndex++);
                }
                else if (isPartiallyLost)
                {
                    // Partially lost - show both buttons
                    RightClickDataGridViewRowMenu.MarkAsLost_Button.Visible = true;
                    RightClickDataGridViewRowMenu.MarkAsLost_Button.Text = LanguageManager.TranslateString("Mark more items as lost");
                    flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.MarkAsLost_Button, currentIndex++);

                    RightClickDataGridViewRowMenu.UndoLoss_Button.Visible = true;
                    RightClickDataGridViewRowMenu.UndoLoss_Button.Text = LanguageManager.TranslateString("Undo partial loss");
                    flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.UndoLoss_Button, currentIndex++);
                }
                else if (!isFullyReturned && !isPartiallyReturned)
                {
                    // Not lost and not returned - show mark as lost button
                    RightClickDataGridViewRowMenu.MarkAsLost_Button.Visible = true;
                    RightClickDataGridViewRowMenu.MarkAsLost_Button.Text = LanguageManager.TranslateString("Mark as lost");
                    flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.MarkAsLost_Button, currentIndex++);
                }
            }

            // Add DeleteBtn
            RightClickDataGridViewRowMenu.Delete_Button.Visible = true;
            flowPanel.Controls.SetChildIndex(RightClickDataGridViewRowMenu.Delete_Button, currentIndex);
        }
        private static bool AnySelectedRowHasReceipt(DataGridView grid)
        {
            foreach (DataGridViewRow row in grid.SelectedRows)
            {
                if (row.Tag is (List<string> tagList, TagData) && IsLastItemAReceipt(tagList[^1]))
                {
                    return true;
                }
                else if (row.Tag is (string item, TagData) && IsLastItemAReceipt(item))
                {
                    return true;
                }
            }
            return false;
        }
        private static void PositionRightClickDataGridViewMenu(Guna2DataGridView grid, MouseEventArgs e, DataGridView.HitTestInfo info)
        {
            FlowLayoutPanel flowPanel = RightClickDataGridViewRowMenu.Panel.Controls.OfType<FlowLayoutPanel>().FirstOrDefault();
            bool hasVisibleButtons = flowPanel.Controls.OfType<Guna2Button>().Any(btn => btn.Visible);

            if (!hasVisibleButtons)
            {
                return;  // Don't show the panel if no visible buttons
            }

            Form parentForm = grid.FindForm();
            int formHeight = parentForm.ClientSize.Height;
            int formWidth = parentForm.ClientSize.Width;

            CustomControls.SetRightClickMenuHeight(RightClickDataGridViewRowMenu.Panel);

            SetHorizontalPosition(grid, RightClickDataGridViewRowMenu.Panel, e, formWidth);
            SetVerticalPosition(grid, RightClickDataGridViewRowMenu.Panel, info, formHeight);

            grid.Parent.Controls.Add(RightClickDataGridViewRowMenu.Panel);
            RightClickDataGridViewRowMenu.Panel.BringToFront();
        }
        public static void SetHorizontalPosition(Guna2DataGridView grid, Control control, MouseEventArgs e, int formWidth)
        {
            if (grid.Left + control.Width + e.X - ReadOnlyVariables.OffsetRightClickPanel + ReadOnlyVariables.PaddingRightClickPanel > formWidth)
            {
                control.Left = formWidth - control.Width - ReadOnlyVariables.PaddingRightClickPanel;
            }
            else
            {
                control.Left = grid.Left + e.X - ReadOnlyVariables.OffsetRightClickPanel;
            }
        }
        public static void SetVerticalPosition(Guna2DataGridView grid, Control control, DataGridView.HitTestInfo info, int formHeight)
        {
            int targetTop;

            // Handle column header clicks differently from row clicks
            if (info.RowIndex == -1)  // Column header click
            {
                // Position the panel right below the column header
                targetTop = grid.Top + grid.ColumnHeadersHeight;
            }
            else  // Regular row click
            {
                int rowHeight = grid.Rows[0].Height;
                int headerHeight = grid.ColumnHeadersHeight;

                // Calculate scroll offset considering only visible rows
                int scrollOffset = 0;
                for (int i = 0; i < grid.FirstDisplayedScrollingRowIndex; i++)
                {
                    if (grid.Rows[i].Visible)
                    {
                        scrollOffset += rowHeight;
                    }
                }

                // Calculate position up to target row, counting only visible rows
                int visibleRowsBeforeTarget = 0;
                for (int i = 0; i < info.RowIndex; i++)
                {
                    if (grid.Rows[i].Visible)
                    {
                        visibleRowsBeforeTarget++;
                    }
                }

                targetTop = headerHeight + ((visibleRowsBeforeTarget + 1) * rowHeight) - scrollOffset + grid.Top;
            }

            // Ensure the panel fits within the form bounds
            if (targetTop + control.Height > formHeight - ReadOnlyVariables.PaddingRightClickPanel)
            {
                control.Top = formHeight - control.Height - ReadOnlyVariables.PaddingRightClickPanel;
            }
            else
            {
                control.Top = targetTop;
            }
        }

        // Methods for DataGridView
        private static bool IsLastItemAReceipt(string lastItem)
        {
            if (lastItem.StartsWith(ReadOnlyVariables.Receipt_text))
            {
                lastItem = ReceiptManager.ProcessReceiptTextFromRowTag(lastItem);
                return File.Exists(lastItem);
            }
            return false;
        }
        public static void DataGridViewRowsAdded(Guna2DataGridView grid, DataGridViewRowsAddedEventArgs e)
        {
            if (MainMenu_Form.IsProgramLoading) { return; }

            DataGridViewRowChanged(grid);
            DataGridViewRow row;

            if (e.RowIndex >= 0 && e.RowIndex < grid.Rows.Count)
            {
                row = grid.Rows[e.RowIndex];
            }
            else
            {
                Log.Error_RowIsOutOfRange();
                return;
            }

            SortDataGridViewByCurrentDirection(grid);

            if (MainMenu_Form.Instance.Selected is MainMenu_Form.SelectedOption.Purchases or MainMenu_Form.SelectedOption.Sales)
            {
                MainMenu_Form.Instance.RefreshDataGridViewAndCharts();
            }

            // Calculate the middle index
            int visibleRowCount = grid.DisplayedRowCount(true);
            int middleIndex = Math.Max(0, row.Index - (visibleRowCount / 2) + 1);

            // Ensure the row at middleIndex is visible
            if (middleIndex >= 0 && middleIndex < grid.RowCount && grid.Rows[middleIndex].Visible)
            {
                grid.FirstDisplayedScrollingRowIndex = middleIndex;
            }

            // Select the added row
            UnselectAllRowsInDataGridView(grid);
            grid.Rows[row.Index].Selected = true;
        }
        private static void LoadColumns<TEnum>(Guna2DataGridView dataGridView, Dictionary<TEnum, string> columnHeaders, List<TEnum>? columnsToLoad = null) where TEnum : Enum
        {
            foreach (KeyValuePair<TEnum, string> columnHeader in columnHeaders)
            {
                if (columnsToLoad != null && !columnsToLoad.Contains(columnHeader.Key))
                {
                    continue;
                }

                DataGridViewTextBoxColumn column = new()
                {
                    HeaderText = columnHeader.Value,
                    Name = columnHeader.Key.ToString()
                };
                dataGridView.Columns.Add(column);
            }
        }
        public static string GetFilePathForDataGridView(MainMenu_Form.SelectedOption selected)
        {
            return selected switch
            {
                MainMenu_Form.SelectedOption.Purchases => Directories.Purchases_file,
                MainMenu_Form.SelectedOption.Sales => Directories.Sales_file,
                MainMenu_Form.SelectedOption.Rentals => Directories.Rentals_file,
                MainMenu_Form.SelectedOption.Customers => Directories.Customers_file,
                MainMenu_Form.SelectedOption.CategoryPurchases => Directories.CategoryPurchases_file,
                MainMenu_Form.SelectedOption.CategorySales => Directories.CategorySales_file,
                MainMenu_Form.SelectedOption.CategoryRentals => Directories.CategoryRentals_file,
                MainMenu_Form.SelectedOption.ProductPurchases => Directories.CategoryPurchases_file,
                MainMenu_Form.SelectedOption.ProductSales => Directories.CategorySales_file,
                MainMenu_Form.SelectedOption.ProductRentals => Directories.CategoryRentals_file,
                MainMenu_Form.SelectedOption.Accountants => Directories.Accountants_file,
                MainMenu_Form.SelectedOption.Companies => Directories.Companies_file,
                _ => ""
            };
        }
        private static void UnselectAllRowsInDataGridView(Guna2DataGridView dataGridView)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                row.Selected = false;
            }
        }
        public static bool DoesValueExistInDataGridView(Guna2DataGridView dataGridView, string column, string purchaseID)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (purchaseID == row.Cells[column].Value.ToString())
                {
                    return true;
                }
            }
            return false;
        }
        public static void SortFirstColumnAndSelectFirstRow(params Guna2DataGridView[] dataGridViews)
        {
            foreach (DataGridView dataGrid in dataGridViews)
            {
                if (dataGrid.Columns.Count > 0)
                {
                    dataGrid.Sort(dataGrid.Columns[0], ListSortDirection.Ascending);

                    // Select the first row
                    if (dataGrid.Rows.Count > 0)
                    {
                        dataGrid.ClearSelection();
                        dataGrid.Rows[0].Selected = true;
                    }
                }

                UpdateRowColors(dataGrid);
            }
        }
        public static void UpdateChargedDifferenceInRowWithMultipleItems(DataGridViewRow selectedRow)
        {
            List<string> items = selectedRow.Tag is (List<string> itemList, TagData) ? itemList : [];

            if (items.Count <= 1) { return; }

            string firstCategoryName = null, firstCountry = null, firstCompany = null;
            bool isCategoryNameConsistent = true, isCountryConsistent = true, isCompanyConsistent = true;
            decimal itemsTotal = 0;
            int totalQuantity = 0;

            bool shouldSkipLastItem = items[^1].StartsWith(ReadOnlyVariables.Receipt_text);

            // Calculate total for all items
            for (int i = 0; i < (shouldSkipLastItem ? items.Count - 1 : items.Count); i++)
            {
                string[] itemDetails = items[i].Split(',');

                string currentCategoryName = itemDetails[1];
                string currentCountry = itemDetails[2];
                string currentCompany = itemDetails[3];
                int quantity = Convert.ToInt32(itemDetails[4]);
                decimal pricePerUnit = decimal.Parse(itemDetails[5]);
                itemsTotal += quantity * pricePerUnit;
                totalQuantity += quantity;

                // Check consistency for category, country, company
                if (firstCategoryName == null) { firstCategoryName = currentCategoryName; }
                else if (isCategoryNameConsistent && firstCategoryName != currentCategoryName) { isCategoryNameConsistent = false; }

                if (firstCountry == null) { firstCountry = currentCountry; }
                else if (isCountryConsistent && firstCountry != currentCountry) { isCountryConsistent = false; }

                if (firstCompany == null) { firstCompany = currentCompany; }
                else if (isCompanyConsistent && firstCompany != currentCompany) { isCompanyConsistent = false; }
            }

            // Update category, country, company fields
            string categoryName = isCategoryNameConsistent ? firstCategoryName : ReadOnlyVariables.EmptyCell;
            string country = isCountryConsistent ? firstCountry : ReadOnlyVariables.EmptyCell;
            string company = isCompanyConsistent ? firstCompany : ReadOnlyVariables.EmptyCell;

            selectedRow.Cells[ReadOnlyVariables.Category_column].Value = categoryName;
            selectedRow.Cells[ReadOnlyVariables.Country_column].Value = country;
            selectedRow.Cells[ReadOnlyVariables.Company_column].Value = company;
            selectedRow.Cells[ReadOnlyVariables.TotalItems_column].Value = totalQuantity;

            // Update charged difference
            decimal shipping = decimal.Parse(selectedRow.Cells[ReadOnlyVariables.Shipping_column].Value.ToString());
            decimal tax = decimal.Parse(selectedRow.Cells[ReadOnlyVariables.Tax_column].Value.ToString());
            decimal fee = decimal.Parse(selectedRow.Cells[ReadOnlyVariables.Fee_column].Value.ToString());
            decimal discount = decimal.Parse(selectedRow.Cells[ReadOnlyVariables.Discount_column].Value.ToString());

            bool isSale = selectedRow.DataGridView == MainMenu_Form.Instance.Sale_DataGridView;
            decimal expectedTotal;

            if (isSale)
            {
                expectedTotal = itemsTotal - shipping - discount;
            }
            else
            {
                expectedTotal = itemsTotal + shipping + tax + fee - discount;
            }

            // Calculate charged difference: Actual Total - Expected Total
            decimal actualTotal = Convert.ToDecimal(selectedRow.Cells[ReadOnlyVariables.Total_column].Value);
            decimal chargedDifference = actualTotal - expectedTotal;

            selectedRow.Cells[ReadOnlyVariables.ChargedDifference_column].Value = chargedDifference.ToString("N2");
        }
        public static void UpdateChargedDifferenceInRowWithNoItems(DataGridViewRow selectedRow)
        {
            int quantity = int.Parse(selectedRow.Cells[ReadOnlyVariables.TotalItems_column].Value.ToString());
            decimal pricePerUnit = decimal.Parse(selectedRow.Cells[ReadOnlyVariables.PricePerUnit_column].Value.ToString());
            decimal shipping = decimal.Parse(selectedRow.Cells[ReadOnlyVariables.Shipping_column].Value.ToString());
            decimal tax = decimal.Parse(selectedRow.Cells[ReadOnlyVariables.Tax_column].Value.ToString());
            decimal fee = decimal.Parse(selectedRow.Cells[ReadOnlyVariables.Fee_column].Value.ToString());
            decimal discount = decimal.Parse(selectedRow.Cells[ReadOnlyVariables.Discount_column].Value.ToString());

            bool isSale = selectedRow.DataGridView == MainMenu_Form.Instance.Sale_DataGridView;
            decimal expectedTotal;

            if (isSale)
            {
                expectedTotal = quantity * pricePerUnit - discount;
            }
            else
            {
                expectedTotal = quantity * pricePerUnit + shipping + tax + fee - discount;
            }

            // Calculate charged difference: Actual Total - Expected Total
            decimal actualTotal = Convert.ToDecimal(selectedRow.Cells[ReadOnlyVariables.Total_column].Value);
            decimal chargedDifference = actualTotal - expectedTotal;

            selectedRow.Cells[ReadOnlyVariables.ChargedDifference_column].Value = chargedDifference.ToString("N2");
        }
        public static void AddNoteToCell(Guna2DataGridView grid, int newRowIndex, string note)
        {
            DataGridViewCell cell = grid.Rows[newRowIndex].Cells[^1];
            cell.Tag = note;
            AddUnderlineToCell(cell);
        }
        public static void AddUnderlineToCell(DataGridViewCell cell)
        {
            cell.Style.Font = new Font(cell.DataGridView.DefaultCellStyle.Font, FontStyle.Underline);
        }
        public static void RemoveUnderlineFromCell(DataGridViewCell cell)
        {
            cell.Style.Font = new Font(cell.DataGridView.DefaultCellStyle.Font, FontStyle.Regular);
        }
        public static bool HasVisibleRowsExcludingReturnedOrLost(params Guna2DataGridView[] dataGridViews)
        {
            foreach (DataGridView dataGridView in dataGridViews)
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.Visible && !LostManager.IsTransactionFullyLost(row) && !ReturnManager.IsTransactionFullyReturned(row))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool HasVisibleRowsForReturn(params Guna2DataGridView[] dataGridViews)
        {
            foreach (DataGridView dataGridView in dataGridViews)
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.Visible && ReturnManager.IsTransactionReturned(row))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool HasVisibleRowsForLoss(params Guna2DataGridView[] dataGridViews)
        {
            foreach (DataGridView dataGridView in dataGridViews)
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.Visible && LostManager.IsTransactionLost(row))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Updates row appearance for returns and losses in ItemsInTransaction forms.
        /// Individual returned items will be displayed in red, lost items in dark red.
        /// If an item is both returned and lost, loss styling takes priority.
        /// </summary>
        public static void UpdateItemRowAppearance(Guna2DataGridView itemsGrid, DataGridViewRow mainTransactionRow)
        {
            if (mainTransactionRow.Tag is not (List<string>, TagData))
            {
                return;
            }

            // Apply styling to individual item rows based on their status
            for (int i = 0; i < itemsGrid.Rows.Count; i++)
            {
                DataGridViewRow itemRow = itemsGrid.Rows[i];
                bool isItemReturned = ReturnManager.IsItemReturned(mainTransactionRow, i);
                bool isItemLost = LostManager.IsItemLost(mainTransactionRow, i);

                if (isItemLost)
                {
                    // Lost items take priority - mark in dark red
                    itemRow.DefaultCellStyle.BackColor = CustomColors.LostItemBackground;
                    itemRow.DefaultCellStyle.SelectionBackColor = CustomColors.LostItemSelection;
                    itemRow.DefaultCellStyle.ForeColor = CustomColors.LostItemText;
                }
                else if (isItemReturned)
                {
                    // Mark returned items in red
                    itemRow.DefaultCellStyle.BackColor = CustomColors.ReturnedItemBackground;
                    itemRow.DefaultCellStyle.SelectionBackColor = CustomColors.ReturnedItemSelection;
                    itemRow.DefaultCellStyle.ForeColor = CustomColors.ReturnedItemText;
                }
                else
                {
                    // Reset to default colors for normal items
                    itemRow.DefaultCellStyle.BackColor = Color.Empty;
                    itemRow.DefaultCellStyle.SelectionBackColor = Color.Empty;
                    itemRow.DefaultCellStyle.ForeColor = Color.Empty;
                }
            }
        }
        public static void SortDataGridViewByCurrentDirection(DataGridView dataGridView)
        {
            if (dataGridView.SortedColumn == null)
            {
                return;
            }

            SortOrder sortOrder = dataGridView.SortOrder;
            DataGridViewColumn sortedColumn = dataGridView.SortedColumn;
            ListSortDirection direction = (sortOrder == SortOrder.Ascending)
                ? ListSortDirection.Ascending
                : ListSortDirection.Descending;
            dataGridView.Sort(sortedColumn, direction);
        }
        public static void UpdateRowColors(DataGridView dataGridView)
        {
            int visibleRowIndex = 0;

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (!row.Visible) { continue; }

                // Check return and loss status for proper color application
                bool isFullyReturned = ReturnManager.IsTransactionFullyReturned(row);
                bool isPartiallyReturned = ReturnManager.IsTransactionPartiallyReturned(row);
                bool isFullyLost = LostManager.IsTransactionFullyLost(row);
                bool isPartiallyLost = LostManager.IsTransactionPartiallyLost(row);

                // Priority order: Lost > Returned > Normal
                if (isFullyLost)
                {
                    // Fully lost - dark red/maroon background
                    LostManager.UpdateRowAppearanceForLoss(row, true, false);
                }
                else if (isPartiallyLost)
                {
                    // Partially lost - dark orange/brown background
                    LostManager.UpdateRowAppearanceForLoss(row, false, true);
                }
                else if (isFullyReturned)
                {
                    // Fully returned - red background
                    ReturnManager.UpdateRowAppearanceForReturn(row, true, false);
                }
                else if (isPartiallyReturned)
                {
                    // Partially returned - orange background
                    ReturnManager.UpdateRowAppearanceForReturn(row, false, true);
                }
                else
                {
                    // Not returned or lost - apply alternating colors
                    row.DefaultCellStyle.BackColor = (visibleRowIndex % 2 == 0)
                        ? dataGridView.DefaultCellStyle.BackColor
                        : dataGridView.AlternatingRowsDefaultCellStyle.BackColor;

                    // Reset other styling
                    row.DefaultCellStyle.SelectionBackColor = Color.Empty;
                    row.DefaultCellStyle.ForeColor = Color.Empty;
                }

                visibleRowIndex++;
            }
        }
        public static void ScrollToTopOfDataGridView(Guna2DataGridView dataGridView)
        {
            if (dataGridView.Rows.Count > 0)
            {
                dataGridView.FirstDisplayedScrollingRowIndex = 0;
            }
        }

        /// <summary>
        /// Searches a DataGridView for the text in the search_TextBox.
        /// </summary>
        /// <returns>True if the searching label should be shown, or false if it should not be shown.</returns>
        public static bool SearchSelectedDataGridViewAndUpdateRowColors(Guna2DataGridView grid, Guna2TextBox search_TextBox)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                bool isVisible = row.Cells.Cast<DataGridViewCell>()
                                          .Any(cell => cell.Value != null && cell.Value.ToString()
                                          .Contains(search_TextBox.Text.Trim(), StringComparison.OrdinalIgnoreCase));
                row.Visible = isVisible;
            }

            UpdateRowColors(grid);

            return !string.IsNullOrEmpty(search_TextBox.Text.Trim());
        }

        // Other methods
        /// <summary>
        /// Shows a MessageBox if the row is being used by another row.
        /// </summary>
        /// <returns>True if it's being used by another row; Otherwise, false.</returns>
        private static bool IsThisBeingUsedByDataGridView(string type, string columnName, string value, string action)
        {
            foreach (DataGridViewRow row in MainMenu_Form.Instance.GetAllRows())
            {
                if (row.Cells[columnName].Value.ToString() == value)
                {
                    ShowInUseMessage(type, action);
                    return true;
                }

                // Skip if we are not checking a product name
                if (columnName != ReadOnlyVariables.Product_column) { continue; }

                // Skip if transaction does not have multiple items
                if (row.Tag is not (List<string> items, TagData)) { continue; }

                // Do not check receipt if present
                int itemsToCheck = items[^1].StartsWith(ReadOnlyVariables.Receipt_text)
                    ? items.Count - 1
                    : items.Count;

                // Check each product name until match found
                for (int i = 0; i < itemsToCheck; i++)
                {
                    if (items[i].Split(',')[0] == value)
                    {
                        ShowInUseMessage(type, action);
                        return true;
                    }
                }
            }

            HandleValueDeletion(type, value);
            return false;
        }
        public static void ShowInUseMessage(string type, string action)
        {
            CustomMessageBox.ShowWithFormat(
                "Cannot be {0}",
                "This {1} is being used by a transaction and cannot be {0}.",
                CustomMessageBoxIcon.Info,
                CustomMessageBoxButtons.Ok,
                action, type);
        }

        /// <summary>
        /// Determines whether a category can be moved or deleted by checking if it contains any products.
        /// Shows a message box if the action cannot be performed.
        /// </summary>
        private static bool CanCategoryBeMovedOrDeleted(string categoryName, List<Category> categoryList, string action)
        {
            if (MainMenu_Form.DoesCategoryHaveProducts(categoryName, categoryList))
            {
                CustomMessageBox.ShowWithFormat(
                    "Cannot {0} category",
                    "Cannot {1} category '{2}' because it contains products",
                    CustomMessageBoxIcon.Error,
                    CustomMessageBoxButtons.Ok,
                    action, action, categoryName);
                return false;
            }
            return true;
        }
        public static DataGridViewColumn? FindColumnByName(DataGridView dataGridView, string fieldName)
        {
            // Try to find column by name
            foreach (DataGridViewColumn column in dataGridView?.Columns)
            {
                if (column.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    return column;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds a DataGridView column by its HeaderText (display name) property.
        /// This is useful when searching for columns by what users see in the UI.
        /// </summary>
        /// <returns>The matching DataGridViewColumn or null if not found</returns>
        public static DataGridViewColumn? FindColumnByDisplayName(DataGridView dataGridView, string displayName)
        {
            if (dataGridView?.Columns == null || string.IsNullOrWhiteSpace(displayName))
            {
                return null;
            }

            // Try to find column by HeaderText (case insensitive)
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                if (column.HeaderText.Equals(displayName, StringComparison.OrdinalIgnoreCase))
                {
                    return column;
                }
            }

            // Handle common aliases and variations that might be used in AI queries
            Dictionary<string, string[]> displayNameAliases = new(StringComparer.OrdinalIgnoreCase)
            {
                { "country", ["Country of origin", "Country of destination"] },
                { "company", ["Company of origin"] },
                { "price", ["Price per unit", "Total"] },
                { "cost", ["Total", "Price per unit"] },
                { "discount", ["Discount"] },
                { "shipping", ["Shipping"] },
                { "date", ["Date"] },
                { "category", ["Category"] },
                { "product", ["Product"] },
                { "quantity", ["Total items"] },
                { "items", ["Total items"] },
                { "tax", ["Tax"] },
                { "fee", ["Fee"] },
                { "accountant", ["Accountant"] },
                { "note", ["Note"] },
                { "receipt", ["Has receipt"] }
            };

            // Check if the search term matches any aliases
            if (displayNameAliases.TryGetValue(displayName, out string[]? aliases))
            {
                foreach (string alias in aliases)
                {
                    foreach (DataGridViewColumn column in dataGridView.Columns)
                    {
                        if (column.HeaderText.Equals(alias, StringComparison.OrdinalIgnoreCase))
                        {
                            return column;
                        }
                    }
                }
            }

            return null;
        }

        // Validate TextBoxes in other forms
        private static void HandleValueDeletion(string type, string value)
        {
            if (Tools.IsFormOpen<AddPurchase_Form>() &&
                Application.OpenForms[nameof(AddPurchase_Form)] is AddPurchase_Form purchaseForm)
            {
                switch (type)
                {
                    case "product":
                        ValidateFormTextBox(purchaseForm.ProductName_TextBox, value);
                        break;
                    case "company":
                        ValidateProductPathCompany(purchaseForm.ProductName_TextBox, value);
                        break;
                    case "category":
                        ValidateProductPathCategory(purchaseForm.ProductName_TextBox, value);
                        break;
                }
            }

            if (Tools.IsFormOpen<AddSale_Form>() &&
                Application.OpenForms[nameof(AddSale_Form)] is AddSale_Form saleForm)
            {
                switch (type)
                {
                    case "product":
                        ValidateFormTextBox(saleForm.ProductName_TextBox, value);
                        break;
                    case "company":
                        ValidateProductPathCompany(saleForm.ProductName_TextBox, value);
                        break;
                    case "category":
                        ValidateProductPathCategory(saleForm.ProductName_TextBox, value);
                        break;
                }
            }
        }
        private static void ValidateFormTextBox(Guna2TextBox textBox, string value)
        {
            if (textBox.Text.Contains(value))
            {
                textBox.Text = "";
            }
        }
        private static void ValidateProductPathCompany(Guna2TextBox textBox, string company)
        {
            if (textBox.Text.StartsWith($"{company} >"))
            {
                textBox.Text = textBox.Text;
            }
        }
        private static void ValidateProductPathCategory(Guna2TextBox textBox, string category)
        {
            if (textBox.Text.Contains($"> {category} >"))
            {
                textBox.Text = textBox.Text;
            }
        }
    }
}