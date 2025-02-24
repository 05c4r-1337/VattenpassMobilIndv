using SkiaSharp;

namespace IndividualInDepthMobile.Services;

public interface ILevelRendererService
{
    void RenderLevel(SKCanvas canvas, SKRect bounds, LevelRenderOptions options, double angle, double bubblePosition);
}

public record LevelRenderOptions(
    SKColor BackgroundColor,
    SKColor BubbleColor,
    SKColor TubeColor,
    SKColor LineColor,
    SKColor TextColor,
    float TextSize = 48f,
    float TubeWidthPercentage = 0.8f,
    float TubeHeightPercentage = 0.2f
);

public class LevelRendererService : ILevelRendererService
{
    private readonly SKFont _textFont;
    private readonly SKPaint _bubblePaint;
    private readonly SKPaint _tubePaint;
    private readonly SKPaint _linePaint;
    private readonly SKPaint _textPaint;

    public LevelRendererService()
    {
        _textFont = new SKFont { Size = 48 };
        
        _bubblePaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        _tubePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 4,
            IsAntialias = true
        };

        _linePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
            IsAntialias = true
        };

        _textPaint = new SKPaint
        {
            IsAntialias = true
        };
    }

    public void RenderLevel(SKCanvas canvas, SKRect bounds, LevelRenderOptions options, double angle, double bubblePosition)
    {
        _bubblePaint.Color = options.BubbleColor;
        _tubePaint.Color = options.TubeColor;
        _linePaint.Color = options.LineColor;
        _textPaint.Color = options.TextColor;
        _textFont.Size = options.TextSize;
        
        canvas.Clear(options.BackgroundColor);

        var width = bounds.Width;
        var height = bounds.Height;
        var centerX = bounds.MidX;
        var centerY = bounds.MidY;

        // Draw tube
        var tubeWidth = width * options.TubeWidthPercentage;
        var tubeHeight = height * options.TubeHeightPercentage;
        var tube = new SKRect(
            centerX - (tubeWidth / 2),
            centerY - (tubeHeight / 2),
            centerX + (tubeWidth / 2),
            centerY + (tubeHeight / 2));
        canvas.DrawRoundRect(tube, tubeHeight / 2, tubeHeight / 2, _tubePaint);

        // Draw bubble
        var maxBubbleTravel = tubeWidth / 2 - (tubeHeight * 0.4f);
        var bubbleX = centerX + (float)(bubblePosition * maxBubbleTravel);
        var bubbleRadius = tubeHeight * 0.4f;
        canvas.DrawCircle(bubbleX, centerY, bubbleRadius, _bubblePaint);

        // Draw center line
        canvas.DrawLine(centerX, centerY - tubeHeight, centerX, centerY + tubeHeight, _linePaint);

        // Draw angle text
        var angleText = $"{Math.Round(angle, 1)}°";
        var textBounds = new SKRect();
        _textPaint.MeasureText(angleText, ref textBounds);
        canvas.DrawText(angleText, centerX, centerY - tubeHeight - textBounds.Height, SKTextAlign.Center, _textFont, _textPaint);
    }
}