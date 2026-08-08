using System.Drawing;
using System.Windows.Forms;
using StrongPasswordGenerator.CustomControls;

namespace StrongPasswordGenerator;

/// <summary>
/// Main application form for generating cryptographically secure passwords.
/// </summary>
public partial class MainForm : Form
{
    #region Fields & Controls

    private bool _isUpdatingControls;
    private readonly System.Windows.Forms.Timer _copyFeedbackTimer = new();

    private TextBox _txtPassword = null!;
    private Button _btnCopy = null!;
    private Button _btnRegenerate = null!;

    private NumericUpDown _numLength = null!;
    private TrackBar _trackLength = null!;
    private NumericUpDown _numBits = null!;
    private StrengthMeter _strengthMeter = null!;

    private ToggleSwitch _toggleUppercase = null!;
    private ToggleSwitch _toggleLowercase = null!;
    private ToggleSwitch _toggleDigits = null!;
    private ToggleSwitch _toggleSpecialSymbols = null!;
    private ToggleSwitch _toggleExtendedSymbols = null!;
    private ToggleSwitch _toggleExcludeAmbiguous = null!;
    private ToggleSwitch _toggleIncludeSpaces = null!;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the MainForm class.
    /// </summary>
    public MainForm()
    {
        InitializeComponent();
        ConfigureFormWindow();
        BuildCustomUiLayout();
        InitializeCopyTimer();
        UpdateMetricsAndGenerate();
    }

    #endregion

    #region Window & UI Construction

    /// <summary>
    /// Configures form window size, dark background, and startup position.
    /// </summary>
    private void ConfigureFormWindow()
    {
        Text = "Cryptographic Password Generator";
        Size = new Size(680, 720);
        MinimumSize = new Size(620, 680);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Theme.BackgroundColor;
        ForeColor = Theme.TextPrimaryColor;
        DoubleBuffered = true;
    }

    /// <summary>
    /// Constructs the entire card-based modern UI hierarchy programmatically.
    /// </summary>
    private void BuildCustomUiLayout()
    {
        var mainContainer = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(24),
            BackColor = Theme.BackgroundColor
        };
        Controls.Add(mainContainer);

        // Header Panel
        var headerPanel = CreateHeaderPanel();
        mainContainer.Controls.Add(headerPanel);

        // Password Display Card
        var passwordCard = CreatePasswordCard();
        mainContainer.Controls.Add(passwordCard);

        // Length & Entropy Controls Card
        var controlsCard = CreateLengthAndEntropyCard();
        mainContainer.Controls.Add(controlsCard);

