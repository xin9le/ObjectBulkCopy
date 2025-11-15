# ObjectBulkCopy
A super simple C# library for performing [`SqlBulkCopy`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.data.sqlclient.sqlbulkcopy) directly on your CLR objects. No need to manually map properties--just plug in your list of objects and copy them to SQL Server with minimal setup. Ideal for high-performance data inserts in .NET applications.

[![Releases](https://img.shields.io/github/release/xin9le/ObjectBulkCopy.svg)](https://github.com/xin9le/ObjectBulkCopy/releases)
[![Nuget packages](https://img.shields.io/nuget/v/ObjectBulkCopy.svg)](https://www.nuget.org/packages/ObjectBulkCopy/)
[![GitHub license](https://img.shields.io/github/license/xin9le/ObjectBulkCopy)](https://github.com/xin9le/ObjectBulkCopy/blob/main/LICENSE)


## Support Platform
- .NET 8.0+


## How to use
```cs
using System.ComponentModel.DataAnnotations.Schema;

[Table("Users")]  // must set table name
public class User
{
    [Column("Id", Order = 0)]  // must set zero-based order
    public required int Id { get; init; }

    [Column("Name", Order = 1)]
    public required string Name { get; init; }
}
```
```cs
using ObjectBulkCopy;

using (var connection = new SqlConnection(...))
{
    User[] records
        = [
            new(){ Id = 0, Name = "xin9le" },
            new(){ Id = 1, Name = "Takaaki Suzuki" },
        ];
    const SqlBulkCopyOptions options = SqlBulkCopyOptions.Default;
    int? timeout = null;
    var affectedCount = await connection.BulkInsertAsync(records, options, timeout, cancellationToken);
}
```


## Installation
Getting started from downloading [NuGet](https://www.nuget.org/packages/ObjectBulkCopy) package.

```
dotnet add package ObjectBulkCopy
```


## License
This library is provided under [MIT License](http://opensource.org/licenses/MIT).


## Author
Takaaki Suzuki (a.k.a [@xin9le](https://twitter.com/xin9le)) is software developer in Japan who awarded Microsoft MVP for Developer Technologies (C#) since July 2012.
