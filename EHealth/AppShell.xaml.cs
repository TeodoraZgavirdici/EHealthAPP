using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace EHealthApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("signup", typeof(SignupPage));
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            Preferences.Remove("logged_user");

            Application.Current.MainPage = new LoginPage();
        }
    }
}
