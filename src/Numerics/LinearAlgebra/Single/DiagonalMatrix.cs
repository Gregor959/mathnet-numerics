// <copyright file="DiagonalMatrix.cs" company="Math.NET">
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

namespace MathNet.Numerics.LinearAlgebra.Single
{
    using Distributions;
    using Generic;
    using Properties;
    using Storage;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Numerics.Random;
    /// <summary>
    /// A matrix type for diagonal matrices.
    /// </summary>
    /// <remarks>
    /// Diagonal matrices can be non-square matrices but the diagonal always starts
    /// at element 0,0. A diagonal matrix will throw an exception if non diagonal
    /// entries are set. The exception to this is when the off diagonal elements are
    /// 0.0 or NaN; these settings will cause no change to the diagonal matrix.
    /// </remarks>
    [Serializable]
    [DebuggerDisplay("DiagonalMatrix {RowCount}x{ColumnCount}-Single")]
    public class DiagonalMatrix : Matrix
    {
        readonly DiagonalMatrixStorage<float> _storage;

        /// <summary>
        /// Gets the matrix's data.
        /// </summary>
        /// <value>The matrix's data.</value>
        readonly float[] _data;

        /// <summary>
        /// Create a new diagonal matrix straight from an initialized matrix storage instance.
        /// The storage is used directly without copying.
        /// Intended for advanced scenarios where you're working directly with
        /// storage for performance or interop reasons.
        /// </summary>
        public DiagonalMatrix(DiagonalMatrixStorage<float> storage)
            : base(storage)
        {
            _storage = storage;
            _data = _storage.Data;
        }

        /// <summary>
        /// Create a new square diagonal matrix with the given number of rows and columns.
        /// All cells of the matrix will be initialized to zero.
        /// Zero-length matrices are not supported.
        /// </summary>
        /// <exception cref="ArgumentException">If the order is less than one.</exception>
        public DiagonalMatrix(int order)
            : this(new DiagonalMatrixStorage<float>(order, order))
        {
        }

        /// <summary>
        /// Create a new diagonal matrix with the given number of rows and columns.
        /// All cells of the matrix will be initialized to zero.
        /// Zero-length matrices are not supported.
        /// </summary>
        /// <exception cref="ArgumentException">If the row or column count is less than one.</exception>
        public DiagonalMatrix(int rows, int columns)
            : this(new DiagonalMatrixStorage<float>(rows, columns))
        {
        }

        /// <summary>
        /// Create a new diagonal matrix with the given number of rows and columns.
        /// All diagonal cells of the matrix will be initialized to the provided value, all non-diagonal ones to zero.
        /// Zero-length matrices are not supported.
        /// </summary>
        /// <exception cref="ArgumentException">If the row or column count is less than one.</exception>
        public DiagonalMatrix(int rows, int columns, float diagonalValue)
            : this(rows, columns)
        {
            for (var i = 0; i < _data.Length; i++)
            {
                _data[i] = diagonalValue;
            }
        }

        /// <summary>
        /// Create a new diagonal matrix with the given number of rows and columns directly binding to a raw array.
        /// The array is assumed to contain the diagonal elements only and is used directly without copying.
        /// Very efficient, but changes to the array and the matrix will affect each other.
        /// </summary>
        public DiagonalMatrix(int rows, int columns, float[] diagonalStorage)
            : this(new DiagonalMatrixStorage<float>(rows, columns, diagonalStorage))
        {
        }

        /// <summary>
        /// Create a new diagonal matrix as a copy of the given other matrix.
        /// This new matrix will be independent from the other matrix.
        /// The matrix to copy from must be diagonal as well.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DiagonalMatrix OfMatrix(Matrix<float> matrix)
        {
            return new DiagonalMatrix(DiagonalMatrixStorage<float>.OfMatrix(matrix.Storage));
        }

        /// <summary>
        /// Create a new diagonal matrix as a copy of the given two-dimensional array.
        /// This new matrix will be independent from the provided array.
        /// The array to copy from must be diagonal as well.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DiagonalMatrix OfArray(float[,] array)
        {
            return new DiagonalMatrix(DiagonalMatrixStorage<float>.OfArray(array));
        }

        /// <summary>
        /// Create a new diagonal matrix and initialize each diagonal value from the provided indexed enumerable.
        /// Keys must be provided at most once, zero is assumed if a key is omitted.
        /// This new matrix will be independent from the enumerable.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DiagonalMatrix OfIndexedDiagonal(int rows, int columns, IEnumerable<Tuple<int, float>> diagonal)
        {
            return new DiagonalMatrix(DiagonalMatrixStorage<float>.OfIndexedEnumerable(rows, columns, diagonal));
        }

        /// <summary>
        /// Create a new diagonal matrix and initialize each diagonal value from the provided enumerable.
        /// This new matrix will be independent from the enumerable.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DiagonalMatrix OfDiagonal(int rows, int columns, IEnumerable<float> diagonal)
        {
            return new DiagonalMatrix(DiagonalMatrixStorage<float>.OfEnumerable(rows, columns, diagonal));
        }

