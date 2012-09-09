﻿// <copyright file="SparseMatrix.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
// http://mathnetnumerics.codeplex.com
//
// Copyright (c) 2009-2010 Math.NET
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

namespace MathNet.Numerics.LinearAlgebra.Complex
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using Generic;
    using Properties;
    using Storage;
    using Threading;

    /// <summary>
    /// A Matrix class with sparse storage. The underlying storage scheme is 3-array compressed-sparse-row (CSR) Format.
    /// <a href="http://en.wikipedia.org/wiki/Sparse_matrix#Compressed_sparse_row_.28CSR_or_CRS.29">Wikipedia - CSR</a>.
    /// </summary>
    [Serializable]
    public class SparseMatrix : Matrix
    {
        readonly SparseCompressedRowMatrixStorage<Complex> _storage;

        /// <summary>
        /// Gets the number of non zero elements in the matrix.
        /// </summary>
        /// <value>The number of non zero elements.</value>
        public int NonZerosCount
        {
            get { return _storage.ValueCount; }
        }

        internal SparseMatrix(SparseCompressedRowMatrixStorage<Complex> storage)
            : base(storage)
        {
            _storage = storage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseMatrix"/> class.
        /// </summary>
        /// <param name="rows">
        /// The number of rows.
        /// </param>
        /// <param name="columns">
        /// The number of columns.
        /// </param>
        public SparseMatrix(int rows, int columns)
            : this(new SparseCompressedRowMatrixStorage<Complex>(rows, columns, Complex.Zero))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseMatrix"/> class. This matrix is square with a given size.
        /// </summary>
        /// <param name="order">the size of the square matrix.</param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="order"/> is less than one.
        /// </exception>
        public SparseMatrix(int order)
            : this(order, order)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseMatrix"/> class with all entries set to a particular value.
        /// </summary>
        /// <param name="rows">
        /// The number of rows.
        /// </param>
        /// <param name="columns">
        /// The number of columns.
        /// </param>
        /// <param name="value">The value which we assign to each element of the matrix.</param>
        public SparseMatrix(int rows, int columns, Complex value)
            : this(rows, columns)
        {
            if (value.IsZero())
            {
                return;
            }

            var rowPointers = _storage.RowPointers;
            var valueCount = _storage.ValueCount = rows * columns;
            var columnIndices = _storage.ColumnIndices = new int[valueCount];
            var values = _storage.Values = new Complex[valueCount];

            for (int i = 0, j = 0; i < values.Length; i++, j++)
            {
                // Reset column position to "0"
                if (j == columns)
                {
                    j = 0;
                }

                values[i] = value;
                columnIndices[i] = j;
            }

            // Set proper row pointers
            for (var i = 0; i < rowPointers.Length; i++)
            {
                rowPointers[i] = ((i + 1) * columns) - columns;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseMatrix"/> class from a one dimensional array. 
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="columns">The number of columns.</param>
        /// <param name="array">The one dimensional array to create this matrix from. This array should store the matrix in column-major order. see: http://en.wikipedia.org/wiki/Column-major_order </param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="array"/> length is less than <paramref name="rows"/> * <paramref name="columns"/>.
        /// </exception>
        public SparseMatrix(int rows, int columns, Complex[] array)
            : this(rows, columns)
        {
            if (rows * columns > array.Length)
            {
                throw new ArgumentOutOfRangeException(Resources.ArgumentMatrixDimensions);
            }

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    _storage.At(i, j, array[i + (j * rows)]);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseMatrix"/> class from a 2D array. 
        /// </summary>
        /// <param name="array">The 2D array to create this matrix from.</param>
        public SparseMatrix(Complex[,] array)
            : this(array.GetLength(0), array.GetLength(1))
        {
            for (var i = 0; i < _storage.RowCount; i++)
            {
                for (var j = 0; j < _storage.ColumnCount; j++)
                {
                    _storage.At(i, j, array[i, j]);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseMatrix"/> class, copying
        /// the values from the given matrix.
        /// </summary>
        /// <param name="matrix">The matrix to copy.</param>
        public SparseMatrix(Matrix<Complex> matrix)
            : this(matrix.RowCount, matrix.ColumnCount)
        {
            matrix.Storage.CopyTo(Storage, skipClearing: true);
        }

        /// <summary>
        /// Creates a <c>SparseMatrix</c> for the given number of rows and columns.
        /// </summary>
        /// <param name="numberOfRows">The number of rows.</param>
        /// <param name="numberOfColumns">The number of columns.</param>
        /// <param name="fullyMutable">True if all fields must be mutable (e.g. not a diagonal matrix).</param>
        /// <returns>
        /// A <c>SparseMatrix</c> with the given dimensions.
        /// </returns>
        public override Matrix<Complex> CreateMatrix(int numberOfRows, int numberOfColumns, bool fullyMutable = false)
        {
            return new SparseMatrix(numberOfRows, numberOfColumns);
        }

        /// <summary>
        /// Creates a <see cref="SparseVector"/> with a the given dimension.
        /// </summary>
        /// <param name="size">The size of the vector.</param>
        /// <param name="fullyMutable">True if all fields must be mutable.</param>
        /// <returns>
        /// A <see cref="SparseVector"/> with the given dimension.
        /// </returns>
        public override Vector<Complex> CreateVector(int size, bool fullyMutable = false)
        {
            return new SparseVector(size);
        }

        /// <summary>
        /// Returns a new matrix containing the lower triangle of this matrix.
        /// </summary>
        /// <returns>The lower triangle of this matrix.</returns>        
        public override Matrix<Complex> LowerTriangle()
        {
            var result = CreateMatrix(RowCount, ColumnCount);
            LowerTriangleImpl(result);
            return result;
        }

        /// <summary>
        /// Puts the lower triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public override void LowerTriangle(Matrix<Complex> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != RowCount || result.ColumnCount != ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, result);
            }

            if (ReferenceEquals(this, result))
            {
                var tmp = result.CreateMatrix(result.RowCount, result.ColumnCount);
                LowerTriangle(tmp);
                tmp.CopyTo(result);
            }
            else
            {
                result.Clear();
                LowerTriangleImpl(result);
            }
        }

        /// <summary>
        /// Puts the lower triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        private void LowerTriangleImpl(Matrix<Complex> result)
        {
            var rowPointers = _storage.RowPointers;
            var columnIndices = _storage.ColumnIndices;
            var values = _storage.Values;
            var valueCount = _storage.ValueCount;

            for (var row = 0; row < result.RowCount; row++)
            {
                var startIndex = rowPointers[row];
                var endIndex = row < rowPointers.Length - 1 ? rowPointers[row + 1] : valueCount;
                for (var j = startIndex; j < endIndex; j++)
                {
                    if (row >= columnIndices[j])
                    {
                        result.At(row, columnIndices[j], values[j]);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a new matrix containing the upper triangle of this matrix.
        /// </summary>
        /// <returns>The upper triangle of this matrix.</returns>   
        public override Matrix<Complex> UpperTriangle()
        {
            var result = CreateMatrix(RowCount, ColumnCount);
            UpperTriangleImpl(result);
            return result;
        }

        /// <summary>
        /// Puts the upper triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public override void UpperTriangle(Matrix<Complex> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != RowCount || result.ColumnCount != ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, result);
            }

            if (ReferenceEquals(this, result))
            {
                var tmp = result.CreateMatrix(result.RowCount, result.ColumnCount);
                UpperTriangle(tmp);
                tmp.CopyTo(result);
            }
            else
            {
                result.Clear();
                UpperTriangleImpl(result);
            }
        }

        /// <summary>
        /// Puts the upper triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        private void UpperTriangleImpl(Matrix<Complex> result)
        {
            var rowPointers = _storage.RowPointers;
            var columnIndices = _storage.ColumnIndices;
            var values = _storage.Values;
            var valueCount = _storage.ValueCount;

            for (var row = 0; row < result.RowCount; row++)
            {
                var startIndex = rowPointers[row];
                var endIndex = row < rowPointers.Length - 1 ? rowPointers[row + 1] : valueCount;
                for (var j = startIndex; j < endIndex; j++)
                {
                    if (row <= columnIndices[j])
                    {
                        result.At(row, columnIndices[j], values[j]);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a new matrix containing the lower triangle of this matrix. The new matrix
        /// does not contain the diagonal elements of this matrix.
        /// </summary>
        /// <returns>The lower triangle of this matrix.</returns>
        public override Matrix<Complex> StrictlyLowerTriangle()
        {
            var result = CreateMatrix(RowCount, ColumnCount);
            StrictlyLowerTriangleImpl(result);
            return result;
        }

        /// <summary>
        /// Puts the strictly lower triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public override void StrictlyLowerTriangle(Matrix<Complex> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != RowCount || result.ColumnCount != ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, result);
            }

            if (ReferenceEquals(this, result))
            {
                var tmp = result.CreateMatrix(result.RowCount, result.ColumnCount);
                StrictlyLowerTriangle(tmp);
                tmp.CopyTo(result);
            }
            else
            {
                result.Clear();
                StrictlyLowerTriangleImpl(result);
            }
        }

        /// <summary>
        /// Puts the strictly lower triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        private void StrictlyLowerTriangleImpl(Matrix<Complex> result)
        {
            var rowPointers = _storage.RowPointers;
            var columnIndices = _storage.ColumnIndices;
            var values = _storage.Values;
            var valueCount = _storage.ValueCount;

            for (var row = 0; row < result.RowCount; row++)
            {
                var startIndex = rowPointers[row];
                var endIndex = row < rowPointers.Length - 1 ? rowPointers[row + 1] : valueCount;
                for (var j = startIndex; j < endIndex; j++)
                {
                    if (row > columnIndices[j])
                    {
                        result.At(row, columnIndices[j], values[j]);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a new matrix containing the upper triangle of this matrix. The new matrix
        /// does not contain the diagonal elements of this matrix.
        /// </summary>
        /// <returns>The upper triangle of this matrix.</returns>
        public override Matrix<Complex> StrictlyUpperTriangle()
        {
            var result = CreateMatrix(RowCount, ColumnCount);
            StrictlyUpperTriangleImpl(result);
            return result;
        }

        /// <summary>
        /// Puts the strictly upper triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public override void StrictlyUpperTriangle(Matrix<Complex> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != RowCount || result.ColumnCount != ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, result);
            }

            if (ReferenceEquals(this, result))
            {
                var tmp = result.CreateMatrix(result.RowCount, result.ColumnCount);
                StrictlyUpperTriangle(tmp);
                tmp.CopyTo(result);
            }
            else
            {
                result.Clear();
                StrictlyUpperTriangleImpl(result);
            }
        }

        /// <summary>
        /// Puts the strictly upper triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        private void StrictlyUpperTriangleImpl(Matrix<Complex> result)
        {
            var rowPointers = _storage.RowPointers;
            var columnIndices = _storage.ColumnIndices;
            var values = _storage.Values;
            var valueCount = _storage.ValueCount;

            for (var row = 0; row < result.RowCount; row++)
            {
                var startIndex = rowPointers[row];
                var endIndex = row < rowPointers.Length - 1 ? rowPointers[row + 1] : valueCount;
                for (var j = startIndex; j < endIndex; j++)
                {
                    if (row < columnIndices[j])
                    {
                        result.At(row, columnIndices[j], values[j]);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the matrix's elements as an array with the data laid out column-wise.
        /// </summary>
        /// <example><pre>
        /// 1, 2, 3
        /// 4, 5, 6  will be returned as  1, 4, 7, 2, 5, 8, 3, 6, 9
        /// 7, 8, 9
        /// </pre></example>
        /// <returns>An array containing the matrix's elements.</returns>
        public override Complex[] ToColumnWiseArray()
        {
            var values = _storage.Values;
            var ret = new Complex[RowCount * ColumnCount];
            for (var j = 0; j < ColumnCount; j++)
            {
                for (var i = 0; i < RowCount; i++)
                {
                    var index = _storage.FindItem(i, j);
                    ret[(j * RowCount) + i] = index >= 0 ? values[index] : 0.0;
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns the transpose of this matrix.
        /// </summary>        
        /// <returns>The transpose of this matrix.</returns>
        public override Matrix<Complex> Transpose()
        {
            var rowPointers = _storage.RowPointers;
            var columnIndices = _storage.ColumnIndices;
            var values = _storage.Values;
            var valueCount = _storage.ValueCount;

            var ret = new SparseCompressedRowMatrixStorage<Complex>(ColumnCount, RowCount, Complex.Zero)
                {
                    ColumnIndices = new int[valueCount],
                    Values = new Complex[valueCount]
                };

            // Do an 'inverse' CopyTo iterate over the rows
            for (var i = 0; i < rowPointers.Length; i++)
            {
                // Get the begin / end index for the current row
                var startIndex = rowPointers[i];
                var endIndex = i < rowPointers.Length - 1 ? rowPointers[i + 1] : NonZerosCount;

                // Get the values for the current row
                if (startIndex == endIndex)
                {
                    // Begin and end are equal. There are no values in the row, Move to the next row
                    continue;
                }

                for (var j = startIndex; j < endIndex; j++)
                {
                    ret.At(columnIndices[j], i, values[j]);
                }
            }

            return new SparseMatrix(ret);
        }

        /// <summary>Calculates the Frobenius norm of this matrix.</summary>
        /// <returns>The Frobenius norm of this matrix.</returns>
        public override Complex FrobeniusNorm()
        {
            var aat = (SparseCompressedRowMatrixStorage<Complex>) (this*Transpose()).Storage;
            var norm = 0d;

            for (var i = 0; i < aat.RowPointers.Length; i++)
            {
                // Get the begin / end index for the current row
                var startIndex = aat.RowPointers[i];
                var endIndex = i < aat.RowPointers.Length - 1 ? aat.RowPointers[i + 1] : aat.ValueCount;

                // Get the values for the current row
                if (startIndex == endIndex)
                {
                    // Begin and end are equal. There are no values in the row, Move to the next row
                    continue;
                }

                for (var j = startIndex; j < endIndex; j++)
                {
                    if (i == aat.ColumnIndices[j])
                    {
                        norm += aat.Values[j].Magnitude;
                    }
                }
            }

            norm = Math.Sqrt(norm);
            return norm;
        }

        /// <summary>Calculates the infinity norm of this matrix.</summary>
        /// <returns>The infinity norm of this matrix.</returns>   
        public override Complex InfinityNorm()
        {
            var rowPointers = _storage.RowPointers;
            var values = _storage.Values;
            var valueCount = _storage.ValueCount;

            var norm = 0d;
            for (var i = 0; i < rowPointers.Length; i++)
            {
                // Get the begin / end index for the current row
                var startIndex = rowPointers[i];
                var endIndex = i < rowPointers.Length - 1 ? rowPointers[i + 1] : valueCount;

                // Get the values for the current row
                if (startIndex == endIndex)
                {
                    // Begin and end are equal. There are no values in the row, Move to the next row
                    continue;
                }

                var s = 0.0;
                for (var j = startIndex; j < endIndex; j++)
                {
                    s += values[j].Magnitude;
                }

                norm = Math.Max(norm, s);
            }

            return norm;
        }

        /// <summary>
        /// Copies the requested row elements into a new <see cref="Vector{T}"/>.
        /// </summary>
        /// <param name="rowIndex">The row to copy elements from.</param>
        /// <param name="columnIndex">The column to start copying from.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <param name="result">The <see cref="Vector{T}"/> to copy the column into.</param>
        /// <exception cref="ArgumentNullException">If the result <see cref="Vector{T}"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="rowIndex"/> is negative,
        /// or greater than or equal to the number of columns.</exception>        
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="columnIndex"/> is negative,
        /// or greater than or equal to the number of rows.</exception>        
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="columnIndex"/> + <paramref name="length"/>  
        /// is greater than or equal to the number of rows.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="length"/> is not positive.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <strong>result.Count &lt; length</strong>.</exception>
        public override void Row(int rowIndex, int columnIndex, int length, Vector<Complex> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (rowIndex >= RowCount || rowIndex < 0)
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }

            if (columnIndex >= ColumnCount || columnIndex < 0)
            {
                throw new ArgumentOutOfRangeException("columnIndex");
            }

            if (columnIndex + length > ColumnCount)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            if (length < 1)
            {
                throw new ArgumentOutOfRangeException("length", Resources.ArgumentMustBePositive);
            }

            if (result.Count < length)
            {
                throw new ArgumentOutOfRangeException("result", Resources.ArgumentVectorsSameLength);
            }

            var rowPointers = _storage.RowPointers;
            var values = _storage.Values;
            var valueCount = _storage.ValueCount;

            // Determine bounds in columnIndices array where this item should be searched (using rowIndex)
            var startIndex = rowPointers[rowIndex];
            var endIndex = rowIndex < rowPointers.Length - 1 ? rowPointers[rowIndex + 1] : valueCount;

            if (startIndex == endIndex)
            {
                result.Clear();
            }
            else
            {
                // If there are non-zero elements use base class implementation
                for (int i = columnIndex, j = 0; i < columnIndex + length; i++, j++)
                {
                    // Copy code from At(row, column) to avoid unnecessary lock
                    var index = _storage.FindItem(rowIndex, i);
                    result[j] = index >= 0 ? values[index] : Complex.Zero;
                }
            }
        }

        #region Static constructors for special matrices.
        /// <summary>
        /// Initializes a square <see cref="SparseMatrix"/> with all zero's except for ones on the diagonal.
        /// </summary>
        /// <param name="order">the size of the square matrix.</param>
        /// <returns>Identity <c>SparseMatrix</c></returns>
        /// <exception cref="ArgumentException">
        /// If <paramref name="order"/> is less than one.
        /// </exception>
        public static SparseMatrix Identity(int order)
        {
            var m = new SparseCompressedRowMatrixStorage<Complex>(order, order, Complex.Zero)
                {
                    ValueCount = order,
                    Values = new Complex[order],
                    ColumnIndices = new int[order]
                };

            for (var i = 0; i < order; i++)
            {
                m.Values[i] = 1d;
                m.ColumnIndices[i] = i;
                m.RowPointers[i] = i;
            }

            return new SparseMatrix(m);
        }
        #endregion

        /// <summary>
        /// Adds another matrix to this matrix.
        /// </summary>
        /// <param name="other">The matrix to add to this matrix.</param>
        /// <param name="result">The matrix to store the result of the addition.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the two matrices don't have the same dimensions.</exception>
        protected override void DoAdd(Matrix<Complex> other, Matrix<Complex> result)
        {
            var sparseOther = other as SparseMatrix;
            var sparseResult = result as SparseMatrix;
            if (sparseOther == null || sparseResult == null)
            {
                base.DoAdd(other, result);
                return;
            }

            if (ReferenceEquals(this, other))
            {
                if (!ReferenceEquals(this, result))
                {
                    CopyTo(result);
                }

                Control.LinearAlgebraProvider.ScaleArray(2.0, _storage.Values, _storage.Values);
                return;
            }

            SparseMatrix left;

            if (ReferenceEquals(sparseOther, sparseResult))
            {
                left = this;
            }
            else if (ReferenceEquals(this, sparseResult))
            {
                left = sparseOther;
            }
            else
            {
                CopyTo(sparseResult);
                left = sparseOther;
            }

            var leftStorage = left._storage;
            for (var i = 0; i < leftStorage.RowCount; i++)
            {
                // Get the begin / end index for the current row
                var startIndex = leftStorage.RowPointers[i];
                var endIndex = i < leftStorage.RowPointers.Length - 1 ? leftStorage.RowPointers[i + 1] : leftStorage.ValueCount;

                for (var j = startIndex; j < endIndex; j++)
                {
                    var columnIndex = leftStorage.ColumnIndices[j];
                    var resVal = leftStorage.Values[j] + result.At(i, columnIndex);
                    result.At(i, columnIndex, resVal);
                }
            }
        }

        /// <summary>
        /// Subtracts another matrix from this matrix.
        /// </summary>
        /// <param name="other">The matrix to subtract to this matrix.</param>
        /// <param name="result">The matrix to store the result of subtraction.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the two matrices don't have the same dimensions.</exception>
        protected override void DoSubtract(Matrix<Complex> other, Matrix<Complex> result)
        {
            var sparseOther = other as SparseMatrix;
            var sparseResult = result as SparseMatrix;

            if (sparseOther == null || sparseResult == null)
            {
                base.DoSubtract(other, result);
                return;
            }

            if (ReferenceEquals(this, other))
            {
                result.Clear();
                return;
            }

            var otherStorage = sparseOther._storage;

            if (ReferenceEquals(this, sparseResult))
            {
                for (var i = 0; i < otherStorage.RowCount; i++)
                {
                    // Get the begin / end index for the current row
                    var startIndex = otherStorage.RowPointers[i];
                    var endIndex = i < otherStorage.RowPointers.Length - 1 ? otherStorage.RowPointers[i + 1] : otherStorage.ValueCount;

                    for (var j = startIndex; j < endIndex; j++)
                    {
                        var columnIndex = otherStorage.ColumnIndices[j];
                        var resVal = sparseResult.At(i, columnIndex) - otherStorage.Values[j];
                        result.At(i, columnIndex, resVal);
                    }
                }
            }
            else
            {
                if (!ReferenceEquals(sparseOther, sparseResult))
                {
                    sparseOther.CopyTo(sparseResult);
                }

                sparseResult.Negate(sparseResult);

                var rowPointers = _storage.RowPointers;
                var columnIndices = _storage.ColumnIndices;
                var values = _storage.Values;
                var valueCount = _storage.ValueCount;

                for (var i = 0; i < RowCount; i++)
                {
                    // Get the begin / end index for the current row
                    var startIndex = rowPointers[i];
                    var endIndex = i < rowPointers.Length - 1 ? rowPointers[i + 1] : valueCount;

                    for (var j = startIndex; j < endIndex; j++)
                    {
                        var columnIndex = columnIndices[j];
                        var resVal = sparseResult.At(i, columnIndex) + values[j];
                        result.At(i, columnIndex, resVal);
                    }
                }
            }
        }

        /// <summary>
        /// Multiplies each element of the matrix by a scalar and places results into the result matrix.
        /// </summary>
        /// <param name="scalar">The scalar to multiply the matrix with.</param>
        /// <param name="result">The matrix to store the result of the multiplication.</param>
        protected override void DoMultiply(Complex scalar, Matrix<Complex> result)
        {
            if (scalar == 1.0)
            {
                CopyTo(result);
                return;
            }

            if (scalar == 0.0 || NonZerosCount == 0)
            {
                result.Clear();
                return;
            }

            var sparseResult = result as SparseMatrix;
            if (sparseResult == null)
            {
                result.Clear();

                var rowPointers = _storage.RowPointers;
                var columnIndices = _storage.ColumnIndices;
                var values = _storage.Values;

                for (var row = 0; row < RowCount; row++)
                {
                    var start = rowPointers[row];
                    var end = rowPointers[row + 1];

                    if (start == end)
                    {
                        continue;
                    }

                    for (var index = start; index < end; index++)
                    {
                        var column = columnIndices[index];
                        result.At(row, column, values[index] * scalar);
                    }
                }
            }
            else
            {
                if (!ReferenceEquals(this, result))
                {
                    CopyTo(sparseResult);
                }

                CommonParallel.For(0, NonZerosCount, index => sparseResult._storage.Values[index] *= scalar);
            }
        }

        /// <summary>
        /// Multiplies this matrix with another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoMultiply(Matrix<Complex> other, Matrix<Complex> result)
        {
            result.Clear();
            var columnVector = new DenseVector(other.RowCount);

            var rowPointers = _storage.RowPointers;
            var columnIndices = _storage.ColumnIndices;
            var values = _storage.Values;
            var valueCount = _storage.ValueCount;

            for (var row = 0; row < RowCount; row++)
            {
                // Get the begin / end index for the current row
                var startIndex = rowPointers[row];
                var endIndex = row < rowPointers.Length - 1 ? rowPointers[row + 1] : valueCount;
                if (startIndex == endIndex)
                {
                    continue;
                }

                for (var column = 0; column < other.ColumnCount; column++)
                {
                    // Multiply row of matrix A on column of matrix B
                    other.Column(column, columnVector);

                    var sum = Complex.Zero;
                    for (var index = startIndex; index < endIndex; index++)
                    {
                        sum += values[index] * columnVector[columnIndices[index]];
                    }

                    result.At(row, column, sum);
                }
            }
        }

        /// <summary>
        /// Multiplies this matrix with a vector and places the results into the result vector.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoMultiply(Vector<Complex> rightSide, Vector<Complex> result)
        {
            var rowPointers = _storage.RowPointers;
            var columnIndices = _storage.ColumnIndices;
            var values = _storage.Values;
            var valueCount = _storage.ValueCount;

            for (var row = 0; row < RowCount; row++)
            {
                // Get the begin / end index for the current row
                var startIndex = rowPointers[row];
                var endIndex = row < rowPointers.Length - 1 ? rowPointers[row + 1] : valueCount;
                if (startIndex == endIndex)
                {
                    continue;
                }

                var sum = Complex.Zero;
                for (var index = startIndex; index < endIndex; index++)
                {
                    sum += values[index] * rightSide[columnIndices[index]];
                }
                
                result[row] = sum;
            }
        }

        /// <summary>
        /// Multiplies this matrix with transpose of another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoTransposeAndMultiply(Matrix<Complex> other, Matrix<Complex> result)
        {
            var otherSparse = other as SparseMatrix;
            var resultSparse = result as SparseMatrix;

            if (otherSparse == null || resultSparse == null)
            {
                base.DoTransposeAndMultiply(other, result);
                return;
            }

            resultSparse.Clear();

            var rowPointers = _storage.RowPointers;
            var values = _storage.Values;
            var valueCount = _storage.ValueCount;

            var otherStorage = otherSparse._storage;

            for (var j = 0; j < RowCount; j++)
            {
                // Get the begin / end index for the row
                var startIndexOther = otherStorage.RowPointers[j];
                var endIndexOther = j < otherStorage.RowPointers.Length - 1 ? otherStorage.RowPointers[j + 1] : otherStorage.ValueCount;
                if (startIndexOther == endIndexOther)
                {
                    continue;
                }

                for (var i = 0; i < RowCount; i++)
                {
                    // Multiply row of matrix A on row of matrix B
                    // Get the begin / end index for the row
                    var startIndexThis = rowPointers[i];
                    var endIndexThis = i < rowPointers.Length - 1 ? rowPointers[i + 1] : valueCount;
                    if (startIndexThis == endIndexThis)
                    {
                        continue;
                    }

                    var sum = Complex.Zero;
                    for (var index = startIndexOther; index < endIndexOther; index++)
                    {
                        var ind = _storage.FindItem(i, otherStorage.ColumnIndices[index]);
                        if (ind >= 0)
                        {
                            sum += otherStorage.Values[index] * values[ind];
                        }
                    }

                    resultSparse._storage.At(i, j, sum + result.At(i, j));
                }
            }
        }

        /// <summary>
        /// Negate each element of this matrix and place the results into the result matrix.
        /// </summary>
        /// <param name="result">The result of the negation.</param>
        protected override void DoNegate(Matrix<Complex> result)
        {
            CopyTo(result);
            DoMultiply(-1, result);
        }

        /// <summary>
        /// Pointwise multiplies this matrix with another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to pointwise multiply with this one.</param>
        /// <param name="result">The matrix to store the result of the pointwise multiplication.</param>
        protected override void DoPointwiseMultiply(Matrix<Complex> other, Matrix<Complex> result)
        {
            result.Clear();

            var rowPointers = _storage.RowPointers;
            var columnIndices = _storage.ColumnIndices;
            var values = _storage.Values;
            var valueCount = _storage.ValueCount;

            for (var i = 0; i < RowCount; i++)
            {
                // Get the begin / end index for the current row
                var startIndex = rowPointers[i];
                var endIndex = i < rowPointers.Length - 1 ? rowPointers[i + 1] : valueCount;

                for (var j = startIndex; j < endIndex; j++)
                {
                    var resVal = values[j]*other.At(i, columnIndices[j]);
                    if (!resVal.IsZero())
                    {
                        result.At(i, columnIndices[j], resVal);
                    }
                }
            }
        }

        /// <summary>
        /// Pointwise divide this matrix by another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to pointwise divide this one by.</param>
        /// <param name="result">The matrix to store the result of the pointwise division.</param>
        protected override void DoPointwiseDivide(Matrix<Complex> other, Matrix<Complex> result)
        {
            result.Clear();

            var rowPointers = _storage.RowPointers;
            var columnIndices = _storage.ColumnIndices;
            var values = _storage.Values;
            var valueCount = _storage.ValueCount;

            for (var i = 0; i < RowCount; i++)
            {
                // Get the begin / end index for the current row
                var startIndex = rowPointers[i];
                var endIndex = i < rowPointers.Length - 1 ? rowPointers[i + 1] : valueCount;

                for (var j = startIndex; j < endIndex; j++)
                {
                    if (!values[j].IsZero())
                    {
                        result.At(i, columnIndices[j], values[j]/other.At(i, columnIndices[j]));
                    }
                }
            }
        }

        public override void KroneckerProduct(Matrix<Complex> other, Matrix<Complex> result)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != (RowCount*other.RowCount) || result.ColumnCount != (ColumnCount*other.ColumnCount))
            {
                throw DimensionsDontMatch<ArgumentOutOfRangeException>(this, other, result);
            }

            var rowPointers = _storage.RowPointers;
            var columnIndices = _storage.ColumnIndices;
            var values = _storage.Values;
            var valueCount = _storage.ValueCount;

            for (var i = 0; i < RowCount; i++)
            {
                // Get the begin / end index for the current row
                var startIndex = rowPointers[i];
                var endIndex = i < rowPointers.Length - 1 ? rowPointers[i + 1] : valueCount;

                for (var j = startIndex; j < endIndex; j++)
                {
                    if (!values[j].IsZero())
                    {
                        result.SetSubMatrix(i*other.RowCount, other.RowCount, columnIndices[j]*other.ColumnCount, other.ColumnCount, values[j]*other);
                    }
                }
            }
        }

        /// <summary>
        /// Iterates throw each element in the matrix (row-wise).
        /// </summary>
        /// <returns>The value at the current iteration along with its position (row, column, value).</returns>
        public override IEnumerable<Tuple<int, int, Complex>> IndexedEnumerator()
        {
            var rowPointers = _storage.RowPointers;
            var columnIndices = _storage.ColumnIndices;
            var values = _storage.Values;
            var valueCount = _storage.ValueCount;

            for (var row = 0; row < RowCount - 1; row++)
            {
                var start = rowPointers[row];
                var end = rowPointers[row + 1];

                if (start == end)
                {
                    continue;
                }

                for (var index = start; index < end; index++)
                {
                    yield return new Tuple<int, int, Complex>(row, columnIndices[index], values[index]);
                }
            }

            var lastRow = rowPointers.Length - 1;

            if (rowPointers[lastRow] < valueCount)
            {
                for (var index = rowPointers[lastRow]; index < valueCount; index++)
                {
                    yield return new Tuple<int, int, Complex>(lastRow, columnIndices[index], values[index]);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this matrix is symmetric.
        /// </summary>
        public override bool IsSymmetric
        {
            get
            {
                if (RowCount != ColumnCount)
                {
                    return false;
                }

                // todo: we might be able to speed this up by caching one half of the matrix
                var rowPointers = _storage.RowPointers;
                for (var row = 0; row < RowCount - 1; row++)
                {
                    var start = rowPointers[row];
                    var end = rowPointers[row + 1];

                    if (start == end)
                    {
                        continue;
                    }

                    if (!CheckIfOppositesAreEqual(start, end, row))
                    {
                        return false;
                    }
                }

                var lastRow = rowPointers.Length - 1;

                if (rowPointers[lastRow] < NonZerosCount)
                {
                    if (!CheckIfOppositesAreEqual(rowPointers[lastRow], _storage.ValueCount, lastRow))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Checks if opposites in a range are equal.
        /// </summary>
        /// <param name="start">The start of the range.</param>
        /// <param name="end">The end of the range.</param>
        /// <param name="row">The row the row to check.</param>
        /// <returns>If the values are equal or not.</returns>
        private bool CheckIfOppositesAreEqual(int start, int end, int row)
        {
            var columnIndices = _storage.ColumnIndices;
            var values = _storage.Values;

            for (var index = start; index < end; index++)
            {
                var column = columnIndices[index];
                var opposite = At(column, row);
                if (!values[index].Equals(opposite))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Adds two matrices together and returns the results.
        /// </summary>
        /// <remarks>This operator will allocate new memory for the result. It will
        /// choose the representation of either <paramref name="leftSide"/> or <paramref name="rightSide"/> depending on which
        /// is denser.</remarks>
        /// <param name="leftSide">The left matrix to add.</param>
        /// <param name="rightSide">The right matrix to add.</param>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> don't have the same dimensions.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SparseMatrix operator +(SparseMatrix leftSide, SparseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            if (leftSide.RowCount != rightSide.RowCount || leftSide.ColumnCount != rightSide.ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentOutOfRangeException>(leftSide, rightSide);
            }

            return (SparseMatrix)leftSide.Add(rightSide);
        }

        /// <summary>
        /// Returns a <strong>Matrix</strong> containing the same values of <paramref name="rightSide"/>. 
        /// </summary>
        /// <param name="rightSide">The matrix to get the values from.</param>
        /// <returns>A matrix containing a the same values as <paramref name="rightSide"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SparseMatrix operator +(SparseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return (SparseMatrix)rightSide.Clone();
        }

        /// <summary>
        /// Subtracts two matrices together and returns the results.
        /// </summary>
        /// <remarks>This operator will allocate new memory for the result. It will
        /// choose the representation of either <paramref name="leftSide"/> or <paramref name="rightSide"/> depending on which
        /// is denser.</remarks>
        /// <param name="leftSide">The left matrix to subtract.</param>
        /// <param name="rightSide">The right matrix to subtract.</param>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> don't have the same dimensions.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SparseMatrix operator -(SparseMatrix leftSide, SparseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            if (leftSide.RowCount != rightSide.RowCount || leftSide.ColumnCount != rightSide.ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentOutOfRangeException>(leftSide, rightSide);
            }

            return (SparseMatrix)leftSide.Subtract(rightSide);
        }

        /// <summary>
        /// Negates each element of the matrix.
        /// </summary>
        /// <param name="rightSide">The matrix to negate.</param>
        /// <returns>A matrix containing the negated values.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SparseMatrix operator -(SparseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return (SparseMatrix)rightSide.Negate();
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> by a constant and returns the result.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The constant to multiply the matrix by.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static SparseMatrix operator *(SparseMatrix leftSide, Complex rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return (SparseMatrix)leftSide.Multiply(rightSide);
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> by a constant and returns the result.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The constant to multiply the matrix by.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SparseMatrix operator *(Complex leftSide, SparseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return (SparseMatrix)rightSide.Multiply(leftSide);
        }

        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        /// <remarks>This operator will allocate new memory for the result. It will
        /// choose the representation of either <paramref name="leftSide"/> or <paramref name="rightSide"/> depending on which
        /// is denser.</remarks>
        /// <param name="leftSide">The left matrix to multiply.</param>
        /// <param name="rightSide">The right matrix to multiply.</param>
        /// <returns>The result of multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the dimensions of <paramref name="leftSide"/> or <paramref name="rightSide"/> don't conform.</exception>
        public static SparseMatrix operator *(SparseMatrix leftSide, SparseMatrix rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            if (leftSide.ColumnCount != rightSide.RowCount)
            {
                throw DimensionsDontMatch<ArgumentException>(leftSide, rightSide);
            }

            return (SparseMatrix)leftSide.Multiply(rightSide);
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> and a Vector.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The vector to multiply.</param>
        /// <returns>The result of multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SparseVector operator *(SparseMatrix leftSide, SparseVector rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return (SparseVector)leftSide.Multiply(rightSide);
        }

        /// <summary>
        /// Multiplies a Vector and a <strong>Matrix</strong>.
        /// </summary>
        /// <param name="leftSide">The vector to multiply.</param>
        /// <param name="rightSide">The matrix to multiply.</param>
        /// <returns>The result of multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SparseVector operator *(SparseVector leftSide, SparseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return (SparseVector)rightSide.LeftMultiply(leftSide);
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> by a constant and returns the result.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The constant to multiply the matrix by.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static SparseMatrix operator %(SparseMatrix leftSide, Complex rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return (SparseMatrix)leftSide.Modulus(rightSide);
        }
 

        /// <summary>
        /// Tests  all the elements of a matrix for a conditon and returns a Matrix with elements of 1.0 where this conditions is true.  
        /// The condition is given using System.Predicate;
        /// </summary>
        /// <param name="matchCondition">System.Predicate. The condition that the matrix elemets are tested for.</param>
        /// <returns> The resultant Matrix.</returns>
        /// <exception cref="ArgumentNullException">If matchCondition is <see langword="null" />.</exception>
        public override Matrix<Complex> FindMask(Predicate<Complex> matchCondition)
        {

            Matrix<Complex> result = new SparseMatrix(RowCount, ColumnCount);

            for (var i = 0; i < RowCount; i++)
            {
                // Get the begin / end index for the current row
                var startIndex = _storage.RowPointers[i];
                var endIndex = i < _storage.RowPointers.Length - 1 ? _storage.RowPointers[i + 1] : NonZerosCount;

                for (var j = startIndex; j < endIndex; j++)
                {
                    if (matchCondition(_storage.Values[j]))
                        result.At(i, _storage.ColumnIndices[j], 1.0);

                }
            }
            return result;
        }

        /// <summary>
        /// Tests  all the elements of a matrix for a conditon and returns and <see cref="IEnumerable{Tuple{int,int}}"/> of a Tuple{int,int} 
        /// of the indeces where this conditions is true.  
        /// The condition is given using System.Predicate.
        /// For example to find all elemets >1.0 :    
        ///    var myFoundIndeces=myMatrix.FindIndices(a => a > 1.0);
        ///    foreach( var currentTuple in myFoundIndeces)
        ///    {
        ///        Console.WriteLine("Found Indeces: rows{0}, columns{1}",currentTuple.Item1, currentTuple.Item2);
        //     }
        /// </summary>
        /// <param name="matchCondition">System.Predicate. The condition that the matrix elemets are tested for.</param>
        /// <returns> The request <see cref="IEnumerable{Tuple{int,int}}"/> of the row and column indexes respectivly.</returns>
        /// <exception cref="ArgumentNullException">If matchCondition is <see langword="null" />.</exception>
        public override IEnumerable<Tuple<int,int>> FindIndices(Predicate<Complex> matchCondition)
        {

            for (var i = 0; i < RowCount; i++)
            {
                // Get the begin / end index for the current row
                var startIndex = _storage.RowPointers[i];
                var endIndex = i < _storage.RowPointers.Length - 1 ? _storage.RowPointers[i + 1] : NonZerosCount;

                for (var j = startIndex; j < endIndex; j++)
                {
                    if (matchCondition(_storage.Values[j]))
                        yield return new Tuple<int,int>(i, _storage.ColumnIndices[j]);

                }
            }
            yield break;
        }



    }

}

