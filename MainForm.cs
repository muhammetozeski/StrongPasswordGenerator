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

    private TrackBar _trackThreatLevel = null!;
    private Label _lblThreatDesc = null!;
    private ToggleSwitch _toggleQuantum = null!;

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

    private void BuildCustomUiLayout()
    {
        var mainContainer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(24),
            BackColor = Theme.BackgroundColor,
            AutoScroll = true
        };
        mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        mainContainer.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainContainer.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainContainer.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainContainer.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        Controls.Add(mainContainer);

        var headerPanel = CreateHeaderPanel();
        mainContainer.Controls.Add(headerPanel, 0, 0);

        var passwordCard = CreatePasswordCard();
        mainContainer.Controls.Add(passwordCard, 0, 1);

        var controlsCard = CreateLengthAndEntropyCard();
        mainContainer.Controls.Add(controlsCard, 0, 2);

        var threatCard = CreateThreatModelCard();
        mainContainer.Controls.Add(threatCard, 0, 3);

        var togglesCard = CreateTogglesCard();
        mainContainer.Controls.Add(togglesCard, 0, 4);
    }

    private static Panel CreateHeaderPanel()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(0, 0, 0, 16)
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        var lblTitle = new Label
        {
            Text = "SECURE PASSWORD GENERATOR",
            Font = Theme.HeaderFont,
            ForeColor = Theme.TextPrimaryColor,
            AutoSize = true,
            Anchor = AnchorStyles.Left | AnchorStyles.Top
        };

        var lblSubtitle = new Label
        {
            Text = "Hardware-grade random entropy powered by System.Security.Cryptography",
            Font = Theme.BodyFont,
            ForeColor = Theme.TextSecondaryColor,
            AutoSize = true,
            Anchor = AnchorStyles.Left | AnchorStyles.Top
        };

        panel.Controls.Add(lblTitle, 0, 0);
        panel.Controls.Add(lblSubtitle, 0, 1);
        return panel;
    }

    private Panel CreatePasswordCard()
    {
        var card = CreateCardPanel();

        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            ColumnCount = 2,
            RowCount = 2
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        table.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        _txtPassword = new TextBox
        {
            Font = Theme.PasswordFont,
            BackColor = Theme.CardElevatedColor,
            ForeColor = Theme.AccentHoverColor,
            BorderStyle = BorderStyle.FixedSingle,
            ReadOnly = false,
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 0, 16)
        };
        _txtPassword.TextChanged += OnPasswordTextChanged;
        table.Controls.Add(_txtPassword, 0, 0);
        table.SetColumnSpan(_txtPassword, 2);

        _btnRegenerate = new Button
        {
            Text = "Regenerate",
            Font = Theme.BodyFont,
            BackColor = Theme.AccentColor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Anchor = AnchorStyles.Left | AnchorStyles.Top,
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
            Anchor = AnchorStyles.Left | AnchorStyles.Top,
            Size = new Size(140, 34),
            Cursor = Cursors.Hand
        };
        _btnCopy.FlatAppearance.BorderColor = Theme.CardBorderColor;
        _btnCopy.Click += OnCopyButtonClick;

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            WrapContents = false
        };
        buttonPanel.Controls.Add(_btnRegenerate);
        buttonPanel.Controls.Add(_btnCopy);

        table.Controls.Add(buttonPanel, 0, 1);
        table.SetColumnSpan(buttonPanel, 2);

        card.Controls.Add(table);
        return card;
    }

    private Panel CreateLengthAndEntropyCard()
    {
        var card = CreateCardPanel();

        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            ColumnCount = 3,
            RowCount = 3
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var lblSection = new Label
        {
            Text = "LENGTH & ENTROPY BITS",
            Font = Theme.SectionFont,
            ForeColor = Theme.TextSecondaryColor,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 16)
        };
        table.Controls.Add(lblSection, 0, 0);
        table.SetColumnSpan(lblSection, 3);

        var lengthPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Dock = DockStyle.Fill, Margin = new Padding(0, 0, 16, 0) };
        var lblLength = new Label { Text = "Character Length:", Font = Theme.BodyFont, ForeColor = Theme.TextPrimaryColor, AutoSize = true, Anchor = AnchorStyles.Left };
        _numLength = new NumericUpDown { Minimum = 4, Maximum = 128, Value = 16, Font = Theme.BodyFont, BackColor = Theme.CardElevatedColor, ForeColor = Theme.TextPrimaryColor, Size = new Size(64, 26) };
        _numLength.ValueChanged += OnLengthNumericChanged;
        lengthPanel.Controls.Add(lblLength);
        lengthPanel.Controls.Add(_numLength);
        
        _trackLength = new TrackBar { Minimum = 4, Maximum = 128, Value = 16, TickStyle = TickStyle.None, Dock = DockStyle.Fill };
        _trackLength.ValueChanged += OnLengthTrackBarChanged;

        var bitsPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Margin = new Padding(16, 0, 0, 0) };
        _numBits = new NumericUpDown { Minimum = 10, Maximum = 512, Value = 96, Font = Theme.BodyFont, BackColor = Theme.CardElevatedColor, ForeColor = Theme.TextPrimaryColor, Size = new Size(94, 26) };
        _numBits.ValueChanged += OnBitsNumericChanged;
        var lblBits = new Label { Text = "Entropy Bits:", Font = Theme.BodyFont, ForeColor = Theme.TextPrimaryColor, AutoSize = true, Anchor = AnchorStyles.Right };
        bitsPanel.Controls.Add(_numBits);
        bitsPanel.Controls.Add(lblBits);

        table.Controls.Add(lengthPanel, 0, 1);
        table.Controls.Add(_trackLength, 1, 1);
        table.Controls.Add(bitsPanel, 2, 1);

        _strengthMeter = new StrengthMeter { Dock = DockStyle.Fill, Margin = new Padding(0, 16, 0, 0) };
        table.Controls.Add(_strengthMeter, 0, 2);
        table.SetColumnSpan(_strengthMeter, 3);

        card.Controls.Add(table);
        return card;
    }

    private Panel CreateThreatModelCard()
    {
        var card = CreateCardPanel();

        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            ColumnCount = 2,
            RowCount = 3
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var lblSection = new Label
        {
            Text = "ATTACKER THREAT MODEL",
            Font = Theme.SectionFont,
            ForeColor = Theme.TextSecondaryColor,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 16)
        };
        table.Controls.Add(lblSection, 0, 0);
        table.SetColumnSpan(lblSection, 2);

        _trackThreatLevel = new TrackBar
        {
            Minimum = 200,
            Maximum = 1500,
            Value = 900, // Default to 10^9 = 1 GH/s
            TickFrequency = 100,
            TickStyle = TickStyle.BottomRight,
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 0, 16)
        };
        _trackThreatLevel.ValueChanged += OnThreatLevelChanged;

        _toggleQuantum = new ToggleSwitch
        {
            LabelText = "Quantum Threat",
            Checked = false,
            Dock = DockStyle.Fill,
            Margin = new Padding(16, 0, 0, 16),
            MinimumSize = new Size(180, 28)
        };
        _toggleQuantum.CheckedChanged += OnThreatLevelChanged;

        _lblThreatDesc = new Label
        {
            Font = Theme.BodyFont,
            ForeColor = Theme.TextPrimaryColor,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 16)
        };

        table.Controls.Add(_trackThreatLevel, 0, 1);
        table.Controls.Add(_toggleQuantum, 1, 1);
        table.Controls.Add(_lblThreatDesc, 0, 2);
        table.SetColumnSpan(_lblThreatDesc, 2);

        card.Controls.Add(table);
        
        UpdateThreatDescription();
        return card;
    }

    private Panel CreateTogglesCard()
    {
        var card = CreateCardPanel();

        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            ColumnCount = 2,
            RowCount = 5
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

        var lblSection = new Label
        {
            Text = "CHARACTER SET OPTIONS",
            Font = Theme.SectionFont,
            ForeColor = Theme.TextSecondaryColor,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 16)
        };
        table.Controls.Add(lblSection, 0, 0);
        table.SetColumnSpan(lblSection, 2);

        _toggleUppercase = CreateToggle("Uppercase (A-Z)", true);
        _toggleLowercase = CreateToggle("Lowercase (a-z)", true);
        _toggleDigits = CreateToggle("Digits (0-9)", true);
        _toggleSpecialSymbols = CreateToggle("Special Symbols (!@#$)", true);
        _toggleExtendedSymbols = CreateToggle("Extended Symbols (~`/)", false);
        _toggleExcludeAmbiguous = CreateToggle("Exclude Ambiguous (O,0,l,1)", false);
        _toggleIncludeSpaces = CreateToggle("Include Spaces (' ')", false);

        table.Controls.Add(_toggleUppercase, 0, 1);
        table.Controls.Add(_toggleLowercase, 1, 1);
        table.Controls.Add(_toggleDigits, 0, 2);
        table.Controls.Add(_toggleSpecialSymbols, 1, 2);
        table.Controls.Add(_toggleExtendedSymbols, 0, 3);
        table.Controls.Add(_toggleExcludeAmbiguous, 1, 3);
        table.Controls.Add(_toggleIncludeSpaces, 0, 4);

        card.Controls.Add(table);
        return card;
    }

    private static Panel CreateCardPanel()
    {
        return new Panel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Theme.CardBackgroundColor,
            Margin = new Padding(0, 0, 0, 16),
            Padding = new Padding(16)
        };
    }

    private ToggleSwitch CreateToggle(string label, bool initialChecked)
    {
        var toggle = new ToggleSwitch
        {
            LabelText = label,
            Checked = initialChecked,
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 16, 16),
            MinimumSize = new Size(200, 28)
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
    /// Handles manual password typing by the user to sync UI controls.
    /// </summary>
    private void OnPasswordTextChanged(object? sender, EventArgs e)
    {
        if (_isUpdatingControls) return;
        
        string pass = _txtPassword.Text;
        if (string.IsNullOrEmpty(pass))
        {
            _strengthMeter.EntropyBits = 0;
            _strengthMeter.CrackTimeText = "";
            return;
        }

        _isUpdatingControls = true;

        // Sync Length
        int newLength = pass.Length;
        _numLength.Value = Math.Clamp(newLength, _numLength.Minimum, _numLength.Maximum);
        _trackLength.Value = Math.Clamp(newLength, _trackLength.Minimum, _trackLength.Maximum);

        // Sync Toggles based on typed characters
        _toggleUppercase.Checked = pass.Any(char.IsUpper);
        _toggleLowercase.Checked = pass.Any(char.IsLower);
        _toggleDigits.Checked = pass.Any(char.IsDigit);
        
        // Symbols check (basic heuristic)
        string specials = "!@#$%^&*()_+-=[]{}|;':\",./<>?";
        string extended = "~`\\";
        string ambiguous = "O0l1";
        
        _toggleSpecialSymbols.Checked = pass.Any(c => specials.Contains(c));
        _toggleExtendedSymbols.Checked = pass.Any(c => extended.Contains(c));
        
        // If they typed an ambiguous char, we check it? Actually, if they type one, maybe turn on the "Exclude" toggle?
        // Wait, the toggle is "EXCLUDE Ambiguous". If they type an ambiguous char, it means they DID NOT exclude it.
        if (pass.Any(c => ambiguous.Contains(c)))
            _toggleExcludeAmbiguous.Checked = false;

        _toggleIncludeSpaces.Checked = pass.Contains(' ');

        // Sync Bits and Strength Meter
        var options = ReadCurrentOptions();
        var poolSize = PasswordGenerator.CalculatePoolSize(options);
        var bits = PasswordGenerator.CalculateEntropyBits(newLength, poolSize);
        _numBits.Value = (decimal)Math.Clamp(bits, (double)_numBits.Minimum, (double)_numBits.Maximum);

        // We use the actual typed password's entropy for the strength meter, not the theoretical max
        UpdateCrackTimeDisplay();

        _isUpdatingControls = false;
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

    private void GeneratePassword()
    {
        var options = ReadCurrentOptions();
        var password = PasswordGenerator.Generate(options);
        _txtPassword.Text = password;
        
        UpdateCrackTimeDisplay();
    }

    private void OnThreatLevelChanged(object? sender, EventArgs e)
    {
        if (_isUpdatingControls) return;
        UpdateThreatDescription();
        UpdateCrackTimeDisplay();
    }

    private void UpdateThreatDescription()
    {
        ulong hashRate = GetCurrentHashRate();
        var profile = GetProfileForHashRate(hashRate);
        string formattedHashRate = FormatHashRateString(hashRate);
        _lblThreatDesc.Text = $"Attacker Power: {formattedHashRate}\nTarget Profile: {profile.Title}\n{profile.Description}";
    }

    private void UpdateCrackTimeDisplay()
    {
        string pass = _txtPassword.Text;
        if (string.IsNullOrEmpty(pass)) return;
        
        ulong hashRate = GetCurrentHashRate();
        bool isQuantum = _toggleQuantum.Checked;
        
        _strengthMeter.QuantumThreatActive = isQuantum;
        _strengthMeter.EntropyBits = PasswordGenerator.CalculateEntropyBits(pass);
        
        var crackTimeMs = PasswordGenerator.CalculateCrackTimeMilliseconds(pass, hashRate, isQuantum);
        _strengthMeter.CrackTimeText = PasswordGenerator.FormatCrackTime(crackTimeMs);
    }

    private ulong GetCurrentHashRate()
    {
        double exponent = _trackThreatLevel.Value / 100.0;
        return (ulong)Math.Pow(10, exponent);
    }

    private PasswordGenerator.ThreatProfile GetProfileForHashRate(ulong hashRate)
    {
        var profiles = PasswordGenerator.ThreatProfiles;
        for (int i = profiles.Length - 1; i >= 0; i--)
        {
            if (hashRate >= (ulong)profiles[i].HashesPerSecond)
            {
                return profiles[i];
            }
        }
        return profiles[0];
    }

    private string FormatHashRateString(ulong hashRate)
    {
        string exact = $"{hashRate:N0} H/s";
        if (hashRate >= 1_000_000_000_000_000) return $"{exact} (1 PetaHash/s)";
        if (hashRate >= 1_000_000_000_000) return $"{exact} ({hashRate / 1_000_000_000_000.0:F1} TH/s)";
        if (hashRate >= 1_000_000_000) return $"{exact} ({hashRate / 1_000_000_000.0:F1} GH/s)";
        if (hashRate >= 1_000_000) return $"{exact} ({hashRate / 1_000_000.0:F1} MH/s)";
        if (hashRate >= 1_000) return $"{exact} ({hashRate / 1_000.0:F1} kH/s)";
        return exact;
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
