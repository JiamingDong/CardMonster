using UnityEngine;
using System.Data;
using System;
using System.Collections;
using Mono.Data.Sqlite;
using System.Collections.Generic;

/// <summary>
/// 数据工具类
/// </summary>
public class DbAccess
{
    private SqliteConnection dbConnection;
    private SqliteCommand dbCommand;
    private SqliteDataReader reader;

    public DbAccess(string connectionString)
    {
        OpenDB(connectionString);
    }

    /// <summary>
    /// 打开数据库
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    public void OpenDB(string connectionString)
    {
        try
        {
            dbConnection = new SqliteConnection(connectionString);
            dbConnection.Open();
            //Debug.Log("DbAccess.OpenDB：打开数据库" + connectionString);
        }
        catch (Exception e)
        {
            Debug.Log("DbAccess.OpenDB：异常" + e.ToString());
        }
    }

    /// <summary>
    /// 关闭数据库
    /// </summary>
    public void CloseSqlConnection()
    {
        if (dbCommand != null)
        {
            dbCommand.Dispose();
        }
        dbCommand = null;
        if (reader != null)
        {
            reader.Dispose();
        }
        reader = null;
        if (dbConnection != null)
        {
            dbConnection.Close();
        }
        dbConnection = null;
        //Debug.Log("DbAccess.CloseSqlConnection：关闭数据库");
    }

    /// <summary>
    /// 执行sql语句
    /// </summary>
    /// <returns>结果</returns>
    /// <param name="sql">sql语句</param>
    public SqliteDataReader ExecuteSQL(string sql)
    {
        //Debug.Log("DbAccess.ExecuteQuery：执行sql=" + sql);
        dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = sql;
        reader = dbCommand.ExecuteReader();
        return reader;
    }

    /// <summary>
    /// 插入数据
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="record">需要插入的数据</param>
    public void Insert(string tableName, Dictionary<string, string> record)
    {
        if (record.Count == 0)
        {
            //Debug.Log("DbAccess.Insert：将插入的数据为空");
            return;
        }
        string columnNames = "";
        string columnValues = "";
        foreach (KeyValuePair<string, string> column in record)
        {
            columnNames += column.Key + ",";
            columnValues += "'" + column.Value + "',";
        }
        string query = "INSERT INTO " + tableName + "(" + columnNames.Substring(0, columnNames.Length - 1) + ") VALUES(" + columnValues.Substring(0, columnValues.Length - 1) + ")";
        ExecuteSQL(query);
    }

    /// <summary>
    /// 删除数据
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="fliter">筛选条件，必须以and开头</param>
    public void Delete(string tableName, string fliter)
    {
        string query = "DELETE FROM " + tableName + " WHERE 1=1 " + fliter;
        ExecuteSQL(query);
    }

    /// <summary>
    /// 更新数据
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="record">更新用的数据</param>
    /// <param name="fliter">查询条件，必须以and开头</param>
    public void Update(string tableName, Dictionary<string, string> record, string fliter)
    {
        if (record.Count == 0)
        {
            //Debug.Log("DbAccess.Insert：将更新的数据为空");
            return;
        }
        string query = "UPDATE " + tableName + " SET ";
        foreach (KeyValuePair<string, string> column in record)
        {
            query += column.Key + "='" + column.Value + "',";
        }
        query = query.Substring(0, query.Length - 1);
        query += " WHERE 1=1 " + fliter;
        ExecuteSQL(query);
    }

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="fliter">查询条件，必须以and开头</param>
    /// <returns>结果</returns>
    public List<Dictionary<string, string>> Query(string tableName, string fliter)
    {
        List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
        string sql = "select * from " + tableName + " where 1=1 " + fliter;
        SqliteDataReader sqliteDataReader = ExecuteSQL(sql);
        List<string> allColumnName = new List<string>();

        if (sqliteDataReader.Read())
        {
            for (int i = 0; i < sqliteDataReader.FieldCount; i++)
            {
                allColumnName.Add(sqliteDataReader.GetName(i).Trim());
            }
            Dictionary<string, string> column = new Dictionary<string, string>();
            foreach (string columnName in allColumnName)
            {
                column.Add(columnName, sqliteDataReader[columnName].ToString());
            }
            result.Add(column);
        }

        while (sqliteDataReader.Read())
        {
            Dictionary<string, string> column = new Dictionary<string, string>();
            foreach (string columnName in allColumnName)
            {
                column.Add(columnName, sqliteDataReader[columnName].ToString());
            }
            result.Add(column);
        }

        sqliteDataReader.Close();

        return result;
    }
}