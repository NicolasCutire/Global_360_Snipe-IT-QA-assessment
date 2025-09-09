using Microsoft.Playwright;

namespace SnipeItQAAssessment.Pages;

public class AssetDetailsPage
{
    private readonly IPage _page;
    
    // Selectors adapted to Snipe-IT details DOM
    private const string AssetTagSelector = ".js-copy-assettag"; // span containing asset tag
    private const string DetailsTab = "a[href='#details']";
    private const string DetailsPanelPrimary = "#details";
    private const string DetailsPanelAlt = ".info-stack .row-new-striped";
    private const string HistoryTab = "a[href='#history']";
    private const string HistoryContent = "#history";
    private const string HistoryEntries = "#history tbody tr";
    
    public AssetDetailsPage(IPage page)
    {
        _page = page;
    }
    
    public async Task EnsureDetailsTabAsync()
    {
        var detailsTab = _page.Locator(DetailsTab);
        if (await detailsTab.CountAsync() > 0)
        {
            await detailsTab.First.ClickAsync();
        }
        var panel = _page.Locator(DetailsPanelPrimary);
        if (await panel.CountAsync() == 0)
        {
            panel = _page.Locator(DetailsPanelAlt);
        }
        await panel.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
    }

    public async Task<bool> IsAssetDetailsVisible()
    {
        await EnsureDetailsTabAsync();
        return await _page.Locator(AssetTagSelector).IsVisibleAsync();
    }
    
    public async Task<string> GetAssetTag()
    {
        await EnsureDetailsTabAsync();
        var tag = _page.Locator(AssetTagSelector);
        await tag.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        return (await tag.InnerTextAsync()).Trim();
    }
    
    public async Task<string> GetAssetModel()
    {
        await EnsureDetailsTabAsync();
        // Row with strong 'Model' then the value anchor in the .col-md-9
        var row = _page.Locator("div.row:has(> .col-md-3 > strong:has-text('Model')) > .col-md-9");
        if (await row.CountAsync() == 0) return string.Empty;
        var link = row.First.Locator("a");
        if (await link.CountAsync() > 0)
        {
            return ((await link.First.InnerTextAsync()) ?? string.Empty).Trim();
        }
        return ((await row.First.InnerTextAsync()) ?? string.Empty).Trim();
    }
    
    public async Task<string> GetAssetStatus()
    {
        await EnsureDetailsTabAsync();
        var row = _page.Locator("div.row:has(strong:has-text('Status')) .col-md-9");
        if (await row.CountAsync() == 0) return string.Empty;
        return ((await row.First.InnerTextAsync()) ?? string.Empty).Trim();
    }
    
    public async Task<string> GetAssignedTo()
    {
        await EnsureDetailsTabAsync();
        // Status row includes assigned user link
        var link = _page.Locator("div.row:has(strong:has-text('Status')) .col-md-9 a[href*='/users/']");
        if (await link.CountAsync() == 0) return string.Empty;
        return ((await link.First.InnerTextAsync()) ?? string.Empty).Trim();
    }
    
    public async Task ClickHistoryTab()
    {
        var tab = _page.Locator(HistoryTab);
        if (await tab.CountAsync() > 0)
        {
            await tab.First.ClickAsync();
            // Wait for the table rows to load inside the history tab
            var rows = _page.Locator(HistoryEntries);
            await rows.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Attached });
        }
    }
    
    public async Task<bool> IsHistoryTabVisible()
    {
        return await _page.Locator(HistoryContent).IsVisibleAsync();
    }
    
    public async Task<int> GetHistoryEntriesCount()
    {
        var rows = _page.Locator(HistoryEntries);
        return await rows.CountAsync();
    }
    
    public async Task<List<string>> GetHistoryEntries()
    {
        var rows = _page.Locator(HistoryEntries);
        int count = await rows.CountAsync();
        var list = new List<string>(capacity: count);
        for (int i = 0; i < count; i++)
        {
            var text = await rows.Nth(i).InnerTextAsync();
            if (!string.IsNullOrWhiteSpace(text)) list.Add(text.Trim());
        }
        return list;
    }
}
