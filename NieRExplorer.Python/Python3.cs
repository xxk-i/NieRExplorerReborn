using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace NieRExplorer.Python
{
	public class Python3
	{
		private ProcessStartInfo startCMD = new ProcessStartInfo();

		public Python3()
		{
			startCMD.FileName = "cmd.exe";
			startCMD.Arguments = "py";
			startCMD.UseShellExecute = false;
			startCMD.RedirectStandardError = true;
			startCMD.CreateNoWindow = true;
			using (Process process = Process.Start(startCMD))
			{
				using (StreamReader streamReader = process.StandardError)
				{
					string text = streamReader.ReadToEnd();
					MessageBox.Show(text.StartsWith("Python 3").ToString());
				}
			}
		}
	}
}
