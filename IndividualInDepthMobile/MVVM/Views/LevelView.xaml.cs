using System.ComponentModel;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using IndividualInDepthMobile.MVVM.ViewModels;

namespace IndividualInDepthMobile.MVVM.Views;

public partial class LevelView : ContentPage
{
    private readonly SKFont _textFont;
    private readonly SKPaint _bubblePaint;
    private readonly SKPaint _tubePaint;
    private readonly SKPaint _linePaint;
    private readonly SKPaint _textPaint;
    private IDispatcherTimer? _updateTimer;
    private LevelViewModel _viewModel;

    public LevelView(LevelViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;

        _textFont = new SKFont
        {
            Size = 48
        };
        
        _bubblePaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.White,
            IsAntialias = true
        };

        _tubePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
            StrokeWidth = 4,
            IsAntialias = true
        };

        _linePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Red,
            StrokeWidth = 2,
            IsAntialias = true
        };

        _textPaint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true
        };
        
        SetupTimer();
    }

    private void SetupTimer()
    {
        _updateTimer = Application.Current?.Dispatcher.CreateTimer();
        if (_updateTimer != null)
        {
            _updateTimer.Interval = TimeSpan.FromMilliseconds(16);
            _updateTimer.Tick += (s, e) => 
            {
                Console.WriteLine($"Timer tick - Invalidating surface");
                canvasView.InvalidateSurface();
            };
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LevelViewModel.CurrentReading))
        {
            Console.WriteLine($"ViewModel updated - Current angle: {_viewModel.CurrentReading?.Angle}");
            MainThread.BeginInvokeOnMainThread(() => canvasView.InvalidateSurface());
        }
    }

    private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
    {
        if (_viewModel?.CurrentReading == null)
        {
            Console.WriteLine("Paint surface called but no reading available");
            return;
        }

        Console.WriteLine($"Drawing surface - Angle: {_viewModel.CurrentReading.Angle}, Position: {_viewModel.CurrentReading.BubblePosition}");

        var surface = args.Surface;
        var canvas = surface.Canvas;
        canvas.Clear(new SKColor(102, 255, 51));

        var width = args.Info.Width;
        var height = args.Info.Height;
        var centerX = width / 2f;
        var centerY = height / 2f;

        //horizontal tube
        var tubeWidth = width * 0.8f;
        var tubeHeight = height * 0.2f;
        var tube = new SKRect(
            centerX - (tubeWidth / 2),
            centerY - (tubeHeight / 2),
            centerX + (tubeWidth / 2),
            centerY + (tubeHeight / 2));
        canvas.DrawRoundRect(tube, tubeHeight / 2, tubeHeight / 2, _tubePaint);

        //bubble position
        var maxBubbleTravel = tubeWidth / 2 - (tubeHeight * 0.4f);
        var bubbleX = centerX + (float)(_viewModel.CurrentReading.BubblePosition * maxBubbleTravel);
        
        //bubble
        var bubbleRadius = tubeHeight * 0.4f;
        canvas.DrawCircle(bubbleX, centerY, bubbleRadius, _bubblePaint);

        //center line
        canvas.DrawLine(centerX, centerY - tubeHeight, centerX, centerY + tubeHeight, _linePaint);

        //angle
        var angleText = $"{Math.Round(_viewModel.CurrentReading.Angle, 1)}°";
        var textBounds = new SKRect();
        _textPaint.MeasureText(angleText, ref textBounds);
        canvas.DrawText(angleText, centerX, centerY - tubeHeight - textBounds.Height, SKTextAlign.Center, _textFont, _textPaint);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Console.WriteLine("View appearing - Starting measurements");
        _viewModel?.StartMeasuring();
        _updateTimer?.Start();
    }

    protected override void OnDisappearing()
    {
        Console.WriteLine("View disappearing - Cleaning up");
        _updateTimer?.Stop();
        _viewModel?.Cleanup();
        base.OnDisappearing();
    }
}