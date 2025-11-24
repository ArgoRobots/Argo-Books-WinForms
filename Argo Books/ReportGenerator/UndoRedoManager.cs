using Argo_Books.ReportGenerator.Elements;
using Argo_Books.ReportGenerator.Menus;
using System.Reflection;
using System.Text;
using Timer = System.Windows.Forms.Timer;

namespace Argo_Books.ReportGenerator
{
    /// <summary>
    /// Manages undo/redo operations for the report layout designer.
    /// </summary>
    public class UndoRedoManager(int maxStackSize = 100)
    {
        private readonly Stack<IUndoableAction> _undoStack = new();
        private readonly Stack<IUndoableAction> _redoStack = new();
        private readonly int _maxStackSize = maxStackSize;
        private bool _isExecutingUndoRedo = false;
        private int? _savedStateUndoCount = 0;  // Track undo count at save point (null if saved state was lost)

        public event EventHandler StateChanged;

        /// <summary>
        /// Gets the count of actions in the undo stack.
        /// </summary>
        public int UndoCount => _undoStack.Count;

        /// <summary>
        /// Gets the count of actions in the redo stack.
        /// </summary>
        public int RedoCount => _redoStack.Count;

        /// <summary>
        /// Gets whether an undo operation can be performed.
        /// </summary>
        public bool CanUndo => _undoStack.Count > 0;

        /// <summary>
        /// Gets whether a redo operation can be performed.
        /// </summary>
        public bool CanRedo => _redoStack.Count > 0;

        /// <summary>
        /// Gets the description of the next undo action.
        /// </summary>
        public string UndoDescription => CanUndo ? _undoStack.Peek().Description : "";

        /// <summary>
        /// Gets the description of the next redo action.
        /// </summary>
        public string RedoDescription => CanRedo ? _redoStack.Peek().Description : "";

        /// <summary>
        /// Gets whether the current state matches the last saved state.
        /// </summary>
        public bool IsAtSavedState => _savedStateUndoCount.HasValue && _undoStack.Count == _savedStateUndoCount.Value;

        /// <summary>
        /// Marks the current state as saved. This should be called after successfully saving.
        /// </summary>
        public void MarkSaved()
        {
            _savedStateUndoCount = _undoStack.Count;
            OnStateChanged();
        }

        /// <summary>
        /// Records an action for undo/redo.
        /// </summary>
        public void RecordAction(IUndoableAction action)
        {
            if (_isExecutingUndoRedo) { return; }

            _undoStack.Push(action);
            _redoStack.Clear();

            // If we're recording a new action after undoing past the saved state,
            // the saved state becomes unreachable
            if (_savedStateUndoCount.HasValue && _undoStack.Count - 1 < _savedStateUndoCount.Value)
            {
                _savedStateUndoCount = null;
            }

            // Limit stack size
            while (_undoStack.Count > _maxStackSize)
            {
                IUndoableAction[] oldestActions = _undoStack.ToArray();
                _undoStack.Clear();
                for (int i = 1; i < oldestActions.Length; i++)
                {
                    _undoStack.Push(oldestActions[i]);
                }

                // If the saved state was in the removed action, mark it as lost
                if (_savedStateUndoCount.HasValue)
                {
                    _savedStateUndoCount--;
                    if (_savedStateUndoCount < 0)
                    {
                        _savedStateUndoCount = null;
                    }
                }
            }

            OnStateChanged();
        }

        /// <summary>
        /// Performs an undo operation.
        /// </summary>
        public void Undo()
        {
            if (!CanUndo) { return; }

            _isExecutingUndoRedo = true;
            try
            {
                // Clear selections before undoing
                ReportLayoutDesigner_Form.Instance.ClearAllSelections();

                IUndoableAction action = _undoStack.Pop();
                action.Undo();
                _redoStack.Push(action);
                OnStateChanged();
            }
            finally
            {
                _isExecutingUndoRedo = false;
            }
        }

        /// <summary>
        /// Performs a redo operation.
        /// </summary>
        public void Redo()
        {
            if (!CanRedo) { return; }

            _isExecutingUndoRedo = true;
            try
            {
                // Clear selections before redoing
                ReportLayoutDesigner_Form.Instance.ClearAllSelections();

                IUndoableAction action = _redoStack.Pop();
                action.Redo();
                _undoStack.Push(action);
                OnStateChanged();
            }
            finally
            {
                _isExecutingUndoRedo = false;
            }
        }

        /// <summary>
        /// Clears all undo/redo history.
        /// </summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            _savedStateUndoCount = 0;  // Reset to initial saved state
            OnStateChanged();
        }