        /// <summary>
        /// Create a new diagonal matrix and initialize each diagonal value using the provided init function.
        /// </summary>
        public static DiagonalMatrix Create(int rows, int columns, Func<int, float> init)
        {
            return new DiagonalMatrix(DiagonalMatrixStorage<float>.OfInit(rows, columns, init));
        }

        /// <summary>
        /// Create a new diagonal matrix with diagonal values sampled from the provided random distribution.
        /// </summary>
        public static DiagonalMatrix CreateRandom(int rows, int columns, IContinuousDistribution distribution)
        {
            return new DiagonalMatrix(DiagonalMatrixStorage<float>.OfInit(rows, columns,
                i => (float) distribution.Sample()));
        }

        /// <summary>
        /// Create a new diagonal matrix as a copy of the given two-dimensional array.
        /// This new matrix will be independent from the provided array.
        /// The array to copy from must be diagonal as well.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        [Obsolete("Use DiagonalMatrix.OfArray instead. Scheduled for removal in v3.0.")]
        public DiagonalMatrix(float[,] array)
            : this(DiagonalMatrixStorage<float>.OfArray(array))
        {
        }

        /// <summary>
        /// Create a new diagonal matrix as a copy of the given other matrix.
        /// This new matrix will be independent from the other matrix.
        /// The matrix to copy from must be diagonal as well.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        [Obsolete("Use DiagonalMatrix.OfMatrix instead. Scheduled for removal in v3.0.")]
        public DiagonalMatrix(Matrix<float> matrix)
            : this(DiagonalMatrixStorage<float>.OfMatrix(matrix.Storage))
        {
        }

        /// <summary>
        /// Creates a <c>DiagonalMatrix</c> for the given number of rows and columns.
        /// </summary>
        /// <param name="numberOfRows">The number of rows.</param>
        /// <param name="numberOfColumns">The number of columns.</param>
        /// <param name="fullyMutable">True if all fields must be mutable (e.g. not a diagonal matrix).</param>
        /// <returns>
        /// A <c>DiagonalMatrix</c> with the given dimensions.
        /// </returns>
        public override Matrix<float> CreateMatrix(int numberOfRows, int numberOfColumns, bool fullyMutable = false)
        {
            return fullyMutable
                ? (Matrix<float>) new SparseMatrix(numberOfRows, numberOfColumns)
                : new DiagonalMatrix(numberOfRows, numberOfColumns);
        }

        /// <summary>
        /// Creates a <see cref="Vector{T}"/> with a the given dimension.
        /// </summary>
        /// <param name="size">The size of the vector.</param>
        /// <param name="fullyMutable">True if all fields must be mutable.</param>
        /// <returns>
        /// A <see cref="Vector{T}"/> with the given dimension.
        /// </returns>
        public override Vector<float> CreateVector(int size, bool fullyMutable = false)
        {
            return new SparseVector(size);
        }

        #region Elementary operations

        /// <summary>
        /// Adds another matrix to this matrix.
        /// </summary>
        /// <param name="other">The matrix to add to this matrix.</param>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the two matrices don't have the same dimensions.</exception>
        public override Matrix<float> Add(Matrix<float> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (other.RowCount != RowCount || other.ColumnCount != ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentOutOfRangeException>(this, other, "other");
            }

            Matrix<float> result;
            if (other is DiagonalMatrix)
            {
                result = new DiagonalMatrix(RowCount, ColumnCount);
            }
            else
            {
                result = new DenseMatrix(RowCount, ColumnCount);
            }

            Add(other, result);
            return result;
        }

        /// <summary>
        /// Adds another matrix to this matrix.
        /// </summary>
        /// <param name="other">The matrix to add to this matrix.</param>
        /// <param name="result">The matrix to store the result of the addition.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the two matrices don't have the same dimensions.</exception>
        protected override void DoAdd(Matrix<float> other, Matrix<float> result)
        {
            var diagOther = other as DiagonalMatrix;
            var diagResult = result as DiagonalMatrix;

            if (diagOther == null || diagResult == null)
            {
                base.DoAdd(other, result);
            }
            else
            {
                Control.LinearAlgebraProvider.AddArrays(_data, diagOther._data, diagResult._data);
            }
        }

        /// <summary>
        /// Subtracts another matrix from this matrix.
        /// </summary>
        /// <param name="other">The matrix to subtract.</param>
        /// <returns>The result of the subtraction.</returns>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the two matrices don't have the same dimensions.</exception>
        public override Matrix<float> Subtract(Matrix<float> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (other.RowCount != RowCount || other.ColumnCount != ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentOutOfRangeException>(this, other, "other");
            }

            Matrix<float> result;
            if (other is DiagonalMatrix)
            {
                result = new DiagonalMatrix(RowCount, ColumnCount);
            }
            else
            {
                result = new DenseMatrix(RowCount, ColumnCount);
            }

            Subtract(other, result);
            return result;
        }

