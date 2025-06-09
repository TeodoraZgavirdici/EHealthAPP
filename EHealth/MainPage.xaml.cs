using EHealthApp.Data;

namespace EHealthApp
{
    public partial class MainPage : ContentPage
    {
        private readonly AppDatabase _database;

        // Constructor fără parametri (necesar pentru XAML)
        public MainPage()
        {
            InitializeComponent();

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "ehealth.db3");
            _database = new AppDatabase(dbPath);
        }

        // Constructor cu parametrul AppDatabase (poți să-l păstrezi pentru teste sau altă folosire)
        public MainPage(AppDatabase database)
        {
            InitializeComponent();
            _database = database;
        }

        private void OnAddUserClicked(object sender, EventArgs e)
        {
            DisplayAlert("Add User", "Add User button clicked!", "OK");
        }

        private void OnShowUsersClicked(object sender, EventArgs e)
        {
            DisplayAlert("Show Users", "Show Users button clicked!", "OK");
        }

        private async void OnDocumenteMedicaleClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MedicalDocumentsPage(_database));
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            DisplayAlert("Button Clicked", "Counter button clicked!", "OK");
        }
    }
}
