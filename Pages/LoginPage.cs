using Microsoft.Playwright;
using FluentAssertions;
using SnipeItQAAssessment.Configuration;

namespace SnipeItQAAssessment.Pages;

public class LoginPage
{
    private readonly IPage _page;
    
    // Selectors
    private const string UsernameField = "input[name='username']";
    private const string PasswordField = "input[name='password']";
    private const string LoginButton = "button[type='submit']";
    private const string ErrorMessage = ".alert-danger";
    
    public LoginPage(IPage page)
    {
        _page = page;
    }
    
    public async Task NavigateToLoginPage()
    {
        await _page.GotoAsync(TestConfiguration.LoginUrl);
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
    
    public async Task LoginAsync(string username, string password)
    {
        await _page.FillAsync(UsernameField, username);
        await _page.FillAsync(PasswordField, password);
        await _page.ClickAsync(LoginButton);
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
    
    public async Task<bool> IsLoginSuccessful()
    {
        // Check if we're redirected to dashboard or if login error is shown
        var currentUrl = _page.Url;
        var hasError = await _page.IsVisibleAsync(ErrorMessage);
        
        return !hasError && !currentUrl.Contains("/login");
    }
    
    public async Task<string> GetErrorMessage()
    {
        if (await _page.IsVisibleAsync(ErrorMessage))
        {
            return await _page.TextContentAsync(ErrorMessage) ?? string.Empty;
        }
        return string.Empty;
    }
}
