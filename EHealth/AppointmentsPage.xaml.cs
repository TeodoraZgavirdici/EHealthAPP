using Syncfusion.Maui.Calendar;
using System.Collections.ObjectModel;

namespace EHealthApp;

public partial class AppointmentsPage : ContentPage
{
    public ObservableCollection<AppointmentModel> Programari { get; set; } = new();

    public AppointmentsPage()
    {
        InitializeComponent();
        AppointmentsList.ItemsSource = Programari;
        ActualizeazaProgramari(DateTime.Today);
    }

    private void MedicalCalendar_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.NewValue is DateTime ziSelectata)
            ActualizeazaProgramari(ziSelectata);
    }

    private void ActualizeazaProgramari(DateTime zi)
    {
        // Exemplu: Înlocuiește cu încărcare din baza de date locală/fișier/etc.
        Programari.Clear();

        // Exemplu static: dacă ziua e astăzi, afișează o programare
        if (zi.Date == DateTime.Today)
        {
            Programari.Add(new AppointmentModel
            {
                Ora = "14:00",
                Descriere = "Consultație la medicul de familie"
            });
        }
    }

    private async void OnAddAppointmentClicked(object sender, EventArgs e)
    {
        // Exemplu simplu de dialog pentru adăugare programare
        var ora = await DisplayPromptAsync("Oră", "Introduceți ora programării (ex: 15:30)");
        var descriere = await DisplayPromptAsync("Descriere", "Descrieți scopul programării");

        if (!string.IsNullOrWhiteSpace(ora) && !string.IsNullOrWhiteSpace(descriere))
        {
            Programari.Add(new AppointmentModel
            {
                Ora = ora,
                Descriere = descriere
            });
            await DisplayAlert("Succes", "Programare adăugată!", "OK");
        }
    }
}

public class AppointmentModel
{
    public string Ora { get; set; }
    public string Descriere { get; set; }
}