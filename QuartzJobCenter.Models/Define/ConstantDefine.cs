using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzJobCenter.Models.Define
{
    public struct ConstantDefine
    {
        /// <summary>
        /// 请求url RequestUrl
        /// </summary>
        public const string REQUESTURL = "RequestUrl";
        /// <summary>
        /// 请求参数 RequestParameters
        /// </summary>
        public const string REQUESTPARAMETERS = "RequestParameters";
        /// <summary>
        /// Headers（可以包含：Authorization授权认证）
        /// </summary>
        public const string HEADERS = "Headers";
        /// <summary>
        /// 是否发送邮件
        /// </summary>
        public const string MAILMESSAGE = "MailMessage";
        /// <summary>
        /// 请求类型 RequestType
        /// </summary>
        public const string REQUESTTYPE = "RequestType";
        /// <summary>
        /// 日志 LogList
        /// </summary>
        public const string LOGLIST = "LogList";
        /// <summary>
        /// 异常 Exception
        /// </summary>
        public const string EXCEPTION = "Exception";

        public const string DbConnedtions = "DbConnedtions";

        public const string SqlServer = "SqlServer";

        public const string MySql = "MySql";

        public const string TaskCenterConnection = "TaskCenterConnection";
    }
}
