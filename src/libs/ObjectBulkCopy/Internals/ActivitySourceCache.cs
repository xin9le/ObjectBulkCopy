using System.Diagnostics;

namespace ObjectBulkCopy.Internals;



/// <summary>
/// Provides a caching mechanism for <see cref="ActivitySource"/>.
/// </summary>
internal static class ActivitySourceCache
{
    #region Fields
#pragma warning disable OBC001
    private static readonly ActivitySource s_activitySource = new(ActivitySourceNames.Default);
#pragma warning restore OBC001
    #endregion


    #region Propeties
    /// <summary>
    /// Gets singleton instance.
    /// </summary>
    public static ActivitySource Instance
        => s_activitySource;
    #endregion


    #region Constructors
    /// <summary>
    /// 
    /// </summary>
    static ActivitySourceCache()
    {
        var listener = new ActivityListener();
        listener.ShouldListenTo += static src => src.Name == s_activitySource.Name;
        listener.Sample += static (ref options) => ActivitySamplingResult.AllDataAndRecorded;
        listener.SampleUsingParentId += static (ref options) => ActivitySamplingResult.AllDataAndRecorded;
        ActivitySource.AddActivityListener(listener);
    }
    #endregion
}
