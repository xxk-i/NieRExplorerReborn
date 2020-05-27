using System;
using System.Runtime.InteropServices;

namespace NieRExplorer
{
	public static class NativeMethods
	{
		[DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
		public static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr window, int message, int wParam, IntPtr lParam);
	}
}
