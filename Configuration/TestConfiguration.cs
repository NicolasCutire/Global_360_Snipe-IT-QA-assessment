namespace SnipeItQAAssessment.Configuration;

public static class TestConfiguration
{
    public const string BaseUrl = "https://demo.snipeitapp.com";
    public const string LoginUrl = $"{BaseUrl}/login";
    public const string AssetsUrl = $"{BaseUrl}/hardware";
    public const string CreateAssetUrl = $"{BaseUrl}/hardware/create";
    
    // Demo credentials
    public const string DemoUsername = "admin";
    public const string DemoPassword = "password";
    
    // Test data
    public const string MacBookProModel = "MacBook Pro 13\"";
    public const string ReadyToDeployStatus = "Ready to Deploy";
    public const string DefaultAssignedUser = "Admin User";
    
    // Timeouts
    public const int DefaultTimeout = 30000; // 30 seconds
    public const int ShortTimeout = 5000; // 5 seconds
    public const int LongTimeout = 60000; // 60 seconds
}
