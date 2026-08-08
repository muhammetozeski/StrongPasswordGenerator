using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace StrongPasswordGenerator.CustomControls;

/// <summary>
/// Modern custom painted toggle switch control for WinForms.
/// </summary>
[DefaultEvent(nameof(CheckedChanged))]
public class ToggleSwitch : Control
{
    #region Fields & Events

    private bool _checked;
    private string _labelText = "Toggle Option";

    /// <summary>
    /// Occurs when the Checked property value changes.
    /// </summary>
    public event EventHandler? CheckedChanged;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether the switch is checked.
    /// </summary>
    [Category("Behavior")]
    [DefaultValue(false)]
    public bool Checked
    {
        get => _checked;
        set
        {
            if (_checked != value)
            {
                _checked = value;
                OnCheckedChanged();
                Invalidate();
            }
        }
    }

    /// <summary>
    /// Gets or sets the descriptive label text beside the toggle.
    /// </summary>
    [Category("Appearance")]
    [DefaultValue("Toggle Option")]
    public string LabelText
    {
        get => _labelText;
        set
        {
            _labelText = value;
            Invalidate();
        }
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the ToggleSwitch class.
    /// </summary>
    public ToggleSwitch()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor | ControlStyles.ResizeRedraw, true);
        DoubleBuffered = true;
        Size = new Size(220, 28);
        Cursor = Cursors.Hand;
        Font = Theme.BodyFont;
        ForeColor = Theme.TextPrimaryColor;
    }

    #endregion

    #region Overrides

    /// <summary>
    /// Handles mouse click events to toggle state.
    /// </summary>
    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        Checked = !Checked;
    }

    /// <summary>
    /// Custom painting for the toggle switch pill and label.
    /// </summary>
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Parent?.BackColor ?? Theme.CardBackgroundColor);

        const int switchWidth = 44;
        const int switchHeight = 22;
        var switchY = (Height - switchHeight) / 2;

        // Draw background pill
        var pillRect = new Rectangle(0, switchY, switchWidth, switchHeight);
        using var pillPath = CreateRoundedRectanglePath(pillRect, switchHeight);
        var pillColor = _checked ? Theme.AccentColor : Theme.CardBorderColor;
        using var pillBrush = new SolidBrush(pillColor);
        g.FillPath(pillBrush, pillPath);

        // Draw thumb circle
        const int thumbMargin = 3;
        var thumbDiameter = switchHeight - (thumbMargin * 2);
        var thumbX = _checked ? (switchWidth - thumbDiameter - thumbMargin) : thumbMargin;
        var thumbRect = new Rectangle(thumbX, switchY + thumbMargin, thumbDiameter, thumbDiameter);
        using var thumbBrush = new SolidBrush(Color.White);
        g.FillEllipse(thumbBrush, thumbRect);

        // Draw label text
        var textX = switchWidth + 12;
        var textRect = new Rectangle(textX, 0, Width - textX, Height);
        TextRenderer.DrawText(g, _labelText, Font, textRect, _checked ? Theme.TextPrimaryColor : Theme.TextSecondaryColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
    }

    #endregion

    #region Event Helpers

    /// <summary>
    /// Triggers the CheckedChanged event.
    /// </summary>
    protected virtual void OnCheckedChanged()
    {
        CheckedChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Creates a GraphicsPath for rounded rectangle corners.
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
