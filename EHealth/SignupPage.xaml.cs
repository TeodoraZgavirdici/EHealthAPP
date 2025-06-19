using EHealthApp.Data;
using EHealthApp.Models;

namespace EHealthApp
{
    public partial class SignupPage : ContentPage
    {
        private readonly AppDatabase _database;

        public SignupPage() : this(App.Database) { }

        public SignupPage(AppDatabase database)
        {
            InitializeComponent();
            _database = database;
        }

        private async void OnSignupClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text?.Trim();
            string email = EmailEntry.Text?.Trim();
            string password = PasswordEntry.Text?.Trim();        
            string confirmPassword = ConfirmPasswordEntry.Text?.Trim(); 

            if (string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(confirmPassword))
            {
                await DisplayAlert("Eroare", "Completează toate câmpurile!", "OK");
                return;
            }

            if (password != confirmPassword)
            {
                await DisplayAlert("Eroare", "Parolele nu coincid!", "OK");
                return;
            }

            var existingEmail = await _database.GetUserByEmailAsync(email);
            if (existingEmail != null)
            {
                await DisplayAlert("Eroare", "Email-ul este deja folosit!", "OK");
                return;
            }

            var existingUser = await _database.GetUserByUsernameAsync(username);
            if (existingUser != null)
            {
                await DisplayAlert("Eroare", "Username-ul este deja folosit!", "OK");
                return;
            }

            var user = new User
            {
                Username = username,
                Email = email,
                Password = password
            };

            await _database.SaveUserAsync(user);

            await DisplayAlert("Succes", "Cont creat cu succes!", "OK");

            await Navigation.PopAsync();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}