        // Character Options Toggles Card
        var togglesCard = CreateTogglesCard();
        mainContainer.Controls.Add(togglesCard);
    }

    /// <summary>
    /// Creates the header panel containing title and security subtitle.
    /// </summary>
    private static Panel CreateHeaderPanel()
    {
        var panel = new Panel
        {
            Size = new Size(610, 50),
            Margin = new Padding(0, 0, 0, 16)
        };

        var lblTitle = new Label
        {
            Text = "SECURE PASSWORD GENERATOR",
            Font = Theme.HeaderFont,
            ForeColor = Theme.TextPrimaryColor,
            AutoSize = true,
            Location = new Point(0, 0)
        };

        var lblSubtitle = new Label
        {
            Text = "Hardware-grade random entropy powered by System.Security.Cryptography",
            Font = Theme.BodyFont,
            ForeColor = Theme.TextSecondaryColor,
            AutoSize = true,
            Location = new Point(2, 28)
        };

        panel.Controls.Add(lblTitle);
        panel.Controls.Add(lblSubtitle);
        return panel;
    }

    /// <summary>
    /// Creates the top password display card with copy and regenerate buttons.
    /// </summary>
    private Panel CreatePasswordCard()
    {
        var card = CreateCardPanel(610, 110);

        _txtPassword = new TextBox
        {
            Font = Theme.PasswordFont,
            BackColor = Theme.CardElevatedColor,
            ForeColor = Theme.AccentHoverColor,
            BorderStyle = BorderStyle.FixedSingle,
            ReadOnly = true,
            Location = new Point(16, 16),
            Size = new Size(578, 36)
        };

        _btnRegenerate = new Button
        {
            Text = "Regenerate",
            Font = Theme.BodyFont,
            BackColor = Theme.AccentColor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Location = new Point(16, 60),
            Size = new Size(130, 34),
            Cursor = Cursors.Hand
        };
        _btnRegenerate.FlatAppearance.BorderSize = 0;
        _btnRegenerate.Click += (_, _) => GeneratePassword();

        _btnCopy = new Button
        {
            Text = "Copy Password",
            Font = Theme.BodyFont,
            BackColor = Theme.CardElevatedColor,
            ForeColor = Theme.TextPrimaryColor,
            FlatStyle = FlatStyle.Flat,
            Location = new Point(156, 60),
            Size = new Size(140, 34),
            Cursor = Cursors.Hand
        };
        _btnCopy.FlatAppearance.BorderColor = Theme.CardBorderColor;
        _btnCopy.Click += OnCopyButtonClick;

        card.Controls.Add(_txtPassword);
        card.Controls.Add(_btnRegenerate);
        card.Controls.Add(_btnCopy);

        return card;
    }

    /// <summary>
    /// Creates the middle card containing password length, bit entropy inputs, and strength meter.
    /// </summary>
    private Panel CreateLengthAndEntropyCard()
    {
        var card = CreateCardPanel(610, 165);

        var lblSection = new Label
        {
            Text = "LENGTH & ENTROPY BITS",
            Font = Theme.SectionFont,
            ForeColor = Theme.TextSecondaryColor,
            Location = new Point(16, 12),
            AutoSize = true
        };

        // Password Length controls
        var lblLength = new Label
        {
            Text = "Character Length:",
            Font = Theme.BodyFont,
            ForeColor = Theme.TextPrimaryColor,
            Location = new Point(16, 42),
            AutoSize = true
        };

        _numLength = new NumericUpDown
        {
            Minimum = 4,
            Maximum = 128,
            Value = 16,
            Font = Theme.BodyFont,
            BackColor = Theme.CardElevatedColor,
            ForeColor = Theme.TextPrimaryColor,
            Location = new Point(140, 38),
            Size = new Size(64, 26)
        };
        _numLength.ValueChanged += OnLengthNumericChanged;

        _trackLength = new TrackBar
        {
            Minimum = 4,
            Maximum = 128,
            Value = 16,
            Location = new Point(210, 36),
            Size = new Size(180, 45),
            TickStyle = TickStyle.None,
            AutoSize = false
        };
        _trackLength.ValueChanged += OnLengthTrackBarChanged;

        // Bit Count controls
        var lblBits = new Label
        {
            Text = "Entropy Bits:",
            Font = Theme.BodyFont,
            ForeColor = Theme.TextPrimaryColor,
            Location = new Point(410, 42),
            AutoSize = true
        };

        _numBits = new NumericUpDown
        {
            Minimum = 10,
            Maximum = 512,
            Value = 96,
            Font = Theme.BodyFont,
            BackColor = Theme.CardElevatedColor,
            ForeColor = Theme.TextPrimaryColor,
            Location = new Point(500, 38),
            Size = new Size(94, 26)
        };
        _numBits.ValueChanged += OnBitsNumericChanged;

        // Strength Meter
        _strengthMeter = new StrengthMeter
        {
            Location = new Point(16, 92),
            Size = new Size(578, 48)
        };

        card.Controls.Add(lblSection);
        card.Controls.Add(lblLength);
        card.Controls.Add(_numLength);
        card.Controls.Add(_trackLength);
        card.Controls.Add(lblBits);
        card.Controls.Add(_numBits);
        card.Controls.Add(_strengthMeter);

        return card;
    }

    /// <summary>
    /// Creates the bottom card containing character set inclusion toggles.
    /// </summary>
    private Panel CreateTogglesCard()
    {
        var card = CreateCardPanel(610, 220);

        var lblSection = new Label
        {
            Text = "CHARACTER SET OPTIONS",
            Font = Theme.SectionFont,
            ForeColor = Theme.TextSecondaryColor,
            Location = new Point(16, 12),
            AutoSize = true
        };
        card.Controls.Add(lblSection);

        _toggleUppercase = CreateToggle("Uppercase (A-Z)", true, 16, 44);
        _toggleLowercase = CreateToggle("Lowercase (a-z)", true, 310, 44);

        _toggleDigits = CreateToggle("Digits (0-9)", true, 16, 80);
        _toggleSpecialSymbols = CreateToggle("Special Symbols (!@#$)", true, 310, 80);

        _toggleExtendedSymbols = CreateToggle("Extended Symbols (~`/)", false, 16, 116);
        _toggleExcludeAmbiguous = CreateToggle("Exclude Ambiguous (O,0,l,1)", false, 310, 116);

        _toggleIncludeSpaces = CreateToggle("Include Spaces (' ')", false, 16, 152);

        card.Controls.Add(_toggleUppercase);
        card.Controls.Add(_toggleLowercase);
        card.Controls.Add(_toggleDigits);
        card.Controls.Add(_toggleSpecialSymbols);
        card.Controls.Add(_toggleExtendedSymbols);
        card.Controls.Add(_toggleExcludeAmbiguous);
        card.Controls.Add(_toggleIncludeSpaces);

        return card;
    }

    /// <summary>
    /// Creates a standard card panel container.
    /// </summary>
    private static Panel CreateCardPanel(int width, int height)
    {
        return new Panel
        {
            Size = new Size(width, height),
            BackColor = Theme.CardBackgroundColor,
            Margin = new Padding(0, 0, 0, 16),
            Padding = new Padding(16)
        };
    }

    /// <summary>
    /// Creates a toggle switch control and attaches change handler for dynamic password regeneration.
    /// </summary>
    private ToggleSwitch CreateToggle(string label, bool initialChecked, int x, int y)
    {
        var toggle = new ToggleSwitch
        {
            LabelText = label,
            Checked = initialChecked,
            Location = new Point(x, y),
            Size = new Size(270, 28)
        };
        toggle.CheckedChanged += (_, _) => OnOptionToggleChanged();
        return toggle;
    }

    #endregion

    #region Event Handlers & Dynamic Synchronization

    /// <summary>
    /// Handles changes in character set toggle switches.
    /// </summary>
    private void OnOptionToggleChanged()
    {
        if (_isUpdatingControls) return;
        UpdateMetricsAndGenerate();
    }

    /// <summary>
    /// Handles character length numeric up-down changes.
    /// </summary>
    private void OnLengthNumericChanged(object? sender, EventArgs e)
    {
        if (_isUpdatingControls) return;
        _isUpdatingControls = true;

        var newLength = (int)_numLength.Value;
        _trackLength.Value = newLength;

        var options = ReadCurrentOptions();
        var poolSize = PasswordGenerator.CalculatePoolSize(options);
        var bits = PasswordGenerator.CalculateEntropyBits(newLength, poolSize);

        _numBits.Value = (decimal)Math.Clamp(bits, (double)_numBits.Minimum, (double)_numBits.Maximum);
        _strengthMeter.EntropyBits = bits;

        _isUpdatingControls = false;
        GeneratePassword();
    }

    /// <summary>
    /// Handles character length track bar slider changes.
    /// </summary>
    private void OnLengthTrackBarChanged(object? sender, EventArgs e)
    {
        if (_isUpdatingControls) return;
        _numLength.Value = _trackLength.Value;
    }

    /// <summary>
    /// Handles entropy bit numeric up-down changes.
    /// </summary>
    private void OnBitsNumericChanged(object? sender, EventArgs e)
    {
        if (_isUpdatingControls) return;
        _isUpdatingControls = true;

        var options = ReadCurrentOptions();
        var poolSize = PasswordGenerator.CalculatePoolSize(options);
        var requiredLength = PasswordGenerator.CalculateLengthFromBits((double)_numBits.Value, poolSize);

        _numLength.Value = requiredLength;
        _trackLength.Value = requiredLength;

        var actualBits = PasswordGenerator.CalculateEntropyBits(requiredLength, poolSize);
        _strengthMeter.EntropyBits = actualBits;

        _isUpdatingControls = false;
        GeneratePassword();
    }

    /// <summary>
    /// Recalculates pool metrics, synchronizes controls, and generates a new password.
    /// </summary>
    private void UpdateMetricsAndGenerate()
    {
        _isUpdatingControls = true;

        var options = ReadCurrentOptions();
        var poolSize = PasswordGenerator.CalculatePoolSize(options);
        var length = (int)_numLength.Value;
        var bits = PasswordGenerator.CalculateEntropyBits(length, poolSize);

        _numBits.Value = (decimal)Math.Clamp(bits, (double)_numBits.Minimum, (double)_numBits.Maximum);
        _strengthMeter.EntropyBits = bits;

        _isUpdatingControls = false;
        GeneratePassword();
    }

    /// <summary>
    /// Generates a new password using PasswordGenerator and updates password textbox.
    /// </summary>
    private void GeneratePassword()
    {
        var options = ReadCurrentOptions();
        var password = PasswordGenerator.Generate(options);
        _txtPassword.Text = password;
    }

    /// <summary>
    /// Compiles current user selections into a PasswordOptions record.
    /// </summary>
    private PasswordOptions ReadCurrentOptions()
    {
        return new PasswordOptions
        {
            IncludeUppercase = _toggleUppercase.Checked,
            IncludeLowercase = _toggleLowercase.Checked,
            IncludeDigits = _toggleDigits.Checked,
            IncludeSpecialSymbols = _toggleSpecialSymbols.Checked,
            IncludeExtendedSymbols = _toggleExtendedSymbols.Checked,
            ExcludeAmbiguousCharacters = _toggleExcludeAmbiguous.Checked,
            IncludeSpaces = _toggleIncludeSpaces.Checked,
            Length = (int)_numLength.Value
        };
    }

    #endregion

    #region Copy & Feedback Helpers

    /// <summary>
    /// Copies generated password to OS clipboard and displays visual feedback.
    /// </summary>
    private void OnCopyButtonClick(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_txtPassword.Text)) return;

        Clipboard.SetText(_txtPassword.Text);
        _btnCopy.Text = "Copied!";
        _btnCopy.BackColor = Theme.StrengthStrongColor;
        _btnCopy.ForeColor = Color.White;
        _copyFeedbackTimer.Start();
    }

    /// <summary>
    /// Initializes timer for resetting copy button appearance.
    /// </summary>
    private void InitializeCopyTimer()
    {
        _copyFeedbackTimer.Interval = 2000;
        _copyFeedbackTimer.Tick += (_, _) =>
        {
            _copyFeedbackTimer.Stop();
            _btnCopy.Text = "Copy Password";
            _btnCopy.BackColor = Theme.CardElevatedColor;
            _btnCopy.ForeColor = Theme.TextPrimaryColor;
        };
    }

    #endregion
}
