namespace BriskChat.BusinessLogic.Models
{
    public class EmailMessageModel : BaseModel
    {
        public string From { get; set; }

        public string Recipient { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public int FailureCount { get; set; }

        public string FailError { get; set; }

        public string FailErrorMessage { get; set; }

        public override bool IsValid()
        {
            if (string.IsNullOrEmpty(From) || string.IsNullOrEmpty(Recipient) || string.IsNullOrEmpty(Message) || string.IsNullOrEmpty(Subject))
            {
                return false;
            }

            return !string.IsNullOrEmpty(From.Trim()) && !string.IsNullOrEmpty(Recipient.Trim()) && !string.IsNullOrEmpty(Message.Trim()) && !string.IsNullOrEmpty(Subject.Trim());
        }
    }
}