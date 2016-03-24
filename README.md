# HttpListener for .NET Core

This is a library that fills the void for the missing System.Net.Http.HttpListener in .NET Core.

Taking a modern approach, this API is not meant to be entirely compatible with the HttpListener found in the .NET Framework on the desktop.

Please, be aware that this is an early concept, and thus not ready for production.

## Sample

```CSharp
    var listener = new HttpListener(IPAddress.Parse("127.0.0.1"), 8081);
    listener.Request += async (sender, context) => {
        var request = context.Request;
        var response = context.Response;
        if(request.HttpMethod == HttpMethod.Get) 
        {
            await response.WriteAsync($"Hello from Server at: {DateTime.Now}\r\n");
        }
        else
        {
            response.MethodNotAllowed();
        }
    };
    listener.Start();
```

Visit 127.0.0.1:8081 in your browser.
