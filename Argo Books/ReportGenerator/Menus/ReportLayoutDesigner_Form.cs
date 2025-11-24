using Argo_Books.ReportGenerator;
using Argo_Books.ReportGenerator.Elements;
using Argo_Books.Theme;
using Argo_Books.UI;
using Guna.UI2.WinForms;
using Argo_Books.Language;
using Argo_Books.Theme;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using Timer = System.Windows.Forms.Timer;

namespace Argo_Books.ReportGenerator.Menus
{
    /// <summary>
    /// Second step in report generation - drag-and-drop canvas for arranging report elements.
    /// </summary>
    public partial class ReportLayoutDesigner_Form : Form
    {
        // Properties
        private static ReportLayoutDesigner_Form _instance;
        private int _initialFormWidth;
        private int _initialLeftPanelWidth;
        private int _initialRightPanelWidth;
        private bool _isDragging = false;
        private Point _dragStartPoint;
        private BaseElement _selectedElement;
        private bool _isResizing = false;
        private ResizeHandle _activeResizeHandle;
        private Rectangle _originalBounds;
        private Rectangle _lastElementBounds;
        private bool _deferPropertyUpdate = false;
        private Timer _propertyUpdateTimer;
        private readonly List<BaseElement> _selectedElements = [];
        private bool _isMultiSelecting = false;
        private Rectangle _selectionRectangle;
        private Point _selectionStartPoint;
        private BaseElement _currentPropertyElement = null;
        private string _currentTemplateName = null;

        /// <summary>
        /// Gets the current report configuration.
        /// </summary>
        private static ReportConfiguration? ReportConfig => ReportGenerator_Form.Instance.CurrentReportConfiguration;

        // Undo and redo properties
        public UndoRedoManager UndoHistoryDropdown { get; private set; }
        public UndoRedoManager RedoHistoryDropdown { get; private set; }

        // Resize handle enumeration
        private enum ResizeHandle
        {
            None,
            TopLeft, TopRight, BottomLeft, BottomRight,
            Top, Bottom, Left, Right
        }

        // Getters
        public static ReportLayoutDesigner_Form Instance => _instance;
        public static bool HasUnsavedChanges { get; set; } = false;

        // Init.
        public ReportLayoutDesigner_Form()
        {
            InitializeComponent();
            _instance = this;

            _undoRedoManager = new UndoRedoManager();
            _undoRedoManager.StateChanged += OnUndoRedoStateChanged;

            SetupCanvas();
            StoreInitialSizes();
            InitializeUndoRedoButtons();
            SetToolTips();
            UpdateLayoutButtonStates();
            OnPageSettingsChanged();
            InitializeResizeDebounceTimer();
            ScaleControls();
        }
        private void SetupCanvas()
        {
            // Enable double buffering for smooth rendering
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, Canvas_Panel, [true]);

            // Initialize property update timer
            _propertyUpdateTimer = new Timer
            {
                Interval = 100  // 100ms delay
            };
            _propertyUpdateTimer.Tick += (s, e) =>
            {
                _propertyUpdateTimer.Stop();
                if (!_deferPropertyUpdate)
                {
                    UpdatePropertyValues();
                }
            };
        }
        private void StoreInitialSizes()
        {
            _initialFormWidth = Width;
            _initialLeftPanelWidth = LeftTools_Panel.Width;
            _initialRightPanelWidth = RightCanvas_Panel.Width;
        }
        private void InitializeUndoRedoButtons()
        {
            // Create the dropdown panels
            UndoHistoryDropdown = new UndoRedoManager();
            RedoHistoryDropdown = new UndoRedoManager();

            // Subscribe to state changes to update button states
            _undoRedoManager.StateChanged += (s, e) =>
            {
                UpdateUndoRedoButtonStates();
            };

            // Set initial button states
            UpdateUndoRedoButtonStates();
        }
        private void SetToolTips()
        {
            CustomTooltip.SetToolTip(Undo_Button, "", "Undo (Ctrl+Z)");
            CustomTooltip.SetToolTip(UndoDropdown_Button, "", "Undo history");
            CustomTooltip.SetToolTip(Redo_Button, "", "Redo (Ctrl+Y)");
            CustomTooltip.SetToolTip(RedoDropdown_Button, "", "Redo history");

            CustomTooltip.SetToolTip(AlignLeft_Button, "", "Align left");
            CustomTooltip.SetToolTip(AlignCenter_Button, "", "Align center");
            CustomTooltip.SetToolTip(AlignRight_Button, "", "Align right");
            CustomTooltip.SetToolTip(AlignTop_Button, "", "Align top");
            CustomTooltip.SetToolTip(AlignMiddle_Button, "", "Align middle");
            CustomTooltip.SetToolTip(AlignBottom_Button, "", "Align bottom");
            CustomTooltip.SetToolTip(DistributeHorizontally_Button, "", "Distribute horizontally");
            CustomTooltip.SetToolTip(DistributeVertically_Button, "", "Distribute vertically");
            CustomTooltip.SetToolTip(MakeSameWidth_Button, "", "Make same width");
            CustomTooltip.SetToolTip(MakeSameHeight_Button, "", "Make same height");
            CustomTooltip.SetToolTip(MakeSameSize_Button, "", "Make same size");

            CustomTooltip.SetToolTip(SaveTemplate_Button, "", "Save template");
            CustomTooltip.SetToolTip(Settings_Button, "", "Settings");
        }
        private void ScaleControls()
        {
            DpiHelper.ScaleGroupBox(Elements_GroupBox);
            DpiHelper.ScaleGroupBox(Properties_GroupBox);
        }
        public void UpdateTheme()
        {
            ThemeManager.CustomizeScrollBar(_selectedElement?.CachedPropertyPanel);
            UpdatePropertyContainerTheme();
        }

        // Form event handlers
        private void ReportLayoutDesigner_Form_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                ClearAllSelections();

                if (ReportConfig == null)
                {
                    NotifyParentValidationChanged();
                    return;
                }

