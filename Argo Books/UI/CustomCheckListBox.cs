using Argo_Books.DataClasses;
using Guna.UI2.WinForms;
using System.Collections;
using System.ComponentModel;

namespace Argo_Books.UI
{
    /// <summary>
    /// Custom control that provides CheckedListBox-like functionality using Guna2CustomCheckBox controls.
    /// Guna doesn't have a CheckedListBox, so this is a custom implementation.
    /// </summary>
    public class CustomCheckListBox : UserControl
    {
        // Properties
        public Panel ContainerPanel { get; private set; }
        private readonly Panel _bottomSpacer;
        private readonly List<CheckItem> _items = [];
        private bool _isUpdating = false;

        /// <summary>
        /// Gets the collection of checked item indices.
        /// </summary>
        public CheckedIndexCollection CheckedIndices { get; private set; }

        /// <summary>
        /// Gets the collection of checked items.
        /// </summary>
        public CheckedItemCollection CheckedItems { get; private set; }

        /// <summary>
        /// Gets the number of items in the control.
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// Gets the items in the control.
        /// </summary>
        public ItemCollection Items { get; private set; }

        /// <summary>
        /// Event that occurs when an item's check state changes.
        /// </summary>
        public event ItemCheckEventHandler ItemCheck;

        /// <summary>
        /// Gets or sets whether items can be checked by clicking on them.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CheckOnClick { get; set; } = true;

        /// <summary>
        /// Gets or sets the border style of the control.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new BorderStyle BorderStyle
        {
            get => base.BorderStyle;
            set
            {
                base.BorderStyle = value;
                ContainerPanel.BorderStyle = BorderStyle.None;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Padding Padding
        {
            get => new(
                ContainerPanel.Padding.Left,
                ContainerPanel.Padding.Top,
                ContainerPanel.Padding.Right,
                _bottomSpacer.Height
            );
            set
            {
                ContainerPanel.Padding = new Padding(value.Left, value.Top, value.Right, 0);
                _bottomSpacer.Height = value.Bottom;
            }
        }

        // Init.
        public CustomCheckListBox()
        {
            // Initialize the container panel
            ContainerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Padding = new Padding(10, 10, 10, 0)  // Bottom padding doesn't work here
            };
            Controls.Add(ContainerPanel);

            // Add bottom spacer for padding
            _bottomSpacer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 10,
                BackColor = Color.Transparent
            };
            ContainerPanel.Controls.Add(_bottomSpacer);

            // Set default properties on the control itself
            BackColor = Color.White;
            ForeColor = Color.Black;
            Font = new Font("Segoe UI", 9);
            Margin = new Padding(4, 5, 4, 5);
            CheckOnClick = true;
            BorderStyle = BorderStyle.None;
            MinimumSize = new Size(100, 100);

            // Initialize collections
            CheckedIndices = new CheckedIndexCollection(this);
            CheckedItems = new CheckedItemCollection(this);
            Items = new ItemCollection(this);
        }

        // Public methods
        /// <summary>
        /// Adds an item to the control with the specified checked state.
        /// </summary>
        public void Add(object? item, bool isChecked)
        {
            AddItem(item?.ToString(), isChecked);
        }

        /// <summary>
        /// Adds a section header separator to the control.
        /// </summary>
        public void AddSection(string sectionTitle)
        {
            // Create section header panel
            Panel sectionPanel = new()
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.Transparent,
                Padding = new Padding(5, 8, 5, 5)
            };

            // Create section label
            Label sectionLabel = new()
            {
                Text = sectionTitle,
                AutoSize = false,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate
            };

            sectionPanel.Controls.Add(sectionLabel);

            // Add to container
            ContainerPanel.Controls.Add(sectionPanel);
            sectionPanel.BringToFront();
        }

        /// <summary>
        /// Adds a small spacing separator.
        /// </summary>
        public void AddSpacer(int height = 10)
        {
            Panel spacer = new()
            {
                Dock = DockStyle.Top,
                Height = height,
                BackColor = Color.Transparent
            };

            ContainerPanel.Controls.Add(spacer);
            spacer.BringToFront();
        }

