using System.Collections.Generic;
using BriskChat.Web.Models.Common;

namespace BriskChat.Web.Helpers
{
    public class AlertHelper
    {
        private readonly List<AlertModel> alerts = new List<AlertModel>();

        public List<AlertModel> GetAlerts()
        {
            return alerts;
        }

        public void Success(string message = "Action completed", string moreInfo = null)
        {
            alerts.Add(new AlertModel
            {
                Message = message,
                MoreInfo = moreInfo,
                Type = AlertModel.Class.Success
            });
        }

        public void Info(string message, string moreInfo = null)
        {
            alerts.Add(new AlertModel
            {
                Message = message,
                MoreInfo = moreInfo,
                Type = AlertModel.Class.Info
            });
        }

        public void Warning(string message = "Your data is invalid", string moreInfo = null)
        {
            alerts.Add(new AlertModel
            {
                Message = message,
                MoreInfo = moreInfo,
                Type = AlertModel.Class.Warning
            });
        }

        public void Danger(string message, string moreInfo = null)
        {
            alerts.Add(new AlertModel
            {
                Message = message,
                MoreInfo = moreInfo,
                Type = AlertModel.Class.Error
            });
        }
    }
}