using System;
using System.Diagnostics;
using System.Text;

namespace GitWorker
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Please provide your working dir");
      string workingDir = Console.Read();
      Console.WriteLine("Please name your branch");
      string branchName = Console.ReadLine();

      var result = CommandOutput(branchName, workingDir);

      Console.WriteLine(result);
      Console.ReadKey();
		}

    public static string CommandOutput(string command,
                                     string workingDirectory = null)
    {
      try
      {
        command = $"git checkout -b {command}";
        ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + command);

        procStartInfo.RedirectStandardError = procStartInfo.RedirectStandardInput = procStartInfo.RedirectStandardOutput = true;
        procStartInfo.UseShellExecute = false;
        procStartInfo.CreateNoWindow = true;
        if (null != workingDirectory)
        {
          procStartInfo.WorkingDirectory = workingDirectory;
        }

        Process proc = new Process();
        proc.StartInfo = procStartInfo;
        proc.Start();

        StringBuilder sb = new StringBuilder();
        proc.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e)
        {
          sb.AppendLine(e.Data);
        };
        proc.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e)
        {
          sb.AppendLine(e.Data);
        };

        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();
        proc.WaitForExit();
        return sb.ToString();
      }
      catch (Exception objException)
      {
        return $"Error in command: {command}, {objException.Message}";
      }
    }
  }
}
