namespace Argo_Books
{
    partial class ReceiptViewer_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            ControlsPanel = new Panel();
            Zoom_Label = new Label();
            Export_Button = new Guna.UI2.WinForms.Guna2Button();
            FitToWindow_Button = new Guna.UI2.WinForms.Guna2Button();
            ResetZoom_Button = new Guna.UI2.WinForms.Guna2Button();
            ZoomIn_Button = new Guna.UI2.WinForms.Guna2Button();
            ZoomOut_Button = new Guna.UI2.WinForms.Guna2Button();
            ImagePanel = new Panel();
            ReceiptPictureBox = new PictureBox();
            WebViewer = new Microsoft.Web.WebView2.WinForms.WebView2();
            ControlsPanel.SuspendLayout();
            ImagePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ReceiptPictureBox).BeginInit();
            SuspendLayout();
            // 
            // ControlsPanel
            // 
            ControlsPanel.Controls.Add(Zoom_Label);
            ControlsPanel.Controls.Add(Export_Button);
            ControlsPanel.Controls.Add(FitToWindow_Button);
            ControlsPanel.Controls.Add(ResetZoom_Button);
            ControlsPanel.Controls.Add(ZoomIn_Button);
            ControlsPanel.Controls.Add(ZoomOut_Button);
            ControlsPanel.Dock = DockStyle.Bottom;
            ControlsPanel.Location = new Point(0, 914);
            ControlsPanel.Margin = new Padding(4, 5, 4, 5);
            ControlsPanel.Name = "ControlsPanel";
            ControlsPanel.Size = new Size(1128, 80);
            ControlsPanel.TabIndex = 0;
            // 
            // Zoom_Label
            // 
            Zoom_Label.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Zoom_Label.AutoSize = true;
            Zoom_Label.Font = new Font("Segoe UI", 9F);
            Zoom_Label.Location = new Point(622, 28);
            Zoom_Label.Margin = new Padding(4, 0, 4, 0);
            Zoom_Label.Name = "Zoom_Label";
            Zoom_Label.Size = new Size(57, 25);
            Zoom_Label.TabIndex = 5;
            Zoom_Label.Text = "100%";
            // 
            // Export_Button
            // 
            Export_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Export_Button.BorderColor = Color.LightGray;
            Export_Button.BorderRadius = 2;
            Export_Button.BorderThickness = 1;
            Export_Button.CustomizableEdges = customizableEdges1;
            Export_Button.DisabledState.BorderColor = Color.DarkGray;
            Export_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            Export_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            Export_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            Export_Button.FillColor = Color.White;
            Export_Button.Font = new Font("Segoe UI", 9F);
            Export_Button.ForeColor = Color.Black;
            Export_Button.Location = new Point(915, 15);
            Export_Button.Margin = new Padding(4, 5, 4, 5);
            Export_Button.Name = "Export_Button";
            Export_Button.ShadowDecoration.CustomizableEdges = customizableEdges2;
            Export_Button.Size = new Size(200, 50);
            Export_Button.TabIndex = 4;
            Export_Button.Text = "Export";
            Export_Button.Click += Export_Button_Click;
            // 
            // FitToWindow_Button
            // 
            FitToWindow_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            FitToWindow_Button.BorderColor = Color.LightGray;
            FitToWindow_Button.BorderRadius = 2;
            FitToWindow_Button.BorderThickness = 1;
            FitToWindow_Button.CustomizableEdges = customizableEdges3;
            FitToWindow_Button.DisabledState.BorderColor = Color.DarkGray;
            FitToWindow_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            FitToWindow_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            FitToWindow_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            FitToWindow_Button.FillColor = Color.White;
            FitToWindow_Button.Font = new Font("Segoe UI", 9F);
            FitToWindow_Button.ForeColor = Color.Black;
            FitToWindow_Button.Location = new Point(400, 15);
            FitToWindow_Button.Margin = new Padding(4, 5, 4, 5);
            FitToWindow_Button.Name = "FitToWindow_Button";
            FitToWindow_Button.ShadowDecoration.CustomizableEdges = customizableEdges4;
            FitToWindow_Button.Size = new Size(200, 50);
            FitToWindow_Button.TabIndex = 3;
            FitToWindow_Button.Text = "Fit to window";
            FitToWindow_Button.Click += FitToWindow_Button_Click;
            // 
            // ResetZoom_Button
            // 
            ResetZoom_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ResetZoom_Button.BorderColor = Color.LightGray;
            ResetZoom_Button.BorderRadius = 2;
            ResetZoom_Button.BorderThickness = 1;
            ResetZoom_Button.CustomizableEdges = customizableEdges5;
            ResetZoom_Button.DisabledState.BorderColor = Color.DarkGray;
            ResetZoom_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            ResetZoom_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            ResetZoom_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            ResetZoom_Button.FillColor = Color.White;
            ResetZoom_Button.Font = new Font("Segoe UI", 9F);
            ResetZoom_Button.ForeColor = Color.Black;
            ResetZoom_Button.Location = new Point(271, 15);
            ResetZoom_Button.Margin = new Padding(4, 5, 4, 5);
            ResetZoom_Button.Name = "ResetZoom_Button";
            ResetZoom_Button.ShadowDecoration.CustomizableEdges = customizableEdges6;
            ResetZoom_Button.Size = new Size(115, 50);
            ResetZoom_Button.TabIndex = 2;
            ResetZoom_Button.Text = "100%";
            ResetZoom_Button.Click += ResetZoom_Button_Click;
            // 
            // ZoomIn_Button
            // 
            ZoomIn_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ZoomIn_Button.BorderColor = Color.LightGray;
            ZoomIn_Button.BorderRadius = 2;
            ZoomIn_Button.BorderThickness = 1;
            ZoomIn_Button.CustomizableEdges = customizableEdges7;
            ZoomIn_Button.DisabledState.BorderColor = Color.DarkGray;
            ZoomIn_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            ZoomIn_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            ZoomIn_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            ZoomIn_Button.FillColor = Color.White;
            ZoomIn_Button.Font = new Font("Segoe UI", 18F);
            ZoomIn_Button.ForeColor = Color.Black;
            ZoomIn_Button.Location = new Point(143, 15);
            ZoomIn_Button.Margin = new Padding(4, 5, 4, 5);
            ZoomIn_Button.Name = "ZoomIn_Button";
            ZoomIn_Button.ShadowDecoration.CustomizableEdges = customizableEdges8;
            ZoomIn_Button.Size = new Size(115, 50);
            ZoomIn_Button.TabIndex = 1;
            ZoomIn_Button.Text = "+";
            ZoomIn_Button.Click += ZoomIn_Button_Click;
            // 
            // ZoomOut_Button
            // 
            ZoomOut_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ZoomOut_Button.BorderColor = Color.LightGray;
            ZoomOut_Button.BorderRadius = 2;
            ZoomOut_Button.BorderThickness = 1;
            ZoomOut_Button.CustomizableEdges = customizableEdges9;
            ZoomOut_Button.DisabledState.BorderColor = Color.DarkGray;
            ZoomOut_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            ZoomOut_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            ZoomOut_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            ZoomOut_Button.FillColor = Color.White;
            ZoomOut_Button.Font = new Font("Segoe UI", 18F);
            ZoomOut_Button.ForeColor = Color.Black;
            ZoomOut_Button.Location = new Point(14, 15);
            ZoomOut_Button.Margin = new Padding(4, 5, 4, 5);
            ZoomOut_Button.Name = "ZoomOut_Button";
            ZoomOut_Button.ShadowDecoration.CustomizableEdges = customizableEdges10;
            ZoomOut_Button.Size = new Size(115, 50);
            ZoomOut_Button.TabIndex = 0;
            ZoomOut_Button.Text = "−";
            ZoomOut_Button.Click += ZoomOut_Button_Click;
            // 
            // ImagePanel
            // 
            ImagePanel.AutoScroll = true;
            ImagePanel.Controls.Add(ReceiptPictureBox);
            ImagePanel.Dock = DockStyle.Fill;
            ImagePanel.Location = new Point(0, 0);
            ImagePanel.Margin = new Padding(4, 5, 4, 5);
            ImagePanel.Name = "ImagePanel";
            ImagePanel.Size = new Size(1128, 914);
            ImagePanel.TabIndex = 1;
            // 
            // ReceiptPictureBox
            // 
            ReceiptPictureBox.Anchor = AnchorStyles.None;
            ReceiptPictureBox.BackColor = Color.Transparent;
            ReceiptPictureBox.Location = new Point(-7, 7);
            ReceiptPictureBox.Margin = new Padding(4, 5, 4, 5);
            ReceiptPictureBox.Name = "ReceiptPictureBox";
            ReceiptPictureBox.Size = new Size(143, 83);
            ReceiptPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            ReceiptPictureBox.TabIndex = 0;
            ReceiptPictureBox.TabStop = false;
            // 
            // WebView2r
            // 
            WebViewer.Dock = DockStyle.Fill;
            WebViewer.Location = new Point(0, 0);
            WebViewer.Margin = new Padding(4, 5, 4, 5);
            WebViewer.MinimumSize = new Size(29, 33);
            WebViewer.Name = "WebView2r";
            WebViewer.Size = new Size(1128, 914);
            WebViewer.TabIndex = 2;
            WebViewer.Visible = false;
            // 
            // ReceiptViewer_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1128, 994);
            Controls.Add(WebViewer);
            Controls.Add(ImagePanel);
            Controls.Add(ControlsPanel);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Margin = new Padding(4, 5, 4, 5);
            MinimumSize = new Size(970, 500);
            Name = "ReceiptViewer_Form";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Receipt Viewer";
            FormClosed += ReceiptViewer_Form_FormClosed;
            Shown += ReceiptViewer_Form_Shown;
            Resize += ReceiptViewer_Form_Resize;
            ControlsPanel.ResumeLayout(false);
            ControlsPanel.PerformLayout();
            ImagePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)ReceiptPictureBox).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel ControlsPanel;
        private Guna.UI2.WinForms.Guna2Button ZoomOut_Button;
        private Guna.UI2.WinForms.Guna2Button ZoomIn_Button;
        private Guna.UI2.WinForms.Guna2Button ResetZoom_Button;
        private Guna.UI2.WinForms.Guna2Button FitToWindow_Button;
        private Guna.UI2.WinForms.Guna2Button Export_Button;
        private System.Windows.Forms.Label Zoom_Label;
        private System.Windows.Forms.Panel ImagePanel;
        private System.Windows.Forms.PictureBox ReceiptPictureBox;
        private Microsoft.Web.WebView2.WinForms.WebView2 WebViewer;
    }
}