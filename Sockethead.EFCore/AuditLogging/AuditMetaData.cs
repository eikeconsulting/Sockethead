namespace Sockethead.EFCore.AuditLogging
{
    public interface IAuditMetaData
    {
        string UserEmail { get; set; }
        string UserName { get; set; }
    }

    public class AuditMetaData : IAuditMetaData
    {
        public string UserEmail { get; set; }
        public string UserName { get; set; }
    }
}
