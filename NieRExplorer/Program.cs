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
            get { return CurrentVersion; }
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
