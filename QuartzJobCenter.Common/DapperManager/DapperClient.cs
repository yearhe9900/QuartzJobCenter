using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using static Dapper.SqlMapper;

namespace QuartzJobCenter.Common.DapperManager
{
    public class DapperClient
    {
        public ConnectionConfig CurrentConnectionConfig { get; set; }

        public DapperClient(ConnectionConfig config) { CurrentConnectionConfig = config; }

        IDbConnection _connection = null;
        public IDbConnection Connection
        {
            get
            {
                _connection = CurrentConnectionConfig.DbType switch
                {
                    //case Enums.EnumDbStoreType.MySql:
                    //    _connection = new MySql.Data.MySqlClient.MySqlConnection(CurrentConnectionConfig.ConnectionString);
                    //    break;
                    //case DbStoreType.Sqlite:
                    //    _connection = new SQLiteConnection(CurrentConnectionConfig.ConnectionString);
                    //    break;
                    Enums.EnumDbStoreType.SqlServer => new System.Data.SqlClient.SqlConnection(CurrentConnectionConfig.ConnectionString),
                    //case Enums.EnumDbStoreType.Oracle:
                    //    _connection = new Oracle.ManagedDataAccess.Client.OracleConnection(CurrentConnectionConfig.ConnectionString);
                    //    break;
                    _ => throw new Exception("未指定数据库类型！"),
                };
                return _connection;
            }
        }

        /// <summary>
        /// 执行SQL返回集合
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="obj">参数model</param>
        /// <returns></returns>
        public virtual IEnumerable<T> Query<T>(string strSql, object param = null)
        {
            using IDbConnection conn = Connection;
            return conn.Query<T>(strSql, param);
        }

        /// <summary>
        /// 异步执行SQL返回集合
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> QueryAsync<T>(string strSql, object param = null)
        {
            using IDbConnection conn = Connection;
            return await conn.QueryAsync<T>(strSql, param);
        }

        /// <summary>
        /// 执行SQL返回一个对象
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="obj">参数model</param>
        /// <returns></returns>
        public virtual T QueryFirst<T>(string strSql, object param = null)
        {
            using IDbConnection conn = Connection;
            return conn.Query<T>(strSql, param).FirstOrDefault<T>();
        }

        /// <summary>
        /// 异步执行SQL返回一个对象
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns></returns>
        public virtual async Task<T> QueryFirstAsync<T>(string strSql, object param = null)
        {
            using IDbConnection conn = Connection;
            var res = await conn.QueryAsync<T>(strSql, param);
            return res.FirstOrDefault<T>();
        }

        /// <summary>
        /// 同步执行批量SQL返回集合
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <returns></returns>
        public virtual GridReader QueryMultiple(string strSql, object param = null)
        {
            using IDbConnection conn = Connection;
            return conn.QueryMultiple(strSql, param);
        }

        /// <summary>
        /// 异步执行批量SQL返回集合
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <returns></returns>
        public virtual async Task<GridReader> QueryMultipleAsync(string strSql, object param = null)
        {
            using IDbConnection conn = Connection;
            return await conn.QueryMultipleAsync(strSql, param);
        }

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="param">参数</param>
        /// <returns>返回影响条数</returns>
        public virtual int Execute(string strSql, object param = null)
        {
            using IDbConnection conn = Connection;
            try
            {
                return conn.Execute(strSql, param);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 异步执行SQL
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="param">参数</param>
        /// <returns>返回影响条数</returns>
        public virtual async Task<int> ExecuteAsync(string strSql, object param = null)
        {
            using IDbConnection conn = Connection;
            try
            {
                return await conn.ExecuteAsync(strSql, param);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 事务执行
        /// </summary>
        /// <param name="funcs"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExcuteWithTrans(params Func<Task<int>>[] funcs)
        {
            using IDbConnection conn = Connection;
            conn.Open();
            IDbTransaction transaction = conn.BeginTransaction();
            try
            {
                foreach (var fun in funcs)
                {
                    var funResult = await fun.Invoke() >= 0;
                    if (!funResult)
                    {
                        return false;
                    }
                }
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="strProcedure">过程名</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public virtual int ExecuteStoredProcedure(string strProcedure, object param = null)
        {
            using IDbConnection conn = Connection;
            try
            {
                return conn.Execute(strProcedure, param, null, null, CommandType.StoredProcedure) == 0 ? 0 : -1;
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
