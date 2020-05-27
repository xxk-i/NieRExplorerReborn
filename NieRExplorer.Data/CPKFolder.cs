using System.Collections.Generic;

namespace NieRExplorer.Data
{
	public class CPKFolder
	{
		public string Name
		{
			get;
			private set;
		}

		public long ItemsCount
		{
			get;
			private set;
		}

		public long TotalSize
		{
			get;
			private set;
		}

		public List<CPKTable> CPKTables
		{
			get;
			private set;
		}

		public CPKFolder(string name, long itemsCount, long totalSize, List<CPKTable> cpkTable)
		{
			Name = name;
			ItemsCount = itemsCount;
			TotalSize = totalSize;
			CPKTables = cpkTable;
		}
	}
}
