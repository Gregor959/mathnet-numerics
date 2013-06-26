// <copyright file="Matrix.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
// http://mathnetnumerics.codeplex.com
//
// Copyright (c) 2009-2013 Math.NET
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

namespace MathNet.Numerics.LinearAlgebra.Generic
{
    using System.Linq;
    using Factorization;
    using Numerics;
    using Properties;
    using Storage;
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Runtime;
    using Numerics.Random;

    /// <summary>
    /// Defines the base class for <c>Matrix</c> classes.
    /// </summary>
    /// <typeparam name="T">Supported data types are <c>double</c>, <c>single</c>, <see cref="Complex"/>, and <see cref="Complex32"/>.</typeparam>
    [Serializable]
    public abstract partial class Matrix<T> :
        IFormattable, IEquatable<Matrix<T>>
#if !PORTABLE
        , ICloneable
#endif
        where T : struct, IEquatable<T>, IFormattable
    {
        /// <summary>
        /// Initializes a new instance of the Matrix class.
        /// </summary>
        protected Matrix(MatrixStorage<T> storage)
        {
            Storage = storage;
            RowCount = storage.RowCount;
            ColumnCount = storage.ColumnCount;
        }

        /// <summary>
        /// Gets the raw matrix data storage.
        /// </summary>
        public MatrixStorage<T> Storage { get; private set; }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        /// <value>The number of columns.</value>
        public int ColumnCount { get; private set; }

        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        /// <value>The number of rows.</value>
        public int RowCount { get; private set; }

        /// <summary>
        /// Constructs matrix from a list of column vectors.
        /// </summary>
        /// <param name="columnVectors">The vectors to construct the matrix from.</param>
        /// <returns>The matrix constructed from the list of column vectors.</returns>
        /// <remarks>Creates a matrix of size Max(<paramref name="columnVectors"/>[i].Count) x <paramref name="columnVectors"/>.Count</remarks>
        [Obsolete("Use DenseMatrix.OfColumns or SparseMatrix.OfColumns instead. Scheduled for removal in v3.0.")]
        public static Matrix<T> CreateFromColumns(IList<Vector<T>> columnVectors)
        {
            if (columnVectors == null)
            {
                throw new ArgumentNullException("columnVectors");
            }

            if (columnVectors.Count == 0)
            {
                throw new ArgumentOutOfRangeException("columnVectors");
            }

            var rows = columnVectors[0].Count;
            var columns = columnVectors.Count;

            for (var column = 1; column < columns; column++)
            {
                rows = Math.Max(rows, columnVectors[column].Count);
            }

            var matrix = columnVectors[0].CreateMatrix(rows, columns);
            for (var j = 0; j < columns; j++)
            {
                for (var i = 0; i < columnVectors[j].Count; i++)
                {
                    matrix.At(i, j, columnVectors[j].At(i));
                }
            }

            return matrix;
        }

        /// <summary>
        /// Constructs matrix from a list of  row vectors.
        /// </summary>
        /// <param name="rowVectors">The vectors to construct the matrix from.</param>
        /// <returns>The matrix constructed from the list of row vectors.</returns>
        /// <remarks>Creates a matrix of size Max(<paramref name="rowVectors"/>.Count) x <paramref name="rowVectors"/>[i].Count</remarks>
        [Obsolete("Use DenseMatrix.OfRows or SparseMatrix.OfRows instead. Scheduled for removal in v3.0.")]
        public static Matrix<T> CreateFromRows(IList<Vector<T>> rowVectors)
        {
            if (rowVectors == null)
            {
                throw new ArgumentNullException("rowVectors");
            }

            if (rowVectors.Count == 0)
            {
                throw new ArgumentOutOfRangeException("rowVectors");
            }

            var rows = rowVectors.Count;
            var columns = rowVectors[0].Count;

            for (var row = 1; row < rows; row++)
            {
                columns = Math.Max(columns, rowVectors[row].Count);
            }

            var matrix = rowVectors[0].CreateMatrix(rows, columns);
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < rowVectors[i].Count; j++)
                {
                    matrix.At(i, j, rowVectors[i].At(j));
                }
            }

