using Argo_Books.ReportGenerator;
using Argo_Books.Theme;
using Guna.UI2.WinForms;
using Argo_Books.Theme;

namespace Argo_Books.UI
{
    /// <summary>
    /// Static helper for displaying undo/redo history dropdown panels.
    /// </summary>
    public static class UndoRedoHistoryDropdown
    {
        // Constants
        private const int ITEM_HEIGHT = 30;
        private const int MAX_VISIBLE_ITEMS = 10;
        private const int PANEL_WIDTH = 300;
        private const int PANEL_PADDING = 5;
        private const int LABEL_HEIGHT = 35;

        // Static fields for tracking current dropdown
        public static Guna2Panel? Panel { get; private set; }
        private static Panel? _itemsPanel;
        public static Label? ActionLabel { get; private set; }
        private static readonly List<HistoryItem> _historyItems = [];
        private static bool _isUndoHistory;

        /// <summary>
        /// Represents a single item in the history list.
        /// </summary>
        private class HistoryItem : Guna2Button
        {
            public int Index { get; }

            public HistoryItem(int index, string description, Action<int> onClick)
            {
                Index = index;
                Height = ITEM_HEIGHT;
                Text = description;
                Font = new Font("Segoe UI", 9, FontStyle.Regular);
                Location = new Point(35, 0);
                Size = new Size(PANEL_WIDTH - 40, ITEM_HEIGHT);
                FillColor = CustomColors.ControlBack;
                ForeColor = CustomColors.Text;
                BorderColor = CustomColors.ControlPanelBorder;

                // Click event
                Click += (s, e) => onClick(index);
            }
        }

        /// <summary>
        /// Creates the base panel structure for the dropdown.
        /// </summary>
        public static void Construct()
        {
            Panel = new()
            {
                BorderThickness = 1,
                BorderRadius = 4,
                Visible = true,
                Width = PANEL_WIDTH + PANEL_PADDING * 2,
                FillColor = CustomColors.ControlBack,
                BorderColor = CustomColors.ControlPanelBorder
            };

            // Items container panel
            _itemsPanel = new Panel
            {
                Location = new Point(PANEL_PADDING, PANEL_PADDING),
                Width = PANEL_WIDTH,
                AutoScroll = true
            };

            // Action count label at bottom
            ActionLabel = new Label
            {
                Width = PANEL_WIDTH,
                Height = LABEL_HEIGHT,
                Text = "",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = CustomColors.Text,
                BackColor = CustomColors.MainBackground,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };

            Panel.Controls.Add(_itemsPanel);
            Panel.Controls.Add(ActionLabel);
        }

        /// <summary>
        /// Shows the undo/redo history dropdown at the specified location.
        /// </summary>
        public static void Show(Control parent, Point location, UndoRedoManager undoRedoManager, bool isUndoHistory, Action onActionPerformed)
        {
            // Remove any existing dropdown
            Remove();

            _isUndoHistory = isUndoHistory;

            parent.Controls.Add(Panel);
            Panel.Location = location;

            // Populate with history items
            PopulateHistory(undoRedoManager, isUndoHistory, onActionPerformed);

            Panel.BringToFront();
            Panel.Focus();
        }

        /// <summary>
        /// Removes the current dropdown from view.
        /// </summary>
        public static void Remove()
        {
            Panel.Parent?.Controls.Remove(Panel);
            _historyItems.Clear();
            if (ActionLabel != null)
            {
                ActionLabel.Visible = false;
            }
        }

