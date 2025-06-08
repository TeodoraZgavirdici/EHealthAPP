namespace EHealthApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnAddUserClicked(object sender, EventArgs e)
        {
            // Logica pentru adăugarea unui utilizator
            DisplayAlert("Add User", "Add User button clicked!", "OK");
        }

        private void OnShowUsersClicked(object sender, EventArgs e)
        {
            // Logica pentru afișarea utilizatorilor
            DisplayAlert("Show Users", "Show Users button clicked!", "OK");
        }
        private async void OnDocumenteMedicaleClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MedicalDocumentsPage());
        }
        private void OnCounterClicked(object sender, EventArgs e)
        {
            // Exemplu logică pentru butonul de incrementare
            DisplayAlert("Button Clicked", "Counter button clicked!", "OK");
        }
    }
}
