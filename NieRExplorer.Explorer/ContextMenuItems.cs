using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NieRExplorer.Explorer
{
	public class ContextMenuItems
	{
		public List<MenuItem> MenuItems
		{
			get;
			private set;
		}

		public ContextMenuItems()
		{
			MenuItems = new List<MenuItem>();
		}

		public void AddMenuItem(string text, EventHandler e, Shortcut shortcut)
		{
			MenuItems.Add(new MenuItem(text, e, shortcut));
		}
	}
}
