using System;
using System.IO;
using EHealthApp.Data;
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
        SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NNaF1cWWhPYVtpR2Nbek5xdV9HZ1ZUQGYuP1ZhSXxWdkNjWH5fcXNQQmJVU0d9XUs=\r\n");

        InitializeComponent();
        MainPage = new AppShell();
    }
}