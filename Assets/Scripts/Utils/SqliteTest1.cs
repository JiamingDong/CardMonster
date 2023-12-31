//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Mono.Data.Sqlite;
//using System.IO;

//public class SqliteTest1 : MonoBehaviour
//{
//    string dbPath;
//    string dbName;
//    DbAccess db;

//    // Use this for initialization
//    void Start()
//    {
//        dbName = "CardMonster.db";
//        dbPath = Path.Combine(LoadAssetBundle.streamingAssetsPath, dbName);
//        Debug.Log(dbPath);
//        db = new DbAccess("data source=" + dbPath);
//    }

//    void OnDestroy()
//    {
//        Debug.Log("<color=#00ff00>close database</color>");
//        //¹Ø±Õ¶ÔÏó
//        db.CloseSqlConnection();
//    }

//    void InsertData()
//    {
//        db.InsertInto("user", new string[] { "'zhangsan'", "11", "'man'", "'beijing'" });
//        db.InsertInto("user", new string[] { "'lisi'", "11", "'woman'", "'huoying'" });
//    }

//    void UpdateData()
//    {
//        db.UpdateInto("user", new string[] { "age", "sex" }, new string[] { "82", "'woman'" }, "name", "'zhangsan'");
//    }

//    void DeleteData()
//    {
//        db.Delete("user", new string[] { "age", "sex" }, new string[] { "82", "'woman'" });
//    }

//    void QueryData()
//    {
//        string query = "select * from user where name='zhangsan'";
//        SqliteDataReader reader = db.ExecuteSQL(query);
//        while (reader.Read())
//        {
//            Debug.Log("name=" + reader["name"]);
//            Debug.Log("age=" + reader["age"]);
//            Debug.Log("sex=" + reader["sex"]);
//            Debug.Log("adress=" + reader["adress"]);
//        }
//    }

//    void SelectData()
//    {
//        // "select (name,age,sex) from user where age > 20";
//        SqliteDataReader reader = db.SelectWhere("user", new string[] { "name", "age", "sex" }, new string[] { "age" }, new string[] { ">" }, new string[] { "20" },"");
//        while (reader.Read())
//        {
//            Debug.Log("name=" + reader["name"]);
//            Debug.Log("age=" + reader["age"]);
//            Debug.Log("sex=" + reader["sex"]);
//        }
//    }

//    void OnGUI()
//    {
//        if (GUI.Button(new Rect(10, 20, 100, 40), "insert data"))
//        {
//            InsertData();
//        }
//        else if (GUI.Button(new Rect(10, 80, 100, 40), "delete data"))
//        {
//            DeleteData();
//        }
//        else if (GUI.Button(new Rect(10, 120, 100, 40), "updata data"))
//        {
//            UpdateData();
//        }
//        else if (GUI.Button(new Rect(10, 180, 100, 40), "query data"))
//        {
//            QueryData();
//        }
//        else if (GUI.Button(new Rect(10, 250, 100, 40), "select data"))
//        {
//            SelectData();
//        }
//        else if (GUI.Button(new Rect(180, 20, 100, 40), "query all data"))
//        {
//            SqliteDataReader reader = db.ReadFullTable("user","");
//            int count = 0;
//            while (reader.Read())
//            {
//                count++;
//                //          Debug.Log ("name="+reader["name"]);
//                //          Debug.Log ("age="+reader["age"]);
//                //          Debug.Log ("sex="+reader["sex"]);
//                //          Debug.Log ("adress="+reader["adress"]);
//                Debug.Log("name=" + reader["name"] + ",age=" + reader["age"] + ",sex=" + reader["sex"] + ",adress=" + reader["adress"]);
//            }
//            Debug.Log("count=" + count);
//        }
//    }
//}