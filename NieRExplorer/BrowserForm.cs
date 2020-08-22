using LibCPK;
using NieRExplorer.Data;
using NieRExplorer.Explorer;
using NieRExplorer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NieRExplorer
{
	public class BrowserForm : Form
	{
		public const long CPK_MAX_SIZE_ALERT = 1073741824L;

		public const long CPK_MAX_SIZE_MEM = 2147483648L;

		private string gamePath = string.Empty;

		private DirectoryInfo tempDirectory;

		private Version currentVersion;

		private Version serverVersion;

		private FileSystemWatcher watcher;

		private ImageList iconsImageList;

		private List<ExplorerItem> explorerItems;

		private ContextMenuItems ctxMenuItemsExplorerTreeView;

		private ContextMenuItems ctxMenuItemsCPKListView;

		private readonly string[] SizeSuffixes = new string[9]
		{
			"bytes",
			"KB",
			"MB",
			"GB",
			"TB",
			"PB",
			"EB",
			"ZB",
			"YB"
		};

		private CPKData currentlyOpenCPK;

		private IContainer components = null;

		private StatusStrip statusStrip1;

		private ToolStripStatusLabel statusLabel;

		private ToolStripStatusLabel storageStatusLabel;

		private TreeView explorerTreeView;

		private SplitContainer splitContainer1;

		private MenuStrip menuStrip;

		private ListView explorerListView;

		private TabControl tabControl;

		private TabPage greetTabPage;

		private Panel greetPanel;

		private PictureBox mainLogoPictureBox;

		private TabPage fileBrowserTab;

		private ToolStripMenuItem editToolStripMenuItem;

		private ToolStripMenuItem helpToolStripMenuItem;

		private ToolStripMenuItem checkForUpdatesToolStripMenuItem;

		private ToolStripSeparator toolStripMenuItem1;

		private ToolStripMenuItem aboutNieRExplorerToolStripMenuItem;

		private ListView cpkListView;

		private ToolStrip toolStrip1;

		private ToolStripButton saveChangesCPKBtn;

		private ToolStripMenuItem optionsToolStripMenuItem;

		private LinkLabel linkLabel1;

		private LinkLabel linkLabel2;

		private Label label1;

		private ToolStripMenuItem createDATFileToolStripMenuItem;
        private TextBox CPKSearchBox;

		public BrowserForm()
		{
			InitializeComponent();
			gamePath = AppDomain.CurrentDomain.BaseDirectory;
			currentVersion = Program.CurrentVersion;
			serverVersion = Program.ServerVersion;
			Text = $"NieR:Explorer ({serverVersion.ToString()})";
		}

		private void BrowserForm_Load(object sender, EventArgs e)
		{
			if (currentVersion < serverVersion)
			{
				DialogResult dialogResult = MessageBox.Show($"A new version of NieR:Explorer is available (Version: {serverVersion.ToString()}) Would you like to go to the website and download the new version?", "New Version", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
				if (dialogResult == DialogResult.Yes)
				{
					Process.Start("http://dennisstanistan.com/#nierexplorer");
					Application.Exit();
				}
			}
			InitFileSystemWatcher();
			InitTempFolder();
			InitImageList();
			InitCtxMenus();
			InitExplorerTreeViewProperties();
			InitExplorerListView();
			UpdateStorageLabel();
		}

		private void InitFileSystemWatcher()
		{
			watcher = new FileSystemWatcher();
			watcher.Path = gamePath;
			watcher.NotifyFilter = (NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.LastAccess);
			watcher.Filter = " *.*";
			watcher.Changed += Watcher_OnChanged;
			watcher.Created += Watcher_OnChanged;
			watcher.Deleted += Watcher_OnChanged;
			watcher.Renamed += Watcher_OnRenamed;
			watcher.EnableRaisingEvents = true;
		}

		private void InitTempFolder()
		{
			string path = Path.Combine(gamePath, "_temp");
			if (Directory.Exists(path))
			{
				Directory.Delete(path, recursive: true);
			}
			if (!Directory.Exists(path))
			{
				tempDirectory = Directory.CreateDirectory(path);
				tempDirectory.Attributes = (FileAttributes.Hidden | FileAttributes.Directory);
			}
		}

		private void DeleteTempFolder()
		{
			string path = Path.Combine(gamePath, "_temp");
			if (Directory.Exists(path))
			{
				Directory.Delete(path, recursive: true);
			}
		}

		private void InitImageList()
		{
			iconsImageList = new ImageList();
			iconsImageList.ImageSize = new Size(24, 24);
			iconsImageList.Images.Add(new Bitmap(Resources.folderIcon));
			iconsImageList.Images.Add(new Bitmap(Resources.blank));
			iconsImageList.Images.Add(new Bitmap(Resources.cpkFileIcon));
			iconsImageList.Images.Add(new Bitmap(Resources.dllFileIcon));
			iconsImageList.Images.Add(new Bitmap(Resources.dttFileIcon));
			iconsImageList.Images.Add(new Bitmap(Resources.usmFileIcon));
			iconsImageList.Images.Add(new Bitmap(Resources.audioFileIcon));
			iconsImageList.ColorDepth = ColorDepth.Depth32Bit;
			explorerTreeView.ImageList = iconsImageList;
			explorerTreeView.ImageIndex = 0;
		}

		private void InitExplorerTreeViewProperties()
		{
			explorerTreeView.Nodes.Clear();
			explorerItems = new List<ExplorerItem>();
			ListDirectory(explorerTreeView, gamePath);
			NativeMethods.SetWindowTheme(explorerTreeView.Handle, "explorer", null);
			explorerTreeView.Nodes[0].Expand();
		}

		private void InitExplorerListView()
		{
			cpkListView.Columns.Add("Name", 40, HorizontalAlignment.Left);
			cpkListView.Columns.Add("Type", 40, HorizontalAlignment.Left);
			cpkListView.Columns.Add("Size", 40, HorizontalAlignment.Left);
			cpkListView.Columns.Add("Full Path", 40, HorizontalAlignment.Right);
			cpkListView.SmallImageList = iconsImageList;
			cpkListView.LargeImageList = iconsImageList;
			cpkListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			cpkListView.View = View.Details;
		}

		private void InitCtxMenus()
		{
			ctxMenuItemsExplorerTreeView = new ContextMenuItems();
			ctxMenuItemsExplorerTreeView.AddMenuItem("Reveal in Explorer", RevealInExplorer_OnClick, Shortcut.CtrlShiftR);
			explorerTreeView.ContextMenu = new ContextMenu(ctxMenuItemsExplorerTreeView.MenuItems.ToArray());
			ctxMenuItemsCPKListView = new ContextMenuItems();
			ctxMenuItemsCPKListView.AddMenuItem("Export {0}", ExportFile_OnClick, Shortcut.CtrlE);
			ctxMenuItemsCPKListView.AddMenuItem("Import {0}", ImportFile_OnClick, Shortcut.CtrlI);
			cpkListView.ContextMenu = new ContextMenu(ctxMenuItemsCPKListView.MenuItems.ToArray());
			cpkListView.ContextMenu.Popup += CPKContextMenu_Open;
		}

		private void ListDirectory(TreeView treeView, string path)
		{
			treeView.Nodes.Clear();
			ExplorerItem explorerItem = new ExplorerItem(path);
			treeView.Nodes.Add(CreateDirectoryNode(explorerItem));
		}

		private ExplorerTreeNode CreateDirectoryNode(ExplorerItem explorerItem)
		{
			ExplorerTreeNode explorerTreeNode = new ExplorerTreeNode(explorerItem);
			foreach (ExplorerItem explorerItem2 in explorerItem.ExplorerItems)
			{
				if (explorerItem2.IsFile)
				{
					ExplorerTreeNode node = new ExplorerTreeNode(explorerItem2);
					explorerTreeNode.Nodes.Add(node);
				}
				else
				{
					explorerItems.Add(explorerItem);
					explorerTreeNode.Nodes.Add(CreateDirectoryNode(explorerItem2));
				}
			}
			return explorerTreeNode;
		}

		private string ParseFileSize(double len)
		{
			int num = 0;
			while (len >= 1024.0 && num < SizeSuffixes.Length - 1)
			{
				num++;
				len /= 1024.0;
			}
			return $"{len:0.##} {SizeSuffixes[num]}";
		}

		private void UpdateStorageLabel()
		{
			try
			{
				long num = tempDirectory.GetFiles("*.*", SearchOption.AllDirectories).Sum((FileInfo x) => x.Length);
				long availableFreeSpace = new DriveInfo(Path.GetPathRoot(gamePath)).AvailableFreeSpace;
				storageStatusLabel.Text = $"{ParseFileSize(num)} / {ParseFileSize(availableFreeSpace)}";
			}
			catch
			{
			}
		}

		private void GetFilesFromPath(string directoryname, ref List<string> ls)
		{
			FileInfo[] files = new DirectoryInfo(directoryname).GetFiles();
			DirectoryInfo[] directories = new DirectoryInfo(directoryname).GetDirectories();
			if (files.Length != 0)
			{
				FileInfo[] array = files;
				foreach (FileInfo fileInfo in array)
				{
					ls.Add(fileInfo.FullName);
				}
			}
			if (directories.Length != 0)
			{
				DirectoryInfo[] array2 = directories;
				foreach (DirectoryInfo directoryInfo in array2)
				{
					GetFilesFromPath(directoryInfo.FullName, ref ls);
				}
			}
		}

		private string PatchCPK(CPKData data, string patchDir)
		{
			string text = Path.Combine(tempDirectory.FullName, $"{Path.GetFileNameWithoutExtension(data.FilePath)}-new.cpk");
			string filePath = data.FilePath;
			bool flag = true;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			List<string> ls = new List<string>();
			if (data.CPK != null && Directory.Exists(patchDir))
			{
				GetFilesFromPath(patchDir, ref ls);
				foreach (string item in ls)
				{
					string text2 = item.Remove(0, patchDir.Length + 1);
					text2 = text2.Replace("\\", "/");
					if (!text2.Contains("/"))
					{
						text2 = "/" + text2;
					}
					dictionary.Add(text2, item);
				}
				CPK cPK = data.CPK;
				BinaryReader binaryReader = new BinaryReader(File.OpenRead(data.FilePath));
				BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite(text));
				List<FileEntry> list = cPK.FileTable.OrderBy((FileEntry x) => x.FileOffset).ToList();
				Tools tools = new Tools();
				bool flag2 = Tools.CheckListRedundant(list);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].FileType != "CONTENT")
					{
						if (list[i].FileType == "FILE" && (ulong)binaryWriter.BaseStream.Position < cPK.ContentOffset)
						{
							ulong num = (ulong)((long)cPK.ContentOffset - binaryWriter.BaseStream.Position);
							for (ulong num2 = 0uL; num2 < num; num2++)
							{
								binaryWriter.Write((byte)0);
							}
						}
						int num3 = Convert.ToInt32(list[i].ID);
						string text3 = (!(num3 > 0 && flag2)) ? (((list[i].DirName != null) ? string.Concat(list[i].DirName, "/") : "") + list[i].FileName) : (((list[i].DirName != null) ? string.Concat(list[i].DirName, "/") : "") + $"[{num3.ToString()}]" + list[i].FileName);
						if (!text3.Contains("/"))
						{
							text3 = "/" + text3;
						}
						if (!dictionary.Keys.Contains(text3.ToString()))
						{
							binaryReader.BaseStream.Seek((long)list[i].FileOffset, SeekOrigin.Begin);
							list[i].FileOffset = (ulong)binaryWriter.BaseStream.Position;
							if (list[i].FileName.ToString() == "ETOC_HDR")
							{
								cPK.EtocOffset = list[i].FileOffset;
							}
							cPK.UpdateFileEntry(list[i]);
							byte[] buffer = binaryReader.ReadBytes(int.Parse(list[i].FileSize.ToString()));
							binaryWriter.Write(buffer);
							if (binaryWriter.BaseStream.Position % 2048 > 0 && i < list.Count - 1)
							{
								long position = binaryWriter.BaseStream.Position;
								for (int j = 0; j < 2048 - position % 2048; j++)
								{
									binaryWriter.Write((byte)0);
								}
							}
							continue;
						}
						string path = dictionary[text3.ToString()];
						byte[] array = File.ReadAllBytes(path);
						list[i].FileOffset = (ulong)binaryWriter.BaseStream.Position;
						int num4 = int.Parse(list[i].ExtractSize.ToString());
						int num5 = int.Parse(list[i].FileSize.ToString());
						if (num5 < num4 && list[i].FileType == "FILE" && flag)
						{
							byte[] array2 = cPK.CompressCRILAYLA(array);
							list[i].FileSize = Convert.ChangeType(array2.Length, list[i].FileSizeType);
							list[i].ExtractSize = Convert.ChangeType(array.Length, list[i].FileSizeType);
							cPK.UpdateFileEntry(list[i]);
							binaryWriter.Write(array2);
						}
						else
						{
							list[i].FileSize = Convert.ChangeType(array.Length, list[i].FileSizeType);
							list[i].ExtractSize = Convert.ChangeType(array.Length, list[i].FileSizeType);
							cPK.UpdateFileEntry(list[i]);
							binaryWriter.Write(array);
						}
						if (binaryWriter.BaseStream.Position % 2048 > 0 && i < list.Count - 1)
						{
							long position2 = binaryWriter.BaseStream.Position;
							for (int k = 0; k < 2048 - position2 % 2048; k++)
							{
								binaryWriter.Write((byte)0);
							}
						}
					}
					else
					{
						cPK.UpdateFileEntry(list[i]);
					}
				}
				cPK.WriteCPK(binaryWriter);
				cPK.WriteITOC(binaryWriter);
				cPK.WriteTOC(binaryWriter);
				cPK.WriteETOC(binaryWriter, cPK.EtocOffset);
				cPK.WriteGTOC(binaryWriter);
				binaryWriter.Close();
				binaryWriter.Dispose();
				binaryReader.Close();
				binaryReader = null;
				binaryWriter = null;
				Directory.Delete(patchDir, recursive: true);
				return text;
			}
			MessageBox.Show("Error, cpkdata or patchdata not found.");
			return string.Empty;
		}

		private byte[] PatchCPKMemoryStream(CPKData data, string patchDir)
		{
			byte[] result = null;
			string filePath = data.FilePath;
			bool flag = true;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			List<string> ls = new List<string>();
			if (data.CPK != null && Directory.Exists(patchDir))
			{
				GetFilesFromPath(patchDir, ref ls);
				foreach (string item in ls)
				{
					string text = item.Remove(0, patchDir.Length + 1);
					text = text.Replace("\\", "/");
					if (!text.Contains("/"))
					{
						text = "/" + text;
					}
					dictionary.Add(text, item);
				}
				CPK cPK = data.CPK;
				BinaryReader binaryReader = new BinaryReader(File.OpenRead(data.FilePath));
				List<FileEntry> list = cPK.FileTable.OrderBy((FileEntry x) => x.FileOffset).ToList();
				Tools tools = new Tools();
				MemoryStream memoryStream;
				using (memoryStream = new MemoryStream())
				{
					BinaryWriter binaryWriter;
					using (binaryWriter = new BinaryWriter(memoryStream))
					{
						bool flag2 = Tools.CheckListRedundant(list);
						for (int i = 0; i < list.Count; i++)
						{
							if (list[i].FileType != "CONTENT")
							{
								if (list[i].FileType == "FILE" && (ulong)binaryWriter.BaseStream.Position < cPK.ContentOffset)
								{
									ulong num = (ulong)((long)cPK.ContentOffset - binaryWriter.BaseStream.Position);
									for (ulong num2 = 0uL; num2 < num; num2++)
									{
										binaryWriter.Write((byte)0);
									}
								}
								int num3 = Convert.ToInt32(list[i].ID);
								string text2 = (!(num3 > 0 && flag2)) ? (((list[i].DirName != null) ? string.Concat(list[i].DirName, "/") : "") + list[i].FileName) : (((list[i].DirName != null) ? string.Concat(list[i].DirName, "/") : "") + $"[{num3.ToString()}]" + list[i].FileName);
								if (!text2.Contains("/"))
								{
									text2 = "/" + text2;
								}
								if (!dictionary.Keys.Contains(text2.ToString()))
								{
									binaryReader.BaseStream.Seek((long)list[i].FileOffset, SeekOrigin.Begin);
									list[i].FileOffset = (ulong)binaryWriter.BaseStream.Position;
									if (list[i].FileName.ToString() == "ETOC_HDR")
									{
										cPK.EtocOffset = list[i].FileOffset;
									}
									cPK.UpdateFileEntry(list[i]);
									byte[] buffer = binaryReader.ReadBytes(int.Parse(list[i].FileSize.ToString()));
									binaryWriter.Write(buffer);
									if (binaryWriter.BaseStream.Position % 2048 > 0 && i < list.Count - 1)
									{
										long position = binaryWriter.BaseStream.Position;
										for (int j = 0; j < 2048 - position % 2048; j++)
										{
											binaryWriter.Write((byte)0);
										}
									}
								}
								else
								{
									string path = dictionary[text2.ToString()];
									byte[] array = File.ReadAllBytes(path);
									list[i].FileOffset = (ulong)binaryWriter.BaseStream.Position;
									int num4 = int.Parse(list[i].ExtractSize.ToString());
									int num5 = int.Parse(list[i].FileSize.ToString());
									if (num5 < num4 && list[i].FileType == "FILE" && flag)
									{
										byte[] array2 = cPK.CompressCRILAYLA(array);
										list[i].FileSize = Convert.ChangeType(array2.Length, list[i].FileSizeType);
										list[i].ExtractSize = Convert.ChangeType(array.Length, list[i].FileSizeType);
										cPK.UpdateFileEntry(list[i]);
										binaryWriter.Write(array2);
									}
									else
									{
										list[i].FileSize = Convert.ChangeType(array.Length, list[i].FileSizeType);
										list[i].ExtractSize = Convert.ChangeType(array.Length, list[i].FileSizeType);
										cPK.UpdateFileEntry(list[i]);
										binaryWriter.Write(array);
									}
									if (binaryWriter.BaseStream.Position % 2048 > 0 && i < list.Count - 1)
									{
										long position2 = binaryWriter.BaseStream.Position;
										for (int k = 0; k < 2048 - position2 % 2048; k++)
										{
											binaryWriter.Write((byte)0);
										}
									}
								}
							}
							else
							{
								cPK.UpdateFileEntry(list[i]);
							}
						}
						cPK.WriteCPK(binaryWriter);
						cPK.WriteITOC(binaryWriter);
						cPK.WriteTOC(binaryWriter);
						cPK.WriteETOC(binaryWriter, cPK.EtocOffset);
						cPK.WriteGTOC(binaryWriter);
						binaryReader.Close();
						binaryReader.Dispose();
						binaryWriter.Close();
						binaryWriter.Flush();
					}
					binaryWriter.Dispose();
					Directory.Delete(patchDir, recursive: true);
					memoryStream.Flush();
					return memoryStream.ToArray();
				}
			}
			MessageBox.Show("Error, cpkdata or patchdata not found.");
			return result;
		}

		private void IHateMemoryLeaks(params object[] o)
		{
			object[] array = o;
			foreach (object obj in array)
			{
				o = null;
			}
		}

		// Actually just a directory
		private class CPKGroup
        {
			public CPKGroup(String name)
            {
				dirName = name;
            }

			String headerName()
            {
				return $"{dirName} ({children.Count} Files)";
			}

			public ListViewGroup group()
            {
				return new ListViewGroup(headerName());
            }

			String dirName;
			public List<ListViewItem> children = new List<ListViewItem>();
        }

		private List<String> directoryList = new List<String>();
		private Dictionary<String, CPKGroup> cpkGroupDict = new Dictionary<string, CPKGroup>();

		private void PopulateListViewWithCPKFiles(string cpkPath)
		{
			cpkListView.Groups.Clear();
			currentlyOpenCPK = new CPKData(cpkPath);
			for (int i = 0; i < currentlyOpenCPK.Tables.Count; i++)
			{
				CPKTable cPKTable = currentlyOpenCPK.Tables[i];
				if (!(cPKTable.FileType == "FILE"))
				{
					continue;
				}
				FileExtensionsData fileExtensionsData = new FileExtensionsData(Path.GetExtension(cPKTable.FileName));
				string[] prefixSplit = cPKTable.FileName.Split('/');
				if (prefixSplit[0] != string.Empty && prefixSplit.Length > 1)
				{
					string prefixType = PrefixData.GetPrefixType(prefixSplit[0]);

					CPKGroup g;
					if (!cpkGroupDict.ContainsKey(prefixType))
					{
						g = new CPKGroup(prefixType);
						directoryList.Add(prefixType);
						cpkGroupDict[prefixType] = g;
					}

					else
                    {
						g = cpkGroupDict[prefixType];
					}

					ListViewItem listViewItem = new ListViewItem(cPKTable.LocalName, fileExtensionsData.ImageIndex);
					listViewItem.SubItems.Add(fileExtensionsData.Type);
					listViewItem.SubItems.Add(ParseFileSize(cPKTable.FileSize));
					listViewItem.SubItems.Add(cPKTable.FileName);
					listViewItem.ImageIndex = fileExtensionsData.ImageIndex;
					cpkGroupDict[prefixType].children.Add(listViewItem);
				}
				if (prefixSplit[0] == string.Empty)
				{
					String prefixType = "Root1";
					CPKGroup g;
					if (!cpkGroupDict.ContainsKey(prefixType))
					{
						g = new CPKGroup(prefixType);
						directoryList.Add(prefixType);
						cpkGroupDict[prefixType] = g;
					}

					else
					{
						g = cpkGroupDict[prefixType];
					}
					ListViewItem listViewItem2 = new ListViewItem(cPKTable.LocalName, fileExtensionsData.ImageIndex);
					listViewItem2.SubItems.Add(fileExtensionsData.Type);
					listViewItem2.SubItems.Add(ParseFileSize(cPKTable.FileSize));
					listViewItem2.SubItems.Add(cPKTable.FileName);
					listViewItem2.ImageIndex = fileExtensionsData.ImageIndex;
					g.children.Add(listViewItem2);	
				}
				if (prefixSplit.Length == 1)
				{
					String prefixType = "Root2";
					CPKGroup g;
					if (!cpkGroupDict.ContainsKey(prefixType))
					{
						g = new CPKGroup(prefixType);
						directoryList.Add(prefixType);
						cpkGroupDict[prefixType] = g;
					}

					else
					{
						g = cpkGroupDict[prefixType];
					}
					ListViewItem listViewItem3 = new ListViewItem(cPKTable.LocalName, fileExtensionsData.ImageIndex);
					listViewItem3.SubItems.Add(fileExtensionsData.Type);
					listViewItem3.SubItems.Add(ParseFileSize(cPKTable.FileSize));
					listViewItem3.SubItems.Add(cPKTable.FileName);
					listViewItem3.ImageIndex = fileExtensionsData.ImageIndex;
					g.children.Add(listViewItem3);
				}
			}
			
			PopulateFilteredListFromItems("");
			cpkListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		private void CPKSearchBox_TextChanged(object sender, EventArgs e)
		{
			PopulateFilteredListFromItems(CPKSearchBox.Text);
		}

		private void PopulateFilteredListFromItems(String filter) 
		{
			// first clear the view
			cpkListView.Items.Clear();

			foreach(String dir in directoryList)
            {
				CPKGroup g = cpkGroupDict[dir];
				List<ListViewItem> l = g.children.Where<ListViewItem>(x => x.Text.Contains(filter)).ToList();
				if (l.Count == 0) continue;

				ListViewGroup group = g.group();
				cpkListView.Groups.Add(group);
				foreach (ListViewItem item in l)
                {
					item.Group = group;
					cpkListView.Items.Add(item);
				}
            }
		}

		private int GetIndexOfGroup(ListView listView, string header)
		{
			foreach (ListViewGroup group in listView.Groups)
			{
				if (group.Header == header)
				{
					return listView.Groups.IndexOf(group);
				}
			}
			return -1;
		}

		private bool GroupExists(ListView listView, string header)
		{
			foreach (ListViewGroup group in listView.Groups)
			{
				if (group.Header == header)
				{
					return true;
				}
			}
			return false;
		}

		private void ExportFileFromCPK(CPKData cpkData, string file)
		{
			CPKTable cPKTable = cpkData.Tables.Where((CPKTable x) => x.FileName == file).First();
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.FileName = cPKTable.LocalName;
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				string fileName = saveFileDialog.FileName;
				File.WriteAllBytes(saveFileDialog.FileName, cPKTable.Extract(currentlyOpenCPK.FilePath));
			}
		}

		private bool GenerateDAT(string directory, string datName)
		{
			FilePackInfo packInfo = new FilePackInfo(directory);
			if (!Directory.Exists(directory))
			{
				return false;
			}
			try
			{
				Writer writer = new Writer(packInfo);
				writer.WriteToFile(Path.Combine(directory, datName));
				return true;
			}
			catch
			{
				return false;
			}
		}

		private void explorerTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				TreeNode selectedNode = explorerTreeView.SelectedNode;
				selectedNode = explorerTreeView.GetNodeAt(e.X, e.Y);
			}
		}

		private void BrowserForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			DeleteTempFolder();
		}

		private void RevealInExplorer_OnClick(object sender, EventArgs e)
		{
			ExplorerTreeNode explorerTreeNode = (ExplorerTreeNode)explorerTreeView.SelectedNode;
			string arguments = $"/e, /select, \"{explorerTreeNode.ExplorerItem.FullPath}\"";
			Process.Start("explorer.exe", arguments);
		}

		private void ExportFile_OnClick(object sender, EventArgs e)
		{
			ListViewItem listViewItem = cpkListView.SelectedItems[0];
			MenuItem menuItem = (MenuItem)sender;
			ExportFileFromCPK(currentlyOpenCPK, listViewItem.SubItems[3].Text);
		}

		private void ImportFile_OnClick(object sender, EventArgs e)
		{
			ListViewItem listViewItem = cpkListView.SelectedItems[0];
			MenuItem menuItem = (MenuItem)sender;
			OpenFileDialog openFileDialog = new OpenFileDialog();
			string extension = Path.GetExtension(listViewItem.Text);
			string fileName = listViewItem.SubItems[3].Text;
			openFileDialog.Filter = string.Format("{0} (*{1}) |*{1}", new FileExtensionsData(extension).Type, extension);
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				CPKTable cPKTable = currentlyOpenCPK.Tables.Where((CPKTable x) => x.FileName == fileName).First();
				string fileName2 = Path.GetFileName(currentlyOpenCPK.FilePath);
				string text = Path.Combine(tempDirectory.FullName, fileName2);
				string[] array = cPKTable.FileName.Split('/');
				DirectoryInfo directoryInfo = null;
				File.Copy(destFileName: Path.Combine(((array.Length <= 1) ? Directory.CreateDirectory(text) : Directory.CreateDirectory(Path.Combine(text, array[0]))).FullName, cPKTable.LocalName), sourceFileName: openFileDialog.FileName, overwrite: true);
				listViewItem.ForeColor = Color.Blue;
				saveChangesCPKBtn.Enabled = true;
			}
			UpdateStorageLabel();
		}

		private void Watcher_OnChanged(object source, FileSystemEventArgs e)
		{
			UpdateStorageLabel();
		}

		private void Watcher_OnRenamed(object source, FileSystemEventArgs e)
		{
		}

		private void explorerTreeView_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			try
			{
				string text = string.Empty;
				string path = string.Empty;
				bool flag = false;
				if (currentlyOpenCPK != null)
				{
					text = currentlyOpenCPK.FilePath;
				}
				if (text != string.Empty)
				{
					path = Path.Combine(tempDirectory.FullName, Path.GetFileName(text));
					flag = Directory.Exists(path);
				}
				if (!flag)
				{
					goto IL_008c;
				}
				DialogResult dialogResult = MessageBox.Show("You have unsaved changes, do you wish to continue?", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
				if (dialogResult != DialogResult.No)
				{
					goto IL_008c;
				}
				goto end_IL_0001;
				IL_008c:
				ExplorerTreeNode explorerTreeNode = (ExplorerTreeNode)explorerTreeView.SelectedNode;
				if (Path.GetExtension(explorerTreeNode.Path).ToLower() == ".cpk")
				{
					PopulateListViewWithCPKFiles(explorerTreeNode.Path);
					tabControl.SelectTab(1);
					tabControl.TabPages[1].Text = $"File Browser ({explorerTreeNode.Name})";
					if (text != string.Empty && Directory.Exists(path))
					{
						Directory.Delete(path, recursive: true);
					}
					saveChangesCPKBtn.Enabled = false;
				}
				UpdateStorageLabel();
				end_IL_0001:;
			}
			catch
			{
			}
		}

		private void CPKContextMenu_Open(object sender, EventArgs e)
		{
			ContextMenu contextMenu = (ContextMenu)sender;
			if (cpkListView.SelectedItems.Count > 0)
			{
				contextMenu.MenuItems[0].Text = $"Export {cpkListView.SelectedItems[0].Text}";
				contextMenu.MenuItems[1].Text = $"Import {cpkListView.SelectedItems[0].Text}";
				contextMenu.MenuItems[0].Visible = true;
				contextMenu.MenuItems[1].Visible = true;
			}
			else
			{
				contextMenu.MenuItems[0].Visible = false;
				contextMenu.MenuItems[1].Visible = false;
			}
		}

		private void saveChangesCPKBtn_Click(object sender, EventArgs e)
		{
			bool useMemoryMethod = Program.settingsData.UseMemoryMethod;
			base.Enabled = false;
			long length = new FileInfo(currentlyOpenCPK.FilePath).Length;
			if (length > 1073741824)
			{
				if (!useMemoryMethod)
				{
					DialogResult dialogResult = MessageBox.Show($"You are about to patch a CPK file that is larger than {ParseFileSize(1073741824.0)} in size, this may take a long time depending on your hardware. Do you want to continue?", "File Size Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
					if (dialogResult == DialogResult.No)
					{
						base.Enabled = true;
						return;
					}
				}
				else if (length > 2147483648u && useMemoryMethod)
				{
					MessageBox.Show($"Patching CPK files that are larger than {ParseFileSize(2147483648.0)} using the memory method is currently not supported.", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					base.Enabled = true;
					return;
				}
			}
			if (!useMemoryMethod)
			{
				Task.Run(delegate
				{
					Stopwatch stopwatch2 = new Stopwatch();
					stopwatch2.Start();
					string fileName2 = Path.GetFileName(currentlyOpenCPK.FilePath);
					string patchDir2 = Path.Combine(tempDirectory.FullName, fileName2);
					string a = PatchCPK(currentlyOpenCPK, patchDir2);
					if (a != string.Empty)
					{
						string sourceFileName = Path.Combine(tempDirectory.FullName, $"{Path.GetFileNameWithoutExtension(currentlyOpenCPK.FilePath)}-new.cpk");
						File.Delete(currentlyOpenCPK.FilePath);
						File.Move(sourceFileName, currentlyOpenCPK.FilePath);
						stopwatch2.Stop();
						MessageBox.Show($"Changes saved! Time elapsed: {stopwatch2.Elapsed:hh\\:mm\\:ss}");
					}
					else
					{
						MessageBox.Show("An error occured, could not save changes.");
					}
				}).ContinueWith(delegate
				{
					Invoke((Action)delegate
					{
						PopulateListViewWithCPKFiles(currentlyOpenCPK.FilePath);
						base.Enabled = true;
						saveChangesCPKBtn.Enabled = false;
					});
				});
			}
			else
			{
				Task.Run(delegate
				{
					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();
					string fileName = Path.GetFileName(currentlyOpenCPK.FilePath);
					string patchDir = Path.Combine(tempDirectory.FullName, fileName);
					byte[] array = PatchCPKMemoryStream(currentlyOpenCPK, patchDir);
					if (array != null)
					{
						File.WriteAllBytes(currentlyOpenCPK.FilePath, array);
						stopwatch.Stop();
						Array.Clear(array, 0, array.Length);
						patchDir = Path.Combine(tempDirectory.FullName, Path.GetFileName(currentlyOpenCPK.FilePath));
						MessageBox.Show($"Changes saved! Time elapsed: {stopwatch.Elapsed:hh\\:mm\\:ss}");
					}
					else
					{
						MessageBox.Show("An error occured, could not save changes.");
					}
				}).ContinueWith(delegate
				{
					Invoke((Action)delegate
					{
						PopulateListViewWithCPKFiles(currentlyOpenCPK.FilePath);
						base.Enabled = true;
						saveChangesCPKBtn.Enabled = false;
					});
				});
			}
		}

		private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OptionsForm optionsForm = new OptionsForm();
			optionsForm.ShowDialog();
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://github.com/wmltogether/CriPakTools");
		}

		private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://github.com/xxk-i/DATrepacker/");
		}

		private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://dennisstanistan.com/#nierexplorer");
		}

		private void aboutNieRExplorerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			tabControl.SelectTab(0);
		}

		private void createDATFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "DAT File|*dat";
			saveFileDialog.Title = "Save DAT file";
			saveFileDialog.FileName = "GeneratedDAT.dat";
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				if (GenerateDAT(folderBrowserDialog.SelectedPath, saveFileDialog.FileName))
				{
					MessageBox.Show("DAT file saved.");
				}
				else
				{
					MessageBox.Show("An error occured, could not generate DAT file.");
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowserForm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.storageStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.explorerTreeView = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.greetTabPage = new System.Windows.Forms.TabPage();
            this.greetPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.mainLogoPictureBox = new System.Windows.Forms.PictureBox();
            this.fileBrowserTab = new System.Windows.Forms.TabPage();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.saveChangesCPKBtn = new System.Windows.Forms.ToolStripButton();
            this.cpkListView = new System.Windows.Forms.ListView();
            this.explorerListView = new System.Windows.Forms.ListView();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createDATFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutNieRExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CPKSearchBox = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.greetTabPage.SuspendLayout();
            this.greetPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainLogoPictureBox)).BeginInit();
            this.fileBrowserTab.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.storageStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 539);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(756, 17);
            this.statusLabel.Spring = true;
            this.statusLabel.Text = "Ready";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // storageStatusLabel
            // 
            this.storageStatusLabel.Name = "storageStatusLabel";
            this.storageStatusLabel.Size = new System.Drawing.Size(13, 17);
            this.storageStatusLabel.Text = "0";
            this.storageStatusLabel.ToolTipText = "Temp Folder Size / Drive Size";
            // 
            // explorerTreeView
            // 
            this.explorerTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.explorerTreeView.Location = new System.Drawing.Point(0, 0);
            this.explorerTreeView.Name = "explorerTreeView";
            this.explorerTreeView.Size = new System.Drawing.Size(261, 515);
            this.explorerTreeView.TabIndex = 1;
            this.explorerTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.explorerTreeView_NodeMouseClick);
            this.explorerTreeView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.explorerTreeView_MouseDoubleClick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.explorerTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl);
            this.splitContainer1.Panel2.Controls.Add(this.explorerListView);
            this.splitContainer1.Size = new System.Drawing.Size(784, 515);
            this.splitContainer1.SplitterDistance = 261;
            this.splitContainer1.TabIndex = 2;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.greetTabPage);
            this.tabControl.Controls.Add(this.fileBrowserTab);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(519, 515);
            this.tabControl.TabIndex = 0;
            // 
            // greetTabPage
            // 
            this.greetTabPage.Controls.Add(this.greetPanel);
            this.greetTabPage.Location = new System.Drawing.Point(4, 22);
            this.greetTabPage.Name = "greetTabPage";
            this.greetTabPage.Size = new System.Drawing.Size(511, 489);
            this.greetTabPage.TabIndex = 0;
            this.greetTabPage.Text = "About";
            this.greetTabPage.UseVisualStyleBackColor = true;
            // 
            // greetPanel
            // 
            this.greetPanel.AutoScroll = true;
            this.greetPanel.Controls.Add(this.label1);
            this.greetPanel.Controls.Add(this.linkLabel2);
            this.greetPanel.Controls.Add(this.linkLabel1);
            this.greetPanel.Controls.Add(this.mainLogoPictureBox);
            this.greetPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.greetPanel.Location = new System.Drawing.Point(0, 0);
            this.greetPanel.Name = "greetPanel";
            this.greetPanel.Size = new System.Drawing.Size(511, 489);
            this.greetPanel.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 215);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(500, 76);
            this.label1.TabIndex = 3;
			this.label1.Text = "NierExplorer: Reborn\r\nImprovements to original NieRExplorer by Dennis Stanistanr\nCredits for wmltogther for the CriPak-Tools fork";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkLabel2
            // 
            this.linkLabel2.Location = new System.Drawing.Point(3, 314);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(500, 23);
            this.linkLabel2.TabIndex = 2;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "https://github.com/xxk-i/DATrepacker/";
            this.linkLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Location = new System.Drawing.Point(3, 291);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(500, 23);
            this.linkLabel1.TabIndex = 2;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "https://github.com/wmltogether/CriPakTools";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // mainLogoPictureBox
            // 
            this.mainLogoPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainLogoPictureBox.BackgroundImage = global::NieRExplorer.Properties.Resources.cover;
            this.mainLogoPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mainLogoPictureBox.Location = new System.Drawing.Point(3, 3);
            this.mainLogoPictureBox.Name = "mainLogoPictureBox";
            this.mainLogoPictureBox.Size = new System.Drawing.Size(505, 199);
            this.mainLogoPictureBox.TabIndex = 0;
            this.mainLogoPictureBox.TabStop = false;
            // 
            // fileBrowserTab
            // 
            this.fileBrowserTab.Controls.Add(this.CPKSearchBox);
            this.fileBrowserTab.Controls.Add(this.toolStrip1);
            this.fileBrowserTab.Controls.Add(this.cpkListView);
            this.fileBrowserTab.Location = new System.Drawing.Point(4, 22);
            this.fileBrowserTab.Name = "fileBrowserTab";
            this.fileBrowserTab.Size = new System.Drawing.Size(511, 489);
            this.fileBrowserTab.TabIndex = 0;
            this.fileBrowserTab.Text = "File Browser";
            this.fileBrowserTab.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(30, 30);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveChangesCPKBtn});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(511, 37);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // saveChangesCPKBtn
            // 
            this.saveChangesCPKBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveChangesCPKBtn.Enabled = false;
            this.saveChangesCPKBtn.Image = ((System.Drawing.Image)(resources.GetObject("saveChangesCPKBtn.Image")));
            this.saveChangesCPKBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveChangesCPKBtn.Name = "saveChangesCPKBtn";
            this.saveChangesCPKBtn.Size = new System.Drawing.Size(34, 34);
            this.saveChangesCPKBtn.Text = "Save Changes";
            this.saveChangesCPKBtn.Click += new System.EventHandler(this.saveChangesCPKBtn_Click);
            // 
            // cpkListView
            // 
            this.cpkListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cpkListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cpkListView.FullRowSelect = true;
            this.cpkListView.HideSelection = false;
            this.cpkListView.Location = new System.Drawing.Point(0, 40);
            this.cpkListView.MultiSelect = false;
            this.cpkListView.Name = "cpkListView";
            this.cpkListView.Size = new System.Drawing.Size(511, 449);
            this.cpkListView.TabIndex = 0;
            this.cpkListView.UseCompatibleStateImageBehavior = false;
            // 
            // explorerListView
            // 
            this.explorerListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.explorerListView.FullRowSelect = true;
            this.explorerListView.HideSelection = false;
            this.explorerListView.Location = new System.Drawing.Point(0, 0);
            this.explorerListView.MultiSelect = false;
            this.explorerListView.Name = "explorerListView";
            this.explorerListView.Size = new System.Drawing.Size(519, 515);
            this.explorerListView.TabIndex = 0;
            this.explorerListView.UseCompatibleStateImageBehavior = false;
            this.explorerListView.View = System.Windows.Forms.View.Details;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(784, 24);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "menuStrip1";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createDATFileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // createDATFileToolStripMenuItem
            // 
            this.createDATFileToolStripMenuItem.Name = "createDATFileToolStripMenuItem";
            this.createDATFileToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.createDATFileToolStripMenuItem.Text = "Create DAT File";
            this.createDATFileToolStripMenuItem.Click += new System.EventHandler(this.createDATFileToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForUpdatesToolStripMenuItem,
            this.toolStripMenuItem1,
            this.aboutNieRExplorerToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            this.checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.checkForUpdatesToolStripMenuItem.Text = "Check for Updates";
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(178, 6);
            // 
            // aboutNieRExplorerToolStripMenuItem
            // 
            this.aboutNieRExplorerToolStripMenuItem.Name = "aboutNieRExplorerToolStripMenuItem";
            this.aboutNieRExplorerToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.aboutNieRExplorerToolStripMenuItem.Text = "About NieR:Explorer";
            this.aboutNieRExplorerToolStripMenuItem.Click += new System.EventHandler(this.aboutNieRExplorerToolStripMenuItem_Click);
            // 
            // CPKSearchBox
            // 
            this.CPKSearchBox.Location = new System.Drawing.Point(351, 14);
            this.CPKSearchBox.Name = "CPKSearchBox";
            this.CPKSearchBox.Size = new System.Drawing.Size(152, 20);
            this.CPKSearchBox.TabIndex = 2;
            this.CPKSearchBox.TextChanged += new System.EventHandler(this.CPKSearchBox_TextChanged);
            // 
            // BrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "BrowserForm";
            this.Text = "NieRExplorer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BrowserForm_FormClosing);
            this.Load += new System.EventHandler(this.BrowserForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.greetTabPage.ResumeLayout(false);
            this.greetPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainLogoPictureBox)).EndInit();
            this.fileBrowserTab.ResumeLayout(false);
            this.fileBrowserTab.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
    }
}
