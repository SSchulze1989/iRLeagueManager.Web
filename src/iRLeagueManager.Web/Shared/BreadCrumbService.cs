using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace iRLeagueManager.Web.Shared;

internal sealed class BreadCrumbService
{
    private List<BreadcrumbItem> items = new();
    public List<BreadcrumbItem> Items 
    {
        get => items; 
        set
        {
            if (items != value)
            {
                items = value;
                ItemsChanged?.Invoke(this, EventArgs.Empty);
            }
        } 
    } 
    
    public event EventHandler? ItemsChanged;

    public void Add(BreadcrumbItem item)
    {
        items.Add(item);
        ItemsChanged?.Invoke(this, EventArgs.Empty);
    }
}
