using Newtonsoft.Json;
using Quartz;
using QuartzJobCenter.Models.Define;
using QuartzJobCenter.Common.Helper;
using QuartzJobCenter.Models.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using static QuartzJobCenter.Models.Enums.EnumDefine;
using NLog;

namespace QuartzJobCenter.Web.Job
{
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class HttpJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var maxLogCount = 20;//最多保存日志数量
            var warnTime = 20;//接口请求超过多少秒记录警告日志         
            //获取相关参数
            var requestUrl = context.JobDetail.JobDataMap.GetString(ConstantDefine.REQUESTURL);
            requestUrl = requestUrl?.IndexOf("http") == 0 ? requestUrl : "http://" + requestUrl;
            var requestParameters = context.JobDetail.JobDataMap.GetString(ConstantDefine.REQUESTPARAMETERS);
            var headersString = context.JobDetail.JobDataMap.GetString(ConstantDefine.HEADERS);
            var mailMessage = (MailMessageEnum)int.Parse(context.JobDetail.JobDataMap.GetString(ConstantDefine.MAILMESSAGE) ?? "0");
            var headers = headersString != null ? JsonConvert.DeserializeObject<Dictionary<string, string>>(headersString?.Trim()) : null;
            var requestType = (RequestTypeEnum)int.Parse(context.JobDetail.JobDataMap.GetString(ConstantDefine.REQUESTTYPE));

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart(); //  开始监视代码运行时间
            IRestResponse response = default;

            var loginfo = new LogInfoModel
            {
                Url = requestUrl,
                BeginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                RequestType = requestType.ToString(),
                Parameters = requestParameters,
                JobName = $"{context.JobDetail.Key.Group}.{context.JobDetail.Key.Name}"
            };

            var logs = context.JobDetail.JobDataMap[ConstantDefine.LOGLIST] as List<string> ?? new List<string>();
            if (logs.Count >= maxLogCount)
                logs.RemoveRange(0, logs.Count - maxLogCount);

            try
            {
                var http = RestHttpHelper.Instance;
                switch (requestType)
                {
                    case RequestTypeEnum.Get:
                        response = await http.GetAsync(requestUrl, headers);
                        break;
                    case RequestTypeEnum.Post:
                        response = await http.PostAsync(requestUrl, requestParameters, headers);
                        break;
                        //case RequestTypeEnum.Put:
                        //    response = await http.PutAsync(requestUrl, requestParameters, headers);
                        //    break;
                        //case RequestTypeEnum.Delete:
                        //    response = await http.DeleteAsync(requestUrl, headers);
                        //    break;
                }
                var result = HttpUtility.HtmlEncode(response.Content);

                stopwatch.Stop(); //  停止监视            
                double seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数                                
                loginfo.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                loginfo.Seconds = seconds;
                loginfo.Result = result;

                if (!response.IsSuccessful)
                {
                    loginfo.ErrorMsg = response.ErrorMessage;
                    var loginfoJson = JsonConvert.SerializeObject(loginfo);
                    await ErrorAsync(loginfo.JobName, new Exception(result), loginfoJson, mailMessage);
                    context.JobDetail.JobDataMap[ConstantDefine.EXCEPTION] = loginfoJson;
                }
                else
                {
                    var loginfoJson = JsonConvert.SerializeObject(loginfo);
                    try
                    {
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            loginfo.ErrorMsg = response.StatusDescription;
                            await ErrorAsync(loginfo.JobName, new Exception(result), loginfoJson, mailMessage);
                            context.JobDetail.JobDataMap[ConstantDefine.EXCEPTION] = loginfoJson;
                        }
                        await InformationAsync(loginfo.JobName, loginfoJson, mailMessage);
                    }
                    catch (Exception)
                    {
                        await ErrorAsync(loginfo.JobName, new Exception(result), loginfoJson, mailMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop(); //  停止监视            
                double seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数
                loginfo.ErrorMsg = $"{ex.Message} {ex.StackTrace}";
                context.JobDetail.JobDataMap[ConstantDefine.EXCEPTION] = JsonConvert.SerializeObject(loginfo);
                loginfo.Seconds = seconds;
                await ErrorAsync(loginfo.JobName, ex, JsonConvert.SerializeObject(loginfo), mailMessage);
            }
            finally
            {
                logs.Add(JsonConvert.SerializeObject(loginfo));
                context.JobDetail.JobDataMap[ConstantDefine.LOGLIST] = logs;
                double seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数
                if (seconds >= warnTime)//如果请求超过20秒，记录警告日志    
                {
                    await WarningAsync(loginfo.JobName, "耗时过长 - " + JsonConvert.SerializeObject(loginfo), mailMessage);
                }
            }
        }

        public async Task WarningAsync(string title, string msg, MailMessageEnum mailMessage)
        {
            if (mailMessage == MailMessageEnum.All)
            {
                //await new SendMail(new SendMailModel()
                //{
                //    Title = $"任务调度-{title}【警告】消息",
                //    Content = msg
                //});
            }
        }

        public async Task InformationAsync(string title, string msg, MailMessageEnum mailMessage)
        {
            if (mailMessage == MailMessageEnum.All)
            {
                //await new SendMail(new SendMailModel()
                //{
                //    Title = $"任务调度-{title}消息",
                //    Content = msg
                //});
            }
        }

        public async Task ErrorAsync(string title, Exception ex, string msg, MailMessageEnum mailMessage)
        {
            if (mailMessage == MailMessageEnum.Err || mailMessage == MailMessageEnum.All)
            {
                //await new SendMail(new SendMailModel()
                //{
                //    Title = $"任务调度-{title}【异常】消息",
                //    Content = msg
                //});
            }
        }
    }
}
