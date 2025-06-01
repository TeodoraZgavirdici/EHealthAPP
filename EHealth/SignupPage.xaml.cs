using EHealthApp.Data;
using Microsoft.Maui.Controls;
using System;
using EHealthApp.Models;

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

            await _database.SaveUserAsync(user);

            await DisplayAlert("Succes", "Cont creat cu succes!", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }
}