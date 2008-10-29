﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Alex Lyman (<mailto:mail.alex.lyman@gmail.com>)
 *  Simon Wollwage (<mailto:rootnode@mosa-project.org>)
 *  Michael Ruck (<mailto:sharpos@michaelruck.de>)
 *  Kai P. Reisert (<mailto:kpreisert@googlemail.com>)
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using MbUnit.Framework;

namespace Test.Mosa.Runtime.CompilerFramework.IL
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class Mul : CodeDomTestRunner
    {
        private static string CreateTestCode(string name, string typeIn, string typeOut)
        {
            return @"
                static class Test
                {
                    static bool " + name + "(" + typeOut + " expect, " + typeIn + " a, " + typeIn + @" b)
                    {
                        return expect == (a * b);
                    }
                }";
        }
        
        private static string CreateConstantTestCode(string name, string typeIn, string typeOut, string constLeft, string constRight)
        {
            if (String.IsNullOrEmpty(constRight))
            {
                return @"
                    static class Test
                    {
                        static bool " + name + "(" + typeOut + " expect, " + typeIn + @" x)
                        {
                            return expect == (" + constLeft + @" * x);
                        }
                    }";
            }
            else if (String.IsNullOrEmpty(constLeft))
            {
                return @"
                    static class Test
                    {
                        static bool " + name + "(" + typeOut + " expect, " + typeIn + @" x)
                        {
                            return expect == (x * " + constRight + @");
                        }
                    }";
            }
            else
            {
                throw new NotSupportedException();
            }
        }
        
        #region C
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool C_C_C([MarshalAs(UnmanagedType.U2)]char expect, [MarshalAs(UnmanagedType.U2)]char a, [MarshalAs(UnmanagedType.U2)]char b);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(0, 0)]
        [Row(17, 128)]
        [Row('a', 'Z')]
        [Row(char.MinValue, char.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulC(char a, char b)
        {
            CodeSource = CreateTestCode("MulC", "char", "char");
            Assert.IsTrue((bool)Run<C_C_C>("", "Test", "MulC", (char)(a * b), a, b));
        }
        
        delegate bool C_Constant_C([MarshalAs(UnmanagedType.U2)]char expect, [MarshalAs(UnmanagedType.U2)]char x);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(0, 1)]
        [Row(17, 128)]
        [Row('a', 'Z')]
        [Row(char.MinValue, char.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantCRight(char a, char b)
        {
            CodeSource = CreateConstantTestCode("MulConstantCRight", "char", "char", null, "'" + b.ToString() + "'");
            Assert.IsTrue((bool)Run<C_Constant_C>("", "Test", "MulConstantCRight", (char)(a * b), a));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1, 0)]
        [Row(17, 128)]
        [Row('a', 'Z')]
        [Row(1, char.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantCLeft(char a, char b)
        {
            CodeSource = CreateConstantTestCode("MulConstantCLeft", "char", "char", "'" + a.ToString() + "'", null);
            Assert.IsTrue((bool)Run<C_Constant_C>("", "Test", "MulConstantCLeft", (char)(a * b), b));
        }
        #endregion
        
        #region I1
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool I4_I1_I1(int expect, sbyte a, sbyte b);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1, 2)]
        [Row(23, 21)]
        [Row(1, -2)]
        [Row(-1, 2)]
        [Row(0, 0)]
        [Row(-17, -2)]
        // And reverse
        [Row(2, 1)]
        [Row(21, 23)]
        [Row(-2, 1)]
        [Row(2, -1)]
        [Row(-2, -17)]
        // (MinValue, X) Cases
        [Row(sbyte.MinValue, 0)]
        [Row(sbyte.MinValue, 1)]
        [Row(sbyte.MinValue, 17)]
        [Row(sbyte.MinValue, 123)]
        [Row(sbyte.MinValue, -0)]
        [Row(sbyte.MinValue, -1)]
        [Row(sbyte.MinValue, -17)]
        [Row(sbyte.MinValue, -123)]
        // (MaxValue, X) Cases
        [Row(sbyte.MaxValue, 0)]
        [Row(sbyte.MaxValue, 1)]
        [Row(sbyte.MaxValue, 17)]
        [Row(sbyte.MaxValue, 123)]
        [Row(sbyte.MaxValue, -0)]
        [Row(sbyte.MaxValue, -1)]
        [Row(sbyte.MaxValue, -17)]
        [Row(sbyte.MaxValue, -123)]
        // (X, MinValue) Cases
        [Row(0, sbyte.MinValue)]
        [Row(1, sbyte.MinValue)]
        [Row(17, sbyte.MinValue)]
        [Row(123, sbyte.MinValue)]
        [Row(-0, sbyte.MinValue)]
        [Row(-1, sbyte.MinValue)]
        [Row(-17, sbyte.MinValue)]
        [Row(-123, sbyte.MinValue)]
        // (X, MaxValue) Cases
        [Row(0, sbyte.MaxValue)]
        [Row(1, sbyte.MaxValue)]
        [Row(17, sbyte.MaxValue)]
        [Row(123, sbyte.MaxValue)]
        [Row(-0, sbyte.MaxValue)]
        [Row(-1, sbyte.MaxValue)]
        [Row(-17, sbyte.MaxValue)]
        [Row(-123, sbyte.MaxValue)]
        // Extremvaluecases
        [Row(sbyte.MinValue, sbyte.MaxValue)]
        [Row(sbyte.MaxValue, sbyte.MinValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void MulI1(sbyte a, sbyte b)
        {
            CodeSource = CreateTestCode("MulI1", "sbyte", "int");
            Assert.IsTrue((bool)Run<I4_I1_I1>("", "Test", "MulI1", a * b, a, b));
        }
        
        delegate bool I4_Constant_I1(int expect, sbyte x); 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(23, 21)]
        [Row(2, -17)]
        [Row(0, 0)]
        [Row(sbyte.MinValue, sbyte.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantI1Right(sbyte a, sbyte b)
        {
            CodeSource = CreateConstantTestCode("MulConstantI1Right", "sbyte", "int", null, b.ToString());
            Assert.IsTrue((bool)Run<I4_Constant_I1>("", "Test", "MulConstantI1Right", (a * b), a));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(23, 21)]
        [Row(2, -17)]
        [Row(0, 0)]
        [Row(sbyte.MinValue, sbyte.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantI1Left(sbyte a, sbyte b)
        {
            CodeSource = CreateConstantTestCode("MulConstantI1Left", "sbyte", "int", a.ToString(), null);
            Assert.IsTrue((bool)Run<I4_Constant_I1>("", "Test", "MulConstantI1Left", (a * b), b));
        }
        #endregion

        #region U1
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool U4_U1_U1(uint expect, byte a, byte b);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1, 2)]
        [Row(23, 21)]
        [Row(0, 0)]
        // And reverse
        [Row(2, 1)]
        [Row(21, 23)]
        // (MinValue, X) Cases
        [Row(byte.MinValue, 0)]
        [Row(byte.MinValue, 1)]
        [Row(byte.MinValue, 17)]
        [Row(byte.MinValue, 123)]
        // (MaxValue, X) Cases
        [Row(byte.MaxValue, 0)]
        [Row(byte.MaxValue, 1)]
        [Row(byte.MaxValue, 17)]
        [Row(byte.MaxValue, 123)]
        // (X, MinValue) Cases
        [Row(0, byte.MinValue)]
        [Row(1, byte.MinValue)]
        [Row(17, byte.MinValue)]
        [Row(123, byte.MinValue)]
        // (X, MaxValue) Cases
        [Row(0, byte.MaxValue)]
        [Row(1, byte.MaxValue)]
        [Row(17, byte.MaxValue)]
        [Row(123, byte.MaxValue)]
        // Extremvaluecases
        [Row(byte.MinValue, byte.MaxValue)]
        [Row(byte.MaxValue, byte.MinValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void MulU1(byte a, byte b)
        {
            CodeSource = CreateTestCode("MulU1", "byte", "uint");
            Assert.IsTrue((bool)Run<U4_U1_U1>("", "Test", "MulU1", (uint)(a * b), a, b));
        }
        
        delegate bool U4_Constant_U1(uint expect, byte x); 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(23, 21)]
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(byte.MinValue, byte.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantU1Right(byte a, byte b)
        {
            CodeSource = CreateConstantTestCode("MulConstantU1Right", "byte", "uint", null, b.ToString());
            Assert.IsTrue((bool)Run<U4_Constant_U1>("", "Test", "MulConstantU1Right", (uint)(a * b), a));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(23, 21)]
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(byte.MinValue, byte.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantU1Left(byte a, byte b)
        {
            CodeSource = CreateConstantTestCode("MulConstantU1Left", "byte", "uint", a.ToString(), null);
            Assert.IsTrue((bool)Run<U4_Constant_U1>("", "Test", "MulConstantU1Left", (uint)(a * b), b));
        }
        #endregion

        #region I2
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool I4_I2_I2(int expect, short a, short b);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1, 2)]
        [Row(23, 21)]
        [Row(1, -2)]
        [Row(-1, 2)]
        [Row(0, 0)]
        [Row(-17, -2)]
        // And reverse
        [Row(2, 1)]
        [Row(21, 23)]
        [Row(-2, 1)]
        [Row(2, -1)]
        [Row(-2, -17)]
        // (MinValue, X) Cases
        [Row(short.MinValue, 0)]
        [Row(short.MinValue, 1)]
        [Row(short.MinValue, 17)]
        [Row(short.MinValue, 123)]
        [Row(short.MinValue, -0)]
        [Row(short.MinValue, -1)]
        [Row(short.MinValue, -17)]
        [Row(short.MinValue, -123)]
        // (MaxValue, X) Cases
        [Row(short.MaxValue, 0)]
        [Row(short.MaxValue, 1)]
        [Row(short.MaxValue, 17)]
        [Row(short.MaxValue, 123)]
        [Row(short.MaxValue, -0)]
        [Row(short.MaxValue, -1)]
        [Row(short.MaxValue, -17)]
        [Row(short.MaxValue, -123)]
        // (X, MinValue) Cases
        [Row(0, short.MinValue)]
        [Row(1, short.MinValue)]
        [Row(17, short.MinValue)]
        [Row(123, short.MinValue)]
        [Row(-0, short.MinValue)]
        [Row(-1, short.MinValue)]
        [Row(-17, short.MinValue)]
        [Row(-123, short.MinValue)]
        // (X, MaxValue) Cases
        [Row(0, short.MaxValue)]
        [Row(1, short.MaxValue)]
        [Row(17, short.MaxValue)]
        [Row(123, short.MaxValue)]
        [Row(-0, short.MaxValue)]
        [Row(-1, short.MaxValue)]
        [Row(-17, short.MaxValue)]
        [Row(-123, short.MaxValue)]
        // Extremvaluecases
        [Row(short.MinValue, short.MaxValue)]
        [Row(short.MaxValue, short.MinValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void MulI2(short a, short b)
        {
            CodeSource = CreateTestCode("MulI2", "short", "int");
            Assert.IsTrue((bool)Run<I4_I2_I2>("", "Test", "MulI2", (a * b), a, b));
        }
        
        delegate bool I4_Constant_I2(int expect, short x); 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(-23, 21)]
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(short.MinValue, short.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantI2Right(short a, short b)
        {
            CodeSource = CreateConstantTestCode("MulConstantI2Right", "short", "int", null, b.ToString());
            Assert.IsTrue((bool)Run<I4_Constant_I2>("", "Test", "MulConstantI2Right", (a * b), a));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(-23, 21)]
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(short.MinValue, short.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantI2Left(short a, short b)
        {
            CodeSource = CreateConstantTestCode("MulConstantI2Left", "short", "int", a.ToString(), null);
            Assert.IsTrue((bool)Run<I4_Constant_I2>("", "Test", "MulConstantI2Left", (a * b), b));
        }
        #endregion

        #region U2
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool U4_U2_U2(uint expect, ushort a, ushort b);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1, 2)]
        [Row(23, 21)]
        [Row(0, 0)]
        // And reverse
        [Row(2, 1)]
        [Row(21, 23)]
        // (MinValue, X) Cases
        [Row(ushort.MinValue, 0)]
        [Row(ushort.MinValue, 1)]
        [Row(ushort.MinValue, 17)]
        [Row(ushort.MinValue, 123)]
        // (MaxValue, X) Cases
        [Row(ushort.MaxValue, 0)]
        [Row(ushort.MaxValue, 1)]
        [Row(ushort.MaxValue, 17)]
        [Row(ushort.MaxValue, 123)]
        // (X, MinValue) Cases
        [Row(0, ushort.MinValue)]
        [Row(1, ushort.MinValue)]
        [Row(17, ushort.MinValue)]
        [Row(123, ushort.MinValue)]
        // (X, MaxValue) Cases
        [Row(0, ushort.MaxValue)]
        [Row(1, ushort.MaxValue)]
        [Row(17, ushort.MaxValue)]
        [Row(123, ushort.MaxValue)]
        // Extremvaluecases
        [Row(ushort.MinValue, ushort.MaxValue)]
        [Row(ushort.MaxValue, ushort.MinValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void MulU2(ushort a, ushort b)
        {
            CodeSource = CreateTestCode("MulU2", "ushort", "uint");
            Assert.IsTrue((bool)Run<U4_U2_U2>("", "Test", "MulU2", (uint)(a * b), a, b));
        }
        
        delegate bool U4_Constant_U2(uint expect, ushort x); 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(23, 21)]
        //[Row(23, 148)] FIXME: Uncommenting this crashes the testrunner
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(ushort.MinValue, ushort.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantU2Right(ushort a, ushort b)
        {
            CodeSource = CreateConstantTestCode("MulConstantU2Right", "ushort", "uint", null, b.ToString());
            Assert.IsTrue((bool)Run<U4_Constant_U2>("", "Test", "MulConstantU2Right", (uint)(a * b), a));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(23, 21)]
        //[Row(23, 148)] FIXME: Uncommenting this crashes the testrunner
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(ushort.MinValue, ushort.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantU2Left(ushort a, ushort b)
        {
            CodeSource = CreateConstantTestCode("MulConstantU2Left", "ushort", "uint", a.ToString(), null);
            Assert.IsTrue((bool)Run<U4_Constant_U2>("", "Test", "MulConstantU2Left", (uint)(a * b), b));
        }
        #endregion

        #region I4
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool I4_I4_I4(int expect, int a, int b);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1, 2)]
        [Row(23, 21)]
        [Row(1, -2)]
        [Row(-1, 2)]
        [Row(0, 0)]
        [Row(-17, -2)]
        // And reverse
        [Row(2, 1)]
        [Row(21, 23)]
        [Row(-2, 1)]
        [Row(2, -1)]
        [Row(-2, -17)]
        // (MinValue, X) Cases
        [Row(int.MinValue, 0)]
        [Row(int.MinValue, 1)]
        [Row(int.MinValue, 17)]
        [Row(int.MinValue, 123)]
        [Row(int.MinValue, -0)]
        [Row(int.MinValue, -1)]
        [Row(int.MinValue, -17)]
        [Row(int.MinValue, -123)]
        // (MaxValue, X) Cases
        [Row(int.MaxValue, 0)]
        [Row(int.MaxValue, 1)]
        [Row(int.MaxValue, 17)]
        [Row(int.MaxValue, 123)]
        [Row(int.MaxValue, -0)]
        [Row(int.MaxValue, -1)]
        [Row(int.MaxValue, -17)]
        [Row(int.MaxValue, -123)]
        // (X, MinValue) Cases
        [Row(0, int.MinValue)]
        [Row(1, int.MinValue)]
        [Row(17, int.MinValue)]
        [Row(123, int.MinValue)]
        [Row(-0, int.MinValue)]
        [Row(-1, int.MinValue)]
        [Row(-17, int.MinValue)]
        [Row(-123, int.MinValue)]
        // (X, MaxValue) Cases
        [Row(0, int.MaxValue)]
        [Row(1, int.MaxValue)]
        [Row(17, int.MaxValue)]
        [Row(123, int.MaxValue)]
        [Row(-0, int.MaxValue)]
        [Row(-1, int.MaxValue)]
        [Row(-17, int.MaxValue)]
        [Row(-123, int.MaxValue)]
        // Extremvaluecases
        [Row(int.MinValue, int.MaxValue)]
        [Row(int.MaxValue, int.MinValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void MulI4(int a, int b)
        {
            CodeSource = CreateTestCode("MulI4", "int", "int");
            Assert.IsTrue((bool)Run<I4_I4_I4>("", "Test", "MulI4", (a * b), a, b));
        }
        
        delegate bool I4_Constant_I4(int expect, int x); 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(-23, 21)]
        //[Row(-23, 148)] FIXME: Uncommenting this crashes the testrunner
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(int.MinValue, int.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantI4Right(int a, int b)
        {
            CodeSource = CreateConstantTestCode("MulConstantI4Right", "int", "int", null, b.ToString());
            Assert.IsTrue((bool)Run<I4_Constant_I4>("", "Test", "MulConstantI4Right", (a * b), a));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(-23, 21)]
        //[Row(-23, 148)] FIXME: Uncommenting this crashes the testrunner
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(int.MinValue, int.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantI4Left(int a, int b)
        {
            CodeSource = CreateConstantTestCode("MulConstantI4Left", "int", "int", a.ToString(), null);
            Assert.IsTrue((bool)Run<I4_Constant_I4>("", "Test", "MulConstantI4Left", (a * b), b));
        }
        #endregion

        #region U4
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool U4_U4_U4(uint expect, uint a, uint b);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1, 2)]
        [Row(23, 21)]
        [Row(0, 0)]
        // And reverse
        [Row(2, 1)]
        [Row(21, 23)]
        // (MinValue, X) Cases
        [Row(uint.MinValue, 0)]
        [Row(uint.MinValue, 1)]
        [Row(uint.MinValue, 17)]
        [Row(uint.MinValue, 123)]
        // (MaxValue, X) Cases
        [Row(uint.MaxValue, 0)]
        [Row(uint.MaxValue, 1)]
        [Row(uint.MaxValue, 17)]
        [Row(uint.MaxValue, 123)]
        // (X, MinValue) Cases
        [Row(0, uint.MinValue)]
        [Row(1, uint.MinValue)]
        [Row(17, uint.MinValue)]
        [Row(123, uint.MinValue)]
        // (X, MaxValue) Cases
        [Row(0, uint.MaxValue)]
        [Row(1, uint.MaxValue)]
        [Row(17, uint.MaxValue)]
        [Row(123, uint.MaxValue)]
        // Extremvaluecases
        [Row(uint.MinValue, uint.MaxValue)]
        [Row(uint.MaxValue, uint.MinValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void MulU4(uint a, uint b)
        {
            CodeSource = CreateTestCode("MulU4", "uint", "uint");
            Assert.IsTrue((bool)Run<U4_U4_U4>("", "Test", "MulU4", (uint)(a * b), a, b));
        }
        
        delegate bool U4_Constant_U4(uint expect, uint x); 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(23, 21)]
        //[Row(23, 148)] FIXME: Uncommenting this crashes the testrunner
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(uint.MinValue, uint.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantU4Right(uint a, uint b)
        {
            CodeSource = CreateConstantTestCode("MulConstantU4Right", "uint", "uint", null, b.ToString());
            Assert.IsTrue((bool)Run<U4_Constant_U4>("", "Test", "MulConstantU4Right", (uint)(a * b), a));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(23, 21)]
        //[Row(23, 148)] FIXME: Uncommenting this crashes the testrunner
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(uint.MinValue, uint.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantU4Left(uint a, uint b)
        {
            CodeSource = CreateConstantTestCode("MulConstantU4Left", "uint", "uint", a.ToString(), null);
            Assert.IsTrue((bool)Run<U4_Constant_U4>("", "Test", "MulConstantU4Left", (uint)(a * b), b));
        }
        #endregion
         
        #region I8
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool I8_I8_I8(long expect, long a, long b);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1, 2)]
        [Row(23, 21)]
        [Row(1, -2)]
        [Row(-1, 2)]
        [Row(0, 0)]
        [Row(-17, -2)]
        // And reverse
        [Row(2, 1)]
        [Row(21, 23)]
        [Row(-2, 1)]
        [Row(2, -1)]
        [Row(-2, -17)]
        // (MinValue, X) Cases
        [Row(long.MinValue, 0)]
        [Row(long.MinValue, 1)]
        [Row(long.MinValue, 17)]
        [Row(long.MinValue, 123)]
        [Row(long.MinValue, -0)]
        [Row(long.MinValue, -1)]
        [Row(long.MinValue, -17)]
        [Row(long.MinValue, -123)]
        // (MaxValue, X) Cases
        [Row(long.MaxValue, 0)]
        [Row(long.MaxValue, 1)]
        [Row(long.MaxValue, 17)]
        [Row(long.MaxValue, 123)]
        [Row(long.MaxValue, -0)]
        [Row(long.MaxValue, -1)]
        [Row(long.MaxValue, -17)]
        [Row(long.MaxValue, -123)]
        // (X, MinValue) Cases
        [Row(0, long.MinValue)]
        [Row(1, long.MinValue)]
        [Row(17, long.MinValue)]
        [Row(123, long.MinValue)]
        [Row(-0, long.MinValue)]
        [Row(-1, long.MinValue)]
        [Row(-17, long.MinValue)]
        [Row(-123, long.MinValue)]
        // (X, MaxValue) Cases
        [Row(0, long.MaxValue)]
        [Row(1, long.MaxValue)]
        [Row(17, long.MaxValue)]
        [Row(123, long.MaxValue)]
        [Row(-0, long.MaxValue)]
        [Row(-1, long.MaxValue)]
        [Row(-17, long.MaxValue)]
        [Row(-123, long.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void MulI8(long a, long b)
        {
            CodeSource = CreateTestCode("MulI8", "long", "long");
            Assert.IsTrue((bool)Run<I8_I8_I8>("", "Test", "MulI8", (a * b), a, b));
        }
        
        delegate bool I8_Constant_I8(long expect, long x);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(-23, 148)]
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(-123, long.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantI8Right(long a, long b)
        {
            CodeSource = CreateConstantTestCode("MulConstantI8Right", "long", "long", null, b.ToString());
            Assert.IsTrue((bool)Run<I8_Constant_I8>("", "Test", "MulConstantI8Right", (a * b), a));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(-23, 148)]
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(-123, long.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantI8Left(long a, long b)
        {
            CodeSource = CreateConstantTestCode("MulConstantI8Left", "long", "long", a.ToString(), null);
            Assert.IsTrue((bool)Run<I8_Constant_I8>("", "Test", "MulConstantI8Left", (a * b), b));
        }
        #endregion

        #region U8
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool U8_U8_U8(ulong expect, ulong a, ulong b);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1, 2)]
        [Row(23, 21)]
        [Row(0, 0)]
        // And reverse
        [Row(2, 1)]
        [Row(21, 23)]
        // (MinValue, X) Cases
        [Row(ulong.MinValue, 0)]
        [Row(ulong.MinValue, 1)]
        [Row(ulong.MinValue, 17)]
        [Row(ulong.MinValue, 123)]
        // (MaxValue, X) Cases
        [Row(ulong.MaxValue, 0)]
        [Row(ulong.MaxValue, 1)]
        [Row(ulong.MaxValue, 17)]
        [Row(ulong.MaxValue, 123)]
        // (X, MinValue) Cases
        [Row(0, ulong.MinValue)]
        [Row(1, ulong.MinValue)]
        [Row(17, ulong.MinValue)]
        [Row(123, ulong.MinValue)]
        // (X, MaxValue) Cases
        [Row(0, ulong.MaxValue)]
        [Row(1, ulong.MaxValue)]
        [Row(17, ulong.MaxValue)]
        [Row(123, ulong.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void MulU8(ulong a, ulong b)
        {
            CodeSource = CreateTestCode("MulU8", "ulong", "ulong");
            Assert.IsTrue((bool)Run<U8_U8_U8>("", "Test", "MulU8", (ulong)(a * b), a, b));
        }
        
        delegate bool U8_Constant_U8(ulong expect, ulong x);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(23, 148)]
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(1, ulong.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantU8Right(ulong a, ulong b)
        {
            CodeSource = CreateConstantTestCode("MulConstantU8Right", "ulong", "ulong", null, b.ToString());
            Assert.IsTrue((bool)Run<U8_Constant_U8>("", "Test", "MulConstantU8Right", (ulong)(a * b), a));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(23, 148)]
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(1, ulong.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantU8Left(ulong a, ulong b)
        {
            CodeSource = CreateConstantTestCode("MulConstantU8Left", "ulong", "ulong", a.ToString(), null);
            // left side constant
            CodeSource = "static class Test { static bool MulConstantU8Left(ulong expect, ulong b) { return expect == (" + a.ToString() + " * b); } }";
            Assert.IsTrue((bool)Run<U8_Constant_U8>("", "Test", "MulConstantU8Left", (ulong)(a * b), b));
        }
        #endregion
        
        #region R4
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool R4_R4_R4(float expect, float a, float b);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1.0f, 2.0f)]
        [Row(2.0f, 0.0f)]
        [Row(1.0f, float.NaN)]
        [Row(float.NaN, 1.0f)]
        [Row(1.0f, float.PositiveInfinity)]
        [Row(float.PositiveInfinity, 1.0f)]
        [Row(1.0f, float.NegativeInfinity)]
        [Row(float.NegativeInfinity, 1.0f)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void MulR4(float a, float b)
        {
            CodeSource = CreateTestCode("MulR4", "float", "float");
            Assert.IsTrue((bool)Run<R4_R4_R4>("", "Test", "MulR4", (a * b), a, b));
        }
        
        delegate bool R4_Constant_R4(float expect, float x);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(23f, 148.0016f)]
        [Row(17.2f, 1f)]
        [Row(0f, 0f)]
        [Row(float.MinValue, float.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantR4Right(float a, float b)
        {
            CodeSource = CreateConstantTestCode("MulConstantR4Right", "float", "float", null, b.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");
            Assert.IsTrue((bool)Run<R4_Constant_R4>("", "Test", "MulConstantR4Right", (a * b), a));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(23f, 148.0016f)]
        [Row(17.2f, 1f)]
        [Row(0f, 0f)]
        [Row(float.MinValue, float.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantR4Left(float a, float b)
        {
            CodeSource = CreateConstantTestCode("MulConstantR4Left", "float", "float", a.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f", null);
            Assert.IsTrue((bool)Run<R4_Constant_R4>("", "Test", "MulConstantR4Left", (a * b), b));
        }
        #endregion
        
        #region R8
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool R8_R8_R8(double expect, double a, double b);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1.0, 2.0)]
        [Row(2.0, 0.0)]
        [Row(1.0, double.NaN)]
        [Row(double.NaN, 1.0)]
        [Row(1.0, double.PositiveInfinity)]
        [Row(double.PositiveInfinity, 1.0)]
        [Row(1.0, double.NegativeInfinity)]
        [Row(double.NegativeInfinity, 1.0)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void MulR8(double a, double b)
        {
            CodeSource = CreateTestCode("MulR8", "double", "double");
            Assert.IsTrue((bool)Run<R8_R8_R8>("", "Test", "MulR8", (a * b), a, b));
        }
        
        delegate bool R8_Constant_R8(double expect, double x);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(23, 148.0016)]
        [Row(17.2, 1.0)]
        [Row(0.0, 0.0)]
        [Row(-1.79769313486231E+308, 1.79769313486231E+308)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantR8Right(double a, double b)
        {
            CodeSource = CreateConstantTestCode("MulConstantR8Right", "double", "double", null, b.ToString(System.Globalization.CultureInfo.InvariantCulture));
            Assert.IsTrue((bool)Run<R8_Constant_R8>("", "Test", "MulConstantR8Right", (a * b), a));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(23, 148.0016)]
        [Row(17.2, 1.0)]
        [Row(0.0, 0.0)]
        [Row(-1.79769313486231E+308, 1.79769313486231E+308)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void MulConstantR8Left(double a, double b)
        {
            CodeSource = CreateConstantTestCode("MulConstantR8Left", "double", "double", a.ToString(System.Globalization.CultureInfo.InvariantCulture), null);
            Assert.IsTrue((bool)Run<R8_Constant_R8>("", "Test", "MulConstantR8Left", (a * b), b));
        }
        #endregion
    }
}
