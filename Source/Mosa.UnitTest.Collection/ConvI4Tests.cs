﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.UnitTest.Collection
{
	public static class ConvI4Tests
	{
		[MosaUnitTest(Series = "I4I1")]
		public static bool ConvI4_I1(int expect, sbyte a)
		{
			return expect == a;
		}

		[MosaUnitTest(Series = "I4I2")]
		public static bool ConvI4_I2(int expect, short a)
		{
			return expect == a;
		}

		[MosaUnitTest(Series = "I4I4")]
		public static bool ConvI4_I4(int expect, int a)
		{
			return expect == a;
		}

		[MosaUnitTest(Series = "I4I8")]
		public static bool ConvI4_I8(int expect, long a)
		{
			return expect == ((int)a);
		}

		public static bool ConvI4_R4(int expect, float a)
		{
			return expect == ((int)a);
		}

		public static bool ConvI4_R8(int expect, double a)
		{
			return expect == ((int)a);
		}
	}
}
