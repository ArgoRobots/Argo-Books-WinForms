using Guna.UI2.WinForms;
using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.LostProduct;
using Sales_Tracker.ReturnProduct;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker.GridView
{
    internal class RightClickDataGridViewRowMenu
    {
        // Enum for category move direction
        private enum CategoryMoveDirection
        {
            PurchaseToSale,
            PurchaseToRental,
            SaleToPurchase,
            SaleToRental,
            RentalToPurchase,
            RentalToSale
        }

        // Getters
        public static Guna2Panel Panel { get; private set; }
        public static Guna2Button Modify_Button { get; private set; }
        public static Guna2Button Move_Button { get; private set; }
        public static Guna2Button MoveTo_Button { get; private set; }
        public static Guna2Button ViewReceipt_Button { get; private set; }
        public static Guna2Button ExportReceipt_Button { get; private set; }
        public static Guna2Button ShowItems_Button { get; private set; }
        public static Guna2Button ViewReturnDetails_Button { get; private set; }
        public static Guna2Button ViewLossDetails_Button { get; private set; }
        public static Guna2Button Return_Button { get; private set; }
        public static Guna2Button UndoReturn_Button { get; private set; }
        public static Guna2Button MarkAsLost_Button { get; private set; }
        public static Guna2Button UndoLoss_Button { get; private set; }
        public static Guna2Button Delete_Button { get; private set; }
        public static Guna2Button RentOut_Button { get; private set; }

        // Methods
        public static void Construct()
        {
            Panel = CustomControls.ConstructPanelForMenu(
                new Size(CustomControls.PanelWidth, 10 * CustomControls.PanelButtonHeight + CustomControls.SpaceForPanel),
                "rightClickDataGridView_Panel"
            );

            FlowLayoutPanel flowPanel = (FlowLayoutPanel)Panel.Controls[0];

            Modify_Button = CustomControls.ConstructBtnForMenu("Modify", CustomControls.PanelBtnWidth, flowPanel);
            Modify_Button.Click += ModifyRow;

            Move_Button = CustomControls.ConstructBtnForMenu("Move", CustomControls.PanelBtnWidth, flowPanel);
            Move_Button.Click += MoveRows;

            MoveTo_Button = CustomControls.ConstructBtnForMenu("Move to...", CustomControls.PanelBtnWidth, flowPanel);
            MoveTo_Button.Click += MoveRowsTo;

            ViewReceipt_Button = CustomControls.ConstructBtnForMenu("View receipt", CustomControls.PanelBtnWidth, flowPanel);
            ViewReceipt_Button.Click += ViewReceipt;

            ExportReceipt_Button = CustomControls.ConstructBtnForMenu("Export receipt", CustomControls.PanelBtnWidth, flowPanel);
            ExportReceipt_Button.Click += ExportReceipt;

            ShowItems_Button = CustomControls.ConstructBtnForMenu("Show items", CustomControls.PanelBtnWidth, flowPanel);
            ShowItems_Button.Click += ShowItems;

            ViewReturnDetails_Button = CustomControls.ConstructBtnForMenu("View return details", CustomControls.PanelBtnWidth, flowPanel);
            ViewReturnDetails_Button.Click += ViewReturnDetails;

            ViewLossDetails_Button = CustomControls.ConstructBtnForMenu("View loss details", CustomControls.PanelBtnWidth, flowPanel);
            ViewLossDetails_Button.Click += ViewLossDetails;

            Return_Button = CustomControls.ConstructBtnForMenu("Mark as  returned", CustomControls.PanelBtnWidth, flowPanel);
            Return_Button.Click += ReturnProduct;

            UndoReturn_Button = CustomControls.ConstructBtnForMenu("Undo return", CustomControls.PanelBtnWidth, flowPanel);
            UndoReturn_Button.Click += UndoReturnProduct;

            MarkAsLost_Button = CustomControls.ConstructBtnForMenu("Mark as lost", CustomControls.PanelBtnWidth, flowPanel);
            MarkAsLost_Button.Click += MarkAsLost;

            UndoLoss_Button = CustomControls.ConstructBtnForMenu("Undo loss", CustomControls.PanelBtnWidth, flowPanel);
            UndoLoss_Button.Click += UndoLoss;

            Delete_Button = CustomControls.ConstructBtnForMenu("Delete", CustomControls.PanelBtnWidth, flowPanel);
            Delete_Button.ForeColor = CustomColors.AccentRed;
            Delete_Button.Click += DeleteRow;

            RentOut_Button = CustomControls.ConstructBtnForMenu("Rent out", CustomControls.PanelBtnWidth, flowPanel);
            RentOut_Button.Click += RentOut;
        }
        private static void ModifyRow(object sender, EventArgs e)
        {
            Guna2DataGridView grid = (Guna2DataGridView)Panel.Tag;
            Tools.OpenForm(new ModifyRow_Form(grid.SelectedRows[0]));
        }
        private static void MoveRows(object sender, EventArgs e)
        {
            Guna2DataGridView grid = (Guna2DataGridView)Panel.Tag;
            List<DataGridViewRow> selectedRows = grid.SelectedRows.Cast<DataGridViewRow>().ToList();
            if (selectedRows.Count == 0) { return; }

            // Save scroll position
            int scrollPosition = grid.FirstDisplayedScrollingRowIndex;
            int firstSelectedIndex = selectedRows[0].Index;

            // Move rows based on current selection (default direction)
            if (MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.CategoryPurchases)
            {
                MoveCategoryRows(selectedRows, CategoryMoveDirection.PurchaseToSale);
            }
            else if (MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.CategorySales)
            {
                MoveCategoryRows(selectedRows, CategoryMoveDirection.SaleToPurchase);
            }
            else if (MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.CategoryRentals)
            {
                MoveCategoryRows(selectedRows, CategoryMoveDirection.RentalToPurchase);
            }

            RestoreSelectionAndScroll(grid, firstSelectedIndex, scrollPosition);
        }
        private static void MoveRowsTo(object sender, EventArgs e)
        {
            Guna2DataGridView grid = (Guna2DataGridView)Panel.Tag;
            List<DataGridViewRow> selectedRows = grid.SelectedRows.Cast<DataGridViewRow>().ToList();
            if (selectedRows.Count == 0) { return; }

            // Save scroll position
            int scrollPosition = grid.FirstDisplayedScrollingRowIndex;
            int firstSelectedIndex = selectedRows[0].Index;

            // Ask user where to move categories based on current selection
            CategoryMoveDirection? direction = null;

            if (MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.CategoryPurchases)
            {
                CustomMessageBoxResult result = CustomMessageBox.Show(
                    "Move to",
                    "Move selected categories to Sales or Rentals?",
                    CustomMessageBoxIcon.Question,
                    CustomMessageBoxButtons.YesNoCancel);

                if (result == CustomMessageBoxResult.Yes)
                {
                    direction = CategoryMoveDirection.PurchaseToSale;
                }
                else if (result == CustomMessageBoxResult.No)
                {
                    direction = CategoryMoveDirection.PurchaseToRental;
                }
            }
            else if (MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.CategorySales)
            {
                CustomMessageBoxResult result = CustomMessageBox.Show(
                    "Move to",
                    "Move selected categories to Purchases or Rentals?",
                    CustomMessageBoxIcon.Question,
                    CustomMessageBoxButtons.YesNoCancel);

                if (result == CustomMessageBoxResult.Yes)
                {
                    direction = CategoryMoveDirection.SaleToPurchase;
                }
                else if (result == CustomMessageBoxResult.No)
                {
                    direction = CategoryMoveDirection.SaleToRental;
                }
            }
            else if (MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.CategoryRentals)
            {
                CustomMessageBoxResult result = CustomMessageBox.Show(
                    "Move to",
                    "Move selected categories to Purchases or Sales?",
                    CustomMessageBoxIcon.Question,
                    CustomMessageBoxButtons.YesNoCancel);

                if (result == CustomMessageBoxResult.Yes)
                {
                    direction = CategoryMoveDirection.RentalToPurchase;
                }
                else if (result == CustomMessageBoxResult.No)
                {
                    direction = CategoryMoveDirection.RentalToSale;
                }
            }

            if (direction.HasValue)
            {
                MoveCategoryRows(selectedRows, direction.Value);
            }

            RestoreSelectionAndScroll(grid, firstSelectedIndex, scrollPosition);
        }
        private static void ViewReceipt(object sender, EventArgs e)
        {
            Guna2DataGridView grid = (Guna2DataGridView)Panel.Tag;
            if (grid.SelectedRows.Count != 1) { return; }

            DataGridViewRow selectedRow = grid.SelectedRows[0];
            string receiptPath = ReceiptManager.GetReceiptPathFromRow(selectedRow);

            if (string.IsNullOrEmpty(receiptPath))
            {
                CustomMessageBox.Show("No Receipt", "This transaction does not have a receipt attached.",
                    CustomMessageBoxIcon.Info, CustomMessageBoxButtons.Ok);
                return;
            }

            if (!File.Exists(receiptPath))
            {
                CustomMessageBox.Show("Receipt Not Found", "The receipt file could not be found. It may have been moved or deleted.",
                    CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok);
                return;
            }

            if (!ReceiptViewer_Form.IsFormatSupported(receiptPath))
            {
                string supportedFormats = ReceiptViewer_Form.GetSupportedFormatsDescription();
                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat("Unsupported Format",
                     "The receipt format is not supported for viewing.\n\nSupported formats:\n{0}\n\nWould you like to open it with the default system application instead?",
                     CustomMessageBoxIcon.Question, CustomMessageBoxButtons.YesNo, supportedFormats);

                if (result == CustomMessageBoxResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = receiptPath,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.ShowWithFormat("Error", "Could not open the receipt: {0}",
                            CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok, ex.Message);
                    }
                }
                return;
            }

            try
            {
                Tools.OpenForm(new ReceiptViewer_Form(receiptPath));
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowWithFormat("Error", "Could not open the receipt viewer: {0}",
                    CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok, ex.Message);
            }
        }
        private static void MoveCategoryRows(List<DataGridViewRow> rowsToMove, CategoryMoveDirection direction)
        {
            Guna2DataGridView sourceGrid = direction switch
            {
                CategoryMoveDirection.PurchaseToSale => Categories_Form.Instance.Purchase_DataGridView,
                CategoryMoveDirection.PurchaseToRental => Categories_Form.Instance.Purchase_DataGridView,
                CategoryMoveDirection.SaleToPurchase => Categories_Form.Instance.Sale_DataGridView,
                CategoryMoveDirection.SaleToRental => Categories_Form.Instance.Sale_DataGridView,
                CategoryMoveDirection.RentalToPurchase => Categories_Form.Instance.Rent_DataGridView,
                CategoryMoveDirection.RentalToSale => Categories_Form.Instance.Rent_DataGridView,
                _ => null
            };

            Guna2DataGridView targetGrid = direction switch
            {
                CategoryMoveDirection.PurchaseToSale => Categories_Form.Instance.Sale_DataGridView,
                CategoryMoveDirection.PurchaseToRental => Categories_Form.Instance.Rent_DataGridView,
                CategoryMoveDirection.SaleToPurchase => Categories_Form.Instance.Purchase_DataGridView,
                CategoryMoveDirection.SaleToRental => Categories_Form.Instance.Rent_DataGridView,
                CategoryMoveDirection.RentalToPurchase => Categories_Form.Instance.Purchase_DataGridView,
                CategoryMoveDirection.RentalToSale => Categories_Form.Instance.Sale_DataGridView,
                _ => null
            };

            List<Category> sourceList = direction switch
            {
                CategoryMoveDirection.PurchaseToSale => MainMenu_Form.Instance.CategoryPurchaseList,
                CategoryMoveDirection.PurchaseToRental => MainMenu_Form.Instance.CategoryPurchaseList,
                CategoryMoveDirection.SaleToPurchase => MainMenu_Form.Instance.CategorySaleList,
                CategoryMoveDirection.SaleToRental => MainMenu_Form.Instance.CategorySaleList,
                CategoryMoveDirection.RentalToPurchase => MainMenu_Form.Instance.CategoryRentalList,
                CategoryMoveDirection.RentalToSale => MainMenu_Form.Instance.CategoryRentalList,
                _ => null
            };

            List<Category> targetList = direction switch
            {
                CategoryMoveDirection.PurchaseToSale => MainMenu_Form.Instance.CategorySaleList,
                CategoryMoveDirection.PurchaseToRental => MainMenu_Form.Instance.CategoryRentalList,
                CategoryMoveDirection.SaleToPurchase => MainMenu_Form.Instance.CategoryPurchaseList,
                CategoryMoveDirection.SaleToRental => MainMenu_Form.Instance.CategoryRentalList,
                CategoryMoveDirection.RentalToPurchase => MainMenu_Form.Instance.CategoryPurchaseList,
                CategoryMoveDirection.RentalToSale => MainMenu_Form.Instance.CategorySaleList,
                _ => null
            };

            MainMenu_Form.IsProgramLoading = true;
            int successfulMoves = 0;

            foreach (DataGridViewRow row in rowsToMove)
            {
                string categoryName = row.Cells[0].Value.ToString();
                Category? category = MainMenu_Form.GetCategoryCategoryNameIsFrom(sourceList, categoryName);

                // Check if category is being used in transactions
                if (IsCategoryBeingUsedInTransactions(categoryName))
                {
                    DataGridViewManager.ShowInUseMessage("category", "moved");
                    continue;
                }

                // Check if category has products
                if (category != null && category.ProductList.Count > 0)
                {

                    // Prepare products list for message box
                    string allProductsList = string.Join("\n", category.ProductList.Select(p => $"• {p.Name} ({p.CompanyOfOrigin})"));

                    // Determine target name based on direction
                    string targetName = direction switch
                    {
                        CategoryMoveDirection.PurchaseToSale => "Sales",
                        CategoryMoveDirection.PurchaseToRental => "Rentals",
                        CategoryMoveDirection.SaleToPurchase => "Purchases",
                        CategoryMoveDirection.SaleToRental => "Rentals",
                        CategoryMoveDirection.RentalToPurchase => "Purchases",
                        CategoryMoveDirection.RentalToSale => "Sales",
                        _ => ""
                    };

                    // Ask user if they want to move products
                    CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                        "Move category with {0} products",
                        "The category '{1}' contains the following products:\n\n{2}\n\nDo you want to move all these products to the {3} category?",
                        CustomMessageBoxIcon.Question,
                        CustomMessageBoxButtons.OkCancel,
                        category.ProductList.Count, categoryName, allProductsList, targetName);

                    if (result != CustomMessageBoxResult.Ok)
                    {
                        continue;
                    }

                    // Find or create a category in the target list
                    Category targetCategory = targetList.FirstOrDefault(c => c.Name == categoryName);
                    if (targetCategory == null)
                    {
                        targetCategory = new Category { Name = categoryName };
                        targetList.Add(targetCategory);
                    }

                    // Open Products_Form if it exists
                    Products_Form? productsForm = Tools.IsFormOpen<Products_Form>()
                        ? (Products_Form)Application.OpenForms[nameof(Products_Form)]
                        : null;

                    // Determine which grids to update based on current view
                    Guna2DataGridView sourceProductGrid = direction switch
                    {
                        CategoryMoveDirection.PurchaseToSale => productsForm?.Purchase_DataGridView,
                        CategoryMoveDirection.PurchaseToRental => productsForm?.Purchase_DataGridView,
                        CategoryMoveDirection.SaleToPurchase => productsForm?.Sale_DataGridView,
                        CategoryMoveDirection.SaleToRental => productsForm?.Sale_DataGridView,
                        CategoryMoveDirection.RentalToPurchase => productsForm?.Rentals_DataGridView,
                        CategoryMoveDirection.RentalToSale => productsForm?.Rentals_DataGridView,
                        _ => null
                    };

                    Guna2DataGridView targetProductGrid = direction switch
                    {
                        CategoryMoveDirection.PurchaseToSale => productsForm?.Sale_DataGridView,
                        CategoryMoveDirection.PurchaseToRental => productsForm?.Rentals_DataGridView,
                        CategoryMoveDirection.SaleToPurchase => productsForm?.Purchase_DataGridView,
                        CategoryMoveDirection.SaleToRental => productsForm?.Rentals_DataGridView,
                        CategoryMoveDirection.RentalToPurchase => productsForm?.Purchase_DataGridView,
                        CategoryMoveDirection.RentalToSale => productsForm?.Sale_DataGridView,
                        _ => null
                    };

                    // Move products
                    foreach (Product product in category.ProductList.ToList())
                    {
                        category.ProductList.Remove(product);
                        targetCategory.AddProduct(product);

                        // Update Products_Form if open
                        if (productsForm != null && sourceProductGrid != null && targetProductGrid != null)
                        {
                            // Find and remove the product row from source grid
                            DataGridViewRow? productRowToRemove = sourceProductGrid.Rows
                                .Cast<DataGridViewRow>()
                                .FirstOrDefault(r =>
                                    r.Cells[Products_Form.Column.ProductName.ToString()].Value.ToString() == product.Name &&
                                    r.Cells[Products_Form.Column.CompanyOfOrigin.ToString()].Value.ToString() == product.CompanyOfOrigin
                                );

                            if (productRowToRemove != null)
                            {
                                sourceProductGrid.Rows.Remove(productRowToRemove);
                            }

                            // Add product to the target grid
                            targetProductGrid.Rows.Add(
                                product.ProductID,
                                product.Name,
                                targetCategory.Name,
                                product.CountryOfOrigin,
                                product.CompanyOfOrigin
                            );
                        }
                    }
                }

                // Move the category row
                sourceGrid.Rows.Remove(row);
                targetGrid.Rows.Add(row);

                // Update category lists
                sourceList.Remove(category);
                targetList.Add(category);

                successfulMoves++;
            }

            if (successfulMoves > 0)
            {
                string targetName = direction switch
                {
                    CategoryMoveDirection.PurchaseToSale => "Sales",
                    CategoryMoveDirection.PurchaseToRental => "Rentals",
                    CategoryMoveDirection.SaleToPurchase => "Purchases",
                    CategoryMoveDirection.SaleToRental => "Rentals",
                    CategoryMoveDirection.RentalToPurchase => "Purchases",
                    CategoryMoveDirection.RentalToSale => "Sales",
                    _ => ""
                };

                string message = successfulMoves == 1
                    ? $"Moved {successfulMoves} category to {targetName}"
                    : $"Moved {successfulMoves} categories to {targetName}";
                CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 2, message);
            }

            // Update UI
            LabelManager.ShowTotalLabel(Categories_Form.Instance.Total_Label, sourceGrid);
            DataGridViewManager.SortDataGridViewByCurrentDirection(targetGrid);

            // Determine source and target options for saving
            MainMenu_Form.SelectedOption sourceOption = direction switch
            {
                CategoryMoveDirection.PurchaseToSale => MainMenu_Form.SelectedOption.CategoryPurchases,
                CategoryMoveDirection.PurchaseToRental => MainMenu_Form.SelectedOption.CategoryPurchases,
                CategoryMoveDirection.SaleToPurchase => MainMenu_Form.SelectedOption.CategorySales,
                CategoryMoveDirection.SaleToRental => MainMenu_Form.SelectedOption.CategorySales,
                CategoryMoveDirection.RentalToPurchase => MainMenu_Form.SelectedOption.CategoryRentals,
                CategoryMoveDirection.RentalToSale => MainMenu_Form.SelectedOption.CategoryRentals,
                _ => MainMenu_Form.SelectedOption.CategoryPurchases
            };

            MainMenu_Form.SelectedOption targetOption = direction switch
            {
                CategoryMoveDirection.PurchaseToSale => MainMenu_Form.SelectedOption.CategorySales,
                CategoryMoveDirection.PurchaseToRental => MainMenu_Form.SelectedOption.CategoryRentals,
                CategoryMoveDirection.SaleToPurchase => MainMenu_Form.SelectedOption.CategoryPurchases,
                CategoryMoveDirection.SaleToRental => MainMenu_Form.SelectedOption.CategoryRentals,
                CategoryMoveDirection.RentalToPurchase => MainMenu_Form.SelectedOption.CategoryPurchases,
                CategoryMoveDirection.RentalToSale => MainMenu_Form.SelectedOption.CategorySales,
                _ => MainMenu_Form.SelectedOption.CategorySales
            };

            // Save changes to file
            MainMenu_Form.Instance.SaveCategoriesToFile(sourceOption);
            MainMenu_Form.Instance.SaveCategoriesToFile(targetOption);

            MainMenu_Form.IsProgramLoading = false;
        }
        private static bool IsCategoryBeingUsedInTransactions(string categoryName)
        {
            foreach (DataGridViewRow row in MainMenu_Form.Instance.GetAllRows())
            {
                if (row.Cells[ReadOnlyVariables.Category_column].Value?.ToString() == categoryName)
                {
                    return true;
                }

                // Check if transaction has multiple items
                if (row.Tag is not (List<string> items, TagData))
                {
                    continue;
                }

                // Do not check receipt if present
                int itemsToCheck = items[^1].StartsWith(ReadOnlyVariables.Receipt_text)
                    ? items.Count - 1
                    : items.Count;

                // Check each item's category
                for (int i = 0; i < itemsToCheck; i++)
                {
                    string[] itemDetails = items[i].Split(',');
                    if (itemDetails.Length > 1 && itemDetails[1] == categoryName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private static void RestoreSelectionAndScroll(DataGridView gridView, int previousIndex, int scrollPosition)
        {
            gridView.ClearSelection();

            // Select the row at the previous index or the last row if the previous index is now out of bounds
            int newRowIndex = previousIndex < gridView.Rows.Count - 1 ? previousIndex : gridView.Rows.Count - 1;
            if (newRowIndex >= 0 && newRowIndex < gridView.Rows.Count)
            {
                gridView.Rows[newRowIndex].Selected = true;
            }

            // Restore scroll position
            if (scrollPosition != 0 && scrollPosition < gridView.Rows.Count)
            {
                gridView.FirstDisplayedScrollingRowIndex = scrollPosition;
            }
        }
        private static void ExportReceipt(object sender, EventArgs e)
        {
            Guna2DataGridView grid = (Guna2DataGridView)Panel.Tag;
            ReceiptManager.ExportSelectedReceipts(grid);
        }
        private static void ShowItems(object sender, EventArgs e)
        {
            Guna2DataGridView grid = (Guna2DataGridView)Panel.Tag;
            Tools.OpenForm(new ItemsInTransaction_Form(grid.SelectedRows[0]));
        }
        private static void ViewReturnDetails(object sender, EventArgs e)
        {
            Guna2DataGridView grid = (Guna2DataGridView)Panel.Tag;
            if (grid.SelectedRows.Count != 1) { return; }

            DataGridViewRow selectedRow;

            // Check if we're in the items view of a transaction
            if (MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.ItemsInPurchase ||
                MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.ItemsInSale)
            {
                // Use the main transaction row, not the individual item row
                selectedRow = DataGridViewManager.SelectedRowInMainMenu;
            }
            else
            {
                // Normal case - we're in the main purchases/sales view
                selectedRow = grid.SelectedRows[0];
            }

            using ViewTransactionDetails_Form viewForm = new(selectedRow, ViewTransactionDetails_Form.ViewType.Return);
            viewForm.ShowDialog();
        }
        private static void ViewLossDetails(object sender, EventArgs e)
        {
            Guna2DataGridView grid = (Guna2DataGridView)Panel.Tag;
            if (grid.SelectedRows.Count != 1) { return; }

            DataGridViewRow selectedRow;

            // Check if we're in the items view of a transaction
            if (MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.ItemsInPurchase ||
                MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.ItemsInSale)
            {
                // Use the main transaction row, not the individual item row
                selectedRow = DataGridViewManager.SelectedRowInMainMenu;
            }
            else
            {
                // Normal case - we're in the main purchases/sales view
                selectedRow = grid.SelectedRows[0];
            }

            using ViewTransactionDetails_Form viewForm = new(selectedRow, ViewTransactionDetails_Form.ViewType.Loss);
            viewForm.ShowDialog();
        }
        private static void ReturnProduct(object sender, EventArgs e)
        {
            Guna2DataGridView grid = (Guna2DataGridView)Panel.Tag;
            if (grid.SelectedRows.Count != 1) { return; }

            DataGridViewRow selectedRow;
            bool isPurchase;

            // Check if we're in the items view of a transaction
            if (MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.ItemsInPurchase ||
                MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.ItemsInSale)
            {
                // Use the main transaction row, not the individual item row
                selectedRow = DataGridViewManager.SelectedRowInMainMenu;
                isPurchase = MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.ItemsInPurchase;
            }
            else
            {
                // Normal case - we're in the main purchases/sales view
                selectedRow = grid.SelectedRows[0];
                isPurchase = MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.Purchases;
            }

            using ReturnProduct_Form returnForm = new(selectedRow, isPurchase);
            if (returnForm.ShowDialog() == DialogResult.OK)
            {
                Hide();

                // Refresh ItemsInTransaction_Form if it's open
                if (Tools.IsFormOpen<ItemsInTransaction_Form>()
                    && Application.OpenForms[nameof(ItemsInTransaction_Form)] is ItemsInTransaction_Form itemsForm)
                {
                    itemsForm.RefreshItemReturnStatus();
                }
            }
        }
        private static void UndoReturnProduct(object sender, EventArgs e)
        {
            Guna2DataGridView grid = (Guna2DataGridView)Panel.Tag;
            if (grid.SelectedRows.Count != 1) { return; }

            DataGridViewRow selectedRow;

            // Check if we're in the items view of a transaction
            if (MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.ItemsInPurchase ||
                MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.ItemsInSale)
            {
                // Use the main transaction row, not the individual item row
                selectedRow = DataGridViewManager.SelectedRowInMainMenu;
            }
            else
            {
                // Normal case - we're in the main purchases/sales view
                selectedRow = grid.SelectedRows[0];
            }

            using UndoReturn_Form undoForm = new(selectedRow);
            if (undoForm.ShowDialog() == DialogResult.OK)
            {
                Hide();

                // Refresh ItemsInTransaction_Form if it's open
                if (Tools.IsFormOpen<ItemsInTransaction_Form>()
                    && Application.OpenForms[nameof(ItemsInTransaction_Form)] is ItemsInTransaction_Form itemsForm)
                {
                    itemsForm.RefreshItemReturnStatus();
                }
            }
        }
        private static void MarkAsLost(object sender, EventArgs e)
        {
            Guna2DataGridView grid = (Guna2DataGridView)Panel.Tag;
            if (grid.SelectedRows.Count != 1) { return; }

            DataGridViewRow selectedRow;
            bool isPurchase;

            // Check if we're in the items view of a transaction
            if (MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.ItemsInPurchase ||
                MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.ItemsInSale)
            {
                // Use the main transaction row, not the individual item row
                selectedRow = DataGridViewManager.SelectedRowInMainMenu;
                isPurchase = MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.ItemsInPurchase;
            }
            else
            {
                // Normal case - we're in the main purchases/sales view
                selectedRow = grid.SelectedRows[0];
                isPurchase = MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.Purchases;
            }

            using MarkProductAsLost_Form lostForm = new(selectedRow, isPurchase);
            if (lostForm.ShowDialog() == DialogResult.OK)
            {
                Hide();

                // Refresh ItemsInTransaction_Form if it's open
                if (Tools.IsFormOpen<ItemsInTransaction_Form>()
                    && Application.OpenForms[nameof(ItemsInTransaction_Form)] is ItemsInTransaction_Form itemsForm)
                {
                    itemsForm.RefreshItemReturnStatus();
                }
            }
        }
        private static void UndoLoss(object sender, EventArgs e)
        {
            Guna2DataGridView grid = (Guna2DataGridView)Panel.Tag;
            if (grid.SelectedRows.Count != 1) { return; }

            DataGridViewRow selectedRow;

            // Check if we're in the items view of a transaction
            if (MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.ItemsInPurchase ||
                MainMenu_Form.Instance.Selected == MainMenu_Form.SelectedOption.ItemsInSale)
            {
                // Use the main transaction row, not the individual item row
                selectedRow = DataGridViewManager.SelectedRowInMainMenu;
            }
            else
            {
                // Normal case - we're in the main purchases/sales view
                selectedRow = grid.SelectedRows[0];
            }

            using UndoLoss_Form undoLossForm = new(selectedRow);
            if (undoLossForm.ShowDialog() == DialogResult.OK)
            {
                Hide();

                // Refresh ItemsInTransaction_Form if it's open
                if (Tools.IsFormOpen<ItemsInTransaction_Form>()
                    && Application.OpenForms[nameof(ItemsInTransaction_Form)] is ItemsInTransaction_Form itemsForm)
                {
                    itemsForm.RefreshItemReturnStatus();
                }
            }
        }
        private static void DeleteRow(object sender, EventArgs e)
        {
            Guna2DataGridView grid = (Guna2DataGridView)Panel.Tag;
            int selectedCount = grid.SelectedRows.Count;

            // Show confirmation dialog
            CustomMessageBoxResult result;

            if (selectedCount == 1)
            {
                // Determine the type and identifier based on the current selection
                string itemType;
                string identifier;

                switch (MainMenu_Form.Instance.Selected)
                {
                    case MainMenu_Form.SelectedOption.Accountants:
                        itemType = "the accountant";
                        identifier = grid.SelectedRows[0].Cells[Accountants_Form.Column.AccountantName.ToString()].Value?.ToString() ?? "Unknown";
                        break;

                    case MainMenu_Form.SelectedOption.Companies:
                        itemType = "the company";
                        identifier = grid.SelectedRows[0].Cells[Companies_Form.Column.Company.ToString()].Value?.ToString() ?? "Unknown";
                        break;

                    case MainMenu_Form.SelectedOption.CategoryPurchases:
                    case MainMenu_Form.SelectedOption.CategorySales:
                        itemType = "the category";
                        identifier = grid.SelectedRows[0].Cells[Categories_Form.Column.CategoryName.ToString()].Value?.ToString() ?? "Unknown";
                        break;

                    case MainMenu_Form.SelectedOption.ProductPurchases:
                    case MainMenu_Form.SelectedOption.ProductSales:
                        itemType = "the product";
                        identifier = grid.SelectedRows[0].Cells[Products_Form.Column.ProductName.ToString()].Value?.ToString() ?? "Unknown";
                        break;

                    case MainMenu_Form.SelectedOption.Customers:
                        itemType = "the customer";
                        string firstName = grid.SelectedRows[0].Cells[Customers_Form.Column.FirstName.ToString()].Value?.ToString() ?? "";
                        string lastName = grid.SelectedRows[0].Cells[Customers_Form.Column.LastName.ToString()].Value?.ToString() ?? "";
                        identifier = $"{firstName} {lastName}".Trim();
                        if (string.IsNullOrEmpty(identifier))
                        {
                            identifier = "Unknown";
                        }
                        break;

                    case MainMenu_Form.SelectedOption.Purchases:
                    case MainMenu_Form.SelectedOption.Sales:
                    case MainMenu_Form.SelectedOption.ItemsInPurchase:
                    case MainMenu_Form.SelectedOption.ItemsInSale:
                    default:
                        itemType = "transaction";
                        identifier = grid.SelectedRows[0].Cells[0].Value?.ToString() ?? "Unknown";
                        break;
                }

                result = CustomMessageBox.ShowWithFormat(
                    "Confirm Deletion",
                    "Are you sure you want to delete {0} '{1}'?",
                    CustomMessageBoxIcon.Question,
                    CustomMessageBoxButtons.YesNo,
                    itemType,
                    identifier
                );
            }
            else
            {
                // Determine the plural form based on the current selection
                string itemType = MainMenu_Form.Instance.Selected switch
                {
                    MainMenu_Form.SelectedOption.Accountants => "accountants",
                    MainMenu_Form.SelectedOption.Companies => "companies",
                    MainMenu_Form.SelectedOption.CategoryPurchases or MainMenu_Form.SelectedOption.CategorySales => "categories",
                    MainMenu_Form.SelectedOption.ProductPurchases or MainMenu_Form.SelectedOption.ProductSales => "products",
                    MainMenu_Form.SelectedOption.Customers => "customers",
                    MainMenu_Form.SelectedOption.Purchases or MainMenu_Form.SelectedOption.Sales => "transactions",
                    _ => "rows"
                };

                result = CustomMessageBox.ShowWithFormat(
                    "Confirm Deletion",
                    "Are you sure you want to delete these {0} {1}?",
                    CustomMessageBoxIcon.Question,
                    CustomMessageBoxButtons.YesNo,
                    selectedCount,
                    itemType
                );
            }

            // Only proceed if user confirms
            if (result != CustomMessageBoxResult.Yes)
            {
                return;
            }

            int index = grid.SelectedRows[^1].Index;

            // Delete all selected rows
            foreach (DataGridViewRow item in grid.SelectedRows)
            {
                DataGridViewRowCancelEventArgs eventArgs = new(item);
                DataGridViewManager.DataGridView_UserDeletingRow(grid, eventArgs);
                if (item.Index == -1) { continue; }  // This can happen if ItemsInTransaction_Form is closed in DataGridView_UserDeletingRow()

                if (!eventArgs.Cancel)
                {
                    grid.Rows.Remove(item);
                }
            }

            // Select the row under the row that was just deleted
            if (grid.Rows.Count != 0)
            {
                // If the deleted row was not the last one, select the next row
                if (index < grid.Rows.Count)
                {
                    grid.Rows[index].Selected = true;
                }
                else  // If the deleted row was the last one, select the new last row
                {
                    grid.Rows[^1].Selected = true;
                }
            }
        }
        private static void RentOut(object sender, EventArgs e)
        {
            Guna2DataGridView grid = (Guna2DataGridView)Panel.Tag;
            if (grid.SelectedRows.Count != 1) { return; }

            DataGridViewRow selectedRow = grid.SelectedRows[0];

            // Get the rental item from the row tag
            if (selectedRow.Tag is RentalItem rentalItem)
            {
                // Check if any units are available
                if (rentalItem.QuantityAvailable == 0)
                {
                    CustomMessageBox.ShowWithFormat(
                        "No Units Available",
                        "'{0}' has no units available for rent. All units are currently rented or in maintenance.",
                        CustomMessageBoxIcon.Info,
                        CustomMessageBoxButtons.Ok,
                        rentalItem.ProductName);
                    return;
                }

                Tools.OpenForm(new RentOutItem_Form(rentalItem, selectedRow));
                Hide();
            }
        }

        // Helper methods
        public static void Hide()
        {
            Panel?.Parent?.Controls.Remove(Panel);
        }
        public static string GetFilePathFromRowTag(object tag)
        {
            if (tag is (List<string> tagList, TagData) && tagList[^1].Contains('\\'))
            {
                return ProcessDirectoryFromString(tagList[^1]);
            }
            else if (tag is (string tagString, TagData))
            {
                return ProcessDirectoryFromString(tagString);
            }
            else if (tag is string tagString2)
            {
                return ProcessDirectoryFromString(tagString2);
            }
            return "";
        }
        private static string ProcessDirectoryFromString(string path)
        {
            string[] pathParts = path.Split(Path.DirectorySeparatorChar);
            if (pathParts.Length > 7 && pathParts[7] == ReadOnlyVariables.CompanyName_text)
            {
                pathParts[7] = Directories.CompanyName;
            }
            string newPath = string.Join(Path.DirectorySeparatorChar.ToString(), pathParts);

            newPath = newPath.Replace(ReadOnlyVariables.Receipt_text, "");

            return File.Exists(newPath) ? newPath : "";
        }
    }
}