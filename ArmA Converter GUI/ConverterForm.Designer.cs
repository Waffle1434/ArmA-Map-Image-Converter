using System;
using System.Linq;

namespace ArmA_Converter_GUI {
	partial class ConverterForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.inputPathButton = new System.Windows.Forms.Button();
            this.outputPathButton = new System.Windows.Forms.Button();
            this.inputPathBox = new System.Windows.Forms.MaskedTextBox();
            this.outputPathBox = new System.Windows.Forms.MaskedTextBox();
            this.outLabel = new System.Windows.Forms.Label();
            this.inLabel = new System.Windows.Forms.Label();
            this.runButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.bitsCombo = new System.Windows.Forms.ComboBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.outFormatCombo = new System.Windows.Forms.ComboBox();
            this.overlapNumeric = new System.Windows.Forms.NumericUpDown();
            this.toolTabs = new System.Windows.Forms.TabControl();
            this.ImageConvTab = new System.Windows.Forms.TabPage();
            this.imToPAALabel = new System.Windows.Forms.Label();
            this.imToPAAPathBox = new System.Windows.Forms.MaskedTextBox();
            this.formatLabel = new System.Windows.Forms.Label();
            this.imToPAAButton = new System.Windows.Forms.Button();
            this.XYZTab = new System.Windows.Forms.TabPage();
            this.StitchTab = new System.Windows.Forms.TabPage();
            this.overlapLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.overlapNumeric)).BeginInit();
            this.toolTabs.SuspendLayout();
            this.ImageConvTab.SuspendLayout();
            this.XYZTab.SuspendLayout();
            this.StitchTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // inputPathButton
            // 
            this.inputPathButton.Location = new System.Drawing.Point(247, 19);
            this.inputPathButton.Name = "inputPathButton";
            this.inputPathButton.Size = new System.Drawing.Size(75, 23);
            this.inputPathButton.TabIndex = 3;
            this.inputPathButton.Text = "...";
            this.inputPathButton.UseVisualStyleBackColor = true;
            this.inputPathButton.Click += new System.EventHandler(this.inputPathButton_Click);
            // 
            // outputPathButton
            // 
            this.outputPathButton.Location = new System.Drawing.Point(247, 56);
            this.outputPathButton.Name = "outputPathButton";
            this.outputPathButton.Size = new System.Drawing.Size(75, 23);
            this.outputPathButton.TabIndex = 4;
            this.outputPathButton.Text = "...";
            this.outputPathButton.UseVisualStyleBackColor = true;
            this.outputPathButton.Click += new System.EventHandler(this.outputButton_Click);
            // 
            // inputPathBox
            // 
            this.inputPathBox.Location = new System.Drawing.Point(5, 21);
            this.inputPathBox.Name = "inputPathBox";
            this.inputPathBox.Size = new System.Drawing.Size(236, 20);
            this.inputPathBox.TabIndex = 0;
            // 
            // outputPathBox
            // 
            this.outputPathBox.Location = new System.Drawing.Point(5, 59);
            this.outputPathBox.Name = "outputPathBox";
            this.outputPathBox.Size = new System.Drawing.Size(236, 20);
            this.outputPathBox.TabIndex = 1;
            // 
            // outLabel
            // 
            this.outLabel.AutoSize = true;
            this.outLabel.Location = new System.Drawing.Point(6, 43);
            this.outLabel.Name = "outLabel";
            this.outLabel.Size = new System.Drawing.Size(64, 13);
            this.outLabel.TabIndex = 5;
            this.outLabel.Text = "Output Path";
            // 
            // inLabel
            // 
            this.inLabel.AutoSize = true;
            this.inLabel.Location = new System.Drawing.Point(6, 5);
            this.inLabel.Name = "inLabel";
            this.inLabel.Size = new System.Drawing.Size(56, 13);
            this.inLabel.TabIndex = 6;
            this.inLabel.Text = "Input Path";
            // 
            // runButton
            // 
            this.runButton.Location = new System.Drawing.Point(247, 95);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(75, 23);
            this.runButton.TabIndex = 10;
            this.runButton.Text = "Run";
            this.toolTip1.SetToolTip(this.runButton, "Execute the command.");
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.statusLabel.Location = new System.Drawing.Point(160, 100);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.statusLabel.Size = new System.Drawing.Size(81, 13);
            this.statusLabel.TabIndex = 8;
            this.statusLabel.Text = "Status";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // bitsCombo
            // 
            this.bitsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.bitsCombo.FormattingEnabled = true;
            this.bitsCombo.Location = new System.Drawing.Point(6, 102);
            this.bitsCombo.Name = "bitsCombo";
            this.bitsCombo.Size = new System.Drawing.Size(57, 21);
            this.bitsCombo.TabIndex = 2;
            this.toolTip1.SetToolTip(this.bitsCombo, "Bits to represent height values.\r\nThe more bits, the higher the precision, \r\nbut " +
        "more space.");
            this.bitsCombo.SelectedIndexChanged += new System.EventHandler(this.bitsCombo_SelectedIndexChanged);
            // 
            // outFormatCombo
            // 
            this.outFormatCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.outFormatCombo.FormattingEnabled = true;
            this.outFormatCombo.Location = new System.Drawing.Point(6, 102);
            this.outFormatCombo.Name = "outFormatCombo";
            this.outFormatCombo.Size = new System.Drawing.Size(57, 21);
            this.outFormatCombo.TabIndex = 2;
            this.toolTip1.SetToolTip(this.outFormatCombo, "Format of output images.");
            this.outFormatCombo.SelectedIndexChanged += new System.EventHandler(this.outFormatCombo_SelectedIndexChanged);
            // 
            // overlapNumeric
            // 
            this.overlapNumeric.Location = new System.Drawing.Point(6, 102);
            this.overlapNumeric.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.overlapNumeric.Name = "overlapNumeric";
            this.overlapNumeric.Size = new System.Drawing.Size(57, 20);
            this.overlapNumeric.TabIndex = 2;
            this.toolTip1.SetToolTip(this.overlapNumeric, "Overlap of tiles in pixels.\r\nMost ArmA maps seem to be 16px.");
            // 
            // toolTabs
            // 
            this.toolTabs.Controls.Add(this.ImageConvTab);
            this.toolTabs.Controls.Add(this.XYZTab);
            this.toolTabs.Controls.Add(this.StitchTab);
            this.toolTabs.Location = new System.Drawing.Point(12, 12);
            this.toolTabs.Name = "toolTabs";
            this.toolTabs.SelectedIndex = 0;
            this.toolTabs.Size = new System.Drawing.Size(343, 204);
            this.toolTabs.TabIndex = 10;
            // 
            // ImageConvTab
            // 
            this.ImageConvTab.Controls.Add(this.imToPAALabel);
            this.ImageConvTab.Controls.Add(this.imToPAAPathBox);
            this.ImageConvTab.Controls.Add(this.formatLabel);
            this.ImageConvTab.Controls.Add(this.outFormatCombo);
            this.ImageConvTab.Controls.Add(this.imToPAAButton);
            this.ImageConvTab.Location = new System.Drawing.Point(4, 22);
            this.ImageConvTab.Name = "ImageConvTab";
            this.ImageConvTab.Size = new System.Drawing.Size(335, 178);
            this.ImageConvTab.TabIndex = 0;
            this.ImageConvTab.Text = "Convert Images";
            this.ImageConvTab.UseVisualStyleBackColor = true;
            // 
            // imToPAALabel
            // 
            this.imToPAALabel.AutoSize = true;
            this.imToPAALabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.imToPAALabel.Location = new System.Drawing.Point(6, 126);
            this.imToPAALabel.Name = "imToPAALabel";
            this.imToPAALabel.Size = new System.Drawing.Size(115, 13);
            this.imToPAALabel.TabIndex = 14;
            this.imToPAALabel.Text = "ImageToPAA.exe Path";
            // 
            // imToPAAPathBox
            // 
            this.imToPAAPathBox.Location = new System.Drawing.Point(5, 142);
            this.imToPAAPathBox.Name = "imToPAAPathBox";
            this.imToPAAPathBox.Size = new System.Drawing.Size(236, 20);
            this.imToPAAPathBox.TabIndex = 12;
            // 
            // formatLabel
            // 
            this.formatLabel.AutoSize = true;
            this.formatLabel.Location = new System.Drawing.Point(6, 86);
            this.formatLabel.Name = "formatLabel";
            this.formatLabel.Size = new System.Drawing.Size(20, 13);
            this.formatLabel.TabIndex = 11;
            this.formatLabel.Text = "To";
            // 
            // imToPAAButton
            // 
            this.imToPAAButton.Location = new System.Drawing.Point(247, 140);
            this.imToPAAButton.Name = "imToPAAButton";
            this.imToPAAButton.Size = new System.Drawing.Size(75, 23);
            this.imToPAAButton.TabIndex = 13;
            this.imToPAAButton.Text = "...";
            this.imToPAAButton.UseVisualStyleBackColor = true;
            this.imToPAAButton.Click += new System.EventHandler(this.imToPAAButton_Click);
            // 
            // XYZTab
            // 
            this.XYZTab.Controls.Add(this.bitsCombo);
            this.XYZTab.Location = new System.Drawing.Point(4, 22);
            this.XYZTab.Name = "XYZTab";
            this.XYZTab.Size = new System.Drawing.Size(335, 178);
            this.XYZTab.TabIndex = 1;
            this.XYZTab.Text = "Convert XYZ";
            this.XYZTab.UseVisualStyleBackColor = true;
            // 
            // StitchTab
            // 
            this.StitchTab.Controls.Add(this.inLabel);
            this.StitchTab.Controls.Add(this.inputPathBox);
            this.StitchTab.Controls.Add(this.inputPathButton);
            this.StitchTab.Controls.Add(this.outputPathBox);
            this.StitchTab.Controls.Add(this.outLabel);
            this.StitchTab.Controls.Add(this.outputPathButton);
            this.StitchTab.Controls.Add(this.statusLabel);
            this.StitchTab.Controls.Add(this.runButton);
            this.StitchTab.Controls.Add(this.overlapLabel);
            this.StitchTab.Controls.Add(this.overlapNumeric);
            this.StitchTab.Location = new System.Drawing.Point(4, 22);
            this.StitchTab.Name = "StitchTab";
            this.StitchTab.Size = new System.Drawing.Size(335, 178);
            this.StitchTab.TabIndex = 2;
            this.StitchTab.Text = "Stitch Tiles";
            this.StitchTab.UseVisualStyleBackColor = true;
            // 
            // overlapLabel
            // 
            this.overlapLabel.AutoSize = true;
            this.overlapLabel.Location = new System.Drawing.Point(6, 86);
            this.overlapLabel.Name = "overlapLabel";
            this.overlapLabel.Size = new System.Drawing.Size(44, 13);
            this.overlapLabel.TabIndex = 11;
            this.overlapLabel.Text = "Overlap";
            // 
            // Converter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 227);
            this.Controls.Add(this.toolTabs);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Converter";
            this.Text = "Converter";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.overlapNumeric)).EndInit();
            this.toolTabs.ResumeLayout(false);
            this.ImageConvTab.ResumeLayout(false);
            this.ImageConvTab.PerformLayout();
            this.XYZTab.ResumeLayout(false);
            this.StitchTab.ResumeLayout(false);
            this.StitchTab.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button inputPathButton;
		private System.Windows.Forms.Button outputPathButton;
		private System.Windows.Forms.MaskedTextBox inputPathBox;
		private System.Windows.Forms.MaskedTextBox outputPathBox;
		private System.Windows.Forms.Label outLabel;
		private System.Windows.Forms.Label inLabel;
		private System.Windows.Forms.Button runButton;
		private System.Windows.Forms.Label statusLabel;
		private System.Windows.Forms.ComboBox bitsCombo;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.TabControl toolTabs;
		private System.Windows.Forms.TabPage ImageConvTab;
		private System.Windows.Forms.TabPage XYZTab;
		private System.Windows.Forms.TabPage StitchTab;
		private System.Windows.Forms.ComboBox outFormatCombo;
		private System.Windows.Forms.NumericUpDown overlapNumeric;
		private System.Windows.Forms.Label overlapLabel;
		private System.Windows.Forms.Label formatLabel;
		private System.Windows.Forms.Label imToPAALabel;
		private System.Windows.Forms.MaskedTextBox imToPAAPathBox;
		private System.Windows.Forms.Button imToPAAButton;
	}
}

