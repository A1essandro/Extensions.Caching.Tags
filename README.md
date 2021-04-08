# Extensions.Caching.Tags

Marking _IMemoryCache_ entries by tags for easier removing.

## Installation

EF Core is available on [NuGet](https://www.nuget.org/packages/Extensions.Caching.Tags/).

`dotnet add package Extensions.Caching.Tags`

## Usage

Use registration of service for `IMemoryCache` by general way:

```cs
using Microsoft.Extensions.DependencyInjection;

//...

services.AddMemoryCache();
```

After getting service you may add tag or tags for caching entries. Just add it as last parameter:

```cs
_cache.Set(key, value, optionsOrExpiration, new CacheTags("tag0", "tag1"));
_cache.Set(key, value, optionsOrExpiration, new CacheTags { "tag0", "tag1" }); //similar behaviour
_cache.Set(key, value, optionsOrExpiration, new[] {"tag0", "tag1"}); //similar behaviour
```

`CacheTags` is inherited class from `List<string>`.

After that you can remove entries from cache by this way:

```cs
_cache.RemoveByTag("tag0"); //will remove all entries tagged by "tag0"
```

## Contribute

Contributions to the package are always welcome!

## License

The code base is licensed under the Apache License v2.0.

## References

Some methods of this package are wrappers for methods from [Microsoft.Extensions.Caching.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.Caching.Abstractions/). Also this package is using namespace _Microsoft.Extensions.Caching.Memory_ for easier use.

See [Caching](https://github.com/aspnet/Caching)
