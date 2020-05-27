using System.Collections.Generic;
using System.IO;

namespace NieRExplorer.Explorer
{
	public class ExplorerItem
	{
		public List<ExplorerItem> ExplorerItems
		{
			get;
			private set;
		}

		public FileExtensionsData FileData
		{
			get;
			private set;
		}

		public string FullPath
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public int ImageIndex
		{
			get;
			private set;
		}

		public bool IsFile
		{
			get;
			private set;
		}

		public ExplorerItem(string fullPath)
		{
			FullPath = fullPath;
			FileAttributes attributes = File.GetAttributes(fullPath);
			ExplorerItems = new List<ExplorerItem>();
			IsFile = false;
			if (attributes.HasFlag(FileAttributes.Directory) && !attributes.HasFlag(FileAttributes.Hidden))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(fullPath);
				Name = directoryInfo.Name;
				DirectoryInfo[] directories = directoryInfo.GetDirectories();
				FileInfo[] files = directoryInfo.GetFiles();
				for (int i = 0; i < directories.Length; i++)
				{
					ExplorerItems.Add(new ExplorerItem(directories[i].FullName));
				}
				for (int j = 0; j < files.Length; j++)
				{
					ExplorerItems.Add(new ExplorerItem(files[j].FullName));
				}
			}
			else
			{
				FileInfo fileInfo = new FileInfo(fullPath);
				Name = fileInfo.Name;
				IsFile = true;
				FileData = new FileExtensionsData(fileInfo.Extension);
			}
		}
	}
}
