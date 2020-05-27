using NieRExplorer.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NieRExplorer
{
	public class OptionsForm : Form
	{
		private IContainer components = null;

		private GroupBox groupBox1;

		private RadioButton patchingMethodRB2;

		private RadioButton patchingMethodRB1;

		private PictureBox infoIcon;

		private Label patchingDescLabel;

		private Button saveBtn;

		public OptionsForm()
		{
			InitializeComponent();
		}

		private void OptionsForm_Load(object sender, EventArgs e)
		{
			if (Program.settingsData.UseMemoryMethod)
			{
				patchingMethodRB1.Checked = false;
				patchingMethodRB2.Checked = true;
			}
		}

		private void saveBtn_Click(object sender, EventArgs e)
		{
			Program.settingsData.SaveToFile();
		}

		private void patchingMethodRB1_CheckedChanged(object sender, EventArgs e)
		{
			patchingDescLabel.Text = "This will write the modified CPK file to disk and then will be used to override the CPK file inside the Data folder at a cost of disk space. Temporary folders will be deleted when the application is closed.";
			Program.settingsData.UseMemoryMethod = false;
		}

		private void patchingMethodRB2_CheckedChanged(object sender, EventArgs e)
		{
			patchingDescLabel.Text = "This will write the modified CPK file to memory and then will be written to the CPK file inside the Data folder at a cost of RAM. You will want to use this setting if you are low on disk space but high on memory (4GB+ of RAM is recommended if you plan on using this method)";
			Program.settingsData.UseMemoryMethod = true;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NieRExplorer.OptionsForm));
			groupBox1 = new System.Windows.Forms.GroupBox();
			patchingMethodRB1 = new System.Windows.Forms.RadioButton();
			patchingMethodRB2 = new System.Windows.Forms.RadioButton();
			infoIcon = new System.Windows.Forms.PictureBox();
			patchingDescLabel = new System.Windows.Forms.Label();
			saveBtn = new System.Windows.Forms.Button();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)infoIcon).BeginInit();
			SuspendLayout();
			groupBox1.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
			groupBox1.Controls.Add(patchingDescLabel);
			groupBox1.Controls.Add(infoIcon);
			groupBox1.Controls.Add(patchingMethodRB2);
			groupBox1.Controls.Add(patchingMethodRB1);
			groupBox1.Location = new System.Drawing.Point(12, 12);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(393, 179);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = "Patching Methods";
			patchingMethodRB1.AutoSize = true;
			patchingMethodRB1.Checked = true;
			patchingMethodRB1.Location = new System.Drawing.Point(6, 19);
			patchingMethodRB1.Name = "patchingMethodRB1";
			patchingMethodRB1.Size = new System.Drawing.Size(167, 17);
			patchingMethodRB1.TabIndex = 0;
			patchingMethodRB1.TabStop = true;
			patchingMethodRB1.Text = "Write to Disk (Recommended)";
			patchingMethodRB1.UseVisualStyleBackColor = true;
			patchingMethodRB1.CheckedChanged += new System.EventHandler(patchingMethodRB1_CheckedChanged);
			patchingMethodRB2.AutoSize = true;
			patchingMethodRB2.Location = new System.Drawing.Point(6, 42);
			patchingMethodRB2.Name = "patchingMethodRB2";
			patchingMethodRB2.Size = new System.Drawing.Size(102, 17);
			patchingMethodRB2.TabIndex = 0;
			patchingMethodRB2.Text = "Write to Memory";
			patchingMethodRB2.UseVisualStyleBackColor = true;
			patchingMethodRB2.CheckedChanged += new System.EventHandler(patchingMethodRB2_CheckedChanged);
			infoIcon.BackgroundImage = NieRExplorer.Properties.Resources.info;
			infoIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			infoIcon.Location = new System.Drawing.Point(6, 65);
			infoIcon.Name = "infoIcon";
			infoIcon.Size = new System.Drawing.Size(32, 32);
			infoIcon.TabIndex = 1;
			infoIcon.TabStop = false;
			patchingDescLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			patchingDescLabel.Location = new System.Drawing.Point(44, 65);
			patchingDescLabel.Name = "patchingDescLabel";
			patchingDescLabel.Size = new System.Drawing.Size(316, 111);
			patchingDescLabel.TabIndex = 2;
			patchingDescLabel.Text = resources.GetString("patchingDescLabel.Text");
			saveBtn.Location = new System.Drawing.Point(330, 268);
			saveBtn.Name = "saveBtn";
			saveBtn.Size = new System.Drawing.Size(75, 23);
			saveBtn.TabIndex = 1;
			saveBtn.Text = "Save";
			saveBtn.UseVisualStyleBackColor = true;
			saveBtn.Click += new System.EventHandler(saveBtn_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(417, 303);
			base.Controls.Add(saveBtn);
			base.Controls.Add(groupBox1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "OptionsForm";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Options";
			base.TopMost = true;
			base.Load += new System.EventHandler(OptionsForm_Load);
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)infoIcon).EndInit();
			ResumeLayout(false);
		}
	}
}