        /// <summary>
        /// Subtracts another matrix from this matrix.
        /// </summary>
        /// <param name="other">The matrix to subtract.</param>
        /// <param name="result">The matrix to store the result of the subtraction.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the two matrices don't have the same dimensions.</exception>
        protected override void DoSubtract(Matrix<float> other, Matrix<float> result)
        {
            var diagOther = other as DiagonalMatrix;
            var diagResult = result as DiagonalMatrix;

            if (diagOther == null || diagResult == null)
            {
                base.DoSubtract(other, result);
            }
            else
            {
                Control.LinearAlgebraProvider.SubtractArrays(_data, diagOther._data, diagResult._data);
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
        public override void SetDiagonal(float[] source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (source.Length != _data.Length)
            {
                throw new ArgumentException(Resources.ArgumentArraysSameLength, "source");
            }

            Buffer.BlockCopy(source, 0, _data, 0, source.Length * Constants.SizeOfFloat);
        }

        /// <summary>
        /// Copies the values of the given <see cref="Vector{T}"/> to the diagonal.
        /// </summary>
        /// <param name="source">The vector to copy the values from. The length of the vector should be
        /// Min(Rows, Columns).</param>
        /// <exception cref="ArgumentNullException">If <paramref name="source"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the length of <paramref name="source"/> does not
        /// equal Min(Rows, Columns).</exception>
        /// <remarks>For non-square matrices, the elements of <paramref name="source"/> are copied to
        /// this[i,i].</remarks>
        public override void SetDiagonal(Vector<float> source)
        {
            var denseSource = source as DenseVector;
            if (denseSource == null)
            {
                base.SetDiagonal(source);
                return;
            }

            if (_data.Length != denseSource.Values.Length)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "source");
            }

            Buffer.BlockCopy(denseSource.Values, 0, _data, 0, denseSource.Values.Length * Constants.SizeOfFloat);
        }

        /// <summary>
        /// Multiplies each element of the matrix by a scalar and places results into the result matrix.
        /// </summary>
        /// <param name="scalar">The scalar to multiply the matrix with.</param>
        /// <param name="result">The matrix to store the result of the multiplication.</param>
        /// <exception cref="ArgumentNullException">If the result matrix is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        protected override void DoMultiply(float scalar, Matrix<float> result)
        {
            if (scalar == 0.0)
            {
                result.Clear();
                return;
            }

            if (scalar == 1.0)
            {
                CopyTo(result);
                return;
            }

            var diagResult = result as DiagonalMatrix;
            if (diagResult == null)
            {
                base.Multiply(scalar, result);
            }
            else
            {
                if (!ReferenceEquals(this, result))
                {
                    CopyTo(diagResult);
                }

                Control.LinearAlgebraProvider.ScaleArray(scalar, _data, diagResult._data);
            }
        }

        /// <summary>
        /// Multiplies this matrix with another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException">If the result matrix is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If <strong>this.Columns != other.Rows</strong>.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the this.Rows x other.Columns.</exception>
        public override void Multiply(Matrix<float> other, Matrix<float> result)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (ColumnCount != other.RowCount || result.RowCount != RowCount || result.ColumnCount != other.ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, other, result);
            }

            var m = other as DiagonalMatrix;
            var r = result as DiagonalMatrix;

