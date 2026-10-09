using System.Text.Json;

namespace AditiKraft.Krafter.UI.Web.Client.Common.Components.Grids;

public partial class AdvancedDataGrid<TItem> : ComponentBase where TItem : notnull
{
    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    [Parameter] public IEnumerable<TItem>? Data { get; set; }
    [Parameter] public int Count { get; set; }
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public EventCallback<LoadDataArgs> LoadData { get; set; }
    [Parameter] public RenderFragment? Columns { get; set; }
    [Parameter] public RenderFragment? ToolbarActions { get; set; }

    [Parameter] public bool ShowCellDataAsTooltip { get; set; } = true;
    [Parameter] public Density Density { get; set; } = Density.Compact;
    [Parameter] public Density MobileDensity { get; set; } = Density.Compact;
    [Parameter] public bool UseAdaptiveDensity { get; set; } = true;

    [Parameter] public bool AllowColumnResize { get; set; } = true;
    [Parameter] public bool AllowColumnPicking { get; set; } = true;
    [Parameter] public bool AllowFiltering { get; set; } = true;
    [Parameter] public FilterMode FilterMode { get; set; } = FilterMode.Advanced;
    [Parameter] public LogicalFilterOperator LogicalFilterOperator { get; set; } = LogicalFilterOperator.Or;
    [Parameter] public bool AllowSorting { get; set; } = true;
    [Parameter] public bool AllowMultiColumnSorting { get; set; } = true;
    [Parameter] public bool AllowPaging { get; set; } = true;
    [Parameter] public bool ShowPagingSummary { get; set; } = true;
    [Parameter] public int PageSize { get; set; } = 10;
    [Parameter] public int MobilePageSize { get; set; } = 10;
    [Parameter] public IEnumerable<int> PageSizeOptions { get; set; } = [10, 20, 50, 100, 500];
    [Parameter] public int PageNumbersCount { get; set; } = 5;
    [Parameter] public bool PagerAlwaysVisible { get; set; } = true;
    [Parameter] public string PagingSummaryFormat { get; set; } = "Showing {0}-{1} of {2}";
    [Parameter] public bool AllowGrouping { get; set; } = true;
    [Parameter] public string GroupPanelText { get; set; } = "Drag a column header here to group results";

    [Parameter] public bool AllowVirtualization { get; set; } = true;
    [Parameter] public string GridStyle { get; set; } = "max-height: 70vh; overflow: auto;";

    [Parameter] public bool ShowSkeletonWhenLoading { get; set; } = true;
    [Parameter] public int SkeletonRows { get; set; } = 10;

    [Parameter] public bool ShowExportControls { get; set; } = true;
    [Parameter] public bool ShowResetButton { get; set; } = true;
    [Parameter] public int ExportChunkSize { get; set; } = 50;
    [Parameter] public string ExportFileBaseName { get; set; } = "grid-data";
    [Parameter] public string ExportTitle { get; set; } = "Export";
    [Parameter] public bool ExportUseTheme { get; set; } = true;
    [Parameter] public bool EnableStateManager { get; set; } = true;
    [Parameter] public string ComponentKey { get; set; } = string.Empty;
    [Parameter] public EventCallback OnFilterCleared { get; set; }
    [Parameter] public EventCallback OnGridReset { get; set; }

    private RadzenDataGrid<TItem>? _grid;
    private bool _isMobile;
    private bool _isExporting;
    private int _loadedRows;
    private int _totalRows;
    private string? _statusMessage;
    private DataGridSettings? _settings;
    private int _gridStateResetVersion;

