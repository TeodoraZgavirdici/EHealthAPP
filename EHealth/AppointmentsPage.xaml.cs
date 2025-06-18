using EHealth.Services;
using EHealthApp.Data;
using EHealthApp.Models;
using Microsoft.Maui.Controls;
using Syncfusion.Maui.Calendar;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace EHealthApp;

public partial class AppointmentsPage
{
    private readonly AppDatabase _database;
    private readonly ObservableCollection<Appointment> _programariAfisate = new();

    public AppointmentsPage()
    {
        InitializeComponent();
        _database = App.Database;
        AppointmentsList.ItemsSource = _programariAfisate;
        LoadAppointments(DateTime.Today);
    }

    private void MedicalCalendar_SelectionChanged(object sender, CalendarSelectionChangedEventArgs e)
    {
        if (e.NewValue is DateTime ziSelectata)
            LoadAppointments(ziSelectata);
    }

    private async void LoadAppointments(DateTime zi)
    {
        _programariAfisate.Clear();
        var lista = await _database.GetAppointmentsByDateAsync(zi);
        foreach (var p in lista)
            _programariAfisate.Add(p);
    }

    private async void OnAddAppointmentClicked(object sender, EventArgs e)
    {
        if (MedicalCalendar.SelectedDate is not DateTime ziSelectata)
        {
            await DisplayAlert("Eroare", "Selectează o zi în calendar!", "OK");
            return;
        }

        var ora = await DisplayPromptAsync("Oră", "Introduceți ora programării (ex: 15:30)");
        var titlu = await DisplayPromptAsync("Titlu", "Introduceți titlul programării");
        var descriere = await DisplayPromptAsync("Descriere", "Descrieți scopul programării");

        if (!string.IsNullOrWhiteSpace(ora) && !string.IsNullOrWhiteSpace(titlu) && !string.IsNullOrWhiteSpace(descriere))
        {
            if (!TimeSpan.TryParse(ora, out var timespan))
            {
                await DisplayAlert("Eroare", "Ora nu este validă!", "OK");
                return;
            }

            var dataOraProgramare = ziSelectata.Date + timespan;

            var appointment = new Appointment
            {
                AppointmentDate = dataOraProgramare,
                Title = titlu,
                Description = descriere
            };

            await _database.SaveAppointmentAsync(appointment);

            var notificationService = DependencyService.Get<ILocalNotificationService>();

            // NOTIFICARE cu 24 de ore înainte
            var notify24h = dataOraProgramare.AddHours(-24);
            if (notify24h > DateTime.Now)
            {
                notificationService?.ScheduleNotification(
                    notify24h,
                    $"Reminder programare: {titlu}",
                    $"Mâine la {dataOraProgramare:HH:mm}: {descriere}");
            }

            // NOTIFICARE cu 30 de minute înainte
            var notify30min = dataOraProgramare.AddMinutes(-30);
            if (notify30min > DateTime.Now)
            {
                notificationService?.ScheduleNotification(
                    notify30min,
                    $"Reminder programare: {titlu}",
                    $"Ai programare în 30 de minute: {descriere}");
            }

            // Refresh lista și alertă confirmare
            LoadAppointments(ziSelectata);
            await DisplayAlert(
                "Succes",
                "Programare adăugată! Vei primi notificări: una cu 24h înainte, alta cu 30 minute înainte (dacă există timp rămas până atunci).",
                "OK"
            );
        }
    }

    private async void OnViewAllAppointmentsClicked(object sender, EventArgs e)
    {
        var toate = await _database.GetAppointmentsAsync();
        var lista = toate
            .OrderBy(p => p.AppointmentDate)
            .Select(x => $"{x.AppointmentDate:dd.MM.yyyy} - {x.AppointmentDate:HH:mm} - {x.Title} - {x.Description}")
            .ToList();

        string mesaj = lista.Any()
            ? string.Join("\n", lista)
            : "Nu există programări.";

        await DisplayAlert("Toate programările", mesaj, "OK");
    }
}
