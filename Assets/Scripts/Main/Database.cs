using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// 数据库
/// </summary>
public class Database : MonoBehaviour
{
    /// <summary>
    /// 数据库文件CardMonster.db
    /// </summary>
    public static DbAccess cardMonster;

    void Start()
    {
        if (cardMonster == null)
        {
            string dbName = Path.Combine("Database", "CardMonster.db");


            //#if UNITY_EDITOR
            //            //editor
            //            cardMonster = new DbAccess("data source=" + dbPath);
            //#endif

#if UNITY_STANDALONE_WIN
            //win
            string dbPath = Path.Combine(Application.streamingAssetsPath, dbName);
            cardMonster = new DbAccess("data source=" + dbPath);
#endif

#if UNITY_ANDROID
            //安卓
            string dbPath = Path.Combine(Application.persistentDataPath, dbName);
            if (!File.Exists(dbPath))
            {
                Tool.SAToPD(dbName);
            }
            cardMonster = new DbAccess("URI=file:" + dbPath);
#endif
        }
    }
}