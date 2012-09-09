// <copyright file="MatrixTests.cs" company="Math.NET">
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

namespace MathNet.Numerics.UnitTests.LinearAlgebraTests.Complex32
{
    using NUnit.Framework;
    using System;
    using Complex32 = Numerics.Complex32;

    /// <summary>
    /// Abstract class with the common set of matrix tests
    /// </summary>
    public abstract partial class MatrixTests : MatrixLoader
    {
        /// <summary>
        /// Can transpose a matrix.
        /// </summary>
        /// <param name="name">Matrix name.</param>
        [TestCase("Singular3x3")]
        [TestCase("Square3x3")]
        [TestCase("Square4x4")]
        [TestCase("Tall3x2")]
        [TestCase("Wide2x3")]
        public void CanTransposeMatrix(string name)
        {
            var matrix = CreateMatrix(TestData2D[name]);
            var transpose = matrix.Transpose();

            Assert.AreNotSame(matrix, transpose);
            Assert.AreEqual(matrix.RowCount, transpose.ColumnCount);
            Assert.AreEqual(matrix.ColumnCount, transpose.RowCount);
            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    Assert.AreEqual(matrix[i, j], transpose[j, i]);
                }
            }
        }

        /// <summary>
        /// Can conjugate transpose a matrix.
        /// </summary>
        /// <param name="name">Matrix name.</param>
        [TestCase("Singular3x3")]
        [TestCase("Square3x3")]
        [TestCase("Square4x4")]
        [TestCase("Tall3x2")]
        [TestCase("Wide2x3")]
        public void CanConjugateTransposeMatrix(string name)
        {
            var matrix = CreateMatrix(TestData2D[name]);
            var transpose = matrix.ConjugateTranspose();

            Assert.AreNotSame(matrix, transpose);
            Assert.AreEqual(matrix.RowCount, transpose.ColumnCount);
            Assert.AreEqual(matrix.ColumnCount, transpose.RowCount);
            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    Assert.AreEqual(matrix[i, j], transpose[j, i].Conjugate());
                }
            }
        }

        /// <summary>
        /// Can compute Frobenius norm.
        /// </summary>
        [Test]
        public virtual void CanComputeFrobeniusNorm()
        {
            var matrix = TestMatrices["Square3x3"];
            AssertHelpers.AlmostEqual(10.8819655f, matrix.FrobeniusNorm().Real, 7);

            matrix = TestMatrices["Wide2x3"];
            AssertHelpers.AlmostEqual(5.1905256f, matrix.FrobeniusNorm().Real, 7);

            matrix = TestMatrices["Tall3x2"];
            AssertHelpers.AlmostEqual(7.5904115f, matrix.FrobeniusNorm().Real, 7);
        }

        /// <summary>
        /// Can compute Infinity norm.
        /// </summary>
        [Test]
        public virtual void CanComputeInfinityNorm()
        {
            var matrix = TestMatrices["Square3x3"];
            AssertHelpers.AlmostEqual(16.7777033f, matrix.InfinityNorm().Real, 6);

            matrix = TestMatrices["Wide2x3"];
            AssertHelpers.AlmostEqual(7.3514039f, matrix.InfinityNorm().Real, 6);

            matrix = TestMatrices["Tall3x2"];
            AssertHelpers.AlmostEqual(10.1023756f, matrix.InfinityNorm().Real, 6);
        }

        /// <summary>
        /// Can compute L1 norm.
        /// </summary>
        [Test]
        public virtual void CanComputeL1Norm()
        {
            var matrix = TestMatrices["Square3x3"];
            AssertHelpers.AlmostEqual(12.5401248f, matrix.L1Norm().Real, 7);

            matrix = TestMatrices["Wide2x3"];
            AssertHelpers.AlmostEqual(5.8647971f, matrix.L1Norm().Real, 7);

            matrix = TestMatrices["Tall3x2"];
            AssertHelpers.AlmostEqual(9.4933860f, matrix.L1Norm().Real, 7);
        }

        /// <summary>
        /// Can compute L2 norm.
        /// </summary>
        [Test]
        public virtual void CanComputeL2Norm()
        {
            var matrix = TestMatrices["Square3x3"];
            AssertHelpers.AlmostEqual(10.6381752f, matrix.L2Norm().Real, 6);

            matrix = TestMatrices["Wide2x3"];
            AssertHelpers.AlmostEqual(5.2058554f, matrix.L2Norm().Real, 6);
            matrix = TestMatrices["Tall3x2"];
            AssertHelpers.AlmostEqual(7.3582664f, matrix.L2Norm().Real, 6);
        }

        /// <summary>
        /// Can check if a matrix is symmetric.
        /// </summary>
        [Test]
        public virtual void CanCheckIfMatrixIsSymmetric()
        {
            var matrix = TestMatrices["Symmetric3x3"];
            Assert.IsTrue(matrix.IsSymmetric);

            matrix = TestMatrices["Square3x3"];
            Assert.IsFalse(matrix.IsSymmetric);
        }


        /// <summary>
        /// Tests where we can shuffle the columns of a matrix.
        /// </summary>
        /// <param name="name"></param>
        //Test will not work  on certain singular matrices because some Columns are possible the same.
        [TestCase("Square3x3")]
        [TestCase("Wide2x3")]
        public virtual void CanShuffleMatrixOnColumns(string name)
        {
            var testMatrix = CreateMatrix(TestData2D[name]);// TestMatrices[name];
            var orignalMatrix = testMatrix.Clone();
            var shuffledMatrix = testMatrix.Clone();
            Permutation aColPerm = null;
            bool wasChanged = false;
            int count = 0;

            //try to Shuffle the matrix up to 100 times by the columms, at least once should be a random Permutation and not just 0,1,2
            while (wasChanged == false && count <= 100)
            {
                shuffledMatrix = testMatrix.Clone();
                wasChanged = false;

                // Shuffle testMatrix and create a random Permutation of the Columns.
                aColPerm = shuffledMatrix.ShuffleColumns();
                count++;
                //test if the Permutation is the not simple permutation 0,1,2 
                for (int i = 0; i < aColPerm.Dimension; i++)
                {
                    if (aColPerm[i] != i)
                        wasChanged = true;
                }
            }

            //if fails means 100 attempts failed to shuffle columns in an order other than 0,1,2..something is probably wrong!
            Assert.AreEqual(wasChanged, true);

            //should be diffrent now that testmatrix is shuffled...will not be true on all matrices.
            Assert.AreNotEqual(orignalMatrix, shuffledMatrix);

            //check the Permuation given by the random Shuffle can be used on another matrix of same number of columns 
            orignalMatrix.PermuteColumns(aColPerm);
            // and orders it the same way as testmatrix
            Assert.AreEqual(orignalMatrix, shuffledMatrix);
        }


        //Test will not work on certain singular because two or more Rows are possible the same.
        /// <summary>
        /// Tests where we can shuffle the rows of a matrix.
        /// </summary>
        /// <param name="name"></param>
        [TestCase("Square3x3")]
        [TestCase("Wide2x3")]
        public virtual void CanShuffleMatrixOnRows(string name)
        {
            var testMatrix = CreateMatrix(TestData2D[name]);// TestMatrices[name];
            var orignalMatrix = testMatrix.Clone();
            var shuffledMatrix = testMatrix.Clone();
            Permutation aRowPerm = null;
            bool wasChanged = false;
            int count = 0;

            //try to Shuffle the matrix up to 100 times by the columms, at least once should be a random Permutation and not just 0,1,2
            while (wasChanged == false && count <= 100)
            {
                shuffledMatrix = testMatrix.Clone();
                wasChanged = false;

                // Shuffle testMatrix and create a random Permutation of the Rows.
                aRowPerm = shuffledMatrix.ShuffleRows();
                count++;
                //test if the Permutation is the not simple permutation 0,1,2 
                for (int i = 0; i < aRowPerm.Dimension; i++)
                {
                    if (aRowPerm[i] != i)
                        wasChanged = true;
                }
            }

            //if fails means 100 attempts failed to shuffle columns in an order other than 0,1,2..something is probably wrong!
            Assert.AreEqual(wasChanged, true);

            //should be diffrent now that testmatrix is shuffled. This will not work on all Matrices.
            Assert.AreNotEqual(orignalMatrix, shuffledMatrix);

            //check the Permuation given by the random Shuffle can be used on another matrix of same number of columns 
            orignalMatrix.PermuteRows(aRowPerm);
            // and orders it the same way as testmatrix
            Assert.AreEqual(orignalMatrix, shuffledMatrix);
        }



        /// <summary>
        /// Test whether we can sort a matrix by a specific column. So sort each row of the matrix by a specific column in ascending order.
        /// </summary>
        [TestCase("Singular3x3")]
        [TestCase("Square3x3")]
        [TestCase("Wide2x3")]
        public virtual void CanSortMatrixOnColumns(string name)
        {
            var testMatrix = CreateMatrix(TestData2D[name]);
            var w = new LinearAlgebra.Complex32.DenseVector(testMatrix.RowCount, 1);
            var v = new LinearAlgebra.Complex32.DenseVector(testMatrix.ColumnCount, 1);

            for (int i = 0; i < testMatrix.ColumnCount; i++)
            {
                var copyOfMatrix = testMatrix.Clone();
                var SortMatrix = testMatrix.Clone();
                Permutation sortPermutation = SortMatrix.SortByColumn(i);
                copyOfMatrix.PermuteRows(sortPermutation);

                //check the Permuation could be used to sort another matrix the same way.
                Assert.AreEqual(SortMatrix, copyOfMatrix);

                // check total Sum of Elements equals same as in Orignal TestMatrix.
                AssertHelpers.AlmostEqual(w * SortMatrix * v, w * testMatrix * v, 6);

                //check next element in the same Column is greater or equal to preceding element.
                for (int j = 0; j < testMatrix.RowCount - 1; j++)
                {
                    Assert.GreaterOrEqual(SortMatrix[j + 1, i].Magnitude, SortMatrix[j, i].Magnitude);
                }
            }


        }


        /// <summary>
        /// Test whether we can sort a matrix by a specific row. So sort each column  of the matrix by a specific row in ascending order.
        /// </summary>
        [TestCase("Singular3x3")]
        [TestCase("Square3x3")]
        [TestCase("Wide2x3")]
        public virtual void CanSortMatrixOnRows(string name)
        {
            var testMatrix = CreateMatrix(TestData2D[name]);
            var w = new LinearAlgebra.Complex32.DenseVector(testMatrix.RowCount, 1);
            var v = new LinearAlgebra.Complex32.DenseVector(testMatrix.ColumnCount, 1);

            for (int i = 0; i < testMatrix.RowCount; i++)
            {
                var copyOfMatrix = testMatrix.Clone();
                var SortMatrix = testMatrix.Clone();
                Permutation sortPermutation = SortMatrix.SortByRow(i);
                copyOfMatrix.PermuteColumns(sortPermutation);

                //check the Permuation could be used to sort another matrix the same way.
                Assert.AreEqual(SortMatrix, copyOfMatrix);

                // check total Sum of Elements equals same as in Orignal TestMatrix.
                AssertHelpers.AlmostEqual(w * SortMatrix * v, w * testMatrix * v, 6);

                //check next element in the same Row is greater or equal to preceding element.
                for (int j = 0; j < testMatrix.ColumnCount - 1; j++)
                {
                    Assert.GreaterOrEqual(SortMatrix[i, j + 1].Magnitude, SortMatrix[i, j].Magnitude);
                }
            }
        }


        /// <summary>
        /// Tests whether we can find the mask matrix, of a specific premmise. The mask matrix if a matrix with ones where the premise is true.
        /// </summary>
        [Test]
        public virtual void CanFindMask()
        {
            var testMatrix = CreateMatrix(TestData2D["Tallx3"]);
            var negativesMatrix = CreateMatrix(TestData2D["Tallx3Negatives"]);

            Predicate<Complex32> match = elem => (elem.Real < 0.0f);

            var maskMatrix = testMatrix.FindMask(match);
            Assert.AreEqual(maskMatrix, negativesMatrix);

            ///// Next Test: test nothing is found that should not be found

            // set newValue to 1 greater than the max.
            float newValue = 0f;
            for (int i = 0; i < testMatrix.RowCount; i++)
            {
                for (int j = 0; j < testMatrix.ColumnCount; j++)
                {
                    if (testMatrix[i, j].Real >= newValue)
                        newValue = testMatrix[i, j].Real;
                }
            }
            newValue = newValue + 1.0f;

            //compare to a matrix of all zeros.
            Assert.AreEqual(testMatrix.FindMask(elem => elem.Real >= newValue), new LinearAlgebra.Complex32.DenseMatrix(testMatrix.RowCount, testMatrix.ColumnCount));

        }


        /// <summary>
        /// Test whether we can set the elements of a matrix to a value where the mask matrix, which needs to be of the same size, has a 1.
        /// </summary>
        [Test]
        public virtual void CanOnMaskSet()
        {
            var testMatrix = CreateMatrix(TestData2D["Tallx3"]);
            var negativesMatrix = CreateMatrix(TestData2D["Tallx3Negatives"]);

            Predicate<Complex32> match = elem => (elem.Real < 0.0f);

            var maskMatrixLtZero = testMatrix.FindMask(match);

            //set all found elements that were < 0 * real part , to a value 1.0 greater than the previous biggest value, (method max matrix does not exist yet.)
            float newValue = 0.0f;

            for (int i = 0; i < testMatrix.RowCount; i++)
            {
                for (int j = 0; j < testMatrix.ColumnCount; j++)
                {
                    if ((testMatrix[i, j]).Real >= newValue)
                        newValue = testMatrix[i, j].Real;
                }
            }

            newValue = newValue + 1.0f;
            testMatrix.OnMaskSet(maskMatrixLtZero, newValue);

            //find the mask of all elements equal to the biggest value

            Predicate<Complex32> matchNewValue = elem => (elem == newValue);
            var maskMatrixNewValue = testMatrix.FindMask(matchNewValue);

            Assert.AreEqual(maskMatrixNewValue, maskMatrixLtZero);

        }


        /// <summary>
        /// Tests whether we can apply a formula to the elements of a matrix , where the mask matrix has a 1.
        /// The mask matrix needs to be the same size.
        /// </summary>
        [Test]
        public virtual void CanOnMaskApply()
        {
            var testMatrix = CreateMatrix(TestData2D["Tallx3"]);
            var negativesMatrix = CreateMatrix(TestData2D["Tallx3Negatives"]);

            //find all elements where ther real part is negative.
            Predicate<Complex32> match = elem => (elem.Real < 0.0f);

            var maskMatrixLtZero = testMatrix.FindMask(match);

            // multiply all negative elements with -1 so that the real value is positive.
            testMatrix.OnMaskApply(maskMatrixLtZero, elem => -elem);

            // check no elements are negative anymore 
            Assert.AreEqual(testMatrix.FindMask(match), CreateMatrix(testMatrix.RowCount, testMatrix.ColumnCount));

        }


        /// <summary>
        /// Test whether we can enumerate over the elements of a matrices, where the Mask matrix has elements of  1 .
        /// </summary>
        [Test]
        public virtual void CanEnumerateMask()
        {
            var testMatrix = CreateMatrix(TestData2D["Tallx3"]);
            var negativesMatrix = CreateMatrix(TestData2D["Tallx3Negatives"]);

            Predicate<Complex32> match = elem => (elem.Real < 0.0f);

            //use these later to sum all entries of testMatrix pointwise multiplied with negativesMatrix
            var w = new LinearAlgebra.Complex32.DenseVector(testMatrix.RowCount, 1);
            var v = new LinearAlgebra.Complex32.DenseVector(testMatrix.ColumnCount, 1);

            var maskMatrixLtZero = testMatrix.FindMask(match);
            var negativeElements = testMatrix.EnumerateMask(maskMatrixLtZero);
            Complex32 sumNegativeElements = Complex32.Zero;

            foreach (var elem in negativeElements)
            {
                sumNegativeElements = sumNegativeElements + elem;
            }

            var matrixPProduct = negativesMatrix.PointwiseMultiply(testMatrix);
            var sumMatrixProduct = w * matrixPProduct * v;
            Assert.AreEqual(sumNegativeElements, sumMatrixProduct);
        }


        /// <summary>
        /// Test whether we can find the indeces of a matrix elements where a specific premise is true.
        /// </summary>
        [Test]
        public virtual void CanFindIndices()
        {
            var testMatrix = CreateMatrix(TestData2D["Tallx3"]);
            var negativesMatrix = CreateMatrix(TestData2D["Tallx3Negatives"]);

            Predicate<Complex32> match = elem => (elem.Real < 0.0f);

            var myFoundIndices = testMatrix.FindIndices(match);

            int count = 0;
            foreach (var ftup in myFoundIndices)
            {
                Assert.AreEqual(negativesMatrix[ftup.Item1, ftup.Item2], Complex32.One);
                count++;
            }

            //make sure only 3 ( or 2 for the diagnoal test matrix) are found.
            Assert.GreaterOrEqual(3, count);

        }

        /// <summary>
        /// Test whether we can remove a column from a matrix. Does not make sense to run for a diagonal matrix.
        /// </summary>
        [TestCase("Singular3x3")]
        [TestCase("Square3x3")]
        [TestCase("Wide2x3")]
        public virtual void CanRemoveColumn(string name)
        {
            var testMatrix = CreateMatrix(TestData2D[name]);
            foreach (int i in testMatrix.ColumnIndices())
            {
                var alteredMatrix = testMatrix.InsertColumn(i, testMatrix.Column(i));
                alteredMatrix = alteredMatrix.RemoveColumn(i);
                Assert.AreEqual(testMatrix, alteredMatrix);
            }
        }

        /// <summary>
        /// Test whether we can remove a row  from a matrix. Does not make sense to run for a diagonal matrix.
        /// </summary>
        [TestCase("Singular3x3")]
        [TestCase("Square3x3")]
        [TestCase("Wide2x3")]
        public virtual void CanRemoveRow(string name)
        {
            var testMatrix = CreateMatrix(TestData2D[name]);

            foreach (int i in testMatrix.RowIndices())
            {
                var alteredMatrix = testMatrix.InsertRow(i, testMatrix.Row(i));
                alteredMatrix = alteredMatrix.RemoveRow(i);
                Assert.AreEqual(testMatrix, alteredMatrix);
            }
        }


        /// <summary>
        /// Test whether we can remove a row and column at the same time from a matrix. 
        /// </summary>
        [TestCase("Singular3x3")]
        [TestCase("Square3x3")]
        public virtual void CanRemoveRowAndColumn(string name)
        {
            var testMatrix = CreateMatrix(TestData2D[name]);

            for (int i = 0; i < testMatrix.RowCount && i < testMatrix.ColumnCount; i++)
            {
                var alteredMatrix = testMatrix.InsertRow(i, testMatrix.Row(i));
                alteredMatrix = alteredMatrix.InsertColumn(i, alteredMatrix.Column(i));
                alteredMatrix = alteredMatrix.RemoveRowAndColumn(i);
                Assert.AreEqual(testMatrix, alteredMatrix);
            }

        }

        /// <summary>
        /// Test wether we can forma a new matrix which is a selection of Columns of a matrix. 
        /// </summary>
        /// <param name="rowIndex">the column Index to start from </param>
        /// <param name="numberOfColumns">The number of columns</param>
        /// <param name="name">The matrix name to test on.</param>
        [TestCase(0, 2, "Singular3x3")]
        [TestCase(1, 1, "Square3x3")]
        [TestCase(2, 1, "Square3x3")]
        [TestCase(0, 2, "Square3x3")]
        [TestCase(0, 1, "Square3x3")]
        [TestCase(1, 2, "Square3x3")]
        public void CanSelectColumns(int columnIndex, int numberOfColumns, string name)
        {
            var matrix = TestMatrices[name];
            var columnsM = matrix.SelectColumns(columnIndex, numberOfColumns);

            var columnFirst = matrix.Column(columnIndex);
            var matrixCombined = columnFirst.ToColumnMatrix();

            for (int i = 1; i < numberOfColumns; i++)
            {
                var columni = matrix.Column(columnIndex + i);
                matrixCombined = matrixCombined.Append(columni.ToColumnMatrix());
            }

            Assert.AreEqual(columnsM, matrixCombined);



        }


        /// <summary>
        /// Test wether we can forma a new matrix which is a selection of Rows of a matrix 
        /// </summary>
        /// <param name="rowIndex">the row Index to start from </param>
        /// <param name="numberOfRows">The number of Rows</param>
        /// <param name="name"></param>
        [TestCase(0, 2, "Singular3x3")]
        [TestCase(1, 1, "Square3x3")]
        [TestCase(2, 1, "Square3x3")]
        [TestCase(0, 2, "Square3x3")]
        [TestCase(0, 1, "Square3x3")]
        [TestCase(1, 2, "Square3x3")]
        public void CanSelectRows(int rowIndex, int numberOfRows, string name)
        {
            var matrix = TestMatrices[name];
            var rowsM = matrix.SelectRows(rowIndex, numberOfRows);

            var rowFirst = matrix.Row(rowIndex);
            var matrixCombined = rowFirst.ToRowMatrix();

            for (int i = 1; i < numberOfRows; i++)
            {
                var rowi = matrix.Row(rowIndex + i);
                matrixCombined = matrixCombined.Stack(rowi.ToRowMatrix());
            }

            Assert.AreEqual(rowsM, matrixCombined);

        }



        /// <summary>
        /// Test wether we can set a selection of Rows of a matrix to another Matrix with the same numer of Columns.
        /// </summary>
        /// <param name="rowIndex">the row Index to start from </param>
        /// <param name="numberOfRows">The number of Rows</param>
        /// <param name="name"></param>
        [TestCase(0, 2, "Singular3x3")]
        [TestCase(1, 1, "Square3x3")]
        [TestCase(2, 1, "Square3x3")]
        [TestCase(0, 2, "Square3x3")]
        [TestCase(0, 1, "Square3x3")]
        [TestCase(1, 2, "Square3x3")]
        public void CanSetRows(int rowIndex, int numberOfRows, string name)
        {
            var matrix = TestMatrices[name];
            var rowsM = matrix.SelectRows(rowIndex, numberOfRows);

            var matrixCopy = matrix.Clone();
            //set certain rows to 0.
            var zerosMatrix = new LinearAlgebra.Complex32.DenseMatrix(numberOfRows, matrixCopy.ColumnCount);
            matrixCopy.SetRows(rowIndex, numberOfRows, zerosMatrix);
            Assert.AreNotEqual(matrix, matrixCopy);
            Assert.AreEqual(zerosMatrix, matrixCopy.SelectRows(rowIndex, numberOfRows));

            //set rows back to original
            matrixCopy.SetRows(rowIndex, numberOfRows, rowsM);
            Assert.AreEqual(matrix, matrixCopy);

        }


        /// <summary>
        /// Test wether we can set a selection of Columns of a matrix to another Matrix with the same numer of Columns.
        /// </summary>
        /// <param name="ColumnIndex">the Column Index to start from </param>
        /// <param name="numberOfColumns">The number of Columns</param>
        /// <param name="name"></param>
        [TestCase(0, 3, "Singular3x3")]
        [TestCase(1, 1, "Square3x3")]
        [TestCase(2, 1, "Square3x3")]
        [TestCase(0, 2, "Square3x3")]
        [TestCase(0, 1, "Square3x3")]
        [TestCase(1, 2, "Square3x3")]
        public void CanSetColumns(int ColumnIndex, int numberOfColumns, string name)
        {
            var matrix = TestMatrices[name];
            var ColumnsM = matrix.SelectColumns(ColumnIndex, numberOfColumns);

            var matrixCopy = matrix.Clone();
            //set certain Columns to 0.
            var zerosMatrix = new LinearAlgebra.Complex32.DenseMatrix(matrixCopy.RowCount, numberOfColumns);
            matrixCopy.SetColumns(ColumnIndex, numberOfColumns, zerosMatrix);
            Assert.AreNotEqual(matrix, matrixCopy);
            Assert.AreEqual(zerosMatrix, matrixCopy.SelectColumns(ColumnIndex, numberOfColumns));

            //set Columns back to original
            matrixCopy.SetColumns(ColumnIndex, numberOfColumns, ColumnsM);
            Assert.AreEqual(matrix, matrixCopy);

        }






    }

}
