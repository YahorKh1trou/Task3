using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Task3
{
    public partial class Service1 : ServiceBase
    {
        Logger logger;
        public Service1()
        {
            InitializeComponent();
            CanStop = true;
            CanPauseAndContinue = true;
            AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            logger = new Logger();
            Thread loggerThread = new Thread(new ThreadStart(logger.Start));
            loggerThread.Start();
        }

        protected override void OnStop()
        {
            logger.Stop();
            Thread.Sleep(1000);
        }
    }

    class Logger
    {
        private readonly string MainFolderPath = "C:\\Source";
        private readonly string FileWasRenamed = "переименован в ";
        private readonly string FileWasChanged = "изменен";
        private readonly string FileWasCreated = "создан";
        private readonly string FileWasDeleted = "удален";
        private readonly string LogFilePath = "C:\\testlog\\entry.txt";
        private readonly string FileAction = "{0} файл {1} был {2}";
        private readonly string DateFormat = "dd/MM/yyyy hh:mm:ss";
//        private string[] extensions = { ".csv", ".txt" };
        FileSystemWatcher watcher;
        object obj = new object();
        bool enabled = true;
        public Logger()
        {
            watcher = new FileSystemWatcher(MainFolderPath);
            watcher.Deleted += Watcher_Deleted;
            watcher.Created += Watcher_Created;
            watcher.Changed += Watcher_Changed;
            watcher.Renamed += Watcher_Renamed;
        }

        public void Start()
        {
            watcher.EnableRaisingEvents = true;
            while (enabled)
            {
                Thread.Sleep(1000);
            }
        }
        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            enabled = false;
        }
        // переименование файлов
        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            string fileEvent = FileWasRenamed + e.FullPath;
            string filePath = e.OldFullPath;
            RecordEntry(fileEvent, filePath);
        }
        // изменение файлов
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string fileEvent = FileWasChanged;
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }
        // создание файлов
        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string fileEvent = FileWasCreated;
            string filePath = e.FullPath;
//            string filePathText = (Path.GetExtension(e.FullPath) ?? string.Empty).ToLower();
/*
            if (extensions.Any(filePathText.Equals))
            {
                Process prc = new Process();
                prc.StartInfo.FileName = "C:\\Users\\Yahor\\source\\repos\\Task1\\Task1\\bin\\Debug\\net6.0\\Task1.exe";
                prc.Start();
            }
*/
            RecordEntry(fileEvent, filePath);
        }

        // удаление файлов
        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string fileEvent = FileWasDeleted;
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }
        // запись в лог файл
        private void RecordEntry(string fileEvent, string filePath)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter(LogFilePath, true))
                {
                    writer.WriteLine(String.Format(FileAction,
                        DateTime.Now.ToString(DateFormat), filePath, fileEvent));
                    writer.Flush();
                }
            }
        }
    }
}
