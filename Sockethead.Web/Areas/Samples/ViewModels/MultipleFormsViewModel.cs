namespace Sockethead.Web.Areas.Samples.ViewModels
{
    public class UserProfile1 : UserProfile
    {
    }

    public class UserProfile2 : UserProfile
    {
    }

    public class UserProfile3 : UserProfile
    {
    }
    
    public class MultipleFormsViewModel
    {
        public UserProfile1 UserProfile1 { get; set; }
        public UserProfile2 UserProfile2 { get; set; }
        public UserProfile3 UserProfile3 { get; set; }
    }
}