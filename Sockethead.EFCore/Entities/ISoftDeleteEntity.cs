namespace Sockethead.EFCore.Entities
{
    public interface ISoftDeleteEntity
    {
        public bool IsDeleted { get; set; }
    }
}
