using System.IO;
using System.Text;

namespace NieRExplorer.Data
{
	internal class SettingsData
	{
		public const string SettingsFileName = "NierExplorerSettings.ini";

		public static SettingsData Default => new SettingsData(useMemoryMethod: false);

		public bool UseMemoryMethod
		{
			get;
			set;
		}

		public SettingsData(bool useMemoryMethod)
		{
			UseMemoryMethod = useMemoryMethod;
		}

		public bool ReadFromFile()
		{
			SettingsData @default = Default;
			try
			{
				string[] array = File.ReadAllLines("NierExplorerSettings.ini");
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split('=');
					if (array2.Length > 1 && array2[0].StartsWith("useMemoryMethod"))
					{
						UseMemoryMethod = bool.Parse(array2[1]);
					}
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool SaveToFile()
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine($"useMemoryMethod={UseMemoryMethod.ToString()}");
				File.WriteAllText("NierExplorerSettings.ini", stringBuilder.ToString());
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
