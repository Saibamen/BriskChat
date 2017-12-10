using Quartz;

namespace BriskChat.BusinessLogic.Quartz.Jobs.EmailUsers
{
    public class EmailUsersJobConfig
    {
        public IJobDetail GetJobDetail()
        {
            var userEmailsJob = JobBuilder.Create<EmailUsersJob>()
                .WithIdentity("SendUserEmails")
                .Build();
            return userEmailsJob;
        }

        public ITrigger GetJobTrigger()
        {
            var userEmailsTrigger = TriggerBuilder.Create()
                .WithIdentity("UserEmailsCron")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(15).RepeatForever())
                .Build();

            return userEmailsTrigger;
        }
    }
}