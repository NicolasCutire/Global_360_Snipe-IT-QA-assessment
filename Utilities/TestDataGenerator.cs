namespace SnipeItQAAssessment.Utilities;

public static class TestDataGenerator
{
    private static readonly Random _random = new();
    
    public static string GenerateAssetName(string baseName = "MacBook Pro 13\"")
    {
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var randomSuffix = _random.Next(1000, 9999);
        return $"{baseName} - {timestamp}-{randomSuffix}";
    }
    
    public static string GenerateAssetTag(string prefix = "MBP13")
    {
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var randomSuffix = _random.Next(100, 999);
        return $"{prefix}-{timestamp}-{randomSuffix}";
    }
    
    public static string GetRandomUser()
    {
        var users = new[]
        {
            "Admin User",
            "Test User",
            "Demo User",
            "QA User"
        };
        
        return users[_random.Next(users.Length)];
    }
    
    public static string GetRandomEmail()
    {
        var domains = new[] { "example.com", "test.com", "demo.com" };
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var randomSuffix = _random.Next(100, 999);
        var domain = domains[_random.Next(domains.Length)];
        
        return $"testuser{timestamp}{randomSuffix}@{domain}";
    }
}
