﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Xunit;

namespace Mosa.UnitTest.Collection.xUnit
{
	public class Int16Fixture : TestFixture
	{
		[Theory]
		[MemberData(nameof(I2I2), DisableDiscoveryEnumeration = true)]
		public void AddI2I2(short a, short b)
		{
			Assert.Equal(Int16Tests.AddI2I2(a, b), Run<int>("Mosa.UnitTest.Collection.Int16Tests.AddI2I2", a, b));
		}

		[Theory]
		[MemberData(nameof(I2I2), DisableDiscoveryEnumeration = true)]
		public void SubI2I2(short a, short b)
		{
			Assert.Equal(Int16Tests.SubI2I2(a, b), Run<int>("Mosa.UnitTest.Collection.Int16Tests.SubI2I2", a, b));
		}

		[Theory]
		[MemberData(nameof(I2I2), DisableDiscoveryEnumeration = true)]
		public void MulI2I2(short a, short b)
		{
			Assert.Equal(Int16Tests.MulI2I2(a, b), Run<int>("Mosa.UnitTest.Collection.Int16Tests.MulI2I2", a, b));
		}

		[Theory]
		[MemberData(nameof(I2I2), DisableDiscoveryEnumeration = true)]
		public void DivI2I2(short a, short b)
		{
			if (b == 0)
			{
				return;
			}

			Assert.Equal(Int16Tests.DivI2I2(a, b), Run<int>("Mosa.UnitTest.Collection.Int16Tests.DivI2I2", a, b));
		}

		//[Theory]
		//[ExpectedException(typeof(DivideByZeroException))]
		private void DivI2I2DivideByZeroException(short a)
		{
			Assert.Equal(Int16Tests.DivI2I2(a, 0), Run<int>("Mosa.UnitTest.Collection.Int16Tests.DivI2I2", a, (short)0));
		}

		[Theory]
		[MemberData(nameof(I2I2), DisableDiscoveryEnumeration = true)]
		public void RemI2I2(short a, short b)
		{
			if (b == 0)
			{
				return;
			}

			Assert.Equal(Int16Tests.RemI2I2(a, b), Run<int>("Mosa.UnitTest.Collection.Int16Tests.RemI2I2", a, b));
		}

		//[Theory]
		//[ExpectedException(typeof(DivideByZeroException))]
		private void RemI2I2DivideByZeroException(short a)
		{
			Assert.Equal(Int16Tests.RemI2I2(a, 0), Run<int>("Mosa.UnitTest.Collection.Int16Tests.RemI2I2", a, (short)0));
		}

		[Theory]
		[MemberData(nameof(I2), DisableDiscoveryEnumeration = true)]
		public void RetI2(short a)
		{
			Assert.Equal(Int16Tests.RetI2(a), Run<short>("Mosa.UnitTest.Collection.Int16Tests.RetI2", a));
		}

		[Theory]
		[MemberData(nameof(I2I2), DisableDiscoveryEnumeration = true)]
		public void AndI2I2(short a, short b)
		{
			Assert.Equal(Int16Tests.AndI2I2(a, b), Run<int>("Mosa.UnitTest.Collection.Int16Tests.AndI2I2", a, b));
		}

		[Theory]
		[MemberData(nameof(I2I2), DisableDiscoveryEnumeration = true)]
		public void OrI2I2(short a, short b)
		{
			Assert.Equal(Int16Tests.OrI2I2(a, b), Run<int>("Mosa.UnitTest.Collection.Int16Tests.OrI2I2", a, b));
		}

		[Theory]
		[MemberData(nameof(I2I2), DisableDiscoveryEnumeration = true)]
		public void XorI2I2(short a, short b)
		{
			Assert.Equal(Int16Tests.XorI2I2(a, b), Run<int>("Mosa.UnitTest.Collection.Int16Tests.XorI2I2", a, b));
		}

		[Theory]
		[MemberData(nameof(I2), DisableDiscoveryEnumeration = true)]
		public void CompI2(short a)
		{
			Assert.Equal(Int16Tests.CompI2(a), Run<int>("Mosa.UnitTest.Collection.Int16Tests.CompI2", a));
		}

