using EHealthApp.Data; // Namespace-ul tău pentru AppDatabase
using Syncfusion.Maui.Core.Hosting; // Adaugă acest using!

namespace EHealthApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureSyncfusionCore() // << ADĂUGAT pentru controalele Syncfusion!
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ✅ Înregistrează baza de date ca serviciu singleton
        builder.Services.AddSingleton<AppDatabase>(provider =>
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "ehealth.db3");
            return new AppDatabase(dbPath);
        });

        return builder.Build();
    }
}