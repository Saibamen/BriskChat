using TrollChat.Web.Models.Common;

namespace TrollChat.Web.Helpers
{
    public class EmailBodyHelper : EmailBodyModel
    {
        public EmailBodyModel GetRegisterEmailBodyModel(string callbackUrl)
        {
            var body = new EmailBodyModel()
            {
                TopicFirst = "We are ready to activate your account.",
                TopicSecend = "Only we have to check if the email is yours.",
                ButtonValue = callbackUrl,
                Buttontext = "Confirm Email",
                AditionalNotesFirst = "If you did not create a TrollChat account,",
                AditionalNotesSecend = "remove this email and everything will return to normal."
            };
            return body;
        }

        public EmailBodyModel GetResetPasswordBodyModel(string callbackUrl)
        {
            var body = new EmailBodyModel()
            {
                TopicFirst = "Password reset action has been initiated,",
                TopicSecend = "Please click on the button to confirm this action",
                ButtonValue = callbackUrl,
                Buttontext = "Reset password",
                AditionalNotesFirst = "Disregard this email if you didn't want to reset the password,",
                AditionalNotesSecend = "this is not the email you are looking for"
            };
            return body;
        }
    }
}