		[Theory]
		[MemberData(nameof(I2U1UpTo16), DisableDiscoveryEnumeration = true)]
		public void ShiftLeftI2I2(short a, byte b)
		{
			Assert.Equal(Int16Tests.ShiftLeftI2I2(a, b), Run<int>("Mosa.UnitTest.Collection.Int16Tests.ShiftLeftI2I2", a, b));
		}

		[Theory]
		[MemberData(nameof(I2U1UpTo16), DisableDiscoveryEnumeration = true)]
		public void ShiftRightI2I2(short a, byte b)
		{
			Assert.Equal(Int16Tests.ShiftRightI2I2(a, b), Run<int>("Mosa.UnitTest.Collection.Int16Tests.ShiftRightI2I2", a, b));
		}

		[Theory]
		[MemberData(nameof(I2I2), DisableDiscoveryEnumeration = true)]
		public void CeqI2I2(short a, short b)
		{
			Assert.Equal(Int16Tests.CeqI2I2(a, b), Run<bool>("Mosa.UnitTest.Collection.Int16Tests.CeqI2I2", a, b));
		}

		[Theory]
		[MemberData(nameof(I2I2), DisableDiscoveryEnumeration = true)]
		public void CltI2I2(short a, short b)
		{
			Assert.Equal(Int16Tests.CltI2I2(a, b), Run<bool>("Mosa.UnitTest.Collection.Int16Tests.CltI2I2", a, b));
		}

		[Theory]
		[MemberData(nameof(I2I2), DisableDiscoveryEnumeration = true)]
		public void CgtI2I2(short a, short b)
		{
			Assert.Equal(Int16Tests.CgtI2I2(a, b), Run<bool>("Mosa.UnitTest.Collection.Int16Tests.CgtI2I2", a, b));
		}

		[Theory]
		[MemberData(nameof(I2I2), DisableDiscoveryEnumeration = true)]
		public void CleI2I2(short a, short b)
		{
			Assert.Equal(Int16Tests.CleI2I2(a, b), Run<bool>("Mosa.UnitTest.Collection.Int16Tests.CleI2I2", a, b));
		}

		[Theory]
		[MemberData(nameof(I2I2), DisableDiscoveryEnumeration = true)]
		public void CgeI2I2(short a, short b)
		{
			Assert.Equal(Int16Tests.CgeI2I2(a, b), Run<bool>("Mosa.UnitTest.Collection.Int16Tests.CgeI2I2", a, b));
		}

		[Fact]
		public void Newarr()
		{
			Assert.True(Run<bool>("Mosa.UnitTest.Collection.Int16Tests.Newarr"));
		}

		[Theory]
		[MemberData(nameof(I4Small), DisableDiscoveryEnumeration = true)]
		public void Ldlen(int length)
		{
			Assert.True(Run<bool>("Mosa.UnitTest.Collection.Int16Tests.Ldlen", length));
		}

		[Theory]
		[MemberData(nameof(I4SmallI2), DisableDiscoveryEnumeration = true)]
		public void StelemI2(int index, short value)
		{
			Assert.True(Run<bool>("Mosa.UnitTest.Collection.Int16Tests.Stelem", index, value));
		}

		[Theory]
		[MemberData(nameof(I4SmallI2), DisableDiscoveryEnumeration = true)]
		public void LdelemI2(int index, short value)
		{
			Assert.Equal(Int16Tests.Ldelem(index, value), Run<short>("Mosa.UnitTest.Collection.Int16Tests.Ldelem", index, value));
		}

		[Theory]
		[MemberData(nameof(I4SmallI2), DisableDiscoveryEnumeration = true)]
		public void LdelemaI2(int index, short value)
		{
			Assert.Equal(Int16Tests.Ldelema(index, value), Run<short>("Mosa.UnitTest.Collection.Int16Tests.Ldelema", index, value));
		}
	}
}
