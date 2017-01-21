﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Utility.DebugEngine
{
	public static class DebugCode
	{
		public const int Connecting = 10;
		public const int Connected = 11;
		public const int Disconnected = 12;

		public const int UnknownData = 99;

		public const int Alive = 1000;
		public const int Ready = 1001;
		public const int Ping = 1002;
		public const int InformationalMessage = 1003;
		public const int WarningMessage = 1004;
		public const int ErrorMessage = 1005;
		public const int SendNumber = 1006;
		public const int ReadMemory = 1010;
		public const int WriteMemory = 1011;
		public const int ReadCR3 = 1012;
		public const int Scattered32BitReadMemory = 1013;
		public const int ClearMemory = 1014;

		public const int CompressedWriteMemory = 1021;

		public const int HardJump = 1111;

		public const int ExecuteUnitTest = 2000;
		public const int AbortUnitTest = 2001;
	}
}
