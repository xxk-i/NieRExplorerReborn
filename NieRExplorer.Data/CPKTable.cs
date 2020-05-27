using LibCPK;
using System.IO;
using System.Text;

namespace NieRExplorer.Data
{
	public class CPKTable
	{
		public int id
		{
			get;
			set;
		}

		public string FileName
		{
			get;
			set;
		}

		public string LocalName
		{
			get;
			set;
		}

		public ulong FileOffset
		{
			get;
			set;
		}

		public int FileSize
		{
			get;
			set;
		}

		public int ExtractSize
		{
			get;
			set;
		}

		public string FileType
		{
			get;
			set;
		}

		public float Pt
		{
			get;
			set;
		}

		public byte[] Extract(string cpkFile)
		{
			BinaryReader binaryReader = new BinaryReader(File.OpenRead(cpkFile));
			binaryReader.BaseStream.Seek((long)FileOffset, SeekOrigin.Begin);
			string @string = Encoding.ASCII.GetString(binaryReader.ReadBytes(8));
			binaryReader.BaseStream.Seek((long)FileOffset, SeekOrigin.Begin);
			byte[] array = binaryReader.ReadBytes(int.Parse(FileSize.ToString()));
			string b = "CRILAYLA";
			if (@string == b)
			{
				int num = (ExtractSize != 0) ? ExtractSize : FileSize;
				if (num != 0)
				{
					array = new CPK(new Tools()).DecompressLegacyCRI(array, num);
				}
			}
			binaryReader.Close();
			return array;
		}

		public bool Equals(CPKTable obj)
		{
			return id == obj.id;
		}
	}
}
