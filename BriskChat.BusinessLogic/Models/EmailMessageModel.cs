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
            if (string.IsNullOrWhiteSpace(From) || string.IsNullOrWhiteSpace(Recipient) || string.IsNullOrWhiteSpace(Message) || string.IsNullOrWhiteSpace(Subject))
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(From.Trim()) && !string.IsNullOrWhiteSpace(Recipient.Trim()) && !string.IsNullOrWhiteSpace(Message.Trim()) && !string.IsNullOrWhiteSpace(Subject.Trim());
        }
    }
}