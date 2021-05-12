namespace Sockethead.Web.Areas.Samples.ViewModels
{
    public class SocketheadRazorFeature
    {
        public string Name { get; set; }
        public string Model { get; set; } = "Movies";
        public string Url { get; set; }
        public string Description { get; set; }

        public string BuildUrl() => Url ?? $"Sample?name={System.Net.WebUtility.UrlEncode(Name)}";
    }
}
