﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Xunit;

namespace Mosa.UnitTest.Collection.xUnit
{
	public class OptimizationFixture : TestFixture
	{
		[Fact]
		public void OptimizationTest1()
		{
			Assert.Equal(Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest1(), Run<bool>("Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest1"));
		}

		[Fact]
		public void OptimizationTest2()
		{
			Assert.Equal(Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest2(), Run<bool>("Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest2"));
		}

		[Fact]
		public void OptimizationTest3()
		{
			Assert.Equal(Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest3(), Run<bool>("Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest3"));
		}

		[Fact]
		public void OptimizationTest4()
		{
			Assert.Equal(Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest4(), Run<bool>("Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest4"));
		}

		[Fact]
		public void OptimizationTest5()
		{
			Assert.Equal(Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest5(), Run<bool>("Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest5"));
		}

		[Fact]
		public void OptimizationTest6()
		{
			Assert.Equal(Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest6(), Run<bool>("Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest6"));
		}

		[Fact]
		public void OptimizationTest7()
		{
			Assert.Equal(Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest7(), Run<bool>("Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest7"));
		}

		[Fact]
		public void OptimizationTest8()
		{
			Assert.Equal(Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest8(), Run<bool>("Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest8"));
		}

		[Fact]
		public void OptimizationTest9()
		{
			Assert.Equal(Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest9(), Run<bool>("Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest9"));
		}

		[Fact]
		public void OptimizationTest10()
		{
			Assert.Equal(Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest10(), Run<bool>("Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest10"));
		}

		[Fact]
		public void OptimizationTest11()
		{
			Assert.Equal(Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest11(), Run<bool>("Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest11"));
		}

		[Fact]
		public void OptimizationTest12()
		{
			Assert.Equal(Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest12(), Run<int>("Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest12"));
		}

		[Fact]
		public void ConditionalConstantPropagation1()
		{
			Assert.Equal(Mosa.UnitTest.Collection.OptimizationTest.ConditionalConstantPropagation1(), Run<int>("Mosa.UnitTest.Collection.OptimizationTest.ConditionalConstantPropagation1"));
		}

		[Fact]
		public void ConditionalConstantPropagation2()
		{
			Assert.Equal(Mosa.UnitTest.Collection.OptimizationTest.ConditionalConstantPropagation2(), Run<int>("Mosa.UnitTest.Collection.OptimizationTest.ConditionalConstantPropagation2"));
		}

		[Theory]
		[MemberData("I4", DisableDiscoveryEnumeration = true)]
		public void OptimizationTest13(int a)
		{
			Assert.Equal(Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest13(a), Run<int>("Mosa.UnitTest.Collection.OptimizationTest.OptimizationTest13", a));
		}
	}
}
