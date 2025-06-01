using System;
using Microsoft.Maui.Controls;

namespace EHealth
{
    public partial class SignupPage : ContentPage
    {
        public SignupPage()
        {
            InitializeComponent();
        }

        private async void OnSignupClicked(object sender, EventArgs e)
        {
            string name = NameEntry.Text?.Trim();
            string email = EmailEntry.Text?.Trim();
            string password = PasswordEntry.Text;

            // Validare simplă:
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageLabel.Text = "Completează toate câmpurile!";
                MessageLabel.IsVisible = true;
                return;
            }

            // TODO: Adaugă aici logica de salvare (API/backend sau local)
            // Exemplu: afișare mesaj succes
            await DisplayAlert("Succes", "Cont creat cu succes!", "OK");
            await Navigation.PopAsync();
        }
    }
}