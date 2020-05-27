using System.Windows.Forms;

namespace NieRExplorer.Explorer
{
	public class ExplorerTreeNode : TreeNode
	{
		private ExplorerItem explorerItem;

		public ExplorerItem ExplorerItem
		{
			get
			{
				return explorerItem;
			}
			set
			{
				explorerItem = value;
			}
		}

		public string Path
		{
			get;
			private set;
		}

		public ExplorerTreeNode(ExplorerItem explorerItem)
		{
			this.explorerItem = explorerItem;
			base.Name = explorerItem.Name;
			base.Text = explorerItem.Name;
			Path = explorerItem.FullPath;
			if (explorerItem.FileData != null)
			{
				base.ImageIndex = explorerItem.FileData.ImageIndex;
				base.SelectedImageIndex = explorerItem.FileData.ImageIndex;
			}
		}
	}
}
