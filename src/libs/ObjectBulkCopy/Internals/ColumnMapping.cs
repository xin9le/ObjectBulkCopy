using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Reflection;

namespace ObjectBulkCopy.Internals;



/// <summary>
/// Provides column mapping information.
/// </summary>
[DebuggerDisplay("{Order,nq}. {Name}")]
internal sealed class ColumnMapping
{
    #region Properties
    /// <summary>
    /// Gets the name of column.
    /// </summary>
    public string Name { get; }


    /// <summary>
    /// Gets the order of column.
    /// </summary>
    public int Order { get; }


    /// <summary>
    /// Gets the mapped property info.
    /// </summary>
    public PropertyInfo PropertyInfo { get; }
    #endregion


    #region Constructors
    /// <summary>
    /// Creates instance.
    /// </summary>
    public ColumnMapping(PropertyInfo property, int order)
    {
        var attr = property.GetCustomAttribute<ColumnAttribute>();
        this.Name = attr?.Name ?? property.Name;
        this.Order = attr?.Order ?? order;
        this.PropertyInfo = property;
    }
    #endregion
}