        /// <summary>
        /// Populates the dropdown with history items.
        /// </summary>
        private static void PopulateHistory(UndoRedoManager undoRedoManager, bool isUndoHistory, Action onActionPerformed)
        {
            if (_itemsPanel == null || Panel == null || ActionLabel == null) { return; }

            _historyItems.Clear();
            _itemsPanel.Controls.Clear();

            string[] descriptions = isUndoHistory
                ? undoRedoManager.GetUndoDescriptions()
                : undoRedoManager.GetRedoDescriptions();

            // Calculate button width based on whether scrollbar will appear
            bool willHaveScrollbar = descriptions.Length > MAX_VISIBLE_ITEMS;
            int buttonWidth = willHaveScrollbar
                ? PANEL_WIDTH - SystemInformation.VerticalScrollBarWidth - 4
                : PANEL_WIDTH;

            int yPosition = 0;
            for (int i = 0; i < descriptions.Length; i++)
            {
                HistoryItem item = new(i, descriptions[i],
                    (index) => OnItemClick(index, undoRedoManager, isUndoHistory, onActionPerformed))
                {
                    Location = new Point(0, yPosition),
                    Size = new Size(buttonWidth, ITEM_HEIGHT)
                };

                // Add hover event handlers
                item.MouseEnter += (s, e) => OnItemHover(item.Index);
                item.MouseLeave += (s, e) => OnItemLeave();

                _historyItems.Add(item);
                _itemsPanel.Controls.Add(item);

                yPosition += ITEM_HEIGHT;
            }

            // Set panel height for scrolling
            if (_historyItems.Count > 0)
            {
                int totalHeight = _historyItems.Count * ITEM_HEIGHT;
                int itemsPanelHeight = Math.Min(totalHeight, MAX_VISIBLE_ITEMS * ITEM_HEIGHT);
                _itemsPanel.Height = itemsPanelHeight;

                // Position and size the label
                ActionLabel.Location = new Point(PANEL_PADDING, itemsPanelHeight + PANEL_PADDING);
                ActionLabel.Width = PANEL_WIDTH;

                // Set total panel height including label
                Panel.Height = itemsPanelHeight + LABEL_HEIGHT + PANEL_PADDING * 2;

                // Show the label with default text
                UpdateActionLabel(1);

                // Apply custom scrollbar theme after the scrollbar is created
                if (willHaveScrollbar)
                {
                    ThemeManager.CustomizeScrollBar(_itemsPanel);
                }
            }
        }

        /// <summary>
        /// Handles hover effect over history items.
        /// </summary>
        private static void OnItemHover(int hoveredIndex)
        {
            // Set border thickness for hovered item and all items above it
            for (int i = 0; i <= hoveredIndex && i < _historyItems.Count; i++)
            {
                _historyItems[i].BorderThickness = 1;
                _historyItems[i].FillColor = CustomColors.PanelBtnHover;
            }

            // Reset border thickness for items below the hovered item
            for (int i = hoveredIndex + 1; i < _historyItems.Count; i++)
            {
                _historyItems[i].BorderThickness = 0;
                _historyItems[i].FillColor = CustomColors.ControlBack;
            }

            // Update the action label
            UpdateActionLabel(hoveredIndex + 1);
        }

        /// <summary>
        /// Resets hover effects when mouse leaves items.
        /// </summary>
        private static void OnItemLeave()
        {
            foreach (HistoryItem item in _historyItems)
            {
                item.BorderThickness = 0;
                item.FillColor = CustomColors.ControlBack;
            }

            // Reset the action label to default (1 action)
            UpdateActionLabel(1);
        }

        /// <summary>
        /// Updates the action label text based on the number of selected actions.
        /// </summary>
        private static void UpdateActionLabel(int count)
        {
            if (ActionLabel == null) { return; }

            string actionType = _isUndoHistory ? "Undo" : "Redo";
            string pluralSuffix = count == 1 ? "" : "s";

            ActionLabel.Text = $"{actionType} {count} Action{pluralSuffix}";
            ActionLabel.Visible = true;
        }

        /// <summary>
        /// Handles clicking on a history item.
        /// </summary>
        private static void OnItemClick(int index, UndoRedoManager undoRedoManager,
            bool isUndoHistory, Action onActionPerformed)
        {
            // Perform multiple undo/redo operations to reach the selected point
            int operationCount = index + 1;

            for (int i = 0; i < operationCount; i++)
            {
                if (isUndoHistory)
                {
                    if (undoRedoManager.CanUndo)
                    {
                        undoRedoManager.Undo();
                    }
                }
                else
                {
                    if (undoRedoManager.CanRedo)
                    {
                        undoRedoManager.Redo();
                    }
                }
            }

            // Notify that an action was performed
            onActionPerformed?.Invoke();

            Remove();
        }
    }
}