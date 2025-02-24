using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using IndividualInDepthMobile.Model;
using IndividualInDepthMobile.Services;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using SkiaSharp;

namespace IndividualInDepthMobile.MVVM.ViewModels;

public class LevelViewModel : INotifyPropertyChanged
{
    private readonly IAccelerometerService _accelerometerService;
    private LevelReading _currentReading;
    private LevelRenderOptions _renderOptions;

    public event PropertyChangedEventHandler? PropertyChanged;

    public LevelReading CurrentReading
    {
        get => _currentReading;
        private set
        {
            if (_currentReading != value)
            {
                _currentReading = value;
                OnPropertyChanged();
            }
        }
    }
    
    public LevelRenderOptions RenderOptions
    {
        get => _renderOptions;
        set
        {
            if (_renderOptions != value)
            {
                _renderOptions = value;
                OnPropertyChanged();
            }
        }
    }
    
    public Command StartMeasuringCommand { get; }
    public Command StopMeasuringCommand { get; }

    public LevelViewModel(IAccelerometerService accelerometerService)
    {
        _accelerometerService = accelerometerService;
        _currentReading = new LevelReading();
        
        _renderOptions = new LevelRenderOptions(
            BackgroundColor: SKColors.GreenYellow,
            BubbleColor: SKColors.White,
            TubeColor: SKColors.Black,
            LineColor: SKColors.Red,
            TextColor: SKColors.Black
        );

        StartMeasuringCommand = new Command(StartMeasuring);
        StopMeasuringCommand = new Command(StopMeasuring);

        _accelerometerService.ReadingChanged += OnAccelerometerReadingChanged;
    }

    private void OnAccelerometerReadingChanged(LevelSensorData reading)
    {
        double angle;
        
        if (Math.Abs(reading.Y) > Math.Abs(reading.X))
        {
            //Portrait
            angle = Math.Atan2(reading.X, Math.Sqrt(reading.Y * reading.Y + reading.Z * reading.Z)) * (180.0 / Math.PI);
        }
        else
        {
            //Landscape
            angle = Math.Atan2(reading.Y, Math.Sqrt(reading.X * reading.X + reading.Z * reading.Z)) * (180.0 / Math.PI);
        }
        
        double maxAngle = 45.0;
        double normalizedPosition = -Math.Min(Math.Max(angle / maxAngle, -1), 1);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            CurrentReading = new LevelReading
            {
                Angle = angle,
                BubblePosition = normalizedPosition
            };
        });
    }

    public void StartMeasuring()
    {
        _accelerometerService.Start();
    }

    public void StopMeasuring()
    {
        _accelerometerService.Stop();
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Cleanup()
    {
        _accelerometerService.ReadingChanged -= OnAccelerometerReadingChanged;
        StopMeasuring();
    }
}