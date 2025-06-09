using EHealthApp.Data; // Namespace-ul pentru AppDatabase
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
        // Înregistrează licența Syncfusion aici (fără \r\n la final)
        SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NNaF1cWWhPYVtpR2Nbek5xdV9HZ1ZUQGYuP1ZhSXxWdkNjWH5fcXNQQmJVU0d9XUs=");

        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit() // Pentru Toast, Snackbar, etc.
            .ConfigureSyncfusionCore()  // Pentru controalele Syncfusion
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Înregistrează AppDatabase ca serviciu singleton
        builder.Services.AddSingleton<AppDatabase>(provider =>
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "ehealth.db3");
            return new AppDatabase(dbPath);
        });

        return builder.Build();
    }
}
