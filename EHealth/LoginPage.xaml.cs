using EHealthApp.Data;
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
        string email = EmailEntry.Text?.Trim();
        string password = PasswordEntry.Text?.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Eroare", "Completează toate câmpurile!", "OK");
            return;
        }

        var allUsers = await App.Database.GetUsersAsync();
        string debug = string.Join("\n", allUsers.Select(u => $"{u.Email} | '{u.Password}'"));
        await DisplayAlert("Utilizatori existenți", debug, "OK");

        var user = await App.Database.GetUserByEmailAndPassword(email, password);

        if (user != null)
        {
            Preferences.Set("logged_user", email);
            await DisplayAlert("Succes", $"Bine ai venit, {user.FullName ?? user.Email}!", "OK");
            Application.Current.MainPage = new NavigationPage(new MainPage());
        }
        else
        {
            await DisplayAlert("Eroare", "Email sau parolă incorectă!", "OK");
        }
    }

    private async void OnSignupButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SignupPage());
    }
}