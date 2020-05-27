using System;
using System.IO;
using System.Text;

namespace NieRExplorer.Data
{
	public class Writer
	{
		private static readonly byte[] MAGIC = new byte[4]
		{
			68,
			65,
			84,
			0
		};

		private static readonly byte[] NUL = new byte[1];

		private readonly FilePackInfo PackInfo;

		public Writer(FilePackInfo packInfo)
		{
			PackInfo = packInfo;
		}

		public void WriteToFile(string outFile)
		{
			if (PackInfo == null)
			{
				throw new InvalidOperationException("Error: need valid FilePackInfo to write file");
			}
			try
			{
				using (FileStream stream = new FileStream(outFile, FileMode.Create, FileAccess.Write))
				{
					WriteFile(stream);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private byte[] PaddedBytes(int num)
		{
			byte[] bytes = BitConverter.GetBytes(num);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}

		private static void _write(FileStream stream, byte[] bytes)
		{
			stream.Write(bytes, 0, bytes.Length);
		}

		private static int _calculate16BytePadding(int offset)
		{
			int num = offset % 16;
			if (num == 0)
			{
				return 0;
			}
			return 16 - num;
		}

		private static byte[] NulPad(int length)
		{
			return new byte[length];
		}

		private void WriteFile(FileStream stream)
		{
			FilePackInfo packInfo = PackInfo;
			stream.Write(MAGIC, 0, 4);
			_write(stream, PaddedBytes(packInfo.FileCount));
			foreach (int headerTableOffset in packInfo.GetHeaderTableOffsets())
			{
				_write(stream, PaddedBytes(headerTableOffset));
			}
			_write(stream, PaddedBytes(0));
			int num = packInfo.CrcTable.EndOffset();
			foreach (string qualifiedFileName in packInfo.QualifiedFileNames)
			{
				int num2 = (int)packInfo.FileSizeDict[qualifiedFileName];
				_write(stream, PaddedBytes(num));
				num += num2;
				num += _calculate16BytePadding(num);
			}
			foreach (string fileExtension in packInfo.FileExtensions)
			{
				_write(stream, Encoding.ASCII.GetBytes(fileExtension));
				_write(stream, NUL);
			}
			_write(stream, PaddedBytes(packInfo.NameTableBlockSize));
			foreach (string fileBaseName in packInfo.FileBaseNames)
			{
				int length = packInfo.NameTableBlockSize - fileBaseName.Length;
				_write(stream, Encoding.ASCII.GetBytes(fileBaseName));
				_write(stream, NulPad(length));
			}
			int num3 = (int)stream.Position % 4;
			if (num3 != 0)
			{
				_write(stream, NulPad(4 - num3));
			}
			foreach (string qualifiedFileName2 in packInfo.QualifiedFileNames)
			{
				_write(stream, PaddedBytes((int)packInfo.FileSizeDict[qualifiedFileName2]));
			}
			_write(stream, NulPad(packInfo.CrcTable.TotalSize()));
			foreach (string qualifiedFileName3 in packInfo.QualifiedFileNames)
			{
				int offset = (int)packInfo.FileSizeDict[qualifiedFileName3];
				int num4 = _calculate16BytePadding(offset);
				byte[] bytes = File.ReadAllBytes(qualifiedFileName3);
				_write(stream, bytes);
				if (num4 > 0)
				{
					_write(stream, NulPad(num4));
				}
			}
		}
	}
}
