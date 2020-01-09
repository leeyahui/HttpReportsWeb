using Dapper;
using Dapper.Contrib.Extensions;
using HttpReports.Web.DataContext;
using HttpReports.Web.Implements;
using HttpReports.Web.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HttpReports.Web.DataAccessors
{
    public class DataAccessorOracle : IDataAccessor
    {
        private OracleConnection conn;
        private string connectionString;

        public DataAccessorOracle(DBFactory factory)
        {
            conn = factory.GeOracleConnection();
            connectionString = conn.ConnectionString;
        }

        public string BuildSqlWhere(GetIndexDataRequest request)
        {
            string where = " where 1=1 ";

            request.Start = request.Start.IsEmpty() ? $"to_date('{DateTime.Now.ToString("yyyy-MM-dd")}','yyyy-MM-dd')" : request.Start;
            if (!request.Start.Contains("to_date"))
            {
                request.Start = $"to_date('{request.Start}','yyyy-MM-dd')";
            }
            request.End = request.End.IsEmpty() ? $"to_date('{DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")}','yyyy-MM-dd')" : request.End;
            if (!request.End.Contains("to_date"))
            {
                request.End = $"to_date('{request.End}','yyyy-MM-dd')";
            }
            if (!request.Node.IsEmpty())
            {
                string nodes = string.Join(",", request.Node.Split(',').ToList().Select(x => "'" + x + "'"));

                where = where + $" AND Node IN ({nodes})";
            }

            if (!request.Start.IsEmpty())
            {
                where = where + $" AND CreateTime >= {request.Start} ";
            }

            if (!request.End.IsEmpty())
            {
                where = where + $" AND CreateTime < {request.End} ";
            }

            return where;

        }

        public List<EchartPineDataModel> GetStatusCode(GetIndexDataRequest request)
        {
            string where = BuildSqlWhere(request);

            string sql = $@"

                   Select '200' Name,COUNT(1) Value From  RequestInfo {where} AND StatusCode = 200
                    Union
                    Select '301' Name,COUNT(1) Value From  RequestInfo {where} AND StatusCode = 301
                    Union
                    Select '302' Name,COUNT(1) Value From  RequestInfo {where} AND StatusCode = 302
                    Union
                    Select '303' Name,COUNT(1) Value From  RequestInfo {where} AND StatusCode = 303
                    Union
                    Select '400' Name,COUNT(1) Value From  RequestInfo {where} AND StatusCode = 400
                    Union
                    Select '401' Name,COUNT(1) Value From  RequestInfo {where} AND StatusCode = 401
                    Union
                    Select '403' Name,COUNT(1) Value From  RequestInfo {where} AND StatusCode = 403
                    Union
                    Select '404' Name,COUNT(1) Value From  RequestInfo {where} AND StatusCode = 404
                    Union
                    Select '500' Name,COUNT(1) Value From  RequestInfo {where} AND StatusCode = 500
                    Union
                    Select '502' Name,COUNT(1) Value From  RequestInfo {where} AND StatusCode = 502
                    Union
                    Select '503' Name,COUNT(1) Value From  RequestInfo {where} AND StatusCode = 503

               ";

            return conn.Query<EchartPineDataModel>(sql).ToList();
        }


        public List<EchartPineDataModel> GetResponseTimePie(GetIndexDataRequest request)
        {
            string where = BuildSqlWhere(request);

            string sql = $@"  

                    Select Name,Value from ( 

                      Select 1 ID, '1-100' Name, Count(1) Value   From     RequestInfo {where} AND Milliseconds > 0 AND Milliseconds <= 100
                      union
                      Select 2 ID, '100-500' Name, Count(1) Value From   RequestInfo {where} AND Milliseconds > 100 AND Milliseconds <= 500
                      union
                      Select 3 ID, '500-1000' Name, Count(1) Value From  RequestInfo {where} AND Milliseconds > 500 AND Milliseconds <= 1000
                      union
                      Select 4 ID,'1000-3000' Name, Count(1) Value From  RequestInfo {where} AND Milliseconds > 1000 AND Milliseconds <= 3000
                      union
                      Select 5 Id,'3000-5000' Name, Count(1) Value From  RequestInfo {where} AND Milliseconds > 3000 AND Milliseconds <= 5000
                      union
                      Select 6 Id,'5000以上' Name, Count(1) Value From   RequestInfo {where} AND Milliseconds > 5000 

                  ) T Order By ID";

            return conn.Query<EchartPineDataModel>(sql).ToList();
        }

        public List<EchartPineDataModel> GetDayRequestTimes(GetIndexDataRequest request)
        {
            string where = " where 1=1 ";

            request.Day = request.Day.IsEmpty() ? DateTime.Now.ToString("yyyy-MM-dd") : request.Day;

            string End = request.Day.ToDateTime().AddDays(1).ToString("yyyy-MM-dd");

            if (!request.Node.IsEmpty())
            {
                string nodes = string.Join(",", request.Node.Split(',').ToList().Select(x => "'" + x + "'"));

                where = where + $" AND Node IN ({nodes})";
            }

            where = where + $" AND CreateTime >= to_date('{request.Day}','yyyy-MM-dd') AND CreateTime < to_date('{End}','yyyy-MM-dd') ";


            string sql = $" Select to_char(CreateTime,'HH24') Name ,COUNT(1) Value From RequestInfo {where} Group by to_char(CreateTime,'HH24') ";

            return conn.Query<EchartPineDataModel>(sql).ToList();
        }


        public List<EchartPineDataModel> GetLatelyDayData(GetIndexDataRequest request)
        {
            string where = " where 1=1 ";

            if (!request.Node.IsEmpty())
            {
                string nodes = string.Join(",", request.Node.Split(',').ToList().Select(x => "'" + x + "'"));

                where = where + $" AND Node IN ({nodes})";
            }

            where = where + $" AND CreateTime >= to_date('{request.Start}','yyyy-MM-dd') AND CreateTime < to_date('{request.End}','yyyy-MM-dd') ";


            string sql = $" Select to_char(CreateTime,'yyyy-MM') Name ,COUNT(1) Value From RequestInfo {where} Group by to_char(CreateTime,'yyyy-MM') ";

            return conn.Query<EchartPineDataModel>(sql).ToList();

        }


        public List<EchartPineDataModel> GetMonthDataByYear(GetIndexDataRequest request)
        {
            string where = " where 1=1 ";

            if (!request.Node.IsEmpty())
            {
                string nodes = string.Join(",", request.Node.Split(',').ToList().Select(x => "'" + x + "'"));

                where = where + $" AND Node IN ({nodes})";
            }

            where = where + $" AND CreateTime >= to_date('{request.Start}','yyyy-MM-dd') AND CreateTime < to_date('{request.End}','yyyy-MM-dd')  ";

            string sql = $" Select to_char(CreateTime,'yyyy-MM') Name,Count(1) Value From RequestInfo {where} Group by to_char(CreateTime,'yyyy-MM')";

            return conn.Query<EchartPineDataModel>(sql).ToList();

        }



        public List<EchartPineDataModel> GetDayResponseTime(GetIndexDataRequest request)
        {
            string where = " where 1=1 ";

            request.Day = request.Day.IsEmpty() ? DateTime.Now.ToString("yyyy-MM-dd") : request.Day;

            string End = request.Day.ToDateTime().AddDays(1).ToString("yyyy-MM-dd");

            if (!request.Node.IsEmpty())
            {
                string nodes = string.Join(",", request.Node.Split(',').ToList().Select(x => "'" + x + "'"));

                where = where + $" AND Node IN ({nodes})";
            }

            where = where + $" AND CreateTime >= to_date('{request.Day}','yyyy-MM-dd') AND CreateTime < to_date('{End}','yyyy-MM-dd')  ";

            string sql = $" Select to_char(CreateTime, 'HH24') Name ,ROUND(AVG(Milliseconds)) Value From RequestInfo {where} Group by to_char(CreateTime, 'HH24')";
            return conn.Query<EchartPineDataModel>(sql).ToList();
        }

        public List<string> GetNodes()
        {
            return conn.Query<string>(" Select Distinct Node  FROM RequestInfo ").ToList();
        }

        public GetIndexDataResponse GetIndexData(GetIndexDataRequest request)
        {
            string where = BuildSqlWhere(request);

            // string sql = $@" 
            //    Select  AVG(Milliseconds) ART From RequestInfo {where} ;
            //    Select  COUNT(1) Total From RequestInfo {where} ;
            //    Select  COUNT(1) Code404 From RequestInfo {where} AND StatusCode = 404 ;
            //    Select  COUNT(1) Code500 From RequestInfo {where} AND StatusCode = 500 ;
            //   Select Count(1) From ( Select Distinct Url From RequestInfo ) A;
            //";

            GetIndexDataResponse response = new GetIndexDataResponse();
            using (var con = new OracleConnection(connectionString))
            {
                response.ART = con.QueryFirst<string>($"Select round(AVG(Milliseconds)) ART From RequestInfo {where}");
                response.Total = con.QueryFirst<string>($"Select  COUNT(1) Total From RequestInfo {where}");
                response.Code404 = con.QueryFirst<string>($"Select  COUNT(1) Code404 From RequestInfo {where} AND StatusCode = 404 ");
                response.Code500 = con.QueryFirst<string>($"Select  COUNT(1) Code500 From RequestInfo {where} AND StatusCode = 500 ");
                response.APICount = con.QueryFirst<int>($" Select Count(1) From ( Select Distinct Url From RequestInfo ) A");
                response.ErrorPercent = response.Total.ToInt() == 0 ? "0.00%" : (Convert.ToDouble(response.Code500) / Convert.ToDouble(response.Total)).ToString("0.00%");
            }

            return response;
        }

        public List<GetTopResponse> GetTopRequest(GetTopRequest request)
        {
            string where = BuildTopWhere(request);

            string sql = $" Select  Url,COUNT(1) as Total From RequestInfo {where}  Group By Url order by Total {(request.IsDesc ? "Desc" : "Asc")}";

            return conn.Query<GetTopResponse>(sql).ToList();
        }

        public string BuildTopWhere(GetTopRequest request)
        {
            string where = " where 1=1 ";

            if (!request.Node.IsEmpty())
            {
                string nodes = string.Join(",", request.Node.Split(',').ToList().Select(x => "'" + x + "'"));

                where = where + $" AND Node IN ({nodes})";
            }

            if (!request.Start.IsEmpty())
            {
                where = where + $" AND CreateTime >= to_date('{request.Start}','yyyy-MM-dd') ";
            }

            if (!request.End.IsEmpty())
            {
                where = where + $" AND CreateTime < to_date('{request.End}','yyyy-MM-dd') ";
            }

            where = where + $" AND ROWNUM <= {request.TOP}";

            return where;
        }

        public List<GetTopResponse> GetCode500Response(GetTopRequest request)
        {
            string where = BuildTopWhere(request);

            string sql = $" Select Url,COUNT(1) as Total From RequestInfo {where} AND StatusCode = 500 Group By Url order by Total {(request.IsDesc ? "Desc" : "Asc")}";

            return conn.Query<GetTopResponse>(sql).ToList();
        }


        /// <summary>
        /// 获取接口平均处理时间
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<EchartPineDataModel> GetTOPART(GetTopRequest request)
        {
            string where = BuildTopWhere(request);

            string sql = $" Select Url Name ,round(Avg(Milliseconds)) Value From RequestInfo {where} Group By Url order by Value {(request.IsDesc ? "Desc" : "Asc")}";

            return conn.Query<EchartPineDataModel>(sql).ToList();

        }

        public List<RequestInfo> GetRequestList(GetRequestListRequest request, out int totalCount)
        {
            string where = " where 1=1 ";

            if (!request.Node.IsEmpty())
            {
                string nodes = string.Join(",", request.Node.Split(',').ToList().Select(x => "'" + x + "'"));

                where = where + $" AND Node IN ({nodes})";
            }

            if (!request.Start.IsEmpty())
            {
                where = where + $" AND CreateTime >=to_date('{request.Start}','yyyy-MM-dd')";
            }

            if (!request.End.IsEmpty())
            {
                where = where + $" AND CreateTime < to_date('{request.End}','yyyy-MM-dd') ";
            }

            if (!request.IP.IsEmpty())
            {
                where = where + $" AND IP = '{request.IP}' ";
            }

            if (!request.Url.IsEmpty())
            {
                where = where + $" AND  Url like '%{request.Url}%' ";
            }

            string sql = $"Select * From RequestInfo {where} AND rownum>= {(request.pageNumber - 1) * request.pageSize}and rownum <= {request.pageSize} ";

            totalCount = conn.QueryFirstOrDefault<int>(" Select count(1) From RequestInfo " + where);

            return conn.Query<RequestInfo>(sql).ToList();
        }

        public void AddJob(Models.Job job)
        {
            conn.Insert<Models.Job>(job);
        }

        public List<Models.Job> GetJobs()
        {
            using (var con = new OracleConnection(connectionString))
            {
                return con.GetAll<Models.Job>().ToList();
            }
        }

        public CheckModel CheckRt(Models.Job job, int minute)
        {
            using (var con = new OracleConnection(connectionString))
            {
                string where = " where 1=1 ";

                where = BuildWhereByTime(where, minute);
                where = BuildWhereByNode(where, job.Servers);
                string Time = DateTime.Now.AddMinutes(-minute).ToString("yyyy-MM-dd HH:mm:ss") + "-" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                int now = con.QueryFirstOrDefault<int>($" Select Count(1) From RequestInfo {where} AND Milliseconds >= {job.RtTime} ");

                int current = con.QueryFirstOrDefault<int>($"Select Count(1) From RequestInfo {where} ");

                if (now == 0 || current == 0)
                {
                    return new CheckModel();
                }

                double percent = Math.Round(Convert.ToDouble(now) / Convert.ToDouble(current), 6);

                if (percent < job.RtStatus * 0.01)
                {
                    return new CheckModel();
                }
                else
                {
                    return new CheckModel()
                    {
                        Ok = false,
                        Value = Math.Round(percent, 4).ToString(),
                        Time = Time
                    };
                }

            }
        }

        public CheckModel CheckHttp(Models.Job job, int minute)
        {
            using (var con = new OracleConnection(connectionString))
            {

                string where = " where 1=1 ";

                where = BuildWhereByTime(where, minute);
                where = BuildWhereByNode(where, job.Servers);
                string Time = DateTime.Now.AddMinutes(-minute).ToString("yyyy-MM-dd HH:mm:ss") + "-" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                var now = con.QueryFirstOrDefault<int>($" Select Count(1) From RequestInfo {where} {BuildWhereByHttpCode(job.HttpCodes)} ");

                var current = con.QueryFirstOrDefault<int>($" Select Count(1) From RequestInfo {where} ");

                if (now == 0 || current == 0)
                {
                    return new CheckModel();
                }

                double percent = Math.Round(Convert.ToDouble(now) / Convert.ToDouble(current), 6);

                if (percent < job.HttpRate * 0.01)
                {
                    return new CheckModel();
                }
                else
                {
                    return new CheckModel()
                    {
                        Ok = false,
                        Value = Math.Round(percent, 4).ToString(),
                        Time = Time
                    };
                }


            }
        }

        public CheckModel CheckIP(Models.Job job, int minute)
        {
            using (var con = new OracleConnection(connectionString))
            {
                string where = " where 1=1 ";

                where = BuildWhereByTime(where, minute);
                where = BuildWhereByNode(where, job.Servers);
                string Time = DateTime.Now.AddMinutes(-minute).ToString("yyyy-MM-dd HH:mm:ss") + "-" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                var now = con.QueryFirstOrDefault<int>($"Select  COUNT(1) From RequestInfo {where} {BuildWhereByIP(job.IPWhiteList)} and rownum<=1 Group By IP Order BY COUNT(1) Desc");

                var current = con.QueryFirstOrDefault<int>($" Select Count(1) From RequestInfo {where} {BuildWhereByIP(job.IPWhiteList)} ");

                if (now == 0 || current == 0)
                {
                    return new CheckModel();
                }

                double percent = Math.Round(Convert.ToDouble(now) / Convert.ToDouble(current), 6);

                if (percent < job.IPRate * 0.01)
                {
                    return new CheckModel();
                }
                else
                {
                    return new CheckModel()
                    {
                        Ok = false,
                        Value = Math.Round(percent, 4).ToString(),
                        Time = Time
                    };
                }

            }
        }

        public CheckModel CheckRequestCount(Models.Job job, int minute)
        {
            using (var con = new OracleConnection(connectionString))
            {
                string where = " where 1=1 ";

                where = BuildWhereByTime(where, minute);
                where = BuildWhereByNode(where, job.Servers);
                string Time = DateTime.Now.AddMinutes(-minute).ToString("yyyy-MM-dd HH:mm:ss") + "-" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                var now = con.QueryFirstOrDefault<int>($" Select Count(1) From RequestInfo {where} ");

                if (now < job.RequestCount || now == 0)
                {
                    return new CheckModel();
                }
                else
                {
                    return new CheckModel()
                    {
                        Ok = false,
                        Value = now.ToString(),
                        Time = Time
                    };

                }
            }
        }

        private string BuildWhereByTime(string where, int minute)
        {
            var start = DateTime.Now.AddMinutes(-minute).ToString("yyyy-MM-dd HH:mm:ss");
            var end = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            where = where + $" AND CreateTime >= to_date('{start}','yyyy-MM-dd HH24:mm:ss') AND CreateTime < to_date('{end}','yyyy-MM-dd HH24:mm:ss') ";

            return where;
        }

        private string BuildWhereByNode(string where, string node)
        {
            if (node.IsEmpty())
            {
                return where;
            }

            string nodes = string.Join(",", node.Split(',').ToList().Select(x => "'" + x + "'"));

            where = where + $" AND Node IN ({nodes})";

            return where;

        }


        private string BuildWhereByHttpCode(string codes)
        {
            if (codes.IsEmpty())
            {
                return string.Empty;
            }

            string code = string.Join(",", codes.Split(',').ToList().Select(x => x));

            string where = $" AND StatusCode In ({code})  ";

            return where;
        }

        private string BuildWhereByIP(string iplist)
        {
            if (iplist.IsEmpty())
            {
                return string.Empty;
            }

            string ip = string.Join(",", iplist.Split(',').ToList().Select(x => "'" + x + "'"));

            string where = $" AND IP Not IN ({ip})";

            return where;
        }

        public Models.Job GetJob(int Id)
        {
            return conn.Get<Models.Job>(Id);
        }

        public void UpdateJob(Models.Job job)
        {
            conn.Update<Models.Job>(job);
        }

        public void DeleteJob(Models.Job job)
        {
            conn.Delete<Models.Job>(job);
        }
    }
}
