// <copyright file="VectorTests.cs" company="Math.NET">
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

namespace MathNet.Numerics.UnitTests.LinearAlgebraTests.Double
{
    using Distributions;
    using LinearAlgebra.Double;
    using LinearAlgebra.Generic;
    using NUnit.Framework;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Abstract class with the common set of vector tests.
    /// </summary>
    public abstract partial class VectorTests
    {
        /// <summary>
        /// Test vector values.
        /// </summary>
        protected readonly double[] Data = { 1, 2, 3, 4, 5 };

        /// <summary>
        /// Can clone a vector.
        /// </summary>
        [Test]
        public void CanCloneVector()
        {
            var vector = CreateVector(Data);
            var clone = vector.Clone();

            Assert.AreNotSame(vector, clone);
            Assert.AreEqual(vector.Count, clone.Count);
            CollectionAssert.AreEqual(vector, clone);
        }

#if !PORTABLE
        /// <summary>
        /// Can clone a vector using <c>IClonable</c> interface method.
        /// </summary>
        [Test]
        public void CanCloneVectorUsingICloneable()
        {
            var vector = CreateVector(Data);
            var clone = (Vector<double>)((ICloneable)vector).Clone();

            Assert.AreNotSame(vector, clone);
            Assert.AreEqual(vector.Count, clone.Count);
            CollectionAssert.AreEqual(vector, clone);
        }
#endif

        /// <summary>
        /// Can convert vector to string.
        /// </summary>
        [Test]
        public void CanConvertVectorToString()
        {
            var vector = CreateVector(Data);
            var str = vector.ToVectorString(1, int.MaxValue, 1);
            Assert.AreEqual("1 2 3 4 5", str);
        }

        /// <summary>
        /// Can copy part of a vector to another vector.
        /// </summary>
        [Test]
        public void CanCopyPartialVectorToAnother()
        {
            var vector = CreateVector(Data);
            var other = CreateVector(Data.Length);

            vector.CopySubVectorTo(other, 2, 2, 2);

            Assert.AreEqual(0.0, other[0]);
            Assert.AreEqual(0.0, other[1]);
            Assert.AreEqual(3.0, other[2]);
            Assert.AreEqual(4.0, other[3]);
            Assert.AreEqual(0.0, other[4]);
        }

        /// <summary>
        /// Can copy part of a vector to the same vector.
        /// </summary>
        [Test]
        public void CanCopyPartialVectorToSelf()
        {
            var vector = CreateVector(Data);
            vector.CopySubVectorTo(vector, 0, 2, 2);

            Assert.AreEqual(1.0, vector[0]);
            Assert.AreEqual(2.0, vector[1]);
            Assert.AreEqual(1.0, vector[2]);
            Assert.AreEqual(2.0, vector[3]);
            Assert.AreEqual(5.0, vector[4]);
        }

        /// <summary>
        /// Can copy a vector to another vector.
        /// </summary>
        [Test]
        public void CanCopyVectorToAnother()
        {
            var vector = CreateVector(Data);
            var other = CreateVector(Data.Length);

            vector.CopyTo(other);
            CollectionAssert.AreEqual(vector, other);
        }

        /// <summary>
        /// Can create a matrix using instance of a vector.
        /// </summary>
        [Test]
        public void CanCreateMatrix()
        {
            var vector = CreateVector(Data);
            var matrix = vector.CreateMatrix(10, 10);
            Assert.AreEqual(matrix.RowCount, 10);
            Assert.AreEqual(matrix.ColumnCount, 10);
        }

        /// <summary>
        /// Can create a vector using the instance of vector.
        /// </summary>
        [Test]
        public void CanCreateVector()
        {
            var expected = CreateVector(5);
            var actual = expected.CreateVector(5);
            Assert.AreEqual(expected.GetType(), actual.GetType(), "vectors are same type.");
        }

        /// <summary>
        /// Can enumerate over a vector.
        /// </summary>
        [Test]
        public void CanEnumerateOverVector()
        {
            var vector = CreateVector(Data);
            for (var i = 0; i < Data.Length; i++)
            {
                Assert.AreEqual(Data[i], vector[i]);
            }
        }

        /// <summary>
        /// Can enumerate over a vector using <c>IEnumerable</c> interface.
        /// </summary>
        [Test]
        public void CanEnumerateOverVectorUsingIEnumerable()
        {
            var enumerable = (IEnumerable)CreateVector(Data);
            var index = 0;
            foreach (var element in enumerable)
            {
                Assert.AreEqual(Data[index++], (double)element);
            }
        }

