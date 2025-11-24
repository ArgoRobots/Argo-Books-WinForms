
namespace Argo_Books.Startup.Menus
{
    partial class GetStarted_Form
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
            ArgoSalesTracker_Label = new Label();
            OpenRecent_FlowLayoutPanel = new FlowLayoutPanel();
            CreateCompany_Button = new Guna.UI2.WinForms.Guna2Button();
            OpenRecent_Label = new Label();
            OpenCompany_Button = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // ArgoSalesTracker_Label
            // 
            ArgoSalesTracker_Label.AccessibleDescription = "";
            ArgoSalesTracker_Label.AutoSize = true;
            ArgoSalesTracker_Label.Font = new Font("Segoe UI Semibold", 22F, FontStyle.Bold);
            ArgoSalesTracker_Label.Location = new Point(30, 15);
            ArgoSalesTracker_Label.Name = "ArgoSalesTracker_Label";
            ArgoSalesTracker_Label.Size = new Size(390, 60);
            ArgoSalesTracker_Label.TabIndex = 0;
            ArgoSalesTracker_Label.Text = "Argo Books";
            // 
            // OpenRecent_FlowLayoutPanel
            // 
            OpenRecent_FlowLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            OpenRecent_FlowLayoutPanel.AutoScroll = true;
            OpenRecent_FlowLayoutPanel.BorderStyle = BorderStyle.FixedSingle;
            OpenRecent_FlowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            OpenRecent_FlowLayoutPanel.Location = new Point(45, 150);
            OpenRecent_FlowLayoutPanel.Name = "OpenRecent_FlowLayoutPanel";
            OpenRecent_FlowLayoutPanel.Size = new Size(390, 570);
            OpenRecent_FlowLayoutPanel.TabIndex = 0;
            OpenRecent_FlowLayoutPanel.WrapContents = false;
            OpenRecent_FlowLayoutPanel.Resize += OpenRecent_FlowLayoutPanel_Resize;
            // 
            // CreateCompany_Button
            // 
            CreateCompany_Button.BorderThickness = 1;
            CreateCompany_Button.CustomizableEdges = customizableEdges1;
            CreateCompany_Button.FillColor = Color.FromArgb(240, 240, 240);
            CreateCompany_Button.Font = new Font("Segoe UI", 12F);
            CreateCompany_Button.ForeColor = Color.Black;
            CreateCompany_Button.Image = Properties.Resources.CreateFileBlack;
            CreateCompany_Button.ImageAlign = HorizontalAlignment.Left;
            CreateCompany_Button.ImageOffset = new Point(5, 0);
            CreateCompany_Button.ImageSize = new Size(30, 30);
            CreateCompany_Button.Location = new Point(458, 150);
            CreateCompany_Button.Name = "CreateCompany_Button";
            CreateCompany_Button.ShadowDecoration.CustomizableEdges = customizableEdges2;
            CreateCompany_Button.Size = new Size(420, 105);
            CreateCompany_Button.TabIndex = 1;
            CreateCompany_Button.Text = "Create new company";
            CreateCompany_Button.TextAlign = HorizontalAlignment.Left;
            CreateCompany_Button.TextOffset = new Point(10, 0);
            CreateCompany_Button.Click += CreateNewCompany_Click;
            // 
            // OpenRecent_Label
            // 
            OpenRecent_Label.AutoSize = true;
            OpenRecent_Label.Font = new Font("Segoe UI", 12F);
            OpenRecent_Label.Location = new Point(45, 110);
            OpenRecent_Label.Name = "OpenRecent_Label";
            OpenRecent_Label.Size = new Size(147, 32);
            OpenRecent_Label.TabIndex = 0;
            OpenRecent_Label.Text = "Open recent";
            // 
            // OpenCompany_Button
            // 
            OpenCompany_Button.BorderThickness = 1;
            OpenCompany_Button.CustomizableEdges = customizableEdges3;
            OpenCompany_Button.FillColor = Color.FromArgb(240, 240, 240);
            OpenCompany_Button.Font = new Font("Segoe UI", 12F);
            OpenCompany_Button.ForeColor = Color.Black;
            OpenCompany_Button.Image = Properties.Resources.OpenFolderGray;
            OpenCompany_Button.ImageAlign = HorizontalAlignment.Left;
            OpenCompany_Button.ImageOffset = new Point(5, 0);
            OpenCompany_Button.ImageSize = new Size(30, 30);
            OpenCompany_Button.Location = new Point(458, 278);
            OpenCompany_Button.Name = "OpenCompany_Button";
            OpenCompany_Button.ShadowDecoration.CustomizableEdges = customizableEdges4;
            OpenCompany_Button.Size = new Size(420, 105);
            OpenCompany_Button.TabIndex = 2;
            OpenCompany_Button.Text = "Open company";
            OpenCompany_Button.TextAlign = HorizontalAlignment.Left;
            OpenCompany_Button.TextOffset = new Point(10, 0);
            OpenCompany_Button.Click += OpenCompany_Button_Click;
            // 
            // GetStarted_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(250, 250, 250);
            ClientSize = new Size(1320, 825);
            Controls.Add(OpenCompany_Button);
            Controls.Add(CreateCompany_Button);
            Controls.Add(ArgoSalesTracker_Label);
            Controls.Add(OpenRecent_FlowLayoutPanel);
            Controls.Add(OpenRecent_Label);
            FormBorderStyle = FormBorderStyle.None;
            Name = "GetStarted_Form";
            Shown += GetStarted_Form_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label ArgoSalesTracker_Label;
        private System.Windows.Forms.FlowLayoutPanel OpenRecent_FlowLayoutPanel;
        private System.Windows.Forms.Label OpenRecent_Label;
        private Guna.UI2.WinForms.Guna2Button CreateCompany_Button;
        private Guna.UI2.WinForms.Guna2Button OpenCompany_Button;
    }
}