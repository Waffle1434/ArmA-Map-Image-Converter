using ArmA_Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMG = System.Drawing.Imaging;
using UTIL = CommandLineUtils.Utils;

namespace ArmA_Converter_CMD {
    class CMDProgram {
        static void InputFormat() => Console.WriteLine("Arguments: <Input Path> [<Output Path>] [-paa | -stitch | -xyz] [-pause]\n\t( Output Path will default to Input Path. )");

        static void Main(string[] args) {
            bool pause = args.Contains("-pause");

            if (args.Contains("-paa")) CmdPaa(args);
            else if (args.Contains("-stitch")) CmdStitch(args);
            else if (args.Contains("-xyz")) CmdXYZ(args);
            else if (args.Contains("-cfg")) CmdCFG(args);
            else Console.Write("No command detected");

            if (pause) UTIL.KeyToContinue();
        }

        #region Command Line Parsing
        static bool ArgIsNotParam(string arg) => !arg.StartsWith("-");

        static void CmdPaa(string[] args) {
            if (args.Length < 1) {
                Console.WriteLine("Missing Input Images Path.");
                InputFormat();
                return;
            }
            string inPath = args[0];

            string outPath;
            if (args.Length < 2 || !ArgIsNotParam(args[1])) {
                string defaultFileType = "png";
                outPath = UTIL.SetExtension(inPath, defaultFileType);
            }
            else outPath = args[1];

            if (Converter.ImgToPaaPath == "") {
                Converter.ImgToPaaPath = @"D:\Steam Games\steamapps\common\Arma 3 Tools\ImageToPAA";
                Converter.ImgToPaaPath += @"\ImageToPAA.exe";
            }

            Converter.ConvertImages(inPath, outPath);
        }
        static void CmdStitch(string[] args) {
            if (args.Length < 1) {
                Console.WriteLine("Missing Input Images Path.");
                InputFormat();
                return;
            }
            string inPath = args[0];

            string outPath;
            if (args.Length < 2 || !ArgIsNotParam(args[1])) {
                string defaultFileType = "jpg";
                outPath = UTIL.SetExtension(inPath, defaultFileType);
            }
            else outPath = args[1];

            Converter.StitchMapTiles(inPath, outPath, overlap: 16);
        }
        static void CmdXYZ(string[] args) {
            if (args.Length < 1) {
                Console.WriteLine("Missing Input Path.");
                InputFormat();
                return;
            }
            string inPath = args[0];

            string outPath;
            if (args.Length < 2 || !ArgIsNotParam(args[1])) {
                string defaultFileType = "jpg";
                outPath = UTIL.SetExtension(inPath, defaultFileType);
            }
            else outPath = args[1];

            IMG.PixelFormat format = IMG.PixelFormat.Format24bppRgb;
            string arg;
            if ((arg = args.First(x => x.StartsWith("-bits="))) != null) {
                string bits = arg.Substring(arg.IndexOf('=') + 1);
                switch (bits) {
                    case "8": format = IMG.PixelFormat.Format24bppRgb; break;
                    case "16": format = IMG.PixelFormat.Format48bppRgb; break;
                    default:
                        Console.WriteLine($"{bits} Bit Format Not Supported, Supported Formats: 8 (Default), 16");
                        goto case "8";
                }
            }

            Converter.ConvertXYZ(inPath, outPath, format);
        }
        static void CmdCFG(string[] args) {
            if (args.Length < 1) {
                Console.WriteLine("Missing Input Path.");
                InputFormat();
                return;
            }
            string inPath = args[0];

            /*string outPath;
            if (args.Length < 2) {
                outPath = UTIL.SetExtension(inPath, "cpp");
            }
            else outPath = args[1];*/

            Converter.ParseCFGFile(inPath);
        }
        #endregion
    }
}
