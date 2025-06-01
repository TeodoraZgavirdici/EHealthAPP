using Syncfusion.Maui.Calendar;
using System.Collections.ObjectModel;

namespace EHealthApp;

public partial class AppointmentsPage : ContentPage
{
    // Dicționar pentru programări grupate pe zi
    private readonly Dictionary<DateTime, ObservableCollection<AppointmentModel>> _programariPeZi = new();

    // Listă temporară pentru afișarea programărilor zilei selectate
    private readonly ObservableCollection<AppointmentModel> _programariAfisate = new();

    public AppointmentsPage()
    {
        InitializeComponent();
        // NU declara AppointmentsList sau MedicalCalendar aici! Se generează automat din XAML

        AppointmentsList.ItemsSource = _programariAfisate;
        ActualizeazaProgramari(DateTime.Today);
    }

    private void MedicalCalendar_SelectionChanged(object sender, CalendarSelectionChangedEventArgs e)
    {
        if (e.NewValue is DateTime ziSelectata)
            ActualizeazaProgramari(ziSelectata);
    }

    private void ActualizeazaProgramari(DateTime zi)
    {
        _programariAfisate.Clear();
        if (_programariPeZi.TryGetValue(zi.Date, out var lista))
        {
            foreach (var p in lista)
                _programariAfisate.Add(p);
        }
    }

    private async void OnAddAppointmentClicked(object sender, EventArgs e)
    {
        if (MedicalCalendar.SelectedDate is not DateTime ziSelectata)
        {
            await DisplayAlert("Eroare", "Selectează o zi în calendar!", "OK");
            return;
        }

        var ora = await DisplayPromptAsync("Oră", "Introduceți ora programării (ex: 15:30)");
        var descriere = await DisplayPromptAsync("Descriere", "Descrieți scopul programării");

        if (!string.IsNullOrWhiteSpace(ora) && !string.IsNullOrWhiteSpace(descriere))
        {
            if (!_programariPeZi.ContainsKey(ziSelectata.Date))
                _programariPeZi[ziSelectata.Date] = new ObservableCollection<AppointmentModel>();

            _programariPeZi[ziSelectata.Date].Add(new AppointmentModel
            {
                Ora = ora,
                Descriere = descriere
            });

            ActualizeazaProgramari(ziSelectata);
            await DisplayAlert("Succes", "Programare adăugată!", "OK");
        }
    }

    private async void OnViewAllAppointmentsClicked(object sender, EventArgs e)
    {
        var toate = _programariPeZi
            .OrderBy(p => p.Key)
            .SelectMany(p => p.Value.Select(x => $"{p.Key:dd.MM.yyyy} - {x.Ora} - {x.Descriere}"))
            .ToList();

        string mesaj = toate.Any()
            ? string.Join("\n", toate)
            : "Nu există programări.";

        await DisplayAlert("Toate programările", mesaj, "OK");
    }
}

public class AppointmentModel
{
    public string Ora { get; set; }
    public string Descriere { get; set; }
}
