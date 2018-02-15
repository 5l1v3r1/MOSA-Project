﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Xunit;

namespace Mosa.UnitTest.Collection.xUnit
{
	public class BoxingFixture : TestFixture
	{
		//private double DoubleTolerance = 0.000001d;
		//private float FloatTolerance = 0.00001f;

		[Theory]
		[MemberData(nameof(U1))]
		public void BoxU1(byte value)
		{
			Assert.Equal(BoxingTests.BoxU1(value), Run<byte>("Mosa.UnitTest.Collection.BoxingTests.BoxU1", value));
		}

		[Theory]
		[MemberData(nameof(U2))]
		public void BoxU2(ushort value)
		{
			Assert.Equal(BoxingTests.BoxU2(value), Run<ushort>("Mosa.UnitTest.Collection.BoxingTests.BoxU2", value));
		}

		[Theory]
		[MemberData(nameof(U4))]
		public void BoxU4(uint value)
		{
			Assert.Equal(BoxingTests.BoxU4(value), Run<uint>("Mosa.UnitTest.Collection.BoxingTests.BoxU4", value));
		}

		[Theory]
		[MemberData(nameof(U8))]
		public void BoxU8(ulong value)
		{
			Assert.Equal(BoxingTests.BoxU8(value), Run<ulong>("Mosa.UnitTest.Collection.BoxingTests.BoxU8", value));
		}

		[Theory]
		[MemberData(nameof(I1))]
		public void BoxI1(sbyte value)
		{
			Assert.Equal(BoxingTests.BoxI1(value), Run<sbyte>("Mosa.UnitTest.Collection.BoxingTests.BoxI1", value));
		}

		[Theory]
		[MemberData(nameof(I2))]
		public void BoxI2(short value)
		{
			Assert.Equal(BoxingTests.BoxI2(value), Run<short>("Mosa.UnitTest.Collection.BoxingTests.BoxI2", value));
		}

		[Theory]
		[MemberData(nameof(I4))]
		public void BoxI4(int value)
		{
			Assert.Equal(BoxingTests.BoxI4(value), Run<int>("Mosa.UnitTest.Collection.BoxingTests.BoxI4", value));
		}

		[Theory]
		[MemberData(nameof(I8))]
		public void BoxI8(long value)
		{
			Assert.Equal(BoxingTests.BoxI8(value), Run<long>("Mosa.UnitTest.Collection.BoxingTests.BoxI8", value));
		}

		//[Theory]
		//public void BoxR4([R4NumberNoExtremes]float value)
		//{
		//	float result = Run<float>("Mosa.UnitTest.Collection.BoxingTests.BoxR4", value);
		//	float expected = BoxingTests.BoxR4(value);

		//	Assert.ApproximatelyEqual(expected, result, FloatTolerance);
		//	Assert.False(float.IsNaN(result), "Returned NaN");
		//}

		//[Theory]
		//public void BoxR8([R8NumberNoExtremes]double value)
		//{
		//	double result = Run<double>("Mosa.UnitTest.Collection.BoxingTests.BoxR8", value);
		//	double expected = BoxingTests.BoxR8(value);

		//	Assert.ApproximatelyEqual(expected, result, DoubleTolerance);
		//	Assert.False(double.IsNaN(result), "Returned NaN");
		//}

		[Theory]
		[MemberData(nameof(C))]
		public void BoxC(char value)
		{
			Assert.Equal(BoxingTests.BoxC(value), Run<char>("Mosa.UnitTest.Collection.BoxingTests.BoxC", value));
		}

		[Theory]
		[MemberData(nameof(U1))]
		public void EqualsU1(byte value)
		{
			Assert.Equal(Mosa.UnitTest.Collection.BoxingTests.EqualsU1(value), Run<bool>("Mosa.UnitTest.Collection.BoxingTests.EqualsU1", value));
		}

		[Theory]
		[MemberData(nameof(U2))]
		public void EqualsU2(ushort value)
		{
			Assert.Equal(Mosa.UnitTest.Collection.BoxingTests.EqualsU2(value), Run<bool>("Mosa.UnitTest.Collection.BoxingTests.EqualsU2", value));
		}

		[Theory]
		[MemberData(nameof(U4))]
		public void EqualsU4(uint value)
		{
			Assert.Equal(Mosa.UnitTest.Collection.BoxingTests.EqualsU4(value), Run<bool>("Mosa.UnitTest.Collection.BoxingTests.EqualsU4", value));
		}

		[Theory]
		[MemberData(nameof(U8))]
		public void EqualsU8(ulong value)
		{
			Assert.Equal(Mosa.UnitTest.Collection.BoxingTests.EqualsU8(value), Run<bool>("Mosa.UnitTest.Collection.BoxingTests.EqualsU8", value));
		}

		[Theory]
		[MemberData(nameof(I1))]
		public void EqualsI1(sbyte value)
		{
			Assert.Equal(Mosa.UnitTest.Collection.BoxingTests.EqualsI1(value), Run<bool>("Mosa.UnitTest.Collection.BoxingTests.EqualsI1", value));
		}

		[Theory]
		[MemberData(nameof(I2))]
		public void EqualsI2(short value)
		{
			Assert.Equal(Mosa.UnitTest.Collection.BoxingTests.EqualsI2(value), Run<bool>("Mosa.UnitTest.Collection.BoxingTests.EqualsI2", value));
		}

		[Theory]
		[MemberData(nameof(I4))]
		public void EqualsI4(int value)
		{
			Assert.Equal(Mosa.UnitTest.Collection.BoxingTests.EqualsI4(value), Run<bool>("Mosa.UnitTest.Collection.BoxingTests.EqualsI4", value));
		}

		[Theory]
		[MemberData(nameof(I8))]
		public void EqualsI8(long value)
		{
			Assert.Equal(Mosa.UnitTest.Collection.BoxingTests.EqualsI8(value), Run<bool>("Mosa.UnitTest.Collection.BoxingTests.EqualsI8", value));
		}

		[Theory]
		[MemberData(nameof(R4NotNaN))]
		public void EqualsR4(float value)
		{
			Assert.Equal(Mosa.UnitTest.Collection.BoxingTests.EqualsR4(value), Run<bool>("Mosa.UnitTest.Collection.BoxingTests.EqualsR4", value));
		}

		[Theory]
		[MemberData(nameof(R8NotNaN))]
		public void EqualsR8(double value)
		{
			Assert.Equal(Mosa.UnitTest.Collection.BoxingTests.EqualsR8(value), Run<bool>("Mosa.UnitTest.Collection.BoxingTests.EqualsR8", value));
		}

		[Theory]
		[MemberData(nameof(C))]
		public void EqualsC(char value)
		{
			Assert.Equal(Mosa.UnitTest.Collection.BoxingTests.EqualsC(value), Run<bool>("Mosa.UnitTest.Collection.BoxingTests.EqualsC", value));
		}
	}
}
