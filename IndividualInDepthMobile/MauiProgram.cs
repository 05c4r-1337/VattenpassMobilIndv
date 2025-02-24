using IndividualInDepthMobile.MVVM.ViewModels;
using IndividualInDepthMobile.MVVM.Views;
using IndividualInDepthMobile.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace IndividualInDepthMobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        builder.Services.AddSingleton<IAccelerometerService, AccelerometerService>();
        
        builder.Services.AddSingleton<ILevelRendererService, LevelRendererService>();
        
        builder.Services.AddTransient<LevelViewModel>();
        
        builder.Services.AddTransient<LevelView>();


#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}