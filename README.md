# FluffySpoon.Templates
A library allowing users to use Razor as markup in a template, with API operations from ASP .NET Controllers without making HTTP requests.

## Usage
To use the templating system, 3 steps are required.

### Wiring up
Add the following to the `Startup.cs` file's `ConfigureServices` method right after the `services.AddMvc` invocation.

`services.AddFluffySpoonTemplating();`

### Writing some API controllers
Let's say we have the following two controllers.

```csharp
public class GroupController : Controller
{
	[HttpGet("api/groups")]
	public string[] GetAllGroups()
	{
		return new[]
		{
			"foo", "bar"
		};
	}
}

public class UserController : Controller
{
	[HttpGet("api/users/{userId}")]
	public string GetUsername(int userId)
    {
        return "username" + userId;
    }
}
```

### Writing a view
The above controllers can of course be invoked via HTTP(S) calls, but `FluffySpoon.Templates` allow using the API in Razor directly, without making HTTP(S) calls at all.

**Views/MyView.cshtml**
```razor
@model ApiModel

<h1>Welcome @Model.Get("/api/users/1337")</h1>

@foreach(var groupName in Model.GetCollection<string>("/api/groups"))
{
    <h2>@groupName</h2>
}
```

### Rendering the view
If we want to render the view, we can then write a controller that does it for us.

```csharp
public class HomeController : Controller
{
    private readonly IFluffySpoonTemplateRenderer _templateRenderer;

    public HomeController(
        IFluffySpoonTemplateRenderer templateRenderer)
    {
        _templateRenderer = templateRenderer;
    }

    public async Task<IActionResult> Index()
    {
		//here we specify that only UserController and GroupController can be invoked via the view.
        var html = await _templateRenderer.RenderAsync(
            "MyView",
            new UserController(),
            new GroupController());
        return Content(html, "text/html");
    }
}
```

The result is seen below.

```html
<h1>Welcome username1337</h1>

<h2>foo</h2>
<h2>bar</h2>
```