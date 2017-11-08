﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Xunit;

namespace Mosa.UnitTest.Collection.xUnit
{
	public class Ldarga : TestFixture
	{
		//private double DoubleTolerance = 0.000001d;
		//private float FloatTolerance = 0.00001f;

		#region CheckValue

		[Theory]
		[MemberData(nameof(U1), DisableDiscoveryEnumeration = true)]
		public void LdargaCheckValueU1(byte a)
		{
			Assert.Equal(a, Run<byte>("Mosa.UnitTest.Collection.LdargaTests.LdargaCheckValueU1", a));
		}

		[Theory]
		[MemberData(nameof(U2), DisableDiscoveryEnumeration = true)]
		public void LdargaCheckValueU2(ushort a)
		{
			Assert.Equal(a, Run<ushort>("Mosa.UnitTest.Collection.LdargaTests.LdargaCheckValueU2", a));
		}

		[Theory]
		[MemberData(nameof(U4), DisableDiscoveryEnumeration = true)]
		public void LdargaCheckValueU4(uint a)
		{
			Assert.Equal(a, Run<uint>("Mosa.UnitTest.Collection.LdargaTests.LdargaCheckValueU4", a));
		}

		[Theory]
		[MemberData(nameof(U8), DisableDiscoveryEnumeration = true)]
		public void LdargaCheckValueU8(ulong a)
		{
			Assert.Equal(a, Run<ulong>("Mosa.UnitTest.Collection.LdargaTests.LdargaCheckValueU8", a));
		}

		[Theory]
		[MemberData(nameof(I1), DisableDiscoveryEnumeration = true)]
		public void LdargaCheckValueI1(sbyte a)
		{
			Assert.Equal(a, Run<sbyte>("Mosa.UnitTest.Collection.LdargaTests.LdargaCheckValueI1", a));
		}

		[Theory]
		[MemberData(nameof(I2), DisableDiscoveryEnumeration = true)]
		public void LdargaCheckValueI2(short a)
		{
			Assert.Equal(a, Run<short>("Mosa.UnitTest.Collection.LdargaTests.LdargaCheckValueI2", a));
		}

		[Theory]
		[MemberData(nameof(I4), DisableDiscoveryEnumeration = true)]
		public void LdargaCheckValueI4(int a)
		{
			Assert.Equal(a, Run<int>("Mosa.UnitTest.Collection.LdargaTests.LdargaCheckValueI4", a));
		}

		[Theory]
		[MemberData(nameof(I8), DisableDiscoveryEnumeration = true)]
		public void LdargaCheckValueI8(long a)
		{
			Assert.Equal(a, Run<long>("Mosa.UnitTest.Collection.LdargaTests.LdargaCheckValueI8", a));
		}

		[Theory]
		[MemberData(nameof(C), DisableDiscoveryEnumeration = true)]
		public void LdargaCheckValueC(char a)
		{
			Assert.Equal(a, Run<char>("Mosa.UnitTest.Collection.LdargaTests.LdargaCheckValueC", a));
		}

		[Theory]
		[MemberData(nameof(B), DisableDiscoveryEnumeration = true)]
		public void LdargaCheckValueB(bool a)
		{
			Assert.Equal(a, Run<bool>("Mosa.UnitTest.Collection.LdargaTests.LdargaCheckValueB", a));
		}

		[Theory]
		[MemberData(nameof(R4), DisableDiscoveryEnumeration = true)]
		public void LdargaCheckValueR4(float a)
		{
			Assert.Equal(a, Run<float>("Mosa.UnitTest.Collection.LdargaTests.LdargaCheckValueR4", a));
		}

		[Theory]
		[MemberData(nameof(R8), DisableDiscoveryEnumeration = true)]
		public void LdargaCheckValueR8(double a)
		{
			Assert.Equal(a, Run<double>("Mosa.UnitTest.Collection.LdargaTests.LdargaCheckValueR8", a));
		}

		#endregion CheckValue

