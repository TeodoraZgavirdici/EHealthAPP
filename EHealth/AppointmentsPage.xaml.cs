using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using EHealthApp.Data;
using EHealthApp.Models;

namespace EHealthApp
{
    public partial class AppointmentsPage : ContentPage
    {
        private readonly AppDatabase _database;

        public AppointmentsPage(AppDatabase database)
        {
            InitializeComponent();
            _database = database;

            // Actualizează lista programărilor
            LoadAppointments();
        }

        // Încarcă programările din baza de date
        private async void LoadAppointments()
        {
            var appointments = await _database.GetAppointmentsAsync();
            AppointmentsList.ItemsSource = appointments;
        }

        // Adaugă o programare nouă
        private async void OnAddAppointmentClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleEntry.Text)) // Folosim TitleEntry în loc de NameEntry
            {
                await DisplayAlert("Eroare", "Introdu un titlu valid!", "OK");
                return;
            }

            var appointment = new Appointment
            {
                Title = TitleEntry.Text, // Folosim Title
                AppointmentDate = DatePicker.Date   // Modificăm proprietatea "AppointmentDate" cu "Date"
            };

            await _database.SaveAppointmentAsync(appointment);

            // Resetează câmpurile
            TitleEntry.Text = string.Empty; // Resetează TitleEntry în loc de NameEntry
            DatePicker.Date = DateTime.Now;

            // Reîncarcă lista
            LoadAppointments();
        }

        // Șterge o programare
        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var id = (int)((Button)sender).CommandParameter;
            var appointment = await _database.GetAppointmentByIdAsync(id);

            if (appointment != null)
            {
                await _database.DeleteAppointmentAsync(appointment);
                LoadAppointments();
            }
        }
    }
}
