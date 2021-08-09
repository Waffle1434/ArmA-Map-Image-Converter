using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using IMG = System.Drawing.Imaging;
using UTIL = CommandLineUtils.Utils;

namespace CommandLineUtils {
    public static class Utils {
        public static void KeyToContinue() {
            Console.Write("Press any key to continue.");
            Console.ReadKey();
        }

        //public static string GetFullFileName(string path) => path.Substring(path.LastIndexOf('\\') + 1);
        public static string GetExtension(string path) => path.Substring(path.LastIndexOf('.') + 1);
        public static string SetExtension(string file, string ext) {
            int i;
            if ((i = file.LastIndexOf('.')) != -1) return Path.ChangeExtension(file, ext);
            else return file;
        }
    }
}

namespace ArmA_Converter {
    public class Converter {
        public static event Action<int> ProgressChanged;
        static int progress = 0;
        public static int Progress {
            get => progress;
            set {
                if (progress != value) {
                    progress = value;
                    ProgressChanged?.Invoke(progress);
                }
            }
        }

        #region PAA Conversion
        #region Util
        public static string ImgToPaaPath = "";
        public static string[] inExts = new string[] { "JPG", "TGA", "PNG", "PAA", "PAC" }, outExts = new string[] { "TGA", "PNG", "PAA", "PAC" };

        static void PrintFileConversion(string inFile, string outFile, bool tab = true) => Console.WriteLine($"{(tab ? "\t" : "")}{inFile}->{outFile}");

        static bool ValidateExtension(string ext, string[] exts, string type) {
            ext = ext.ToUpper();
            if (!exts.Contains(ext)) {
                Console.WriteLine($"{ext} {type} not supported, supported extensions: {string.Join(", ", exts)}");
                return false;
            }
            return true;
        }
        static bool ValidateExtensions(string inFormat, string outFormat) {
            if (!ValidateExtension(inFormat, inExts, "input")) return false;
            if (!ValidateExtension(outFormat, inExts, "output")) return false;
            return true;
        }
        #endregion

