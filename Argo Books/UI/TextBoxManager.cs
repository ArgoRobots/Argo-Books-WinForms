using Argo_Books.DataClasses;
using Guna.UI2.WinForms;
using Argo_Books;

namespace Argo_Books.UI
{
    /// <summary>
    /// Manages keyboard shortcuts and text manipulation functionality for Guna2TextBox controls.
    /// Provides undo/redo capability and clipboard operations.
    /// </summary>
    internal class TextBoxManager
    {
        // Dictionaries to hold undo/redo stacks and flags for each TextBox
        private static readonly Dictionary<Guna2TextBox, Stack<TextState>> _undoStacks = [];
        private static readonly Dictionary<Guna2TextBox, Stack<TextState>> _redoStacks = [];
        private static readonly Dictionary<Guna2TextBox, bool> _isTextChangedByUserFlags = [];
        private static readonly Dictionary<Guna2TextBox, bool> _showRightClickPanelFlags = [];
        private const byte _maxStackSize = 250;
        private static Point _mouseDownLocation;

        /// <summary>
        /// Represents the state of a TextBox, including its text content and cursor position.
        /// </summary>
        private class TextState(string text, int cursorPosition)
        {
            public string Text { get; } = text ?? "";
            public int CursorPosition { get; } = cursorPosition >= 0 ? cursorPosition : 0;
        }

        // Main methods
        /// <summary>
        /// Attaches keyboard shortcut functionality (copy, paste, undo, redo) and other custom behavior to a Guna2TextBox.
        /// Shows the right-click context menu panel by default.
        /// </summary>
        public static void Attach(params Guna2TextBox[] textBoxes)
        {
            Attach(true, textBoxes);
        }

        /// <summary>
        /// Attaches keyboard shortcut functionality (copy, paste, undo, redo) and other custom behavior to a Guna2TextBox.
        /// </summary>
        public static void Attach(bool showRightClickPanel = true, params Guna2TextBox[] textBoxes)
        {
            foreach (Guna2TextBox textBox in textBoxes)
            {
                if (IsAttached(textBox)) { continue; }

                InitializeTextBox(textBox, showRightClickPanel);
                AttachEventHandlers(textBox, showRightClickPanel);
                TextBoxTooltip.SetOverflowTooltip(textBox);
            }
        }

        /// <summary>
        /// Initializes the undo/redo stacks and flags for a TextBox.
        /// </summary>
        private static void InitializeTextBox(Guna2TextBox textBox, bool showRightClickPanel)
        {
            _undoStacks[textBox] = new Stack<TextState>();
            _redoStacks[textBox] = new Stack<TextState>();
            _isTextChangedByUserFlags[textBox] = true;
            _showRightClickPanelFlags[textBox] = showRightClickPanel;
            _undoStacks[textBox].Push(new TextState(textBox.Text, textBox.SelectionStart));
        }
        private static void AttachEventHandlers(Guna2TextBox textBox, bool showRightClickPanel)
        {
            textBox.TextChanged += TextBox_TextChanged;
            textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            textBox.KeyDown += TextBox_KeyDown;

            // Only attach mouse events if right-click panel should be shown
            if (showRightClickPanel)
            {
                textBox.MouseDown += TextBox_MouseDown;
                textBox.MouseUp += TextBox_MouseUp;
            }
        }
        private static bool IsAttached(Guna2TextBox textBox) => _undoStacks.ContainsKey(textBox);
        public static void HideRightClickPanel()
        {
            RightClickTextBox_Panel.Parent?.Controls.Remove(RightClickTextBox_Panel);
        }

        // TextBox event handlers
        /// <summary>
        /// Handles text changes and manages undo/redo stacks.
        /// </summary>
        private static void TextBox_TextChanged(object sender, EventArgs e)
        {
            Guna2TextBox textBox = (Guna2TextBox)sender;

            if (!_isTextChangedByUserFlags[textBox]) { return; }

            Stack<TextState> undoStack = _undoStacks[textBox];
            Stack<TextState> redoStack = _redoStacks[textBox];

            if (undoStack.Count == 0 || undoStack.Peek().Text != textBox.Text)
            {
                UpdateUndoStack(textBox, undoStack);
                redoStack.Clear();
            }
        }