        /// <summary>
        /// Gets descriptions of all actions in the undo stack (most recent first).
        /// </summary>
        public string[] GetUndoDescriptions()
        {
            return _undoStack.Select(action => action.Description).ToArray();
        }

        /// <summary>
        /// Gets descriptions of all actions in the redo stack (next to redo first).
        /// </summary>
        public string[] GetRedoDescriptions()
        {
            return _redoStack.Select(action => action.Description).ToArray();
        }

        protected virtual void OnStateChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Interface for undoable actions.
    /// </summary>
    public interface IUndoableAction
    {
        string Description { get; }
        void Undo();
        void Redo();
    }

    /// <summary>
    /// Action for adding an element to the canvas.
    /// </summary>
    public class AddElementAction(BaseElement element, ReportConfiguration reportConfig, Action refreshCanvas) : IUndoableAction
    {
        private readonly BaseElement _element = element;
        private readonly ReportConfiguration _reportConfig = reportConfig;
        private readonly Action _refreshCanvas = refreshCanvas;

        public string Description => $"Add {_element.DisplayName}";

        public void Undo()
        {
            _reportConfig.Elements.Remove(_element);
            _refreshCanvas();
        }
        public void Redo()
        {
            _reportConfig.Elements.Add(_element);
            _refreshCanvas();
        }
    }

    /// <summary>
    /// Action for removing an element from the canvas.
    /// </summary>
    public class RemoveElementAction(BaseElement element, ReportConfiguration reportConfig, Action refreshCanvas) : IUndoableAction
    {
        private readonly BaseElement _element = element;
        private readonly ReportConfiguration _reportConfig = reportConfig;
        private readonly int _originalIndex = reportConfig.Elements.IndexOf(element);
        private readonly Action _refreshCanvas = refreshCanvas;

        public string Description => $"Remove {_element.DisplayName}";

        public void Undo()
        {
            if (_originalIndex >= 0 && _originalIndex <= _reportConfig.Elements.Count)
            {
                _reportConfig.Elements.Insert(_originalIndex, _element);
            }
            else
            {
                _reportConfig.Elements.Add(_element);
            }
            _refreshCanvas();
        }
        public void Redo()
        {
            _reportConfig.Elements.Remove(_element);
            _refreshCanvas();
        }
    }

    /// <summary>
    /// Action for moving elements.
    /// </summary>
    public class MoveElementsAction : IUndoableAction
    {
        private readonly List<ElementMove> _moves = [];
        private readonly Action _refreshCanvas;

        private class ElementMove
        {
            public BaseElement Element { get; set; }
            public Rectangle OldBounds { get; set; }
            public Rectangle NewBounds { get; set; }
        }

