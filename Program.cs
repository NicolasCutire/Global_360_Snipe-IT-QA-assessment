using Microsoft.Playwright;

namespace SnipeItQAAssessment;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Install Playwright browsers if not already installed
        Microsoft.Playwright.Program.Main(new[] { "install" });
        
        Console.WriteLine("Playwright browsers installed successfully!");
        Console.WriteLine("You can now run the tests using: dotnet test");
    }
}
