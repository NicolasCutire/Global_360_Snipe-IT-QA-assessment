## Snipe-IT QA Assessment (Playwright + .NET 8)

Automation that covers the end‑to‑end asset flow in the Snipe‑IT demo: login, create asset, verify in list, validate details, and confirm history.

### Scenario covered
1) Login: `admin` / `password` to `https://demo.snipeitapp.com/login`
2) Create asset: Macbook Pro 13" with status “Ready to Deploy” and checkout to a random user
3) Verify: search in list by the generated Asset Tag and open the asset
4) Validate details: Tag, Model, Status, and Assigned user on the Details tab
5) History: entries include “create new” and “checkout”

### Prerequisites
- .NET 8 SDK
- Internet access

### Install and run
```bash
dotnet restore
dotnet run             # installs Playwright browsers
dotnet test --logger "console;verbosity=detailed"
```

### Project layout
```
Pages/
  LoginPage.cs           # login flow
  AssetsPage.cs          # list/search/open
  CreateAssetPage.cs     # create form (Select2 + status + checkout to)
  AssetDetailsPage.cs    # details + history tab parsing
Tests/
  SnipeItAssetCreationTest.cs
Configuration/
  TestConfiguration.cs   # base URLs and constants
```

### Key design choices
- Page Object Model with explicit, stable selectors for the demo UI
- Direct navigation to `/hardware/create` for reliability, then defensive waits
- Select2 interactions (type to filter, choose first match) for Model and User
- Case/whitespace‑insensitive assertions for Model/Status/AssignedTo
- History parsed as `#history tbody tr` to match the demo DOM

### Troubleshooting
- First run fails with browser missing → run `dotnet run` to install browsers
- “Not found” in list → we click Refresh and wait for the loading overlay to hide
- Saving doesn’t navigate → we click `#submit_button` and wait for navigation (NetworkIdle)
- Details mismatch → selectors target Details tab; we activate it before reads

### Notes
- This suite targets the public demo. If the DOM changes, update selectors in the `Pages/` classes.
