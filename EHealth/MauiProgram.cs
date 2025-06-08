using EHealthApp.Data; // Namespace-ul pentru AppDatabase
using Syncfusion.Maui.Core.Hosting; // Pentru controale Syncfusion
using CommunityToolkit.Maui;        // Pentru CommunityToolkit.Maui
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Storage;
using System.IO;

namespace EHealthApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit() // << Adaugă această linie pentru Toast, Snackbar, etc!
            .ConfigureSyncfusionCore() // Pentru controalele Syncfusion
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Înregistrează baza de date ca serviciu singleton
        builder.Services.AddSingleton<AppDatabase>(provider =>
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "ehealth.db3");
            return new AppDatabase(dbPath);
        });

        return builder.Build();
    }
}