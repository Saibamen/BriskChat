namespace TrollChat.Web.Models.Common
{
    public class AlertModel
    {
        public enum Class
        {
            Success,
            Info,
            Warning,
            Danger
        }

        public Class Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}