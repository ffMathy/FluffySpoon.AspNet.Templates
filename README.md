# FluffySpoon.AspNet.Templates
A library allowing users to use Razor as markup in a template, with API operations from ASP .NET Controllers without making HTTP requests.

Useful if you are hosting a system (for instance a webshop system) where the user can modify his templates (cshtml files). By using this approach, the user gets all the flexibility of the shop system's API, but it is rendered server-side (so safe and hidden from the webbrowser).

This also has the benefit of you only having to worry about your own API and its scalability and stability. In other words, dogfeeding your own stuff.

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

## Is this safe for production use?
Yes. With this library, it is impossible to use anything else than `Model` and its properties in a view file. If you (for instance) try to invoke unsafe code like in `System.Reflection`, the view will not run and throw a `ViewValidationException`.

There is no magic going on in this library. I use Microsoft's routing engine, their Razor engine and action selector engine straight out of ASP .NET Core. This means that there is no funky behavior involved.

For the validation part, I use Roslyn to very strictly only allow any form of statement from `Model` in the view.
