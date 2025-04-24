using EHealthApp.Data; // Include AppDatabase
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace EHealthApp
{
    public partial class LoginPage : ContentPage
    {
        private readonly AppDatabase _database;

        public LoginPage(AppDatabase database)
        {
            InitializeComponent();
            _database = database; // Inițializează baza de date
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            // Preia input-ul utilizatorului
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;

            // Validare input
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowErrorMessage("Username and password are required.");
                return;
            }

            // Caută utilizatorul în baza de date
            var user = await _database.GetUserByUsernameAsync(username);
            if (user == null || user.Password != password)
            {
                ShowErrorMessage("Invalid username or password.");
                return;
            }

            // Navighează la pagina principală dacă autentificarea a reușit
            await Navigation.PushAsync(new MainPage());
        }

        private void ShowErrorMessage(string message)
        {
            // Afișează mesajul de eroare
            ErrorMessageLabel.Text = message;
            ErrorMessageLabel.IsVisible = true;
        }
    }
}