        public static bool ConvertImage(string inPath, string outPath, bool redirectOutput = false) {
            try {
                /*Thread th = new Thread(new ThreadStart(delegate {
                    
                }));
                th.Start();*/

                using (Process proc = new Process()) {
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.FileName = ImgToPaaPath;
                    proc.StartInfo.Arguments = $"\"{inPath}\" \"{outPath}\"";
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.RedirectStandardOutput = redirectOutput;
                    proc.Start();

                    if (redirectOutput) {
                        while (!proc.StandardOutput.EndOfStream)
                            Console.WriteLine(proc.StandardOutput.ReadLine());
                    }
                }

                return true;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public static void ConvertImages(string inPath, string outPath, string inFormat = "paa", string outFormat = "png") {
            Console.WriteLine($"Input Image Path: {inPath}\nOutput Image Path: {outPath}\n");

            if (Path.HasExtension(inPath)) {
                inFormat = UTIL.GetExtension(inPath);
                outFormat = UTIL.GetExtension(outPath);
                if (!ValidateExtensions(inFormat, outFormat)) return;

                if (ConvertImage(inPath, outPath)) {
                    string name = Path.GetFileName(inPath);
                    PrintFileConversion(name, UTIL.SetExtension(name, outFormat));
                }
            }
            else {
                if (!ValidateExtensions(inFormat, outFormat)) return;

                try {
                    List<string> inPaths = new List<string>();
                    foreach (string file in Directory.GetFiles(inPath, $"*.{inFormat}"))
                        inPaths.Add(file);
                    ConvertImages(inPaths.ToArray(), outPath, inFormat, outFormat);
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                    return;
                }
            }
        }
        public static void ConvertImages(string[] inPaths, string outPath, string inFormat = "paa", string outFormat = "png") {
            Console.WriteLine("Files Processed:");
            try {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                foreach (string file in inPaths) {
                    string name = Path.GetFileName(file);
                    string outName = UTIL.SetExtension(name, outFormat);

                    if (ConvertImage(file, $"{outPath}\\{outName}"))
                        PrintFileConversion(name, outName);
                }
                Console.WriteLine($"{sw.ElapsedMilliseconds * .001f} Seconds");
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return;
            }
            Console.WriteLine();
        }
        #endregion

        #region Map Stitch
        #region Util
        enum MapType { Mask, Satellite, Normal }

        static void StitchTiles(Dictionary<Point, Bitmap> tiles, int w, int h, int dimX, int dimY, string path, System.Drawing.Color background, int overlapX = 0, int overlapY = 0, int tileOffX = 0, int tileOffY = 0) {
            int w_tot = w * dimX - (overlapX * 2 * dimX), h_tot = h * dimY - (overlapY * 2 * dimY);

            //8192 16384 32768
            if (w_tot > 32768) w_tot = 32768 - 2 * overlapX;
            if (h_tot > 32768) h_tot = 32768 - 2 * overlapY;

            Console.WriteLine($"{Path.GetFileName(path)}: {dimX}x{dimY} Tiles: {w_tot}x{h_tot}");

            using (Bitmap img = new Bitmap(w_tot, h_tot, IMG.PixelFormat.Format24bppRgb)) {
                using (Graphics g = Graphics.FromImage(img)) {
                    g.Clear(background);//System.Drawing.Color.Black
                    g.CompositingMode = CompositingMode.SourceCopy;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.SmoothingMode = SmoothingMode.HighQuality;

                    int w2 = w - 2 * overlapX;
                    int h2 = h - 2 * overlapY;
                    Rectangle destRect = new Rectangle(0, 0, w2, h2);
                    Rectangle srcFixedRect = new Rectangle(overlapX, overlapY, w2, h2);

                    foreach (KeyValuePair<Point, Bitmap> v in tiles) {
                        int x = v.Key.X - tileOffX, y = v.Key.Y - tileOffY;
                        destRect.X = x * w2;
                        destRect.Y = y * h2;
                        Image tile = v.Value;
                        Rectangle srcRect = srcFixedRect;
                        if (tile.Width != w || tile.Height != h)
                            srcRect = new Rectangle(0, 0, tile.Width, tile.Height);
                        g.DrawImage(tile, destRect, srcRect, GraphicsUnit.Pixel);
                    }
                    DisposeImages(tiles);
                }

                img.Save(path);
            }

            DisposeImages(tiles);
        }

        static void DisposeImages(Dictionary<Point, Bitmap> images) {
            foreach (KeyValuePair<Point, Bitmap> v in images) v.Value.Dispose();
            images.Clear();
        }

        static string GetMapName(MapType type) {
            switch (type) {
                case MapType.Mask: return "M";
                case MapType.Satellite: return "S";
                case MapType.Normal: return "N";
                default: return "Stitched";
            }
        }
        #endregion

        public static void StitchMapTiles(string inPath, string outPath, string inFormat = "png", string outFormat = "jpg", int overlap = 0) {
            try {
                string[] files = Directory.GetFiles(inPath, $"*.{inFormat}");
                if (files.Length == 0) {
                    Console.WriteLine($"No files of {inFormat.ToUpper()} format found.");
                    return;
                }

                StitchMapTiles(files, outPath, inFormat, outFormat);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return;
            }
        }
        public static void StitchMapTiles(string[] inPaths, string outPath, string inFormat = "png", string outFormat = "jpg", int overlap = 0) {
            MapType currentMap = (MapType)(-1);
            Dictionary<Point, Bitmap> satelliteTiles = new Dictionary<Point, Bitmap>(),
                maskTiles = new Dictionary<Point, Bitmap>(),
                normalTiles = new Dictionary<Point, Bitmap>();
            int w = 0, h = 0;
            int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;

            string maskPrefix = "M_", satellitePrefix = "S_", normalPrefix = "N_";

            Console.WriteLine("Files Processed:");
            foreach (string file in inPaths) {
                string name = Path.GetFileName(file);
                if (name.ToUpper().StartsWith(maskPrefix)) currentMap = MapType.Mask;
                else if (name.ToUpper().StartsWith(satellitePrefix)) currentMap = MapType.Satellite;
                else if (name.ToUpper().StartsWith(normalPrefix)) currentMap = MapType.Normal;
                else continue;// Invalid File

                Bitmap tile = new Bitmap(file);
                int _w = tile.Width, _h = tile.Height;
                if (_w > w) w = _w;
                if (_h > h) h = _h;

                Point p;
                {
                    int i1 = name.IndexOf('_');
                    string[] coords = name.Substring(i1 + 1, name.LastIndexOf('_') - i1).Split('_');
                    p = new Point(int.Parse(coords[0]), int.Parse(coords[1]));
                    if (p.X < minX) minX = p.X;
                    else if (p.X > maxX) maxX = p.X;
                    if (p.Y < minY) minY = p.Y;
                    else if (p.Y > maxY) maxY = p.Y;
                }

                switch (currentMap) {
                    case MapType.Mask: maskTiles.Add(p, tile); break;
                    case MapType.Satellite: satelliteTiles.Add(p, tile); break;
                    case MapType.Normal: normalTiles.Add(p, tile); break;
                }

                Console.WriteLine($"{name}: {_w}x{_h}");
            }

            int overlapX, overlapY;
            overlapX = overlapY = overlap;

            int dimX = (maxX - minX) + 1, dimY = (maxY - minY) + 1;
            System.Drawing.Color col = System.Drawing.Color.Black;
            //if (true) col = System.Drawing.Color.FromArgb(72, 72, 72);

            if (satelliteTiles.Count > 0)
                StitchTiles(satelliteTiles, w, h, dimX, dimY, $"{outPath}\\{GetMapName(MapType.Satellite)}.{outFormat}", col, overlapX, overlapY, minX, minY);
            if (maskTiles.Count > 0)
                StitchTiles(maskTiles, w, h, dimX, dimY, $"{outPath}\\{GetMapName(MapType.Mask)}.{outFormat}", System.Drawing.Color.Black, overlapX, overlapY, minX, minY);
            if (normalTiles.Count > 0)
                StitchTiles(normalTiles, w, h, dimX, dimY, $"{outPath}\\{GetMapName(MapType.Normal)}.{outFormat}", System.Drawing.Color.Black, overlapX, overlapY, minX, minY);

            //Mash rvmat color order: BLACK_RED_GREEN_BLUE

            Console.WriteLine();
        }
        #endregion

        #region XYZ Conversion
        #region Util
        public struct XYZ {
            public double x, y, z;

            public XYZ(double x, double y, double z) {
                this.x = x;
                this.y = y;
                this.z = z;
            }
            public XYZ(double a) {
                x = y = z = a;
            }

            public double this[int i] {
                get {
                    switch (i) {
                        case 0: return x;
                        case 1: return y;
                        case 2: return z;
                        default: throw new IndexOutOfRangeException();
                    }
                }
                set {
                    switch (i) {
                        case 0: x = value; break;
                        case 1: y = value; break;
                        case 2: z = value; break;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public static XYZ operator -(XYZ a, XYZ b) => new XYZ(a.x - b.x, a.y - b.y, a.z - b.z);

            /*public static implicit operator XYZ(string str) {

            }*/
            public static bool FromString(string str, out XYZ coord, bool zOnly = false) {
                coord = new XYZ();
                if ((str = str.Trim()) != "") {
                    int i = 0;
                    foreach (string s in str.Split())
                        if (s != "") coord[i++] = double.Parse(s);
                    if (i != 3 && i != 1) return false;
                    else if (!zOnly && i == 1) zOnly = true;

                    if (zOnly) {
                        coord.z = coord.x;
                        coord.x = coord.y = 0.0;
                    }

                    return true;
                }
                return false;
            }

            public override string ToString() {
                return $"{x}, {y}, {z}";
            }
        }

        static double GetHeight(List<XYZ> coords, int x, int y, int h) => coords[x * h + y].z;

        static void SaveBmp(Bitmap bmp, string path) {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

            BitmapData bitmapData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);
            System.Windows.Media.PixelFormat format = ConvertBmpPixelFormat(bmp.PixelFormat);
            //BitmapPalette p = new BitmapPalette()
            BitmapSource source = BitmapSource.Create(bmp.Width,
                                                      bmp.Height,
                                                      bmp.HorizontalResolution,
                                                      bmp.VerticalResolution,
                                                      format,
                                                      null,
                                                      bitmapData.Scan0,
                                                      bitmapData.Stride * bmp.Height,
                                                      bitmapData.Stride);

            bmp.UnlockBits(bitmapData);


            FileStream stream = new FileStream(path, FileMode.Create);

            TiffBitmapEncoder encoder = new TiffBitmapEncoder();

            encoder.Compression = TiffCompressOption.Zip;
            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(stream);

            stream.Close();
        }

        static System.Windows.Media.PixelFormat ConvertBmpPixelFormat(IMG.PixelFormat pixelformat) {
            switch (pixelformat) {
                case IMG.PixelFormat.Format64bppArgb: return PixelFormats.Rgba64;
                case IMG.PixelFormat.Format48bppRgb: return PixelFormats.Rgb48;
                case IMG.PixelFormat.Format32bppArgb: return PixelFormats.Bgr32;
                case IMG.PixelFormat.Format24bppRgb: return PixelFormats.Rgb24;
                case IMG.PixelFormat.Format16bppRgb555: return PixelFormats.Bgr555;
                case IMG.PixelFormat.Format16bppGrayScale: return PixelFormats.Gray16;
                case IMG.PixelFormat.Format8bppIndexed: return PixelFormats.Gray8;
                default: return PixelFormats.Default;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        struct ShortBitConverter {
            [FieldOffset(0)] short ShortValue;
            [FieldOffset(0)] ushort UShortValue;

            public static short Convert(ushort source) {
                ShortBitConverter converter = new ShortBitConverter();
                converter.UShortValue = source;
                return converter.ShortValue;
            }
        }
        #endregion

        public static void ConvertXYZ(string inFile, string outFile, IMG.PixelFormat outFormat) {
            if (UTIL.GetExtension(inFile).ToLower() != "xyz") {
                Console.WriteLine("File type is not .xyz");
                return;
            }

            #region Parse XYZ
            List<XYZ> coords = new List<XYZ>();
            XYZ min = new XYZ(double.MaxValue), max = new XYZ(double.MinValue);
            bool zOnly = false;
            string line;
            try {
                using (StreamReader fs = new StreamReader(inFile)) {
                    int l = 0;
                    while ((line = fs.ReadLine()) != null) {
                        l++;

                        XYZ coord;
                        if (XYZ.FromString(line, out coord, zOnly)) {
                            if (!zOnly) {
                                if (coord.x < min.x) min.x = coord.x;
                                if (coord.y < min.y) min.y = coord.y;
                                if (coord.z < min.z) min.z = coord.z;

                                if (coord.x > max.x) max.x = coord.x;
                                if (coord.y > max.y) max.y = coord.y;
                                if (coord.z > max.z) max.z = coord.z;
                            }

                            coords.Add(coord);
                        }
                        else {
                            Console.WriteLine($"Bad Line: {l}");
                        }
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return;
            }

            int w, h;
            w = h = (int)Math.Sqrt(coords.Count);
            XYZ dim = max - min;

            bool trimPositions = true;
            if (trimPositions) for (int i = 0; i < coords.Count; i++) coords[i] -= min;
            #endregion

            #region Image Generation
            int bits;
            switch (outFormat) {
                case IMG.PixelFormat.Format16bppRgb555:
                case IMG.PixelFormat.Format16bppArgb1555:
                    bits = 5; break;
                case IMG.PixelFormat.Format24bppRgb:
                case IMG.PixelFormat.Format32bppRgb:
                case IMG.PixelFormat.Format32bppArgb:
                    bits = 8; break;
                case IMG.PixelFormat.Format48bppRgb:
                case IMG.PixelFormat.Format64bppArgb:
                    bits = 16; break;
                default: goto case IMG.PixelFormat.Format24bppRgb;
            }

            using (Bitmap heightmap = new Bitmap(w, h, outFormat)) {
                BitmapData bmpData = heightmap.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, outFormat);
                IntPtr ptr = bmpData.Scan0;
                int valCount = w * h * 3;
                double hCoef = (Math.Pow(2, bits) - 1) / dim.z;

                #region 8 Bit
                if (bits == 8) {
                    byte[] pixVals = new byte[valCount];

                    int i = 0;
                    for (int y = 0; y < h; y++) {
                        for (int x = 0; x < w; x++) {
                            byte z = (byte)Math.Round(GetHeight(coords, x, h - 1 - y, h) * hCoef);
                            pixVals[i++] = pixVals[i++] = pixVals[i++] = z;
                        }
                    }

                    Marshal.Copy(pixVals, 0, ptr, valCount);
                }
                #endregion
                #region 16 Bit
                else if (bits == 16) {
                    short[] pixVals = new short[valCount];

                    int i = 0;
                    for (int y = 0; y < h; y++) {
                        for (int x = 0; x < w; x++) {
                            // get pixel index based on current coord
                            ushort z = (ushort)Math.Round(GetHeight(coords, x, h - 1 - y, h) * hCoef);
                            pixVals[i++] = pixVals[i++] = pixVals[i++] = ShortBitConverter.Convert(z);
                            if (i % 384 == 0) Progress = (int)(i * 100f / valCount);
                        }
                    }

                    Marshal.Copy(pixVals, 0, ptr, valCount);
                }
                #endregion

                heightmap.UnlockBits(bmpData);
                SaveBmp(heightmap, outFile);

                #region OLD gen
                /*double hCoef = (Math.Pow(2, bits) - 1) / dim.z;
				for (int x = 0; x < w; x++) {
					for (int y = 0; y < h; y++) {
						int z = (int)Math.Round(coords[x * h + (h - 1 - y)].z * hCoef);
						heightmap.SetPixel(x, y, System.Drawing.Color.FromArgb(z, z, z));
					}
				}
				heightmap.Save(outFile);*/
                #endregion

                Console.WriteLine($"{bits} Bit Format: {Math.Pow(2, bits)} Heigh values");
                PrintFileConversion(Path.GetFileName(inFile), Path.GetFileName(outFile), false);
            }

            bool imgInfo = true;
            if (imgInfo) {
                using (StreamWriter sw = new StreamWriter(Path.ChangeExtension(outFile, "txt"))) {
                    sw.WriteLine($"Resolution: {w}x{h}");
                    sw.WriteLine($"Dimensions: {dim}");
                    sw.WriteLine($"Sea Level: {Math.Abs(min.z) / dim.z}%");
                }
            }
            #endregion
        }
        #endregion

        #region Parse CFG
        public static void ParseCFGFile(string cfgFile) => ParseCFG(File.ReadAllText(cfgFile));

        enum ParseState { BuildWord, Ignore, CommentSingle, CommentMulti, Class, Field, Value }
        //comment states: binary |
        static void ClearWord(ref string word) => word = "";
        public static void ParseCFG(string cfg) {
            ParseState state = ParseState.BuildWord;
            string word = "";

            bool addChar = true;

            foreach (char c in cfg) {
                if (state != ParseState.CommentSingle && state != ParseState.CommentMulti) {
                    switch (c) {
                        case ' ':
                        case ';':
                        case '\r':
                        case '\n':
                            addChar = false;
                            break;
                    }

                    if (addChar) {
                        word += c;

                        switch (word) {
                            case "//":// Comment Start
                                state = ParseState.CommentSingle;
                                ClearWord(ref word);
                                break;
                            case "/*":// Comment Start
                                state = ParseState.CommentMulti;
                                ClearWord(ref word);
                                break;
                        }
                    }
                    else addChar = true;
                }
                else {
                    switch (c) {
                        case '\r':
                        case '\n':// Comment End
                            state = ParseState.BuildWord;
                            ClearWord(ref word);
                            break;
                    }

                    if (addChar) {
                        word += c;
                        switch (word) {
                            case "*/":
                                if (state == ParseState.CommentMulti) {
                                    state = ParseState.BuildWord;
                                    ClearWord(ref word);
                                }
                                break;
                        }
                    }
                    else addChar = true;
                }
            }
        }
        #endregion
    }
}
