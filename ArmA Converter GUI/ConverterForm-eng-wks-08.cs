using ArmA_Converter;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ArmA_Converter_GUI {
    public partial class ConverterForm : Form {
        public ConverterForm() {
            InitializeComponent();

            sharedControls = new Control[] { inLabel, inputPathBox, inputPathButton, outLabel, outputPathBox, outputPathButton, statusLabel, runButton };
            toolTabs.SelectedIndexChanged += TabChanged;
            toolTabs.ShowToolTips = true;

            /*foreach (ConverterMode m in (Enum.GetValues(typeof(ConverterMode)).Cast<ConverterMode>()))
				converterModeCombo.Items.Add(m.GetDescription());*/
            //Mode = ConverterMode.ConvertImage;

            #region ImageConverter
            ImageConvTab.ToolTipText = "Convert ArmA image formats to more usable formats.";
            foreach (ImgConvOutFormats e in (Enum.GetValues(typeof(ImgConvOutFormats)).Cast<ImgConvOutFormats>()))
                outFormatCombo.Items.Add(e);
            OutFormat = ImgConvOutFormats.PNG;
            imToPAAPathBox.TextChanged += ImToPAAPathBox_TextChanged;
            #endregion

            #region XYZ
            XYZTab.ToolTipText = "Convert XYZ heightmap to an image format.";
            foreach (ZBits m in (Enum.GetValues(typeof(ZBits)).Cast<ZBits>()))
                bitsCombo.Items.Add(m.GetDescription());
            ZbitMode = ZBits.Bit16;
            #endregion

            #region Stitch
            StitchTab.ToolTipText = "Combine image tiles into master image.";
            overlapNumeric.Value = 16;
            #endregion

            SetModeTab();
            SetReady();

            string path = LibCommon.Helpers.GameHelpers.GetArma3Path();
            if (path != "") {
                path = Path.GetFullPath(path + @"\\..\") + @"ArmA 3 Tools\ImageToPAA\ImageToPAA.exe";
                if (File.Exists(path)) {
                    imToPAAPathBox.Text = path;//TODO set text and update cursor method
                    TextBoxSelectEnd(imToPAAPathBox);
                }
            }
        }
        void Form1_Load(object sender, EventArgs e) {

        }

        Control[] sharedControls;

        void SetModeTab() {
            int i = toolTabs.SelectedIndex;
            Mode = (ConverterMode)i;

            foreach (Control c in sharedControls) {
                c.Parent = toolTabs.TabPages[i];
            }
        }
        void TabChanged(object sender, EventArgs e) => SetModeTab();

        void SetReady() {
            statusLabel.Text = "Ready";
            Refresh();
        }
        void SetProcessing() {
            statusLabel.Text = "Processing";
            Refresh();
        }

        #region Enums
        enum ConverterMode {
            [Description("Convert Image")] ConvertImage,
            [Description("XYZ To Image")] ConvertXYZ,
            [Description("Stitch Tiles")] Stitch
        };
        enum ZBits {
            //[Description("5 Bits")] Bit5,
            [Description("8 Bits")] Bit8,
            [Description("16 Bits")] Bit16
        }
        enum ImgConvOutFormats { PNG, TGA, PAA, PAC };
        #endregion

        #region Properties
        ConverterMode mode = (ConverterMode)(-1);
        ConverterMode Mode {
            get => mode;
            set {
                if (mode != value) {
                    mode = value;

                    //converterModeCombo.SelectedIndex = (int)Mode;
                    ClearPaths();

                    bitsCombo.Enabled = mode == ConverterMode.ConvertXYZ;

                    switch (mode) {
                        case ConverterMode.ConvertImage:
                            inLabel.Text = "Input Images";
                            outLabel.Text = "Output Path";
                            break;
                        case ConverterMode.ConvertXYZ:
                            inLabel.Text = "Input XYZ";
                            outLabel.Text = "Output Image";
                            break;
                        case ConverterMode.Stitch:
                            inLabel.Text = "Input Tiles";
                            outLabel.Text = "Output Path";
                            break;
                    }
                }
            }
        }

        ZBits zbitMode = (ZBits)(-1);
        ZBits ZbitMode {
            get => zbitMode;
            set {
                if (zbitMode != value) {
                    zbitMode = value;
                    bitsCombo.SelectedIndex = (int)zbitMode;
                }
            }
        }

        ImgConvOutFormats outFormat = (ImgConvOutFormats)(-1);
        ImgConvOutFormats OutFormat {
            get => outFormat;
            set {
                if (outFormat != value) {
                    outFormat = value;
                    outFormatCombo.SelectedIndex = (int)outFormat;
                }
            }
        }
        #endregion

        #region Open Dialog Buttons
        static CommonFileDialogFilter RVImages = new CommonFileDialogFilter("Images", ".png;.jpg;.bmp;.tga;.paa"),
            Heightmap = new CommonFileDialogFilter("Heightmap", ".xyz"),
            Images = new CommonFileDialogFilter("Images", ".png;.jpg;.bmp;.tga"),
            EXE = new CommonFileDialogFilter("Executable", ".exe");

        string[] inPaths, outPaths;
        void ClearPaths() {
            inPaths = outPaths = null;
            inputPathBox.Text = outputPathBox.Text = "";
        }

        void OpenFolderDialog(bool isInput) {
            #region Init Vars
            bool save = false;
            switch (Mode) {
                case ConverterMode.ConvertXYZ:
                    if (!isInput) save = true;
                    break;
            }

            CommonFileDialog fileDialog;
            CommonOpenFileDialog openFolder = null;
            CommonSaveFileDialog saveFile = null;
            #endregion

            #region Setup Dialog
            if (save) {
                fileDialog = saveFile = new CommonSaveFileDialog();
            }
            else {
                fileDialog = openFolder = new CommonOpenFileDialog();
                if (!isInput) openFolder.Multiselect = false;
                openFolder.AllowNonFileSystemItems = true;
            }

            switch (Mode) {
                case ConverterMode.ConvertImage:
                    if (isInput) {
                        openFolder.Title = "Select Images To Convert";
                        openFolder.Multiselect = true;
                        openFolder.IsFolderPicker = false;
                        openFolder.Filters.Add(RVImages);
                    }
                    else {
                        openFolder.Title = "Select Output Folder";
                        openFolder.IsFolderPicker = true;
                    }
                    break;
                case ConverterMode.ConvertXYZ:
                    if (isInput) {
                        openFolder.IsFolderPicker = false;
                        openFolder.Multiselect = false;
                        openFolder.Title = "Select .XYZ File";
                        openFolder.Filters.Add(Heightmap);
                    }
                    else {
                        saveFile.Title = "Select Output Image File";
                        saveFile.Filters.Add(Images);
                    }
                    break;
                case ConverterMode.Stitch:
                    if (isInput) {
                        openFolder.Title = "Select Image Tiles";
                        openFolder.Multiselect = true;
                        openFolder.IsFolderPicker = false;
                        openFolder.Filters.Add(Images);
                    }
                    else {
                        openFolder.Title = "Select Output Folder";
                        openFolder.IsFolderPicker = true;
                    }
                    break;
            }
            #endregion

            CommonFileDialogResult res = fileDialog.ShowDialog();

            #region Handle Result
            switch (res) {
                case CommonFileDialogResult.Cancel:
                    ClearPaths();
                    break;
                case CommonFileDialogResult.None:
                    MessageBox.Show("Nothing Selected");
                    ClearPaths();
                    break;
                case CommonFileDialogResult.Ok:
                    int c = 1;
                    if (!save) {
                        if (isInput) {
                            inPaths = openFolder.FileNames.ToArray();
                            c = inPaths.Length;
                        }
                        else {
                            outPaths = openFolder.FileNames.ToArray();
                            c = outPaths.Length;
                        }
                    }
                    else {
                        outPaths = new string[] { fileDialog.FileName };
                    }

                    MaskedTextBox box = isInput ? inputPathBox : outputPathBox;
                    box.Text = c == 1 ? fileDialog.FileName : $"\t{c} Files";
                    TextBoxSelectEnd(box);
                    break;
            }
            #endregion
        }

        void inputPathButton_Click(object sender, EventArgs e) => OpenFolderDialog(true);
        void outputButton_Click(object sender, EventArgs e) => OpenFolderDialog(false);
        void imToPAAButton_Click(object sender, EventArgs e) {
            CommonOpenFileDialog selFile = new CommonOpenFileDialog("Select ImageToPAA.exe");
            selFile.Filters.Add(EXE);

            CommonFileDialogResult res = selFile.ShowDialog();
            switch (res) {
                case CommonFileDialogResult.Cancel:
                    break;
                case CommonFileDialogResult.None:
                    MessageBox.Show("Nothing Selected");
                    break;
                case CommonFileDialogResult.Ok:
                    imToPAAPathBox.Text = selFile.FileName;
                    TextBoxSelectEnd(imToPAAPathBox);
                    break;
            }
        }
        #endregion

        void TextBoxSelectEnd(MaskedTextBox box) {
            box.SelectionStart = box.Text.Length;
            box.SelectionLength = 0;
        }

        ProgressPopup prog;

        void runButton_Click(object sender, EventArgs e) {
            prog = new ProgressPopup();

            prog.bw.DoWork += Bw_DoWork;

            prog.ShowDialog();
            SetReady();
        }

        void Bw_DoWork(object sender, DoWorkEventArgs e) {
            //prog.bw.ReportProgress(25);
            switch (Mode) {
                case ConverterMode.ConvertImage:
                    //SetProcessing();
                    Converter.ConvertImages(inPaths, outputPathBox.Text, outFormat: OutFormat.ToString().ToLower());
                    break;
                case ConverterMode.ConvertXYZ:
                    //SetProcessing();

                    PixelFormat format;
                    switch (ZbitMode) {
                        //case ZBits.Bit5: format = PixelFormat.Format16bppRgb555; break;
                        case ZBits.Bit8: format = PixelFormat.Format24bppRgb; break;
                        case ZBits.Bit16: format = PixelFormat.Format48bppRgb; break;
                        default: goto case ZBits.Bit8;
                    }

                    Converter.ProgressChanged += Converter_ProgressChanged;

                    Converter.ConvertXYZ(inputPathBox.Text, outputPathBox.Text, format);

                    Converter.ProgressChanged -= Converter_ProgressChanged;
                    break;
                case ConverterMode.Stitch:
                    //SetProcessing();
                    if (inPaths == null) Converter.StitchMapTiles(inputPathBox.Text, outputPathBox.Text);
                    else Converter.StitchMapTiles(inPaths, outputPathBox.Text, overlap: (int)overlapNumeric.Value);
                    break;
            }
        }

        void Converter_ProgressChanged(int obj) => prog.bw.ReportProgress(obj);

        //void converterModeCombo_SelectedIndexChanged(object sender, EventArgs e) => Mode = (ConverterMode)converterModeCombo.SelectedIndex;
        void outFormatCombo_SelectedIndexChanged(object sender, EventArgs e) {
            OutFormat = (ImgConvOutFormats)outFormatCombo.SelectedIndex;
        }
        void bitsCombo_SelectedIndexChanged(object sender, EventArgs e) => ZbitMode = (ZBits)bitsCombo.SelectedIndex;

        void HandlePathTextBoxChange(MaskedTextBox textBox) {
            if (textBox.Text != "" && !textBox.Text.StartsWith("\t")) ClearPaths();
        }

        void InputPathBox_TextChanged(object sender, EventArgs e) => HandlePathTextBoxChange((MaskedTextBox)sender);
        void OutputPathBox_TextChanged(object sender, EventArgs e) => HandlePathTextBoxChange((MaskedTextBox)sender);
        void ImToPAAPathBox_TextChanged(object sender, EventArgs e) => ArmA_Converter.Converter.ImgToPaaPath = imToPAAPathBox.Text;
    }


    public static class Util {
        public static string GetDescription(this Enum value) {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null) {
                FieldInfo field = type.GetField(name);
                if (field != null) {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null) {
                        return attr.Description;
                    }
                }
            }
            return null;
        }
    }
}
