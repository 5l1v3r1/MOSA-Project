/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (<mailto:sharpos@michaelruck.de>)
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace Mosa.Runtime.CompilerFramework {
    /// <summary>
    /// 
    /// </summary>
	public sealed class InstructionStream : Stream {

		#region Types

        /// <summary>
        /// 
        /// </summary>
		[Flags]
		enum MethodFlags : ushort {
            /// <summary>
            /// 
            /// </summary>
			TinyFormat = 0x02,
            /// <summary>
            /// 
            /// </summary>
			FatFormat = 0x03,
            /// <summary>
            /// 
            /// </summary>
			MoreSections = 0x08,
            /// <summary>
            /// 
            /// </summary>
			InitLocals = 0x10,
            /// <summary>
            /// 
            /// </summary>
			CodeSizeMask = 0xF000,
            /// <summary>
            /// 
            /// </summary>
			HeaderMask = 0x0003
		}

        /// <summary>
        /// 
        /// </summary>
		[Flags]
		enum MethodDataSectionType {
            /// <summary>
            /// 
            /// </summary>
			EHTable = 0x01,
            /// <summary>
            /// 
            /// </summary>
			OptIL = 0x02,
            /// <summary>
            /// 
            /// </summary>
			FatFormat = 0x40,
            /// <summary>
            /// 
            /// </summary>
			MoreSections = 0x80
		}

		#endregion // Types

		#region Data members

		/// <summary>
		/// The CIL stream offset.
		/// </summary>
		private long _startOffset;

		/// <summary>
		/// Stream, which holds the il code to decode.
		/// </summary>
		private Stream _stream;

		#endregion // Data members

		#region Construction


        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionStream"/> class.
        /// </summary>
        /// <param name="assemblyStream">The stream, which represents the IL assembly.</param>
        /// <param name="offset">The offset, where the IL stream starts.</param>
		public InstructionStream(Stream assemblyStream, long offset)
		{
			// Check preconditions
			if (null == assemblyStream)
				throw new ArgumentNullException(@"assembly");

			// Store the arguments
			_stream = assemblyStream;
			_startOffset = offset;
			_stream.Position = offset;
		}

		#endregion // Construction

		#region Stream Overrides

        /// <summary>
        /// Ruft beim �berschreiben in einer abgeleiteten Klasse einen Wert ab, der angibt, ob der aktuelle Stream Lesevorg�nge unterst�tzt.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// true, wenn der Stream Lesevorg�nge unterst�tzt, andernfalls false.
        /// </returns>
		public override bool CanRead
		{
			get { return true; }
		}

        /// <summary>
        /// Ruft beim �berschreiben in einer abgeleiteten Klasse einen Wert ab, der angibt, ob der aktuelle Stream Suchvorg�nge unterst�tzt.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// true, wenn der Stream Suchvorg�nge unterst�tzt, andernfalls false.
        /// </returns>
		public override bool CanSeek
		{
			get { return true; }
		}

        /// <summary>
        /// Ruft beim �berschreiben in einer abgeleiteten Klasse einen Wert ab, der angibt, ob der aktuelle Stream Schreibvorg�nge unterst�tzt.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// true, wenn der Stream Schreibvorg�nge unterst�tzt, andernfalls false.
        /// </returns>
		public override bool CanWrite
		{
			get { return false; }
		}

        /// <summary>
        /// L�scht beim �berschreiben in einer abgeleiteten Klasse alle Puffer f�r diesen Stream und veranlasst die Ausgabe aller gepufferten Daten an das zugrunde liegende Ger�t.
        /// </summary>
        /// <exception cref="T:System.IO.IOException">
        /// Ein E/A-Fehler tritt auf.
        /// </exception>
		public override void Flush()
		{
			// Do nothing. We can not flush.
		}

        /// <summary>
        /// Ruft beim �berschreiben in einer abgeleiteten Klasse die L�nge des Streams in Bytes ab.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// Ein Long-Wert, der die L�nge des Streams in Bytes darstellt.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// Eine aus Stream abgeleitete Klasse unterst�tzt keine Suchvorg�nge.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Es wurden Methoden aufgerufen, nachdem der Stream geschlossen wurde.
        /// </exception>
		public override long Length
		{
			get { return _stream.Length; }
		}

        /// <summary>
        /// Ruft beim �berschreiben in einer abgeleiteten Klasse die Position im aktuellen Stream ab oder legt diese fest.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// Die aktuelle Position innerhalb des Streams.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">
        /// Ein E/A-Fehler tritt auf.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// Der Stream unterst�tzt keine Suchvorg�nge.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Es wurden Methoden aufgerufen, nachdem der Stream geschlossen wurde.
        /// </exception>
		public override long Position
		{
			get
			{
				return _stream.Position;
			}
			set
			{
				if (0 > value)
					throw new ArgumentOutOfRangeException(@"value");

				_stream.Position = value;
			}
		}

        /// <summary>
        /// Liest beim �berschreiben in einer abgeleiteten Klasse eine Folge von Bytes aus dem aktuellen Stream und erh�ht die Position im Stream um die Anzahl der gelesenen Bytes.
        /// </summary>
        /// <param name="buffer">Ein Bytearray. Nach dem Beenden dieser Methode enth�lt der Puffer das angegebene Bytearray mit den Werten zwischen <paramref name="offset"/> und (<paramref name="offset"/> + <paramref name="count"/> - 1), die durch aus der aktuellen Quelle gelesene Bytes ersetzt wurden.</param>
        /// <param name="offset">Der nullbasierte Byteoffset im <paramref name="buffer"/>, ab dem die aus dem aktuellen Stream gelesenen Daten gespeichert werden.</param>
        /// <param name="count">Die maximale Anzahl an Bytes, die aus dem aktuellen Stream gelesen werden sollen.</param>
        /// <returns>
        /// Die Gesamtanzahl der in den Puffer gelesenen Bytes. Dies kann weniger als die Anzahl der angeforderten Bytes sein, wenn diese Anzahl an Bytes derzeit nicht verf�gbar ist, oder 0, wenn das Ende des Streams erreicht ist.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// Die Summe aus <paramref name="offset"/> und <paramref name="count"/> ist gr��er als die Pufferl�nge.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="buffer"/> ist null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="offset"/> oder <paramref name="count"/> ist negativ.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        /// Ein E/A-Fehler tritt auf.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// Der Stream unterst�tzt keine Lesevorg�nge.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Es wurden Methoden aufgerufen, nachdem der Stream geschlossen wurde.
        /// </exception>
		public override int Read(byte[] buffer, int offset, int count)
		{
			// Check preconditions
			if (null == buffer)
				throw new ArgumentNullException(@"buffer");

			return _stream.Read(buffer, offset, count);
		}

        /// <summary>
        /// Legt beim �berschreiben in einer abgeleiteten Klasse die Position im aktuellen Stream fest.
        /// </summary>
        /// <param name="offset">Ein Byteoffset relativ zum <paramref name="origin"/>-Parameter.</param>
        /// <param name="origin">Ein Wert vom Typ <see cref="T:System.IO.SeekOrigin"/>, der den Bezugspunkt angibt, von dem aus die neue Position ermittelt wird.</param>
        /// <returns>
        /// Die neue Position innerhalb des aktuellen Streams.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">
        /// Ein E/A-Fehler tritt auf.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// Der Stream unterst�tzt keine Suchvorg�nge. Dies ist beispielsweise der Fall, wenn der Stream aus einer Pipe- oder Konsolenausgabe erstellt wird.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Es wurden Methoden aufgerufen, nachdem der Stream geschlossen wurde.
        /// </exception>
		public override long Seek(long offset, SeekOrigin origin)
		{
			// FIXME: Fix the seeking...
			return _stream.Seek(offset, origin);
		}

        /// <summary>
        /// Legt beim �berschreiben in einer abgeleiteten Klasse die L�nge des aktuellen Streams fest.
        /// </summary>
        /// <param name="value">Die gew�nschte L�nge des aktuellen Streams in Bytes.</param>
        /// <exception cref="T:System.IO.IOException">
        /// Ein E/A-Fehler tritt auf.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// Der Stream unterst�tzt nicht sowohl Lese- als auch Schreibvorg�nge. Dies ist beispielsweise der Fall, wenn der Stream aus einer Pipe- oder Konsolenausgabe erstellt wird.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Es wurden Methoden aufgerufen, nachdem der Stream geschlossen wurde.
        /// </exception>
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

        /// <summary>
        /// Schreibt beim �berschreiben in einer abgeleiteten Klasse eine Folge von Bytes in den aktuellen Stream und erh�ht die aktuelle Position im Stream um die Anzahl der geschriebenen Bytes.
        /// </summary>
        /// <param name="buffer">Ein Bytearray. Diese Methode kopiert <paramref name="count"/> Bytes aus dem <paramref name="buffer"/> in den aktuellen Stream.</param>
        /// <param name="offset">Der nullbasierte Byteoffset im <paramref name="buffer"/>, ab dem Bytes in den aktuellen Stream kopiert werden.</param>
        /// <param name="count">Die Anzahl an Bytes, die in den aktuellen Stream geschrieben werden sollen.</param>
        /// <exception cref="T:System.ArgumentException">
        /// Die Summe aus <paramref name="offset"/> und <paramref name="count"/> ist gr��er als die Pufferl�nge.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="buffer"/> ist null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="offset"/> oder <paramref name="count"/> ist negativ.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        /// Ein E/A-Fehler tritt auf.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// Der Stream unterst�tzt keine Schreibvorg�nge.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Es wurden Methoden aufgerufen, nachdem der Stream geschlossen wurde.
        /// </exception>
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		#endregion // #region Stream Overrides
	}
}