        /// <summary>
        /// Handles PreviewKeyDown event to ensure keyboard shortcuts work properly.
        /// </summary>
        private static void TextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.A || e.KeyCode == Keys.C || e.KeyCode == Keys.X ||
                            e.KeyCode == Keys.V || e.KeyCode == Keys.Z || e.KeyCode == Keys.Y))
            {
                e.IsInputKey = true;
            }
        }

        /// Handles KeyDown event for keyboard shortcuts and text manipulation.
        /// </summary>
        private static void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Block the '>' symbol
            if ((e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.Decimal) && e.Shift)
            {
                e.SuppressKeyPress = true;
                return;
            }

            if (e.Control && e.KeyCode == Keys.Y)
            {
                e.SuppressKeyPress = true;
                Guna2TextBox textBox = (Guna2TextBox)sender;
                Redo(textBox);
                return;
            }

            if (!IsInputKey(e.KeyCode) && !e.Control)
            {
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
            else
            {
                Guna2TextBox textBox = (Guna2TextBox)sender;
                if (e.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Z when e.Shift:
                            Redo(textBox);
                            break;
                        case Keys.Z:
                            Undo(textBox);
                            break;
                    }
                }
            }
        }
        private static void TextBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _mouseDownLocation = e.Location;
            }
        }
        private static void TextBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Guna2TextBox textBox = (Guna2TextBox)sender;

                // Check if right-click panel should be shown for this textbox
                if (!_showRightClickPanelFlags.TryGetValue(textBox, out bool showPanel) || !showPanel)
                {
                    return;
                }

                // Check if mouse hasn't moved too far from the down location
                byte threshold = 3;
                if (Math.Abs(e.Location.X - _mouseDownLocation.X) <= threshold &&
                    Math.Abs(e.Location.Y - _mouseDownLocation.Y) <= threshold)
                {
                    ShowRightClickMenu(textBox, e.Location);
                }
            }
        }

        // Methods for event handlers
        private static bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Tab:
                case Keys.Enter:
                case Keys.Escape:
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                case Keys.Delete:
                case Keys.Back:
                case Keys.Home:
                case Keys.End:
                case Keys.Subtract:
                case Keys.OemMinus:
                case Keys.Decimal:
                case Keys.OemPeriod:
                    return true;
            }

            char keyChar = (char)keyData;
            return char.IsLetterOrDigit(keyChar) ||
                   char.IsPunctuation(keyChar) ||
                   char.IsWhiteSpace(keyChar);
        }

        // Stack methods (undo, redo)
        /// <summary>
        /// Updates the undo stack and manages its size.
        /// </summary>
        private static void UpdateUndoStack(Guna2TextBox textBox, Stack<TextState> undoStack)
        {
            undoStack.Push(new TextState(textBox.Text, textBox.SelectionStart));

            if (undoStack.Count > _maxStackSize)
            {
                LimitStackSize(textBox);
            }
        }

        /// <summary>
        /// Limits the undo stack size to prevent memory bloat.
        /// </summary>
        private static void LimitStackSize(Guna2TextBox textBox)
        {
            Stack<TextState> tempStack = new();
            Stack<TextState> currentStack = _undoStacks[textBox];

            for (int i = 0; i < _maxStackSize && currentStack.Count > 0; i++)
            {
                tempStack.Push(currentStack.Pop());
            }

            _undoStacks[textBox] = new Stack<TextState>(tempStack.Reverse());
        }

        /// <summary>
        /// Performs undo operation if available.
        /// </summary>
        private static void Undo(Guna2TextBox textBox)
        {
            Stack<TextState> undoStack = _undoStacks[textBox];
            Stack<TextState> redoStack = _redoStacks[textBox];

            if (undoStack.Count > 1)
            {
                redoStack.Push(undoStack.Pop());
                TextState state = undoStack.Peek();
                UpdateTextBoxContent(textBox, state.Text, state.CursorPosition);
            }
        }

        /// <summary>
        /// Performs redo operation if available.
        /// </summary>
        private static void Redo(Guna2TextBox textBox)
        {
            Stack<TextState> undoStack = _undoStacks[textBox];
            Stack<TextState> redoStack = _redoStacks[textBox];

            if (redoStack.Count > 0)
            {
                TextState state = redoStack.Pop();
                undoStack.Push(state);
                UpdateTextBoxContent(textBox, state.Text, state.CursorPosition);
            }
        }

        /// <summary>
        /// Updates TextBox content without triggering TextChanged event.
        /// </summary>
        private static void UpdateTextBoxContent(Guna2TextBox textBox, string newText, int cursorPosition)
        {
            _isTextChangedByUserFlags[textBox] = false;
            textBox.Text = newText;
            textBox.SelectionStart = cursorPosition;
            _isTextChangedByUserFlags[textBox] = true;
        }

        // Right click menu
        public static Guna2Panel RightClickTextBox_Panel { get; private set; }
        public static void ConstructRightClickTextBoxMenu()
        {
            RightClickTextBox_Panel = CustomControls.ConstructPanelForMenu(
                new Size(CustomControls.PanelWidth, 6 * CustomControls.PanelButtonHeight + CustomControls.SpaceForPanel),
                "rightClickTextBox_Panel"
            );

            FlowLayoutPanel flowPanel = (FlowLayoutPanel)RightClickTextBox_Panel.Controls[0];

            Guna2Button selectAllBtn = CustomControls.ConstructBtnForMenu("Select all", CustomControls.PanelBtnWidth, flowPanel);
            selectAllBtn.Click += HandleSelectAll;
            CustomControls.ConstructKeyShortcut("Ctrl+A", selectAllBtn);

            Guna2Button copyBtn = CustomControls.ConstructBtnForMenu("Copy", CustomControls.PanelBtnWidth, flowPanel);
            copyBtn.Click += HandleCopy;
            CustomControls.ConstructKeyShortcut("Ctrl+C", copyBtn);

            Guna2Button pasteBtn = CustomControls.ConstructBtnForMenu("Paste", CustomControls.PanelBtnWidth, flowPanel);
            pasteBtn.Click += HandlePaste;
            CustomControls.ConstructKeyShortcut("Ctrl+V", pasteBtn);

            Guna2Button cutBtn = CustomControls.ConstructBtnForMenu("Cut", CustomControls.PanelBtnWidth, flowPanel);
            cutBtn.Click += HandleCut;
            CustomControls.ConstructKeyShortcut("Ctrl+X", cutBtn);

            Guna2Button undoBtn = CustomControls.ConstructBtnForMenu("Undo", CustomControls.PanelBtnWidth, flowPanel);
            undoBtn.Click += HandleUndo;
            CustomControls.ConstructKeyShortcut("Ctrl+Z", undoBtn);

            Guna2Button redoBtn = CustomControls.ConstructBtnForMenu("Redo", CustomControls.PanelBtnWidth, flowPanel);
            redoBtn.Click += HandleRedo;
            CustomControls.ConstructKeyShortcut("Ctrl+Y", redoBtn);
        }
        private static void HandleSelectAll(object sender, EventArgs e)
        {
            Guna2TextBox textBox = (Guna2TextBox)RightClickTextBox_Panel.Tag;
            textBox.SelectAll();
            textBox.Focus();
            HideRightClickPanel();
        }
        private static void HandleCopy(object sender, EventArgs e)
        {
            Guna2TextBox textBox = (Guna2TextBox)RightClickTextBox_Panel.Tag;
            if (textBox.SelectedText != "")
            {
                Clipboard.SetText(textBox.SelectedText);
            }
            HideRightClickPanel();
        }
        private static void HandlePaste(object sender, EventArgs e)
        {
            string clipboardText = Clipboard.GetText();
            if (string.IsNullOrEmpty(clipboardText)) { return; }

            Guna2TextBox textBox = (Guna2TextBox)RightClickTextBox_Panel.Tag;
            int cursorPosition = textBox.SelectionStart;
            string text = textBox.Text;

            if (textBox.SelectionLength > 0)
            {
                // Replace selected text
                text = text.Remove(textBox.SelectionStart, textBox.SelectionLength);
            }

            text = text.Insert(cursorPosition, clipboardText);
            textBox.Text = text;
            textBox.SelectionStart = cursorPosition + clipboardText.Length;

            HideRightClickPanel();
        }
        private static void HandleCut(object sender, EventArgs e)
        {
            Guna2TextBox textBox = (Guna2TextBox)RightClickTextBox_Panel.Tag;
            if (textBox.SelectedText == "") { return; }

            Clipboard.SetText(textBox.SelectedText);
            int cursorPosition = textBox.SelectionStart;
            textBox.Text = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength);
            textBox.SelectionStart = cursorPosition;

            HideRightClickPanel();
        }
        private static void HandleUndo(object sender, EventArgs e)
        {
            Guna2TextBox textBox = (Guna2TextBox)RightClickTextBox_Panel.Tag;
            Undo(textBox);
            HideRightClickPanel();
        }
        private static void HandleRedo(object sender, EventArgs e)
        {
            Guna2TextBox textBox = (Guna2TextBox)RightClickTextBox_Panel.Tag;
            Redo(textBox);
            HideRightClickPanel();
        }

        // Methods for right click menu
        private static void ShowRightClickMenu(Guna2TextBox textBox, Point mouseLocation)
        {
            // First hide other right click panels
            MainMenu_Form.CloseRightClickPanels();

            Form parentForm = textBox.FindForm();
            if (parentForm == null) { return; }

            // Convert textbox location to form coordinates
            Point textBoxInForm = parentForm.PointToClient(textBox.PointToScreen(Point.Empty));

            // Calculate initial position
            Point initialPosition = new(
                textBoxInForm.X + mouseLocation.X - ReadOnlyVariables.OffsetRightClickPanel,
                textBoxInForm.Y + textBox.Height
            );

            // Get form's client rectangle for boundary checking
            Rectangle formBounds = parentForm.ClientRectangle;

            // Adjust X position if panel would overflow horizontally
            int panelRight = initialPosition.X + RightClickTextBox_Panel.Width;
            if (panelRight > formBounds.Right)
            {
                initialPosition.X = formBounds.Right - RightClickTextBox_Panel.Width;
            }

            // Adjust Y position if panel would overflow vertically
            int panelBottom = initialPosition.Y + RightClickTextBox_Panel.Height;
            if (panelBottom > formBounds.Bottom)
            {
                // Try to show above the textbox instead
                int alternateY = textBoxInForm.Y - RightClickTextBox_Panel.Height;
                if (alternateY >= formBounds.Top)
                {
                    initialPosition.Y = alternateY;
                }
                else
                {
                    // If it doesn't fit above either, place it at the bottom of the form
                    initialPosition.Y = formBounds.Bottom - RightClickTextBox_Panel.Height;
                }
            }

            // Show the panel at the calculated position
            RightClickTextBox_Panel.Tag = textBox;
            parentForm.Controls.Add(RightClickTextBox_Panel);
            RightClickTextBox_Panel.Location = initialPosition;
            RightClickTextBox_Panel.BringToFront();
        }
    }
}