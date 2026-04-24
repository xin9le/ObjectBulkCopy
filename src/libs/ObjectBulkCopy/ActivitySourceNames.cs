using System.Diagnostics.CodeAnalysis;

namespace ObjectBulkCopy;



/// <summary>
/// Provides well-known activity source names used for tracing and diagnostics within the application.
/// </summary>
[Experimental("OBC001", UrlFormat = "https://github.com/dotnet/SqlClient/issues/2210#issuecomment-4281339991")]
public static class ActivitySourceNames
{
    /// <summary>
    /// Represents the default name.
    /// </summary>
    public const string Default = "ObjectBulkCopy";
}
