using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class WriteLog : MonoBehaviour
{
    private static FileStream FileWriter;
    private static UTF8Encoding encoding;
    private static WriteLog _consoleLog;
    private static bool _AllDisplay;
    private static bool _LogDisplay;
    private static bool _WarningDisplay;
    private static bool _LogData;
    private static bool IsIDE;
    private FileInfo fileInfo;
    private string NowTime;

    public static WriteLog console //��������
    {
        get
        {
            if (_consoleLog == null)
                _consoleLog = new WriteLog();
            return _consoleLog;
        }
    }

    /// <summary>
    ///     ��ʼд����־������һ���Ƿ�д��Warning�������ݣ�Ĭ�ϲ�д�룬���������Ƿ�д��Debug.Log�������ݣ�Ĭ�ϲ�д�룬���������Ƿ�д��ȫ�����ݣ�Ĭ�ϲ�д��,�����ģ��Ƿ�Log������Ϣ���������̨��Ĭ�����
    /// </summary>
    /// <param name="WarningDisplay"></param>
    public void LogStart(bool WarningDisplay = false, bool LogDisplay = false, bool AllDisplay = false,
        bool LogData = true)
    {

        if ((FileWriter == null))
        {
            IsIDE = Application.isEditor; //��ȡ��ǰ�������л���
            _WarningDisplay = WarningDisplay;
            _LogDisplay = LogDisplay;
            _AllDisplay = AllDisplay;
            _LogData = LogData;
            if (IsIDE == false) //�жϵ�ǰ�������л����������Editor����ִ��
            {
                Directory.CreateDirectory(Application.dataPath + "/StreamingAssets");
                Directory.CreateDirectory(Application.dataPath + "/StreamingAssets/" + "Log");
                NowTime = DateTime.Now.ToString().Replace(" ", "_").Replace("/", "_").Replace(":", "_");
                fileInfo = new FileInfo(Application.dataPath + "/StreamingAssets/Log/" + NowTime + "_Log.txt");
                //����Log�ļ������ַ
                FileWriter = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                encoding = new UTF8Encoding();
                Application.logMessageReceived += LogCallback;
            }
        }
    }

    /// <summary>
    ///     ���Debug.logд��Log��Ϣ
    /// </summary>
    /// <param name="_log"></param>
    /// <param name="con"></param>
    public static void Log(object _log)
    {
        if ((_LogDisplay == false) && (_AllDisplay == false))
        {
            if (_LogData)
                Debug.Log(_log);
            if (IsIDE == false)
            {
                try
                {
                    var trace = new StackTrace(); //��ȡ��������Ϣ
                    var ClassName = trace.GetFrame(1).GetMethod().DeclaringType.Name;
                    var WayName = trace.GetFrame(1).GetMethod().Name;
                    var log = DateTime.Now + " " + "[" + ClassName + "." + WayName + "]" + " " + ":" + " " + _log +
                              Environment.NewLine;
                    FileWriter.Write(encoding.GetBytes(log), 0, encoding.GetByteCount(log));
                }
                catch (Exception)
                {
                    Debug.Log("�����Ƿ������Console.LogStart����,���߹رտ���̨Logд������������д����");
                }

            }
        }
        else
        {
            Debug.Log("�����Ƿ������Console.LogStart����,���߹رտ���̨Logд������������д����");
        }
    }

    private void LogCallback(string condition, string stackTrace, LogType type) //д�����̨����
    {
        string content = null;
        if (_AllDisplay == false)
        {
            if (type.ToString() == "Warning")
                if (_WarningDisplay == false)
                {
                    condition = "";
                    stackTrace = "";
                    content = "";
                }
                else
                {
                    content = DateTime.Now + " " + "[" + type + "]" + "[" + stackTrace + "]" + " " + ":" + " " +
                              condition +
                              Environment.NewLine;
                }

            if (type.ToString() == "Log")
                if (_LogDisplay == false)
                {
                    condition = "";
                    stackTrace = "";
                    content = "";
                }
                else
                {
                    content = DateTime.Now + " " + "[" + type + "]" + "[" + stackTrace + "]" + " " + ":" + " " +
                              condition +
                              Environment.NewLine;
                }
            if (type.ToString() == "Exception")
                content = DateTime.Now + " " + "[" + type + "]" + "[" + stackTrace + "]" + " " + ":" + " " + condition +
                          Environment.NewLine;
        }
        else
        {
            content = DateTime.Now + " " + "[" + type + "]" + "[" + stackTrace + "]" + " " + ":" + " " + condition +
                      Environment.NewLine;
        }
        FileWriter.Write(encoding.GetBytes(content), 0, encoding.GetByteCount(content));
        FileWriter.Flush();
    }

    private void OnDestroy() //�ر�д��
    {
        if ((FileWriter != null) && (IsIDE == false))
        {
            FileWriter.Close();
            Application.logMessageReceived -= LogCallback;
        }
    }
}