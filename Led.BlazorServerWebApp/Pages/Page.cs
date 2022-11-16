using Led.BlazorServerWebApp.Shared;
using Microsoft.AspNetCore.Components;

namespace Led.BlazorServerWebApp.Pages;

public class Page : ComponentBase
{
    private string title;

    [CascadingParameter] public MainLayout Layout { get; set; }
    public string Title
    {
        get => title; 
        set
        {
            title = value;
            Layout.Title = Title;
        }
    }
}
