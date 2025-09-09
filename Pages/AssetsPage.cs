using Microsoft.Playwright;
using FluentAssertions;
using SnipeItQAAssessment.Configuration;

namespace SnipeItQAAssessment.Pages;

public class AssetsPage
{
    private readonly IPage _page;
    
    // Selectors
    private const string CreateAssetButton = "button[name='btnAdd'], a[href*='/hardware/create']";
    private const string AssetTable = "#assetsListingTable";
    private const string AssetRows = "#assetsListingTable tbody tr";
    // We search by free-text using the table row's text, not a specific column
    private const string AssetStatusColumn = "td:nth-child(6)";
    private const string AssetLink = "a[href*='/hardware/']";
    private const string SearchField = "input[type='search']";
    private const string NextPageButton = "a[aria-label='Next']";
    private const string RefreshButton = "button[name='refresh']";
    private const string LoadingOverlay = ".fixed-table-loading";
    
    public AssetsPage(IPage page)
    {
        _page = page;
    }
    
    public async Task NavigateToAssetsPage()
    {
        await _page.GotoAsync(TestConfiguration.AssetsUrl);
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
    
    public async Task ClickCreateAsset()
    {
        // Navigate directly to the create asset page for reliability
        await _page.GotoAsync(TestConfiguration.CreateAssetUrl);
        // Network idle is enough; field values like asset tag are provided by the system
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
    
    public async Task<bool> IsAssetInList(string assetName)
    {
        // Search for the asset
        await _page.FillAsync(SearchField, assetName);
        // Click refresh to enforce the filter applies
        var refresh = _page.Locator(RefreshButton);
        if (await refresh.CountAsync() > 0)
        {
            await refresh.First.ClickAsync();
        }
        // Wait for loading overlay to disappear and table to settle
        await _page.Locator(LoadingOverlay).WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden, Timeout = TestConfiguration.DefaultTimeout });
        
        var assetRows = await _page.QuerySelectorAllAsync(AssetRows);
        foreach (var row in assetRows)
        {
            var rowText = (await row.InnerTextAsync()) ?? string.Empty;
            if (rowText.Contains(assetName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        
        return false;
    }
    
    public async Task<string> GetAssetStatus(string assetName)
    {
        await _page.FillAsync(SearchField, assetName);
        var refresh2 = _page.Locator(RefreshButton);
        if (await refresh2.CountAsync() > 0)
        {
            await refresh2.First.ClickAsync();
        }
        await _page.Locator(LoadingOverlay).WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden, Timeout = TestConfiguration.DefaultTimeout });
        
        var assetRows = await _page.QuerySelectorAllAsync(AssetRows);
        foreach (var row in assetRows)
        {
            var rowText = (await row.InnerTextAsync()) ?? string.Empty;
            if (rowText.Contains(assetName, StringComparison.OrdinalIgnoreCase))
            {
                var statusCell = await row.QuerySelectorAsync(AssetStatusColumn);
                return statusCell != null ? await statusCell.TextContentAsync() ?? string.Empty : string.Empty;
            }
        }
        
        return string.Empty;
    }
    
    public async Task ClickAssetLink(string assetName)
    {
        await _page.FillAsync(SearchField, assetName);
        var refresh3 = _page.Locator(RefreshButton);
        if (await refresh3.CountAsync() > 0)
        {
            await refresh3.First.ClickAsync();
        }
        await _page.Locator(LoadingOverlay).WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden, Timeout = TestConfiguration.DefaultTimeout });
        
        var assetRows = await _page.QuerySelectorAllAsync(AssetRows);
        foreach (var row in assetRows)
        {
            var rowText = (await row.InnerTextAsync()) ?? string.Empty;
            if (rowText.Contains(assetName, StringComparison.OrdinalIgnoreCase))
            {
                var link = await row.QuerySelectorAsync(AssetLink);
                if (link != null)
                {
                    await link.ClickAsync();
                    await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    return;
                }
            }
        }
        
        throw new Exception($"Asset '{assetName}' not found in the list");
    }
}
