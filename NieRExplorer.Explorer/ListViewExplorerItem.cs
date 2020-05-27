using System.Windows.Forms;

namespace NieRExplorer.Explorer
{
	public class ListViewExplorerItem : ListViewItem
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

		public ListViewExplorerItem(ExplorerItem explorerItem)
		{
			this.explorerItem = explorerItem;
			base.Name = explorerItem.Name;
			base.Text = explorerItem.Name;
			if (explorerItem.FileData != null)
			{
				base.ImageIndex = explorerItem.FileData.ImageIndex;
			}
			else
			{
				base.ImageIndex = 0;
			}
		}
	}
}
