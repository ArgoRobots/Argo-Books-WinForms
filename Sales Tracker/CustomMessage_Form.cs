using Guna.UI2.WinForms;
using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.Language;
using Sales_Tracker.Properties;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker
{
    /// <summary>
    /// Custom message box form for displaying dialogs and user prompts.
    /// </summary>
    public partial class CustomMessage_Form : BaseForm
    {
        // Getter
        public CustomMessageBoxResult Result { get; private set; }

        // Init.
        public CustomMessage_Form() : this("", "", CustomMessageBoxIcon.Info, CustomMessageBoxButtons.Ok) { }  // This is needed for TranslationGenerator.GenerateAllLanguageTranslationFiles()
        public CustomMessage_Form(string title, string message, CustomMessageBoxIcon icon, CustomMessageBoxButtons buttons)
        {
            InitializeComponent();

            ThemeManager.SetThemeForForm(this);
            SetMessageBox(title, message, icon, buttons);
            SetAccessibleDescriptions();
            LanguageManager.UpdateLanguageForControl(this);
            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void SetAccessibleDescriptions()
        {
            Message_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
        }

        // Form event handlers
        private void CustomMessage_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
        }
        private void CustomMessage_Form_Resize(object sender, EventArgs e)
        {
            SetMessageLabelMaxWidth();
        }

        // Methods
        private void SetMessageBox(string title, string message, CustomMessageBoxIcon icon, CustomMessageBoxButtons buttons)
        {
            if (title == "" && message == "")
            {
                return;
            }

            string translatedTitle = LanguageManager.TranslateString(title);
            string translatedMessage = LanguageManager.TranslateString(message);

            // Set text
            Text = translatedTitle;
            Message_Label.Text = translatedMessage;
            Message_Label.LinkArea = new LinkArea(CustomMessageBoxVariables.LinkStart, CustomMessageBoxVariables.LinkLength);
            Message_Label.LinkColor = CustomColors.LinkColor;
            Message_Label.ActiveLinkColor = CustomColors.LinkColor;

            CustomMessageBoxVariables.Reset();
            Controls.Add(Icon_PictureBox);

            SetMessageLabelMaxWidth();
            Height = 150 + Message_Label.Height;

            // Set icon
            switch (icon)
            {
                case CustomMessageBoxIcon.Question:
                    Icon_PictureBox.BackgroundImage = Resources.HelpGray;
                    break;
                case CustomMessageBoxIcon.Exclamation:
                    Icon_PictureBox.BackgroundImage = Resources.ExclamationMark;
                    break;
                case CustomMessageBoxIcon.Error:
                    Icon_PictureBox.BackgroundImage = Resources.Error;
                    break;
                case CustomMessageBoxIcon.Info:
                    Icon_PictureBox.BackgroundImage = Resources.Info;
                    break;
                case CustomMessageBoxIcon.Success:
                    Icon_PictureBox.BackgroundImage = Resources.Checkmark;
                    break;
                case CustomMessageBoxIcon.None:
                    Icon_PictureBox.BackgroundImage = null;
                    Controls.Remove(Icon_PictureBox);
                    break;
            }

            // Clear buttons
            Controls.Remove(Ok_Button);
            Controls.Remove(Yes_Button);
            Controls.Remove(No_Button);
            Controls.Remove(Cancel_Button);
            Controls.Remove(Save_Button);
            Controls.Remove(DontSave_Button);
            Controls.Remove(Retry_Button);
            Controls.Remove(Skip_Button);
            Controls.Remove(YesAll_Button);
            Controls.Remove(NoAll_Button);

            // Set buttons
            byte buttonSpace = 35;
            switch (buttons)
            {
                case CustomMessageBoxButtons.YesNo:
                    No_Button.Left = Width - No_Button.Width - buttonSpace;
                    Yes_Button.Left = No_Button.Left - Yes_Button.Width - CustomControls.SpaceBetweenControls;
                    Controls.Add(Yes_Button);
                    Controls.Add(No_Button);
                    Yes_Button.Focus();
                    break;

                case CustomMessageBoxButtons.YesNoCancel:
                    Cancel_Button.Left = Width - Cancel_Button.Width - buttonSpace;
                    No_Button.Left = Cancel_Button.Left - No_Button.Width - CustomControls.SpaceBetweenControls;
                    Yes_Button.Left = No_Button.Left - Yes_Button.Width - CustomControls.SpaceBetweenControls;
                    Controls.Add(Yes_Button);
                    Controls.Add(No_Button);
                    Controls.Add(Cancel_Button);
                    Yes_Button.Focus();
                    break;

                case CustomMessageBoxButtons.YesNoAll:
                    NoAll_Button.Left = Width - NoAll_Button.Width - buttonSpace;
                    No_Button.Left = NoAll_Button.Left - No_Button.Width - CustomControls.SpaceBetweenControls;
                    YesAll_Button.Left = No_Button.Left - YesAll_Button.Width - CustomControls.SpaceBetweenControls;
                    Yes_Button.Left = YesAll_Button.Left - Yes_Button.Width - CustomControls.SpaceBetweenControls;
                    Controls.Add(Yes_Button);
                    Controls.Add(YesAll_Button);
                    Controls.Add(No_Button);
                    Controls.Add(NoAll_Button);
                    Yes_Button.Focus();
                    break;

                case CustomMessageBoxButtons.Ok:
                    Ok_Button.Left = Width - Ok_Button.Width - buttonSpace;
                    Controls.Add(Ok_Button);
                    Ok_Button.Focus();
                    break;
                case CustomMessageBoxButtons.OkCancel:
                    Cancel_Button.Left = Width - Cancel_Button.Width - buttonSpace;
                    Ok_Button.Left = Cancel_Button.Left - Ok_Button.Width - CustomControls.SpaceBetweenControls;
                    Controls.Add(Ok_Button);
                    Controls.Add(Cancel_Button);
                    Ok_Button.Focus();
                    break;
                case CustomMessageBoxButtons.SaveDontSaveCancel:
                    Cancel_Button.Left = Width - Cancel_Button.Width - buttonSpace;
                    DontSave_Button.Left = Cancel_Button.Left - DontSave_Button.Width - CustomControls.SpaceBetweenControls;
                    Save_Button.Left = DontSave_Button.Left - Save_Button.Width - CustomControls.SpaceBetweenControls;
                    Controls.Add(Save_Button);
                    Controls.Add(DontSave_Button);
                    Controls.Add(Cancel_Button);
                    Save_Button.Focus();

                    ShowThingsThatHaveChanged();
                    break;

                case CustomMessageBoxButtons.RetryCancel:
                    Cancel_Button.Left = Width - Cancel_Button.Width - buttonSpace;
                    Retry_Button.Left = Cancel_Button.Left - Retry_Button.Width - CustomControls.SpaceBetweenControls;
                    Controls.Add(Retry_Button);
                    Controls.Add(Cancel_Button);
                    Retry_Button.Focus();
                    break;

                case CustomMessageBoxButtons.SkipCancel:
                    Cancel_Button.Left = Width - Cancel_Button.Width - buttonSpace;
                    Skip_Button.Left = Cancel_Button.Left - Skip_Button.Width - CustomControls.SpaceBetweenControls;
                    Controls.Add(Skip_Button);
                    Controls.Add(Cancel_Button);
                    Skip_Button.Focus();
                    break;

                case CustomMessageBoxButtons.SkipRetryCancel:
                    Cancel_Button.Left = Width - Cancel_Button.Width - buttonSpace;
                    Retry_Button.Left = Cancel_Button.Left - Retry_Button.Width - CustomControls.SpaceBetweenControls;
                    Skip_Button.Left = Retry_Button.Left - Skip_Button.Width - CustomControls.SpaceBetweenControls;
                    Controls.Add(Skip_Button);
                    Controls.Add(Retry_Button);
                    Controls.Add(Cancel_Button);
                    Skip_Button.Focus();
                    break;
            }

            MinimumSize = new Size(MinimumSize.Width, Height);
        }

        private Panel changed_Panel;
        private Guna2Panel changedBackground_Panel;
        private void AdjustFormHeight(int contentHeight)
        {
            int requiredHeight = Math.Min(contentHeight, MaximumSize.Height);

            if (contentHeight > MaximumSize.Height - 150)
            {
                Height = MaximumSize.Height;
                changedBackground_Panel.Height = ClientSize.Height - 150;
                changed_Panel.Height = ClientSize.Height - 10;
                FormBorderStyle = FormBorderStyle.SizableToolWindow;
                changed_Panel.AutoScroll = true;
            }
            else
            {
                Height = requiredHeight + 200;
                changedBackground_Panel.Height = ClientSize.Height - 150;
                changed_Panel.Height = ClientSize.Height - 150;
                FormBorderStyle = FormBorderStyle.FixedToolWindow;
                changed_Panel.AutoScroll = false;
            }
        }
        private void SetMessageLabelMaxWidth()
        {
            Message_Label.MaximumSize = new Size(Width - Message_Label.Left - 40, 0);
        }

        // Things that have changed
        private void ShowThingsThatHaveChanged()
        {
            // Construct panels
            changedBackground_Panel = new()
            {
                Top = 50,
                Size = new Size(450, 70),
                BackColor = CustomColors.MainBackground,
                BorderColor = CustomColors.ControlBorder,
                BorderThickness = 1,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom
            };

            changed_Panel = new()
            {
                Location = new Point(1, 1),
                Size = new Size(448, 68),
                BackColor = CustomColors.MainBackground,
                AutoScroll = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom
            };
            changedBackground_Panel.Left = (ClientSize.Width - changed_Panel.Width) / 2;  // Center
            changedBackground_Panel.Controls.Add(changed_Panel);
            ThemeManager.CustomizeScrollBar(changed_Panel);

            Controls.Add(changedBackground_Panel);
            Controls.Remove(Icon_PictureBox);
            Controls.Remove(Message_Label);

            // Construct new message label
            Label label = new()
            {
                Top = 10,
                Text = Message_Label.Text,
                Name = "SaveChanges_Label",
                AutoSize = true,
                AccessibleDescription = AccessibleDescriptionManager.DoNotCache,
                Anchor = AnchorStyles.Top,
                Font = new Font("Segoe UI", 11),
                ForeColor = CustomColors.Text,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(label);
            label.Left = (ClientSize.Width - label.Width) / 2;

            // Add list of things that have changed to panel
            int top = 5;

            top = AddListForThingsChanged("General", MainMenu_Form.ThingsThatHaveChangedInFile, top);
            top = AddListForThingsChanged("Settings", MainMenu_Form.SettingsThatHaveChangedInFile, top);
            top = AddListForThingsChanged("Accountants", Accountants_Form.ThingsThatHaveChangedInFile, top);
            top = AddListForThingsChanged("Categories", Categories_Form.ThingsThatHaveChangedInFile, top);
            top = AddListForThingsChanged("Companies", Companies_Form.ThingsThatHaveChangedInFile, top);
            top = AddListForThingsChanged("Purchases", AddPurchase_Form.ThingsThatHaveChangedInFile, top);
            top = AddListForThingsChanged("Sales", AddSale_Form.ThingsThatHaveChangedInFile, top);
            top = AddListForThingsChanged("Products", Products_Form.ThingsThatHaveChangedInFile, top);
            top = AddListForThingsChanged("Rentals", AddRentalItem_Form.ThingsThatHaveChangedInFile, top);
            top = AddListForThingsChanged("Customers", Customers_Form.ThingsThatHaveChangedInFile, top);

            // This is a dummy control to add extra space at the end, even when the panel is scrollable
            changed_Panel.Controls.Add(new Control()
            {
                Size = new Size(1, 15),
                Left = 1,
                Top = top,
            });
            top += 20;

            AdjustFormHeight(top);
        }
        private int AddListForThingsChanged(string title, List<string> list, int top)
        {
            if (list.Count == 0) { return top; }

            Message_Label.Location = new Point((ClientSize.Width - Message_Label.Width) / 2, 0);

            Label label = new()
            {
                Text = title,
                AccessibleDescription = AccessibleDescriptionManager.DoNotCache,
                AutoSize = true,
                Location = new Point(10, top),
                Font = new Font("Segoe UI", 11),
                ForeColor = CustomColors.Text,
                Cursor = Cursors.Arrow
            };
            changed_Panel.Controls.Add(label);
            top += label.Height + 5;

            foreach (string item in list)
            {
                label = new()
                {
                    Text = item,
                    AccessibleDescription = AccessibleDescriptionManager.DoNotCache,
                    AutoSize = true,
                    Location = new Point(25, top),
                    Font = new Font("Segoe UI", 11),
                    ForeColor = CustomColors.Text,
                    Cursor = Cursors.Arrow
                };
                changed_Panel.Controls.Add(label);
                top += label.Height + 5;
            }

            return top;
        }
        private static void AddThingThatHasChanged(List<string> list, string thing)
        {
            bool isChanged = false;

            if (!list.Contains(thing))
            {
                list.Add(thing);
                isChanged = true;
            }

            DataFileManager.SetValue(AppDataSettings.ChangesMade, isChanged.ToString());
        }

        /// <summary>
        /// Log level index: 0 = [Error], 1 = [Debug], 2 = [General], 3 = [Product manager], 4 = [Password manager].
        /// </summary>
        public static void AddThingThatHasChangedAndLogMessage(List<string> list, byte logIndex, string thing)
        {
            AddThingThatHasChanged(list, thing);
            Log.Write(logIndex, thing);
        }

        // Event handlers
        private void No_Button_Click(object sender, EventArgs e)
        {
            Result = CustomMessageBoxResult.No;
            Close();
        }
        private void Yes_Button_Click(object sender, EventArgs e)
        {
            Result = CustomMessageBoxResult.Yes;
            Close();
        }
        private void YesAll_Button_Click(object sender, EventArgs e)
        {
            Result = CustomMessageBoxResult.YesAll;
            Close();
        }
        private void NoAll_Button_Click(object sender, EventArgs e)
        {
            Result = CustomMessageBoxResult.NoAll;
            Close();
        }
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            Result = CustomMessageBoxResult.Cancel;
            Close();
        }
        private void Ok_Button_Click(object sender, EventArgs e)
        {
            Result = CustomMessageBoxResult.Ok;
            Close();
        }
        private void Save_Button_Click(object sender, EventArgs e)
        {
            Result = CustomMessageBoxResult.Save;
            Close();
        }
        private void DontSave_Button_Click(object sender, EventArgs e)
        {
            Result = CustomMessageBoxResult.DontSave;
            Close();
        }
        private void Retry_Button_Click(object sender, EventArgs e)
        {
            Result = CustomMessageBoxResult.Retry;
            Close();
        }
        private void Skip_Button_Click(object sender, EventArgs e)
        {
            Result = CustomMessageBoxResult.Skip;
            Close();
        }
        private void Message_Label_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Tools.OpenLink(CustomMessageBoxVariables.Link);
        }
    }

    public enum CustomMessageBoxIcon
    {
        Question,
        Exclamation,
        Error,
        Info,
        Success,
        None,
        Warning
    }
    public enum CustomMessageBoxButtons
    {
        YesNo,
        YesNoCancel,
        YesNoAll,
        Ok,
        OkCancel,
        SaveDontSaveCancel,
        RetryCancel,
        SkipCancel,
        SkipRetryCancel,
        OK
    }
    public enum CustomMessageBoxResult
    {
        Ok,
        Cancel,
        Yes,
        No,
        YesAll,
        NoAll,
        Save,
        DontSave,
        Retry,
        Skip,
        None
    }

    public static class CustomMessageBox
    {
        private static bool _isMessageBoxShowing = false;
        public static CustomMessageBoxResult Show(string title, string message, CustomMessageBoxIcon icon, CustomMessageBoxButtons buttons)
        {
            // Only allow one MessageBox to appear at a time
            if (_isMessageBoxShowing)
            {
                return CustomMessageBoxResult.None;
            }

            _isMessageBoxShowing = true;

            try
            {
                // Check if we have any open forms and need to invoke
                if (Application.OpenForms.Count > 0)
                {
                    return Application.OpenForms[0].InvokeIfRequired(() =>
                    {
                        using CustomMessage_Form form = new(title, message, icon, buttons);
                        form.ShowDialog();
                        return form.Result;
                    });
                }
                else
                {
                    // No forms available, create directly
                    using CustomMessage_Form form = new(title, message, icon, buttons);
                    form.ShowDialog();
                    return form.Result;
                }
            }
            finally
            {
                _isMessageBoxShowing = false;
            }
        }
        public static CustomMessageBoxResult ShowWithFormat(string titleTemplate, string messageTemplate,
            CustomMessageBoxIcon icon, CustomMessageBoxButtons buttons, params object[] args)
        {
            // Format the title
            string translatedTitle = LanguageManager.TranslateString(titleTemplate);
            string formattedTitle = translatedTitle != titleTemplate && translatedTitle.Contains('{')
                ? string.Format(translatedTitle, args)
                : string.Format(titleTemplate, args);

            // Format the message
            string translatedMessage = LanguageManager.TranslateString(messageTemplate);
            string formattedMessage = translatedMessage != messageTemplate && translatedMessage.Contains('{')
                ? string.Format(translatedMessage, args)
                : string.Format(messageTemplate, args);

            return Show(formattedTitle, formattedMessage, icon, buttons);
        }
    }

    public static class CustomMessageBoxVariables
    {
        public static string Link { get; set; }
        public static int LinkStart { get; set; }
        public static int LinkLength { get; set; }

        public static void Reset()
        {
            LinkStart = 0;
            LinkLength = 0;
        }
    }
}