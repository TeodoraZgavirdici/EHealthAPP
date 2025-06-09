using EHealthApp.Data;
using Microsoft.Maui.Storage;
using Syncfusion.Licensing;

namespace EHealthApp;

public partial class App : Application
{
    // Singleton pentru AppDatabase
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

    // Constructor
    public App()
    {
        // Înregistrare licență Syncfusion (înlocuiește cu cheia ta reală)
        SyncfusionLicenseProvider.RegisterLicense("Mzg5OTcyM0AzMjM5MmUzMDJlMzAzYjMyMzkzYlpmVnoyTXZQOUJtb2hxRWt0S3FOQXFIdGxaMVh2dytSSXFBbkJLbXUvTzQ9\r\n\r\n");

        InitializeComponent();

        // LOGIN PERSISTENT:
        string loggedUser = Preferences.Get("logged_user", null);

        if (!string.IsNullOrEmpty(loggedUser))
        {
            // User logat, sari la AppShell (Home)
            MainPage = new AppShell();
        }
        else
        {
            // Niciun user logat, mergi la LoginPage
            MainPage = new LoginPage();
        }
    }
}