            return matrix;
        }

        /// <summary>
        /// Gets or sets the value at the given row and column, with range checking.
        /// </summary>
        /// <param name="row">
        /// The row of the element.
        /// </param>
        /// <param name="column">
        /// The column of the element.
        /// </param>
        /// <value>The value to get or set.</value>
        /// <remarks>This method is ranged checked. <see cref="At(int,int)"/> and <see cref="At(int,int,T)"/>
        /// to get and set values without range checking.</remarks>
        public T this[int row, int column]
        {
            [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
            //[MethodImpl(MethodImplOptions.AggressiveInlining)] .Net 4.5 only
            get { return Storage[row, column]; }

            [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
            //[MethodImpl(MethodImplOptions.AggressiveInlining)] .Net 4.5 only
            set { Storage[row, column] = value; }
        }

        /// <summary>
        /// Retrieves the requested element without range checking.
        /// </summary>
        /// <param name="row">
        /// The row of the element.
        /// </param>
        /// <param name="column">
        /// The column of the element.
        /// </param>
        /// <returns>
        /// The requested element.
        /// </returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        //[MethodImpl(MethodImplOptions.AggressiveInlining)] .Net 4.5 only
        public T At(int row, int column)
        {
            return Storage.At(row, column);
        }

        /// <summary>
        /// Sets the value of the given element without range checking.
        /// </summary>
        /// <param name="row">
        /// The row of the element.
        /// </param>
        /// <param name="column">
        /// The column of the element.
        /// </param>
        /// <param name="value">
        /// The value to set the element to.
        /// </param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        //[MethodImpl(MethodImplOptions.AggressiveInlining)] .Net 4.5 only
        public void At(int row, int column, T value)
        {
            Storage.At(row, column, value);
        }

        /// <summary>
        /// Sets all values to zero.
        /// </summary>
        public void Clear()
        {
            Storage.Clear();
        }

        /// <summary>
        /// Sets all values of a column to zero.
        /// </summary>
        public void ClearColumn(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= ColumnCount)
            {
                throw new ArgumentOutOfRangeException("columnIndex");
            }

            Storage.Clear(0,RowCount,columnIndex,1);
        }

        /// <summary>
        /// Sets all values of a row to zero.
        /// </summary>
        public void ClearRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= RowCount)
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }

            Storage.Clear(rowIndex, 1, 0, ColumnCount);
        }

        /// <summary>
        /// Sets all values of a submatrix to zero.
        /// </summary>
        public void ClearSubMatrix(int rowIndex, int rowCount, int columnIndex, int columnCount)
        {
            if (rowCount < 1)
            {
                throw new ArgumentOutOfRangeException("rowCount", Resources.ArgumentMustBePositive);
            }

            if (columnCount < 1)
            {
                throw new ArgumentOutOfRangeException("columnCount", Resources.ArgumentMustBePositive);
            }

            if (rowIndex + rowCount > RowCount || rowIndex < 0)
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }

            if (columnIndex + columnCount > ColumnCount || columnIndex < 0)
            {
                throw new ArgumentOutOfRangeException("columnIndex");
            }

            Storage.Clear(rowIndex, rowCount, columnIndex, columnCount);
        }

        /// <summary>
        /// Creates a clone of this instance.
        /// </summary>
        /// <returns>
        /// A clone of the instance.
        /// </returns>
        public Matrix<T> Clone()
        {
            var result = CreateMatrix(RowCount, ColumnCount);
            Storage.CopyToUnchecked(result.Storage, skipClearing: true);
            return result;
        }

        /// <summary>
        /// Copies the elements of this matrix to the given matrix.
        /// </summary>
        /// <param name="target">
        /// The matrix to copy values into.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If target is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If this and the target matrix do not have the same dimensions..
        /// </exception>
        public void CopyTo(Matrix<T> target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            Storage.CopyTo(target.Storage);
        }

        /// <summary>
        /// Creates a <strong>Matrix</strong> for the given number of rows and columns.
        /// </summary>
        /// <param name="numberOfRows">The number of rows.</param>
        /// <param name="numberOfColumns">The number of columns.</param>
        /// <param name="fullyMutable">True if all fields must be mutable (e.g. not a diagonal matrix).</param>
        /// <returns>
        /// A <strong>Matrix</strong> with the given dimensions.
        /// </returns>
        /// <remarks>
        /// Creates a matrix of the same matrix type as the current matrix.
        /// </remarks>
        public abstract Matrix<T> CreateMatrix(int numberOfRows, int numberOfColumns, bool fullyMutable = false);

        /// <summary>
        /// Creates a Vector with a the given dimension.
        /// </summary>
        /// <param name="size">The size of the vector.</param>
        /// <param name="fullyMutable">True if all fields must be mutable.</param>
        /// <returns>
        /// A Vector with the given dimension.
        /// </returns>
        /// <remarks>
        /// Creates a vector of the same type as the current matrix.
        /// </remarks>
        public abstract Vector<T> CreateVector(int size, bool fullyMutable = false);

        /// <summary>
        /// Copies a row into an Vector.
        /// </summary>
        /// <param name="index">The row to copy.</param>
        /// <returns>A Vector containing the copied elements.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is negative,
        /// or greater than or equal to the number of rows.</exception>
        public Vector<T> Row(int index)
        {
            if (index >= RowCount || index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            var ret = CreateVector(ColumnCount);
            Storage.CopySubRowToUnchecked(ret.Storage, index, 0, 0, ColumnCount);
            return ret;
        }

        /// <summary>
        /// Copies a row into to the given Vector.
        /// </summary>
        /// <param name="index">The row to copy.</param>
        /// <param name="result">The Vector to copy the row into.</param>
        /// <exception cref="ArgumentNullException">If the result vector is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is negative,
        /// or greater than or equal to the number of rows.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <b>this.Columns != result.Count</b>.</exception>
        public void Row(int index, Vector<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            Storage.CopyRowTo(result.Storage, index);
        }

        /// <summary>
        /// Copies the requested row elements into a new Vector.
        /// </summary>
        /// <param name="rowIndex">The row to copy elements from.</param>
        /// <param name="columnIndex">The column to start copying from.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <returns>A Vector containing the requested elements.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If:
        /// <list><item><paramref name="rowIndex"/> is negative,
        /// or greater than or equal to the number of rows.</item>
        /// <item><paramref name="columnIndex"/> is negative,
        /// or greater than or equal to the number of columns.</item>
        /// <item><c>(columnIndex + length) &gt;= Columns.</c></item></list></exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="length"/> is not positive.</exception>
        public Vector<T> Row(int rowIndex, int columnIndex, int length)
        {
            var ret = CreateVector(length);
            Storage.CopySubRowTo(ret.Storage, rowIndex, columnIndex, 0, length);
            return ret;
        }

        /// <summary>
        /// Copies the requested row elements into a new Vector.
        /// </summary>
        /// <param name="rowIndex">The row to copy elements from.</param>
        /// <param name="columnIndex">The column to start copying from.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <param name="result">The Vector to copy the column into.</param>
        /// <exception cref="ArgumentNullException">If the result Vector is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="rowIndex"/> is negative,
        /// or greater than or equal to the number of columns.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="columnIndex"/> is negative,
        /// or greater than or equal to the number of rows.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="columnIndex"/> + <paramref name="length"/>
        /// is greater than or equal to the number of rows.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="length"/> is not positive.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <strong>result.Count &lt; length</strong>.</exception>
        public void Row(int rowIndex, int columnIndex, int length, Vector<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            Storage.CopySubRowTo(result.Storage, rowIndex, columnIndex, 0, length);
        }

        /// <summary>
        /// Copies a column into a new Vector>.
        /// </summary>
        /// <param name="index">The column to copy.</param>
        /// <returns>A Vector containing the copied elements.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is negative,
        /// or greater than or equal to the number of columns.</exception>
        public Vector<T> Column(int index)
        {
            if (index >= ColumnCount || index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            var ret = CreateVector(RowCount);
            Storage.CopySubColumnToUnchecked(ret.Storage, index, 0, 0, RowCount);
            return ret;
        }

        /// <summary>
        /// Copies a column into to the given Vector.
        /// </summary>
        /// <param name="index">The column to copy.</param>
        /// <param name="result">The Vector to copy the column into.</param>
        /// <exception cref="ArgumentNullException">If the result Vector is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is negative,
        /// or greater than or equal to the number of columns.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <b>this.Rows != result.Count</b>.</exception>
        public void Column(int index, Vector<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            Storage.CopyColumnTo(result.Storage, index);
        }

        /// <summary>
        /// Copies the requested column elements into a new Vector.
        /// </summary>
        /// <param name="columnIndex">The column to copy elements from.</param>
        /// <param name="rowIndex">The row to start copying from.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <returns>A Vector containing the requested elements.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If:
        /// <list><item><paramref name="columnIndex"/> is negative,
        /// or greater than or equal to the number of columns.</item>
        /// <item><paramref name="rowIndex"/> is negative,
        /// or greater than or equal to the number of rows.</item>
        /// <item><c>(rowIndex + length) &gt;= Rows.</c></item></list>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="length"/> is not positive.</exception>
        public Vector<T> Column(int columnIndex, int rowIndex, int length)
        {
            var ret = CreateVector(length);
            Storage.CopySubColumnTo(ret.Storage, columnIndex, rowIndex, 0, length);
            return ret;
        }

        /// <summary>
        /// Copies the requested column elements into the given vector.
        /// </summary>
        /// <param name="columnIndex">The column to copy elements from.</param>
        /// <param name="rowIndex">The row to start copying from.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <param name="result">The Vector to copy the column into.</param>
        /// <exception cref="ArgumentNullException">If the result Vector is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="columnIndex"/> is negative,
        /// or greater than or equal to the number of columns.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="rowIndex"/> is negative,
        /// or greater than or equal to the number of rows.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="rowIndex"/> + <paramref name="length"/>
        /// is greater than or equal to the number of rows.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="length"/> is not positive.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <strong>result.Count &lt; length</strong>.</exception>
        public void Column(int columnIndex, int rowIndex, int length, Vector<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            Storage.CopySubColumnTo(result.Storage, columnIndex, rowIndex, 0, length);
        }

        /// <summary>
        /// Returns a new matrix containing the upper triangle of this matrix.
        /// </summary>
        /// <returns>The upper triangle of this matrix.</returns>
        public virtual Matrix<T> UpperTriangle()
        {
            var ret = CreateMatrix(RowCount, ColumnCount);

            for (var row = 0; row < RowCount; row++)
            {
                for (var column = row; column < ColumnCount; column++)
                {
                    ret.At(row, column, At(row, column));
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns a new matrix containing the lower triangle of this matrix.
        /// </summary>
        /// <returns>The lower triangle of this matrix.</returns>
        public virtual Matrix<T> LowerTriangle()
        {
            var ret = CreateMatrix(RowCount, ColumnCount);

            for (var row = 0; row < RowCount; row++)
            {
                for (var column = 0; column <= row && column < ColumnCount; column++)
                {
                    ret.At(row, column, At(row, column));
                }
            }

            return ret;
        }

        /// <summary>
        /// Puts the lower triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public virtual void LowerTriangle(Matrix<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != RowCount || result.ColumnCount != ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, result, "result");
            }

            for (var row = 0; row < RowCount; row++)
            {
                for (var column = 0; column < ColumnCount; column++)
                {
                    result.At(row, column, row >= column ? At(row, column) : Zero);
                }
            }
        }

        /// <summary>
        /// Puts the upper triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public virtual void UpperTriangle(Matrix<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != RowCount || result.ColumnCount != ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, result, "result");
            }

            for (var row = 0; row < RowCount; row++)
            {
                for (var column = 0; column < ColumnCount; column++)
                {
                    result.At(row, column, row <= column ? At(row, column) : Zero);
                }
            }
        }

        /// <summary>
        /// Creates a matrix that contains the values from the requested sub-matrix.
        /// </summary>
        /// <param name="rowIndex">The row to start copying from.</param>
        /// <param name="rowCount">The number of rows to copy. Must be positive.</param>
        /// <param name="columnIndex">The column to start copying from.</param>
        /// <param name="columnCount">The number of columns to copy. Must be positive.</param>
        /// <returns>The requested sub-matrix.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If: <list><item><paramref name="rowIndex"/> is
        /// negative, or greater than or equal to the number of rows.</item>
        /// <item><paramref name="columnIndex"/> is negative, or greater than or equal to the number
        /// of columns.</item>
        /// <item><c>(columnIndex + columnLength) &gt;= Columns</c></item>
        /// <item><c>(rowIndex + rowLength) &gt;= Rows</c></item></list></exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="rowCount"/> or <paramref name="columnCount"/>
        /// is not positive.</exception>
        public virtual Matrix<T> SubMatrix(int rowIndex, int rowCount, int columnIndex, int columnCount)
        {
            var target = CreateMatrix(rowCount, columnCount);
            Storage.CopySubMatrixTo(target.Storage, rowIndex, 0, rowCount, columnIndex, 0, columnCount, skipClearing: true);
            return target;
        }

        /// <summary>
        /// Returns the elements of the diagonal in a Vector.
        /// </summary>
        /// <returns>The elements of the diagonal.</returns>
        /// <remarks>For non-square matrices, the method returns Min(Rows, Columns) elements where
        /// i == j (i is the row index, and j is the column index).</remarks>
        public virtual Vector<T> Diagonal()
        {
            var min = Math.Min(RowCount, ColumnCount);
            var diagonal = CreateVector(min);

            for (var i = 0; i < min; i++)
            {
                diagonal.At(i, At(i, i));
            }

            return diagonal;
        }

        /// <summary>
        /// Returns a new matrix containing the lower triangle of this matrix. The new matrix
        /// does not contain the diagonal elements of this matrix.
        /// </summary>
        /// <returns>The lower triangle of this matrix.</returns>
        public virtual Matrix<T> StrictlyLowerTriangle()
        {
            var result = CreateMatrix(RowCount, ColumnCount);

            for (var row = 0; row < RowCount; row++)
            {
                for (var column = 0; column < row; column++)
                {
                    result.At(row, column, At(row, column));
                }
            }

            return result;
        }

        /// <summary>
        /// Puts the strictly lower triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public virtual void StrictlyLowerTriangle(Matrix<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != RowCount || result.ColumnCount != ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, result, "result");
            }

            for (var row = 0; row < RowCount; row++)
            {
                for (var column = 0; column < ColumnCount; column++)
                {
                    result.At(row, column, row > column ? At(row, column) : Zero);
                }
            }
        }

        /// <summary>
        /// Returns a new matrix containing the upper triangle of this matrix. The new matrix
        /// does not contain the diagonal elements of this matrix.
        /// </summary>
        /// <returns>The upper triangle of this matrix.</returns>
        public virtual Matrix<T> StrictlyUpperTriangle()
        {
            var result = CreateMatrix(RowCount, ColumnCount);

            for (var row = 0; row < RowCount; row++)
            {
                for (var column = row + 1; column < ColumnCount; column++)
                {
                    result.At(row, column, At(row, column));
                }
            }

            return result;
        }

        /// <summary>
        /// Puts the strictly upper triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public virtual void StrictlyUpperTriangle(Matrix<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != RowCount || result.ColumnCount != ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, result, "result");
            }

            for (var row = 0; row < RowCount; row++)
            {
                for (var column = 0; column < ColumnCount; column++)
                {
                    result.At(row, column, row < column ? At(row, column) : Zero);
                }
            }
        }

        /// <summary>
        /// Creates a new matrix and inserts the given column at the given index.
        /// </summary>
        /// <param name="columnIndex">The index of where to insert the column.</param>
        /// <param name="column">The column to insert.</param>
        /// <returns>A new matrix with the inserted column.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="column "/> is <see langword="null" />. </exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="columnIndex"/> is &lt; zero or &gt; the number of columns.</exception>
        /// <exception cref="ArgumentException">If the size of <paramref name="column"/> != the number of rows.</exception>
        public virtual Matrix<T> InsertColumn(int columnIndex, Vector<T> column)
        {
            if (column == null)
            {
                throw new ArgumentNullException("column");
            }

            if (columnIndex < 0 || columnIndex > ColumnCount)
            {
                throw new ArgumentOutOfRangeException("columnIndex");
            }

            if (column.Count != RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameRowDimension, "column");
            }

            var result = CreateMatrix(RowCount, ColumnCount + 1);

            for (var i = 0; i < columnIndex; i++)
            {
                result.SetColumn(i, Column(i));
            }

            result.SetColumn(columnIndex, column);

            for (var i = columnIndex + 1; i < ColumnCount + 1; i++)
            {
                result.SetColumn(i, Column(i - 1));
            }

            return result;
        }

        /// <summary>
        /// Copies the values of the given Vector to the specified column.
        /// </summary>
        /// <param name="columnIndex">The column to copy the values to.</param>
        /// <param name="column">The vector to copy the values from.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="column"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="columnIndex"/> is less than zero,
        /// or greater than or equal to the number of columns.</exception>
        /// <exception cref="ArgumentException">If the size of <paramref name="column"/> does not
        /// equal the number of rows of this <strong>Matrix</strong>.</exception>
        public void SetColumn(int columnIndex, Vector<T> column)
        {
            if (column == null)
            {
                throw new ArgumentNullException("column");
            }

            column.Storage.CopyToColumn(Storage, columnIndex);
        }

        /// <summary>
        /// Copies the values of the given array to the specified column.
        /// </summary>
        /// <param name="columnIndex">The column to copy the values to.</param>
        /// <param name="column">The array to copy the values from.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="column"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="columnIndex"/> is less than zero,
        /// or greater than or equal to the number of columns.</exception>
        /// <exception cref="ArgumentException">If the size of <paramref name="column"/> does not
        /// equal the number of rows of this <strong>Matrix</strong>.</exception>
        /// <exception cref="ArgumentException">If the size of <paramref name="column"/> does not
        /// equal the number of rows of this <strong>Matrix</strong>.</exception>
        public void SetColumn(int columnIndex, T[] column)
        {
            if (column == null)
            {
                throw new ArgumentNullException("column");
            }

            new DenseVectorStorage<T>(column.Length, column).CopyToColumn(Storage, columnIndex);
        }

        /// <summary>
        /// Creates a new matrix and inserts the given row at the given index.
        /// </summary>
        /// <param name="rowIndex">The index of where to insert the row.</param>
        /// <param name="row">The row to insert.</param>
        /// <returns>A new matrix with the inserted column.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="row"/> is <see langword="null" />. </exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="rowIndex"/> is &lt; zero or &gt; the number of rows.</exception>
        /// <exception cref="ArgumentException">If the size of <paramref name="row"/> != the number of columns.</exception>
        public virtual Matrix<T> InsertRow(int rowIndex, Vector<T> row)
        {
            if (row == null)
            {
                throw new ArgumentNullException("row");
            }

            if (rowIndex < 0 || rowIndex > RowCount)
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }

            if (row.Count != ColumnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameRowDimension, "row");
            }

            var result = CreateMatrix(RowCount + 1, ColumnCount);

            for (var i = 0; i < rowIndex; i++)
            {
                result.SetRow(i, Row(i));
            }

            result.SetRow(rowIndex, row);

            for (var i = rowIndex + 1; i < RowCount + 1; i++)
            {
                result.SetRow(i, Row(i - 1));
            }

            return result;
        }

        /// <summary>
        /// Copies the values of the given Vector to the specified row.
        /// </summary>
        /// <param name="rowIndex">The row to copy the values to.</param>
        /// <param name="row">The vector to copy the values from.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="row"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="rowIndex"/> is less than zero,
        /// or greater than or equal to the number of rows.</exception>
        /// <exception cref="ArgumentException">If the size of <paramref name="row"/> does not
        /// equal the number of columns of this <strong>Matrix</strong>.</exception>
        public void SetRow(int rowIndex, Vector<T> row)
        {
            if (row == null)
            {
                throw new ArgumentNullException("row");
            }

            row.Storage.CopyToRow(Storage, rowIndex);
        }

        /// <summary>
        /// Copies the values of the given array to the specified row.
        /// </summary>
        /// <param name="rowIndex">The row to copy the values to.</param>
        /// <param name="row">The array to copy the values from.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="row"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="rowIndex"/> is less than zero,
        /// or greater than or equal to the number of rows.</exception>
        /// <exception cref="ArgumentException">If the size of <paramref name="row"/> does not
        /// equal the number of columns of this <strong>Matrix</strong>.</exception>
        public void SetRow(int rowIndex, T[] row)
        {
            if (row == null)
            {
                throw new ArgumentNullException("row");
            }

            new DenseVectorStorage<T>(row.Length, row).CopyToRow(Storage, rowIndex);
        }

        /// <summary>
        /// Copies the values of a given matrix into a region in this matrix.
        /// </summary>
        /// <param name="rowIndex">The row to start copying to.</param>
        /// <param name="rowCount">The number of rows to copy. Must be positive.</param>
        /// <param name="columnIndex">The column to start copying to.</param>
        /// <param name="columnCount">The number of columns to copy. Must be positive.</param>
        /// <param name="subMatrix">The sub-matrix to copy from.</param>
        /// <exception cref="ArgumentOutOfRangeException">If: <list><item><paramref name="rowIndex"/> is
        /// negative, or greater than or equal to the number of rows.</item>
        /// <item><paramref name="columnIndex"/> is negative, or greater than or equal to the number
        /// of columns.</item>
        /// <item><c>(columnIndex + columnLength) &gt;= Columns</c></item>
        /// <item><c>(rowIndex + rowLength) &gt;= Rows</c></item></list></exception>
        /// <exception cref="ArgumentNullException">If <paramref name="subMatrix"/> is <see langword="null" /></exception>
        /// <item>the size of <paramref name="subMatrix"/> is not at least <paramref name="rowCount"/> x <paramref name="columnCount"/>.</item>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="rowCount"/> or <paramref name="columnCount"/>
        /// is not positive.</exception>
        public void SetSubMatrix(int rowIndex, int rowCount, int columnIndex, int columnCount, Matrix<T> subMatrix)
        {
            if (subMatrix == null)
            {
                throw new ArgumentNullException("subMatrix");
            }

            subMatrix.Storage.CopySubMatrixTo(Storage, 0, rowIndex, rowCount, 0, columnIndex, columnCount);
        }

        /// <summary>
        /// Copies the values of the given Vector to the diagonal.
        /// </summary>
        /// <param name="source">The vector to copy the values from. The length of the vector should be
        /// Min(Rows, Columns).</param>
        /// <exception cref="ArgumentNullException">If <paramref name="source"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the length of <paramref name="source"/> does not
        /// equal Min(Rows, Columns).</exception>
        /// <remarks>For non-square matrices, the elements of <paramref name="source"/> are copied to
        /// this[i,i].</remarks>
        public virtual void SetDiagonal(Vector<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var min = Math.Min(RowCount, ColumnCount);

            if (source.Count != min)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "source");
            }

            for (var i = 0; i < min; i++)
            {
                At(i, i, source.At(i));
            }
        }

        /// <summary>
        /// Copies the values of the given array to the diagonal.
        /// </summary>
        /// <param name="source">The array to copy the values from. The length of the vector should be
        /// Min(Rows, Columns).</param>
        /// <exception cref="ArgumentNullException">If <paramref name="source"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the length of <paramref name="source"/> does not
        /// equal Min(Rows, Columns).</exception>
        /// <remarks>For non-square matrices, the elements of <paramref name="source"/> are copied to
        /// this[i,i].</remarks>
        public virtual void SetDiagonal(T[] source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var min = Math.Min(RowCount, ColumnCount);

            if (source.Length != min)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "source");
            }

            for (var i = 0; i < min; i++)
            {
                At(i, i, source[i]);
            }
        }

        /// <summary>
        /// Returns the transpose of this matrix.
        /// </summary>
        /// <returns>The transpose of this matrix.</returns>
        public virtual Matrix<T> Transpose()
        {
            var ret = CreateMatrix(ColumnCount, RowCount);
            for (var j = 0; j < ColumnCount; j++)
            {
                for (var i = 0; i < RowCount; i++)
                {
                    ret.At(j, i, At(i, j));
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns the conjugate transpose of this matrix.
        /// </summary>
        /// <returns>The conjugate transpose of this matrix.</returns>
        public abstract Matrix<T> ConjugateTranspose();

        /// <summary>
        /// Permute the rows of a matrix according to a permutation.
        /// </summary>
        /// <param name="p">The row permutation to apply to this matrix.</param>
        public virtual void PermuteRows(Permutation p)
        {
            if (p.Dimension != RowCount)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "p");
            }

            // Get a sequence of inversions from the permutation.
            var inv = p.ToInversions();

            for (var i = 0; i < inv.Length; i++)
            {
                if (inv[i] != i)
                {
                    var q = inv[i];
                    for (var j = 0; j < ColumnCount; j++)
                    {
                        var temp = At(q, j);
                        At(q, j, At(i, j));
                        At(i, j, temp);
                    }
                }
            }
        }

        /// <summary>
        /// Permute the columns of a matrix according to a permutation.
        /// </summary>
        /// <param name="p">The column permutation to apply to this matrix.</param>
        public virtual void PermuteColumns(Permutation p)
        {
            if (p.Dimension != ColumnCount)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "p");
            }

            // Get a sequence of inversions from the permutation.
            var inv = p.ToInversions();

            for (var i = 0; i < inv.Length; i++)
            {
                if (inv[i] != i)
                {
                    var q = inv[i];
                    for (var j = 0; j < RowCount; j++)
                    {
                        var temp = At(j, q);
                        At(j, q, At(j, i));
                        At(j, i, temp);
                    }
                }
            }
        }

        /// <summary>
        ///  Concatenates this matrix with the given matrix.
        /// </summary>
        /// <param name="right">The matrix to concatenate.</param>
        /// <returns>The combined matrix.</returns>
        public Matrix<T> Append(Matrix<T> right)
        {
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            if (right.RowCount != RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameRowDimension);
            }

            var result = CreateMatrix(RowCount, ColumnCount + right.ColumnCount, fullyMutable: true);
            Storage.CopySubMatrixToUnchecked(result.Storage, 0, 0, RowCount, 0, 0, ColumnCount, skipClearing: true);
            right.Storage.CopySubMatrixToUnchecked(result.Storage, 0, 0, right.RowCount, 0, ColumnCount, right.ColumnCount, skipClearing: true);
            return result;
        }

        /// <summary>
        /// Concatenates this matrix with the given matrix and places the result into the result matrix.
        /// </summary>
        /// <param name="right">The matrix to concatenate.</param>
        /// <param name="result">The combined matrix.</param>
        public void Append(Matrix<T> right, Matrix<T> result)
        {
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            if (right.RowCount != RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameRowDimension);
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.ColumnCount != (ColumnCount + right.ColumnCount) || result.RowCount != RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameColumnDimension);
            }

            Storage.CopySubMatrixToUnchecked(result.Storage, 0, 0, RowCount, 0, 0, ColumnCount);
            right.Storage.CopySubMatrixToUnchecked(result.Storage, 0, 0, right.RowCount, 0, ColumnCount, right.ColumnCount);
        }

        /// <summary>
        /// Stacks this matrix on top of the given matrix and places the result into the result matrix.
        /// </summary>
        /// <param name="lower">The matrix to stack this matrix upon.</param>
        /// <returns>The combined matrix.</returns>
        /// <exception cref="ArgumentNullException">If lower is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If <strong>upper.Columns != lower.Columns</strong>.</exception>
        public Matrix<T> Stack(Matrix<T> lower)
        {
            if (lower == null)
            {
                throw new ArgumentNullException("lower");
            }

            if (lower.ColumnCount != ColumnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameColumnDimension, "lower");
            }

            var result = CreateMatrix(RowCount + lower.RowCount, ColumnCount, fullyMutable: true);
            Storage.CopySubMatrixToUnchecked(result.Storage, 0, 0, RowCount, 0, 0, ColumnCount, skipClearing: true);
            lower.Storage.CopySubMatrixToUnchecked(result.Storage, 0, RowCount, lower.RowCount, 0, 0, lower.ColumnCount, skipClearing: true);
            return result;
        }

        /// <summary>
        /// Stacks this matrix on top of the given matrix and places the result into the result matrix.
        /// </summary>
        /// <param name="lower">The matrix to stack this matrix upon.</param>
        /// <param name="result">The combined matrix.</param>
        /// <exception cref="ArgumentNullException">If lower is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If <strong>upper.Columns != lower.Columns</strong>.</exception>
        public void Stack(Matrix<T> lower, Matrix<T> result)
        {
            if (lower == null)
            {
                throw new ArgumentNullException("lower");
            }

            if (lower.ColumnCount != ColumnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameColumnDimension, "lower");
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != (RowCount + lower.RowCount) || result.ColumnCount != ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, result, "result");
            }

            Storage.CopySubMatrixToUnchecked(result.Storage, 0, 0, RowCount, 0, 0, ColumnCount);
            lower.Storage.CopySubMatrixToUnchecked(result.Storage, 0, RowCount, lower.RowCount, 0, 0, lower.ColumnCount);
        }

        /// <summary>
        /// Diagonally stacks his matrix on top of the given matrix. The new matrix is a M-by-N matrix,
        /// where M = this.Rows + lower.Rows and N = this.Columns + lower.Columns.
        /// The values of off the off diagonal matrices/blocks are set to zero.
        /// </summary>
        /// <param name="lower">The lower, right matrix.</param>
        /// <exception cref="ArgumentNullException">If lower is <see langword="null" />.</exception>
        /// <returns>the combined matrix</returns>
        public Matrix<T> DiagonalStack(Matrix<T> lower)
        {
            if (lower == null)
            {
                throw new ArgumentNullException("lower");
            }

            var result = CreateMatrix(RowCount + lower.RowCount, ColumnCount + lower.ColumnCount, fullyMutable: true);
            Storage.CopySubMatrixToUnchecked(result.Storage, 0, 0, RowCount, 0, 0, ColumnCount);
            lower.Storage.CopySubMatrixToUnchecked(result.Storage, 0, RowCount, lower.RowCount, 0, ColumnCount, lower.ColumnCount);
            return result;
        }

        /// <summary>
        /// Diagonally stacks his matrix on top of the given matrix and places the combined matrix into the result matrix.
        /// </summary>
        /// <param name="lower">The lower, right matrix.</param>
        /// <param name="result">The combined matrix</param>
        /// <exception cref="ArgumentNullException">If lower is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException">If the result matrix is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not (this.Rows + lower.rows) x (this.Columns + lower.Columns).</exception>
        public void DiagonalStack(Matrix<T> lower, Matrix<T> result)
        {
            if (lower == null)
            {
                throw new ArgumentNullException("lower");
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != RowCount + lower.RowCount || result.ColumnCount != ColumnCount + lower.ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, result, "result");
            }

            Storage.CopySubMatrixToUnchecked(result.Storage, 0, 0, RowCount, 0, 0, ColumnCount);
            lower.Storage.CopySubMatrixToUnchecked(result.Storage, 0, RowCount, lower.RowCount, 0, ColumnCount, lower.ColumnCount);
        }

        /// <summary>Calculates the L1 norm.</summary>
        /// <returns>The L1 norm of the matrix.</returns>
        public abstract T L1Norm();

        /// <summary>Calculates the L2 norm.</summary>
        /// <returns>The L2 norm of the matrix.</returns>
        /// <remarks>For sparse matrices, the L2 norm is computed using a dense implementation of singular value decomposition.
        /// In a later release, it will be replaced with a sparse implementation.</remarks>
        public virtual T L2Norm()
        {
            return Svd<T>.Create(this, false).Norm2;
        }

        /// <summary>Calculates the Frobenius norm of this matrix.</summary>
        /// <returns>The Frobenius norm of this matrix.</returns>
        public abstract T FrobeniusNorm();

        /// <summary>Calculates the infinity norm of this matrix.</summary>
        /// <returns>The infinity norm of this matrix.</returns>
        public abstract T InfinityNorm();

        /// <summary>
        /// Gets a value indicating whether this matrix is symmetric.
        /// </summary>
        public virtual bool IsSymmetric
        {
            get
            {
                if (RowCount != ColumnCount)
                {
                    return false;
                }

                for (var row = 0; row < RowCount; row++)
                {
                    for (var column = row + 1; column < ColumnCount; column++)
                    {
                        if (!At(row, column).Equals(At(column, row)))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> that enumerates over the matrix columns.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> that enumerates over the matrix columns</returns>
        /// <seealso cref="IEnumerator{T}"/>
        public virtual IEnumerable<Tuple<int, Vector<T>>> ColumnEnumerator()
        {
            for (var i = 0; i < ColumnCount; i++)
            {
                yield return new Tuple<int, Vector<T>>(i, Column(i));
            }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> that enumerates the requested matrix columns.
        /// </summary>
        /// <param name="index">The column to start enumerating over.</param>
        /// <param name="length">The number of columns to enumerating over.</param>
        /// <returns>An <see cref="IEnumerator{T}"/> that enumerates over requested matrix columns.</returns>
        /// <seealso cref="IEnumerator{T}"/>
        /// <exception cref="ArgumentOutOfRangeException">If:
        /// <list><item><paramref name="index"/> is negative,
        /// or greater than or equal to the number of columns.</item>
        /// <item><c>(index + length) &gt;= Columns.</c></item></list>
        /// </exception>
        /// <exception cref="ArgumentException">If <paramref name="length"/> is not positive.</exception>
        public virtual IEnumerable<Tuple<int, Vector<T>>> ColumnEnumerator(int index, int length)
        {
            if (index >= ColumnCount || index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (index + length > ColumnCount)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            if (length < 1)
            {
                throw new ArgumentException(Resources.ArgumentMustBePositive, "length");
            }

            var maxIndex = index + length;
            for (var i = index; i < maxIndex; i++)
            {
                yield return new Tuple<int, Vector<T>>(i, Column(i));
            }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> that enumerates the requested matrix rows.
        /// </summary>
        /// <param name="index">The row to start enumerating over.</param>
        /// <param name="length">The number of rows to enumerating over.</param>
        /// <returns>An <see cref="IEnumerator{T}"/> that enumerates over requested matrix rows.</returns>
        /// <seealso cref="IEnumerator{T}"/>
        /// <exception cref="ArgumentOutOfRangeException">If:
        /// <list><item><paramref name="index"/> is negative,
        /// or greater than or equal to the number of rows.</item>
        /// <item><c>(index + length) &gt;= Rows.</c></item></list></exception>
        /// <exception cref="ArgumentException">If <paramref name="length"/> is not positive.</exception>
        public virtual IEnumerable<Tuple<int, Vector<T>>> RowEnumerator(int index, int length)
        {
            if (index >= RowCount || index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (index + length > RowCount)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            if (length < 1)
            {
                throw new ArgumentException(Resources.ArgumentMustBePositive, "length");
            }

            var maxi = index + length;
            for (var i = index; i < maxi; i++)
            {
                yield return new Tuple<int, Vector<T>>(i, Row(i));
            }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> that enumerates over the matrix rows.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> that enumerates over the matrix rows</returns>
        /// <seealso cref="IEnumerator{T}"/>
        public virtual IEnumerable<Tuple<int, Vector<T>>> RowEnumerator()
        {
            for (var i = 0; i < RowCount; i++)
            {
                yield return new Tuple<int, Vector<T>>(i, Row(i));
            }
        }

        /// <summary>
        /// Iterates through each element in the matrix (row-wise).
        /// </summary>
        /// <returns>The value at the current iteration along with its position (row, column, value).</returns>
        public virtual IEnumerable<Tuple<int, int, T>> IndexedEnumerator()
        {
            for (var row = 0; row < RowCount; row++)
            {
                for (var column = 0; column < ColumnCount; column++)
                {
                    yield return new Tuple<int, int, T>(row, column, At(row, column));
                }
            }
        }

        /// <summary>
        /// Returns this matrix as a multidimensional array.
        /// </summary>
        /// <returns>A multidimensional containing the values of this matrix.</returns>
        public T[,] ToArray()
        {
            return Storage.ToArray();
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
        public T[] ToColumnWiseArray()
        {
            return Storage.ToColumnMajorArray();
        }

        /// <summary>
        /// Returns the matrix's elements as an array with the data laid row-wise.
        /// </summary>
        /// <example><pre>
        /// 1, 2, 3
        /// 4, 5, 6  will be returned as  1, 2, 3, 4, 5, 6, 7, 8, 9
        /// 7, 8, 9
        /// </pre></example>
        /// <returns>An array containing the matrix's elements.</returns>
        public T[] ToRowWiseArray()
        {
            return Storage.ToRowMajorArray();
        }

        /// <summary>
        /// Applies a function to each value of this matrix and replaces the value with its result.
        /// If forceMapZero is not set to true, zero values may or may not be skipped depending
        /// on the actual data storage implementation (relevant mostly for sparse matrices).
        /// </summary>
        public void MapInplace(Func<T, T> f, bool forceMapZeros = false)
        {
            Storage.MapInplace(f, forceMapZeros);
        }

        /// <summary>
        /// Applies a function to each value of this matrix and replaces the value with its result.
        /// The row and column indices of each value (zero-based) are passed as first arguments to the function.
        /// If forceMapZero is not set to true, zero values may or may not be skipped depending
        /// on the actual data storage implementation (relevant mostly for sparse matrices).
        /// </summary>
        public void MapIndexedInplace(Func<int, int, T, T> f, bool forceMapZeros = false)
        {
            Storage.MapIndexedInplace(f, forceMapZeros);
        }




        /// <summary>
        /// Returns an <see cref="IEnumerator{int}"/> that enumerates over the matrix row indices. 0..RowCount-1
        /// </summary>
        /// <returns>An <see cref="IEnumerator{int}"/> that enumerates over the matrix row indices</returns>
        public IEnumerable<int> RowIndices()
        {
            for (int i = 0; i < RowCount; i++)
            {
                yield return i;
            }
            yield break;
        }


        /// <summary>
        /// Returns an <see cref="IEnumerator{int}"/> that enumerates over the matrix column indices. 0..ColumnCount-1
        /// </summary>
        /// <returns>An <see cref="IEnumerator{int}"/> that enumerates over the matrix column indices</returns>
        public IEnumerable<int> ColumnIndices()
        {
            for (int i = 0; i < ColumnCount; i++)
            {
                yield return i;
            }
            yield break;
        }

        /// <summary>
        ///  Shuffle the columns of a Matrix into a random Order.
        /// </summary>
        /// <returns>The Permutation of the columns relative to the previous order.</returns>
        public virtual Permutation ShuffleColumns()
        {
            Permutation rndPerm = SortingExtensions.RandomPermutation(this.ColumnCount);
            this.PermuteColumns(rndPerm);
            return rndPerm;
        }

        /// <summary>
        ///  Shuffle the columns of a Matrix into a random Order.
        /// </summary>
        /// <param name="rnd">An AbstractRandomNumberGenerator that is used to generate the random numbers used in the shuffling process.</param>
        /// <returns>The Permutation of the columns relative to the previous order.</returns>
        public virtual Permutation ShuffleColumns(AbstractRandomNumberGenerator rnd)
        {
            Permutation rndPerm = SortingExtensions.RandomPermutation(this.ColumnCount, rnd);
            this.PermuteColumns(rndPerm);
            return rndPerm;

        }

        /// <summary>
        /// Shuffles the Rows of a Matrix in a random Order
        /// </summary>
        /// <returns>The Permutation of the rows relative to the previous order.</returns>
        public virtual Permutation ShuffleRows()
        {
            Permutation rndPerm = SortingExtensions.RandomPermutation(this.RowCount);
            this.PermuteRows(rndPerm);
            return rndPerm;

        }

        /// <summary>
        /// Shuffles the Rows of a Matrix in a random Order
        /// </summary>
        /// <param name="rnd">An AbstractRandomNumberGenerator that is used to generate the random numbers used in the shuffling process.</param>
        /// <returns>The Permutation of the rows relative to the previous order.</returns>
        public virtual Permutation ShuffleRows(AbstractRandomNumberGenerator rnd)
        {
            Permutation rndPerm = SortingExtensions.RandomPermutation(this.RowCount, rnd);
            this.PermuteRows(rndPerm);
            return rndPerm;
        }


        /// <summary>
        /// Sort the columns of the matrix by a specific row and also returns the Permutation , 
        /// which can be used to arrange the columns of other matrices in the same order.
        /// E.g Sorting Matrix myMatrix on Row index 2: 
        /// myPermuation = myMatrix.SortByRow(2); 
        /// Would use 
        /// otherMatrix.PermuteColumns(myPermuation); to sort otherMatrix in the same order. 
        /// To get the Sorting order as the Matlab, Octave sort method [a,ind] = Sort(myVector),
        /// the vector 'ind's entries are equivalent to the entries of the IEnumerable : myPermutation.Inverse().Elements(),
        /// but using  0 based indices instead of 1 based indexes as in Matlab/ Octave.
        /// </summary>
        /// <param name="rowIndex">The row to sort the matrix by.</param>
        /// <returns>The permutation</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="rowIndex"/> is negative,
        /// or greater than or equal to the number of rows.</exception>
        public virtual Permutation SortByRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= RowCount)
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }


            Vector<T> rowVector = this.Row(rowIndex);
            int[] items = new int[ColumnCount];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = i;
            }


            Sorting.Sort(rowVector.ToList(), items);
            Permutation sortPerm = new Permutation(items).Inverse(); // need to get inverse of sorting indices.
            this.PermuteColumns(sortPerm);
            return sortPerm;
        }

        /// <summary>
        /// Sort the rows of the matrix by a specific column and also returns the Permutation , 
        /// which can be used to arrange the Rows of other matrices in the same order.
        /// E.g Sorting Matrix myMatrix on Row index 2: 
        /// myPermuation = myMatrix.SortByRow(2); 
        /// Would use 
        /// otherMatrix.PermuteColumns(myPermuation); to sort otherMatrix in the same order. 
        /// To get the Sorting order as the Matlab, Octave sort method [a,ind] = Sort(myVector),
        /// the vector 'ind's entries are equivalent to the entries of the IEnumerable : myPermutation.Inverse().Elements()
        /// , but using  0 based indices instead of 1 based indexes as in Matlab/ Octave.
        /// </summary>
        /// <param name="columnIndex">The column to sort the matrix by. </param>
        /// <returns>The resultant permutation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="columnIndex"/> is negative,
        /// or greater than or equal to the number of columns.</exception>
        public virtual Permutation SortByColumn(int columnIndex)
        {

            if (columnIndex < 0 || columnIndex >= ColumnCount)
            {
                throw new ArgumentOutOfRangeException("column");
            }

            Vector<T> ColVector = this.Column(columnIndex);
            int[] items = new int[RowCount];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = i;
            }

            Sorting.Sort(ColVector.ToList(), items);
            Permutation sortPerm = new Permutation(items).Inverse(); // need to get inverse of Sorting.

            this.PermuteRows(sortPerm);
            return sortPerm;
        }



        /// <summary>
        /// Tests  all the elements of a matrix for a conditon and returns a Matrix of 1.0 where this conditions is true.  
        /// The condition is given using System.Predicate;
        /// </summary>
        /// <param name="matchCondition">System.Predicate. The condition that the matrix elemets are tested for.</param>
        /// <returns> The resultant Matrix.</returns>
        /// <exception cref="ArgumentNullException">If matchCondition is <see langword="null" />.</exception>
        public virtual Matrix<T> FindMask(Predicate<T> matchCondition)
        {
            if (matchCondition == null) throw new ArgumentNullException("matchCondition");

            var result = CreateMatrix(RowCount, ColumnCount);

            for (var column = 0; column < ColumnCount; column++)
            {
                for (var row = 0; row < RowCount; row++)
                {
                    if (matchCondition(At(row, column)))
                    {
                        result.At(row, column, One);
                    }
                }
            }

            return result;
        }



        /// <summary>
        /// Set the element of a given Matrix to a spefic value, where the mask Matrix elements ==1. 
        /// The Mask Matrix needs to have the same dimension of the given matrix.
        /// </summary>
        /// <param name="mask">The mask Matrix with 1 and 0's. </param>
        /// <param name="value">The value to se the elements of the given matrix to.</param>
        /// <exception cref="ArgumentNullException">If the mask matrix is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the mask matrix's dimensions are not equal to this dimesions.</exception>
        public virtual void OnMaskSet(Matrix<T> mask, T value)
        {
            if (mask == null)
            {
                throw new ArgumentNullException("mask");
            }

            if (mask.ColumnCount != this.ColumnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameColumnDimension, "mask");
            }

            if (mask.RowCount != this.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameRowDimension, "mask");
            }


            for (var column = 0; column < ColumnCount; column++)
            {
                for (var row = 0; row < RowCount; row++)
                {
                    if (Equals(mask.At(row, column), One))
                        At(row, column, value);
                }
            }

        }


        /// <summary>
        /// Applies a function to each element of a given Matrix , where the mask Matrix value ==1. 
        /// The Mask Matrix needs to have the same dimension of the given matrix.
        /// </summary>
        /// <param name="mask">The mask Matrix , needs to be of the same dimesion as this. </param>
        /// <param name="func">The function to apply to the elements of the matrix.</param>
        /// <exception cref="ArgumentNullException">If the mask matrix is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException">If the Func is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the mask matrix's dimensions are not equal to this dimesions.</exception>
        public virtual void OnMaskApply(Matrix<T> mask, Func<T, T> func)
        {

            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            if (mask == null)
            {
                throw new ArgumentNullException("mask");
            }

            if (mask.ColumnCount != this.ColumnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameColumnDimension, "mask");
            }

            if (mask.RowCount != this.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameRowDimension, "mask");
            }


            for (var column = 0; column < ColumnCount; column++)
            {
                for (var row = 0; row < RowCount; row++)
                {
                    if (Equals(mask.At(row, column), One))
                        At(row, column, func(At(row, column)));
                }
            }

        }

        /// <summary>
        /// Applies a function to each element of a given Matrix. 
        /// </summary>
        /// <param name="func">The function to apply to the elements of the matrix.</param>
        /// <exception cref="ArgumentNullException">If the Func is <see langword="null" />.</exception>
        public virtual void OnMaskApply( Func<T, T> func)
        {

            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            for (var column = 0; column < ColumnCount; column++)
            {
                for (var row = 0; row < RowCount; row++)
                {
                        At(row, column, func(At(row, column)));
                }
            }

        }


        /// <summary>
        /// Creates an iterator on the element of the mask matrix    
        /// Returns the elements of a Matrix where Mask==1 and puts them in an Enumerable 
        /// </summary>
        /// <param name="mask"></param>
        /// <returns>The Matrix Elements enumerated</returns>
        public virtual IEnumerable<T> EnumerateMask(Matrix<T> mask)
        {
            if (mask == null)
            {
                throw new ArgumentNullException("mask");
            }

            if (mask.ColumnCount != this.ColumnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameColumnDimension, "mask");
            }

            if (mask.RowCount != this.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameRowDimension, "mask");
            }


            for (var column = 0; column < ColumnCount; column++)
            {
                for (var row = 0; row < RowCount; row++)
                {
                    if (Equals(mask.At(row, column), One))
                    {
                        yield return At(row, column);
                    }
                }
            }
            yield break;
        }


        /// <summary>
        /// Tests  all the elements of a matrix for a conditon and returns and <see cref="IEnumerable{Tuple{int,int}}"/> of a Tuple{int,int} 
        /// of the indices where this conditions is true.  
        /// The condition is given using System.Predicate.
        /// For example to find all elemets >1.0 :    
        ///    var myFoundindices=myMatrix.FindIndices(a => a > 1.0);
        ///    foreach( var currentTuple in myFoundindices)
        ///    {
        ///        Console.WriteLine("Found indices: rows{0}, columns{1}",currentTuple.Item1, currentTuple.Item2);
        //     }
        /// </summary>
        /// <param name="matchCondition">System.Predicate. The condition that the matrix elemets are tested for.</param>
        /// <returns> The request <see cref="IEnumerable{Tuple{int,int}}"/> of the row and column indexes respectivly.</returns>
        /// <exception cref="ArgumentNullException">If matchCondition is <see langword="null" />.</exception>
        public virtual IEnumerable<Tuple<int, int>> FindIndices(Predicate<T> matchCondition)
        {
            if (matchCondition == null) throw new ArgumentNullException("matchCondition");

            for (var column = 0; column < ColumnCount; column++)
            {
                for (var row = 0; row < RowCount; row++)
                {
                    if (matchCondition(At(row, column)))
                    {
                        yield return new Tuple<int, int>(row, column);
                    }
                }
            }
            yield break;
        }





        /// <summary>
        /// Creates a new Matrix with Column columnIndex removed
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns>The resultant Matrix with the column removed</returns>
        /// <exception cref="ArgumentOutOfRangeException">If:
        /// <paramref name="columnindex"/> is negative,
        /// or greater than or equal to the number of columns.</exception>        
        /// <exception cref="ArgumentException">If the the matrix only has 1 column. <see langword="null" />.</exception>
        public virtual Matrix<T> RemoveColumn(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= ColumnCount)
            {
                throw new ArgumentOutOfRangeException("columnIndex");
            }

            if (ColumnCount <= 1)
            {
                throw new ArgumentException("Cannot remove last Column and copy into a new matrix.");
            }

            if (columnIndex == 0)
            {   //return only right portion keep indexes as per general case
                return SubMatrix(0, RowCount, columnIndex + 1, ColumnCount - columnIndex - 1);
            }

            if (columnIndex == ColumnCount - 1)
            {  //return only left portion keep indexes as per general case
                return SubMatrix(0, RowCount, 0, columnIndex);
            }

            Matrix<T> m1 = SubMatrix(0, RowCount, 0, columnIndex);
            Matrix<T> m2 = SubMatrix(0, RowCount, columnIndex + 1, ColumnCount - columnIndex - 1);
            return m1.Append(m2);
        }

        /// <summary>
        /// Creates a new Matrix with Row RowIndex removed
        /// </summary>
        /// <param name="rowIndex"> The row to remove</param>
        /// <returns>The resultant Matrix with the row removed</returns>
        /// <exception cref="ArgumentOutOfRangeException">If:
        /// <list><item><paramref name="rowIndex"/> is negative or greater than or equal to the number of rows.</item>        
        /// <exception cref="ArgumenException">If the matrix only has one row. </exception>
        public virtual Matrix<T> RemoveRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= ColumnCount)
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }

            if (RowCount <= 1)
            {
                throw new ArgumentException("Cannot remove last Row and copy to a new matrix. The new matrix would be ill defined.");
            }

            if (rowIndex == 0)
            {  //return only lower portion keep indexes as per general case
                return SubMatrix(rowIndex + 1, RowCount - rowIndex - 1, 0, ColumnCount);
            }

            if (rowIndex == RowCount - 1)
            {   //return only upper portion keep indexes as per general case
                return SubMatrix(0, rowIndex, 0, ColumnCount);
            }


            Matrix<T> m1 = SubMatrix(0, rowIndex, 0, ColumnCount);
            Matrix<T> m2 = SubMatrix(rowIndex + 1, RowCount - rowIndex - 1, 0, ColumnCount);
            return m1.Stack(m2);
        }


        /// <summary>
        /// Creates a new Matrix with Row and Column of Index removed
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns>The resultant Matrix with the row and column removed</returns>
        /// <exception cref="ArgumentOutOfRangeException">If:
        /// <list><item><paramref name="index"/> is negative or greater than or equal to the number of columns and rows.</item>     
        /// <exception cref="ArgumenException">If the matrix only has one row.
        public virtual Matrix<T> RemoveRowAndColumn(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (index >= ColumnCount)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (index >= RowCount)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (RowCount <= 1)
            {
                throw new ArgumentException("Cannot remove last Row");
            }

            if (ColumnCount <= 1)
            {
                throw new ArgumentException("Cannot remove last Column");
            }

            if (index == 0)
            {  //return only M22 portion, keep indexes as per general case below
                return SubMatrix(index + 1, RowCount - index - 1, index + 1, ColumnCount - index - 1);
            }

            if (index == ColumnCount - 1)
            {   //return only M11 and M21 portion, keep indexes as per general case below
                Matrix<T> mm11 = SubMatrix(0, index, 0, index);
                Matrix<T> mm21 = SubMatrix(index + 1, RowCount - index - 1, 0, index);
                return mm11.Stack(mm21);
            }

            if (index == RowCount - 1)
            {   //return only M11 and M12 portion, keep indexes as per general case below
                Matrix<T> n11 = SubMatrix(0, index, 0, index);
                Matrix<T> n12 = SubMatrix(0, index, index + 1, ColumnCount - index - 1);
                return n11.Append(n12);
            }

            //  ---------
            // | M11 M12 |
            // | M21 M22 |
            // ----------
            Matrix<T> m11 = SubMatrix(0, index, 0, index);
            Matrix<T> m12 = SubMatrix(0, index, index + 1, ColumnCount - index - 1);
            Matrix<T> m21 = SubMatrix(index + 1, RowCount - index - 1, 0, index);
            Matrix<T> m22 = SubMatrix(index + 1, RowCount - index - 1, index + 1, ColumnCount - index - 1);

            return (m11.Append(m12)).Stack(m21.Append(m22));
        }


        /// <summary>
        /// Creates a matrix which is a submatrix from this one selecting the columns of a specific range and all the rows. 
        /// </summary>
        /// <param name="columnIndex">The index at which to start copying the columns</param>
        /// <param name="numberOfColumns">The number of columns to copy </param>
        /// <returns> The requested Matrix.</returns>
        public virtual Matrix<T> SelectColumns(int columnIndex, int numberOfColumns)
        {
            return SubMatrix(0, RowCount, columnIndex, numberOfColumns);
        }


        /// <summary>
        /// Creates a matrix which is a submatrix from this one selecting the rows of a specific range and all the columns. 
        /// </summary>
        /// <param name="rowIndex"> The Index at which to start copying the rows</param>
        /// <param name="numberOfRows"> The number of rows to copy</param>
        /// <returns>The requested Matrix</returns>
        public virtual Matrix<T> SelectRows(int rowIndex, int numberOfRows)
        {
            return SubMatrix(rowIndex, numberOfRows, 0, ColumnCount);

        }

        /// <summary>
        /// Copies the values of a given matrix into a region in this matrix. Copies all the rows and selected columns.
        /// </summary>
        /// <param name="columnIndex">The columns to start copying to.</param>
        /// <param name="numberOfColumns">The number of columns to copy. Must be positive.</param>
        /// <param name="aMatrix">The matrix to copy from.</param>
        public virtual void SetColumns(int columnIndex, int numberOfColumns, Matrix<T> aMatrix)
        {
            SetSubMatrix(0, RowCount, columnIndex, numberOfColumns, aMatrix);
        }

        /// <summary>
        /// Copies the values of a given matrix into a region in this matrix. Copies all the columns and selected rows.
        /// </summary>
        /// <param name="rowIndex">The rows to start copying to.</param>
        /// <param name="numberOfRows">The number of rows to copy. Must be positive.</param>
        /// <param name="aMatrix">The matrix to copy from.</param>
        public virtual void SetRows(int rowIndex, int numberOfRows, Matrix<T> aMatrix)
        {
            SetSubMatrix(rowIndex, numberOfRows, 0, ColumnCount, aMatrix);
        }



        /// <summary>
        /// Returns a matrix of selected Rows in the order of the given Enumerable.
        /// </summary>
        /// <param name="keep">An IEnumerable, using 0 based indexing of the the selected Rows. </param>
        /// <returns> A matrix that contains the selected rows</returns>
        public virtual Matrix<T> SelectRows(IEnumerable<int> keep)
        {
            var keepList = keep.ToList();
            var target = CreateMatrix( keepList.Count,this.ColumnCount);

            int targetRow = 0;
            foreach (int r in keepList)
            {
                if (r >= RowCount || r<0)
                {
                    throw new IndexOutOfRangeException("Matrix.SelectRows index out of range.");
                }
                Storage.CopySubMatrixTo(target.Storage,  r, targetRow, 1, 0, 0, ColumnCount);
                targetRow++;
            }
            return target;
        
        }


        /// <summary>
        /// Returns a matrix of selected Columns in the order of the given Enumerable.
        /// </summary>
        /// <param name="keep">An IEnumerable, using 0 based indexing of the the selected Columnss. </param>
        /// <returns> A matrix that contains the selected Columns</returns>
        public virtual Matrix<T> SelectColumns(IEnumerable<int> keep)
        {
            var keepList = keep.ToList();
            var target = CreateMatrix(this.RowCount, keepList.Count);
            
            int targetColumn = 0;
            foreach (int c in keepList)
            {
                if (c >= ColumnCount || c < 0) 
                         { 
                            throw new IndexOutOfRangeException("Matrix.SelectColumns index out of range."); 
                         }
                Storage.CopySubMatrixTo(target.Storage, 0, 0, RowCount, c, targetColumn, 1);
                targetColumn++;
            }
            return target;
        
        }


        // 1 based index extension. So columns, rows count 1,2,3... instead of 0,1,2,3.
        // I stand for Roman I. Romans did not have a number for 0 and there first number was roman 1 or I.


        /// <summary>
        ///  Retrieves the requested element without range checking.
        ///  The same as At(..) but using 1 based indexing. 
        /// </summary>
        /// <param name="IRowIndex">The row of the element using 1 based indexing.</param>
        /// <param name="IColumnIndex">he column of the element using 1 based indexing. </param>
        /// <returns>The requested element</returns>
        public T AtI(int IRowIndex, int IColumnIndex)
        {
            return At(IRowIndex - 1, IColumnIndex - 1);
        }

        /// <summary>
        /// Sets the value of the given element. The same as At(..,T value) but using 1 based indexing
        /// </summary>
        /// <param name="IRowIndex">The row of the element using 1 based indexing.</param>
        /// <param name="IColumnIndex">The colum of the element using 1 based indexing.</param>
        /// <param name="value">The value to set the element to.</param>
        public void AtI(int IRowIndex, int IColumnIndex, T value)
        {
            At(IRowIndex - 1, IColumnIndex - 1, value);
        }

        /// <summary>
        /// Copies a row into an Vector. Thee same as Row(int ) but using 1 based indexing.
        /// </summary>
        /// <param name="IRowIndex">The row to copy to using 1 based indexing.</param>
        /// <returns>A Vector containing the copied elements.</returns>
        public Vector<T> RowI(int IRowIndex)
        {
            return this.Row(IRowIndex - 1);
        }


        /// <summary>
        /// Copies a row into to the given Vector. Same as Row(..) but using 1 based indexing.
        /// </summary>
        /// <param name="IIndex">The row to copy using 1 based indexing.</param>
        /// <param name="result">The Vector to copy the row into.</param>
        public void RowI(int IIndex, Vector<T> result)
        {
            Row(IIndex - 1, 0, ColumnCount, result);
        }

        /// <summary>
        /// Copies the requested row elements into a new Vector. Same as method Row with same parameters but using 1 based indexing.
        /// </summary>
        /// <param name="IRowIndex">The row to copy elements from. using 1 based indexing</param>
        /// <param name="IColumnIndex">The column to start copying from but ising 1 based indexing.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <returns>A Vector containing the requested elements.</returns>
        public Vector<T> RowI(int IRowIndex, int IColumnIndex, int length)
        {
            var ret = CreateVector(length);
            Row(IRowIndex - 1, IColumnIndex - 1, length, ret);
            return ret;
        }


        /// <summary>
        /// Copies the requested row elements into a new Vector. Same as method Row with same paramter signiture but using 1 based indexing.
        /// </summary>
        /// <param name="IRowIndex">The row to copy elements from, using 1 based indexing.</param>
        /// <param name="IColumnIndex">The column to start copying from , using 1 based indexing.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <param name="result">The Vector to copy the column into.</param>
        public void RowI(int IRowIndex, int IColumnIndex, int length, Vector<T> result)
        {
            Row(IRowIndex - 1, IColumnIndex - 1, length, result);
        }

        /// <summary>
        /// Copies the requested column into a new Vector. Uses 1 based indexing for the column parameter.
        /// </summary>
        /// <param name="IColumnIndex">The column to copy elements from using 1 based indexing.</param>
        /// <returns>A Vector containing the requested elements.</returns>
        public Vector<T> ColumnI(int IIndex)
        {
            var result = CreateVector(RowCount);
            Column(IIndex - 1, 0, RowCount, result);
            return result;
        }

        /// <summary>
        /// Copies the requested column into the given vector. uses 1 based indexing for the IIndex Parameter..
        /// </summary>
        /// <param name="IIndex">The column to copy elements from, 1 based indexing.</param>
        /// <param name="result">The Vector to copy the column into.</param>
        public void ColumnI(int IIndex, Vector<T> result)
        {
            Column(IIndex - 1, 0, RowCount, result);
        }

        /// <summary>
        /// Copies the requested column elements into a new Vector. Uses 1 based indexing for the parameters.
        /// </summary>
        /// <param name="IColumnIndex">The column to copy elements from using 1 based indexing.</param>
        /// <param name="IRowIndex">The row to start copying from using 1 based indexing.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <returns>A Vector containing the requested elements.</returns>
        public Vector<T> ColumnI(int IColumnIndex, int IRowIndex, int length)
        {
            var result = CreateVector(length);
            Column(IColumnIndex - 1, IRowIndex - 1, length, result);
            return result;
        }

        /// <summary>
        /// Copies the requested column elements into the given vector. uses 1 based indexing.
        /// </summary>
        /// <param name="IColumnIndex">The column to copy elements from, 1 based indexing.</param>
        /// <param name="IRowIndex">The row to start copying from, 1 based indexing.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <param name="result">The Vector to copy the column into.</param>
        public void ColumnI(int IColumnIndex, int IRowIndex, int length, Vector<T> result)
        {
            Column(IColumnIndex - 1, IRowIndex - 1, length, result);
        }

        /// <summary>
        /// Creates a new matrix and inserts the given column at the given index, using 1 based indexing.
        /// </summary>
        /// <param name="IColumnIndex">The index of where to insert the column, 1 based indexing.</param>
        /// <param name="column">The column to insert.</param>
        /// <returns>A new matrix with the inserted column.</returns>
        public Matrix<T> InsertColumnI(int IColumnIndex, Vector<T> column)
        {
            return InsertColumn(IColumnIndex - 1, column);
        }

        /// <summary>
        /// Copies the values of the given array to the specified column, using 1 based indexing
        /// </summary>
        /// <param name="IColumnIndex">The column to copy the values to, using 1 based indexing.</param>
        /// <param name="column">The array to copy the values from.</param>
        public void SetColumnI(int IColumnIndex, T[] column)
        {
            SetColumn(IColumnIndex - 1, column);
        }

        /// <summary>
        /// Copies the values of the given Vector to the specified column, using 1 based indexing for IColumnIndex.
        /// </summary>
        /// <param name="IColumnIndex">The column to copy the values to, using 1 based indexing</param>
        /// <param name="column">The vector to copy the values from.</param>
        public void SetColumnI(int IColumnIndex, Vector<T> column)
        {
            SetColumn(IColumnIndex - 1, column);
        }


        /// <summary>
        /// Creates a new matrix and inserts the given row at the given index, using 1 based indexing.
        /// </summary>
        /// <param name="IRowIndex">The index of where to insert the row, using 1 based indexing</param>
        /// <param name="row">The row to insert.</param>
        /// <returns>A new matrix with the inserted Row.</returns>
        public Matrix<T> InsertRowI(int IRowIndex, Vector<T> row)
        {
            return InsertRow(IRowIndex - 1, row);
        }


        /// <summary>
        /// Copies the values of the given Vector to the specified row, using 1 based indexing for the IRowIndex parameter.
        /// </summary>
        /// <param name="IRowIndex">The row to copy the values to, using 1 based indexing.</param>
        /// <param name="row">The vector to copy the values from.</param>
        public void SetRowI(int IRowIndex, Vector<T> row)
        {
            SetRow(IRowIndex - 1, row);
        }

        /// <summary>
        /// Copies the values of the given array to the specified row, using 1 based indexing for the parameter IRowIndex.
        /// </summary>
        /// <param name="IRowIndex">The row to copy the values to, using 1 based indexing.</param>
        /// <param name="row">The array to copy the values from.</param>
        public void SetRowI(int IRowIndex, T[] row)
        {
            SetRow(IRowIndex - 1, row);
        }

        /// <summary>
        /// Creates a matrix that contains the values from the requested sub-matrix, using 1 based indexing.
        /// </summary>
        /// <param name="IRowIndex">The row to start copying from, using 1 based indexing, must be greater than zero.</param>
        /// <param name="rowLength">The number of rows to copy, using 1 based indexing. Must be positive.</param>
        /// <param name="IColumnIndex">The column to start copying from, using 1 based indexing. Must be positive.</param>
        /// <param name="columnLength">The number of columns to copy. Must be positive.</param>
        /// <returns>The requested sub-matrix.</returns>
        public Matrix<T> SubMatrixI(int IRowIndex, int rowLength, int IColumnIndex, int columnLength)
        {
            return SubMatrix(IRowIndex - 1, rowLength, IColumnIndex - 1, columnLength);
        }

        /// <summary>
        /// Copies the values of a given matrix into a region in this matrix, using 1 based indexing for the row and column indices.
        /// </summary>
        /// <param name="IRowIndex">The row to start copying to, using 1 based indexing. Must be postive.</param>
        /// <param name="rowLength">The number of rows to copy. Must be positive.</param>
        /// <param name="IColumnIndex">The column to start copying to, using 1 based indexing, must be positive.</param>
        /// <param name="columnLength">The number of columns to copy. Must be positive.</param>
        /// <param name="subMatrix">The sub-matrix to copy from.</param>
        public void SetSubMatrixI(int IRowIndex, int rowLength, int IColumnIndex, int columnLength, Matrix<T> subMatrix)
        {
            SetSubMatrix(IRowIndex - 1, rowLength, IColumnIndex - 1, columnLength, subMatrix);
        }

        /// <summary>
        /// Creates a matrix which is a submatrix from this one selecting the columns of a specific range and all the rows. 
        /// Uses 1 based indexing.
        /// </summary>
        /// <param name="IcolumnIndex">The index at which to start copying the columns, using 1 based indexing.</param>
        /// <param name="columnLength">The number of columns to copy.</param>
        /// <returns>The requested Matrix.</returns>
        public virtual Matrix<T> SelectColumnsI(int IcolumnIndex, int columnLength)
        {
            return SubMatrix(0, RowCount, IcolumnIndex - 1, columnLength);
        }

        /// <summary>
        /// Creates a matrix which is a submatrix from this one selecting the columns of a specific range and all the rows. 
        /// Uses 1 based indexing.
        /// </summary>
        /// <param name="IrowIndex">he index at which to start copying the rows, using 1 based indexing.</param>
        /// <param name="rowLength">The number of rows to copy.</param>
        /// <returns>The requested Matrix.</returns>
        public virtual Matrix<T> SelectRowsI(int IrowIndex, int rowLength)
        {
            return SubMatrix(IrowIndex - 1, rowLength, 0, ColumnCount);

        }


        /// <summary>
        /// Copies the values of a given matrix into a region in this matrix. Copies all the rows and selected columns.
        /// Uses 1 based indxing, otherwise the same as SetColumns(..
        /// </summary>
        /// <param name="IcolumnIndex">The first columns to start copying to. Used 1 based indexing.</param>
        /// <param name="columnLength">The number of columns to copy.</param>
        /// <param name="aMatrix">The matrix top copy from.</param>
        public virtual void SetColumnsI(int IcolumnIndex, int columnLength, Matrix<T> aMatrix)
        {
            SetSubMatrix(0, RowCount, IcolumnIndex - 1, columnLength, aMatrix);
        }

        /// <summary>
        /// Copies the values of a given matrix into a region in this matrix. Copies all the columns and selected rows.
        /// Uses 1 based indxing, otherwise the same as SetRows(..
        /// </summary>
        /// <param name="IrowIndex">The first rows to start copying to. Used 1 based indexing.</param>
        /// <param name="rowLength">The number of rows to copy.</param>
        /// <param name="aMatrix">THe matrix to copy from.</param>
        public virtual void SetRowsI(int IrowIndex, int rowLength, Matrix<T> aMatrix)
        {
            SetSubMatrix(IrowIndex - 1, rowLength, 0, ColumnCount, aMatrix);
        }





        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> that enumerates over the matrix columns.Uses 1 based indexing to label the columns.
        /// </summary>
        /// <param name="IIndex">The index of the column to start enumerate from , using 1 based indexing </param>
        /// <param name="length">The number of columns to enumerate </param>
        /// <returns>An <see cref="IEnumerable{T}"/> that enumerates over the matrix columns</returns>
        /// <see also cref="IEnumerable{T}" and ColumnEnumerator/>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="IIndex"/> is negative,
        /// or greater than or equal to the number of columns.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <b> Index+length greater than this matrixes columnCount</b>.</exception>
        public IEnumerable<Tuple<int, Vector<T>>> ColumnEnumeratorI(int IIndex, int length)
        {
            int index = IIndex - 1;
            if (index >= ColumnCount || index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (index + length > ColumnCount)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            if (length < 1)
            {
                throw new ArgumentException(Resources.ArgumentMustBePositive, "length");
            }

            var maxIndex = index + length;
            for (var i = index; i < maxIndex; i++)
            {
                yield return new Tuple<int, Vector<T>>(i + 1, Column(i));
            }
        }


        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> that enumerates over the matrix rows.Uses 1 based indexing to label the rows.
        /// </summary>
        /// <param name="IIndex">The index of the row to start enumerate from , using 1 based indexing </param>
        /// <param name="length">The number of rows to enumerate </param>
        /// <returns>An <see cref="IEnumerable{T}"/> that enumerates over the matrix columns</returns>
        /// <see also cref="IEnumerable{T}" and ColumnEnumerator/>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="IIndex"/> is negative,
        /// or greater than or equal to the number of columns.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <b> IIndex+length greater than this matrixes RowCount</b>.</exception>
        public IEnumerable<Tuple<int, Vector<T>>> RowEnumeratorI(int IIndex, int length)
        {
            int index = IIndex - 1;

            if (index >= RowCount || index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (index + length > RowCount)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            if (length < 1)
            {
                throw new ArgumentException(Resources.ArgumentMustBePositive, "length");
            }

            var maxi = index + length;
            for (var i = index; i < maxi; i++)
            {
                yield return new Tuple<int, Vector<T>>(i + 1, Row(i));
            }

        }


        /// <summary>
        /// Sort the columns of the matrix by a specific row using 1 based index and also returns the Permuation , 
        /// which can be used to arrange the Columns of other matrices in the same order.
        /// E.g Sorting Matrix myMatrix on Row 2: 
        /// myPermuation = myMatrix.SortByRowI(2); 
        /// Would use 
        /// otherMatrix.PermuteColumns(myPermuation); to sort otherMatrix in the same order. 
        /// To get the Sorting order of 1 based index as the Matlab, Octave sort method [a,ind] = Sort(myVector),
        /// the vector 'ind's entries are equivalent to the entries of the IEnumerable : myPermutation.Inverse().ElementsI()
        /// To get a 0 based indices instead of 1 based indexes use myPermutation.Inverse().Elements().
        /// </summary>
        /// <param name="IRowIndex">The row to sort the matrix by, 1 based indexing.</param>
        /// <returns>The Permutation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="IRowIndex"/> is negative,
        /// or greater than or equal to the number of rows.</exception>
        public Permutation SortByRowI(int IRowIndex)
        {
            Permutation zerobased = this.SortByRow(IRowIndex - 1);
            return zerobased;
        }


        /// <summary>
        /// Sort the rows of the matrix by a specific column using 1 based index and also returns the Permuation , 
        /// which can be used to arrange the Columns of other matrices in the same order.
        /// E.g Sorting Matrix myMatrix on Row 2: 
        /// myPermuation = myMatrix.SortByRowI(2); 
        /// Would use 
        /// otherMatrix.PermuteRows(myPermuation); to sort otherMatrix in the same order. 
        /// To get the Sorting order of 1 based index as the Matlab, Octave sort method [a,ind] = Sort(myVector),
        /// the vector 'ind's entries are equivalent to the entries of the IEnumerable : myPermutation.Inverse().ElementsI()
        /// To get a 0 based indices instead of 1 based indexes use myPermutation.Inverse().Elements().
        /// </summary>
        /// <param name="IColumnIndex">The Column to sort the matrix by using 1 based indexing.</param>
        /// <returns>The permutation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="IColumnIndex"/> is negative,
        /// or greater than or equal to the number of columns.</exception>
        public Permutation SortByColumnI(int IColumnIndex)
        {
            Permutation zerobased = this.SortByColumn(IColumnIndex - 1);
            return zerobased;
        }

        /// <summary>
        /// Creates a new Matrix with a Column removed.Uses 1 based Index.
        /// </summary>
        /// <param name="IcolumnIndex">The column to remove in the copy. Uses 1 based Index.</param>
        /// <returns>The resultant Matrix with the Column removed</returns>
        public Matrix<T> RemoveColumnI(int IcolumnIndex)
        {
            return RemoveColumn(IcolumnIndex - 1);
        }

        /// <summary>
        /// Creates a new Matrix with Row RowIndex removed. Uses 1 based Index.
        /// </summary>
        /// <param name="IrowIndex">The row to remove in the copy. Uses 1 based Index.</param>
        /// <returns>The resultant Matrix with the row removed</returns>
        public Matrix<T> RemoveRowI(int IrowIndex)
        {
            return RemoveRow(IrowIndex - 1);
        }


        /// <summary>
        /// Creates a new Matrix with Row and Column of IIndex removed. Uses 1 based Index.
        /// </summary>
        /// <param name="IIndex"> 1 based Index of the row and column to remove.</param>
        /// <returns>The resultant Matrix with the row and column removed</returns>
        public Matrix<T> RemoveRowIAndColumnI(int IIndex)
        {
            return RemoveRowAndColumn(IIndex - 1);
        }

        /// <summary>
        /// Tests  all the elements of a matrix for a conditon and returns and <see cref="IEnumerable{Tuple{int,int}}"/> of a Tuple{int,int} 
        /// of the indices where this conditions is true.  1 based indexing.
        /// The condition is given using System.Predicate.
        /// For example to find all elemets >1.0 :    
        ///    var myFoundindices=myMatrix.FindIndicesI(a => a > 1.0);
        ///    foreach( var currentTuple in myFoundindices)
        ///    {
        ///        Console.WriteLine("Found indices 1 based: Irows{0}, Icolumns{1}",currentTuple.Item1, currentTuple.Item2);
        //     }
        /// </summary>
        /// <param name="matchCondition">System.Predicate. The condition that the matrix elemets are tested for.</param>
        /// <returns> The request <see cref="IEnumerable{Tuple{int,int}}"/> of the row and column indexes respectivly.</returns>
        /// <exception cref="ArgumentNullException">If matchCondition is <see langword="null" />.</exception>
        public virtual IEnumerable<Tuple<int, int>> FindIndicesI(Predicate<T> matchCondition)
        {
            if (matchCondition == null) throw new ArgumentNullException("matchCondition");

            for (var column = 0; column < ColumnCount; column++)
            {
                for (var row = 0; row < RowCount; row++)
                {
                    if (matchCondition(At(row, column)))
                    {
                        yield return new Tuple<int, int>(row + 1, column + 1);
                    }
                }
            }
            yield break;
        }



        /// <summary>
        /// Returns an <see cref="IEnumerator{int}"/> that enumerates over the matrix row indices using 1 based indices:  1..RowCount
        /// </summary>
        /// <returns>An <see cref="IEnumerator{int}"/> that enumerates over the matrix row indices 1 based</returns>
        public IEnumerable<int> RowIndicesI()
        {
            for (int i = 0; i < RowCount; i++)
            {
                yield return i + 1;
            }
            yield break;
        }


        /// <summary>
        /// Returns an <see cref="IEnumerator{int}"/> that enumerates over the matrix column indices usung 1 based indices 1..ColumnCount
        /// </summary>
        /// <returns>An <see cref="IEnumerator{int}"/> that enumerates over the matrix column indices 1 based.</returns>
        public IEnumerable<int> ColumnIndicesI()
        {
            for (int i = 0; i < ColumnCount; i++)
            {
                yield return i + 1;
            }
            yield break;
        }

        /// <summary>
        /// Returns a matrix of selected Rows in the order of the given Enumerable.
        /// </summary>
        /// <param name="keep">An IEnumerable, using 1 based indexing of the the selected Rows. </param>
        /// <returns> A matrix that contains the selected rows</returns>
        public Matrix<T> SelectRowsI(IEnumerable<int> keep)
        {
        //    IEnumerable<Tuple<int, Vector<T>>> vRows = RowEnumeratorI(1, RowCount);

        //    //create an index field on keep because we want to sort the rows the same as in the Enumerator keep
        //    var keepAndIndex = keep.Select((item, index) => new { Row = item, Index = index });

        //    var selected     = from keeps in keepAndIndex
        //                       from elem in vRows
        //                       where (elem.Item1 == keeps.Row)
        //                       orderby keeps.Index
        //                       select elem.Item2;
        //    return Matrix<T>.CreateFromRows(selected.ToList());
        //
            return SelectRows(from row in keep select row - 1);
        }


        /// <summary>
        /// Returns a matrix of selected Columns in the order of the given Enumerable
        /// </summary>
        /// <param name="keep">An IEnumerable, using 1 based indexing of the the selected Columnss. </param>
        /// <returns> A matrix that contains the selected Columns</returns>
        public Matrix<T> SelectColumnsI(IEnumerable<int> keep)
        {
           // IEnumerable<Tuple<int, Vector<T>>> vColumns = ColumnEnumeratorI(1, ColumnCount);

           // //create an index field on keep because we want to sort the columns the same as in the Enumerator keep
           // var keepAndIndex = keep.Select((item, index) => new { Column = item, Index = index });

           // var selected = from keeps in keepAndIndex
           //                from elem in vColumns
           //                where (elem.Item1 == keeps.Column)
           //                orderby keeps.Index
           //                select elem.Item2;
           //return Matrix<T>.CreateFromColumns(selected.ToList());
            return SelectColumns(from column in keep select column - 1);
        }

    }
}
