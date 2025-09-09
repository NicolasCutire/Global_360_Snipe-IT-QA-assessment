using Microsoft.Playwright;
using FluentAssertions;
using System.Text.RegularExpressions;

namespace SnipeItQAAssessment.Pages;

public class CreateAssetPage
{
    private readonly IPage _page;
    
    // Selectors
    private const string AssetTagField = "#asset_tag";
    // Native selects exist but wrapped by Select2; prefer interacting via Select2 widgets
    private const string ModelSelect = "#model_select_id";
    private const string ModelSelect2Display = "#select2-model_select_id-container";
    private const string StatusSelect = "#status_select_id";
    private const string StatusSelect2Display = "#select2-status_select_id-container";
    private const string AssignedUserSelect = "#assigned_user_select";
    private const string AssignedUserSelect2Display = "#select2-assigned_user_select-container";
    private const string CheckoutToUserRadio = "input[name='checkout_to_type'][value='user']";
    private const string CheckoutToUserLabel = "label:has(input[name='checkout_to_type'][value='user'])";
    private const string Select2SearchInput = ".select2-search__field";
    private const string Select2Results = ".select2-results__option[role='option']";
    private const string SaveButton = "#submit_button";
    private const string SuccessMessage = ".alert-success";
    private const string ErrorMessage = ".alert-danger";
    
    public CreateAssetPage(IPage page)
    {
        _page = page;
    }
    
    public async Task<string> GetAssetTagFromForm()
    {
        var tagInput = _page.Locator(AssetTagField);
        await tagInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Attached, Timeout = SnipeItQAAssessment.Configuration.TestConfiguration.DefaultTimeout });
        
        // Poll until the field has a non-empty, auto-generated value
        for (int i = 0; i < 20; i++)
        {
            var value = await tagInput.InputValueAsync();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }
            await _page.WaitForTimeoutAsync(250);
        }
        
        // Last attempt
        return (await tagInput.InputValueAsync()).Trim();
    }
    
    public async Task SelectModel(string queryText)
    {
        // Open Select2 dropdown for model
        await _page.ClickAsync(ModelSelect2Display);
        // Type query to filter
        await _page.FillAsync(Select2SearchInput, queryText);
        // Click the first matching option
        var option = _page.Locator(Select2Results).Filter(new LocatorFilterOptions { HasTextString = queryText }).First;
        await option.ClickAsync();
    }
    
    public async Task SelectStatus(string statusName)
    {
        // Use native select where available for reliability
        await _page.SelectOptionAsync(StatusSelect, new SelectOptionValue { Label = statusName });
    }
    
    public async Task<string> SelectRandomAssignedUser()
    {
        // Ensure the checkout mode is set to User
        var userRadio = _page.Locator(CheckoutToUserRadio);
        var userLabel = _page.Locator(CheckoutToUserLabel);
        await userLabel.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        // Click the label (Bootstrap toggles classes via label click)
        await userLabel.ClickAsync(new LocatorClickOptions { Force = true });
        // Confirm radio is checked or label is active
        for (int i = 0; i < 5; i++)
        {
            if (await userRadio.IsCheckedAsync() || (await userLabel.GetAttributeAsync("class"))?.Contains("active") == true)
            {
                break;
            }
            await _page.WaitForTimeoutAsync(200);
        }

        // Open Select2 dropdown
        await _page.ClickAsync(AssignedUserSelect2Display);
        // Trigger AJAX population by typing a wildcard letter
        await _page.FillAsync(Select2SearchInput, "a");
        // Wait for results to appear
        var results = _page.Locator(Select2Results);
        await results.First.WaitForAsync();
        
        var count = await results.CountAsync();
        if (count == 0)
        {
            throw new Exception("No users available to assign.");
        }
        
        // Choose a random option (avoid any with 'Select a User')
        for (int i = 0; i < count; i++)
        {
            var text = await results.Nth(i).InnerTextAsync();
            if (!string.IsNullOrWhiteSpace(text) && !text.Contains("Select a User", StringComparison.OrdinalIgnoreCase))
            {
                await results.Nth(i).ClickAsync();
                // Read the selected text from the Select2 display and normalize to just the full name
                var selected = (await _page.InnerTextAsync(AssignedUserSelect2Display))?.Trim() ?? string.Empty;
                // Remove leading symbols like "×"
                selected = Regex.Replace(selected, "^\\s*[×x]\\s*", "");
                // Drop username in parentheses and trailing id (e.g., "(jdoe) - #123")
                selected = Regex.Replace(selected, "\\s*\\([^)]*\\)", "");
                selected = Regex.Replace(selected, "\\s*-\\s*#\\d+\\s*$", "");
                return selected.Trim();
            }
        }
        
        throw new Exception("Failed to select a user from the dropdown.");
    }
    
    public async Task SaveAsset()
    {
        var save = _page.Locator(SaveButton).First;
        await save.ScrollIntoViewIfNeededAsync();
        await save.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        // Prefer waiting for navigation away from the create page
        await _page.RunAndWaitForNavigationAsync(async () =>
        {
            await save.ClickAsync(new LocatorClickOptions { Timeout = SnipeItQAAssessment.Configuration.TestConfiguration.DefaultTimeout });
        }, new PageRunAndWaitForNavigationOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });
    }
    
    public async Task<bool> IsAssetCreatedSuccessfully()
    {
        var hasErrorMessage = await _page.Locator(ErrorMessage).IsVisibleAsync();
        if (hasErrorMessage)
        {
            return false;
        }
        // If we are no longer on the create page, treat as success
        if (!_page.Url.Contains("/hardware/create", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        // Fallback to success flash
        var hasSuccessMessage = await _page.Locator(SuccessMessage).IsVisibleAsync();
        return hasSuccessMessage;
    }
    
    public async Task<string> GetSuccessMessage()
    {
        if (await _page.IsVisibleAsync(SuccessMessage))
        {
            return await _page.TextContentAsync(SuccessMessage) ?? string.Empty;
        }
        return string.Empty;
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
