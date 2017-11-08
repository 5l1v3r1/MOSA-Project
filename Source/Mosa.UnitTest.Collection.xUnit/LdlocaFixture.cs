﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.


using Xunit;
using Xunit.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mosa.UnitTest.Collection.xUnit
{
	public class Ldloca : TestFixture
	{
		
		[Theory]
		[MemberData(nameof(U1), DisableDiscoveryEnumeration = true)]
		public void LdlocaCheckValueU1(byte a)
		{
			Assert.Equal(Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueU1(a), Run<bool>("Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueU1", a));
		}
		
		[Theory]
		[MemberData(nameof(U2), DisableDiscoveryEnumeration = true)]
		public void LdlocaCheckValueU2(ushort a)
		{
			Assert.Equal(Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueU2(a), Run<bool>("Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueU2", a));
		}
		
		[Theory]
		[MemberData(nameof(U4), DisableDiscoveryEnumeration = true)]
		public void LdlocaCheckValueU4(uint a)
		{
			Assert.Equal(Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueU4(a), Run<bool>("Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueU4", a));
		}
		
		[Theory]
		[MemberData(nameof(U8), DisableDiscoveryEnumeration = true)]
		public void LdlocaCheckValueU8(ulong a)
		{
			Assert.Equal(Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueU8(a), Run<bool>("Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueU8", a));
		}
		
		[Theory]
		[MemberData(nameof(I1), DisableDiscoveryEnumeration = true)]
		public void LdlocaCheckValueI1(sbyte a)
		{
			Assert.Equal(Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueI1(a), Run<bool>("Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueI1", a));
		}
		
		[Theory]
		[MemberData(nameof(I2), DisableDiscoveryEnumeration = true)]
		public void LdlocaCheckValueI2(short a)
		{
			Assert.Equal(Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueI2(a), Run<bool>("Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueI2", a));
		}
		
		[Theory]
		[MemberData(nameof(I4), DisableDiscoveryEnumeration = true)]
		public void LdlocaCheckValueI4(int a)
		{
			Assert.Equal(Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueI4(a), Run<bool>("Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueI4", a));
		}
		
		[Theory]
		[MemberData(nameof(I8), DisableDiscoveryEnumeration = true)]
		public void LdlocaCheckValueI8(long a)
		{
			Assert.Equal(Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueI8(a), Run<bool>("Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueI8", a));
		}
		
		[Theory]
		[MemberData(nameof(R4), DisableDiscoveryEnumeration = true)]
		public void LdlocaCheckValueR4(float a)
		{
			Assert.Equal(Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueR4(a), Run<bool>("Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueR4", a));
		}
		
		[Theory]
		[MemberData(nameof(R8), DisableDiscoveryEnumeration = true)]
		public void LdlocaCheckValueR8(double a)
		{
			Assert.Equal(Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueR8(a), Run<bool>("Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueR8", a));
		}
		
		[Theory]
		[MemberData(nameof(C), DisableDiscoveryEnumeration = true)]
		public void LdlocaCheckValueC(char a)
		{
			Assert.Equal(Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueC(a), Run<bool>("Mosa.UnitTest.Collection.LdlocaTests.LdlocaCheckValueC", a));
		}
			}
}
