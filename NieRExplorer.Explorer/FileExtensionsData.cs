namespace NieRExplorer.Explorer
{
	public class FileExtensionsData
	{
		public string Extension
		{
			get;
			private set;
		}

		public string Type
		{
			get;
			private set;
		}

		public int ImageIndex
		{
			get;
			private set;
		}

		public FileExtensionsData(string extension)
		{
			string text = extension.ToLower();
			ImageIndex = 1;
			Type = "File";
			switch (text)
			{
			case ".cpk":
				ImageIndex = 2;
				Type = "CRi Middleware Package";
				break;
			case ".dll":
				ImageIndex = 3;
				Type = "Dynamic Link Library";
				break;
			case ".dtt":
				ImageIndex = 4;
				Type = "Model Data";
				break;
			case ".usm":
				ImageIndex = 5;
				Type = "USM Game Video File";
				break;
			case ".bnk":
				ImageIndex = 6;
				Type = "Sound File";
				break;
			case ".dat":
				Type = "Data File";
				break;
			case ".wmb":
				Type = "3D Model Object";
				break;
			case ".wtp":
				Type = "3D Model Texture Package";
				break;
			case ".csv":
				Type = "Spreadsheet File";
				break;
			case ".dds":
				Type = "DirectDraw Surface (Texture)";
				break;
			case ".enlMeta":
				Type = "Enlighten Meta File";
				break;
			case ".xml":
				Type = "XML File";
				break;
			case ".rss":
				Type = "Resource File";
				break;
			case ".eff":
				Type = "Effect File";
				break;
			case ".txt":
				Type = "Text File";
				break;
			}
		}
	}
}
