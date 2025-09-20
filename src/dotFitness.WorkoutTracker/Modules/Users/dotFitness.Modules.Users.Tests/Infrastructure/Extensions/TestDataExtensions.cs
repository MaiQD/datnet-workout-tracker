namespace dotFitness.Modules.Users.Tests.Infrastructure.Extensions;

public static class TestDataExtensions
{
    private static readonly Random _random = new();
    private static readonly HashSet<string> _usedEmails = new();
    private static int _dateCounter = 0;
    
    /// <summary>
    /// Generates a unique email address for testing
    /// </summary>
    public static string GenerateUniqueEmail(this object testInstance)
    {
        string email;
        do
        {
            email = $"test{Guid.NewGuid():N}@example.com";
        } while (!_usedEmails.Add(email));
        
        return email;
    }
    
    /// <summary>
    /// Generates a unique date for testing
    /// Uses a fixed base date to avoid flaky tests, with a counter to ensure uniqueness
    /// </summary>
    public static DateTime GenerateUniqueDate(this object testInstance)
    {
        var baseDate = new DateTime(2024, 1, 1);
        var uniqueDays = Interlocked.Increment(ref _dateCounter);
        return baseDate.AddDays(uniqueDays);
    }
    
    /// <summary>
    /// Generates a unique user ID for testing (for non-existent user scenarios)
    /// </summary>
    public static int GenerateUniqueUserId(this object testInstance)
    {
        return _random.Next(10000, 99999); // High range to avoid conflicts with real user IDs
    }
    
    /// <summary>
    /// Clears all used test data (useful for test cleanup)
    /// </summary>
    public static void ClearTestData(this object testInstance)
    {
        _usedEmails.Clear();
        Interlocked.Exchange(ref _dateCounter, 0);
    }
}