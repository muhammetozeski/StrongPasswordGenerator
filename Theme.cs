using System.Drawing;

namespace StrongPasswordGenerator;

/// <summary>
/// Provides a centralized color palette and UI styling constants for the application.
/// </summary>
public static class Theme
{
    #region Background Colors

    /// <summary>
    /// Gets the primary window background color.
    /// </summary>
    public static readonly Color BackgroundColor = Color.FromArgb(15, 17, 26);

    /// <summary>
    /// Gets the card container background color.
    /// </summary>
    public static readonly Color CardBackgroundColor = Color.FromArgb(24, 27, 41);

    /// <summary>
    /// Gets the elevated card background color.
    /// </summary>
    public static readonly Color CardElevatedColor = Color.FromArgb(32, 36, 54);

    /// <summary>
    /// Gets the card border stroke color.
    /// </summary>
    public static readonly Color CardBorderColor = Color.FromArgb(48, 54, 78);

    #endregion

    #region Accent & Text Colors

    /// <summary>
    /// Gets the primary accent color.
    /// </summary>
    public static readonly Color AccentColor = Color.FromArgb(99, 102, 241);

    /// <summary>
    /// Gets the primary accent hover color.
    /// </summary>
    public static readonly Color AccentHoverColor = Color.FromArgb(129, 140, 248);

    /// <summary>
    /// Gets the primary text color.
    /// </summary>
    public static readonly Color TextPrimaryColor = Color.FromArgb(243, 244, 246);

    /// <summary>
    /// Gets the secondary text color.
    /// </summary>
    public static readonly Color TextSecondaryColor = Color.FromArgb(156, 163, 175);

    /// <summary>
    /// Gets the muted text color.
    /// </summary>
    public static readonly Color TextMutedColor = Color.FromArgb(107, 114, 128);

    #endregion

    #region Strength Metric Colors

    /// <summary>
    /// Gets the color for weak password rating.
    /// </summary>
    public static readonly Color StrengthWeakColor = Color.FromArgb(239, 68, 68);

    /// <summary>
    /// Gets the color for moderate password rating.
    /// </summary>
    public static readonly Color StrengthModerateColor = Color.FromArgb(245, 158, 11);

    /// <summary>
    /// Gets the color for strong password rating.
    /// </summary>
    public static readonly Color StrengthStrongColor = Color.FromArgb(16, 185, 129);

    /// <summary>
    /// Gets the color for ultra/military grade password rating.
    /// </summary>
    public static readonly Color StrengthUltraColor = Color.FromArgb(14, 165, 233);

    #endregion

    #region Fonts

    /// <summary>
    /// Gets the default application font family name.
    /// </summary>
    public static readonly Font HeaderFont = new("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);

    /// <summary>
    /// Gets the section header font.
    /// </summary>
    public static readonly Font SectionFont = new("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);

    /// <summary>
    /// Gets the body text font.
    /// </summary>
    public static readonly Font BodyFont = new("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);

    /// <summary>
    /// Gets the monospace font used for displaying generated passwords.
    /// </summary>
    public static readonly Font PasswordFont = new("Consolas", 15.75F, FontStyle.Bold, GraphicsUnit.Point);

    #endregion
}
