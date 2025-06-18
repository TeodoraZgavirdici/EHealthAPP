using EHealthApp.Data;

namespace EHealthApp
{
    public partial class MainPage : ContentPage
    {
        private readonly AppDatabase _database;

        public MainPage()
        {
            InitializeComponent();
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "ehealth.db3");
            _database = new AppDatabase(dbPath);
        }

        public MainPage(AppDatabase database)
        {
            InitializeComponent();
            _database = database;
        }

        private async void OnProgramariMedicaleClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AppointmentsPage());
        }

        private async void OnDocumenteMedicaleClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MedicalDocumentsPage(_database));
        }

        private void Logout()
        {
            Preferences.Remove("logged_user");

            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }

        private void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            Logout();
        }
    }
}
