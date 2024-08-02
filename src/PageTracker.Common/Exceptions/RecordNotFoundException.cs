namespace PageTracker.Common.Exceptions;

/// <summary>
/// Throw when a specified record is not found, and that recrod is required for operation. <br/>
/// e.g. When deleting or updated a record, if that record doesn't exist
/// </summary>
[Serializable]
public class RecordNotFoundException : Exception
{
    public RecordNotFoundException() : base("Record was not found.")
    {
    }

    public RecordNotFoundException(Type recordType, object id)
        : base($"{recordType.Name} record with identifier {id} was not found.")
    {
    }

    public RecordNotFoundException(Type recordType, object id, Exception? innerException)
        : base($"{recordType.Name} record with identifier {id} was not found.", innerException)
    {
    }
}
