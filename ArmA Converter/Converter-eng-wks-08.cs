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
        enum MapType { Mask, Satellite }

        static void StitchTiles(List<Bitmap> tiles, int w, int h, string path, int overlapX = 0, int overlapY = 0) {
            int dim;
            if (((float)Math.Sqrt(tiles.Count)) % 1 != 0) {
                Console.WriteLine($"Tile count: {tiles.Count} not square!");
                return;
            }
            else dim = (int)Math.Sqrt(tiles.Count);

            int dec = 2 * dim;
            int w_tot = w * dim - (overlapX * dec), h_tot = h * dim - (overlapY * dec);
            Console.WriteLine($"{Path.GetFileName(path)}: {dim}x{dim} Tiles: {w_tot}x{h_tot}");

            using (Bitmap img = new Bitmap(w_tot, h_tot)) {
                using (Graphics g = Graphics.FromImage(img)) {
                    g.Clear(System.Drawing.Color.Black);
                    g.CompositingMode = CompositingMode.SourceCopy;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.SmoothingMode = SmoothingMode.HighQuality;

                    int w2 = w - 2 * overlapX;
                    int h2 = h - 2 * overlapY;
                    Rectangle destRect = new Rectangle(0, 0, w2, h2);
                    Rectangle srcFixedRect = new Rectangle(overlapX, overlapY, w2, h2);

                    for (int x = 0; x < dim; x++) {
                        for (int y = 0; y < dim; y++) {
                            destRect.X = x * w2;
                            destRect.Y = y * h2;
                            Image tile = tiles[y + (x * dim)];
                            Rectangle srcRect = srcFixedRect;
                            //if (tile.Width < overlapX * 2 || tile.Height < overlapY * 2)
                            //srcRect = new Rectangle(0, 0, tile.Width, tile.Height);
                            if (tile.Width != w || tile.Height != h)
                                srcRect = new Rectangle(0, 0, tile.Width, tile.Height);
                            g.DrawImage(tile, destRect, srcRect, GraphicsUnit.Pixel);
                        }
                    }
                    //g.DrawImage(tiles[y + (x * dim)], x * w, y * h, w, h);
                }

                img.Save(path);
            }

            DisposeImages(tiles);
        }

        static void DisposeImages(List<Bitmap> images) {
            for (int i = 0; i < images.Count; i++) images[i].Dispose();
            images.Clear();
        }

        static string GetMapName(MapType type) {
            switch (type) {
                case MapType.Mask: return "M";
                case MapType.Satellite: return "S";
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
            List<Bitmap> SatelliteTiles = new List<Bitmap>();
            List<Bitmap> MaskTiles = new List<Bitmap>();
            int w = 0, h = 0;

            string maskPrefix = "M_", satellitePrefix = "S_";

            Console.WriteLine("Files Processed:");
            foreach (string file in inPaths) {
                string name = Path.GetFileName(file);
                if (name.ToUpper().StartsWith(maskPrefix)) {
                    if (currentMap != MapType.Mask) currentMap = MapType.Mask;
                }
                else if (name.ToUpper().StartsWith(satellitePrefix)) {
                    if (currentMap != MapType.Satellite) currentMap = MapType.Satellite;
                }
                else continue;// Invalid File

                Bitmap tile = new Bitmap(file);
                int _w = tile.Width, _h = tile.Height;
                if (_w > w) w = _w;
                if (_h > h) h = _h;

                switch (currentMap) {
                    case MapType.Mask: MaskTiles.Add(tile); break;
                    case MapType.Satellite: SatelliteTiles.Add(tile); break;
                }

                Console.WriteLine($"{name}: {_w}x{_h}");
            }

            int overlapX, overlapY;
            overlapX = overlapY = overlap;

            if (SatelliteTiles.Count > 0)
                StitchTiles(SatelliteTiles, w, h, $"{outPath}\\{GetMapName(MapType.Satellite)}.{outFormat}", overlapX, overlapY);
            if (MaskTiles.Count > 0)
                StitchTiles(MaskTiles, w, h, $"{outPath}\\{GetMapName(MapType.Mask)}.{outFormat}", overlapX, overlapY);

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

                        /*if ((line = line.Trim()) != "") {
                            XYZ coord = new XYZ();
                            int i = 0;
                            foreach (string s in line.Split())
                                if (s != "") coord[i++] = double.Parse(s);
                            if (i != 3 && i != 1) Console.WriteLine($"Line {l}: {i} coordinates instead of 3 or 1 (height only)");
                            else if (!zOnly && i == 1) zOnly = true;

                            if (!zOnly) {
                                if (coord.x < min.x) min.x = coord.x;
                                if (coord.y < min.y) min.y = coord.y;
                                if (coord.z < min.z) min.z = coord.z;

                                if (coord.x > max.x) max.x = coord.x;
                                if (coord.y > max.y) max.y = coord.y;
                                if (coord.z > max.z) max.z = coord.z;
                            }
                            else {
                                coord.z = coord.x;
                                coord.x = coord.y = 0.0;
                            }

                            coords.Add(coord);
                        }*/
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
            #endregion
        }

        static void CompareImages(string imPath1, string imPath2, string outFile) {
            Bitmap a = new Bitmap(imPath1);
            Bitmap b = new Bitmap(imPath2);

            IMG.PixelFormat imgFormat = IMG.PixelFormat.Format48bppRgb;
            int w = a.Width, h = a.Height;
            using (Bitmap dif = new Bitmap(w, h, imgFormat)) {
                BitmapData aData = a.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, a.PixelFormat);
                IntPtr aPtr = aData.Scan0;
                BitmapData bData = b.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, b.PixelFormat);
                IntPtr bPtr = bData.Scan0;

                BitmapData difData = dif.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, imgFormat);
                IntPtr difPtr = difData.Scan0;

                int valCount = w * h * 3;
                byte[] aVals = new byte[valCount];
                short[] bVals = new short[valCount];
                short[] difVals = new short[valCount];

                try {
                    Marshal.Copy(aPtr, aVals, 0, valCount);
                    Marshal.Copy(bPtr, bVals, 0, valCount);

                    int j = 0;
                    for (int i = 0; i < w * h * 3;) {
                        difVals[i++] = bVals[j++];
                        difVals[i++] = bVals[j++];
                        difVals[i++] = bVals[j++];
                        //difVals[i++] = ShortBitConverter.Convert(ushort.MaxValue);
                    }

                    Marshal.Copy(difVals, 0, difPtr, valCount);
                }
                finally {
                    a.UnlockBits(aData);
                    b.UnlockBits(bData);
                    dif.UnlockBits(difData);
                }

                SaveBmp(dif, outFile);
            }

            a.Dispose();
            b.Dispose();
        }
        #endregion
    }
}