        /// <summary>
        /// Can equate vectors.
        /// </summary>
        [Test]
        public void CanEquateVectors()
        {
            var vector1 = CreateVector(Data);
            var vector2 = CreateVector(Data);
            var vector3 = CreateVector(4);
            Assert.IsTrue(vector1.Equals(vector1));
            Assert.IsTrue(vector1.Equals(vector2));
            Assert.IsFalse(vector1.Equals(vector3));
            Assert.IsFalse(vector1.Equals(null));
            Assert.IsFalse(vector1.Equals(2));
        }

        /// <summary>
        /// <c>CreateVector</c> throws <c>ArgumentOutOfRangeException</c> if size is not positive.
        /// </summary>
        [Test]
        public void SizeIsNotPositiveThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateVector(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateVector(0));
        }

        /// <summary>
        /// Can equate vectors using Object.Equals.
        /// </summary>
        [Test]
        public void CanEquateVectorsUsingObjectEquals()
        {
            var vector1 = CreateVector(Data);
            var vector2 = CreateVector(Data);
            Assert.IsTrue(vector1.Equals((object)vector2));
        }

        /// <summary>
        /// Can get hash code of a vector.
        /// </summary>
        [Test]
        public void CanGetHashCode()
        {
            var vector = CreateVector(new double[] { 1, 2, 3, 4 });
            Assert.AreEqual(vector.GetHashCode(), vector.GetHashCode());
            Assert.AreEqual(vector.GetHashCode(), CreateVector(new double[] { 1, 2, 3, 4 }).GetHashCode());
            Assert.AreNotEqual(vector.GetHashCode(), CreateVector(new double[] { 1 }).GetHashCode());
        }

        /// <summary>
        /// Can enumerate over a vector using indexed enumerator.
        /// </summary>
        [Test]
        public void CanEnumerateOverVectorUsingIndexedEnumerator()
        {
            var vector = CreateVector(Data);
            foreach (var pair in vector.GetIndexedEnumerator())
            {
                Assert.AreEqual(Data[pair.Item1], pair.Item2);
            }
        }

        /// <summary>
        /// Can convert a vector to array.
        /// </summary>
        [Test]
        public void CanConvertVectorToArray()
        {
            var vector = CreateVector(Data);
            var array = vector.ToArray();
            CollectionAssert.AreEqual(array, vector);
        }

        /// <summary>
        /// Can convert a vector to column matrix.
        /// </summary>
        [Test]
        public void CanConvertVectorToColumnMatrix()
        {
            var vector = CreateVector(Data);
            var matrix = vector.ToColumnMatrix();

            Assert.AreEqual(vector.Count, matrix.RowCount);
            Assert.AreEqual(1, matrix.ColumnCount);

            for (var i = 0; i < vector.Count; i++)
            {
                Assert.AreEqual(vector[i], matrix[i, 0]);
            }
        }

        /// <summary>
        /// Can convert a vector to row matrix.
        /// </summary>
        [Test]
        public void CanConvertVectorToRowMatrix()
        {
            var vector = CreateVector(Data);
            var matrix = vector.ToRowMatrix();

            Assert.AreEqual(vector.Count, matrix.ColumnCount);
            Assert.AreEqual(1, matrix.RowCount);

            for (var i = 0; i < vector.Count; i++)
            {
                Assert.AreEqual(vector[i], matrix[0, i]);
            }
        }

        /// <summary>
        /// Can set values in vector.
        /// </summary>
        [Test]
        public void CanSetValues()
        {
            var vector = CreateVector(Data);
            vector.SetValues(Data);
            CollectionAssert.AreEqual(vector, Data);
        }

        /// <summary>
        /// Can get subvector from a vector.
        /// </summary>
        /// <param name="index">The first element to begin copying from.</param>
        /// <param name="length">The number of elements to copy.</param>
        [TestCase(0, 5)]
        [TestCase(2, 2)]
        [TestCase(1, 4)]
        public void CanGetSubVector(int index, int length)
        {
            var vector = CreateVector(Data);
            var sub = vector.SubVector(index, length);
            Assert.AreEqual(length, sub.Count);
            for (var i = 0; i < length; i++)
            {
                Assert.AreEqual(vector[i + index], sub[i]);
            }
        }

        /// <summary>
        /// Getting subvector using wrong parameters throw an exception.
        /// </summary>
        /// <param name="index">The first element to begin copying from.</param>
        /// <param name="length">The number of elements to copy.</param>
        [TestCase(6, 10)]
        [TestCase(1, -10)]
        public void CanGetSubVectorWithWrongValuesShouldThrowException(int index, int length)
        {
            var vector = CreateVector(Data);
            Assert.Throws<ArgumentOutOfRangeException>(() => vector.SubVector(index, length));
        }

