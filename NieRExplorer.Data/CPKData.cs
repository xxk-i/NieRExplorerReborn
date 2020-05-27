using LibCPK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NieRExplorer.Data
{
	public class CPKData
	{
		public static readonly Encoding Encoding = Encoding.GetEncoding(65001);

		public CPK CPK
		{
			get;
			private set;
		}

		public List<CPKTable> Tables
		{
			get;
			private set;
		}

		public int Nums
		{
			get;
			private set;
		}

		public string FilePath
		{
			get;
			private set;
		}

		public CPKData(string filePath)
		{
			FilePath = filePath;
			CPK = new CPK(new Tools());
			CPK.ReadCPK(filePath, Encoding);
			Tables = new List<CPKTable>();
			BinaryReader binaryReader = new BinaryReader(File.OpenRead(filePath));
			List<FileEntry> list = CPK.FileTable.OrderBy((FileEntry x) => x.FileOffset).ToList();
			int i = 0;
			bool flag = Tools.CheckListRedundant(list);
			for (; i < list.Count; i++)
			{
				if (list[i].FileType != null)
				{
					Nums++;
					CPKTable cPKTable = new CPKTable();
					if (list[i].ID == null)
					{
						cPKTable.id = -1;
					}
					else
					{
						cPKTable.id = Convert.ToInt32(list[i].ID);
					}
					if (cPKTable.id >= 0 && flag)
					{
						cPKTable.FileName = ((list[i].DirName != null) ? string.Concat(list[i].DirName, "/") : "") + $"[{cPKTable.id.ToString()}]" + list[i].FileName;
					}
					else
					{
						cPKTable.FileName = ((list[i].DirName != null) ? string.Concat(list[i].DirName, "/") : "") + list[i].FileName;
					}
					cPKTable.LocalName = list[i].FileName.ToString();
					cPKTable.FileOffset = Convert.ToUInt64(list[i].FileOffset);
					cPKTable.FileSize = Convert.ToInt32(list[i].FileSize);
					cPKTable.ExtractSize = Convert.ToInt32(list[i].ExtractSize);
					cPKTable.FileType = list[i].FileType;
					if (list[i].FileType == "FILE")
					{
						cPKTable.Pt = (float)Math.Round((float)cPKTable.FileSize / (float)cPKTable.ExtractSize, 2) * 100f;
					}
					else
					{
						cPKTable.Pt = 100f;
					}
					Tables.Add(cPKTable);
				}
			}
			binaryReader.Close();
		}
	}
}