        /// <summary>
        /// Clears all items from the control.
        /// </summary>
        public void Clear()
        {
            ContainerPanel.Controls.Clear();
            _items.Clear();
        }

        /// <summary>
        /// Gets the checked state of the item at the specified index.
        /// </summary>
        public bool GetItemChecked(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                return _items[index].CheckBox.Checked;
            }

            return false;
        }

        /// <summary>
        /// Sets the checked state of the item at the specified index.
        /// </summary>
        public void SetItemChecked(int index, bool value)
        {
            if (index >= 0 && index < _items.Count && _items[index].CheckBox.Checked != value)
            {
                _isUpdating = true;
                try
                {
                    _items[index].CheckBox.Checked = value;
                }
                finally
                {
                    _isUpdating = false;
                }
            }
        }

        // Private methods
        private string AddItem(string? text, bool isChecked)
        {
            text ??= "";

            // Create container panel for this item
            Panel itemPanel = new()
            {
                Dock = DockStyle.Top,
                Height = 35,
                BackColor = Color.Transparent,
                Padding = new Padding(5, 5, 5, 5)
            };

            // Create check box 
            Guna2CustomCheckBox checkBox = new()
            {
                Checked = isChecked,
                Size = new Size(20, 20),
                Location = new Point(10, 8),
                Animated = true
            };

            // Set the custom appearance for checkbox
            checkBox.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            checkBox.CheckedState.BorderRadius = 2;
            checkBox.CheckedState.BorderThickness = 0;
            checkBox.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            checkBox.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            checkBox.UncheckedState.BorderRadius = 2;
            checkBox.UncheckedState.BorderThickness = 0;
            checkBox.UncheckedState.FillColor = Color.FromArgb(125, 137, 149);

            // Create label
            Label label = new()
            {
                Text = text,
                AutoSize = true,
                Location = new Point(40, 8),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.Black,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent,
                AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate
            };

            // Add controls to item panel
            itemPanel.Controls.Add(checkBox);
            itemPanel.Controls.Add(label);

            // Add to container panel
            ContainerPanel.Controls.Add(itemPanel);
            itemPanel.BringToFront();

            // Create item and add to collection
            int index = _items.Count;
            CheckItem item = new()
            {
                Text = text,
                CheckBox = checkBox,
                Label = label,
                Panel = itemPanel,
                Index = index
            };

            // Set up event handlers
            checkBox.CheckedChanged += (s, e) => OnCheckBoxCheckedChanged(item);
            label.Click += (s, e) => OnLabelClick(item);

            _items.Add(item);

            return text;
        }
        private void OnCheckBoxCheckedChanged(CheckItem item)
        {
            if (_isUpdating) { return; }

            // Notify via event
            CheckState newValue = item.CheckBox.Checked ? CheckState.Checked : CheckState.Unchecked;
            CheckState oldValue = newValue == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked;

            ItemCheckEventArgs args = new(item.Index, newValue, oldValue);
            ItemCheck?.Invoke(this, args);
        }
        private void OnLabelClick(CheckItem item)
        {
            if (CheckOnClick)
            {
                // Toggle checkbox when label is clicked
                item.CheckBox.Checked = !item.CheckBox.Checked;
            }
        }

        /// <summary>
        /// Represents an item in the control.
        /// </summary>
        private class CheckItem
        {
            public string Text { get; set; } = "";
            public Guna2CustomCheckBox CheckBox { get; set; } = null!;
            public Label Label { get; set; } = null!;
            public Panel Panel { get; set; } = null!;
            public int Index { get; set; }
        }

        /// <summary>
        /// Collection of checked indices.
        /// </summary>
        public class CheckedIndexCollection : IList
        {
            private readonly CustomCheckListBox _owner;
            internal CheckedIndexCollection(CustomCheckListBox owner)
            {
                _owner = owner;
            }

            public int Count
            {
                get
                {
                    int count = 0;
                    for (int i = 0; i < _owner._items.Count; i++)
                    {
                        if (_owner._items[i].CheckBox.Checked)
                        {
                            count++;
                        }
                    }
                    return count;
                }
            }
            public bool IsSynchronized => false;
            public object SyncRoot => this;
            public bool IsReadOnly => true;
            public bool IsFixedSize => false;
            object? IList.this[int index]
            {
                get
                {
                    if (index < 0 || index >= Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index));
                    }

                    int currentIndex = -1;
                    for (int i = 0; i < _owner._items.Count; i++)
                    {
                        if (_owner._items[i].CheckBox.Checked)
                        {
                            currentIndex++;
                            if (currentIndex == index)
                            {
                                return i;
                            }
                        }
                    }

                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                set => throw new NotSupportedException();
            }
            public int Add(object? value) => throw new NotSupportedException();
            public void Clear() => throw new NotSupportedException();
            public bool Contains(object? value) => IndexOf(value) != -1;
            public int IndexOf(object? value)
            {
                if (value is int index && index >= 0 && index < _owner._items.Count)
                {
                    return _owner._items[index].CheckBox.Checked ? index : -1;
                }

                return -1;
            }
            public void Insert(int index, object? value) => throw new NotSupportedException();
            public void Remove(object? value) => throw new NotSupportedException();
            public void RemoveAt(int index) => throw new NotSupportedException();
            public void CopyTo(Array array, int index)
            {
                ArgumentNullException.ThrowIfNull(array);
                ArgumentOutOfRangeException.ThrowIfNegative(index);

                if (array.Rank != 1)
                {
                    throw new ArgumentException("Array must be 1-dimensional");
                }

                if (array.Length - index < Count)
                {
                    throw new ArgumentException("Not enough space in array");
                }

                int arrayIndex = index;
                for (int i = 0; i < _owner._items.Count; i++)
                {
                    if (_owner._items[i].CheckBox.Checked)
                    {
                        array.SetValue(i, arrayIndex++);
                    }
                }
            }
            public IEnumerator GetEnumerator()
            {
                for (int i = 0; i < _owner._items.Count; i++)
                {
                    if (_owner._items[i].CheckBox.Checked)
                    {
                        yield return i;
                    }
                }
            }
            public int this[int index]
            {
                get
                {
                    if (index < 0 || index >= Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index));
                    }

                    int currentIndex = -1;
                    for (int i = 0; i < _owner._items.Count; i++)
                    {
                        if (_owner._items[i].CheckBox.Checked)
                        {
                            currentIndex++;
                            if (currentIndex == index)
                            {
                                return i;
                            }
                        }
                    }

                    throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        /// <summary>
        /// Collection of checked items.
        /// </summary>
        public class CheckedItemCollection : IList
        {
            private readonly CustomCheckListBox _owner;
            internal CheckedItemCollection(CustomCheckListBox owner)
            {
                _owner = owner;
            }

            public int Count
            {
                get
                {
                    int count = 0;
                    for (int i = 0; i < _owner._items.Count; i++)
                    {
                        if (_owner._items[i].CheckBox.Checked)
                        {
                            count++;
                        }
                    }
                    return count;
                }
            }
            public bool IsSynchronized => false;
            public object SyncRoot => this;
            public bool IsReadOnly => true;
            public bool IsFixedSize => false;
            public object? this[int index]
            {
                get
                {
                    if (index < 0 || index >= Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index));
                    }

                    int currentIndex = -1;
                    for (int i = 0; i < _owner._items.Count; i++)
                    {
                        if (_owner._items[i].CheckBox.Checked)
                        {
                            currentIndex++;
                            if (currentIndex == index)
                            {
                                return _owner._items[i].Text;
                            }
                        }
                    }

                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                set => throw new NotSupportedException();
            }

            public int Add(object? value) => throw new NotSupportedException();
            public void Clear() => throw new NotSupportedException();
            public bool Contains(object? value)
            {
                string itemText = value?.ToString() ?? "";
                for (int i = 0; i < _owner._items.Count; i++)
                {
                    if (_owner._items[i].CheckBox.Checked && _owner._items[i].Text == itemText)
                    {
                        return true;
                    }
                }

                return false;
            }
            public int IndexOf(object? value)
            {
                string itemText = value?.ToString() ?? "";
                for (int i = 0; i < _owner._items.Count; i++)
                {
                    if (_owner._items[i].CheckBox.Checked && _owner._items[i].Text == itemText)
                    {
                        return i;
                    }
                }

                return -1;
            }
            public void Insert(int index, object? value) => throw new NotSupportedException();
            public void Remove(object? value) => throw new NotSupportedException();
            public void RemoveAt(int index) => throw new NotSupportedException();
            public void CopyTo(Array array, int index)
            {
                ArgumentNullException.ThrowIfNull(array);
                ArgumentOutOfRangeException.ThrowIfNegative(index);

                if (array.Rank != 1)
                {
                    throw new ArgumentException("Array must be 1-dimensional");
                }

                if (array.Length - index < Count)
                {
                    throw new ArgumentException("Not enough space in array");
                }

                int arrayIndex = index;
                for (int i = 0; i < _owner._items.Count; i++)
                {
                    if (_owner._items[i].CheckBox.Checked)
                    {
                        array.SetValue(_owner._items[i].Text, arrayIndex++);
                    }
                }
            }
            public IEnumerator GetEnumerator()
            {
                for (int i = 0; i < _owner._items.Count; i++)
                {
                    if (_owner._items[i].CheckBox.Checked)
                    {
                        yield return _owner._items[i].Text;
                    }
                }
            }
        }

        /// <summary>
        /// Collection of all items.
        /// </summary>
        public class ItemCollection : IList
        {
            private readonly CustomCheckListBox _owner;
            internal ItemCollection(CustomCheckListBox owner)
            {
                _owner = owner;
            }

            public int Count => _owner._items.Count;
            public bool IsSynchronized => false;
            public object SyncRoot => this;
            public bool IsReadOnly => false;
            public bool IsFixedSize => false;

            public object? this[int index]
            {
                get
                {
                    if (index < 0 || index >= _owner._items.Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index));
                    }

                    return _owner._items[index].Text;
                }
                set
                {
                    if (index < 0 || index >= _owner._items.Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index));
                    }

                    string text = value?.ToString() ?? "";
                    _owner._items[index].Text = text;
                    _owner._items[index].Label.Text = text;
                }
            }
            public int Add(object? value)
            {
                _owner.AddItem(value?.ToString(), false);
                return _owner._items.Count - 1;
            }
            public void Add(object? item, bool isChecked)
            {
                _owner.AddItem(item?.ToString(), isChecked);
            }
            public void Clear() => _owner.Clear();
            public bool Contains(object? value)
            {
                string itemText = value?.ToString() ?? "";
                for (int i = 0; i < _owner._items.Count; i++)
                {
                    if (_owner._items[i].Text == itemText)
                    {
                        return true;
                    }
                }

                return false;
            }
            public int IndexOf(object? value)
            {
                string itemText = value?.ToString() ?? "";
                for (int i = 0; i < _owner._items.Count; i++)
                {
                    if (_owner._items[i].Text == itemText)
                    {
                        return i;
                    }
                }

                return -1;
            }
            public void Insert(int index, object? value)
            {
                throw new NotSupportedException("Insert operation is not supported");
            }
            public void Remove(object? value)
            {
                int index = IndexOf(value);
                if (index >= 0)
                {
                    RemoveAt(index);
                }
            }
            public void RemoveAt(int index)
            {
                if (index < 0 || index >= _owner._items.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                _owner.ContainerPanel.Controls.Remove(_owner._items[index].Panel);
                _owner._items.RemoveAt(index);

                // Update indices
                for (int i = index; i < _owner._items.Count; i++)
                {
                    _owner._items[i].Index = i;
                }
            }
            public void CopyTo(Array array, int index)
            {
                ArgumentNullException.ThrowIfNull(array);

                if (index >= 0)
                {
                    if (array.Rank != 1)
                    {
                        throw new ArgumentException("Array must be 1-dimensional");
                    }

                    if (array.Length - index < Count)
                    {
                        throw new ArgumentException("Not enough space in array");
                    }

                    for (int i = 0; i < _owner._items.Count; i++)
                    {
                        array.SetValue(_owner._items[i].Text, index + i);
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
            public IEnumerator GetEnumerator()
            {
                for (int i = 0; i < _owner._items.Count; i++)
                {
                    yield return _owner._items[i].Text;
                }
            }
        }
    }
}