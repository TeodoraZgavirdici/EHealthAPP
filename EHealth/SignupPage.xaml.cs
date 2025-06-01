using Microsoft.Maui.Controls;
using System;
using EHealthApp.Data; // Dacă ai nevoie de User și AppDatabase

namespace EHealth
{
    public partial class SignupPage : ContentPage
    {
        private readonly AppDatabase _database;

        public SignupPage()
        {
            InitializeComponent();
            _database = App.Database; // Sau injecție prin constructor, dacă folosești DI
        }

        private async void OnSignupClicked(object sender, EventArgs e)
        {
            string name = NameEntry.Text?.Trim();
            string email = EmailEntry.Text?.Trim();
            string password = PasswordEntry.Text;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageLabel.Text = "Completează toate câmpurile!";
                MessageLabel.IsVisible = true;
                return;
            }

            var existingUser = await _database.GetUserByUsernameAsync(name);
            if (existingUser != null)
            {
                MessageLabel.Text = "Username-ul este deja folosit!";
                MessageLabel.IsVisible = true;
                return;
            }

            var existingEmail = await _database.GetUserByEmailAsync(email);
            if (existingEmail != null)
            {
                MessageLabel.Text = "Email-ul este deja folosit!";
                MessageLabel.IsVisible = true;
                return;
            }

            var user = new User
            {
                Name = name,
                Email = email,
                Password = password
            };

            await _database.AddUserAsync(user);

            await DisplayAlert("Succes", "Cont creat cu succes!", "OK");
            await Navigation.PopAsync();
        }
    }
}