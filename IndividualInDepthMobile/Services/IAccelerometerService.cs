using System;
using Microsoft.Maui.Devices.Sensors;

namespace IndividualInDepthMobile.Services;

public record LevelSensorData(double X, double Y, double Z);

public interface IAccelerometerService
{
    event Action<LevelSensorData>? ReadingChanged;
    bool IsAvailable { get; }
    void Start();
    void Stop();
}

public class AccelerometerService : IAccelerometerService
{
    private readonly IAccelerometer? _accelerometer;
    private double _currentX;
    private double _currentY;
    private double _currentZ;
    private const float Alpha = 0.1f; // Low-pass filter coefficient

    public event Action<LevelSensorData>? ReadingChanged;
    public bool IsAvailable => _accelerometer?.IsSupported ?? false;

    public AccelerometerService()
    {
        try
        {
            _accelerometer = Accelerometer.Default;
            
            if (_accelerometer != null)
            {
                _accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            }
            else
            {
                Console.WriteLine("Accelerometer is null");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing accelerometer: {ex.Message}");
        }
    }

    private void Accelerometer_ReadingChanged(object? sender, AccelerometerChangedEventArgs e)
    {
        try
        {
            _currentX = (Alpha * e.Reading.Acceleration.X) + (1 - Alpha) * _currentX;
            _currentY = (Alpha * e.Reading.Acceleration.Y) + (1 - Alpha) * _currentY;
            _currentZ = (Alpha * e.Reading.Acceleration.Z) + (1 - Alpha) * _currentZ;

            ReadingChanged?.Invoke(new LevelSensorData(_currentX, _currentY, _currentZ));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in accelerometer reading: {ex.Message}");
        }
    }

    public void Start()
    {
        if (IsAvailable)
        {
            try
            {
                _accelerometer!.Start(SensorSpeed.Game);
                Console.WriteLine("Accelerometer started");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting accelerometer: {ex.Message}");
            }
        }
    }

    public void Stop()
    {
        if (IsAvailable)
        {
            try
            {
                _accelerometer!.Stop();
                Console.WriteLine("Accelerometer stopped");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping accelerometer: {ex.Message}");
            }
        }
    }
}