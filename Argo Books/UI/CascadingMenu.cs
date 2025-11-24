using Guna.UI2.WinForms;
using Argo_Books;
using Timer = System.Windows.Forms.Timer;

namespace Argo_Books.UI
{
    public static class CascadingMenu
    {
        // Properties
        private static Guna2Panel _menuToHide;
        private static readonly Timer _hideMenu_timer = new();

        // Methods
        public static void Init()
        {
            _hideMenu_timer.Interval = 800;
            _hideMenu_timer.Tick += HideMenu_timer_Tick;
        }
        private static void HideMenu_timer_Tick(object sender, EventArgs e)
        {
            _menuToHide.Parent?.Controls.Remove(_menuToHide);
            _hideMenu_timer.Enabled = false;
        }
        public static void OpenMenu()
        {
            _hideMenu_timer.Enabled = false;
        }
        public static void CloseMenu(object sender, EventArgs e)
        {
            Guna2Button btn = (Guna2Button)sender;
            _menuToHide = (Guna2Panel)btn.Tag;
            _hideMenu_timer.Enabled = false;
            _hideMenu_timer.Enabled = true;
        }
        public static void KeepMenuOpen()
        {
            _hideMenu_timer.Enabled = false;
        }
        public static bool IsThisACascadingMenu(Guna2Panel panel)
        {
            return panel == CustomControls.RecentlyOpenedMenu;
        }
        public static void RemoveCascadingMenus()
        {
            MainMenu_Form.Instance.Controls.Remove(CustomControls.RecentlyOpenedMenu);
        }
    }
}