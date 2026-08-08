using System.Security.Cryptography;
using System.Text;

namespace StrongPasswordGenerator;

/// <summary>
/// Cryptographically secure password generation engine using System.Security.Cryptography.RandomNumberGenerator.
/// </summary>
public static class PasswordGenerator
{
    #region Character Pool Constants

    private const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
    private const string DigitChars = "0123456789";
    private const string SpecialSymbolChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
    private const string ExtendedSymbolChars = "~`\\/\"'<>{}";
    private const string SpaceChars = " ";
    private static readonly HashSet<char> AmbiguousChars = ['O', '0', 'l', '1', 'I', '|', 'B', '8'];

    #endregion

    #region Public API

    /// <summary>
    /// Generates a cryptographically secure random password based on specified options.
    /// </summary>
    /// <param name="options">Configuration specifying character inclusions, exclusions, and target length.</param>
    /// <returns>A securely generated password string.</returns>
    public static string Generate(PasswordOptions options)
    {
        var activeSets = BuildActiveCharacterSets(options);
        if (activeSets.Count == 0 || options.Length <= 0)
        {
            return string.Empty;
        }

        var fullPoolBuilder = new StringBuilder();
        foreach (var set in activeSets)
        {
            fullPoolBuilder.Append(set);
        }
        var fullPool = fullPoolBuilder.ToString();

        if (string.IsNullOrEmpty(fullPool))
        {
            return string.Empty;
        }

        var passwordChars = new char[options.Length];
        var charIndex = 0;

        // Guaranteed set inclusion: Pick at least one char from each active set if length permits
        foreach (var set in activeSets)
        {
            if (charIndex >= options.Length)
            {
                break;
            }
            passwordChars[charIndex++] = set[RandomNumberGenerator.GetInt32(0, set.Length)];
        }

        // Fill remaining password length from full pool
        while (charIndex < options.Length)
        {
            passwordChars[charIndex++] = fullPool[RandomNumberGenerator.GetInt32(0, fullPool.Length)];
        }

        // Cryptographically secure Fisher-Yates shuffle
        SecureShuffle(passwordChars);

        return new string(passwordChars);
    }

    /// <summary>
    /// Calculates the total number of unique characters available based on options.
    /// </summary>
    /// <param name="options">Password configuration options.</param>
    /// <returns>The total size of the character pool.</returns>
    public static int CalculatePoolSize(PasswordOptions options)
    {
        var activeSets = BuildActiveCharacterSets(options);
        var uniqueChars = new HashSet<char>();
        foreach (var set in activeSets)
        {
            foreach (var c in set)
            {
                uniqueChars.Add(c);
            }
        }
        return uniqueChars.Count;
    }

    /// <summary>
    /// Calculates the entropy bits for a given password length and pool size.
    /// </summary>
    /// <param name="length">Password character length.</param>
    /// <param name="poolSize">Total unique character pool size.</param>
    /// <returns>Entropy value measured in bits.</returns>
    public static double CalculateEntropyBits(int length, int poolSize)
    {
        if (length <= 0 || poolSize <= 1)
        {
            return 0;
        }
        return Math.Round(length * Math.Log2(poolSize), 1);
    }

    /// <summary>
    /// Calculates required password length to achieve a target entropy bit count.
    /// </summary>
    /// <param name="targetBits">Desired entropy in bits.</param>
    /// <param name="poolSize">Total unique character pool size.</param>
    /// <returns>Required character length.</returns>
    public static int CalculateLengthFromBits(double targetBits, int poolSize)
    {
        if (targetBits <= 0 || poolSize <= 1)
        {
            return 4;
        }

        var bitsPerChar = Math.Log2(poolSize);
        var requiredLength = (int)Math.Ceiling(targetBits / bitsPerChar);
        return Math.Clamp(requiredLength, 4, 128);
    }

    #endregion

    #region Internal Helpers

    /// <summary>
    /// Filters and compiles active character sets based on configuration options.
    /// </summary>
    /// <param name="options">Options determining set inclusions and exclusions.</param>
    /// <returns>List of active character set strings.</returns>
    private static List<string> BuildActiveCharacterSets(PasswordOptions options)
    {
        var sets = new List<string>();

        if (options.IncludeUppercase) AddSet(sets, UppercaseChars, options.ExcludeAmbiguousCharacters);
        if (options.IncludeLowercase) AddSet(sets, LowercaseChars, options.ExcludeAmbiguousCharacters);
        if (options.IncludeDigits) AddSet(sets, DigitChars, options.ExcludeAmbiguousCharacters);
        if (options.IncludeSpecialSymbols) AddSet(sets, SpecialSymbolChars, options.ExcludeAmbiguousCharacters);
        if (options.IncludeExtendedSymbols) AddSet(sets, ExtendedSymbolChars, options.ExcludeAmbiguousCharacters);
        if (options.IncludeSpaces) AddSet(sets, SpaceChars, options.ExcludeAmbiguousCharacters);

        return sets;
    }

    /// <summary>
    /// Adds a character set to the list after applying ambiguous character filter if requested.
    /// </summary>
    private static void AddSet(List<string> targetList, string sourceSet, bool filterAmbiguous)
    {
        if (!filterAmbiguous)
        {
            targetList.Add(sourceSet);
            return;
        }

        var filteredBuilder = new StringBuilder();
        foreach (var c in sourceSet)
        {
            if (!AmbiguousChars.Contains(c))
            {
                filteredBuilder.Append(c);
            }
        }

        var filteredSet = filteredBuilder.ToString();
        if (filteredSet.Length > 0)
        {
            targetList.Add(filteredSet);
        }
    }

    /// <summary>
    /// Shuffles character array in-place using RandomNumberGenerator.GetInt32.
    /// </summary>
    private static void SecureShuffle(char[] array)
    {
        for (var i = array.Length - 1; i > 0; i--)
        {
            var randomIndex = RandomNumberGenerator.GetInt32(0, i + 1);
            (array[i], array[randomIndex]) = (array[randomIndex], array[i]);
        }
    }

    #endregion
}
