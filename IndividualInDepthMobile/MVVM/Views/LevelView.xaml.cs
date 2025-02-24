using SkiaSharp.Views.Maui;
using SkiaSharp;
using IndividualInDepthMobile.MVVM.ViewModels;
using IndividualInDepthMobile.Services;
using System.ComponentModel;

namespace IndividualInDepthMobile.MVVM.Views;

public partial class LevelView : ContentPage
{
    private readonly ILevelRendererService _rendererService;
    private readonly LevelViewModel _viewModel;
    private IDispatcherTimer? _updateTimer;

    public LevelView(LevelViewModel viewModel, ILevelRendererService rendererService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _rendererService = rendererService;
        BindingContext = viewModel;
        
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        SetupTimer();
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LevelViewModel.CurrentReading))
        {
            MainThread.BeginInvokeOnMainThread(() => canvasView.InvalidateSurface());
        }
    }

    private void SetupTimer()
    {
        _updateTimer = Application.Current?.Dispatcher.CreateTimer();
        if (_updateTimer != null)
        {
            _updateTimer.Interval = TimeSpan.FromMilliseconds(16);
            _updateTimer.Tick += (s, e) => canvasView.InvalidateSurface();
        }
    }

    private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
    {
        if (_viewModel.CurrentReading == null) return;

        var bounds = new SKRect(0, 0, args.Info.Width, args.Info.Height);
        _rendererService.RenderLevel(
            args.Surface.Canvas,
            bounds,
            _viewModel.RenderOptions,
            _viewModel.CurrentReading.Angle,
            _viewModel.CurrentReading.BubblePosition
        );
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.StartMeasuring();
        _updateTimer?.Start();
    }

    protected override void OnDisappearing()
    {
        _updateTimer?.Stop();
        _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        _viewModel.Cleanup();
        base.OnDisappearing();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler != null)
        {
            canvasView.InvalidateSurface();
        }
    }
}