                ResizeCanvasToPageSize();
                NotifyParentValidationChanged();
            }
        }
        private void ReportLayoutDesigner_Form_Resize(object sender, EventArgs e)
        {
            if (_initialFormWidth == 0) { return; }

            // Calculate the form's width change ratio
            float widthRatio = (float)Width / _initialFormWidth;

            // Calculate new panel widths while maintaining proportion
            LeftTools_Panel.Width = (int)(_initialLeftPanelWidth * widthRatio);
            RightCanvas_Panel.Width = (int)(_initialRightPanelWidth * widthRatio);

            // Position the right panel
            RightCanvas_Panel.Left = LeftTools_Panel.Width;

            ResizeCanvasToPageSize();

            // Restart the debounce timer to update properties after resizing stops
            _resizeDebounceTimer?.Stop();
            _resizeDebounceTimer?.Start();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Don't intercept arrow keys if properties panel has focus
            if (ActiveControl is Guna2GroupBox)
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            // Undo/Redo shortcuts
            if (keyData == (Keys.Control | Keys.Z))
            {
                if (_undoRedoManager.CanUndo)
                {
                    _undoRedoManager.Undo();
                    RefreshCanvas();
                }
                return true;
            }

            if (keyData == (Keys.Control | Keys.Y) || keyData == (Keys.Control | Keys.Shift | Keys.Z))
            {
                if (_undoRedoManager.CanRedo)
                {
                    _undoRedoManager.Redo();
                    RefreshCanvas();
                }
                return true;
            }

            // Multi-selection shortcuts
            if (keyData == (Keys.Control | Keys.A))
            {
                SelectAllElements();
                return true;
            }

            // Duplicate shortcut
            if (keyData == (Keys.Control | Keys.D))
            {
                DuplicateSelected();
                return true;
            }

            // Delete shortcut
            if (keyData == Keys.Delete)
            {
                DeleteSelected();
                return true;
            }

            // Arrow key movement shortcuts (1 pixel)
            if (keyData == Keys.Left)
            {
                MoveSelectedElements(-1, 0);
                return true;
            }

            if (keyData == Keys.Right)
            {
                MoveSelectedElements(1, 0);
                return true;
            }

            if (keyData == Keys.Up)
            {
                MoveSelectedElements(0, -1);
                return true;
            }

            if (keyData == Keys.Down)
            {
                MoveSelectedElements(0, 1);
                return true;
            }

            // Shift+Arrow or Ctrl+Arrow for larger movements (10 pixels)
            if (keyData == (Keys.Shift | Keys.Left) || keyData == (Keys.Control | Keys.Left))
            {
                MoveSelectedElements(-10, 0);
                return true;
            }
            if (keyData == (Keys.Shift | Keys.Right) || keyData == (Keys.Control | Keys.Right))
            {
                MoveSelectedElements(10, 0);
                return true;
            }
            if (keyData == (Keys.Shift | Keys.Up) || keyData == (Keys.Control | Keys.Up))
            {
                MoveSelectedElements(0, -10);
                return true;
            }
            if (keyData == (Keys.Shift | Keys.Down) || keyData == (Keys.Control | Keys.Down))
            {
                MoveSelectedElements(0, 10);
                return true;
            }

            // Alignment shortcuts
            if (keyData == (Keys.Control | Keys.Left))
            {
                AlignSelectedLeft();
                return true;
            }

            if (keyData == (Keys.Control | Keys.Right))
            {
                AlignSelectedRight();
                return true;
            }

            if (keyData == (Keys.Control | Keys.Up))
            {
                AlignSelectedTop();
                return true;
            }

            if (keyData == (Keys.Control | Keys.Down))
            {
                AlignSelectedBottom();
                return true;
            }

            // Grid toggle
            if (keyData == (Keys.Control | Keys.G))
            {
                ShowGrid = !ShowGrid;
                OnGridSettingsChanged();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // Resize debounce timer
        private Timer _resizeDebounceTimer;
        private void InitializeResizeDebounceTimer()
        {
            _resizeDebounceTimer = new Timer
            {
                Interval = 150
            };
            _resizeDebounceTimer.Tick += (s, e) =>
            {
                _resizeDebounceTimer.Stop();
                UpdatePropertiesForSelection();
            };
        }
        private Point ScaledToPageCoordinates(Point scaledPoint)
        {
            if (_canvasScaleFactor <= 0) { return scaledPoint; }
            return new Point(
                (int)(scaledPoint.X / _canvasScaleFactor),
                (int)(scaledPoint.Y / _canvasScaleFactor)
            );
        }

        // Undo/Redo integration
        private readonly UndoRedoManager _undoRedoManager;
        private List<Rectangle> _dragStartBounds;
        public UndoRedoManager GetUndoRedoManager()
        {
            return _undoRedoManager;

        }

        // Canvas event handlers
        private const int DRAG_THRESHOLD = 5;
        private float _canvasScaleFactor = 1.0f;
        private BaseElement _clickedElement;
        private void Canvas_Panel_Paint(object sender, PaintEventArgs e)
        {
            if (ReportConfig == null) { return; }

            Graphics graphics = e.Graphics;
            graphics.SetClip(e.ClipRectangle);

            // Get the actual page size (not the scaled canvas size)
            Size pageSize = PageDimensions.GetDimensions(ReportConfig.PageSize, ReportConfig.PageOrientation);

            graphics.ScaleTransform(_canvasScaleFactor, _canvasScaleFactor);

            ReportRenderer.RenderReport(graphics, pageSize, ReportConfig, ShowGrid);

            DrawSelectionForElements(graphics);
        }
        private void Canvas_Panel_MouseDown(object sender, MouseEventArgs e)
        {
            Point pageLocation = ScaledToPageCoordinates(e.Location);
            Canvas_Panel.Focus();

            if (e.Button == MouseButtons.Left)
            {
                bool ctrlPressed = (ModifierKeys & Keys.Control) == Keys.Control;

                // Check for resize handles on any selected element
                if (_selectedElements.Count > 0)
                {
                    foreach (BaseElement element in _selectedElements)
                    {
                        ResizeHandle handle = GetResizeHandleAtPoint(pageLocation, element);
                        if (handle != ResizeHandle.None)
                        {
                            _isResizing = true;
                            _activeResizeHandle = handle;
                            _selectedElement = element;
                            _originalBounds = element.Bounds;
                            _lastElementBounds = element.Bounds;
                            _dragStartPoint = pageLocation;
                            _deferPropertyUpdate = true;
                            return;
                        }
                    }
                }
                // Check single selected element
                else if (_selectedElement != null)
                {
                    ResizeHandle handle = GetResizeHandleAtPoint(pageLocation, _selectedElement);
                    if (handle != ResizeHandle.None)
                    {
                        _isResizing = true;
                        _activeResizeHandle = handle;
                        _originalBounds = _selectedElement.Bounds;
                        _lastElementBounds = _selectedElement.Bounds;
                        _dragStartPoint = pageLocation;
                        _deferPropertyUpdate = true;
                        return;
                    }
                }

                BaseElement clickedElement = GetElementAtPoint(pageLocation);

                if (clickedElement != null)
                {
                    if (ctrlPressed)
                    {
                        if (_selectedElements.Remove(clickedElement))
                        {
                            // Remove succeeded, so it was in the list
                            if (_selectedElement == clickedElement)
                            {
                                _selectedElement = _selectedElements.FirstOrDefault();
                            }

                            Canvas_Panel.Invalidate();
                            UpdatePropertiesForSelection();
                            UpdateLayoutButtonStates();
                        }
                        else
                        {
                            // Remove failed, so it wasn't in the list - add it
                            SelectElement(clickedElement, true);
                        }
                    }
                    else
                    {
                        // Single selection or start drag
                        if (!_selectedElements.Contains(clickedElement))
                        {
                            SelectElement(clickedElement);
                        }

                        _clickedElement = clickedElement;  // Store for potential selection change on MouseUp
                        _isDragging = true;
                        _dragStartPoint = pageLocation;

                        // Store original bounds for undo
                        if (_selectedElements.Count > 0)
                        {
                            _dragStartBounds = _selectedElements.Select(el => el.Bounds).ToList();
                        }
                        else if (_selectedElement != null)
                        {
                            _dragStartBounds = [_selectedElement.Bounds];
                        }
                    }
                }
                else
                {
                    _selectionStartPoint = pageLocation;
                }
            }
        }
        private void Canvas_Panel_MouseMove(object sender, MouseEventArgs e)
        {
            Point pageLocation = ScaledToPageCoordinates(e.Location);

            // Handle cursor changes for resize handles when not dragging
            if (!_isResizing && !_isDragging && !_isMultiSelecting)
            {
                Cursor newCursor = Cursors.Default;

                // Check resize handles on selected elements
                if (_selectedElements.Count > 0)
                {
                    foreach (BaseElement element in _selectedElements)
                    {
                        ResizeHandle handle = GetResizeHandleAtPoint(pageLocation, element);
                        if (handle != ResizeHandle.None)
                        {
                            newCursor = GetCursorForHandle(handle);
                            break;
                        }
                    }
                }
                else if (_selectedElement != null)
                {
                    ResizeHandle handle = GetResizeHandleAtPoint(pageLocation, _selectedElement);
                    newCursor = GetCursorForHandle(handle);
                }

                // If not over a resize handle, check if over an element
                if (newCursor == Cursors.Default)
                {
                    BaseElement elementAtPoint = GetElementAtPoint(pageLocation);
                    if (elementAtPoint != null)
                    {
                        newCursor = Cursors.Hand;
                    }
                }

                if (Canvas_Panel.Cursor != newCursor)
                {
                    Canvas_Panel.Cursor = newCursor;
                }

                // Check if we should start rectangle selection from mouse down on empty area
                if (e.Button == MouseButtons.Left && !_isDragging && !_isMultiSelecting &&
                    _selectionStartPoint != Point.Empty && GetElementAtPoint(_selectionStartPoint) == null)
                {
                    int deltaX = Math.Abs(pageLocation.X - _selectionStartPoint.X);
                    int deltaY = Math.Abs(pageLocation.Y - _selectionStartPoint.Y);

                    if (deltaX > DRAG_THRESHOLD || deltaY > DRAG_THRESHOLD)
                    {
                        bool ctrlPressed = (ModifierKeys & Keys.Control) == Keys.Control;
                        if (!ctrlPressed)
                        {
                            ClearAllSelections();
                        }

                        _isMultiSelecting = true;
                        _selectionRectangle = new Rectangle(_selectionStartPoint, Size.Empty);
                    }
                }
            }

            // Handle multi-selection rectangle
            if (_isMultiSelecting)
            {
                int x = Math.Min(_selectionStartPoint.X, pageLocation.X);
                int y = Math.Min(_selectionStartPoint.Y, pageLocation.Y);
                int width = Math.Abs(pageLocation.X - _selectionStartPoint.X);
                int height = Math.Abs(pageLocation.Y - _selectionStartPoint.Y);

                Rectangle oldRect = _selectionRectangle;
                _selectionRectangle = new Rectangle(x, y, width, height);

                // Invalidate the union of old and new rectangles to avoid artifacts
                if (oldRect.Width > 0 || oldRect.Height > 0)
                {
                    Rectangle scaledOldRect = PageToScaledRectangle(oldRect);
                    Rectangle scaledNewRect = PageToScaledRectangle(_selectionRectangle);

                    // Create union of both rectangles to cover all areas
                    Rectangle invalidateRegion = Rectangle.Union(scaledOldRect, scaledNewRect);
                    invalidateRegion.Inflate(5, 5);  // Increased inflation for pen width and anti-aliasing

                    Canvas_Panel.Invalidate(invalidateRegion);
                }
                else
                {
                    // First time drawing the rectangle
                    Rectangle scaledNewRect = PageToScaledRectangle(_selectionRectangle);
                    scaledNewRect.Inflate(5, 5);
                    Canvas_Panel.Invalidate(scaledNewRect);
                }

                UpdateSelectionFromRectangle();
            }

            // Handle resizing
            else if (_isResizing && _selectedElement != null)
            {
                ResizeElement(pageLocation);
            }
            // Handle dragging multiple elements
            else if (_isDragging && _selectedElements.Count > 0)
            {
                int deltaX = pageLocation.X - _dragStartPoint.X;
                int deltaY = pageLocation.Y - _dragStartPoint.Y;

                // Calculate the actual movement possible for each axis
                int actualDeltaX = deltaX;
                int actualDeltaY = deltaY;

                // Find the most restrictive limits
                foreach (BaseElement element in _selectedElements)
                {
                    Rectangle testBounds = element.Bounds;
                    Size pageSize = PageDimensions.GetDimensions(ReportConfig.PageSize, ReportConfig.PageOrientation);

                    // Check X axis limits
                    int newX = testBounds.X + deltaX;
                    if (newX < 0)
                    {
                        actualDeltaX = Math.Max(actualDeltaX, -testBounds.X);
                    }
                    else if (newX + testBounds.Width > pageSize.Width)
                    {
                        actualDeltaX = Math.Min(actualDeltaX, pageSize.Width - testBounds.Width - testBounds.X);
                    }

                    // Check Y axis limits
                    int newY = testBounds.Y + deltaY;
                    if (newY < 0)
                    {
                        actualDeltaY = Math.Max(actualDeltaY, -testBounds.Y);
                    }
                    else if (newY + testBounds.Height > pageSize.Height)
                    {
                        actualDeltaY = Math.Min(actualDeltaY, pageSize.Height - testBounds.Height - testBounds.Y);
                    }
                }

                // Apply movement if there's any valid delta
                if (actualDeltaX != 0 || actualDeltaY != 0)
                {
                    // For grid snapping with multiple elements, snap the first element
                    // and use its actual movement for all others
                    if (ShowGrid && _selectedElements.Count > 0)
                    {
                        BaseElement firstElement = _selectedElements[0];
                        Rectangle firstOldBounds = firstElement.Bounds;
                        Rectangle firstNewBounds = firstOldBounds;
                        firstNewBounds.X += actualDeltaX;
                        firstNewBounds.Y += actualDeltaY;

                        firstNewBounds = SnapToGrid(firstNewBounds);

                        // Recalculate actual deltas based on snapped position
                        actualDeltaX = firstNewBounds.X - firstOldBounds.X;
                        actualDeltaY = firstNewBounds.Y - firstOldBounds.Y;
                    }

                    foreach (BaseElement element in _selectedElements)
                    {
                        Rectangle oldBounds = element.Bounds;
                        Rectangle newBounds = element.Bounds;

                        newBounds.X += actualDeltaX;
                        newBounds.Y += actualDeltaY;

                        element.Bounds = newBounds;

                        InvalidateElementRegion(oldBounds);
                        InvalidateElementRegion(newBounds);
                    }

                    // Update drag point based on actual movement
                    _dragStartPoint.X += actualDeltaX;
                    _dragStartPoint.Y += actualDeltaY;

                    // Defer property update
                    _propertyUpdateTimer.Stop();
                    _propertyUpdateTimer.Start();
                }
            }
            // Handle dragging single element
            else if (_isDragging && _selectedElement != null)
            {
                int deltaX = pageLocation.X - _dragStartPoint.X;
                int deltaY = pageLocation.Y - _dragStartPoint.Y;

                Rectangle oldBounds = _selectedElement.Bounds;
                Rectangle newBounds = _selectedElement.Bounds;

                Size pageSize = PageDimensions.GetDimensions(ReportConfig.PageSize, ReportConfig.PageOrientation);

                // Calculate clamped position for X axis
                int newX = oldBounds.X + deltaX;
                newX = Math.Max(0, Math.Min(newX, pageSize.Width - oldBounds.Width));
                int actualDeltaX = newX - oldBounds.X;

                // Calculate clamped position for Y axis
                int newY = oldBounds.Y + deltaY;
                newY = Math.Max(0, Math.Min(newY, pageSize.Height - oldBounds.Height));
                int actualDeltaY = newY - oldBounds.Y;

                if (actualDeltaX != 0 || actualDeltaY != 0)
                {
                    newBounds.X = newX;
                    newBounds.Y = newY;

                    // Apply grid snapping if enabled
                    if (ShowGrid)
                    {
                        newBounds = SnapToGrid(newBounds);
                        actualDeltaX = newBounds.X - oldBounds.X;
                        actualDeltaY = newBounds.Y - oldBounds.Y;
                    }

                    _selectedElement.Bounds = newBounds;

                    InvalidateElementRegion(oldBounds);
                    InvalidateElementRegion(newBounds);

                    // Update drag point based on actual movement
                    _dragStartPoint.X += actualDeltaX;
                    _dragStartPoint.Y += actualDeltaY;

                    // Defer property panel update
                    _propertyUpdateTimer.Stop();
                    _propertyUpdateTimer.Start();
                }
            }
        }
        private void Canvas_Panel_MouseUp(object sender, MouseEventArgs e)
        {
            bool wasInteracting = _isDragging || _isResizing || _isMultiSelecting;
            bool actuallyDragged = false;

            if (_isDragging && _dragStartBounds != null)
            {
                if (_selectedElements.Count > 0)
                {
                    List<Rectangle> newBounds = _selectedElements.Select(el => el.Bounds).ToList();
                    for (int i = 0; i < _dragStartBounds.Count && i < newBounds.Count; i++)
                    {
                        if (_dragStartBounds[i] != newBounds[i])
                        {
                            actuallyDragged = true;
                            break;
                        }
                    }
                }
                else if (_selectedElement != null && _dragStartBounds.Count > 0)
                {
                    actuallyDragged = _dragStartBounds[0] != _selectedElement.Bounds;
                }
            }

            // Record undo actions for drag
            if (_isDragging && _dragStartBounds != null)
            {
                if (_selectedElements.Count > 0)
                {
                    List<Rectangle> newBounds = _selectedElements.Select(el => el.Bounds).ToList();
                    bool moved = false;

                    for (int i = 0; i < _dragStartBounds.Count && i < newBounds.Count; i++)
                    {
                        if (_dragStartBounds[i] != newBounds[i])
                        {
                            moved = true;
                            break;
                        }
                    }

                    if (moved)
                    {
                        _undoRedoManager.RecordAction(new MoveElementsAction(
                            _selectedElements.ToList(),
                            _dragStartBounds,
                            newBounds,
                            RefreshCanvas));

                        MarkChartLayoutAsManualIfNeeded();
                        MarkAsChanged();
                    }
                }
                else if (_selectedElement != null && _dragStartBounds.Count > 0)
                {
                    if (_dragStartBounds[0] != _selectedElement.Bounds)
                    {
                        _undoRedoManager.RecordAction(new MoveElementsAction(
                            [_selectedElement],
                            _dragStartBounds,
                            [_selectedElement.Bounds],
                            RefreshCanvas));

                        MarkChartLayoutAsManualIfNeeded();
                        MarkAsChanged();
                    }
                }
            }

            // Record undo action for resize
            if (_isResizing && _selectedElement != null)
            {
                if (_originalBounds != _selectedElement.Bounds)
                {
                    _undoRedoManager.RecordAction(new ResizeElementAction(
                        _selectedElement,
                        _originalBounds,
                        _selectedElement.Bounds,
                        RefreshCanvas));

                    MarkChartLayoutAsManualIfNeeded();
                    MarkAsChanged();
                }
            }

            // If we clicked (not dragged) on an already-selected element with multiple selections, change to single selection
            if (!actuallyDragged && _clickedElement != null && _selectedElements.Count > 1
                && _selectedElements.Contains(_clickedElement))
            {
                SelectElement(_clickedElement);
            }
            _clickedElement = null;

            // If mouse up on empty area without dragging, clear selections
            if (e.Button == MouseButtons.Left && _selectionStartPoint != Point.Empty && !_isMultiSelecting)
            {
                bool ctrlPressed = (ModifierKeys & Keys.Control) == Keys.Control;
                if (!ctrlPressed)
                {
                    ClearAllSelections();
                }
            }

            _isDragging = false;
            _isResizing = false;
            _isMultiSelecting = false;
            _activeResizeHandle = ResizeHandle.None;
            _selectionStartPoint = Point.Empty;
            _dragStartBounds = null;
            Canvas_Panel.Cursor = Cursors.Default;
            _deferPropertyUpdate = false;

            // Clear selection rectangle
            if (_selectionRectangle.Width > 0 || _selectionRectangle.Height > 0)
            {
                Rectangle clearRect = _selectionRectangle;
                clearRect.Inflate(2, 2);
                Canvas_Panel.Invalidate(clearRect);
                _selectionRectangle = Rectangle.Empty;
            }

            // Force full invalidation after multi-select to show resize handles for single selection
            if (wasInteracting)
            {
                Canvas_Panel.Invalidate();
            }

            if (e.Button == MouseButtons.Right)
            {
                // Check if there's an element at the click location
                Point pageLocation = ScaledToPageCoordinates(e.Location);
                BaseElement clickedElement = GetElementAtPoint(pageLocation);

                if (clickedElement != null)
                {
                    // If the clicked element is not already selected, select it
                    if (!_selectedElements.Contains(clickedElement) && _selectedElement != clickedElement)
                    {
                        bool ctrlPressed = (ModifierKeys & Keys.Control) == Keys.Control;
                        SelectElement(clickedElement, ctrlPressed);
                    }
                }
                else
                {
                    // Right-clicked on empty area - clear selection unless Ctrl is pressed
                    bool ctrlPressed = (ModifierKeys & Keys.Control) == Keys.Control;
                    if (!ctrlPressed)
                    {
                        ClearAllSelections();
                    }
                }

                bool hasSelection = _selectedElements.Count > 0 || _selectedElement != null;
                Point formLocation = PointToClient(Canvas_Panel.PointToScreen(e.Location));

                RightClickElementMenu.Show(formLocation, this, hasSelection);
            }
        }

        // Add element event handlers
        private void AddChartElement_Button_Click(object sender, EventArgs e)
        {
            CreateElementAtLocation(ReportElementType.Chart, new Point(50, 50));
        }
        private void AddLabelElement_Button_Click(object sender, EventArgs e)
        {
            CreateElementAtLocation(ReportElementType.Label, new Point(50, 220));
        }
        private void AddDateElement_Button_Click(object sender, EventArgs e)
        {
            CreateElementAtLocation(ReportElementType.DateRange, new Point(50, 280));
        }
        private void AddSummaryElement_Button_Click(object sender, EventArgs e)
        {
            CreateElementAtLocation(ReportElementType.Summary, new Point(50, 320));
        }
        private void AddTableElement_Button_Click(object sender, EventArgs e)
        {
            CreateElementAtLocation(ReportElementType.TransactionTable, new Point(50, 450));
        }
        private void AddImageElement_Button_Click(object sender, EventArgs e)
        {
            CreateElementAtLocation(ReportElementType.Image, new Point(50, 360));
        }

        // Settings button
        private void Settings_Button_Click(object sender, EventArgs e)
        {
            using PageSettings_Form pageSettings_Form = new();
            pageSettings_Form.ShowDialog(this);
        }
        public void OnPageSettingsChanged()
        {
            ResizeCanvasToPageSize();
            NotifyParentValidationChanged();
            Canvas_Panel.Invalidate();
        }
        public void OnGridSettingsChanged()
        {
            Canvas_Panel.Invalidate();
        }

        /// <summary>
        /// Resizes the canvas to match the current page dimensions, maximizing size while fitting in the panel.
        /// Always centers the canvas in the available space.
        /// </summary>
        private void ResizeCanvasToPageSize()
        {
            if (ReportConfig == null) { return; }

            Size pageSize = PageDimensions.GetDimensions(ReportConfig.PageSize, ReportConfig.PageOrientation);
            int toolbarHeight = 62;
            int padding = 10;

            // Calculate available space in the panel
            int availableWidth = RightCanvas_Panel.ClientSize.Width - (padding * 2);
            int availableHeight = RightCanvas_Panel.ClientSize.Height - toolbarHeight - (padding * 2);

            // Calculate scale factors for width and height
            float scaleWidth = (float)availableWidth / pageSize.Width;
            float scaleHeight = (float)availableHeight / pageSize.Height;

            // Use the smaller scale factor to ensure canvas fits in both dimensions
            float scaleFactor = Math.Min(scaleWidth, scaleHeight);

            _canvasScaleFactor = scaleFactor;

            // Calculate final canvas size
            int canvasWidth = (int)(pageSize.Width * scaleFactor);
            int canvasHeight = (int)(pageSize.Height * scaleFactor);

            // Update canvas size
            Canvas_Panel.Size = new Size(canvasWidth, canvasHeight);

            // Center horizontally in available width
            int centerX = (RightCanvas_Panel.ClientSize.Width - canvasWidth) / 2;

            // Center vertically in available height
            int availableVerticalSpace = RightCanvas_Panel.ClientSize.Height - toolbarHeight;
            int centerY = toolbarHeight + ((availableVerticalSpace - canvasHeight) / 2);

            // Set the centered location
            Canvas_Panel.Location = new Point(centerX, centerY);
        }

        // Undo and redo button event handlers
        private void Undo_Button_Click(object sender, EventArgs e)
        {
            if (_undoRedoManager.CanUndo)
            {
                _undoRedoManager.Undo();
                Canvas_Panel.Invalidate();
                UpdatePropertyValues();
            }
        }
        private void Redo_Button_Click(object sender, EventArgs e)
        {
            if (_undoRedoManager.CanRedo)
            {
                _undoRedoManager.Redo();
                Canvas_Panel.Invalidate();
                UpdatePropertyValues();
            }
        }
        private void UndoDropdown_Button_Click(object sender, EventArgs e)
        {
            if (_undoRedoManager.UndoCount > 0)
            {
                // Toggle undo dropdown
                if (Controls.Contains(UndoRedoHistoryDropdown.Panel))
                {
                    UndoRedoHistoryDropdown.Remove();
                }
                else
                {
                    Point dropdownLocation = new(
                        RightCanvas_Panel.Left + Undo_Button.Left,
                        Undo_Button.Bottom + 2
                    );

                    UndoRedoHistoryDropdown.Show(this, dropdownLocation, _undoRedoManager, true, OnHistoryActionPerformed);
                }
            }
        }
        private void RedoDropdown_Button_Click(object sender, EventArgs e)
        {
            if (_undoRedoManager.RedoCount > 0)
            {
                // Toggle redo dropdown
                if (Controls.Contains(UndoRedoHistoryDropdown.Panel))
                {
                    UndoRedoHistoryDropdown.Remove();
                }
                else
                {
                    Point dropdownLocation = new(
                        RightCanvas_Panel.Left + Redo_Button.Left,
                        Redo_Button.Bottom + 2
                    );

                    UndoRedoHistoryDropdown.Show(this, dropdownLocation, _undoRedoManager, false, OnHistoryActionPerformed);
                }
            }
        }

        // Update undo and redo button states
        private void UpdateUndoRedoButtonStates()
        {
            // Update Undo buttons
            bool canUndo = _undoRedoManager.CanUndo;
            Undo_Button.Enabled = canUndo;
            UndoDropdown_Button.Enabled = canUndo;

            if (canUndo)
            {
                CustomTooltip.SetToolTip(Undo_Button, "", $"Undo: {_undoRedoManager.UndoDescription} (Ctrl+Z)");
            }
            else
            {
                CustomTooltip.SetToolTip(Undo_Button, "", "Undo (Ctrl+Z)");
            }

            // Update Redo buttons
            bool canRedo = _undoRedoManager.CanRedo;
            Redo_Button.Enabled = canRedo;
            RedoDropdown_Button.Enabled = canRedo;

            if (canRedo)
            {
                CustomTooltip.SetToolTip(Redo_Button, "", $"Redo: {_undoRedoManager.RedoDescription} (Ctrl+Y)");
            }
            else
            {
                CustomTooltip.SetToolTip(Redo_Button, "", "Redo (Ctrl+Y)");
            }
        }
        private void OnHistoryActionPerformed()
        {
            Canvas_Panel.Invalidate();
            UpdatePropertyValues();
        }

        // Move element method
        /// <summary>
        /// Moves all selected elements by the specified delta.
        /// </summary>
        private void MoveSelectedElementsHelper(int deltaX, int deltaY)
        {
            if (_selectedElements.Count == 0 && _selectedElement == null)
            {
                return;
            }

            Size pageSize = PageDimensions.GetDimensions(ReportConfig.PageSize, ReportConfig.PageOrientation);

            // Move all selected elements
            if (_selectedElements.Count > 0)
            {
                foreach (BaseElement element in _selectedElements)
                {
                    Rectangle oldBounds = element.Bounds;
                    Rectangle newBounds = element.Bounds;

                    newBounds.X += deltaX;
                    newBounds.Y += deltaY;

                    // Clamp to canvas bounds
                    newBounds.X = Math.Max(0, Math.Min(newBounds.X, pageSize.Width - newBounds.Width));
                    newBounds.Y = Math.Max(0, Math.Min(newBounds.Y, pageSize.Height - newBounds.Height));

                    element.Bounds = newBounds;

                    InvalidateElementRegion(oldBounds);
                    InvalidateElementRegion(newBounds);
                }
            }
            else if (_selectedElement != null)
            {
                Rectangle oldBounds = _selectedElement.Bounds;
                Rectangle newBounds = _selectedElement.Bounds;

                newBounds.X += deltaX;
                newBounds.Y += deltaY;

                // Clamp to canvas bounds
                newBounds.X = Math.Max(0, Math.Min(newBounds.X, pageSize.Width - newBounds.Width));
                newBounds.Y = Math.Max(0, Math.Min(newBounds.Y, pageSize.Height - newBounds.Height));

                _selectedElement.Bounds = newBounds;

                InvalidateElementRegion(oldBounds);
                InvalidateElementRegion(newBounds);
            }

            UpdatePropertyValues();
            NotifyParentValidationChanged();
        }
        private void MoveSelectedElements(int deltaX, int deltaY)
        {
            if (_selectedElements.Count > 0)
            {
                List<Rectangle> oldBounds = _selectedElements.Select(e => e.Bounds).ToList();
                MoveSelectedElementsHelper(deltaX, deltaY);
                List<Rectangle> newBounds = _selectedElements.Select(e => e.Bounds).ToList();

                _undoRedoManager.RecordAction(new MoveElementsAction(
                    _selectedElements.ToList(),
                    oldBounds,
                    newBounds,
                    RefreshCanvas));

                MarkChartLayoutAsManualIfNeeded();
            }
            else if (_selectedElement != null)
            {
                Rectangle oldBounds = _selectedElement.Bounds;
                MoveSelectedElementsHelper(deltaX, deltaY);

                _undoRedoManager.RecordAction(new MoveElementsAction(
                    [_selectedElement],
                    [oldBounds],
                    [_selectedElement.Bounds],
                    RefreshCanvas));

                MarkChartLayoutAsManualIfNeeded();
            }
        }

        // Alignment tool event handlers
        private void AlignLeft_Button_Click(object sender, EventArgs e) => AlignSelectedLeft();
        private void AlignCenter_Button_Click(object sender, EventArgs e) => AlignSelectedCenter();
        private void AlignRight_Button_Click(object sender, EventArgs e) => AlignSelectedRight();
        private void AlignTop_Button_Click(object sender, EventArgs e) => AlignSelectedTop();
        private void AlignMiddle_Button_Click(object sender, EventArgs e) => AlignSelectedMiddle();
        private void AlignBottom_Button_Click(object sender, EventArgs e) => AlignSelectedBottom();
        private void DistributeHorizontally_Button_Click(object sender, EventArgs e) => DistributeHorizontally();
        private void DistributeVertically_Button_Click(object sender, EventArgs e) => DistributeVertically();
        private void MakeSameWidth_Button_Click(object sender, EventArgs e) => MakeSameWidth();
        private void MakeSameHeight_Button_Click(object sender, EventArgs e) => MakeSameHeight();
        private void MakeSameSize_Button_Click(object sender, EventArgs e) => MakeSameSize();

        // Alignment tool methods
        private void AlignSelectedLeft()
        {
            if (_selectedElements.Count < 2) { return; }

            List<Rectangle> oldBounds = _selectedElements.Select(e => e.Bounds).ToList();
            int leftMost = _selectedElements.Min(e => e.Bounds.X);

            foreach (BaseElement element in _selectedElements)
            {
                Rectangle bounds = element.Bounds;
                bounds.X = leftMost;
                element.Bounds = bounds;
            }

            _undoRedoManager.RecordAction(new AlignmentAction(
                _selectedElements.ToList(),
                oldBounds,
                "left",
                RefreshCanvas));

            MarkChartLayoutAsManualIfNeeded();
            Canvas_Panel.Invalidate();
            OnPropertyChanged();
        }
        private void AlignSelectedCenter()
        {
            if (_selectedElements.Count < 2) { return; }

            List<Rectangle> oldBounds = _selectedElements.Select(e => e.Bounds).ToList();

            // Find the center of all selected elements
            int leftMost = _selectedElements.Min(e => e.Bounds.X);
            int rightMost = _selectedElements.Max(e => e.Bounds.Right);
            int centerX = (leftMost + rightMost) / 2;

            foreach (BaseElement element in _selectedElements)
            {
                Rectangle bounds = element.Bounds;
                bounds.X = centerX - bounds.Width / 2;
                element.Bounds = bounds;
            }

            _undoRedoManager.RecordAction(new AlignmentAction(
                _selectedElements.ToList(),
                oldBounds,
                "center",
                RefreshCanvas));

            MarkChartLayoutAsManualIfNeeded();
            Canvas_Panel.Invalidate();
            OnPropertyChanged();
        }
        private void AlignSelectedRight()
        {
            if (_selectedElements.Count < 2) { return; }

            List<Rectangle> oldBounds = _selectedElements.Select(e => e.Bounds).ToList();
            int rightMost = _selectedElements.Max(e => e.Bounds.Right);

            foreach (BaseElement element in _selectedElements)
            {
                Rectangle bounds = element.Bounds;
                bounds.X = rightMost - bounds.Width;
                element.Bounds = bounds;
            }

            _undoRedoManager.RecordAction(new AlignmentAction(
                _selectedElements.ToList(),
                oldBounds,
                "right",
                RefreshCanvas));

            MarkChartLayoutAsManualIfNeeded();
            Canvas_Panel.Invalidate();
            OnPropertyChanged();
        }
        private void AlignSelectedTop()
        {
            if (_selectedElements.Count < 2) { return; }

            List<Rectangle> oldBounds = _selectedElements.Select(e => e.Bounds).ToList();
            int topMost = _selectedElements.Min(e => e.Bounds.Y);

            foreach (BaseElement element in _selectedElements)
            {
                Rectangle bounds = element.Bounds;
                bounds.Y = topMost;
                element.Bounds = bounds;
            }

            _undoRedoManager.RecordAction(new AlignmentAction(
                _selectedElements.ToList(),
                oldBounds,
                "top",
                RefreshCanvas));

            MarkChartLayoutAsManualIfNeeded();
            Canvas_Panel.Invalidate();
            OnPropertyChanged();
        }
        private void AlignSelectedMiddle()
        {
            if (_selectedElements.Count < 2) { return; }

            List<Rectangle> oldBounds = _selectedElements.Select(e => e.Bounds).ToList();

            // Find the middle of all selected elements
            int topMost = _selectedElements.Min(e => e.Bounds.Y);
            int bottomMost = _selectedElements.Max(e => e.Bounds.Bottom);
            int centerY = (topMost + bottomMost) / 2;

            foreach (BaseElement element in _selectedElements)
            {
                Rectangle bounds = element.Bounds;
                bounds.Y = centerY - bounds.Height / 2;
                element.Bounds = bounds;
            }

            _undoRedoManager.RecordAction(new AlignmentAction(
                _selectedElements.ToList(),
                oldBounds,
                "middle",
                RefreshCanvas));

            MarkChartLayoutAsManualIfNeeded();
            Canvas_Panel.Invalidate();
            OnPropertyChanged();
        }
        private void AlignSelectedBottom()
        {
            if (_selectedElements.Count < 2) { return; }

            List<Rectangle> oldBounds = _selectedElements.Select(e => e.Bounds).ToList();
            int bottomMost = _selectedElements.Max(e => e.Bounds.Bottom);

            foreach (BaseElement element in _selectedElements)
            {
                Rectangle bounds = element.Bounds;
                bounds.Y = bottomMost - bounds.Height;
                element.Bounds = bounds;
            }

            _undoRedoManager.RecordAction(new AlignmentAction(
                _selectedElements.ToList(),
                oldBounds,
                "bottom",
                RefreshCanvas));

            MarkChartLayoutAsManualIfNeeded();
            Canvas_Panel.Invalidate();
            OnPropertyChanged();
        }
        private void DistributeHorizontally()
        {
            if (_selectedElements.Count < 3) { return; }

            List<Rectangle> oldBounds = _selectedElements.Select(e => e.Bounds).ToList();

            // Sort elements by X position
            List<BaseElement> sortedElements = _selectedElements.OrderBy(e => e.Bounds.X).ToList();

            int leftMost = sortedElements.First().Bounds.X;
            int rightMost = sortedElements.Last().Bounds.Right;
            int totalWidth = sortedElements.Sum(e => e.Bounds.Width);
            int totalSpace = rightMost - leftMost - totalWidth;

            if (sortedElements.Count > 1)
            {
                float spacing = (float)totalSpace / (sortedElements.Count - 1);
                float currentX = leftMost;

                foreach (BaseElement element in sortedElements)
                {
                    Rectangle bounds = element.Bounds;
                    bounds.X = (int)currentX;
                    element.Bounds = bounds;
                    currentX += bounds.Width + spacing;
                }
            }

            _undoRedoManager.RecordAction(new DistributeAction(
                _selectedElements.ToList(),
                oldBounds,
                "horizontally",
                RefreshCanvas));

            MarkChartLayoutAsManualIfNeeded();
            Canvas_Panel.Invalidate();
            OnPropertyChanged();
        }
        private void DistributeVertically()
        {
            if (_selectedElements.Count < 3) { return; }

            List<Rectangle> oldBounds = _selectedElements.Select(e => e.Bounds).ToList();

            // Sort elements by Y position
            List<BaseElement> sortedElements = _selectedElements.OrderBy(e => e.Bounds.Y).ToList();

            int topMost = sortedElements.First().Bounds.Y;
            int bottomMost = sortedElements.Last().Bounds.Bottom;
            int totalHeight = sortedElements.Sum(e => e.Bounds.Height);
            int totalSpace = bottomMost - topMost - totalHeight;

            if (sortedElements.Count > 1)
            {
                float spacing = (float)totalSpace / (sortedElements.Count - 1);
                float currentY = topMost;

                foreach (BaseElement element in sortedElements)
                {
                    Rectangle bounds = element.Bounds;
                    bounds.Y = (int)currentY;
                    element.Bounds = bounds;
                    currentY += bounds.Height + spacing;
                }
            }

            _undoRedoManager.RecordAction(new DistributeAction(
                _selectedElements.ToList(),
                oldBounds,
                "vertically",
                RefreshCanvas));

            MarkChartLayoutAsManualIfNeeded();
            Canvas_Panel.Invalidate();
            OnPropertyChanged();
        }

        // Sizing tool methods
        private void MakeSameWidth()
        {
            if (_selectedElements.Count < 2) { return; }

            // Use the first selected element's width as the reference
            int referenceWidth = _selectedElements.First().Bounds.Width;

            foreach (BaseElement element in _selectedElements)
            {
                Rectangle bounds = element.Bounds;
                bounds.Width = referenceWidth;
                element.Bounds = bounds;
            }

            SameSizeAction action = new(_selectedElements.ToList(), "width", RefreshCanvas);
            _undoRedoManager.RecordAction(action);

            MarkChartLayoutAsManualIfNeeded();
            Canvas_Panel.Invalidate();
            OnPropertyChanged();
        }
        private void MakeSameHeight()
        {
            if (_selectedElements.Count < 2) { return; }

            // Use the first selected element's height as the reference
            int referenceHeight = _selectedElements.First().Bounds.Height;

            foreach (BaseElement element in _selectedElements)
            {
                Rectangle bounds = element.Bounds;
                bounds.Height = referenceHeight;
                element.Bounds = bounds;
            }

            SameSizeAction action = new(_selectedElements.ToList(), "height", RefreshCanvas);
            _undoRedoManager.RecordAction(action);

            MarkChartLayoutAsManualIfNeeded();
            Canvas_Panel.Invalidate();
            OnPropertyChanged();
        }
        private void MakeSameSize()
        {
            if (_selectedElements.Count < 2) { return; }

            // Use the first selected element's size as the reference
            Size referenceSize = _selectedElements.First().Bounds.Size;

            foreach (BaseElement element in _selectedElements)
            {
                Rectangle bounds = element.Bounds;
                bounds.Size = referenceSize;
                element.Bounds = bounds;
            }

            SameSizeAction action = new(_selectedElements.ToList(), "size", RefreshCanvas);
            _undoRedoManager.RecordAction(action);

            MarkChartLayoutAsManualIfNeeded();
            Canvas_Panel.Invalidate();
            OnPropertyChanged();
        }

        // Z-Order tool methods
        public void BringElementToFront()
        {
            if (ReportConfig?.Elements == null) { return; }

            // Get elements to move
            List<BaseElement> elementsToMove = _selectedElements.Count > 0
                ? _selectedElements.ToList()
                : (_selectedElement != null ? [_selectedElement] : []);

            if (elementsToMove.Count == 0) { return; }

            // Create undo action before making changes
            string description = elementsToMove.Count == 1
                ? LanguageManager.TranslateString("Bring to front")
                : LanguageManager.TranslateString("Bring") + " " + elementsToMove.Count + " " + LanguageManager.TranslateString("elements to front");
            LayerOrderAction action = new(ReportConfig.Elements.ToList(), description, RefreshCanvas);

            int maxZOrder = ReportConfig.Elements.Max(e => e.ZOrder);

            foreach (BaseElement element in elementsToMove.OrderBy(e => e.ZOrder))
            {
                element.ZOrder = ++maxZOrder;
            }

            // Capture the new state and record the action
            action.CaptureNewState(ReportConfig.Elements.ToList());
            _undoRedoManager.RecordAction(action);

            Canvas_Panel.Invalidate();
        }
        public void SendElementToBack()
        {
            if (ReportConfig?.Elements == null) { return; }

            // Get elements to move
            List<BaseElement> elementsToMove = _selectedElements.Count > 0
                ? _selectedElements.ToList()
                : (_selectedElement != null ? [_selectedElement] : []);

            if (elementsToMove.Count == 0) { return; }

            // Create undo action before making changes
            string description = elementsToMove.Count == 1
                ? LanguageManager.TranslateString("Send to back")
                : LanguageManager.TranslateString("Send") + " " + elementsToMove.Count + " " + LanguageManager.TranslateString("elements to back");
            LayerOrderAction action = new(ReportConfig.Elements.ToList(), description, RefreshCanvas);

            // Shift all non-selected elements up
            int shiftAmount = elementsToMove.Count;
            foreach (BaseElement element in ReportConfig.Elements.Where(e => !elementsToMove.Contains(e)))
            {
                element.ZOrder += shiftAmount;
            }

            // Set selected elements to bottom
            int zOrder = 0;
            foreach (BaseElement element in elementsToMove.OrderBy(e => e.ZOrder))
            {
                element.ZOrder = zOrder++;
            }

            // Capture the new state and record the action
            action.CaptureNewState(ReportConfig.Elements.ToList());
            _undoRedoManager.RecordAction(action);

            Canvas_Panel.Invalidate();
        }

        /// <summary>
        /// Updates the enabled state of layout buttons based on the number of selected elements.
        /// </summary>
        private void UpdateLayoutButtonStates()
        {
            int selectedCount = _selectedElements.Count;

            // Alignment buttons require at least 2 selected elements
            bool canAlign = selectedCount >= 2;
            AlignLeft_Button.Enabled = canAlign;
            AlignCenter_Button.Enabled = canAlign;
            AlignRight_Button.Enabled = canAlign;
            AlignTop_Button.Enabled = canAlign;
            AlignMiddle_Button.Enabled = canAlign;
            AlignBottom_Button.Enabled = canAlign;

            // Distribution buttons require at least 3 selected elements
            bool canDistribute = selectedCount >= 3;
            DistributeHorizontally_Button.Enabled = canDistribute;
            DistributeVertically_Button.Enabled = canDistribute;

            // Size buttons require at least 2 selected elements
            bool canResize = selectedCount >= 2;
            MakeSameWidth_Button.Enabled = canResize;
            MakeSameHeight_Button.Enabled = canResize;
            MakeSameSize_Button.Enabled = canResize;
        }

        // Grids
        public static int GridSize { get; set; } = 30;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowGrid { get; set; } = false;
        private Rectangle SnapToGrid(Rectangle rect)
        {
            if (!ShowGrid) { return rect; }

            Point snappedLocation = GetGridPosition(rect.Location);
            int width = (rect.Width + GridSize / 2) / GridSize * GridSize;
            int height = (rect.Height + GridSize / 2) / GridSize * GridSize;

            return new Rectangle(snappedLocation, new Size(width, height));
        }
        private Point GetGridPosition(Point point)
        {
            if (!ShowGrid) { return point; }

            // Calculate grid offset based on header
            int topY = 0;
            if (ReportConfig.ShowHeader)
            {
                topY = ReportConfig.PageMargins.Top + ReportRenderer.HeaderHeight + ReportRenderer.SeparatorHeight;
            }

            int x = (point.X + GridSize / 2) / GridSize * GridSize;  // Snap to nearest

            // Snap Y relative to content area top
            int relativeY = point.Y - topY;
            int snappedRelativeY = (relativeY + GridSize / 2) / GridSize * GridSize;
            int y = topY + snappedRelativeY;

            return new Point(x, y);
        }

        // Selection
        private void SelectElement(BaseElement element, bool addToSelection = false)
        {
            if (element == null) { return; }

            if (!addToSelection && !_isMultiSelecting)
            {
                // Clear existing selection
                _selectedElements.Clear();
            }

            if (!_selectedElements.Contains(element))
            {
                _selectedElements.Add(element);
                _selectedElement = element;
            }

            Canvas_Panel.Invalidate();
            UpdatePropertiesForSelection();
            UpdateLayoutButtonStates();
        }
        public void SelectAllElements()
        {
            if (ReportConfig?.Elements == null) { return; }

            ClearAllSelections();

            foreach (BaseElement element in ReportConfig.Elements)
            {
                SelectElement(element, true);
            }

            Canvas_Panel.Invalidate();
            UpdateLayoutButtonStates();
        }
        private void DrawSelectionForElements(Graphics g)
        {
            if (_selectedElements.Count > 1)
            {
                // Draw selection rectangles for all selected elements
                DrawSelectionForElement(g, _selectedElements.ToArray());
            }
            else if (_selectedElement != null)
            {
                // Draw selection rectangle for the selected element
                DrawSelectionForElement(g, _selectedElement);

                if (!_isMultiSelecting)
                {
                    DrawResizeHandles(g, _selectedElement.Bounds);
                }
            }

            // Draw selection rectangle if clicking and draging for multi-select
            if (_isMultiSelecting)
            {
                using Pen selectionPen = new(CustomColors.AccentBlue, 2);
                selectionPen.DashStyle = DashStyle.Dash;
                g.DrawRectangle(selectionPen, _selectionRectangle);

                using SolidBrush selectionBrush = new(Color.FromArgb(30, CustomColors.AccentBlue));
                g.FillRectangle(selectionBrush, _selectionRectangle);
            }
        }
        private static void DrawSelectionForElement(Graphics g, params BaseElement[] elements)
        {
            foreach (BaseElement element in elements)
            {
                using Pen pen = new(CustomColors.AccentBlue, 4);
                pen.DashStyle = DashStyle.Solid;
                g.DrawRectangle(pen, element.Bounds);
            }
        }
        public void ClearAllSelections()
        {
            _selectedElements.Clear();
            _selectedElement = null;
            Canvas_Panel.Invalidate();
            HidePropertiesPanel();
            UpdateLayoutButtonStates();
        }
        private void UpdatePropertiesForSelection()
        {
            if (_selectedElements.Count == 0)
            {
                HidePropertiesPanel();
            }
            else if (_selectedElements.Count == 1)
            {
                CreateOrShowPropertiesPanel();
            }
            else
            {
                // Show multi-selection info
                ElementProperties_Label.Text = LanguageManager.TranslateString("Selected") + ": " + _selectedElements.Count + LanguageManager.TranslateString("elements");

                PropertiesContainer_Panel.Controls.Clear();
            }
        }
        private void UpdateSelectionFromRectangle()
        {
            if (ReportConfig?.Elements == null) { return; }

            // Track which elements should be selected based on rectangle intersection
            HashSet<BaseElement> elementsInRectangle = [];

            foreach (BaseElement element in ReportConfig.Elements)
            {
                if (_selectionRectangle.IntersectsWith(element.Bounds))
                {
                    elementsInRectangle.Add(element);
                }
            }

            // Process elements that should be selected but aren't
            foreach (BaseElement element in elementsInRectangle)
            {
                if (!_selectedElements.Contains(element))
                {
                    _selectedElements.Add(element);
                    InvalidateElementRegion(element.Bounds);
                }
            }

            // Process elements that should be unselected
            List<BaseElement> toRemove = _selectedElements
                .Where(e => !elementsInRectangle.Contains(e))
                .ToList();

            foreach (BaseElement element in toRemove)
            {
                _selectedElements.Remove(element);
                InvalidateElementRegion(element.Bounds);
            }

            // Update the primary selected element
            if (_selectedElements.Count > 0 && !_selectedElements.Contains(_selectedElement))
            {
                _selectedElement = _selectedElements.First();
            }
            else if (_selectedElements.Count == 0)
            {
                _selectedElement = null;
            }

            UpdateLayoutButtonStates();
            UpdatePropertiesForSelection();
        }

        /// <summary>
        /// Clears selection for any elements that have been removed from the configuration.
        /// </summary>
        public void ClearSelectionForRemovedElements()
        {
            if (ReportConfig?.Elements == null) { return; }

            bool selectionChanged = false;

            // Check if the primary selected element was removed
            if (_selectedElement != null && !ReportConfig.Elements.Contains(_selectedElement))
            {
                _selectedElement = null;
                selectionChanged = true;
            }

            // Check if any multi-selected elements were removed
            List<BaseElement> removedElements = _selectedElements
                .Where(element => !ReportConfig.Elements.Contains(element))
                .ToList();

            foreach (BaseElement element in removedElements)
            {
                _selectedElements.Remove(element);
                selectionChanged = true;
            }

            // Update the primary selected element if needed
            if (_selectedElement == null && _selectedElements.Count > 0)
            {
                _selectedElement = _selectedElements.First();
            }

            // Update UI if selection changed
            if (selectionChanged)
            {
                Canvas_Panel.Invalidate();
                UpdatePropertiesForSelection();
                UpdateLayoutButtonStates();
            }
        }

        // Property panel methods
        private void CreateOrShowPropertiesPanel()
        {
            // Capitalize first letter of element name for display
            string elementName = char.ToUpper(_selectedElement.DisplayName[0]) + _selectedElement.DisplayName[1..];
            ElementProperties_Label.Text = LanguageManager.TranslateString("Selected") + ": " + elementName;

            // Only recreate if different element selected
            if (_currentPropertyElement != _selectedElement)
            {
                // This prevents flickering
                LoadingPanel.ShowBlankLoadingPanel(PropertiesContainer_Panel, CustomColors.ControlBack);

                _currentPropertyElement = _selectedElement;

                // Element handles its own caching internally
                _selectedElement.CreatePropertyControls(
                    PropertiesContainer_Panel,
                    10,
                    OnPropertyChanged);

                ThemeManager.CustomizeScrollBar(_selectedElement.CachedPropertyPanel);
                UpdatePropertyContainerTheme();

                if (_selectedElement is TableElement)
                {
                    _selectedElement.UpdateAllControlValues();
                }

                LoadingPanel.HideBlankLoadingPanel(PropertiesContainer_Panel);
            }
            else
            {
                // Just update values, controls already exist
                _selectedElement.CreatePropertyControls(
                    PropertiesContainer_Panel,
                    10,
                    OnPropertyChanged);
            }
        }
        private void UpdatePropertyContainerTheme()
        {
            // Get the cached panel if it exists
            Panel cachedPanel = PropertiesContainer_Panel.Controls
                .OfType<Panel>()
                .FirstOrDefault(p => p != LoadingPanel.BlankLoadingPanelInstance);

            if (cachedPanel != null)
            {
                // Set the BackColor of the cached panel itself
                cachedPanel.BackColor = CustomColors.ControlBack;

                // Get controls from inside the cached panel
                List<Control> controls = cachedPanel.Controls
                    .Cast<Control>()
                    .Where(c => c.Tag?.ToString() != BaseElement.ColorPickerTag)
                    .ToList();

                ThemeManager.SetThemeForControls(controls);
            }

            // Set BackColor for TableElement's tab panels if this is a TableElement
            if (_selectedElement is TableElement tableElement)
            {
                if (tableElement.General_Panel != null)
                {
                    tableElement.General_Panel.BackColor = CustomColors.ControlBack;
                    ThemeManager.CustomizeScrollBar(tableElement.General_Panel);
                }
                if (tableElement.Style_Panel != null)
                {
                    tableElement.Style_Panel.BackColor = CustomColors.ControlBack;
                }
                if (tableElement.Columns_Panel != null)
                {
                    tableElement.Columns_Panel.BackColor = CustomColors.ControlBack;
                }
                if (tableElement.Tab_Panel != null)
                {
                    tableElement.Tab_Panel.BackColor = CustomColors.ControlBack;
                }
            }

            // Find all CheckBoxes inside the cached panel
            if (cachedPanel != null)
            {
                foreach (Guna2CustomCheckBox checkBox in cachedPanel.Controls.OfType<Guna2CustomCheckBox>())
                {
                    checkBox.UncheckedState.FillColor = CustomColors.MainBackground;
                }
            }

            // Also find the CheckBoxes inside TableElement's tab panels if this is a TableElement
            if (_selectedElement is TableElement tableElement1)
            {
                Panel[] tabPanels = [tableElement1.General_Panel, tableElement1.Style_Panel, tableElement1.Columns_Panel];

                foreach (Panel panel in tabPanels)
                {
                    if (panel != null)
                    {
                        foreach (Guna2CustomCheckBox checkBox in panel.Controls.OfType<Guna2CustomCheckBox>())
                        {
                            checkBox.UncheckedState.FillColor = CustomColors.MainBackground;
                        }
                    }
                }
            }
        }
        public void UpdatePropertyValues()
        {
            // Don't show element properties if multiple elements are selected
            if (_selectedElements.Count > 1)
            {
                return;
            }

            // Element handles its own value updates internally
            _selectedElement?.CreatePropertyControls(
                PropertiesContainer_Panel,
                10,
                OnPropertyChanged);
        }
        private void HidePropertiesPanel()
        {
            ElementProperties_Label.Text = LanguageManager.TranslateString("No element selected");
            PropertiesContainer_Panel.Controls.Clear();
            _currentPropertyElement = null;
        }
        private void OnPropertyChanged()
        {
            Canvas_Panel.Invalidate();
            NotifyParentValidationChanged();
            MarkAsChanged();
        }

        /// <summary>
        /// Refreshes property panels after language change by clearing cache and recreating controls.
        /// </summary>
        public void RefreshPropertyPanelTranslations()
        {
            // Clear property cache for all elements
            if (ReportConfig?.Elements != null)
            {
                foreach (BaseElement element in ReportConfig.Elements)
                {
                    element.ClearPropertyControlCache();
                }
            }

            _currentPropertyElement = null;

            // Recreate the property panel for the currently selected element with new translations
            if (_selectedElement != null)
            {
                CreateOrShowPropertiesPanel();
            }
        }

        // Canvas methods
        private void InvalidateElementRegion(Rectangle bounds)
        {
            // Convert page bounds to scaled canvas coordinates for invalidation
            Rectangle scaledBounds = PageToScaledRectangle(bounds);
            // Inflate by 50 pixels to ensure complex elements (like tables with multiple rows)
            // are fully repainted without leaving artifacts
            scaledBounds.Inflate(50, 50);
            Canvas_Panel.Invalidate(scaledBounds);
        }
        private Rectangle PageToScaledRectangle(Rectangle pageRect)
        {
            return new Rectangle(
                (int)(pageRect.X * _canvasScaleFactor),
                (int)(pageRect.Y * _canvasScaleFactor),
                (int)(pageRect.Width * _canvasScaleFactor),
                (int)(pageRect.Height * _canvasScaleFactor)
            );
        }
        private void RefreshCanvas()
        {
            Canvas_Panel.Invalidate();
            UpdatePropertiesForSelection();
            NotifyParentValidationChanged();
        }
        private static void OnChartMovedOrResized()
        {
            if (ReportConfig != null)
            {
                ReportConfig.HasManualChartLayout = true;
            }
        }
        private void MarkChartLayoutAsManualIfNeeded()
        {
            if (_selectedElements.Any(e => e is ChartElement) || _selectedElement is ChartElement)
            {
                OnChartMovedOrResized();
            }
        }

        // Resize element methods
        private void ResizeElement(Point currentPoint)
        {
            if (_selectedElement == null) { return; }

            int deltaX = currentPoint.X - _dragStartPoint.X;
            int deltaY = currentPoint.Y - _dragStartPoint.Y;
            Rectangle newBounds = _originalBounds;

            switch (_activeResizeHandle)
            {
                case ResizeHandle.TopLeft:
                    newBounds.X += deltaX;
                    newBounds.Y += deltaY;
                    newBounds.Width -= deltaX;
                    newBounds.Height -= deltaY;
                    break;
                case ResizeHandle.TopRight:
                    newBounds.Y += deltaY;
                    newBounds.Width += deltaX;
                    newBounds.Height -= deltaY;
                    break;
                case ResizeHandle.BottomLeft:
                    newBounds.X += deltaX;
                    newBounds.Width -= deltaX;
                    newBounds.Height += deltaY;
                    break;
                case ResizeHandle.BottomRight:
                    newBounds.Width += deltaX;
                    newBounds.Height += deltaY;
                    break;
                case ResizeHandle.Left:
                    newBounds.X += deltaX;
                    newBounds.Width -= deltaX;
                    break;
                case ResizeHandle.Right:
                    newBounds.Width += deltaX;
                    break;
                case ResizeHandle.Top:
                    newBounds.Y += deltaY;
                    newBounds.Height -= deltaY;
                    break;
                case ResizeHandle.Bottom:
                    newBounds.Height += deltaY;
                    break;
            }

            // Enforce minimum size
            if (newBounds.Width < _selectedElement.MinimumSize)
            {
                newBounds.Width = _selectedElement.MinimumSize;
                if (_activeResizeHandle == ResizeHandle.Left || _activeResizeHandle == ResizeHandle.TopLeft || _activeResizeHandle == ResizeHandle.BottomLeft)
                {
                    newBounds.X = _originalBounds.Right - _selectedElement.MinimumSize;
                }
            }

            if (newBounds.Height < _selectedElement.MinimumSize)
            {
                newBounds.Height = _selectedElement.MinimumSize;
                if (_activeResizeHandle == ResizeHandle.Top || _activeResizeHandle == ResizeHandle.TopLeft || _activeResizeHandle == ResizeHandle.TopRight)
                {
                    newBounds.Y = _originalBounds.Bottom - _selectedElement.MinimumSize;
                }
            }

            // Enforce canvas bounds
            Size pageSize = PageDimensions.GetDimensions(ReportConfig.PageSize, ReportConfig.PageOrientation);
            if (newBounds.X < 0) { newBounds.X = 0; }
            if (newBounds.Y < 0) { newBounds.Y = 0; }
            if (newBounds.Right > pageSize.Width)
            {
                newBounds.Width = pageSize.Width - newBounds.X;
            }
            if (newBounds.Bottom > pageSize.Height)
            {
                newBounds.Height = pageSize.Height - newBounds.Y;
            }

            // Apply grid snapping if enabled
            if (ShowGrid)
            {
                newBounds = SnapToGrid(newBounds);
            }

            // Update element bounds and invalidate
            // Invalidate the union of old and new bounds to ensure no rendering artifacts remain
            // This is especially important for complex elements like tables that render multiple rows
            Rectangle invalidationRect = Rectangle.Union(_lastElementBounds, newBounds);
            InvalidateElementRegion(invalidationRect);

            _selectedElement.Bounds = newBounds;
            _lastElementBounds = newBounds;

            // Defer property panel update
            _propertyUpdateTimer.Stop();
            _propertyUpdateTimer.Start();
        }
        private static void DrawResizeHandles(Graphics g, Rectangle bounds)
        {
            const int handleSize = 12;
            using Brush brush = new SolidBrush(Color.Blue);

            // Corner handles
            g.FillRectangle(brush, bounds.Left - handleSize / 2, bounds.Top - handleSize / 2, handleSize, handleSize);
            g.FillRectangle(brush, bounds.Right - handleSize / 2, bounds.Top - handleSize / 2, handleSize, handleSize);
            g.FillRectangle(brush, bounds.Left - handleSize / 2, bounds.Bottom - handleSize / 2, handleSize, handleSize);
            g.FillRectangle(brush, bounds.Right - handleSize / 2, bounds.Bottom - handleSize / 2, handleSize, handleSize);
        }
        private static ResizeHandle GetResizeHandleAtPoint(Point point, BaseElement element)
        {
            if (element == null) { return ResizeHandle.None; }

            const int handleSize = 8;
            Rectangle bounds = element.Bounds;

            // Corner handles
            if (new Rectangle(bounds.Left - handleSize / 2, bounds.Top - handleSize / 2, handleSize, handleSize).Contains(point))
            {
                return ResizeHandle.TopLeft;
            }
            if (new Rectangle(bounds.Right - handleSize / 2, bounds.Top - handleSize / 2, handleSize, handleSize).Contains(point))
            {
                return ResizeHandle.TopRight;
            }
            if (new Rectangle(bounds.Left - handleSize / 2, bounds.Bottom - handleSize / 2, handleSize, handleSize).Contains(point))
            {
                return ResizeHandle.BottomLeft;
            }
            if (new Rectangle(bounds.Right - handleSize / 2, bounds.Bottom - handleSize / 2, handleSize, handleSize).Contains(point))
            {
                return ResizeHandle.BottomRight;
            }

            // Edge handles
            if (new Rectangle(bounds.Left - handleSize / 2, bounds.Top + handleSize, handleSize, bounds.Height - 2 * handleSize).Contains(point))
            {
                return ResizeHandle.Left;
            }
            if (new Rectangle(bounds.Right - handleSize / 2, bounds.Top + handleSize, handleSize, bounds.Height - 2 * handleSize).Contains(point))
            {
                return ResizeHandle.Right;
            }
            if (new Rectangle(bounds.Left + handleSize, bounds.Top - handleSize / 2, bounds.Width - 2 * handleSize, handleSize).Contains(point))
            {
                return ResizeHandle.Top;
            }
            return new Rectangle(bounds.Left + handleSize, bounds.Bottom - handleSize / 2, bounds.Width - 2 * handleSize, handleSize).Contains(point)
                ? ResizeHandle.Bottom
                : ResizeHandle.None;
        }
        private static Cursor GetCursorForHandle(ResizeHandle handle)
        {
            return handle switch
            {
                ResizeHandle.TopLeft or ResizeHandle.BottomRight => Cursors.SizeNWSE,
                ResizeHandle.TopRight or ResizeHandle.BottomLeft => Cursors.SizeNESW,
                ResizeHandle.Left or ResizeHandle.Right => Cursors.SizeWE,
                ResizeHandle.Top or ResizeHandle.Bottom => Cursors.SizeNS,
                _ => Cursors.Default
            };
        }

        // Element management
        private void CreateElementAtLocation(ReportElementType elementType, Point location)
        {
            BaseElement element = CreateElementByType(elementType, location);
            if (element != null)
            {
                _undoRedoManager.RecordAction(new AddElementAction(element, ReportConfig, RefreshCanvas));
                ReportConfig?.AddElement(element);
                SelectElement(element);
                Canvas_Panel.Invalidate();
                Canvas_Panel.Focus();
                NotifyParentValidationChanged();
                MarkAsChanged();
            }
        }
        private static BaseElement? CreateElementByType(ReportElementType elementType, Point location)
        {
            return elementType switch
            {
                ReportElementType.Chart => new ChartElement
                {
                    Bounds = new Rectangle(location, new Size(350, 250)),
                    ChartType = GetDefaultChartType()
                },
                ReportElementType.Label => new LabelElement
                {
                    Bounds = new Rectangle(location, new Size(200, 30))
                },
                ReportElementType.DateRange => new DateRangeElement
                {
                    Bounds = new Rectangle(location, new Size(250, 30))
                },
                ReportElementType.Summary => new SummaryElement
                {
                    Bounds = new Rectangle(location, new Size(300, 170))
                },
                ReportElementType.TransactionTable => new TableElement
                {
                    Bounds = new Rectangle(location, new Size(400, 200))
                },
                ReportElementType.Image => new ImageElement
                {
                    Bounds = new Rectangle(location, new Size(200, 200))
                },
                _ => null
            };
        }
        public void DuplicateSelected()
        {
            if (_selectedElements.Count == 0 && _selectedElement == null) { return; }
            if (ReportConfig == null) { return; }

            List<BaseElement> duplicates = [];

            string description = _selectedElements.Count > 1
                ? LanguageManager.TranslateString("Duplicate") + " " + _selectedElements.Count + " " + LanguageManager.TranslateString("elements")
                : LanguageManager.TranslateString("Duplicate element");
            CompositeAction composite = new(description);

            if (_selectedElements.Count > 0)
            {
                foreach (BaseElement element in _selectedElements)
                {
                    BaseElement duplicate = element.Clone();

                    // Offset the duplicate slightly
                    Rectangle bounds = duplicate.Bounds;
                    bounds.Offset(20, 20);
                    duplicate.Bounds = bounds;

                    composite.AddAction(new AddElementAction(duplicate, ReportConfig, RefreshCanvas));
                    ReportConfig.AddElement(duplicate);
                    duplicates.Add(duplicate);
                }
            }
            else if (_selectedElement != null)
            {
                BaseElement duplicate = _selectedElement.Clone();

                // Offset the duplicate slightly
                Rectangle bounds = duplicate.Bounds;
                bounds.Offset(20, 20);
                duplicate.Bounds = bounds;

                composite.AddAction(new AddElementAction(duplicate, ReportConfig, RefreshCanvas));
                ReportConfig.AddElement(duplicate);
                duplicates.Add(duplicate);
            }

            _undoRedoManager.RecordAction(composite);

            // Select the duplicated elements
            ClearAllSelections();
            foreach (BaseElement duplicate in duplicates)
            {
                SelectElement(duplicate, true);
            }

            Canvas_Panel.Invalidate();
            OnPropertyChanged();

            // Focus canvas to ensure the duplicated element remains selected
            Canvas_Panel.Focus();
        }
        public void DeleteSelected()
        {
            // Handle both old single-selection and multi-selection
            if (_selectedElements.Count == 0 && _selectedElement == null)
            {
                return;
            }

            if (ReportConfig == null)
            {
                return;
            }

            // Collect all elements to delete
            List<BaseElement> elementsToDelete = [];

            if (_selectedElements.Count > 0)
            {
                elementsToDelete.AddRange(_selectedElements);
            }
            else if (_selectedElement != null)
            {
                elementsToDelete.Add(_selectedElement);
            }

            // Create undo actions
            string description = elementsToDelete.Count == 1
                ? LanguageManager.TranslateString("Delete element")
                : LanguageManager.TranslateString("Delete") + " " + elementsToDelete.Count + " " + LanguageManager.TranslateString("elements");
            CompositeAction composite = new(description);

            // Delete all selected elements
            foreach (BaseElement element in elementsToDelete)
            {
                composite.AddAction(new RemoveElementAction(element, ReportConfig, RefreshCanvas));

                // If deleting a chart element, also remove it from SelectedChartTypes
                if (element is ChartElement chartElement)
                {
                    ReportConfig.Filters.SelectedChartTypes.Remove(chartElement.ChartType);
                }

                ReportConfig.RemoveElement(element.Id);
            }

            _undoRedoManager.RecordAction(composite);

            // If we deleted a chart element, switch to custom template and sync the selection
            if (elementsToDelete.Any(e => e is ChartElement))
            {
                ReportDataSelection_Form.Instance?.SwitchToCustomTemplate();
                ReportDataSelection_Form.Instance?.SyncChartSelectionFromConfig();
            }

            // Clear selection
            ClearAllSelections();
            Canvas_Panel.Invalidate();
            OnPropertyChanged();
            MarkAsChanged();
        }
        private static MainMenu_Form.ChartDataType GetDefaultChartType()
        {
            // Use the first selected chart type, or default to TotalSales
            return ReportConfig?.Filters?.SelectedChartTypes?.FirstOrDefault() ?? MainMenu_Form.ChartDataType.TotalRevenue;
        }
        private static BaseElement? GetElementAtPoint(Point point)
        {
            return ReportConfig?.Elements?
                .Where(e => e.IsVisible && e.Bounds.Contains(point))
                .OrderByDescending(e => e.ZOrder)
                .FirstOrDefault();
        }

        // Form implementation methods
        public static bool IsValidForNextStep()
        {
            return ReportConfig?.Elements?.Count > 0;
        }
        private static void NotifyParentValidationChanged()
        {
            ReportGenerator_Form.Instance.OnChildFormValidationChanged();
        }

        // Template save/load functionality
        private void SaveTemplate_Button_Click(object sender, EventArgs e)
        {
            // For built-in templates, don't allow updating - only save as new
            string currentTemplateForSave = _currentTemplateName;
            if (!string.IsNullOrEmpty(_currentTemplateName) && ReportTemplates.IsBuiltInTemplate(_currentTemplateName))
            {
                currentTemplateForSave = null;
            }

            using SaveTemplate_Form form = new()
            {
                CurrentTemplateName = currentTemplateForSave
            };

            if (form.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(form.TemplateName))
            {
                if (CustomTemplateStorage.SaveTemplate(form.TemplateName, ReportConfig))
                {
                    _currentTemplateName = form.TemplateName;
                    _undoRedoManager.MarkSaved();  // Mark this as the saved state
                    SetUnsavedChanges(false);

                    // Refresh the template list and select the newly saved template
                    ReportDataSelection_Form.Instance?.RefreshTemplates(form.TemplateName);
                }
                else
                {
                    CustomMessageBox.ShowWithFormat(
                        "Save Failed",
                        "Failed to save template '{0}'.",
                        CustomMessageBoxIcon.Error,
                        CustomMessageBoxButtons.Ok,
                        form.TemplateName);
                }
            }
        }
        public void OnConfigurationLoaded()
        {
            _currentTemplateName = ReportConfig?.Title;
            _undoRedoManager.MarkSaved();  // Mark this as the saved state
            SetUnsavedChanges(false);
            RefreshCanvas();
        }

        /// <summary>
        /// Clears the current template if it matches the given template name.
        /// This is used when a template is deleted to ensure the designer doesn't keep a deleted template loaded.
        /// </summary>
        public void ClearIfTemplateDeleted(string deletedTemplateName)
        {
            if (_currentTemplateName == deletedTemplateName)
            {
                _currentTemplateName = null;
                SetUnsavedChanges(false);
                _undoRedoManager.Clear();

                // Clear all elements from the configuration
                if (ReportConfig != null)
                {
                    // Dispose elements
                    foreach (BaseElement element in ReportConfig.Elements.ToList())
                    {
                        if (element is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }

                    ReportConfig.Elements.Clear();

                    ClearAllSelections();
                    RefreshCanvas();
                }
            }
        }

        /// <summary>
        /// Prompts the user to save unsaved changes.
        /// </summary>
        /// <returns>True if the operation should continue, false if cancelled</returns>
        public bool PromptToSaveChanges()
        {
            if (!HasUnsavedChanges) { return true; }

            CustomMessageBoxResult result = CustomMessageBox.Show(
                "Unsaved Changes",
                "You have unsaved changes to your custom template.\nDo you want to save your changes?",
                CustomMessageBoxIcon.Question,
                CustomMessageBoxButtons.YesNoCancel);

            if (result == CustomMessageBoxResult.Yes)
            {
                // Save the template
                SaveTemplate_Button.PerformClick();
                return true;
            }
            if (result == CustomMessageBoxResult.No)
            {
                // User chose not to save
                DisposeCachedImages();
                CustomTemplateStorage.CleanupUnusedImages();
                SetUnsavedChanges(false);
                return true;
            }

            // The user clicked Cancel or closed the message dialog, so do not continue the operation
            return false;
        }

        /// <summary>
        /// Disposes all cached images in ImageElements to release file locks.
        /// </summary>
        public static void DisposeCachedImages()
        {
            if (ReportConfig?.Elements == null) { return; }

            foreach (BaseElement element in ReportConfig.Elements)
            {
                if (element is ImageElement imageElement)
                {
                    imageElement.Dispose();
                }
            }
        }

        private void SetUnsavedChanges(bool hasChanges)
        {
            string title = LanguageManager.TranslateString("Report Layout Designer");

            HasUnsavedChanges = hasChanges;
            UnsavedChanges_Label.Visible = hasChanges;

            // Update form title with asterisk if there are unsaved changes
            if (hasChanges)
            {
                ReportGenerator_Form.Instance.Text = title + " *";
            }
            else
            {
                ReportGenerator_Form.Instance.Text = title;
            }
        }
        private void MarkAsChanged()
        {
            if (!HasUnsavedChanges)
            {
                SetUnsavedChanges(true);
            }
        }

        /// <summary>
        /// Called when the undo/redo state changes. Updates the unsaved changes indicator
        /// based on whether we're at the saved state.
        /// </summary>
        private void OnUndoRedoStateChanged(object sender, EventArgs e)
        {
            // If we're at the saved state, clear the unsaved changes indicator
            if (_undoRedoManager.IsAtSavedState)
            {
                SetUnsavedChanges(false);
            }
            // If we're not at the saved state and indicator is not showing, mark as changed
            else if (!HasUnsavedChanges)
            {
                SetUnsavedChanges(true);
            }
        }
    }
}