		#region ChangeValue

		[Theory]
		[MemberData(nameof(U1U1), DisableDiscoveryEnumeration = true)]
		public void LdargaChangeValueU1(byte a, byte b)
		{
			Assert.Equal(b, Run<byte>("Mosa.UnitTest.Collection.LdargaTests.LdargaChangeValueU1", a, b));
		}

		[Theory]
		[MemberData(nameof(U2U2), DisableDiscoveryEnumeration = true)]
		public void LdargaChangeValueU2(ushort a, ushort b)
		{
			Assert.Equal(b, Run<ushort>("Mosa.UnitTest.Collection.LdargaTests.LdargaChangeValueU2", a, b));
		}

		[Theory]
		[MemberData(nameof(U4U4), DisableDiscoveryEnumeration = true)]
		public void LdargaChangeValueU4(uint a, uint b)
		{
			Assert.Equal(b, Run<uint>("Mosa.UnitTest.Collection.LdargaTests.LdargaChangeValueU4", a, b));
		}

		[Theory]
		[MemberData(nameof(U8U8), DisableDiscoveryEnumeration = true)]
		public void LdargaChangeValueU8(ulong a, ulong b)
		{
			Assert.Equal(b, Run<ulong>("Mosa.UnitTest.Collection.LdargaTests.LdargaChangeValueU8", a, b));
		}

		[Theory]
		[MemberData(nameof(I1I1), DisableDiscoveryEnumeration = true)]
		public void LdargaChangeValueI1(sbyte a, sbyte b)
		{
			Assert.Equal(b, Run<sbyte>("Mosa.UnitTest.Collection.LdargaTests.LdargaChangeValueI1", a, b));
		}

		[Theory]
		[MemberData(nameof(I2I2), DisableDiscoveryEnumeration = true)]
		public void LdargaChangeValueI2(short a, short b)
		{
			Assert.Equal(b, Run<short>("Mosa.UnitTest.Collection.LdargaTests.LdargaChangeValueI2", a, b));
		}

		[Theory]
		[MemberData(nameof(I4I4), DisableDiscoveryEnumeration = true)]
		public void LdargaChangeValueI4(int a, int b)
		{
			Assert.Equal(b, Run<int>("Mosa.UnitTest.Collection.LdargaTests.LdargaChangeValueI4", a, b));
		}

		[Theory]
		[MemberData(nameof(I8I8), DisableDiscoveryEnumeration = true)]
		public void LdargaChangeValueI8(long a, long b)
		{
			Assert.Equal(b, Run<long>("Mosa.UnitTest.Collection.LdargaTests.LdargaChangeValueI8", a, b));
		}

		[Theory]
		[MemberData(nameof(CC), DisableDiscoveryEnumeration = true)]
		public void LdargaChangeValueC(char a, char b)
		{
			Assert.Equal(b, Run<char>("Mosa.UnitTest.Collection.LdargaTests.LdargaChangeValueC", a, b));
		}

		[Theory]
		[MemberData(nameof(BB), DisableDiscoveryEnumeration = true)]
		public void LdargaChangeValueB(bool a, bool b)
		{
			Assert.Equal(b, Run<bool>("Mosa.UnitTest.Collection.LdargaTests.LdargaChangeValueB", a, b));
		}

		[Theory]
		[MemberData(nameof(R4R4), DisableDiscoveryEnumeration = true)]
		public void LdargaChangeValueR4(float a, float b)
		{
			Assert.Equal(b, Run<float>("Mosa.UnitTest.Collection.LdargaTests.LdargaChangeValueR4", a, b));
		}

		[Theory]
		[MemberData(nameof(U1U1), DisableDiscoveryEnumeration = true)]
		public void LdargaChangeValueR8(double a, double b)
		{
			Assert.Equal(b, Run<double>("Mosa.UnitTest.Collection.LdargaTests.LdargaChangeValueR8", a, b));
		}

		#endregion ChangeValue
	}
}
