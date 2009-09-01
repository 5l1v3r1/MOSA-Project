﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Alex Lyman (<mailto:mail.alex.lyman@gmail.com>)
 *  Simon Wollwage (<mailto:kintaro@think-in-co.de>)
 *  
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using NUnit.Framework;

namespace Test.Mosa.Runtime.CompilerFramework.IL
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class Switch : CodeDomTestRunner
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        delegate sbyte I1_I1([MarshalAs(UnmanagedType.I1)]sbyte expect, [MarshalAs(UnmanagedType.I1)]sbyte a);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        [TestCase((sbyte)1)]
        [TestCase((sbyte)23)]
        [TestCase((sbyte)-1)]
        [TestCase((sbyte)0)]
        // And reverse
        [TestCase((sbyte)2)]
        [TestCase((sbyte)-2)]       
        [Test]
        public void SwitchI1(sbyte a)
        {
            CodeSource = @"static class Test { 
                static sbyte SwitchI1(sbyte expect, sbyte a) { return Switch_Target(a); } 
                static sbyte Switch_Target(sbyte a)
                {
                    switch(a)
                    {
                        case 0:
                            return 0;
                            break;
                        case 1:
                            return 1;
                            break;
                        case -1:
                            return -1;
                            break;
                        case 2:
                            return 2;
                            break;
                        case -2:
                            return -2;
                            break;
                        case 23:
                            return 23;
                            break;
                        case sbyte.MinValue:
                            return sbyte.MinValue;
                            break;
                        case sbyte.MaxValue:
                            return sbyte.MaxValue;
                            break;
                        default:
                            return 42;
                            break;
                    }
                }
            }";
            Assert.AreEqual(a, Run<I1_I1>("", "Test", "SwitchI1", a, a));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        delegate bool U1_U1(byte expect, byte a);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        [TestCase((byte)1)]
        [TestCase((byte)23)]
        [TestCase((byte)0)]
        // And reverse
        [TestCase((byte)2)]
        [Test]
        public void SwitchU1(byte a)
        {
            CodeSource = @"static class Test { 
                static bool SwitchU1(byte expect, byte a) { return expect == Switch_Target(a); } 
                static byte Switch_Target(byte a)
                {
                    switch(a)
                    {
                        case 0:
                            return 0;
                            break;
                        case 1:
                            return 1;
                            break;
                        case 2:
                            return 2;
                            break;
                        case 23:
                            return 23;
                            break;
                        case byte.MaxValue:
                            return byte.MaxValue;
                            break;
                        default:
                            return 42;
                            break;
                    }
                }
            }";
            Assert.IsTrue((bool)Run<U1_U1>("", "Test", "SwitchU1", a, a));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        delegate bool I2_I2(short expect, short a);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        [TestCase((short)1)]
        [TestCase((short)23)]
        [TestCase((short)-1)]
        [TestCase((short)0)]
        // And reverse
        [TestCase((short)2)]
        [TestCase((short)-2)]
        // (MinValue, X) Cases
        [TestCase(short.MinValue)]
        // (MaxValue, X) Cases
        [TestCase(short.MaxValue)]
        [Test]
        public void SwitchI2(short a)
        {
            CodeSource = @"static class Test { 
                static bool SwitchI2(short expect, short a) { return expect == Switch_Target(a); } 
                static short Switch_Target(short a)
                {
                    switch(a)
                    {
                        case 0:
                            return 0;
                            break;
                        case 1:
                            return 1;
                            break;
                        case -1:
                            return -1;
                            break;
                        case 2:
                            return 2;
                            break;
                        case -2:
                            return -2;
                            break;
                        case 23:
                            return 23;
                            break;
                        case short.MinValue:
                            return short.MinValue;
                            break;
                        case short.MaxValue:
                            return short.MaxValue;
                            break;
                        default:
                            return 42;
                            break;
                    }
                }
            }";
            Assert.IsTrue((bool)Run<I2_I2>("", "Test", "SwitchI2", a, a));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        delegate bool U2_U2(ushort expect, ushort a);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        [TestCase((ushort)1)]
        [TestCase((ushort)23)]
        [TestCase((ushort)0)]
        // And reverse
        [TestCase((ushort)2)]
        // (MinValue, X) Cases
        [TestCase(ushort.MinValue)]
        // (MaxValue, X) Cases
        [TestCase(ushort.MaxValue)]
        [Test]
        public void SwitchU2(ushort a)
        {
            CodeSource = @"static class Test { 
                static bool SwitchU2(ushort expect, ushort a) { return expect == Switch_Target(a); } 
                static ushort Switch_Target(ushort a)
                {
                    switch(a)
                    {
                        case 0:
                            return 0;
                            break;
                        case 1:
                            return 1;
                            break;
                        case 2:
                            return 2;
                            break;
                        case 23:
                            return 23;
                            break;
                        case ushort.MaxValue:
                            return ushort.MaxValue;
                            break;
                        default:
                            return 42;
                            break;
                    }
                }
            }";
            Assert.IsTrue((bool)Run<U2_U2>("", "Test", "SwitchU2", a, a));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        delegate bool I4_I4(int expect, int a);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        [TestCase(1)]
        [TestCase(23)]
        [TestCase(-1)]
        [TestCase(0)]
        // And reverse
        [TestCase(2)]
        [TestCase(-2)]
        // (MinValue, X) Cases
        [TestCase(int.MinValue)]
        // (MaxValue, X) Cases
        [TestCase(int.MaxValue)]
        [Test]
        public void SwitchI4(int a)
        {
            CodeSource = @"static class Test { 
                static bool SwitchI4(int expect, int a) { return expect == Switch_Target(a); } 
                static int Switch_Target(int a)
                {
                    switch(a)
                    {
                        case 0:
                            return 0;
                            break;
                        case 1:
                            return 1;
                            break;
                        case -1:
                            return -1;
                            break;
                        case 2:
                            return 2;
                            break;
                        case -2:
                            return -2;
                            break;
                        case 23:
                            return 23;
                            break;
                        case int.MinValue:
                            return int.MinValue;
                            break;
                        case int.MaxValue:
                            return int.MaxValue;
                            break;
                        default:
                            return 42;
                            break;
                    }
                }
            }";
            Assert.IsTrue((bool)Run<I4_I4>("", "Test", "SwitchI4", a, a));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        delegate bool U4_U4(uint expect, uint a);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        [TestCase((uint)1)]
        [TestCase((uint)23)]
        [TestCase((uint)0)]
        // And reverse
        [TestCase((uint)2)]
        // (MinValue, X) Cases
        [TestCase(uint.MinValue)]
        // (MaxValue, X) Cases
        [TestCase(uint.MaxValue)]
        [Test]
        public void SwitchU4(uint a)
        {
            CodeSource = @"static class Test { 
                static bool SwitchU4(uint expect, uint a) { return expect == Switch_Target(a); } 
                static uint Switch_Target(uint a)
                {
                    switch(a)
                    {
                        case 0:
                            return 0;
                            break;
                        case 1:
                            return 1;
                            break;
                        case 2:
                            return 2;
                            break;
                        case 23:
                            return 23;
                            break;
                        case uint.MaxValue:
                            return uint.MaxValue;
                            break;
                        default:
                            return 42;
                            break;
                    }
                }
            }";
            Assert.IsTrue((bool)Run<U4_U4>("", "Test", "SwitchU4", a, a));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        delegate bool I8_I8(long expect, long a);
        delegate long I8_I8_R(long expect, long a);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        [TestCase(1)]
        [TestCase(23)]
        [TestCase(-1)]
        [TestCase(0)]
        // And reverse
        [TestCase(2)]
        [TestCase(-2)]
        // (MinValue, X) Cases
        [TestCase(long.MinValue)]
        // (MaxValue, X) Cases
        [TestCase(long.MaxValue)]
        [Test]
        public void SwitchI8(long a)
        {
            CodeSource = @"static class Test { 
                static long SwitchI8(long expect, long a) { return Switch_Target(a); } 
                static long Switch_Target(long a)
                {
                    switch(a)
                    {
                        case 0:
                            return 0;
                            break;
                        case 1:
                            return 1;
                            break;
                        case -1:
                            return -1;
                            break;
                        case 2:
                            return 2;
                            break;
                        case -2:
                            return -2;
                            break;
                        case 23:
                            return 23;
                            break;
                        case long.MinValue:
                            return long.MinValue;
                            break;
                        case long.MaxValue:
                            return long.MaxValue;
                            break;
                        default:
                            return 42;
                            break;
                    }
                }
            }";
            Assert.AreEqual(a, (long)Run<I8_I8_R>("", "Test", "SwitchI8", a, a));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        delegate bool U8_U8(ulong expect, ulong a);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        [TestCase((ulong)1)]
        [TestCase((ulong)23)]
        [TestCase((ulong)0)]
        // And reverse
        [TestCase((ulong)2)]
        // (MinValue, X) Cases
        [TestCase(ulong.MinValue)]
        // (MaxValue, X) Cases
        [TestCase(ulong.MaxValue)]
        [Test]
        public void SwitchU8(ulong a)
        {
            CodeSource = @"static class Test { 
                static bool SwitchU8(ulong expect, ulong a) { return expect == Switch_Target(a); } 
                static ulong Switch_Target(ulong a)
                {
                    switch(a)
                    {
                        case 0:
                            return 0;
                            break;
                        case 1:
                            return 1;
                            break;
                        case 2:
                            return 2;
                            break;
                        case 23:
                            return 23;
                            break;
                        case ulong.MaxValue:
                            return ulong.MaxValue;
                            break;
                        default:
                            return 42;
                            break;
                    }
                }
            }";
            Assert.IsTrue((bool)Run<U8_U8>("", "Test", "SwitchU8", a, a));
        }
    }
}