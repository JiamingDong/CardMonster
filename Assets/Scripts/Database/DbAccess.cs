using UnityEngine;
using System.Data;
using System;
using System.Collections;
using Mono.Data.Sqlite;
using System.Collections.Generic;

/// <summary>
/// ���ݹ�����
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
    /// �����ݿ�
    /// </summary>
    /// <param name="connectionString">�����ַ���</param>
    public void OpenDB(string connectionString)
    {
        try
        {
            dbConnection = new SqliteConnection(connectionString);
            dbConnection.Open();
            //Debug.Log("DbAccess.OpenDB�������ݿ�" + connectionString);
        }
        catch (Exception e)
        {
            Debug.Log("DbAccess.OpenDB���쳣" + e.ToString());
        }
    }

    /// <summary>
    /// �ر����ݿ�
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
        //Debug.Log("DbAccess.CloseSqlConnection���ر����ݿ�");
    }

    /// <summary>
    /// ִ��sql���
    /// </summary>
    /// <returns>���</returns>
    /// <param name="sql">sql���</param>
    public SqliteDataReader ExecuteSQL(string sql)
    {
        //Debug.Log("DbAccess.ExecuteQuery��ִ��sql=" + sql);
        dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = sql;
        reader = dbCommand.ExecuteReader();
        return reader;
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="tableName">����</param>
    /// <param name="record">��Ҫ���������</param>
    public void Insert(string tableName, Dictionary<string, string> record)
    {
        if (record.Count == 0)
        {
            //Debug.Log("DbAccess.Insert�������������Ϊ��");
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
    /// ɾ������
    /// </summary>
    /// <param name="tableName">����</param>
    /// <param name="fliter">ɸѡ������������and��ͷ</param>
    public void Delete(string tableName, string fliter)
    {
        string query = "DELETE FROM " + tableName + " WHERE 1=1 " + fliter;
        ExecuteSQL(query);
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="tableName">����</param>
    /// <param name="record">�����õ�����</param>
    /// <param name="fliter">��ѯ������������and��ͷ</param>
    public void Update(string tableName, Dictionary<string, string> record, string fliter)
    {
        if (record.Count == 0)
        {
            //Debug.Log("DbAccess.Insert�������µ�����Ϊ��");
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
    /// ��ѯ
    /// </summary>
    /// <param name="tableName">����</param>
    /// <param name="fliter">��ѯ������������and��ͷ</param>
    /// <returns>���</returns>
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