        public MoveElementsAction(List<BaseElement> elements, List<Rectangle> oldBounds, List<Rectangle> newBounds, Action refreshCanvas)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                _moves.Add(new ElementMove
                {
                    Element = elements[i],
                    OldBounds = oldBounds[i],
                    NewBounds = newBounds[i]
                });
            }
            _refreshCanvas = refreshCanvas;
        }

        public string Description => _moves.Count == 1 ? "Move element" : $"Move {_moves.Count} elements";

        public void Undo()
        {
            foreach (ElementMove move in _moves)
            {
                move.Element.Bounds = move.OldBounds;
            }
            _refreshCanvas();
        }
        public void Redo()
        {
            foreach (ElementMove move in _moves)
            {
                move.Element.Bounds = move.NewBounds;
            }
            _refreshCanvas();
        }
    }

    /// <summary>
    /// Action for resizing an element.
    /// </summary>
    public class ResizeElementAction(BaseElement element, Rectangle oldBounds, Rectangle newBounds, Action refreshCanvas) : IUndoableAction
    {
        private readonly BaseElement _element = element;
        private readonly Rectangle _oldBounds = oldBounds;
        private readonly Rectangle _newBounds = newBounds;
        private readonly Action _refreshCanvas = refreshCanvas;

        public string Description => $"Resize {_element.DisplayName}";

        public void Undo()
        {
            _element.Bounds = _oldBounds;
            _refreshCanvas();
        }
        public void Redo()
        {
            _element.Bounds = _newBounds;
            _refreshCanvas();
        }
    }

    /// <summary>
    /// Action for changing element properties.
    /// </summary>
    public class PropertyChangeAction(BaseElement element, string propertyName, object oldValue, object newValue, Action refreshCanvas) : IUndoableAction
    {
        private readonly BaseElement _element = element;
        private readonly string _propertyName = propertyName;
        private readonly object _oldValue = oldValue;
        private readonly object _newValue = newValue;
        private readonly Action _refreshCanvas = refreshCanvas;

        public string Description => $"Change {FormatPropertyName(_propertyName)}";

        private static string FormatPropertyName(string propertyName)
        {
            // Convert PascalCase to lowercase with spaces
            // Example: "FontSize" -> "font size", "ShowLegend" -> "show legend"
            StringBuilder result = new();

            for (int i = 0; i < propertyName.Length; i++)
            {
                char c = propertyName[i];

                // Add space before capital letters (except first character)
                if (i > 0 && char.IsUpper(c))
                {
                    result.Append(' ');
                }

                result.Append(char.ToLower(c));
            }

            return result.ToString();
        }

        public void Undo()
        {
            PropertyInfo? property = _element.GetType().GetProperty(_propertyName);
            property?.SetValue(_element, _oldValue);

            // Update the property panel to reflect the change
            ReportLayoutDesigner_Form.Instance?.UpdatePropertyValues();

            _refreshCanvas?.Invoke();
        }
        public void Redo()
        {
            PropertyInfo? property = _element.GetType().GetProperty(_propertyName);
            property?.SetValue(_element, _newValue);

            // Update the property panel to reflect the change
            ReportLayoutDesigner_Form.Instance?.UpdatePropertyValues();

            _refreshCanvas?.Invoke();
        }
    }

    public class PropertyChangeDebouncer
    {
        private readonly Timer _debounceTimer;
        private object _pendingOldValue;
        private object _pendingNewValue;
        private readonly BaseElement _element;
        private readonly string _propertyName;
        private readonly Action _onPropertyChanged;

        public PropertyChangeDebouncer(BaseElement element, string propertyName, Action onPropertyChanged)
        {
            _element = element;
            _propertyName = propertyName;
            _onPropertyChanged = onPropertyChanged;

            _debounceTimer = new Timer { Interval = 500 };  // 500ms delay
            _debounceTimer.Tick += OnDebounceTimerTick;
        }

        public void QueueChange(object oldValue, object newValue)
        {
            _pendingOldValue ??= oldValue;

            _pendingNewValue = newValue;

            _debounceTimer.Stop();
            _debounceTimer.Start();
        }
        private void OnDebounceTimerTick(object sender, EventArgs e)
        {
            _debounceTimer.Stop();

            if (_pendingOldValue != null && _pendingNewValue != null)
            {
                UndoRedoManager? undoRedoManager = ReportLayoutDesigner_Form.Instance?.GetUndoRedoManager();
                undoRedoManager?.RecordAction(new PropertyChangeAction(
                    _element,
                    _propertyName,
                    _pendingOldValue,
                    _pendingNewValue,
                    _onPropertyChanged));

                _pendingOldValue = null;
                _pendingNewValue = null;
            }
        }
    }

    /// <summary>
    /// Composite action that groups multiple actions together.
    /// </summary>
    public class CompositeAction(string description) : IUndoableAction
    {
        private readonly List<IUndoableAction> _actions = [];
        private readonly string _description = description;

        public string Description => _description;

        public void AddAction(IUndoableAction action)
        {
            _actions.Add(action);
        }
        public void Undo()
        {
            // Undo in reverse order
            for (int i = _actions.Count - 1; i >= 0; i--)
            {
                _actions[i].Undo();
            }
        }
        public void Redo()
        {
            foreach (IUndoableAction action in _actions)
            {
                action.Redo();
            }
        }
    }

    /// <summary>
    /// Action for alignment operations.
    /// </summary>
    public class AlignmentAction : IUndoableAction
    {
        private readonly List<BaseElement> _elements;
        private readonly List<Rectangle> _oldBounds;
        private readonly List<Rectangle> _newBounds;
        private readonly string _alignmentType;
        private readonly Action _refreshCanvas;

        public AlignmentAction(List<BaseElement> elements, List<Rectangle> oldBounds, string alignmentType, Action refreshCanvas)
        {
            _elements = [.. elements];
            _oldBounds = [.. oldBounds];
            _newBounds = [];
            _alignmentType = alignmentType;
            _refreshCanvas = refreshCanvas;

            // Capture new bounds after alignment
            foreach (BaseElement element in _elements)
            {
                _newBounds.Add(element.Bounds);
            }
        }

        public string Description => $"Align {_alignmentType}";

        public void Undo()
        {
            for (int i = 0; i < _elements.Count; i++)
            {
                _elements[i].Bounds = _oldBounds[i];
            }
            _refreshCanvas();
        }
        public void Redo()
        {
            for (int i = 0; i < _elements.Count; i++)
            {
                _elements[i].Bounds = _newBounds[i];
            }
            _refreshCanvas();
        }
    }

    /// <summary>
    /// Action for distributing elements.
    /// </summary>
    public class DistributeAction : IUndoableAction
    {
        private readonly List<BaseElement> _elements;
        private readonly List<Rectangle> _oldBounds;
        private readonly List<Rectangle> _newBounds;
        private readonly string _distributeType;
        private readonly Action _refreshCanvas;

        public DistributeAction(List<BaseElement> elements, List<Rectangle> oldBounds, string distributeType, Action refreshCanvas)
        {
            _elements = [.. elements];
            _oldBounds = [.. oldBounds];
            _newBounds = [];
            _distributeType = distributeType;
            _refreshCanvas = refreshCanvas;

            // Capture new bounds after distribution
            foreach (BaseElement element in _elements)
            {
                _newBounds.Add(element.Bounds);
            }
        }

        public string Description => $"Distribute {_distributeType}";

        public void Undo()
        {
            for (int i = 0; i < _elements.Count; i++)
            {
                _elements[i].Bounds = _oldBounds[i];
            }
            _refreshCanvas();
        }
        public void Redo()
        {
            for (int i = 0; i < _elements.Count; i++)
            {
                _elements[i].Bounds = _newBounds[i];
            }
            _refreshCanvas();
        }
    }

    /// <summary>
    /// Action for making elements the same size.
    /// </summary>
    public class SameSizeAction : IUndoableAction
    {
        private readonly List<BaseElement> _elements;
        private readonly List<Size> _oldSizes;
        private readonly List<Size> _newSizes;
        private readonly string _sizeType;
        private readonly Action _refreshCanvas;

        public SameSizeAction(List<BaseElement> elements, string sizeType, Action refreshCanvas)
        {
            _elements = [.. elements];
            _oldSizes = elements.Select(e => e.Bounds.Size).ToList();
            _sizeType = sizeType;
            _refreshCanvas = refreshCanvas;
            _newSizes = [];

            // Capture new sizes after resizing
            foreach (BaseElement element in _elements)
            {
                _newSizes.Add(element.Bounds.Size);
            }
        }

        public string Description => $"Make same {_sizeType}";

        public void Undo()
        {
            for (int i = 0; i < _elements.Count; i++)
            {
                Rectangle bounds = _elements[i].Bounds;
                bounds.Size = _oldSizes[i];
                _elements[i].Bounds = bounds;
            }
            _refreshCanvas();
        }
        public void Redo()
        {
            for (int i = 0; i < _elements.Count; i++)
            {
                Rectangle bounds = _elements[i].Bounds;
                bounds.Size = _newSizes[i];
                _elements[i].Bounds = bounds;
            }
            _refreshCanvas();
        }
    }

    /// <summary>
    /// Action for changing element layer order (bring to front / send to back).
    /// </summary>
    public class LayerOrderAction : IUndoableAction
    {
        private readonly Dictionary<BaseElement, int> _oldZOrders;
        private readonly Dictionary<BaseElement, int> _newZOrders;
        private readonly string _description;
        private readonly Action _refreshCanvas;

        public LayerOrderAction(List<BaseElement> allElements, string description, Action refreshCanvas)
        {
            _oldZOrders = [];
            _newZOrders = [];
            _description = description;
            _refreshCanvas = refreshCanvas;

            // Store old Z-orders for all elements
            foreach (BaseElement element in allElements)
            {
                _oldZOrders[element] = element.ZOrder;
            }
        }

        /// <summary>
        /// Captures the new Z-orders after the operation has been performed.
        /// This should be called after the layer order change is applied.
        /// </summary>
        public void CaptureNewState(List<BaseElement> allElements)
        {
            foreach (BaseElement element in allElements)
            {
                _newZOrders[element] = element.ZOrder;
            }
        }

        public string Description => _description;

        public void Undo()
        {
            foreach (KeyValuePair<BaseElement, int> kvp in _oldZOrders)
            {
                kvp.Key.ZOrder = kvp.Value;
            }
            _refreshCanvas();
        }

        public void Redo()
        {
            foreach (KeyValuePair<BaseElement, int> kvp in _newZOrders)
            {
                kvp.Key.ZOrder = kvp.Value;
            }
            _refreshCanvas();
        }
    }
}