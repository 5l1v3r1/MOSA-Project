﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 */

using MbUnit.Framework;
using Mosa.Compiler.Linker;
using Mosa.Compiler.Metadata.Loader;
using Mosa.Compiler.TypeSystem;
using Mosa.Test.CodeDomCompiler;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Mosa.Test.System
{
	public class TestCompiler
	{
		#region Data members

		/// <summary>
		/// Holds the type system
		/// </summary>
		private ITypeSystem typeSystem;

		/// <summary>
		///
		/// </summary>
		private CompilerSettings cacheSettings = null;

		/// <summary>
		///
		/// </summary>
		private ILinker linker;

		private static long memoryPtr = 0x21700000; // Location for pointer to allocated memory!
		private static uint memorySize = 1024 * 1024 * 2; // 2Mb
		private long memoryAllocated = 0;

		#endregion Data members

		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="TestCompiler"/> class.
		/// </summary>
		public TestCompiler()
		{
			ResetMemory();
		}

		#endregion Construction

		protected void ResetMemory()
		{
			if (memoryAllocated == 0)
			{
				if (Win32Memory.Allocate(memoryPtr, 1024, PageProtectionFlags.Read | PageProtectionFlags.Write | PageProtectionFlags.WriteCombine) != memoryPtr)
					throw new OutOfMemoryException();

				memoryAllocated = Win32Memory.Allocate(0, memorySize, PageProtectionFlags.Read | PageProtectionFlags.Write | PageProtectionFlags.WriteCombine);

				if (memoryAllocated == 0)
					throw new OutOfMemoryException();
			}

			unsafe
			{
				((uint*)memoryPtr)[0] = (uint)memoryAllocated;
				((uint*)memoryPtr)[1] = (uint)memoryAllocated;
				((uint*)memoryPtr)[2] = memorySize;
			}
		}

		public T Run<T>(CompilerSettings settings, string ns, string type, string method, params object[] parameters)
		{
			CompileTestCode(settings);

			// Find the test method to execute
			RuntimeMethod runtimeMethod = FindMethod(
				ns,
				type,
				method
			);

			Debug.Assert(runtimeMethod != null, runtimeMethod.ToString());

			// Get delegate name
			string delegateName;

			if (default(T) is ValueType)
				delegateName = "Mosa.Test.Prebuilt.Delegates+" + DelegateUtility.GetDelegteName(default(T), parameters);
			else
				delegateName = "Mosa.Test.Prebuilt.Delegates+" + DelegateUtility.GetDelegteName(null, parameters);

			// Get the prebuilt delegate type
			Type delegateType = Prebuilt.GetType(delegateName);

			Debug.Assert(delegateType != null, delegateName);

			LinkerSymbol symbol = linker.GetSymbol(runtimeMethod.FullName);
			LinkerSection section = linker.GetSection(symbol.SectionKind);

			long address = symbol.VirtualAddress; // +section.VirtualAddress;

			// Create a delegate for the test method
			Delegate fn = Marshal.GetDelegateForFunctionPointer(
				new IntPtr(address),
				delegateType
			);

			// Reset Memory
			ResetMemory();

			//Debug.WriteLine("Executing: " + runtimeMethod.FullName);

			// Execute the test method
			object tempResult = fn.DynamicInvoke(parameters);

			//Debug.WriteLine("Done");

			try
			{
				if (default(T) is ValueType)
					return (T)tempResult;
				else
					return default(T);
			}
			catch (InvalidCastException e)
			{
				Assert.Fail(@"Failed to convert result {0} of type {1} to type {2}.", tempResult, tempResult.GetType(), typeof(T));
				throw e;
			}
		}

		// Might not keep this as a public method
		public void CompileTestCode(CompilerSettings settings)
		{
			if (cacheSettings == null || !cacheSettings.IsEqual(settings))
			{
				cacheSettings = new CompilerSettings(settings);

				CompilerResults results = Mosa.Test.CodeDomCompiler.Compiler.ExecuteCompiler(cacheSettings);

				//Console.WriteLine("Executing MOSA compiler...");

				Assert.IsFalse(results.Errors.HasErrors, "Failed to compile source code with native compiler");

				linker = RunMosaCompiler(settings, results.PathToAssembly);
			}
		}

		/// <summary>
		/// Finds a runtime method, which represents the requested method.
		/// </summary>
		/// <exception cref="MissingMethodException">The sought method is not found.</exception>
		/// <param name="ns">The namespace of the sought method.</param>
		/// <param name="type">The type, which contains the sought method.</param>
		/// <param name="method">The method to find.</param>
		/// <returns>An instance of <see cref="RuntimeMethod"/>.</returns>
		private RuntimeMethod FindMethod(string ns, string type, string method)
		{
			foreach (RuntimeType t in typeSystem.GetAllTypes())
			{
				if (t.Name != type)
					continue;

				if (!string.IsNullOrEmpty(ns))
					if (t.Namespace != ns)
						continue;

				foreach (RuntimeMethod m in t.Methods)
				{
					if (m.Name == method)
					{
						return m;
					}
				}

				break;
			}

			throw new MissingMethodException(ns + "." + type, method);
		}

		private TestLinker RunMosaCompiler(CompilerSettings settings, string assemblyFile)
		{
			IAssemblyLoader assemblyLoader = new AssemblyLoader();
			assemblyLoader.InitializePrivatePaths(settings.References);

			assemblyLoader.LoadModule(assemblyFile);

			foreach (string file in settings.References)
			{
				assemblyLoader.LoadModule(file);
			}

			typeSystem = new TypeSystem();
			typeSystem.LoadModules(assemblyLoader.Modules);

			return TestCaseCompiler.Compile(typeSystem);
		}
	}
}