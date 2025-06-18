using EHealthApp.Data;
using Microsoft.Maui.Storage;
using Syncfusion.Licensing;

namespace EHealthApp;

public partial class App : Application
{
    private static AppDatabase _database;

    public static AppDatabase Database
    {
        get
        {
            if (_database == null)
            {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EHealthApp.db3");
                _database = new AppDatabase(dbPath);
            }
            return _database;
        }
    }

    public App()
    {
        SyncfusionLicenseProvider.RegisterLicense("Mzg5OTcyM0AzMjM5MmUzMDJlMzAzYjMyMzkzYlpmVnoyTXZQOUJtb2hxRWt0S3FOQXFIdGxaMVh2dytSSXFBbkJLbXUvTzQ9\r\n\r\n");

        InitializeComponent();

        string loggedUser = Preferences.Get("logged_user", null);

        if (!string.IsNullOrEmpty(loggedUser))
        {
            MainPage = new NavigationPage(new MainPage());
        }
        else
        {
            MainPage = new NavigationPage(new LoginPage());
        }
    }
}