        /// <summary>
        /// Can find absolute minimum value index.
        /// </summary>
        [Test]
        public void CanFindAbsoluteMinimumIndex()
        {
            var source = CreateVector(Data);
            const int Expected = 0;
            var actual = source.AbsoluteMinimumIndex();
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Can find absolute minimum value of a vector.
        /// </summary>
        [Test]
        public void CanFindAbsoluteMinimum()
        {
            var source = CreateVector(Data);
            const double Expected = 1;
            var actual = source.AbsoluteMinimum();
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Can find absolute maximum value index.
        /// </summary>
        [Test]
        public void CanFindAbsoluteMaximumIndex()
        {
            var source = CreateVector(Data);
            const int Expected = 4;
            var actual = source.AbsoluteMaximumIndex();
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Can find absolute maximum value of a vector.
        /// </summary>
        [Test]
        public void CanFindAbsoluteMaximum()
        {
            var source = CreateVector(Data);
            const double Expected = 5;
            var actual = source.AbsoluteMaximum();
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Can find maximum value index.
        /// </summary>
        [Test]
        public void CanFindMaximumIndex()
        {
            var vector = CreateVector(Data);

            const int Expected = 4;
            var actual = vector.MaximumIndex();

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Can find maximum value of a vector.
        /// </summary>
        [Test]
        public void CanFindMaximum()
        {
            var vector = CreateVector(Data);

            const double Expected = 5;
            var actual = vector.Maximum();

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Can find minimum value index.
        /// </summary>
        [Test]
        public void CanFindMinimumIndex()
        {
            var vector = CreateVector(Data);

            const int Expected = 0;
            var actual = vector.MinimumIndex();

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Can find minimum value of a vector.
        /// </summary>
        [Test]
        public void CanFindMinimum()
        {
            var vector = CreateVector(Data);

            const double Expected = 1;
            var actual = vector.Minimum();

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Can compute the sum of a vector elements.
        /// </summary>
        [Test]
        public void CanSum()
        {
            double[] testData = { -20, -10, 10, 20, 30, };
            var vector = CreateVector(testData);
            var actual = vector.Sum();
            const double Expected = 30;
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Can compute the sum of the absolute value a vector elements.
        /// </summary>
        [Test]
        public void CanSumMagnitudes()
        {
            double[] testData = { -20, -10, 10, 20, 30, };
            var vector = CreateVector(testData);
            var actual = vector.SumMagnitudes();
            const double Expected = 90;
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// Set values with <c>null</c> parameter throw exception.
        /// </summary>
        [Test]
        public void SetValuesWithNullParameterThrowsArgumentException()
        {
            var vector = CreateVector(Data);
            Assert.Throws<ArgumentNullException>(() => vector.SetValues(null));
        }

        /// <summary>
        /// Set values with non-equal data length throw exception.
        /// </summary>
        [Test]
        public void SetValuesWithNonEqualDataLengthThrowsArgumentException()
        {
            var vector = CreateVector(Data.Length + 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => vector.SetValues(Data));
        }

        /// <summary>
        /// Generate a vector with number of elements less than zero throw an exception.
        /// </summary>
        [Test]
        public void RandomWithNumberOfElementsLessThanZeroThrowsArgumentException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => DenseVector.CreateRandom(-2, new ContinuousUniform()));
        }

        /// <summary>
        /// Can clear a vector.
        /// </summary>
        [Test]
        public void CanClearVector()
        {
            double[] testData = { -20, -10, 10, 20, 30, };
            var vector = CreateVector(testData);
            vector.Clear();
            foreach (var element in vector)
            {
                Assert.AreEqual(0.0, element);
            }
        }



        /// <summary>
        /// Test if find the correct mask of elements > testvalue
        /// </summary>
        [Test]
        public void CanFindMask()
        {
            var vector = CreateVector(Data);
            double testValue = 2.0;
            var maskvector = vector.FindMask(a => a > testValue);

            foreach (var indx in vector.GetIndexedEnumerator())
            {
                if (indx.Item2 > testValue)
                    Assert.AreEqual(1.0f, maskvector.At(indx.Item1));
                else
                    Assert.AreEqual(0.0f, maskvector.At(indx.Item1));
            }
        }


        /// <summary>
        /// Test if find the correct indeces of elements > testvalue
        /// </summary>
        [Test]
        public void CanFindIndices()
        {
            var vector = CreateVector(Data);
            double testValue = 2.0;
            var indEnum = vector.FindIndices(a => a > testValue);

            foreach (var indx in vector.GetIndexedEnumerator())
            {
                if (indx.Item2 > testValue)
                //then it should also be found in the indEnum 
                {
                    int found = 0;
                    foreach (int index in indEnum)
                    {
                        if (index == indx.Item1) found = 1;
                    }
                    Assert.AreEqual(found, 1);
                }

                else
                //it should not  be in indEnum 
                {
                    foreach (int index in indEnum)
                    {
                        Assert.AreNotEqual(index, indx.Item1);
                    }
                }
            }
        }


        /// <summary>
        /// Test if can set on indeces of elements.
        /// </summary>
        [Test]
        public void CanSetOnIndices()
        {
            var vector = CreateVector(Data);
            double testValue = 2.0;
            var indEnum = vector.FindIndices(a => a > testValue);

            //make sure at lesat one is found.
            int found = 0;
            foreach (int index in indEnum)
            {
                found = 1;
            }
            Assert.AreEqual(found, 1);

            vector.SetOnIndices(indEnum, testValue);


            //make sure none are found now.
            var indEnum2 = vector.FindIndices(a => a > testValue);
            int foundAfter = 0;
            foreach (int index in indEnum2)
            {
                found = 1;
            }
            Assert.AreEqual(foundAfter, 0);
        }



        /// <summary>
        /// Test if can apply  a formual on indeces of elements.
        /// </summary>
        [Test]
        public void CanApplyOnIndices()
        {
            var vector = CreateVector(Data);
            double testValue = 2.0;
            var indEnum = vector.FindIndices(a => a > testValue);

            //make sure at lesat one is found.
            int found = 0;
            foreach (int index in indEnum)
            {
                found = 1;
            }
            Assert.AreEqual(found, 1);

            Func<double, double> multiplywithMinus1 = a => -a;

            vector.ApplyOnIndices(indEnum, multiplywithMinus1);


            //make sure none are found now.
            var indEnum2 = vector.FindIndices(a => a > testValue);
            int foundAfter = 0;
            foreach (int index in indEnum2)
            {
                found = 1;
            }
            Assert.AreEqual(foundAfter, 0);
        }


        /// <summary>
        /// Test if can set subvector.
        /// </summary>
        [Test]
        public void CanSetSubVector()
        {
            var vector = CreateVector(Data); // 1,2,3,4,5
            double[] testData = { 20, 30 };
            double[] expectedResult = { 1, 20, 30, 4, 5 };
            var expectedVector = CreateVector(expectedResult);

            var subvector = CreateVector(testData);
            vector.SetSubVector(1, 2, subvector);  //uses 0 based indexing so 1 is the the 2nd element.
            // 1,2,3,4,5 => 1,20,30,4,5
            foreach (int i in vector.Indices())
            {
                Assert.AreEqual(vector.At(i), expectedVector.At(i));
            }
        }


        /// <summary>
        /// Test if can can set on a maskvector.
        /// </summary>
        [Test]
        public void CanOnMaskSet()
        {
            var vector = CreateVector(Data);
            double[] testMask = { 0, 0, 1, 1, 0 };
            float testValue = 30f;

            double[] expectedResult = { 1, 2, 30, 30, 5 };
            var expectedVector = CreateVector(expectedResult);

            var maskVector = CreateVector(testMask);

            vector.OnMaskSet(maskVector, testValue);
            // 1,2,3,4,5 => 1,2,30,30,5
            foreach (int i in vector.Indices())
            {
                Assert.AreEqual(vector.At(i), expectedVector.At(i));
            }
        }


        /// <summary>
        /// Test if can can apply a formula on a maskvector.
        /// </summary>
        [Test]
        public void CanOnMaskApply()
        {
            var vector = CreateVector(Data);
            double[] testMask = { 0.0, 0.0, 1.0, 1.0, 0.0 };
            Func<double, double> testFormula = elem => elem * elem;

            double[] expectedResult = { 1.0, 2.0, 9.0, 16.0, 5.0 };
            var expectedVector = CreateVector(expectedResult);

            var maskVector = CreateVector(testMask);

            vector.OnMaskApply(maskVector, testFormula);
            // 1,2,3,4,5 => 1,2,30,30,5
            foreach (int i in vector.Indices())
            {
                Assert.AreEqual(vector.At(i), expectedVector.At(i));
            }
        }



        /// <summary>
        /// Creates a new instance of the Vector class.
        /// </summary>
        /// <param name="size">The size of the <strong>Vector</strong> to construct.</param>
        /// <returns>The new <c>Vector</c>.</returns>
        protected abstract Vector<double> CreateVector(int size);

        /// <summary>
        /// Creates a new instance of the Vector class.
        /// </summary>
        /// <param name="data">The array to create this vector from.</param>
        /// <returns>The new <c>Vector</c>.</returns>
        protected abstract Vector<double> CreateVector(IList<double> data);
    }
}