    private Density EffectiveDensity => UseAdaptiveDensity && _isMobile ? MobileDensity : Density;
    private int EffectivePageSize => UseAdaptiveDensity && _isMobile ? MobilePageSize : PageSize;
    private IReadOnlyList<int> EffectivePageSizeOptions => BuildPageSizeOptions(Count, PageSizeOptions);
    private double ExportProgressValue => _totalRows > 0
        ? Math.Round(_loadedRows * 100d / _totalRows)
        : 0;
    private string CurrentStateJson => JsonSerializer.Serialize(_settings ?? new DataGridSettings());

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        try
        {
            _isMobile = await JsRuntime.InvokeAsync<bool>("isMobile").ConfigureAwait(false);
            await InvokeAsync(StateHasChanged);
        }
        catch (JSException)
        {
            _isMobile = false;
        }
    }

    private static IReadOnlyList<int> BuildPageSizeOptions(int totalCount, IEnumerable<int> configuredOptions)
    {
        List<int> orderedOptions = configuredOptions
            .Distinct()
            .OrderBy(size => size)
            .ToList();

        if (orderedOptions.Count == 0)
        {
            return [10];
        }

        if (totalCount <= orderedOptions[0])
        {
            return [orderedOptions[0]];
        }

        List<int> effectiveOptions = [];

        foreach (int option in orderedOptions)
        {
            if (option < totalCount)
            {
                effectiveOptions.Add(option);
                continue;
            }

            effectiveOptions.Add(option == totalCount ? option : totalCount);
            break;
        }

        if (effectiveOptions.Count == 0)
        {
            return orderedOptions;
        }

        int lastConfiguredOption = orderedOptions[^1];
        if (totalCount > lastConfiguredOption && effectiveOptions[^1] != lastConfiguredOption)
        {
            effectiveOptions.Add(lastConfiguredOption);
        }

        return effectiveOptions;
    }

    private async Task ResetGridAsync()
    {
        _statusMessage = null;

        if (_grid is null)
        {
            return;
        }

        _settings = null;
        _gridStateResetVersion++;

        await _grid.FirstPage(true).ConfigureAwait(false);
        _grid.Reset(true);
        await _grid.Reload().ConfigureAwait(false);

        if (OnGridReset.HasDelegate)
        {
            await OnGridReset.InvokeAsync().ConfigureAwait(false);
        }
    }

    private async Task OnFilterClearedInternalAsync()
    {
        if (OnFilterCleared.HasDelegate)
        {
            await OnFilterCleared.InvokeAsync().ConfigureAwait(false);
        }

        if (_grid is not null)
        {
            await _grid.FirstPage().ConfigureAwait(false);
        }
    }

    private async Task ApplyStateJsonAsync(string stateJson)
    {
        if (string.IsNullOrWhiteSpace(stateJson))
        {
            return;
        }

        try
        {
            DataGridSettings? settings = JsonSerializer.Deserialize<DataGridSettings>(stateJson);
            if (settings is null)
            {
                return;
            }

            _settings = settings;
            await InvokeAsync(StateHasChanged);

            if (_grid is not null)
            {
                await _grid.Reload().ConfigureAwait(false);
            }
        }
        catch (JsonException)
        {
            _statusMessage = "Failed to apply saved grid state.";
        }
    }

    private async Task ExportExcelAsync()
    {
        if (_grid is null)
        {
            return;
        }

        _isExporting = true;
        _loadedRows = 0;
        _totalRows = Count;
        _statusMessage = null;

        try
        {
            await _grid.ExportToExcelAsync($"{ExportFileBaseName}-{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx",
                new DataGridExcelExportOptions
                {
                    Scope = DataGridExportScope.All,
                    UseTheme = ExportUseTheme,
                    Title = ExportTitle,
                    ChunkSize = ExportChunkSize,
                    Progress = OnExportProgress
                }).ConfigureAwait(false);

            _statusMessage = "Excel export completed.";
        }
        finally
        {
            _isExporting = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task ExportCsvAsync()
    {
        if (_grid is null)
        {
            return;
        }

        _isExporting = true;
        _loadedRows = 0;
        _totalRows = Count;
        _statusMessage = null;

        try
        {
            await _grid.ExportToCsvAsync($"{ExportFileBaseName}-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv",
                new DataGridCsvExportOptions
                {
                    Scope = DataGridExportScope.All,
                    ChunkSize = ExportChunkSize,
                    Progress = OnExportProgress
                }).ConfigureAwait(false);

            _statusMessage = "CSV export completed.";
        }
        finally
        {
            _isExporting = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private void OnExportProgress(DataGridExportProgress progress)
    {
        _loadedRows = progress.Loaded;
        _totalRows = progress.Total;
        _ = InvokeAsync(StateHasChanged);
    }
}
