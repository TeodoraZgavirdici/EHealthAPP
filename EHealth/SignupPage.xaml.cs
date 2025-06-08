using EHealthApp.Data;

namespace EHealthApp
{
    public partial class SignupPage : ContentPage
    {
        private readonly AppDatabase _database;

        // Constructor fără parametri pentru Shell și DI
        public SignupPage() : this(App.Database) { }

        // Constructor cu bază de date pentru testare sau injecție explicită
        public SignupPage(AppDatabase database)
        {
            InitializeComponent();
            _database = database;
        }

        private async void OnSignupClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text?.Trim();
            string password = PasswordEntry.Text;
            string confirmPassword = ConfirmPasswordEntry.Text;

            // Exemplu: verificăm să fie completate toate câmpurile
            if (string.IsNullOrEmpty(email) ||
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

            var user = new User
            {
                Email = email,
                Password = password
            };

            await _database.SaveUserAsync(user);

            await DisplayAlert("Succes", "Cont creat cu succes!", "OK");
            await Shell.Current.GoToAsync("..");
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // Navighează către pagina de login (presupunând că ai o rută definită în Shell)
            await Shell.Current.GoToAsync("..");
        }
    }
}