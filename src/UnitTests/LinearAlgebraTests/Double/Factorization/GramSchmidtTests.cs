// <copyright file="GramSchmidtTests.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
// http://mathnetnumerics.codeplex.com
// Copyright (c) 2009-2010 Math.NET
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

namespace MathNet.Numerics.UnitTests.LinearAlgebraTests.Double.Factorization
{
    using System;
    using LinearAlgebra.Double;
    using LinearAlgebra.Double.Factorization;
    using LinearAlgebra.Generic.Factorization;
    using NUnit.Framework;

    /// <summary>
    /// GramSchmidt factorization tests for a dense matrix.
    /// </summary>
    public class GramSchmidtTests
    {
        /// <summary>
        /// Constructor with <c>null</c> throws <c>ArgumentNullException</c>.
        /// </summary>
        [Test]
        public void ConstructorNullThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DenseGramSchmidt(null));
        }

        /// <summary>
        /// Constructor with wide matrix throws <c>ArgumentException</c>.
        /// </summary>
        [Test]
        public void ConstructorWideMatrixThrowsInvalidMatrixOperationException()
        {
            Assert.Throws<ArgumentException>(() => new DenseGramSchmidt(new DenseMatrix(3, 4)));
        }

        /// <summary>
        /// Can factorize identity matrix.
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public void CanFactorizeIdentity(int order)
        {
            var matrixI = DenseMatrix.Identity(order);
            var factorGramSchmidt = matrixI.GramSchmidt();
            var q = factorGramSchmidt.Q;
            var r = factorGramSchmidt.R;

            Assert.AreEqual(matrixI.RowCount, q.RowCount);
            Assert.AreEqual(matrixI.ColumnCount, q.ColumnCount);

            for (var i = 0; i < r.RowCount; i++)
            {
                for (var j = 0; j < r.ColumnCount; j++)
                {
                    if (i == j)
                    {
                        Assert.AreEqual(1.0, r[i, j]);
                    }
                    else
                    {
                        Assert.AreEqual(0.0, r[i, j]);
                    }
                }
            }

            for (var i = 0; i < q.RowCount; i++)
            {
                for (var j = 0; j < q.ColumnCount; j++)
                {
                    if (i == j)
                    {
                        Assert.AreEqual(1.0, q[i, j]);
                    }
                    else
                    {
                        Assert.AreEqual(0.0, q[i, j]);
                    }
                }
            }
        }

        /// <summary>
        /// Identity determinant is one.
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public void IdentityDeterminantIsOne(int order)
        {
            var matrixI = DenseMatrix.Identity(order);
            var factorGramSchmidt = matrixI.GramSchmidt();
            Assert.AreEqual(1.0, factorGramSchmidt.Determinant);
        }

        /// <summary>
        /// Can factorize a random matrix.
        /// </summary>
        /// <param name="row">Matrix row number.</param>
        /// <param name="column">Matrix column number.</param>
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(5, 5)]
        [TestCase(10, 6)]
        [TestCase(50, 48)]
        [TestCase(100, 98)]
        public void CanFactorizeRandomMatrix(int row, int column)
        {
            var matrixA = MatrixLoader.GenerateRandomDenseMatrix(row, column);
            var factorGramSchmidt = matrixA.GramSchmidt();
            var q = factorGramSchmidt.Q;
            var r = factorGramSchmidt.R;

            // Make sure the Q has the right dimensions.
            Assert.AreEqual(row, q.RowCount);
            Assert.AreEqual(column, q.ColumnCount);

            // Make sure the R has the right dimensions.
            Assert.AreEqual(column, r.RowCount);
            Assert.AreEqual(column, r.ColumnCount);

            // Make sure the R factor is upper triangular.
            for (var i = 0; i < r.RowCount; i++)
            {
                for (var j = 0; j < r.ColumnCount; j++)
                {
                    if (i > j)
                    {
                        Assert.AreEqual(0.0, factorGramSchmidt.R[i, j]);
                    }
                }
            }

            // Make sure the Q*R is the original matrix.
            var matrixQfromR = q * r;
            for (var i = 0; i < matrixQfromR.RowCount; i++)
            {
                for (var j = 0; j < matrixQfromR.ColumnCount; j++)
                {
                    Assert.AreEqual(matrixA[i, j], matrixQfromR[i, j], 1.0e-11);
                }
            }
        }

        /// <summary>
        /// Can solve a system of linear equations for a random vector (Ax=b).
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(50)]
        [TestCase(100)]
        public void CanSolveForRandomVector(int order)
        {
            var matrixA = MatrixLoader.GenerateRandomDenseMatrix(order, order);
            var matrixACopy = matrixA.Clone();
            var factorGramSchmidt = matrixA.GramSchmidt();

            var vectorb = MatrixLoader.GenerateRandomDenseVector(order);
            var resultx = factorGramSchmidt.Solve(vectorb);

            Assert.AreEqual(matrixA.ColumnCount, resultx.Count);

            var matrixBReconstruct = matrixA * resultx;

            // Check the reconstruction.
            for (var i = 0; i < order; i++)
            {
                Assert.AreEqual(vectorb[i], matrixBReconstruct[i], 1.0e-11);
            }

            // Make sure A didn't change.
            for (var i = 0; i < matrixA.RowCount; i++)
            {
                for (var j = 0; j < matrixA.ColumnCount; j++)
                {
                    Assert.AreEqual(matrixACopy[i, j], matrixA[i, j]);
                }
            }
        }

        /// <summary>
        /// Can solve a system of linear equations for a random matrix (AX=B).
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(50)]
        [TestCase(100)]
        public void CanSolveForRandomMatrix(int order)
        {
            var matrixA = MatrixLoader.GenerateRandomDenseMatrix(order, order);
            var matrixACopy = matrixA.Clone();
            var factorGramSchmidt = matrixA.GramSchmidt();

            var matrixB = MatrixLoader.GenerateRandomDenseMatrix(order, order);
            var matrixX = factorGramSchmidt.Solve(matrixB);

            // The solution X row dimension is equal to the column dimension of A
            Assert.AreEqual(matrixA.ColumnCount, matrixX.RowCount);

            // The solution X has the same number of columns as B
            Assert.AreEqual(matrixB.ColumnCount, matrixX.ColumnCount);

            var matrixBReconstruct = matrixA * matrixX;

            // Check the reconstruction.
            for (var i = 0; i < matrixB.RowCount; i++)
            {
                for (var j = 0; j < matrixB.ColumnCount; j++)
                {
                    Assert.AreEqual(matrixB[i, j], matrixBReconstruct[i, j], 1.0e-11);
                }
            }

            // Make sure A didn't change.
            for (var i = 0; i < matrixA.RowCount; i++)
            {
                for (var j = 0; j < matrixA.ColumnCount; j++)
                {
                    Assert.AreEqual(matrixACopy[i, j], matrixA[i, j]);
                }
            }
        }

        /// <summary>
        /// Can solve for a random vector into a result vector.
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(50)]
        [TestCase(100)]
        public void CanSolveForRandomVectorWhenResultVectorGiven(int order)
        {
            var matrixA = MatrixLoader.GenerateRandomDenseMatrix(order, order);
            var matrixACopy = matrixA.Clone();
            var factorGramSchmidt = matrixA.GramSchmidt();
            var vectorb = MatrixLoader.GenerateRandomDenseVector(order);
            var vectorbCopy = vectorb.Clone();
            var resultx = new DenseVector(order);
            factorGramSchmidt.Solve(vectorb, resultx);

            Assert.AreEqual(vectorb.Count, resultx.Count);

            var matrixBReconstruct = matrixA * resultx;

            // Check the reconstruction.
            for (var i = 0; i < vectorb.Count; i++)
            {
                Assert.AreEqual(vectorb[i], matrixBReconstruct[i], 1.0e-11);
            }

            // Make sure A didn't change.
            for (var i = 0; i < matrixA.RowCount; i++)
            {
                for (var j = 0; j < matrixA.ColumnCount; j++)
                {
                    Assert.AreEqual(matrixACopy[i, j], matrixA[i, j]);
                }
            }

            // Make sure b didn't change.
            for (var i = 0; i < vectorb.Count; i++)
            {
                Assert.AreEqual(vectorbCopy[i], vectorb[i]);
            }
        }

        /// <summary>
        /// Can solve a system of linear equations for a random matrix (AX=B) into a result matrix.
        /// </summary>
        /// <param name="order">Matrix order.</param>
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(50)]
        [TestCase(100)]
        public void CanSolveForRandomMatrixWhenResultMatrixGiven(int order)
        {
            var matrixA = MatrixLoader.GenerateRandomDenseMatrix(order, order);
            var matrixACopy = matrixA.Clone();
            var factorGramSchmidt = matrixA.GramSchmidt();

            var matrixB = MatrixLoader.GenerateRandomDenseMatrix(order, order);
            var matrixBCopy = matrixB.Clone();

            var matrixX = new DenseMatrix(order, order);
            factorGramSchmidt.Solve(matrixB, matrixX);

            // The solution X row dimension is equal to the column dimension of A
            Assert.AreEqual(matrixA.ColumnCount, matrixX.RowCount);

            // The solution X has the same number of columns as B
            Assert.AreEqual(matrixB.ColumnCount, matrixX.ColumnCount);

            var matrixBReconstruct = matrixA * matrixX;

            // Check the reconstruction.
            for (var i = 0; i < matrixB.RowCount; i++)
            {
                for (var j = 0; j < matrixB.ColumnCount; j++)
                {
                    Assert.AreEqual(matrixB[i, j], matrixBReconstruct[i, j], 1.0e-11);
                }
            }

            // Make sure A didn't change.
            for (var i = 0; i < matrixA.RowCount; i++)
            {
                for (var j = 0; j < matrixA.ColumnCount; j++)
                {
                    Assert.AreEqual(matrixACopy[i, j], matrixA[i, j]);
                }
            }

            // Make sure B didn't change.
            for (var i = 0; i < matrixB.RowCount; i++)
            {
                for (var j = 0; j < matrixB.ColumnCount; j++)
                {
                    Assert.AreEqual(matrixBCopy[i, j], matrixB[i, j]);
                }
            }
        }

        /// <summary>
        /// Can solve when using a tall matrix.
        /// </summary>
        [Test]
        public void CanSolveForMatrixWithTallRandomMatrix()
        {
            var matrixA = MatrixLoader.GenerateRandomDenseMatrix(20, 10);
            var matrixACopy = matrixA.Clone();
            var factorQR = matrixA.GramSchmidt();

            var matrixB = MatrixLoader.GenerateRandomDenseMatrix(20, 5);
            var matrixX = factorQR.Solve(matrixB);

            // The solution X row dimension is equal to the column dimension of A
            Assert.AreEqual(matrixA.ColumnCount, matrixX.RowCount);

            // The solution X has the same number of columns as B
            Assert.AreEqual(matrixB.ColumnCount, matrixX.ColumnCount);

            var test = (matrixA.Transpose() * matrixA).Inverse() * matrixA.Transpose() * matrixB;

            for (var i = 0; i < matrixX.RowCount; i++)
            {
                for (var j = 0; j < matrixX.ColumnCount; j++)
                {
                    AssertHelpers.AlmostEqual(test[i, j], matrixX[i, j], 9);
                }
            }

            // Make sure A didn't change.
            for (var i = 0; i < matrixA.RowCount; i++)
            {
                for (var j = 0; j < matrixA.ColumnCount; j++)
                {
                    Assert.AreEqual(matrixACopy[i, j], matrixA[i, j]);
                }
            }
        }

        /// <summary>
        /// Can solve when using a tall matrix.
        /// </summary>
        [Test]
        public void CanSolveForVectorWithTallRandomMatrix()
        {
            var matrixA = MatrixLoader.GenerateRandomDenseMatrix(20, 10);
            var matrixACopy = matrixA.Clone();
            var factorQR = matrixA.GramSchmidt();

            var vectorB = MatrixLoader.GenerateRandomDenseVector(20);
            var vectorX = factorQR.Solve(vectorB);

            // The solution x dimension is equal to the column dimension of A
            Assert.AreEqual(matrixA.ColumnCount, vectorX.Count);

            var test = (matrixA.Transpose()*matrixA).Inverse()*matrixA.Transpose()*vectorB;

            for (var i = 0; i < vectorX.Count; i++)
            {
                AssertHelpers.AlmostEqual(test[i], vectorX[i], 9);
            }

            // Make sure A didn't change.
            for (var i = 0; i < matrixA.RowCount; i++)
            {
                for (var j = 0; j < matrixA.ColumnCount; j++)
                {
                    Assert.AreEqual(matrixACopy[i, j], matrixA[i, j]);
                }
            }
        }
    }
}
