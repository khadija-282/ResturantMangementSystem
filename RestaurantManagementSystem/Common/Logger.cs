using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using System.Net;

namespace RestaurantManagementSystem
{
    public enum LogType
    {
        FATAL,
        ERROR,
        WARN,
        INFO
    }
    public enum ActivityType
    {
        VIEW,
        SEARCH,
        ADD,
        UPDATE,
        DELETE
    }
    public static class Logger
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Environment.MachineName);

        public static void LogActivity(ActivityType activityType, string searchCriteria, string message, LogType Type)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                MDC.Set("userName", System.Web.HttpContext.Current.User.Identity.Name);
            MDC.Set("activityType", activityType.ToString());
            MDC.Set("searchCriteria", searchCriteria);
            string hostName = Dns.GetHostName();
            string ipaddress = Dns.GetHostByName(hostName).AddressList[0].ToString();

            MDC.Set("ipAddress", ipaddress);

            switch (Type)
            {
                case LogType.ERROR:
                    logger.Error(message);
                    break;
                case LogType.FATAL:
                    logger.Fatal(message);
                    break;
                case LogType.WARN:
                    logger.Warn(message);
                    break;
                default:
                    logger.Info(message);
                    break;
            }
        }
    }
}