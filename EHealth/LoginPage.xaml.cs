using EHealth.Services;
using Microsoft.Maui.Storage;

namespace EHealthApp;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        string email = UsernameEntry.Text?.Trim();
        string password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Eroare", "Completează toate câmpurile!", "OK");
            return;
        }

        var user = await App.Database.GetUserByEmailAndPassword(email, password);

        if (user != null)
        {
            Preferences.Set("logged_user", email);

            var notificationService = DependencyService.Get<ILocalNotificationService>();
            string displayName = !string.IsNullOrEmpty(user.FullName) ? user.FullName : user.Username;
            notificationService?.ScheduleNotification(DateTime.Now, "Autentificare reușită", $"Bine ai venit, {displayName}!");

            Application.Current.MainPage = new NavigationPage(new MainPage());
        }
        else
        {
            await DisplayAlert("Eroare", "Email sau parola incorecte!", "OK");
        }
    }

    private async void OnSignupButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SignupPage());
    }
}
