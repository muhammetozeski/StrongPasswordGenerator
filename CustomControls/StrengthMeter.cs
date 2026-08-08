using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace StrongPasswordGenerator.CustomControls;

/// <summary>
/// Custom control displaying visual entropy level and strength rating tier.
/// </summary>
public class StrengthMeter : Control
{
    #region Fields

    private double _entropyBits;
    private string _crackTimeText = string.Empty;
    private bool _quantumThreatActive;
    private UInt128 _crackTimeMs;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the calculated entropy in bits.
    /// </summary>
    [Category("Data")]
    [DefaultValue(0.0)]
    public double EntropyBits
    {
        get => _entropyBits;
        set
        {
            _entropyBits = value;
            Invalidate();
        }
    }

    /// <summary>
    /// Gets or sets whether the password is vulnerable to quantum threats.
    /// </summary>
    [Category("Data")]
    [DefaultValue(false)]
    public bool QuantumThreatActive
    {
        get => _quantumThreatActive;
        set
        {
            _quantumThreatActive = value;
            Invalidate();
        }
    }

    /// <summary>
    /// Gets or sets the estimated crack time text.
    /// </summary>
    [Category("Data")]
    [DefaultValue("")]
    public string CrackTimeText
    {
        get => _crackTimeText;
        set
        {
            _crackTimeText = value;
            Invalidate();
        }
    }

    /// <summary>
    /// Gets or sets the estimated crack time in milliseconds to determine visual strength.
    /// </summary>
    [Category("Data")]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public UInt128 CrackTimeMs
    {
        get => _crackTimeMs;
        set
        {
            _crackTimeMs = value;
            Invalidate();
        }
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the StrengthMeter class.
    /// </summary>
    public StrengthMeter()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor | ControlStyles.ResizeRedraw, true);
        DoubleBuffered = true;
        Size = new Size(300, 50); // Increased height to accommodate new text
        Font = Theme.BodyFont;
    }

    #endregion

    #region Overrides

    /// <summary>
    /// Custom painting for strength bar and entropy status label.
    /// </summary>
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Parent?.BackColor ?? Theme.CardBackgroundColor);

        var (tierName, tierColor) = GetStrengthRating(_crackTimeMs);

        // Status text
        var entropyDisplay = _quantumThreatActive ? $"{_entropyBits:F1} (Quantum: {_entropyBits/2.0:F1})" : $"{_entropyBits:F1}";
        var statusText = $"{entropyDisplay} bits  •  {tierName}";
        var textRect = new Rectangle(0, 0, Width, 18);
        TextRenderer.DrawText(g, statusText, Font, textRect, tierColor, TextFormatFlags.Left | TextFormatFlags.Top);

        // Crack time text
        if (!string.IsNullOrEmpty(_crackTimeText))
        {
            var crackTimeRect = new Rectangle(0, 18, Width, 18);
            TextRenderer.DrawText(g, $"Est. Crack Time: {_crackTimeText}", Font, crackTimeRect, Theme.TextSecondaryColor, TextFormatFlags.Left | TextFormatFlags.Top);
        }

        // Track bar
        const int barHeight = 8;
        var barY = Height - barHeight - 2;
        var trackRect = new Rectangle(0, barY, Width, barHeight);
        using var trackPath = CreateRoundedRectanglePath(trackRect, barHeight);
        using var trackBrush = new SolidBrush(Theme.CardBorderColor);
        g.FillPath(trackBrush, trackPath);

        // Filled strength bar (Logarithmic scale based on time, 10,000 years = 100%)
        // 10,000 years = 315,360,000,000,000 ms. Log10(3.15 * 10^14) ~ 14.5
        double percentage = 1.0;
        if (_crackTimeMs < 315_360_000_000_000)
        {
            double msDouble = Math.Max(1.0, (double)_crackTimeMs);
            percentage = Math.Clamp(Math.Log10(msDouble) / 14.5, 0.04, 1.0);
        }

        var fillWidth = (int)(Width * percentage);
        if (fillWidth > 4)
        {
            var fillRect = new Rectangle(0, barY, fillWidth, barHeight);
            using var fillPath = CreateRoundedRectanglePath(fillRect, barHeight);
            using var fillBrush = new SolidBrush(tierColor);
            g.FillPath(fillBrush, fillPath);
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Determines strength category name and color based on crack time in milliseconds.
    /// </summary>
    private static (string TierName, Color TierColor) GetStrengthRating(UInt128 crackTimeMs)
    {
        if (crackTimeMs < 86_400_000) return ("Weak", Theme.StrengthWeakColor); // < 1 day
        if (crackTimeMs < 31_536_000_000) return ("Moderate", Theme.StrengthModerateColor); // < 1 year
        if (crackTimeMs < 315_360_000_000_000) return ("Strong", Theme.StrengthStrongColor); // < 10,000 years
        return ("Ultra Secure", Theme.StrengthUltraColor);
    }

    /// <summary>
    /// Creates a rounded rectangle path.
    /// </summary>
    private static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
    {
        var path = new GraphicsPath();
        var diameter = cornerRadius;
        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    #endregion
}
