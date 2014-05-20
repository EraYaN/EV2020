﻿// <copyright file="DiagonalMatrixStorage.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
// http://mathnetnumerics.codeplex.com
//
// Copyright (c) 2009-2014 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Properties;
using MathNet.Numerics.Threading;

namespace MathNet.Numerics.LinearAlgebra.Storage
{
    [Serializable]
    public class DiagonalMatrixStorage<T> : MatrixStorage<T>
        where T : struct, IEquatable<T>, IFormattable
    {
        // [ruegg] public fields are OK here

        public readonly T[] Data;

        internal DiagonalMatrixStorage(int rows, int columns)
            : base(rows, columns)
        {
            Data = new T[Math.Min(rows, columns)];
        }

        internal DiagonalMatrixStorage(int rows, int columns, T[] data)
            : base(rows, columns)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (data.Length != Math.Min(rows, columns))
            {
                throw new ArgumentOutOfRangeException("data", string.Format(Resources.ArgumentArrayWrongLength, Math.Min(rows, columns)));
            }

            Data = data;
        }

        /// <summary>
        /// True if the matrix storage format is dense.
        /// </summary>
        public override bool IsDense
        {
            get { return false; }
        }

        /// <summary>
        /// True if all fields of this matrix can be set to any value.
        /// False if some fields are fixed, like on a diagonal matrix.
        /// </summary>
        public override bool IsFullyMutable
        {
            get { return false; }
        }

        /// <summary>
        /// True if the specified field can be set to any value.
        /// False if the field is fixed, like an off-diagonal field on a diagonal matrix.
        /// </summary>
        public override bool IsMutableAt(int row, int column)
        {
            return row == column;
        }

        /// <summary>
        /// Retrieves the requested element without range checking.
        /// </summary>
        public override T At(int row, int column)
        {
            return row == column ? Data[row] : Zero;
        }

        /// <summary>
        /// Sets the element without range checking.
        /// </summary>
        public override void At(int row, int column, T value)
        {
            if (row == column)
            {
                Data[row] = value;
            }
            else if (!Zero.Equals(value))
            {
                throw new IndexOutOfRangeException("Cannot set an off-diagonal element in a diagonal matrix.");
            }
        }

        public override void Clear()
        {
            Array.Clear(Data, 0, Data.Length);
        }

        public override void Clear(int rowIndex, int rowCount, int columnIndex, int columnCount)
        {
            var beginInclusive = Math.Max(rowIndex, columnIndex);
            var endExclusive = Math.Min(rowIndex + rowCount, columnIndex + columnCount);
            if (endExclusive > beginInclusive)
            {
                Array.Clear(Data, beginInclusive, endExclusive - beginInclusive);
            }
        }

        public override void ClearRows(int[] rowIndices)
        {
            for (int i = 0; i < rowIndices.Length; i++)
            {
                Data[rowIndices[i]] = Zero;
            }
        }

