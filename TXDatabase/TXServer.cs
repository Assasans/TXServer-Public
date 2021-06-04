using System;
using System.IO;
using System.Diagnostics;
using SimpleJSON;
using Core;

namespace TXDatabase
{
    public class TXServer
    {
        public static TXServer Instance { get; private set; }
        public static JSONNode Config => Program.Config["TXServer"];

        Process Process;
        public bool IsRunning { get; private set; }
        public TXServer()
        {
            Instance?.Destroy(false);
            Instance = this;

            if (Config["EnableModule"]) Start();
            else Logger.Log("TXServer not started. Module is disabled", "TXServer");
        }
        ~TXServer() {
            if (IsRunning) Destroy(false);
        }

        async void Start()
        {
            string path = Path.GetFullPath(Config["Path"]);
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                // Get stuff from config (btw, casts are nessesary... I think)
                WorkingDirectory = path,
                FileName = Path.Combine(path, (string)Config["Excecutable"]),
                Arguments = (string)Config["Arguments"],
                CreateNoWindow = true, // Do not create a new window for this process
                ErrorDialog = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = false,
                UseShellExecute = false
            };

            try
            {
                Process = Process.Start(startInfo);
                Logger.Log("Process Started", "TXServer");
                Process.ErrorDataReceived += (sendingProcess, errorLine) => Console.WriteLine(errorLine.Data); // Temporary, needs to be unified
                Process.OutputDataReceived += (sendingProcess, dataLine) =>
                {
                    Console.WriteLine(dataLine.Data);
                };
                Process.BeginErrorReadLine();
                Process.BeginOutputReadLine();
                IsRunning = true;
                await Process.WaitForExitAsync();
                IsRunning = false;
                if (!Process.HasExited)
                    Process.Kill();
                Logger.Log($"Process exited with code {Process.ExitCode}", "TXServer");
            } catch (Exception err)
            { Logger.LogError(err.ToString(), "TXServer"); }
        }

        public void Destroy(bool restart = true)
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
            if (restart) new TXServer();
        }
    }
}
