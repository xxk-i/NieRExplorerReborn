using System.Collections.Generic;
using System.IO;

namespace NieRExplorer.Data
{
	public class FilePackInfo
	{
		internal struct MetadataTable
		{
			internal int Offset;

			internal int Size;

			internal int EndPadding;

			internal int EndOffset()
			{
				return Offset + Size + EndPadding;
			}

			internal int TotalSize()
			{
				return Size + EndPadding;
			}
		}

		private string PackDirectory;

		internal int FileCount = 0;

		internal List<string> QualifiedFileNames = new List<string>();

		internal List<string> FileBaseNames = new List<string>();

		internal List<string> FileExtensions;

		internal MetadataTable FileOffsetTable;

		internal MetadataTable ExtensionTable;

		internal MetadataTable NameTable;

		internal int NameTableBlockSize;

		internal MetadataTable SizeTable;

		internal MetadataTable CrcTable;

		internal Dictionary<string, long> FileSizeDict;

		public FilePackInfo(string inDirectory)
		{
			PackDirectory = inDirectory;
			QualifiedFileNames = new List<string>(Directory.GetFiles(PackDirectory));
			FileBaseNames = GetFileBaseNames();
			FileCount = QualifiedFileNames.Count;
			FileExtensions = GetFileExtensions();
			NameTableBlockSize = GetNameTableBlockSize();
			FileSizeDict = GetFileSizeDict();
			MetadataTable metadataTable = new MetadataTable
			{
				Size = 4 * FileCount,
				Offset = 32
			};
			FileOffsetTable = metadataTable;
			metadataTable = new MetadataTable
			{
				Size = 4 * FileCount,
				Offset = FileOffsetTable.EndOffset()
			};
			ExtensionTable = metadataTable;
			metadataTable = new MetadataTable
			{
				Size = 4 + NameTableBlockSize * FileCount,
				Offset = ExtensionTable.EndOffset()
			};
			NameTable = metadataTable;
			NameTable.EndPadding = ((NameTable.EndOffset() % 4 != 0) ? (4 - NameTable.EndOffset() % 4) : 0);
			metadataTable = (SizeTable = new MetadataTable
			{
				Size = 4 * FileCount,
				Offset = NameTable.EndOffset()
			});
			metadataTable = (CrcTable = new MetadataTable
			{
				Size = 16,
				Offset = SizeTable.EndOffset()
			});
			CrcTable.EndPadding = ((CrcTable.EndOffset() % 16 != 0) ? (16 - CrcTable.EndOffset() % 16) : 0);
		}

		private List<string> GetFileExtensions()
		{
			List<string> list = new List<string>(FileCount);
			foreach (string qualifiedFileName in QualifiedFileNames)
			{
				string extension = Path.GetExtension(qualifiedFileName);
				extension = extension.TrimStart('.');
				list.Add(extension);
			}
			return list;
		}

		private int GetNameTableBlockSize()
		{
			int num = 0;
			foreach (string fileBaseName in FileBaseNames)
			{
				if (fileBaseName.Length > num)
				{
					num = fileBaseName.Length;
				}
			}
			return num + 1;
		}

		private List<string> GetFileBaseNames()
		{
			List<string> list = new List<string>(FileCount);
			foreach (string qualifiedFileName in QualifiedFileNames)
			{
				FileInfo fileInfo = new FileInfo(qualifiedFileName);
				list.Add(fileInfo.Name);
			}
			return list;
		}

		private Dictionary<string, long> GetFileSizeDict()
		{
			Dictionary<string, long> dictionary = new Dictionary<string, long>();
			foreach (string qualifiedFileName in QualifiedFileNames)
			{
				dictionary.Add(qualifiedFileName, new FileInfo(qualifiedFileName).Length);
			}
			return dictionary;
		}

		internal List<int> GetHeaderTableOffsets()
		{
			return new List<int>
			{
				FileOffsetTable.Offset,
				ExtensionTable.Offset,
				NameTable.Offset,
				SizeTable.Offset,
				CrcTable.Offset
			};
		}
	}
}