            if (m == null || r == null)
            {
                base.Multiply(other, result);
            }
            else
            {
                var thisDataCopy = new float[r._data.Length];
                var otherDataCopy = new float[r._data.Length];
                Buffer.BlockCopy(_data, 0, thisDataCopy, 0, (r._data.Length > _data.Length) ? _data.Length * Constants.SizeOfFloat : r._data.Length * Constants.SizeOfFloat);
                Buffer.BlockCopy(m._data, 0, otherDataCopy, 0, (r._data.Length > m._data.Length) ? m._data.Length * Constants.SizeOfFloat : r._data.Length * Constants.SizeOfFloat);

                Control.LinearAlgebraProvider.PointWiseMultiplyArrays(thisDataCopy, otherDataCopy, r._data);
            }
        }

        /// <summary>
        /// Multiplies this matrix with another matrix and returns the result.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <exception cref="ArgumentException">If <strong>this.Columns != other.Rows</strong>.</exception>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null" />.</exception>
        /// <returns>The result of multiplication.</returns>
        public override Matrix<float> Multiply(Matrix<float> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (ColumnCount != other.RowCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, other);
            }

            var result = other.CreateMatrix(RowCount, other.ColumnCount);
            Multiply(other, result);
            return result;
        }

        /// <summary>
        /// Multiplies this matrix with a vector and places the results into the result matrix.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If <strong>result.Count != this.RowCount</strong>.</exception>
        /// <exception cref="ArgumentException">If <strong>this.ColumnCount != <paramref name="rightSide"/>.Count</strong>.</exception>
        public override void Multiply(Vector<float> rightSide, Vector<float> result)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            if (ColumnCount != rightSide.Count)
            {
                throw DimensionsDontMatch<ArgumentException>(this, rightSide, "rightSide");
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (RowCount != result.Count)
            {
                throw DimensionsDontMatch<ArgumentException>(this, result, "result");
            }

            if (ReferenceEquals(rightSide, result))
            {
                var tmp = result.CreateVector(result.Count);
                Multiply(rightSide, tmp);
                tmp.CopyTo(result);
            }
            else
            {
                // Clear the result vector
                result.Clear();

                // Multiply the elements in the vector with the corresponding diagonal element in this.
                for (var r = 0; r < _data.Length; r++)
                {
                    result[r] = _data[r] * rightSide[r];
                }
            }
        }

        /// <summary>
        /// Left multiply a matrix with a vector ( = vector * matrix ) and place the result in the result vector.
        /// </summary>
        /// <param name="leftSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException">If the result matrix is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If <strong>result.Count != this.ColumnCount</strong>.</exception>
        /// <exception cref="ArgumentException">If <strong>this.RowCount != <paramref name="leftSide"/>.Count</strong>.</exception>
        public override void LeftMultiply(Vector<float> leftSide, Vector<float> result)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            if (RowCount != leftSide.Count)
            {
                throw DimensionsDontMatch<ArgumentException>(this, leftSide, "leftSide");
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (ColumnCount != result.Count)
            {
                throw DimensionsDontMatch<ArgumentException>(this, result, "result");
            }

            if (ReferenceEquals(leftSide, result))
            {
                var tmp = result.CreateVector(result.Count);
                LeftMultiply(leftSide, tmp);
                tmp.CopyTo(result);
            }
            else
            {
                // Clear the result vector
                result.Clear();

                // Multiply the elements in the vector with the corresponding diagonal element in this.
                for (var r = 0; r < _data.Length; r++)
                {
                    result[r] = _data[r] * leftSide[r];
                }
            }
        }

        /// <summary>
        /// Computes the determinant of this matrix.
        /// </summary>
        /// <returns>The determinant of this matrix.</returns>
        public override float Determinant()
        {
            if (RowCount != ColumnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSquare);
            }

            return _data.Aggregate(1.0f, (current, t) => current * t);
        }

        /// <summary>
        /// Returns the elements of the diagonal in a <see cref="DenseVector"/>.
        /// </summary>
        /// <returns>The elements of the diagonal.</returns>
        /// <remarks>For non-square matrices, the method returns Min(Rows, Columns) elements where
        /// i == j (i is the row index, and j is the column index).</remarks>
        public override Vector<float> Diagonal()
        {
            // TODO: Should we return reference to array? In current implementation we return copy of array, so changes in DenseVector will
            // not influence onto diagonal elements
            return new DenseVector((float[])_data.Clone());
        }

        /// <summary>
        /// Multiplies this matrix with transpose of another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException">If the result matrix is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If <strong>this.Columns != other.Rows</strong>.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the this.Rows x other.Columns.</exception>
        public override void TransposeAndMultiply(Matrix<float> other, Matrix<float> result)
        {
            var otherDiagonal = other as DiagonalMatrix;
            var resultDiagonal = result as DiagonalMatrix;

            if (otherDiagonal == null || resultDiagonal == null)
            {
                base.TransposeAndMultiply(other, result);
                return;
            }

            Multiply(otherDiagonal.Transpose(), result);
        }

        /// <summary>
        /// Multiplies this matrix with transpose of another matrix and returns the result.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <exception cref="ArgumentException">If <strong>this.Columns != other.Rows</strong>.</exception>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null" />.</exception>
        /// <returns>The result of multiplication.</returns>
        public override Matrix<float> TransposeAndMultiply(Matrix<float> other)
        {
            var otherDiagonal = other as DiagonalMatrix;
            if (otherDiagonal == null)
            {
                return base.TransposeAndMultiply(other);
            }

            if (ColumnCount != otherDiagonal.ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, otherDiagonal);
            }

            var result = other.CreateMatrix(RowCount, other.RowCount);
            TransposeAndMultiply(other, result);
            return result;
        }

        #endregion

        /// <summary>
        /// Returns the transpose of this matrix.
        /// </summary>
        /// <returns>The transpose of this matrix.</returns>
        public override Matrix<float> Transpose()
        {
            var ret = new DiagonalMatrix(ColumnCount, RowCount);
            Buffer.BlockCopy(_data, 0, ret._data, 0, _data.Length * Constants.SizeOfFloat);
            return ret;
        }

        /// <summary>Calculates the L1 norm.</summary>
        /// <returns>The L1 norm of the matrix.</returns>
        public override float L1Norm()
        {
            return _data.Aggregate(float.NegativeInfinity, (current, t) => Math.Max(current, Math.Abs(t)));
        }

        /// <summary>Calculates the L2 norm.</summary>
        /// <returns>The L2 norm of the matrix.</returns>
        public override float L2Norm()
        {
            return _data.Aggregate(float.NegativeInfinity, (current, t) => Math.Max(current, Math.Abs(t)));
        }

        /// <summary>Calculates the Frobenius norm of this matrix.</summary>
        /// <returns>The Frobenius norm of this matrix.</returns>
        public override float FrobeniusNorm()
        {
            var norm = _data.Sum(t => t * t);
            return Convert.ToSingle(Math.Sqrt(norm));
        }

        /// <summary>Calculates the infinity norm of this matrix.</summary>
        /// <returns>The infinity norm of this matrix.</returns>
        public override float InfinityNorm()
        {
            return L1Norm();
        }

        /// <summary>Calculates the condition number of this matrix.</summary>
        /// <returns>The condition number of the matrix.</returns>
        public override float ConditionNumber()
        {
            var maxSv = float.NegativeInfinity;
            var minSv = float.PositiveInfinity;
            foreach (var t in _data)
            {
                maxSv = Math.Max(maxSv, Math.Abs(t));
                minSv = Math.Min(minSv, Math.Abs(t));
            }

            return maxSv / minSv;
        }

        /// <summary>Computes the inverse of this matrix.</summary>
        /// <exception cref="ArgumentException">If <see cref="DiagonalMatrix"/> is not a square matrix.</exception>
        /// <exception cref="ArgumentException">If <see cref="DiagonalMatrix"/> is singular.</exception>
        /// <returns>The inverse of this matrix.</returns>
        public override Matrix<float> Inverse()
        {
            if (RowCount != ColumnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSquare);
            }

            var inverse = (DiagonalMatrix)Clone();
            for (var i = 0; i < _data.Length; i++)
            {
                if (_data[i] != 0.0)
                {
                    inverse._data[i] = 1.0f / _data[i];
                }
                else
                {
                    throw new ArgumentException(Resources.ArgumentMatrixNotSingular);
                }
            }

            return inverse;
        }

        /// <summary>
        /// Returns a new matrix containing the lower triangle of this matrix.
        /// </summary>
        /// <returns>The lower triangle of this matrix.</returns>
        public override Matrix<float> LowerTriangle()
        {
            return Clone();
        }

        /// <summary>
        /// Puts the lower triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public override void LowerTriangle(Matrix<float> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != RowCount || result.ColumnCount != ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, result, "result");
            }

            if (ReferenceEquals(this, result))
            {
                return;
            }

            result.Clear();
            for (var i = 0; i < _data.Length; i++)
            {
                result.At(i, i, _data[i]);
            }
        }

        /// <summary>
        /// Returns a new matrix containing the lower triangle of this matrix. The new matrix
        /// does not contain the diagonal elements of this matrix.
        /// </summary>
        /// <returns>The lower triangle of this matrix.</returns>
        public override Matrix<float> StrictlyLowerTriangle()
        {
            return new DiagonalMatrix(RowCount, ColumnCount);
        }

        /// <summary>
        /// Puts the strictly lower triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public override void StrictlyLowerTriangle(Matrix<float> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != RowCount || result.ColumnCount != ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, result, "result");
            }

            result.Clear();
        }

        /// <summary>
        /// Returns a new matrix containing the upper triangle of this matrix.
        /// </summary>
        /// <returns>The upper triangle of this matrix.</returns>
        public override Matrix<float> UpperTriangle()
        {
            return Clone();
        }

        /// <summary>
        /// Puts the upper triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public override void UpperTriangle(Matrix<float> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != RowCount || result.ColumnCount != ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, result, "result");
            }

            result.Clear();
            for (var i = 0; i < _data.Length; i++)
            {
                result.At(i, i, _data[i]);
            }
        }

        /// <summary>
        /// Returns a new matrix containing the upper triangle of this matrix. The new matrix
        /// does not contain the diagonal elements of this matrix.
        /// </summary>
        /// <returns>The upper triangle of this matrix.</returns>
        public override Matrix<float> StrictlyUpperTriangle()
        {
            return new DiagonalMatrix(RowCount, ColumnCount);
        }

        /// <summary>
        /// Puts the strictly upper triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public override void StrictlyUpperTriangle(Matrix<float> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != RowCount || result.ColumnCount != ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, result, "result");
            }

            result.Clear();
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
        public override Matrix<float> SubMatrix(int rowIndex, int rowCount, int columnIndex, int columnCount)
        {
            var target = rowIndex == columnIndex
                ? (Matrix<float>)new DiagonalMatrix(rowCount, columnCount)
                : new SparseMatrix(rowCount, columnCount);

            Storage.CopySubMatrixTo(target.Storage, rowIndex, 0, rowCount, columnIndex, 0, columnCount, skipClearing: true);
            return target;
        }

        /// <summary>
        /// Creates a new  <see cref="SparseMatrix"/> and inserts the given column at the given index.
        /// </summary>
        /// <param name="columnIndex">The index of where to insert the column.</param>
        /// <param name="column">The column to insert.</param>
        /// <returns>A new <see cref="SparseMatrix"/> with the inserted column.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="column "/> is <see langword="null" />. </exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="columnIndex"/> is &lt; zero or &gt; the number of columns.</exception>
        /// <exception cref="ArgumentException">If the size of <paramref name="column"/> != the number of rows.</exception>
        public override Matrix<float> InsertColumn(int columnIndex, Vector<float> column)
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

            var result = new SparseMatrix(RowCount, ColumnCount + 1);

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
        /// Creates a new  <see cref="SparseMatrix"/> and inserts the given row at the given index.
        /// </summary>
        /// <param name="rowIndex">The index of where to insert the row.</param>
        /// <param name="row">The row to insert.</param>
        /// <returns>A new  <see cref="SparseMatrix"/> with the inserted column.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="row"/> is <see langword="null" />. </exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="rowIndex"/> is &lt; zero or &gt; the number of rows.</exception>
        /// <exception cref="ArgumentException">If the size of <paramref name="row"/> != the number of columns.</exception>
        public override Matrix<float> InsertRow(int rowIndex, Vector<float> row)
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

            var result = new SparseMatrix(RowCount + 1, ColumnCount);

            for (var i = 0; i < rowIndex; i++)
            {
                result.At(i, i, At(i, i));
            }

            result.SetRow(rowIndex, row);

            for (var i = rowIndex + 1; i < result.RowCount; i++)
            {
                result.At(i, i - 1, At(i - 1, i - 1));
            }

            return result;
        }

        /// <summary>
        /// Permute the columns of a matrix according to a permutation.
        /// </summary>
        /// <param name="p">The column permutation to apply to this matrix.</param>
        /// <exception cref="InvalidOperationException">Always thrown</exception>
        /// <remarks>Permutation in diagonal matrix are senseless, because of matrix nature</remarks>
        public override void PermuteColumns(Permutation p)
        {
            throw new InvalidOperationException("Permutations in diagonal matrix are not allowed");
        }

        /// <summary>
        /// Permute the rows of a matrix according to a permutation.
        /// </summary>
        /// <param name="p">The row permutation to apply to this matrix.</param>
        /// <exception cref="InvalidOperationException">Always thrown</exception>
        /// <remarks>Permutation in diagonal matrix are senseless, because of matrix nature</remarks>
        public override void PermuteRows(Permutation p)
        {
            throw new InvalidOperationException("Permutations in diagonal matrix are not allowed");
        }

        /// <summary>
        /// Gets a value indicating whether this matrix is symmetric.
        /// </summary>
        public override bool IsSymmetric
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Computes the modulus for each element of the matrix.
        /// </summary>
        /// <param name="divisor">The divisor to use.</param>
        /// <param name="result">Matrix to store the results in.</param>
        protected override void DoModulus(float divisor, Matrix<float> result)
        {
            var denseResult = result as DiagonalMatrix;

            if (denseResult == null)
            {
                base.DoModulus(divisor, result);
            }
            else
            {
                if (!ReferenceEquals(this, result))
                {
                    CopyTo(result);
                }

                for (var index = 0; index < _data.Length; index++)
                {
                    denseResult._data[index] %= divisor;
                }
            }
        }

        #region Static constructors for special matrices.

        /// <summary>
        /// Initializes a square <see cref="DiagonalMatrix"/> with all zero's except for ones on the diagonal.
        /// </summary>
        /// <param name="order">the size of the square matrix.</param>
        /// <returns>A diagonal identity matrix.</returns>
        /// <exception cref="ArgumentException">
        /// If <paramref name="order"/> is less than one.
        /// </exception>
        public static DiagonalMatrix Identity(int order)
        {
            var m = new DiagonalMatrix(order);
            for (var i = 0; i < order; i++)
            {
                m._data[i] = 1.0f;
            }

            return m;
        }

        #endregion


        /// <summary>
        /// Sort the columns of the matrix by a specific row  (rowIndex) and also returns the Permuation , which can be used to arrange the Columns of other matrices in the same order as the Permutation.
        /// Would use otherMatrix.PermuteColumns(permutation) for this. 
        /// </summary>
        /// <param name="rowIndex">The row to sort the matrix by.</param>
        /// <returns>The permutation</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="rowIndex"/> is negative,
        /// or greater than or equal to the number of rows.</exception>
        public override Permutation SortByRow(int rowIndex)
        {
            throw new InvalidOperationException("Sort a diagonal matrix by rows is not allowed");
        }

        /// <summary>
        /// Sort the rows of the matrix by a specific column (columnIndex) and also returns the Permuation , which can be used to arrange the Rows of other matrices in the same order as the Permutation.
        /// Would use otherMatrix.PermuteRows(permutation) for this.
        /// </summary>
        /// <param name="columnIndex">The column to sort the matrix by. </param>
        /// <returns>The resultant permutation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="columnIndex"/> is negative,
        /// or greater than or equal to the number of columns.</exception>
        public override Permutation SortByColumn(int columnIndex)
        {
            throw new InvalidOperationException("Sort a diagonal matrix by columns is not allowed");
        }



        /// <summary>
        ///  Shuffle the columns of a Matrix into a random Order. Not allowed for Diagonal Matrices.
        /// </summary>
        /// <returns>The Permutation of the columns relative to the previous order.</returns>
        public override Permutation ShuffleColumns()
        {
            throw new InvalidOperationException("Shuffling Columns in diagonal matrix are not allowed");
        }

        /// <summary>
        ///  Shuffle the columns of a Matrix into a random Order. Not allowed for Diagonal Matrices.
        /// </summary>
        /// <param name="rnd">An AbstractRandomNumberGenerator that is used to generate the random numbers used in the shuffling process.</param>
        /// <returns>The Permutation of the columns relative to the previous order.</returns>
        public override Permutation ShuffleColumns(AbstractRandomNumberGenerator rnd)
        {
            throw new InvalidOperationException("Shuffling Columns in diagonal matrix are not allowed");
        }

        /// <summary>
        /// Shuffles the Rows of a Matrix in a random Order .Not allowed for Diagonal Matrices.
        /// </summary>
        /// <returns>The Permutation of the rows relative to the previous order.</returns>
        public override Permutation ShuffleRows()
        {
            throw new InvalidOperationException("Shuffling Rows in diagonal matrix are not allowed");
        }

        /// <summary>
        /// Shuffles the Rows of a Matrix in a random Order. Not allowed for Diagonal Matrices.
        /// </summary>
        /// <param name="rnd">An AbstractRandomNumberGenerator that is used to generate the random numbers used in the shuffling process.</param>
        /// <returns>The Permutation of the rows relative to the previous order.</returns>
        public override Permutation ShuffleRows(AbstractRandomNumberGenerator rnd)
        {
            throw new InvalidOperationException("Shuffling Rows in diagonal matrix are not allowed");
        }

        /// <summary>
        /// Returns a matrix of selected Rows in the order of the given Enumerable.
        /// </summary>
        /// <param name="keep">An Ienumberable, using 0 based indexing of the the selected Rows. </param>
        /// <returns> A matrix that contains the selected rows</returns>
        public override Matrix<float> SelectRows(IEnumerable<int> keep)
        {
            IEnumerable<Tuple<int, Vector<float>>> vRows = RowEnumerator(0, RowCount);

            //create an index field on keep because we want to sort the rows the same as in the Enumerator keep
            var keepAndIndex = keep.Select((item, index) => new { Row = item, Index = index });

            var selected = from keeps in keepAndIndex
                           from elem in vRows
                           where (elem.Item1 == keeps.Row)
                           orderby keeps.Index
                           select elem.Item2;

            return Matrix<float>.CreateFromRows(selected.ToList());
        }

        /// <summary>
        /// Returns a matrix of selected Columns in the order of the given Enumerable.
        /// </summary>
        /// <param name="keep">An Ienumberable, using 0 based indexing of the the selected Columnss. </param>
        /// <returns> A matrix that contains the selected Columns</returns>
        public override Matrix<float> SelectColumns(IEnumerable<int> keep)
        {
            IEnumerable<Tuple<int, Vector<float>>> vColumns = ColumnEnumerator(0, ColumnCount);

            //create an index field on keep because we want to sort the Columns the same as in the Enumerator keep
            var keepAndIndex = keep.Select((item, index) => new { Column = item, Index = index });

            var selected = from keeps in keepAndIndex
                           from elem in vColumns
                           where (elem.Item1 == keeps.Column)
                           orderby keeps.Index
                           select elem.Item2;

            return Matrix<float>.CreateFromColumns(selected.ToList());
        }


        /// <summary>
        /// Removes the Row of a Matrix.
        /// </summary>
        /// <param name="p">The row to removed.</param>
        /// <exception cref="InvalidOperationException">Always thrown</exception>
        /// <remarks>Removing only a Row in a diagonal matrix are senseless, because of matrix nature</remarks>
        public override Matrix<float> RemoveRow(int rowIndex)
        {
            throw new InvalidOperationException("Removing a row in a diagonal matrix is not allowed. use RemoveRowAndColumn");
        }


        /// <summary>
        /// Removes the Column of a Matrix.
        /// </summary>
        /// <param name="p">The row to removed.</param>
        /// <exception cref="InvalidOperationException">Always thrown</exception>
        /// <remarks>Removing only a Row in a diagonal matrix are senseless, because of matrix nature</remarks>
        public override Matrix<float> RemoveColumn(int columnIndex)
        {
            throw new InvalidOperationException("Removing a Column in a diagonal matrix is not allowed. use RemoveRowAndColumn");
        }

        /// <summary>
        /// Creates a new Matrix with Row and Column of Index removed
        /// </summary>
        /// <param name="index">The index of the row and column to remove</param>
        /// <returns>The resultant Matrix with the row and column removed</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> greater then the matrix rows.</exception>
        public override Matrix<float> RemoveRowAndColumn(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }


            if (index >= RowCount)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            DiagonalMatrix outMatrix = new DiagonalMatrix(RowCount - 1);
            int targetIndex = 0; int sourceIndex = 0;
            for (var row = 0; row < RowCount; row++)
            {
                if (row != index)
                {
                    outMatrix._data[targetIndex] = _data[sourceIndex];
                    targetIndex++;
                }//else skip assignment. But always increment the sourceIndex 
                sourceIndex++;
            }

            return outMatrix;
        }


        /// <summary>
        /// Tests  all the elements of a matrix for a conditon and returns a Matrix with elemets 1.0f where this conditions is true.  
        /// Rest are 0.0 . The condition is given using System.Predicate;
        /// </summary>
        /// <param name="matchCondition">System.Predicate. The condition that the matrix elemets are tested for.</param>
        /// <returns> The resultant Matrix.</returns>
        /// <exception cref="ArgumentNullException">If matchCondition is <see langword="null" />.</exception>
        public override Matrix<float> FindMask(Predicate<float> matchCondition)
        {
            if (matchCondition == null) throw new ArgumentNullException("matchCondition");

            float[] outarr = new float[RowCount];
            int i = 0;
            for (var row = 0; row < RowCount; row++)
            {
                if (matchCondition(_data[i]))
                {
                    outarr[i] = 1.0f;
                }
                i++;

            }

            Matrix<float> outMatrix = new DiagonalMatrix(RowCount, ColumnCount, outarr);
            return outMatrix;
        }

        /// <summary>
        /// Tests  all the elements of a matrix for a conditon and returns and <see cref="IEnumerable{Tuple{int,int}}"/> of a Tuple{int,int} 
        /// of the indeces where this conditions is true.  
        /// The condition is given using System.Predicate.
        /// For example to find all elemets >1.0 :    
        ///    var myFoundIndeces=myMatrix.FindIndices(a => a > 1.0f);
        ///    foreach( var currentTuple in myFoundIndeces)
        ///    {
        ///        Console.WriteLine("Found Indeces: rows{0}, columns{1}",currentTuple.Item1, currentTuple.Item2);
        //     }
        /// </summary>
        /// <param name="matchCondition">System.Predicate. The condition that the matrix elemets are tested for.</param>
        /// <returns> The request <see cref="IEnumerable{Tuple{int,int}}"/> of the row and column indexes respectivly.</returns>
        /// <exception cref="ArgumentNullException">If matchCondition is <see langword="null" />.</exception>
        public override IEnumerable<Tuple<int, int>> FindIndices(Predicate<float> matchCondition)
        {
            if (matchCondition == null) throw new ArgumentNullException("matchCondition");

            for (var row = 0; row < RowCount; row++)
            {
                if (matchCondition(this._data[row]))
                {
                    yield return new Tuple<int, int>(row, row);
                }
            }
            yield break;
        }


        /// <summary>
        /// Tests  all the elements of a matrix for a conditon and returns and <see cref="IEnumerable{Tuple{int,int}}"/> of a Tuple{int,int} 
        /// of the indeces where this conditions is true.  1 based indexing.
        /// The condition is given using System.Predicate.
        /// For example to find all elemets >1.0 :    
        ///    var myFoundIndeces=myMatrix.FindIndicesI(a => a > 1.0f);
        ///    foreach( var currentTuple in myFoundIndeces)
        ///    {
        ///        Console.WriteLine("Found Indeces 1 based: Irows{0}, Icolumns{1}",currentTuple.Item1, currentTuple.Item2);
        //     }
        /// </summary>
        /// <param name="matchCondition">System.Predicate. The condition that the matrix elemets are tested for.</param>
        /// <returns> The request <see cref="IEnumerable{Tuple{int,int}}"/> of the row and column indexes respectivly.</returns>
        /// <exception cref="ArgumentNullException">If matchCondition is <see langword="null" />.</exception>
        public override IEnumerable<Tuple<int, int>> FindIndicesI(Predicate<float> matchCondition)
        {
            if (matchCondition == null) throw new ArgumentNullException("matchCondition");

            for (var row = 0; row < RowCount; row++)
            {
                if (matchCondition(this._data[row]))
                {
                    yield return new Tuple<int, int>(row + 1, row + 1);
                }
            }
            yield break;
        }



    }
}
