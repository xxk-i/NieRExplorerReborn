using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace NieRExplorer.Properties
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					ResourceManager resourceManager = resourceMan = new ResourceManager("NieRExplorer.Properties.Resources", typeof(Resources).Assembly);
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		internal static Bitmap audioFileIcon
		{
			get
			{
				object @object = ResourceManager.GetObject("audioFileIcon", resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap blank
		{
			get
			{
				object @object = ResourceManager.GetObject("blank", resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap cover
		{
			get
			{
				object @object = ResourceManager.GetObject("cover", resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap cpkFileIcon
		{
			get
			{
				object @object = ResourceManager.GetObject("cpkFileIcon", resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap dllFileIcon
		{
			get
			{
				object @object = ResourceManager.GetObject("dllFileIcon", resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap dttFileIcon
		{
			get
			{
				object @object = ResourceManager.GetObject("dttFileIcon", resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap folderIcon
		{
			get
			{
				object @object = ResourceManager.GetObject("folderIcon", resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap info
		{
			get
			{
				object @object = ResourceManager.GetObject("info", resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap usmFileIcon
		{
			get
			{
				object @object = ResourceManager.GetObject("usmFileIcon", resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal Resources()
		{
		}
	}
}
