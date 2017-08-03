﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using CommandLine;
using Mosa.Compiler.Common;
using Mosa.Utility.BootImage;
using System;
using System.IO;

namespace Mosa.Tool.CreateBootImage
{
	/// <summary>
	///
	/// </summary>
	internal class Program
	{
		public static BootImageOptions Parse(string filename)
		{
			Options options = ParseOptions(File.ReadAllText(filename).Split(new char[] { '\r', '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries));
			if (options == null)
				return null;

			return options.BootImageOptions;

			//var options = new BootImageOptions();

			//var reader = File.OpenText(filename);

			//while (true)
			//{
			//	string line = reader.ReadLine();
			//	if (line == null) break;

			//	if (string.IsNullOrEmpty(line))
			//		continue;

			//	string[] parts = line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

			//	switch (parts[0].Trim())
			//	{
			//		case "-mbr": options.MBROption = true; options.MBRCode = (parts.Length > 1) ? File.ReadAllBytes(parts[1]) : null; break;
			//		case "-boot": options.FatBootCode = (parts.Length > 1) ? File.ReadAllBytes(parts[1]) : null; break;
			//		case "-vhd": options.ImageFormat = ImageFormat.VHD; break;
			//		case "-img": options.ImageFormat = ImageFormat.IMG; break;
			//		case "-vdi": options.ImageFormat = ImageFormat.VDI; break;
			//		case "-syslinux": options.PatchSyslinuxOption = true; break;
			//		case "-guid": if (parts.Length > 1) options.MediaGuid = new Guid(parts[1]); break;
			//		case "-snapguid": if (parts.Length > 1) options.MediaLastSnapGuid = new Guid(parts[1]); break;
			//		case "-fat12": options.FileSystem = FileSystem.FAT12; break;
			//		case "-fat16": options.FileSystem = FileSystem.FAT16; break;
			//		case "-fat32": options.FileSystem = FileSystem.FAT32; break;
			//		case "-file":
			//			if (parts.Length > 2) options.IncludeFiles.Add(new IncludeFile(parts[1], parts[2]));
			//			else options.IncludeFiles.Add(new IncludeFile(parts[1])); break;
			//		case "-blocks": options.BlockCount = Convert.ToUInt32(parts[1]); break;
			//		case "-volume": options.VolumeLabel = parts[1]; break;
			//		default: break;
			//	}
			//}

			//reader.Close();

			//return options;
		}

		private static Options ParseOptions(string[] args)
		{
			ParserResult<Options> result = new Parser(config => config.HelpWriter = Console.Out).ParseArguments<Options>(args);

			if (result.Tag == ParserResultType.NotParsed)
			{
				return null;
			}

			return ((Parsed<Options>)result).Value;
		}

		/// <summary>
		/// Main
		/// </summary>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		private static int Main(string[] args)
		{
			Console.WriteLine();
			Console.WriteLine("CreateBootImage v1.6 [www.mosa-project.org]");
			Console.WriteLine("Copyright 2015. New BSD License.");
			Console.WriteLine("Written by Philipp Garcia (phil@thinkedge.com)");
			Console.WriteLine();

			bool valid = args.Length == 2;

			if (valid)
				valid = System.IO.File.Exists(args[0]);

			if (!valid)
			{
				Console.WriteLine("Usage: CreateBootImage <boot.config file> <image name>");
				Console.Error.WriteLine("ERROR: Missing arguments");
				return -1;
			}

			Console.WriteLine("Building image...");

			try
			{
				var bootImageOptions = Parse(args[0]);

				if (bootImageOptions == null)
				{
					Console.WriteLine("Usage: CreateBootImage <boot.config file> <image name>");
					Console.Error.WriteLine("ERROR: Invalid options");
					return -1;
				}

				bootImageOptions.DiskImageFileName = args[1];

				Generator.Create(bootImageOptions);

				Console.WriteLine("Completed!");
			}
			catch (Exception e)
			{
				Console.Error.WriteLine("ERROR: " + e.ToString());
				return -1;
			}

			return 0;
		}
	}
}