        public override void ClearColumns(int[] columnIndices)
        {
            for (int i = 0; i < columnIndices.Length; i++)
            {
                Data[columnIndices[i]] = Zero;
            }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        /// <returns>
        /// <c>true</c> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(MatrixStorage<T> other)
        {
            var diagonal = other as DiagonalMatrixStorage<T>;
            if (diagonal == null)
            {
                return base.Equals(other);
            }

            // Reject equality when the argument is null or has a different shape.
            if (ColumnCount != other.ColumnCount || RowCount != other.RowCount)
            {
                return false;
            }

            // Accept if the argument is the same object as this.
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (diagonal.Data.Length != Data.Length)
            {
                return false;
            }

            // If all else fails, perform element wise comparison.
            return !Data.Where((t, i) => !t.Equals(diagonal.Data[i])).Any();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            var hashNum = Math.Min(Data.Length, 25);
            int hash = 17;
            unchecked
            {
                for (var i = 0; i < hashNum; i++)
                {
                    hash = hash*31 + Data[i].GetHashCode();
                }
            }
            return hash;
        }

        // INITIALIZATION

        public static DiagonalMatrixStorage<T> OfMatrix(MatrixStorage<T> matrix)
        {
            var storage = new DiagonalMatrixStorage<T>(matrix.RowCount, matrix.ColumnCount);
            matrix.CopyToUnchecked(storage, ExistingData.AssumeZeros);
            return storage;
        }

        public static DiagonalMatrixStorage<T> OfArray(T[,] array)
        {
            var storage = new DiagonalMatrixStorage<T>(array.GetLength(0), array.GetLength(1));
            for (var i = 0; i < storage.RowCount; i++)
            {
                for (var j = 0; j < storage.ColumnCount; j++)
                {
                    if (i == j)
                    {
                        storage.Data[i] = array[i, j];
                    }
                    else if (!Zero.Equals(array[i, j]))
                    {
                        throw new ArgumentException("Cannot set an off-diagonal element in a diagonal matrix.");
                    }
                }
            }
            return storage;
        }

        public static DiagonalMatrixStorage<T> OfInit(int rows, int columns, Func<int, T> init)
        {
            var storage = new DiagonalMatrixStorage<T>(rows, columns);
            for (var i = 0; i < storage.Data.Length; i++)
            {
                storage.Data[i] = init(i);
            }
            return storage;
        }

        public static DiagonalMatrixStorage<T> OfEnumerable(int rows, int columns, IEnumerable<T> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            var arrayData = data as T[];
            if (arrayData != null)
            {
                var copy = new T[arrayData.Length];
                Array.Copy(arrayData, copy, arrayData.Length);
                return new DiagonalMatrixStorage<T>(rows, columns, copy);
            }

            return new DiagonalMatrixStorage<T>(rows, columns, data.ToArray());
        }

        public static DiagonalMatrixStorage<T> OfIndexedEnumerable(int rows, int columns, IEnumerable<Tuple<int, T>> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            var storage = new DiagonalMatrixStorage<T>(rows, columns);
            foreach (var item in data)
            {
                storage.Data[item.Item1] = item.Item2;
            }
            return storage;
        }

        // MATRIX COPY

        internal override void CopyToUnchecked(MatrixStorage<T> target, ExistingData existingData = ExistingData.Clear)
        {
            var diagonalTarget = target as DiagonalMatrixStorage<T>;
            if (diagonalTarget != null)
            {
                CopyToUnchecked(diagonalTarget);
                return;
            }

            var denseTarget = target as DenseColumnMajorMatrixStorage<T>;
            if (denseTarget != null)
            {
                CopyToUnchecked(denseTarget, existingData);
                return;
            }

            var sparseTarget = target as SparseCompressedRowMatrixStorage<T>;
            if (sparseTarget != null)
            {
                CopyToUnchecked(sparseTarget, existingData);
                return;
            }

            // FALL BACK

            if (existingData == ExistingData.Clear)
            {
                target.Clear();
            }

            for (int i = 0; i < Data.Length; i++)
            {
                target.At(i, i, Data[i]);
            }
        }

        void CopyToUnchecked(DiagonalMatrixStorage<T> target)
        {
            //Buffer.BlockCopy(Data, 0, target.Data, 0, Data.Length * System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)));
            Array.Copy(Data, 0, target.Data, 0, Data.Length);
        }

        void CopyToUnchecked(SparseCompressedRowMatrixStorage<T> target, ExistingData existingData)
        {
            if (existingData == ExistingData.Clear)
            {
                target.Clear();
            }

            for (int i = 0; i < Data.Length; i++)
            {
                target.At(i, i, Data[i]);
            }
        }

        void CopyToUnchecked(DenseColumnMajorMatrixStorage<T> target, ExistingData existingData)
        {
            if (existingData == ExistingData.Clear)
            {
                target.Clear();
            }

            for (int i = 0; i < Data.Length; i++)
            {
                target.Data[i*(target.RowCount + 1)] = Data[i];
            }
        }

        internal override void CopySubMatrixToUnchecked(MatrixStorage<T> target,
            int sourceRowIndex, int targetRowIndex, int rowCount,
            int sourceColumnIndex, int targetColumnIndex, int columnCount,
            ExistingData existingData = ExistingData.Clear)
        {
            var denseTarget = target as DenseColumnMajorMatrixStorage<T>;
            if (denseTarget != null)
            {
                CopySubMatrixToUnchecked(denseTarget, sourceRowIndex, targetRowIndex, rowCount, sourceColumnIndex, targetColumnIndex, columnCount, existingData);
                return;
            }

            var diagonalTarget = target as DiagonalMatrixStorage<T>;
            if (diagonalTarget != null)
            {
                CopySubMatrixToUnchecked(diagonalTarget, sourceRowIndex, targetRowIndex, rowCount, sourceColumnIndex, targetColumnIndex, columnCount);
                return;
            }

            // TODO: Proper Sparse Implementation

            // FALL BACK

            if (existingData == ExistingData.Clear)
            {
                target.Clear(targetRowIndex, rowCount, targetColumnIndex, columnCount);
            }

            if (sourceRowIndex == sourceColumnIndex)
            {
                for (var i = 0; i < Math.Min(columnCount, rowCount); i++)
                {
                    target.At(targetRowIndex + i, targetColumnIndex + i, Data[sourceRowIndex + i]);
                }
            }
            else if (sourceRowIndex > sourceColumnIndex && sourceColumnIndex + columnCount > sourceRowIndex)
            {
                // column by column, but skip resulting zero columns at the beginning
                int columnInit = sourceRowIndex - sourceColumnIndex;
                for (var i = 0; i < Math.Min(columnCount - columnInit, rowCount); i++)
                {
                    target.At(targetRowIndex + i, columnInit + targetColumnIndex + i, Data[sourceRowIndex + i]);
                }
            }
            else if (sourceRowIndex < sourceColumnIndex && sourceRowIndex + rowCount > sourceColumnIndex)
            {
                // row by row, but skip resulting zero rows at the beginning
                int rowInit = sourceColumnIndex - sourceRowIndex;
                for (var i = 0; i < Math.Min(columnCount, rowCount - rowInit); i++)
                {
                    target.At(rowInit + targetRowIndex + i, targetColumnIndex + i, Data[sourceColumnIndex + i]);
                }
            }
        }

        void CopySubMatrixToUnchecked(DiagonalMatrixStorage<T> target,
            int sourceRowIndex, int targetRowIndex, int rowCount,
            int sourceColumnIndex, int targetColumnIndex, int columnCount)
        {
            if (sourceRowIndex - sourceColumnIndex != targetRowIndex - targetColumnIndex)
            {
                if (Data.Any(x => !Zero.Equals(x)))
                {
                    throw new NotSupportedException();
                }

                target.Clear(targetRowIndex, rowCount, targetColumnIndex, columnCount);
                return;
            }

            var beginInclusive = Math.Max(sourceRowIndex, sourceColumnIndex);
            var endExclusive = Math.Min(sourceRowIndex + rowCount, sourceColumnIndex + columnCount);
            if (endExclusive > beginInclusive)
            {
                var beginTarget = Math.Max(targetRowIndex, targetColumnIndex);
                Array.Copy(Data, beginInclusive, target.Data, beginTarget, endExclusive - beginInclusive);
            }
        }

        void CopySubMatrixToUnchecked(DenseColumnMajorMatrixStorage<T> target,
            int sourceRowIndex, int targetRowIndex, int rowCount,
            int sourceColumnIndex, int targetColumnIndex, int columnCount,
            ExistingData existingData)
        {
            if (existingData == ExistingData.Clear)
            {
                target.Clear(targetRowIndex, rowCount, targetColumnIndex, columnCount);
            }

            if (sourceRowIndex > sourceColumnIndex && sourceColumnIndex + columnCount > sourceRowIndex)
            {
                // column by column, but skip resulting zero columns at the beginning

                int columnInit = sourceRowIndex - sourceColumnIndex;
                int offset = (columnInit + targetColumnIndex)*target.RowCount + targetRowIndex;
                int step = target.RowCount + 1;
                int end = Math.Min(columnCount - columnInit, rowCount) + sourceRowIndex;

                for (int i = sourceRowIndex, j = offset; i < end; i++, j += step)
                {
                    target.Data[j] = Data[i];
                }
            }
            else if (sourceRowIndex < sourceColumnIndex && sourceRowIndex + rowCount > sourceColumnIndex)
            {
                // row by row, but skip resulting zero rows at the beginning

                int rowInit = sourceColumnIndex - sourceRowIndex;
                int offset = targetColumnIndex*target.RowCount + rowInit + targetRowIndex;
                int step = target.RowCount + 1;
                int end = Math.Min(columnCount, rowCount - rowInit) + sourceColumnIndex;

                for (int i = sourceColumnIndex, j = offset; i < end; i++, j += step)
                {
                    target.Data[j] = Data[i];
                }
            }
            else
            {
                int offset = targetColumnIndex*target.RowCount + targetRowIndex;
                int step = target.RowCount + 1;
                var end = Math.Min(columnCount, rowCount) + sourceRowIndex;

                for (int i = sourceRowIndex, j = offset; i < end; i++, j += step)
                {
                    target.Data[j] = Data[i];
                }
            }
        }

        // ROW COPY

        internal override void CopySubRowToUnchecked(VectorStorage<T> target, int rowIndex,
            int sourceColumnIndex, int targetColumnIndex, int columnCount,
            ExistingData existingData = ExistingData.Clear)
        {
            if (existingData == ExistingData.Clear)
            {
                target.Clear(targetColumnIndex, columnCount);
            }

            if (rowIndex >= sourceColumnIndex && rowIndex < sourceColumnIndex + columnCount && rowIndex < Data.Length)
            {
                target.At(rowIndex - sourceColumnIndex + targetColumnIndex, Data[rowIndex]);
            }
        }

        // COLUMN COPY

        internal override void CopySubColumnToUnchecked(VectorStorage<T> target, int columnIndex,
            int sourceRowIndex, int targetRowIndex, int rowCount,
            ExistingData existingData = ExistingData.Clear)
        {
            if (existingData == ExistingData.Clear)
            {
                target.Clear(targetRowIndex, rowCount);
            }

            if (columnIndex >= sourceRowIndex && columnIndex < sourceRowIndex + rowCount && columnIndex < Data.Length)
            {
                target.At(columnIndex - sourceRowIndex + targetRowIndex, Data[columnIndex]);
            }
        }

        // TRANSPOSE

        internal override void TransposeToUnchecked(MatrixStorage<T> target, ExistingData existingData = ExistingData.Clear)
        {
            CopyToUnchecked(target, existingData);
        }

        // EXTRACT

        public override T[] ToRowMajorArray()
        {
            var ret = new T[RowCount*ColumnCount];
            var stride = ColumnCount + 1;
            for (int i = 0; i < Data.Length; i++)
            {
                ret[i*stride] = Data[i];
            }
            return ret;
        }

        public override T[] ToColumnMajorArray()
        {
            var ret = new T[RowCount*ColumnCount];
            var stride = RowCount + 1;
            for (int i = 0; i < Data.Length; i++)
            {
                ret[i*stride] = Data[i];
            }
            return ret;
        }

        public override T[,] ToArray()
        {
            var ret = new T[RowCount, ColumnCount];
            for (int i = 0; i < Data.Length; i++)
            {
                ret[i, i] = Data[i];
            }
            return ret;
        }

        // ENUMERATION

        public override IEnumerable<T> Enumerate()
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                for (int i = 0; i < RowCount; i++)
                {
                    // PERF: consider to break up loop to avoid branching
                    yield return i == j ? Data[i] : Zero;
                }
            }
        }

        public override IEnumerable<Tuple<int, int, T>> EnumerateIndexed()
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                for (int i = 0; i < RowCount; i++)
                {
                    // PERF: consider to break up loop to avoid branching
                    yield return i == j
                        ? new Tuple<int, int, T>(i, i, Data[i])
                        : new Tuple<int, int, T>(i, j, Zero);
                }
            }
        }

        public override IEnumerable<T> EnumerateNonZero()
        {
            return Data.Where(x => !Zero.Equals(x));
        }

        public override IEnumerable<Tuple<int, int, T>> EnumerateNonZeroIndexed()
        {
            for (int i = 0; i < Data.Length; i++)
            {
                if (!Zero.Equals(Data[i]))
                {
                    yield return new Tuple<int, int, T>(i, i, Data[i]);
                }
            }
        }

        // FUNCTIONAL COMBINATORS: MAP

        public override void MapInplace(Func<T, T> f, Zeros zeros = Zeros.AllowSkip)
        {
            if (zeros == Zeros.Include)
            {
                throw new NotSupportedException("Cannot map non-zero off-diagonal values into a diagonal matrix");
            }

            CommonParallel.For(0, Data.Length, 4096, (a, b) =>
            {
                for (int i = a; i < b; i++)
                {
                    Data[i] = f(Data[i]);
                }
            });
        }

        public override void MapIndexedInplace(Func<int, int, T, T> f, Zeros zeros = Zeros.AllowSkip)
        {
            if (zeros == Zeros.Include)
            {
                throw new NotSupportedException("Cannot map non-zero off-diagonal values into a diagonal matrix");
            }

            CommonParallel.For(0, Data.Length, 4096, (a, b) =>
            {
                for (int i = a; i < b; i++)
                {
                    Data[i] = f(i, i, Data[i]);
                }
            });
        }

        internal override void MapToUnchecked<TU>(MatrixStorage<TU> target, Func<T, TU> f,
            Zeros zeros = Zeros.AllowSkip, ExistingData existingData = ExistingData.Clear)
        {
            var processZeros = zeros == Zeros.Include || !Zero.Equals(f(Zero));

            var diagonalTarget = target as DiagonalMatrixStorage<TU>;
            if (diagonalTarget != null)
            {
                if (processZeros)
                {
                    throw new NotSupportedException("Cannot map non-zero off-diagonal values into a diagonal matrix");
                }

                CommonParallel.For(0, Data.Length, 4096, (a, b) =>
                {
                    for (int i = a; i < b; i++)
                    {
                        diagonalTarget.Data[i] = f(Data[i]);
                    }
                });
                return;
            }

            // FALL BACK

            if (existingData == ExistingData.Clear && !processZeros)
            {
                target.Clear();
            }

            if (processZeros)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    for (int i = 0; i < RowCount; i++)
                    {
                        target.At(i, j, f(i == j ? Data[i] : Zero));
                    }
                }
            }
            else
            {
                for (int i = 0; i < Data.Length; i++)
                {
                    target.At(i, i, f(Data[i]));
                }
            }
        }

        internal override void MapIndexedToUnchecked<TU>(MatrixStorage<TU> target, Func<int, int, T, TU> f,
            Zeros zeros = Zeros.AllowSkip, ExistingData existingData = ExistingData.Clear)
        {
            var processZeros = zeros == Zeros.Include || !Zero.Equals(f(0, 1, Zero));

            var diagonalTarget = target as DiagonalMatrixStorage<TU>;
            if (diagonalTarget != null)
            {
                if (processZeros)
                {
                    throw new NotSupportedException("Cannot map non-zero off-diagonal values into a diagonal matrix");
                }

                CommonParallel.For(0, Data.Length, 4096, (a, b) =>
                {
                    for (int i = a; i < b; i++)
                    {
                        diagonalTarget.Data[i] = f(i, i, Data[i]);
                    }
                });
                return;
            }

            // FALL BACK

            if (existingData == ExistingData.Clear && !processZeros)
            {
                target.Clear();
            }

            if (processZeros)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    for (int i = 0; i < RowCount; i++)
                    {
                        target.At(i, j, f(i, j, i == j ? Data[i] : Zero));
                    }
                }
            }
            else
            {
                for (int i = 0; i < Data.Length; i++)
                {
                    target.At(i, i, f(i, i, Data[i]));
                }
            }
        }

        internal override void MapSubMatrixIndexedToUnchecked<TU>(MatrixStorage<TU> target, Func<int, int, T, TU> f,
            int sourceRowIndex, int targetRowIndex, int rowCount,
            int sourceColumnIndex, int targetColumnIndex, int columnCount,
            Zeros zeros = Zeros.AllowSkip, ExistingData existingData = ExistingData.Clear)
        {
            var diagonalTarget = target as DiagonalMatrixStorage<TU>;
            if (diagonalTarget != null)
            {
                MapSubMatrixIndexedToUnchecked(diagonalTarget, f, sourceRowIndex, targetRowIndex, rowCount, sourceColumnIndex, targetColumnIndex, columnCount, zeros);
                return;
            }

            var denseTarget = target as DenseColumnMajorMatrixStorage<TU>;
            if (denseTarget != null)
            {
                MapSubMatrixIndexedToUnchecked(denseTarget, f, sourceRowIndex, targetRowIndex, rowCount, sourceColumnIndex, targetColumnIndex, columnCount, zeros, existingData);
                return;
            }

            // TODO: Proper Sparse Implementation

            // FALL BACK

            if (existingData == ExistingData.Clear)
            {
                target.Clear(targetRowIndex, rowCount, targetColumnIndex, columnCount);
            }

            if (sourceRowIndex == sourceColumnIndex)
            {
                int targetRow = targetRowIndex;
                int targetColumn = targetColumnIndex;
                for (var i = 0; i < Math.Min(columnCount, rowCount); i++)
                {
                    target.At(targetRow, targetColumn, f(targetRow, targetColumn, Data[sourceRowIndex + i]));
                    targetRow++;
                    targetColumn++;
                }
            }
            else if (sourceRowIndex > sourceColumnIndex && sourceColumnIndex + columnCount > sourceRowIndex)
            {
                // column by column, but skip resulting zero columns at the beginning
                int columnInit = sourceRowIndex - sourceColumnIndex;
                int targetRow = targetRowIndex;
                int targetColumn = targetColumnIndex + columnInit;
                for (var i = 0; i < Math.Min(columnCount - columnInit, rowCount); i++)
                {
                    target.At(targetRow, targetColumn, f(targetRow, targetColumn, Data[sourceRowIndex + i]));
                    targetRow++;
                    targetColumn++;
                }
            }
            else if (sourceRowIndex < sourceColumnIndex && sourceRowIndex + rowCount > sourceColumnIndex)
            {
                // row by row, but skip resulting zero rows at the beginning
                int rowInit = sourceColumnIndex - sourceRowIndex;
                int targetRow = targetRowIndex + rowInit;
                int targetColumn = targetColumnIndex;
                for (var i = 0; i < Math.Min(columnCount, rowCount - rowInit); i++)
                {
                    target.At(targetRow, targetColumn, f(targetRow, targetColumn, Data[sourceColumnIndex + i]));
                    targetRow++;
                    targetColumn++;
                }
            }
        }

        void MapSubMatrixIndexedToUnchecked<TU>(DiagonalMatrixStorage<TU> target, Func<int, int, T, TU> f,
            int sourceRowIndex, int targetRowIndex, int rowCount,
            int sourceColumnIndex, int targetColumnIndex, int columnCount,
            Zeros zeros)
            where TU : struct, IEquatable<TU>, IFormattable
        {
            var processZeros = zeros == Zeros.Include || !Zero.Equals(f(0, 1, Zero));
            if (processZeros || sourceRowIndex - sourceColumnIndex != targetRowIndex - targetColumnIndex)
            {
                throw new NotSupportedException("Cannot map non-zero off-diagonal values into a diagonal matrix");
            }

            var beginInclusive = Math.Max(sourceRowIndex, sourceColumnIndex);
            var count = Math.Min(sourceRowIndex + rowCount, sourceColumnIndex + columnCount) - beginInclusive;
            if (count > 0)
            {
                var beginTarget = Math.Max(targetRowIndex, targetColumnIndex);
                CommonParallel.For(0, count, 4096, (a, b) =>
                {
                    int targetIndex = beginTarget + a;
                    for (int i = a; i < b; i++)
                    {
                        target.Data[targetIndex] = f(targetIndex, targetIndex, Data[beginInclusive + i]);
                        targetIndex++;
                    }
                });
            }
        }

        void MapSubMatrixIndexedToUnchecked<TU>(DenseColumnMajorMatrixStorage<TU> target, Func<int, int, T, TU> f,
            int sourceRowIndex, int targetRowIndex, int rowCount,
            int sourceColumnIndex, int targetColumnIndex, int columnCount,
            Zeros zeros, ExistingData existingData)
            where TU : struct, IEquatable<TU>, IFormattable
        {
            var processZeros = zeros == Zeros.Include || !Zero.Equals(f(0, 1, Zero));
            if (existingData == ExistingData.Clear && !processZeros)
            {
                target.Clear(targetRowIndex, rowCount, targetColumnIndex, columnCount);
            }

            if (processZeros)
            {
                CommonParallel.For(0, columnCount, Math.Max(4096/rowCount, 32), (a, b) =>
                {
                    int sourceColumn = sourceColumnIndex + a;
                    int targetColumn = targetColumnIndex + a;
                    for (int j = a; j < b; j++)
                    {
                        int targetIndex = targetRowIndex + (j + targetColumnIndex)*target.RowCount;
                        int sourceRow = sourceRowIndex;
                        int targetRow = targetRowIndex;
                        for (int i = 0; i < rowCount; i++)
                        {
                            target.Data[targetIndex++] = f(targetRow++, targetColumn, sourceRow++ == sourceColumn ? Data[sourceColumn] : Zero);
                        }
                        sourceColumn++;
                        targetColumn++;
                    }
                });
            }
            else
            {
                if (sourceRowIndex > sourceColumnIndex && sourceColumnIndex + columnCount > sourceRowIndex)
                {
                    // column by column, but skip resulting zero columns at the beginning

                    int columnInit = sourceRowIndex - sourceColumnIndex;
                    int offset = (columnInit + targetColumnIndex)*target.RowCount + targetRowIndex;
                    int step = target.RowCount + 1;
                    int count = Math.Min(columnCount - columnInit, rowCount);

                    for (int k = 0, j = offset; k < count; j += step, k++)
                    {
                        target.Data[j] = f(targetRowIndex + k, targetColumnIndex + columnInit + k, Data[sourceRowIndex + k]);
                    }
                }
                else if (sourceRowIndex < sourceColumnIndex && sourceRowIndex + rowCount > sourceColumnIndex)
                {
                    // row by row, but skip resulting zero rows at the beginning

                    int rowInit = sourceColumnIndex - sourceRowIndex;
                    int offset = targetColumnIndex*target.RowCount + rowInit + targetRowIndex;
                    int step = target.RowCount + 1;
                    int count = Math.Min(columnCount, rowCount - rowInit);

                    for (int k = 0, j = offset; k < count; j += step, k++)
                    {
                        target.Data[j] = f(targetRowIndex + rowInit + k, targetColumnIndex + k, Data[sourceColumnIndex + k]);
                    }
                }
                else
                {
                    int offset = targetColumnIndex*target.RowCount + targetRowIndex;
                    int step = target.RowCount + 1;
                    var count = Math.Min(columnCount, rowCount);

                    for (int k = 0, j = offset; k < count; j += step, k++)
                    {
                        target.Data[j] = f(targetRowIndex + k, targetColumnIndex + k, Data[sourceRowIndex + k]);
                    }
                }
            }
        }

        // FUNCTIONAL COMBINATORS: FOLD

        internal override void FoldRowsUnchecked<TU>(VectorStorage<TU> target, Func<TU, T, TU> f, Func<TU, int, TU> finalize, VectorStorage<TU> state, Zeros zeros = Zeros.AllowSkip)
        {
            if (zeros == Zeros.AllowSkip)
            {
                for (int k = 0; k < Data.Length; k++)
                {
                    target.At(k, finalize(f(state.At(k), Data[k]), 1));
                }

                for (int k = Data.Length; k < RowCount; k++)
                {
                    target.At(k, finalize(state.At(k), 0));
                }
            }
            else
            {
                for (int i = 0; i < RowCount; i++)
                {
                    TU s = state.At(i);
                    for (int j = 0; j < ColumnCount; j++)
                    {
                        s = f(s, i == j ? Data[i] : Zero);
                    }
                    target.At(i, finalize(s, ColumnCount));
                }
            }
        }

        internal override void FoldColumnsUnchecked<TU>(VectorStorage<TU> target, Func<TU, T, TU> f, Func<TU, int, TU> finalize, VectorStorage<TU> state, Zeros zeros = Zeros.AllowSkip)
        {
            if (zeros == Zeros.AllowSkip)
            {
                for (int k = 0; k < Data.Length; k++)
                {
                    target.At(k, finalize(f(state.At(k), Data[k]), 1));
                }

                for (int k = Data.Length; k < ColumnCount; k++)
                {
                    target.At(k, finalize(state.At(k), 0));
                }
            }
            else
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    TU s = state.At(j);
                    for (int i = 0; i < RowCount; i++)
                    {
                        s = f(s, i == j ? Data[i] : Zero);
                    }
                    target.At(j, finalize(s, RowCount));
                }
            }
        }
    }
}