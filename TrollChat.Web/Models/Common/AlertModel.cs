namespace BriskChat.Web.Models.Common
{
    public class AlertModel
    {
        public enum Class
        {
            Success,
            Info,
            Warning,
            Error
        }

        public Class Type { get; set; }

        public string Message { get; set; }

        public string MoreInfo { get; set; }
    }
}