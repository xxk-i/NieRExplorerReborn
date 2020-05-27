using NieRExplorer.Data;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Windows.Forms;

namespace NieRExplorer
{
	internal static class Program
	{
		public static SettingsData settingsData = new SettingsData(useMemoryMethod: false);

		public static Version CurrentVersion => Assembly.GetEntryAssembly().GetName().Version;

		public static Version ServerVersion
		{
			get
			{
				try
				{
					WebClient webClient = new WebClient();
					byte[] bytes = webClient.DownloadData("http://www.dennisstanistan.com/nierexplorer/nierexplorerver.txt");
					return new Version(Encoding.UTF8.GetString(bytes));
				}
				catch
				{
					return CurrentVersion;
				}
			}
		}

		[STAThread]
		private static void Main()
		{
			AppDomain.CurrentDomain.FirstChanceException += delegate(object sender, FirstChanceExceptionEventArgs eventArgs)
			{
				MessageBox.Show(eventArgs.Exception.ToString());
			};
			CallSettings();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(defaultValue: false);
			Application.Run(new BrowserForm());
		}

		public static void CallSettings()
		{
			if (File.Exists("NierExplorerSettings.ini"))
			{
				settingsData.ReadFromFile();
			}
			else
			{
				settingsData.SaveToFile();
			}
		}
	}
}
