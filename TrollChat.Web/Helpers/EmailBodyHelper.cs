using BriskChat.Web.Models.Common;

namespace BriskChat.Web.Helpers
{
    public class EmailBodyHelper : EmailBodyModel
    {
        public EmailBodyModel GetRegisterEmailBodyModel(string callbackUrl)
        {
            var body = new EmailBodyModel
            {
                TopicFirst = "We are ready to activate your account.",
                TopicSecond = "Only we have to check if the email is yours.",
                ButtonValue = callbackUrl,
                ButtonText = "Confirm email",
                AdditionalNotesFirst = "If you did not create a BriskChat account,",
                AdditionalNotesSecond = "remove this email and everything will return to normal."
            };

            return body;
        }

        public EmailBodyModel GetResetPasswordBodyModel(string callbackUrl)
        {
            var body = new EmailBodyModel
            {
                TopicFirst = "Password reset action has been initiated.",
                TopicSecond = "Please click on the button to confirm this action:",
                ButtonValue = callbackUrl,
                ButtonText = "Reset password",
                AdditionalNotesFirst = "Disregard this email if you didn't want to reset the password,",
                AdditionalNotesSecond = "this is not the email you are looking for."
            };

            return body;
        }
    }
}