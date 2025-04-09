using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace ObjectBulkCopy.Internals;



/// <summary>
/// Provides an <see cref="IDataReader"/> implementation for bulk copy operations.
/// </summary>
/// <typeparam name="T"></typeparam>
internal sealed class SqlBulkCopyDataReader<T> : IDataReader
{
    #region Fields
    private readonly IEnumerator<T> _dataEnumerator;
    #endregion


    #region Constructors
    /// <summary>
    /// Creates instance.
    /// </summary>
#pragma warning disable IDE0290
    public SqlBulkCopyDataReader(IEnumerator<T> enumerator)
        => this._dataEnumerator = enumerator;
#pragma warning restore IDE0290


    /// <summary>
    /// Creates instance.
    /// </summary>
    public SqlBulkCopyDataReader(IEnumerable<T> data)
        : this(data.GetEnumerator())
    { }
    #endregion


    #region IDataReader
    /// <inheritdoc/>
    public int Depth
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public bool IsClosed
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public int RecordsAffected
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public void Close()
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public DataTable? GetSchemaTable()
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public bool NextResult()
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public bool Read()
        => this._dataEnumerator.MoveNext();
    #endregion


    #region IDataRecord
    /// <inheritdoc/>
    public object this[int i]
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public object this[string name]
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public int FieldCount
        => TableMapping.Get<T>().Columns.Length;


    /// <inheritdoc/>
    public bool GetBoolean(int i)
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public byte GetByte(int i)
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length)
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public char GetChar(int i)
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length)
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public IDataReader GetData(int i)
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public string GetDataTypeName(int i)
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public DateTime GetDateTime(int i)
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public decimal GetDecimal(int i)
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public double GetDouble(int i)
        => throw new NotImplementedException();


    /// <inheritdoc/>
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)]
    public Type GetFieldType(int i)
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public float GetFloat(int i)
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public Guid GetGuid(int i)
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public short GetInt16(int i)
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public int GetInt32(int i)
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public long GetInt64(int i)
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public string GetName(int i)
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public int GetOrdinal(string name)
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public string GetString(int i)
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public object GetValue(int i)
    {
        // todo: Re-implement using source generator for performance

        var mapping = TableMapping.Get<T>().ColumnByOrder[i];
        var record = this._dataEnumerator.Current;
        var value = mapping.PropertyInfo.GetValue(record);
        return value!;
    }


    /// <inheritdoc/>
    public int GetValues(object[] values)
        => throw new NotImplementedException();


    /// <inheritdoc/>
    public bool IsDBNull(int i)
        => throw new NotImplementedException();
    #endregion


    #region IDisposable
    /// <inheritdoc/>
    public void Dispose()
        => this._dataEnumerator.Dispose();
    #endregion
}
