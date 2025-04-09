using System;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ObjectBulkCopy.Internals;



/// <summary>
/// Provides table mapping information.
/// </summary>
[DebuggerDisplay("{TableMappingExtensions.GetFullName(this),nq}")]
internal sealed class TableMapping
{
    #region Properties
    /// <summary>
    /// Gets the type that is mapped to the table.
    /// </summary>
    public Type Type { get; }


    /// <summary>
    /// Gets the schema name.
    /// </summary>
    public string? Schema { get; }


    /// <summary>
    /// Gets the table name.
    /// </summary>
    public string Name { get; }


    /// <summary>
    /// Gets the column mapping information.
    /// </summary>
    public ImmutableArray<ColumnMapping> Columns { get; }


    /// <summary>
    /// Gets the column mapping information by column order.
    /// </summary>
    internal FrozenDictionary<int, ColumnMapping> ColumnByOrder { get; }
    #endregion


    #region Constructors
    /// <summary>
    /// Creates instance.
    /// </summary>
    private TableMapping(Type type, TableAttribute? table, ImmutableArray<ColumnMapping> columns)
    {
        this.Type = type;
        this.Schema = table?.Schema;
        this.Name = table?.Name ?? type.Name;
        this.Columns = columns;
        this.ColumnByOrder = columns.ToFrozenDictionary(static x => x.Order);
    }
    #endregion


    #region Get
    /// <summary>
    /// Gets the table mapping information corresponding to the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static TableMapping Get<T>()
        => Cache<T>.Instance;
    #endregion


    #region Nested Types
    /// <summary>
    /// Provides <see cref="TableMapping"/> cache.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private static class Cache<T>
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static TableMapping Instance { get; }


        /// <summary>
        /// Static constructors
        /// </summary>
        static Cache()
        {
            var type = typeof(T);
            var table = type.GetCustomAttributes<TableAttribute>(true).FirstOrDefault();
            var columns
                = type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(static x => x.CanRead)  // must have getter
                .Where(static x => !x.IsDefined(typeof(NotMappedAttribute)))  // must not be annotated NotMappedAttribute
                .Select(static (x, index) => new ColumnMapping(x, index))
                .ToImmutableArray();
            Instance = new(type, table, columns);
        }
    }
    #endregion
}



/// <summary>
/// Provides extension methods for <see cref="TableMapping"/>.
/// </summary>
internal static class TableMappingExtensions
{
    /// <summary>
    /// Gets the full name of the table.
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    public static string GetFullName(this TableMapping table)
    {
        const char begin = '[';
        const char end = ']';
        return string.IsNullOrWhiteSpace(table.Schema)
            ? $"{begin}{table.Name}{end}"
            : $"{begin}{table.Schema}{end}.{begin}{table.Name}{end}";
    }
}
