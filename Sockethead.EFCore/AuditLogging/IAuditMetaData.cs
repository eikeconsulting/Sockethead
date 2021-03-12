namespace Sockethead.EFCore.AuditLogging
{
    public interface IAuditMetaData
    {
        string UserEmail { get; }
        string UserName { get; }
    }
}
