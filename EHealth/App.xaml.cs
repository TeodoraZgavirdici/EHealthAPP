using System;
using System.IO;
using EHealthApp.Data;
//using EHealth;


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
        InitializeComponent();
        MainPage = new AppShell();
    }
}
