﻿/*
 * (c) 2012 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 */

using Mosa.Platform.x86.Intrinsic;
using Colors = Mosa.Kernel.x86.Colors;

namespace Mosa.CoolWorld.x86
{
	/// <summary>
	/// 
	/// </summary>
	public static class Boot
	{
		public static Console Console;
		public static Screen Screen;
		/// <summary>
		/// Main
		/// </summary>
		public static void Main()
		{
			Mosa.Kernel.x86.Kernel.Setup();

			Mosa.Kernel.x86.IDT.SetInterruptHandler(ProcessInterrupt);

			Screen = new Screen();
			Console = Screen.Boot;
			Console.Screen = Screen;

			Console.GotoTop();
			Console.Color = Colors.White;
			Console.BackgroundColor = Colors.Green;

			Console.Write(@"                   MOSA OS Version 0.1 - Compiler Version 1.0");
			FillLine();
			Console.Color = Colors.White;
			Console.BackgroundColor = Colors.Black;

			Mosa.Kernel.x86.Smbios.BiosInformationStructure biosInfo = new Kernel.x86.Smbios.BiosInformationStructure();
			Mosa.Kernel.x86.Smbios.CpuStructure cpuInfo = new Kernel.x86.Smbios.CpuStructure();

			Console.WriteLine("> Checking BIOS...");
			BulletPoint(); Console.Write("Vendor  "); InBrackets(biosInfo.BiosVendor, Colors.White, Colors.LightBlue); Console.WriteLine();
			BulletPoint(); Console.Write("Version "); InBrackets(biosInfo.BiosVersion, Colors.White, Colors.LightBlue); Console.WriteLine();

			Console.WriteLine("> Checking CPU...");
			BulletPoint(); Console.Write("Vendor  "); InBrackets(cpuInfo.Vendor, Colors.White, Colors.LightBlue); Console.WriteLine();
			BulletPoint(); Console.Write("Version "); InBrackets(cpuInfo.Version, Colors.White, Colors.LightBlue); Console.WriteLine();

			Console.WriteLine("> Initializing hardware abstraction layer...");
			Setup.Initialize();

			Console.WriteLine("> Adding hardware devices...");
			Setup.Start();

			Console.WriteLine("> System ready");
			Console.WriteLine();
			Console.Goto(24, 0);
			Console.Color = Colors.White;
			Console.BackgroundColor = Colors.Green;
			Console.Write("          Copyright (C) 2008-2001 [Managed Operating System Alliance]");
			FillLine();

			Console.BackgroundColor = Colors.Black;
			Console.Goto(15, 0);
			Console.Color = Colors.Green;
			Console.Write("> ");
			Console.Color = Colors.Yellow;

			Mosa.DeviceDrivers.ScanCodeMap.US KBDMAP = new DeviceDrivers.ScanCodeMap.US();

			while (true)
			{
				byte scancode = Setup.Keyboard.GetScanCode();

				if (scancode != 0)
				{
					Mosa.Kernel.x86.Debug.Trace("Main.Main Key Scan Code: " + scancode.ToString());

					DeviceSystem.KeyEvent keyevent = KBDMAP.ConvertScanCode(scancode);

					Mosa.Kernel.x86.Debug.Trace("Main.Main Key Character: " + keyevent.Character.ToString());

					if (keyevent.KeyPress == DeviceSystem.KeyEvent.Press.Make)
					{
						if (keyevent.Character != 0)
							Console.Write(keyevent.Character);
					
						if (keyevent.KeyType == DeviceSystem.KeyType.F1)
							Screen.Active = Screen.Boot;
						else if (keyevent.KeyType == DeviceSystem.KeyType.F2)
							Screen.Active = Screen.Debug;

					}
					Mosa.Kernel.x86.Debug.Trace("Main.Main Key Character: " + ((uint)keyevent.Character).ToString());
				}

				Native.Hlt();
			}
		}

		public static void FillLine()
		{
			for (uint c = 80 - Console.Column; c != 0; c--)
				Console.Write(' ');
		}

		public static void PrintDone()
		{
			InBrackets("Done", Colors.White, Colors.LightGreen); Console.WriteLine();
		}

		public static void BulletPoint()
		{
			Console.Color = Colors.Yellow;
			Console.Write("  * ");
			Console.Color = Colors.White;
		}

		public static void InBrackets(string message, byte outerColor, byte innerColor)
		{
			Console.Color = outerColor;
			Console.Write("[");
			Console.Color = innerColor;
			Console.Write(message);
			Console.Color = outerColor;
			Console.Write("]");
		}

		private static uint counter = 0;

		public static void ProcessInterrupt(byte interrupt, byte errorCode)
		{
			uint c = Console.Column;
			uint r = Console.Row;
			byte col = Console.Color;
			byte back = Console.BackgroundColor;

			Console.Column = 55;
			Console.Row = 23;
			Console.Color = Colors.Cyan;
			Console.BackgroundColor = Colors.Black;
			Console.Write("        ");
			Console.Column = 55;
			Console.Row = 23;

			counter++;
			Console.Write(counter, 10, 7);
			Console.Write(':');
			Console.Write(interrupt, 16, 2);
			Console.Write(':');
			Console.Write(errorCode, 16, 2);

			if (interrupt == 14)
			{
				// Page Fault!
				Mosa.Kernel.x86.PageFaultHandler.Fault(errorCode);
			}
			else if (interrupt == 0x20)
			{
				// Timer Interrupt! Switch Tasks!	
			}
			else if (interrupt >= 0x20 && interrupt < 0x30)
			{
				Console.Write('-');
				Console.Write(counter, 10, 7);
				Console.Write(':');
				Console.Write(interrupt, 16, 2);

				Mosa.DeviceSystem.HAL.ProcessInterrupt((byte)(interrupt - 0x20), errorCode);
				//Mosa.Kernel.x86.Debug.Trace("Returned from HAL.ProcessInterrupt");
			}

			Console.Column = c;
			Console.Row = r;
			Console.Color = col;
			Console.BackgroundColor = back;
		}

	}
}