using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// ���ݿ�
/// </summary>
public class Database : MonoBehaviour
{
    /// <summary>
    /// ���ݿ��ļ�CardMonster.db
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
            //��׿
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