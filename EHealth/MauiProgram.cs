using EHealthApp.Data; // Pentru AppDatabase
using Syncfusion.Maui.Core.Hosting; // Pentru controale Syncfusion
using Syncfusion.Licensing; // Pentru licența Syncfusion
using CommunityToolkit.Maui; // Pentru CommunityToolkit.Maui
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Storage;
using System.IO;

namespace EHealthApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // -- Introdu cheia de licență mai jos (păstrează formatul cheii EXACT, fără spații suplimentare) --
        SyncfusionLicenseProvider.RegisterLicense("@32392e302e303b32393bZfVz2MvP9BmohqEktKqNAqHtlZ1Xvw+RIqAnBKmu/O4=\r\n@32392e302e303b32393bP2rM+ev+5JSatt6L4/sliWwuFQGEKNtwjid2Opu2YLM=\r\n");

        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()        // Pentru Toast, Snackbar, etc.
            .ConfigureSyncfusionCore()        // Pentru controalele Syncfusion
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Înregistrează AppDatabase ca serviciu singleton (SQLite)
        builder.Services.AddSingleton<AppDatabase>(provider =>
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "ehealth.db3");
            return new AppDatabase(dbPath);
        });

        return builder.Build();
    }
}
