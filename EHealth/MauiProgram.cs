using EHealthApp.Data;
using Syncfusion.Maui.Core.Hosting;
using Syncfusion.Licensing;
using CommunityToolkit.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Storage;
using System.IO;

namespace EHealthApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        SyncfusionLicenseProvider.RegisterLicense("@32392e302e303b32393bZfVz2MvP9BmohqEktKqNAqHtlZ1Xvw+RIqAnBKmu/O4=\r\n@32392e302e303b32393bP2rM+ev+5JSatt6L4/sliWwuFQGEKNtwjid2Opu2YLM=\r\n");

        var builder = MauiApp.CreateBuilder();

        _ = builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureSyncfusionCore()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<AppDatabase>(provider =>
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "ehealth.db3");
            return new AppDatabase(dbPath);
        });

        return builder.Build();
    }
}
