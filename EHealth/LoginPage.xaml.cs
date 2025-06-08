using EHealthApp.Data;
using Microsoft.Maui.Storage; // Adaugă acest using pentru Preferences

namespace EHealthApp
{
    public partial class LoginPage : ContentPage
    {
        private readonly AppDatabase _database;

        // Constructor fără parametri pentru Shell
        public LoginPage() : this(App.Database) { }

        public LoginPage(AppDatabase database)
        {
            InitializeComponent();
            _database = database;
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowErrorMessage("Username and password are required.");
                return;
            }

            var user = await _database.GetUserByUsernameAsync(username);
            if (user == null || user.Password != password)
            {
                ShowErrorMessage("Invalid username or password.");
                return;
            }

            // Salvează userul logat în Preferences (persistent login)
            Preferences.Set("logged_user", user.Username);

            // Navighează la Home (MainPage) și resetează stiva de navigare
            await Shell.Current.GoToAsync("//MainPage");
        }

        private async void OnSignupButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("signup");
        }

        private void ShowErrorMessage(string message)
        {
            ErrorMessageLabel.Text = message;
            ErrorMessageLabel.IsVisible = true;
        }
    }
}