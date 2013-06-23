// <copyright file="Vector.cs" company="Math.NET">
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
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime;
    using Properties;
    using Storage;
    using System.Linq;
    /// <summary>
    /// Defines the generic class for <c>Vector</c> classes.
    /// </summary>
    /// <typeparam name="T">Supported data types are double, single, <see cref="Complex"/>, and <see cref="Complex32"/>.</typeparam>
    [Serializable]
    public abstract partial class Vector<T> :
        IFormattable, IEnumerable<T>, IEquatable<Vector<T>>, IList, IList<T>
#if !PORTABLE
        , ICloneable
#endif
        where T : struct, IEquatable<T>, IFormattable
    {
        /// <summary>
        /// The zero value for type T.
        /// </summary>
        private static readonly T Zero = Common.ZeroOf<T>();

        /// <summary>
        /// The value of 1.0 for type T.
        /// </summary>
        private static readonly T One = Common.OneOf<T>();

        /// <summary>
        /// Initializes a new instance of the Vector class.
        /// </summary>
        protected Vector(VectorStorage<T> storage)
        {
            Storage = storage;
            Count = storage.Length;
        }

        /// <summary>
        /// Gets the raw vector data storage.
        /// </summary>
        public VectorStorage<T> Storage { get; private set; }

        /// <summary>
        /// Gets the number of items.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>Gets or sets the value at the given <paramref name="index"/>.</summary>
        /// <param name="index">The index of the value to get or set.</param>
        /// <returns>The value of the vector at the given <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is negative or
        /// greater than the size of the vector.</exception>
        public T this[int index]
        {
            [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
            //[MethodImpl(MethodImplOptions.AggressiveInlining)] .Net 4.5 only
            get { return Storage[index]; }

            [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
            //[MethodImpl(MethodImplOptions.AggressiveInlining)] .Net 4.5 only
            set { Storage[index] = value;}
        }

        /// <summary>Gets the value at the given <paramref name="index"/> without range checking..</summary>
        /// <param name="index">The index of the value to get or set.</param>
        /// <returns>The value of the vector at the given <paramref name="index"/>.</returns>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        //[MethodImpl(MethodImplOptions.AggressiveInlining)] .Net 4.5 only
        public T At(int index)
        {
            return Storage.At(index);
        }

        /// <summary>Sets the <paramref name="value"/> at the given <paramref name="index"/> without range checking..</summary>
        /// <param name="index">The index of the value to get or set.</param>
        /// <param name="value">The value to set.</param>
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        //[MethodImpl(MethodImplOptions.AggressiveInlining)] .Net 4.5 only
        public void At(int index, T value)
        {
            Storage.At(index, value);
        }

        /// <summary>
        /// Resets all values to zero.
        /// </summary>
        public void Clear()
        {
            Storage.Clear();
        }

        /// <summary>
        /// Sets all values of a subvector to zero.
        /// </summary>
        public void ClearSubVector(int index, int count)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException("count", Resources.ArgumentMustBePositive);
            }

            if (index + count > Count || index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            Storage.Clear(index, count);
        }

        /// <summary>
        /// Creates a matrix with the given dimensions using the same storage type
        /// as this vector.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="columns">The number of columns.</param>
        /// <returns>A matrix with the given dimensions.</returns>
        public abstract Matrix<T> CreateMatrix(int rows, int columns);

        /// <summary>
        /// Creates a <strong>Vector</strong> of the given size using the same storage type
        /// as this vector.
        /// </summary>
        /// <param name="size">The size of the <strong>Vector</strong> to create.</param>
        /// <returns>The new <c>Vector</c>.</returns>
        public abstract Vector<T> CreateVector(int size);


        /// <summary>
        /// Negates vector and save result to <paramref name="result"/>
        /// </summary>
        /// <param name="result">Target vector</param>
        protected abstract void DoNegate(Vector<T> result);

        /// <summary>
        /// Complex conjugates vector and save result to <paramref name="result"/>
        /// </summary>
        /// <param name="result">Target vector</param>
        protected abstract void DoConjugate(Vector<T> result);

        /// <summary>
        /// Adds a scalar to each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to add.</param>
        /// <param name="result">The vector to store the result of the addition.</param>
        protected abstract void DoAdd(T scalar, Vector<T> result);

        /// <summary>
        /// Adds another vector to this vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to add to this one.</param>
        /// <param name="result">The vector to store the result of the addition.</param>
        protected abstract void DoAdd(Vector<T> other, Vector<T> result);

        /// <summary>
        /// Subtracts a scalar from each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to subtract.</param>
        /// <param name="result">The vector to store the result of the subtraction.</param>
        protected abstract void DoSubtract(T scalar, Vector<T> result);

        /// <summary>
        /// Subtracts each element of the vector from a scalar and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to subtract from.</param>
        /// <param name="result">The vector to store the result of the subtraction.</param>
        protected virtual void DoSubtractFrom(T scalar, Vector<T> result)
        {
            DoNegate(result);
            result.DoAdd(scalar, result);
        }

        /// <summary>
        /// Subtracts another vector to this vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to subtract from this one.</param>
        /// <param name="result">The vector to store the result of the subtraction.</param>
        protected abstract void DoSubtract(Vector<T> other, Vector<T> result);

        /// <summary>
        /// Multiplies a scalar to each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to multiply.</param>
        /// <param name="result">The vector to store the result of the multiplication.</param>
        protected abstract void DoMultiply(T scalar, Vector<T> result);

        /// <summary>
        /// Computes the dot product between this vector and another vector.
        /// </summary>
        /// <param name="other">The other vector to add.</param>
        /// <returns>The result of the addition.</returns>
        protected abstract T DoDotProduct(Vector<T> other);

        /// <summary>
        /// Divides each element of the vector by a scalar and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to divide with.</param>
        /// <param name="result">The vector to store the result of the division.</param>
        protected abstract void DoDivide(T scalar, Vector<T> result);

        /// <summary>
        /// Divides a scalar by each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to divide.</param>
        /// <param name="result">The vector to store the result of the division.</param>
        protected abstract void DoDivideByThis(T scalar, Vector<T> result);

        /// <summary>
        /// Computes the modulus for each element of the vector for the given divisor.
        /// </summary>
        /// <param name="scalar">The divisor to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected abstract void DoModulus(T scalar, Vector<T> result);

        /// <summary>
        /// Computes the modulus for the given dividend for each element of the vector.
        /// </summary>
        /// <param name="scalar">The dividend to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected abstract void DoModulusByThis(T scalar, Vector<T> result);

        /// <summary>
        /// Pointwise multiplies this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to pointwise multiply with this one.</param>
        /// <param name="result">The vector to store the result of the pointwise multiplication.</param>
        protected abstract void DoPointwiseMultiply(Vector<T> other, Vector<T> result);

        /// <summary>
        /// Pointwise divide this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to pointwise divide this one by.</param>
        /// <param name="result">The result of the division.</param>
        protected abstract void DoPointwiseDivide(Vector<T> other, Vector<T> result);

        /// <summary>
        /// Pointwise modulus this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to pointwise modulus this one by.</param>
        /// <param name="result">The result of the modulus.</param>
        protected abstract void DoPointwiseModulus(Vector<T> other, Vector<T> result);

        /// <summary>
        /// Adds a scalar to each element of the vector.
        /// </summary>
        /// <param name="scalar">The scalar to add.</param>
        /// <returns>A copy of the vector with the scalar added.</returns>
        public Vector<T> Add(T scalar)
        {
            if (scalar.Equals(Zero))
            {
                return Clone();
            }

            var result = CreateVector(Count);
            DoAdd(scalar, result);
            return result;
        }

        /// <summary>
        /// Adds a scalar to each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to add.</param>
        /// <param name="result">The vector to store the result of the addition.</param>
        /// <exception cref="ArgumentNullException">If the result vector is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void Add(T scalar, Vector<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (Count != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            if (scalar.Equals(Zero))
            {
                CopyTo(result);
                return;
            }

            DoAdd(scalar, result);
        }

        /// <summary>
        /// Returns a copy of this vector.
        /// </summary>
        /// <returns>This vector.</returns>
        /// <remarks>
        /// Added as an alternative to the unary addition operator.
        /// </remarks>
        [Obsolete("Use Clone instead. Scheduled for removal in v3.0.")]
        public Vector<T> Plus()
        {
            return Clone();
        }

        /// <summary>
        /// Adds another vector to this vector.
        /// </summary>
        /// <param name="other">The vector to add to this one.</param>
        /// <returns>A new vector containing the sum of both vectors.</returns>
        /// <exception cref="ArgumentNullException">If the other vector is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
        public Vector<T> Add(Vector<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (Count != other.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "other");
            }

            var result = CreateVector(Count);
            DoAdd(other, result);
            return result;
        }

        /// <summary>
        /// Adds another vector to this vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to add to this one.</param>
        /// <param name="result">The vector to store the result of the addition.</param>
        /// <exception cref="ArgumentNullException">If the other vector is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">If the result vector is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void Add(Vector<T> other, Vector<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (Count != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            DoAdd(other, result);
        }

        /// <summary>
        /// Subtracts a scalar from each element of the vector.
        /// </summary>
        /// <param name="scalar">The scalar to subtract.</param>
        /// <returns>A new vector containing the subtraction of this vector and the scalar.</returns>
        public Vector<T> Subtract(T scalar)
        {
            if (scalar.Equals(Zero))
            {
                return Clone();
            }

            var result = CreateVector(Count);
            DoSubtract(scalar, result);
            return result;
        }

        /// <summary>
        /// Subtracts a scalar from each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to subtract.</param>
        /// <param name="result">The vector to store the result of the subtraction.</param>
        /// <exception cref="ArgumentNullException">If the result vector is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void Subtract(T scalar, Vector<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (Count != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            if (scalar.Equals(Zero))
            {
                CopyTo(result);
                return;
            }

            DoSubtract(scalar, result);
        }

        /// <summary>
        /// Subtracts each element of the vector from a scalar.
        /// </summary>
        /// <param name="scalar">The scalar to subtract from.</param>
        /// <returns>A new vector containing the subtraction of the scalar and this vector.</returns>
        public Vector<T> SubtractFrom(T scalar)
        {
            var result = CreateVector(Count);
            DoSubtractFrom(scalar, result);
            return result;
        }

        /// <summary>
        /// Subtracts each element of the vector from a scalar and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to subtract from.</param>
        /// <param name="result">The vector to store the result of the subtraction.</param>
        /// <exception cref="ArgumentNullException">If the result vector is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void SubtractFrom(T scalar, Vector<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (Count != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            DoSubtractFrom(scalar, result);
        }

        /// <summary>
        /// Returns a negated vector.
        /// </summary>
        /// <returns>The negated vector.</returns>
        /// <remarks>Added as an alternative to the unary negation operator.</remarks>
        public Vector<T> Negate()
        {
            var retrunVector = CreateVector(Count);
            DoNegate(retrunVector);
            return retrunVector;
        }

        /// <summary>
        /// Negates vector and save result to <paramref name="result"/>
        /// </summary>
        /// <param name="result">Target vector</param>
        public void Negate(Vector<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (Count != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            DoNegate(result);
        }

        /// <summary>
        /// Subtracts another vector from this vector.
        /// </summary>
        /// <param name="other">The vector to subtract from this one.</param>
        /// <returns>A new vector containing the subtraction of the the two vectors.</returns>
        /// <exception cref="ArgumentNullException">If the other vector is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
        public Vector<T> Subtract(Vector<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (Count != other.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "other");
            }

            var result = CreateVector(Count);
            DoSubtract(other, result);
            return result;
        }

        /// <summary>
        /// Subtracts another vector to this vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to subtract from this one.</param>
        /// <param name="result">The vector to store the result of the subtraction.</param>
        /// <exception cref="ArgumentNullException">If the other vector is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">If the result vector is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void Subtract(Vector<T> other, Vector<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (Count != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            DoSubtract(other, result);
        }

        /// <summary>
        /// Return vector with complex conjugate values of the source vector
        /// </summary>
        /// <returns>Conjugated vector</returns>
        public Vector<T> Conjugate()
        {
            var retrunVector = CreateVector(Count);
            DoConjugate(retrunVector);
            return retrunVector;
        }

        /// <summary>
        /// Complex conjugates vector and save result to <paramref name="result"/>
        /// </summary>
        /// <param name="result">Target vector</param>
        public void Conjugate(Vector<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (Count != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            DoConjugate(result);
        }

        /// <summary>
        /// Multiplies a scalar to each element of the vector.
        /// </summary>
        /// <param name="scalar">The scalar to multiply.</param>
        /// <returns>A new vector that is the multiplication of the vector and the scalar.</returns>
        public Vector<T> Multiply(T scalar)
        {
            if (scalar.Equals(One))
            {
                return Clone();
            }

            if (scalar.Equals(Zero))
            {
                return CreateVector(Count);
            }

            var result = CreateVector(Count);
            DoMultiply(scalar, result);
            return result;
        }

        /// <summary>
        /// Multiplies a scalar to each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to multiply.</param>
        /// <param name="result">The vector to store the result of the multiplication.</param>
        /// <exception cref="ArgumentNullException">If the result vector is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void Multiply(T scalar, Vector<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (Count != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            if (scalar.Equals(One))
            {
                CopyTo(result);
                return;
            }

            if (scalar.Equals(Zero))
            {
                result.Clear();
                return;
            }

            DoMultiply(scalar, result);
        }

        /// <summary>
        /// Computes the dot product between this vector and another vector.
        /// </summary>
        /// <param name="other">The other vector to add.</param>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="ArgumentException">If <paramref name="other"/> is not of the same size.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null"/>.</exception>
        public T DotProduct(Vector<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (Count != other.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "other");
            }

            return DoDotProduct(other);
        }

        /// <summary>
        /// Divides each element of the vector by a scalar.
        /// </summary>
        /// <param name="scalar">The scalar to divide with.</param>
        /// <returns>A new vector that is the division of the vector and the scalar.</returns>
        public Vector<T> Divide(T scalar)
        {
            if (scalar.Equals(One))
            {
                return Clone();
            }

            var result = CreateVector(Count);
            DoDivide(scalar, result);
            return result;
        }

        /// <summary>
        /// Divides each element of the vector by a scalar and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to divide with.</param>
        /// <param name="result">The vector to store the result of the division.</param>
        /// <exception cref="ArgumentNullException">If the result vector is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void Divide(T scalar, Vector<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (Count != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            if (scalar.Equals(One))
            {
                CopyTo(result);
                return;
            }

            DoDivide(scalar, result);
        }

        /// <summary>
        /// Divides a scalar by each element of the vector.
        /// </summary>
        /// <param name="scalar">The scalar to divide.</param>
        /// <returns>A new vector that is the division of the vector and the scalar.</returns>
        public Vector<T> DevideByThis(T scalar)
        {
            var result = CreateVector(Count);
            DoDivideByThis(scalar, result);
            return result;
        }

        /// <summary>
        /// Divides a scalar by each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to divide.</param>
        /// <param name="result">The vector to store the result of the division.</param>
        /// <exception cref="ArgumentNullException">If the result vector is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void DivideByThis(T scalar, Vector<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (Count != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            DoDivideByThis(scalar, result);
        }

        /// <summary>
        /// Computes the modulus for each element of the vector for the given divisor.
        /// </summary>
        /// <param name="scalar">The divisor to use.</param>
        /// <returns>A vector containing the result.</returns>
        public Vector<T> Modulus(T scalar)
        {
            var result = CreateVector(Count);
            DoModulus(scalar, result);
            return result;
        }

        /// <summary>
        /// Computes the modulus for each element of the vector for the given divisor.
        /// </summary>
        /// <param name="scalar">The divisor to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        public void Modulus(T scalar, Vector<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (Count != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            DoModulus(scalar, result);
        }

        /// <summary>
        /// Computes the modulus for the given dividend for each element of the vector.
        /// </summary>
        /// <param name="scalar">The dividend to use.</param>
        /// <returns>A vector containing the result.</returns>
        public Vector<T> ModulusByThis(T scalar)
        {
            var result = CreateVector(Count);
            DoModulusByThis(scalar, result);
            return result;
        }

        /// <summary>
        /// Computes the modulus for the given dividend for each element of the vector.
        /// </summary>
        /// <param name="scalar">The dividend to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        public void ModulusByThis(T scalar, Vector<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (Count != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            DoModulusByThis(scalar, result);
        }

        /// <summary>
        /// Pointwise multiplies this vector with another vector.
        /// </summary>
        /// <param name="other">The vector to pointwise multiply with this one.</param>
        /// <returns>A new vector which is the pointwise multiplication of the two vectors.</returns>
        /// <exception cref="ArgumentNullException">If the other vector is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
        public Vector<T> PointwiseMultiply(Vector<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (Count != other.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "other");
            }

            var result = CreateVector(Count);
            DoPointwiseMultiply(other, result);
            return result;
        }

        /// <summary>
        /// Pointwise multiplies this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to pointwise multiply with this one.</param>
        /// <param name="result">The vector to store the result of the pointwise multiplication.</param>
        /// <exception cref="ArgumentNullException">If the other vector is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException">If the result vector is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void PointwiseMultiply(Vector<T> other, Vector<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (Count != other.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "other");
            }

            if (Count != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            DoPointwiseMultiply(other, result);
        }

        /// <summary>
        /// Pointwise divide this vector with another vector.
        /// </summary>
        /// <param name="other">The vector to pointwise divide this one by.</param>
        /// <returns>A new vector which is the pointwise division of the two vectors.</returns>
        /// <exception cref="ArgumentNullException">If the other vector is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
        public Vector<T> PointwiseDivide(Vector<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (Count != other.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "other");
            }

            var result = CreateVector(Count);
            DoPointwiseDivide(other, result);
            return result;
        }

        /// <summary>
        /// Pointwise divide this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to pointwise divide this one by.</param>
        /// <param name="result">The vector to store the result of the pointwise division.</param>
        /// <exception cref="ArgumentNullException">If the other vector is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException">If the result vector is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void PointwiseDivide(Vector<T> other, Vector<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (Count != other.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "other");
            }

            if (Count != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            DoPointwiseDivide(other, result);
        }

        /// <summary>
        /// Pointwise modulus this vector with another vector.
        /// </summary>
        /// <param name="other">The vector to pointwise modulus this one by.</param>
        /// <returns>A new vector which is the pointwise modulus of the two vectors.</returns>
        /// <exception cref="ArgumentNullException">If the other vector is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
        public Vector<T> PointwiseModulus(Vector<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (Count != other.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "other");
            }

            var result = CreateVector(Count);
            DoPointwiseModulus(other, result);
            return result;
        }

        /// <summary>
        /// Pointwise modulus this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to pointwise modulus this one by.</param>
        /// <param name="result">The vector to store the result of the pointwise modulus.</param>
        /// <exception cref="ArgumentNullException">If the other vector is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException">If the result vector is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void PointwiseModulus(Vector<T> other, Vector<T> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (Count != other.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "other");
            }

            if (Count != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            DoPointwiseModulus(other, result);
        }

        /// <summary>
        /// Outer product of two vectors
        /// </summary>
        /// <param name="u">First vector</param>
        /// <param name="v">Second vector</param>
        /// <returns>Matrix M[i,j] = u[i]*v[j] </returns>
        /// <exception cref="ArgumentNullException">If the u vector is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException">If the v vector is <see langword="null" />.</exception>
        public static Matrix<T> OuterProduct(Vector<T> u, Vector<T> v)
        {
            if (u == null)
            {
                throw new ArgumentNullException("u");
            }

            if (v == null)
            {
                throw new ArgumentNullException("v");
            }

            var matrix = u.CreateMatrix(u.Count, v.Count);

            for (var i = 0; i < u.Count; i++)
            {
                matrix.SetRow(i, v.Multiply(u.At(i)));
            }

            return matrix;
        }

        /// <summary>
        /// Outer product of this and another vector.
        /// </summary>
        /// <param name="v">The vector to operate on.</param>
        /// <returns>
        /// Matrix M[i,j] = this[i] * v[j].
        /// </returns>
        /// <seealso cref="OuterProduct(Vector{T}, Vector{T})"/>
        public Matrix<T> OuterProduct(Vector<T> v)
        {
            return OuterProduct(this, v);
        }

        /// <summary>
        /// Returns a <strong>Vector</strong> containing the same values of <paramref name="rightSide"/>.
        /// </summary>
        /// <remarks>This method is included for completeness.</remarks>
        /// <param name="rightSide">The vector to get the values from.</param>
        /// <returns>A vector containing the same values as <paramref name="rightSide"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static Vector<T> operator +(Vector<T> rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return rightSide.Clone();
        }

        /// <summary>
        /// Returns a <strong>Vector</strong> containing the negated values of <paramref name="rightSide"/>.
        /// </summary>
        /// <param name="rightSide">The vector to get the values from.</param>
        /// <returns>A vector containing the negated values as <paramref name="rightSide"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static Vector<T> operator -(Vector<T> rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return rightSide.Negate();
        }

        /// <summary>
        /// Adds two <strong>Vectors</strong> together and returns the results.
        /// </summary>
        /// <param name="leftSide">One of the vectors to add.</param>
        /// <param name="rightSide">The other vector to add.</param>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="ArgumentException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> are not the same size.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static Vector<T> operator +(Vector<T> leftSide, Vector<T> rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return leftSide.Add(rightSide);
        }

        /// <summary>
        /// Adds a scalar to each element of a vector.
        /// </summary>
        /// <param name="leftSide">The vector to add to.</param>
        /// <param name="rightSide">The scalar value to add.</param>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static Vector<T> operator +(Vector<T> leftSide, T rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return leftSide.Add(rightSide);
        }

        /// <summary>
        /// Adds a scalar to each element of a vector.
        /// </summary>
        /// <param name="leftSide">The scalar value to add.</param>
        /// <param name="rightSide">The vector to add to.</param>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static Vector<T> operator +(T leftSide, Vector<T> rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return rightSide.Add(leftSide);
        }

        /// <summary>
        /// Subtracts two <strong>Vectors</strong> and returns the results.
        /// </summary>
        /// <param name="leftSide">The vector to subtract from.</param>
        /// <param name="rightSide">The vector to subtract.</param>
        /// <returns>The result of the subtraction.</returns>
        /// <exception cref="ArgumentException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> are not the same size.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static Vector<T> operator -(Vector<T> leftSide, Vector<T> rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return leftSide.Subtract(rightSide);
        }

        /// <summary>
        /// Subtracts a scalar from each element of a vector.
        /// </summary>
        /// <param name="leftSide">The vector to subtract from.</param>
        /// <param name="rightSide">The scalar value to subtract.</param>
        /// <returns>The result of the subtraction.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static Vector<T> operator -(Vector<T> leftSide, T rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return leftSide.Subtract(rightSide);
        }

        /// <summary>
        /// Substracts each element of a vector from a scalar.
        /// </summary>
        /// <param name="leftSide">The scalar value to subtract from.</param>
        /// <param name="rightSide">The vector to subtract.</param>
        /// <returns>The result of the subtraction.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static Vector<T> operator -(T leftSide, Vector<T> rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return rightSide.SubtractFrom(leftSide);
        }

        /// <summary>
        /// Multiplies a vector with a scalar.
        /// </summary>
        /// <param name="leftSide">The vector to scale.</param>
        /// <param name="rightSide">The scalar value.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static Vector<T> operator *(Vector<T> leftSide, T rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return leftSide.Multiply(rightSide);
        }

        /// <summary>
        /// Multiplies a vector with a scalar.
        /// </summary>
        /// <param name="leftSide">The scalar value.</param>
        /// <param name="rightSide">The vector to scale.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static Vector<T> operator *(T leftSide, Vector<T> rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return rightSide.Multiply(leftSide);
        }

        /// <summary>
        /// Computes the dot product between two <strong>Vectors</strong>.
        /// </summary>
        /// <param name="leftSide">The left row vector.</param>
        /// <param name="rightSide">The right column vector.</param>
        /// <returns>The dot product between the two vectors.</returns>
        /// <exception cref="ArgumentException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> are not the same size.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static T operator *(Vector<T> leftSide, Vector<T> rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return leftSide.DotProduct(rightSide);
        }

        /// <summary>
        /// Divides a scalar with a vector.
        /// </summary>
        /// <param name="leftSide">The scalar to divide.</param>
        /// <param name="rightSide">The vector.</param>
        /// <returns>The result of the division.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static Vector<T> operator /(T leftSide, Vector<T> rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return rightSide.DevideByThis(leftSide);
        }

        /// <summary>
        /// Divides a vector with a scalar.
        /// </summary>
        /// <param name="leftSide">The vector to divide.</param>
        /// <param name="rightSide">The scalar value.</param>
        /// <returns>The result of the division.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static Vector<T> operator /(Vector<T> leftSide, T rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return leftSide.Divide(rightSide);
        }

        /// <summary>
        /// Pointwise divides two <strong>Vectors</strong>.
        /// </summary>
        /// <param name="leftSide">The vector to divide.</param>
        /// <param name="rightSide">The other vector.</param>
        /// <returns>The result of the division.</returns>
        /// <exception cref="ArgumentException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> are not the same size.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static Vector<T> operator /(Vector<T> leftSide, Vector<T> rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return leftSide.PointwiseDivide(rightSide);
        }

        /// <summary>
        /// Computes the modulus of each element of the vector of the given divisor.
        /// </summary>
        /// <param name="leftSide">The vector whose elements we want to compute the modulus of.</param>
        /// <param name="rightSide">The divisor to use.</param>
        /// <returns>The result of the calculation</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static Vector<T> operator %(Vector<T> leftSide, T rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return leftSide.Modulus(rightSide);
        }

        /// <summary>
        /// Computes the modulus of the given dividend of each element of the vector.
        /// </summary>
        /// <param name="leftSide">The dividend we want to compute the modulus of.</param>
        /// <param name="rightSide">The vector whose elements we want to use as divisor.</param>
        /// <returns>The result of the calculation</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static Vector<T> operator %(T leftSide, Vector<T> rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return rightSide.ModulusByThis(leftSide);
        }

        /// <summary>
        /// Computes the pointwise modulus of each element of two <strong>vectors</strong>.
        /// </summary>
        /// <param name="leftSide">The vector whose elements we want to compute the modulus of.</param>
        /// <param name="rightSide">The divisor to use.</param>
        /// <returns>The result of the calculation</returns>
        /// <exception cref="ArgumentException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> are not the same size.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static Vector<T> operator %(Vector<T> leftSide, Vector<T> rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return leftSide.PointwiseModulus(rightSide);
        }

        /// <summary>
        /// Computes the p-Norm.
        /// </summary>
        /// <param name="p">
        /// The p value.
        /// </param>
        /// <returns>
        /// <c>Scalar ret = (sum(abs(this[i])^p))^(1/p)</c>
        /// </returns>
        public abstract T Norm(double p);

        /// <summary>
        /// Normalizes this vector to a unit vector with respect to the p-norm.
        /// </summary>
        /// <param name="p">
        /// The p value.
        /// </param>
        /// <returns>
        /// This vector normalized to a unit vector with respect to the p-norm.
        /// </returns>
        public abstract Vector<T> Normalize(double p);

        /// <summary>
        /// Returns the value of the absolute minimum element.
        /// </summary>
        /// <returns>The value of the absolute minimum element.</returns>
        public abstract T AbsoluteMinimum();

        /// <summary>
        /// Returns the index of the absolute minimum element.
        /// </summary>
        /// <returns>The index of absolute minimum element.</returns>
        public abstract int AbsoluteMinimumIndex();

        /// <summary>
        /// Returns the value of the absolute maximum element.
        /// </summary>
        /// <returns>The value of the absolute maximum element.</returns>
        public abstract T AbsoluteMaximum();

        /// <summary>
        /// Returns the index of the absolute maximum element.
        /// </summary>
        /// <returns>The index of absolute maximum element.</returns>
        public abstract int AbsoluteMaximumIndex();

        /// <summary>
        /// Returns the value of maximum element.
        /// </summary>
        /// <returns>The value of maximum element.</returns>
        public T Maximum()
        {
            return At(MaximumIndex());
        }

        /// <summary>
        /// Returns the index of the absolute maximum element.
        /// </summary>
        /// <returns>The index of absolute maximum element.</returns>
        public abstract int MaximumIndex();

        /// <summary>
        /// Returns the value of the minimum element.
        /// </summary>
        /// <returns>The value of the minimum element.</returns>
        public T Minimum()
        {
            return At(MinimumIndex());
        }

        /// <summary>
        /// Returns the index of the minimum element.
        /// </summary>
        /// <returns>The index of minimum element.</returns>
        public abstract int MinimumIndex();

        /// <summary>
        /// Computes the sum of the vector's elements.
        /// </summary>
        /// <returns>The sum of the vector's elements.</returns>
        public abstract T Sum();

        /// <summary>
        /// Computes the sum of the absolute value of the vector's elements.
        /// </summary>
        /// <returns>The sum of the absolute value of the vector's elements.</returns>
        public abstract T SumMagnitudes();

        /// <summary>
        /// Returns a deep-copy clone of the vector.
        /// </summary>
        /// <returns>A deep-copy clone of the vector.</returns>
        public Vector<T> Clone()
        {
            var result = CreateVector(Count);
            Storage.CopyToUnchecked(result.Storage, skipClearing: true);
            return result;
        }

        /// <summary>
        /// Set the values of this vector to the given values.
        /// </summary>
        /// <param name="values">The array containing the values to use.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="values"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If <paramref name="values"/> is not the same size as this vector.</exception>
        public void SetValues(T[] values)
        {
            var source = new DenseVectorStorage<T>(Count, values);
            source.CopyTo(Storage);
        }

        /// <summary>
        /// Copies the values of this vector into the target vector.
        /// </summary>
        /// <param name="target">The vector to copy elements into.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="target"/> is not the same size as this vector.</exception>
        public void CopyTo(Vector<T> target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            Storage.CopyTo(target.Storage);
        }

        /// <summary>
        /// Creates a vector containing specified elements.
        /// </summary>
        /// <param name="index">The first element to begin copying from.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <returns>A vector containing a copy of the specified elements.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><list><item>If <paramref name="index"/> is not positive or
        /// greater than or equal to the size of the vector.</item>
        /// <item>If <paramref name="index"/> + <paramref name="count"/> is greater than or equal to the size of the vector.</item>
        /// </list></exception>
        /// <exception cref="ArgumentException">If <paramref name="count"/> is not positive.</exception>
        public Vector<T> SubVector(int index, int count)
        {
            var target = CreateVector(count);
            Storage.CopySubVectorTo(target.Storage, index, 0, count, skipClearing: true);
            return target;
        }

        /// <summary>
        /// Copies the values of a given vector into a region in this vector.
        /// </summary>
        /// <param name="index">The field to start copying to</param>
        /// <param name="count">The number of fields to cpy. Must be positive.</param>
        /// <param name="subVector">The sub-vector to copy from.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="subVector"/> is <see langword="null" /></exception>
        public void SetSubVector(int index, int count, Vector<T> subVector)
        {
            if (subVector == null)
            {
                throw new ArgumentNullException("subVector");
            }

            subVector.Storage.CopySubVectorTo(Storage, 0, index, count);
        }

        /// <summary>
        /// Copies the requested elements from this vector to another.
        /// </summary>
        /// <param name="destination">The vector to copy the elements to.</param>
        /// <param name="sourceIndex">The element to start copying from.</param>
        /// <param name="targetIndex">The element to start copying to.</param>
        /// <param name="count">The number of elements to copy.</param>
        public void CopySubVectorTo(Vector<T> destination, int sourceIndex, int targetIndex, int count)
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            // TODO: refactor range checks
            Storage.CopySubVectorTo(destination.Storage, sourceIndex, targetIndex, count);
        }

        [Obsolete("Use CopySubVectorTo instead. Scheduled for removal in v3.0.")]
        public void CopyTo(Vector<T> destination, int sourceIndex, int targetIndex, int count)
        {
            CopySubVectorTo(destination, sourceIndex, targetIndex, count);
        }

        /// <summary>
        /// Returns the data contained in the vector as an array.
        /// </summary>
        /// <returns>
        /// The vector's data as an array.
        /// </returns>
        public T[] ToArray()
        {
            var result = new DenseVectorStorage<T>(Count);
            Storage.CopyToUnchecked(result, skipClearing: true);
            return result.Data;
        }

        /// <summary>
        /// Create a matrix based on this vector in column form (one single column).
        /// </summary>
        /// <returns>
        /// This vector as a column matrix.
        /// </returns>
        public Matrix<T> ToColumnMatrix()
        {
            var result = CreateMatrix(Count, 1);
            Storage.CopyToColumnUnchecked(result.Storage, 0, skipClearing: true);
            return result;
        }

        /// <summary>
        /// Create a matrix based on this vector in row form (one single row).
        /// </summary>
        /// <returns>
        /// This vector as a row matrix.
        /// </returns>
        public Matrix<T> ToRowMatrix()
        {
            var result = CreateMatrix(1, Count);
            Storage.CopyToRowUnchecked(result.Storage, 0, skipClearing: true);
            return result;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Storage.Enumerate().GetEnumerator();
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> that contains the position and value of the element, for all non-zero elements.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> over this vector that contains the position and value of each element.
        /// </returns>
        /// <remarks>
        /// The enumerator returns a <seealso cref="Tuple{T,K}"/>
        /// with the first value being the element index and the second value
        /// being the value of the element at that index.
        /// The enumerator will exclude all elements with a zero value.
        /// </remarks>
        public IEnumerable<Tuple<int, T>> GetIndexedEnumerator()
        {
            return Storage.EnumerateNonZero();
        }

        /// <summary>
        /// Applies a function to each value of this vector and replaces the value with its result.
        /// If forceMapZero is not set to true, zero values may or may not be skipped depending
        /// on the actual data storage implementation (relevant mostly for sparse vectors).
        /// </summary>
        public void MapInplace(Func<T, T> f, bool forceMapZeros = false)
        {
            Storage.MapInplace(f, forceMapZeros);
        }

        /// <summary>
        /// Applies a function to each value of this vector and replaces the value with its result.
        /// The index of each value (zero-based) is passed as first argument to the function.
        /// If forceMapZero is not set to true, zero values may or may not be skipped depending
        /// on the actual data storage implementation (relevant mostly for sparse vectors).
        /// </summary>
        public void MapIndexedInplace(Func<int, T, T> f, bool forceMapZeros = false)
        {
            Storage.MapIndexedInplace(f, forceMapZeros);
        }
    


    
        /// <summary>
        /// Tests  all the elements of a given Vector  for a conditon and returns a Vector of the same dimesionns of 1.0 where this conditions is true.  
        /// 0.0 where the condition is not true. The condition is given using System.Predicate;
        /// </summary>
        /// <param name="matchCondition">System.Predicate. The condition that the Vector elements are tested for.</param>
        /// <returns> The resultant Vector.</returns>
        /// <exception cref="ArgumentNullException">If matchCondition is <see langword="null" />.</exception>
        public virtual Vector<T> FindMask(Predicate<T> matchCondition)
        {
            if (matchCondition == null) throw new ArgumentNullException("matchCondition");

            var result = CreateVector(this.Count);

            for (var index = 0; index < Count; index++)
            {
                    if (matchCondition(At(index)))
                    {
                        result.At(index, One);
                    }
            }

            return result;
        }


        /// <summary>
        /// Tests  all the elements of a given Vector  for a conditon and returns an <see cref="IEnumerable{T}"/> of indeces (0 based) where this conditions is true.  
        /// The condition is given using System.Predicate;
        /// </summary>
        /// <param name="matchCondition">System.Predicate. The condition that the Vector elements are tested for.</param>
        /// <returns> The resultant <see cref="IEnumerable{T}"/> of ints where the Predicate is true , using 0 based indexing.</returns>
        /// <exception cref="ArgumentNullException">If matchCondition is <see langword="null" />.</exception>
        public virtual IEnumerable<int> FindIndices(Predicate<T> matchCondition)
        {
            if (matchCondition == null) throw new ArgumentNullException("matchCondition");

            for (var index = 0; index < Count; index++)
            {
                if (matchCondition(At(index)))
                {
                    yield return index;
                }
            }

            yield break;
        }

        /// <summary>
        /// For each int in an enumerable of ints set all the elements of the given Vector to a specified value.
        /// </summary>
        /// <param name="indxs">The enumerables of ints, using 0 based indexing.</param>
        /// <param name="value">The value to set the Elements to.</param>
        /// <exception cref="ArgumentNullException">If indxs is <see langword="null" />.</exception>
        public virtual void SetOnIndices(IEnumerable<int> indxs, T value)
        {
            if (indxs == null) throw new ArgumentNullException("indxs");

            foreach (int i in indxs)
            { 
                At(i,value); 
            }
        }

        /// <summary>
        /// For each int in an enumerable of ints applies a function to the given Vector and sets the elements to the outcome of the function.
        /// </summary>
        /// <param name="indxs">The enumerables of ints , using 0 based indexing</param>
        /// <param name="fun"> The function </param>
        /// <exception cref="ArgumentNullException">If indxs is <see langword="null" />.</exception>
        public virtual void ApplyOnIndices(IEnumerable<int> indxs, Func<T, T> fun)
        {
            if (indxs == null) throw new ArgumentNullException("indxs");

            foreach (int i in indxs)
            {
                At( i,  fun(At(i)) );
            }
        }

        
        /// <summary>
        /// Updates specified elements of the Vector.
        /// </summary>
        /// <param name="index">The first element to begin copying from.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <param name="vectorToCopyFrom">The vector to copy from .</param>
        /// <exception cref="ArgumentOutOfRangeException"><list><item>If <paramref name="index"/> is not positive or
        /// greater than or equal to the size of the vector.</item>
        /// <item>If <paramref name="index"/> + <paramref name="length"/> is greater than or equal to the size of the vector.</item>
        /// </list></exception>
        /// <exception cref="ArgumentException">If <paramref name="length"/> is not positive.</exception>
        public virtual void SetSubVector(int index, int length, Vector<T> vectorToCopyFrom)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            if (index + length > Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (length > vectorToCopyFrom.Count)
            {
                throw new ArgumentOutOfRangeException("length, length greater than length of vectorToCopyFrom.");
            }


            CommonParallel.For(
                index,
                index + length,
                i => this.At(i,vectorToCopyFrom.At(i-index)));
        }


        /// <summary>
        /// Updates specified elements of the Vector with the other vector's elements.
        /// </summary>
        /// <param name="index">The first element to begin copying from.</param>
        /// <param name="vectorToCopyFrom">The vector to copy from .</param>
        /// <exception cref="ArgumentOutOfRangeException"><list><item>If <paramref name="index"/> is not positive or
        /// greater than or equal to the size of the vector.</item>
        /// <item>If <paramref name="index"/> + <paramref name="vectorToCopyFrom"/> 'size  is greater than or equal to the size of the vector.</item>
        public virtual void SetSubVector(int index, Vector<T> vectorToCopyFrom)
        {
            SetSubVector(index, vectorToCopyFrom.Count, vectorToCopyFrom);
        }


        /// <summary>
        /// Set the element of a given Vector  to a spefic value, where the mask Vectors elements ==1. 
        /// The Mask Matrix needs to have the same dimension of the given matrix.
        /// </summary>
        /// <param name="mask">The Mask Vector with elements 1's and 0's. needs to be of the same dimesions as this. </param>
        /// <param name="value"> The value to set the elements of the given vector to. </param>
        /// <exception cref="ArgumentNullException">If the mask vector is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the mask vectors dimensions are not equal to this dimesions.</exception>
        public virtual void OnMaskSet(Vector<T> mask, T value)
        {
            if (mask == null)
            {
                throw new ArgumentNullException("mask");
            }

            if (mask.Count != this.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "mask");
            }

            for (var index = 0; index < Count; index++)
            {
                if (Equals(mask.At(index), One))
                {
                    At(index, value);
                }
            }
        }

        /// <summary>
        /// Applies a function to the elements of a given Vector , where the mask Vector's elements == 1. 
        /// The Mask Vector needs to have the same dimension of the given vector.
        /// </summary>
        /// <param name="mask">The Mask Vector needs to be of the same dimesions as this. </param>
        /// <param name="func"></param>
        /// <exception cref="ArgumentNullException">If the mask vector is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException">If func is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the mask vectors dimensions are not equal to this dimesions.</exception>
        public virtual void OnMaskApply(Vector<T> mask, Func<T, T> func)
        {

            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            if (mask == null )
            {
                throw new ArgumentNullException("mask");
            }


            if (mask.Count != this.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "mask");
            }

            for (var index = 0; index < Count; index++)
            {
                if (Equals(mask.At(index), One))
                {
                     At(index, func(At(index)));
                }
            }
   
   
        }


        /// <summary>
        /// Applies a function to all the elements of a given Vector. No specialisation for SparsVectors yet.
        /// </summary>
        /// <param name="func"></param>
        /// <exception cref="ArgumentNullException">If func is <see langword="null" />.</exception>
        public virtual void OnMaskApply( Func<T, T> func)
        {

            if (func == null)
            {
                throw new ArgumentNullException("func");
            }


            for (var index = 0; index < Count; index++)
            {
                  At(index, func(At(index)));
                
            }

        }


        /// <summary>
        /// Returns an <see cref="IEnumerator{int}"/> that enumerates over the vectors  indices. 0..Count-1
        /// </summary>
        /// <returns>An <see cref="IEnumerator{int}"/> that enumerates over the vectors indices</returns>
        public IEnumerable<int> Indices()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return i;
            }
            yield break;
        }

                /// <summary>
        ///  one based index methods so start counting at 1 instead of 0. Romans did not have a number for 0 so using Roman I to after the same name 
        ///  of the 0 based index methids for these methods.
        /// </summary>
        /// <returns></returns>
        /////


        /// <summary>
        /// Creates a vector containing specified elements. This methos uses 1 based indexing for the arguments.
        /// </summary>
        /// <param name="index">The first element to begin copying from.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <returns>A vector containing a copy of the specified elements.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><list><item>If <paramref name="index"/> is not positive or
        /// greater than or equal to the size of the vector.</item>
        /// <item>If <paramref name="IIndex"/> + <paramref name="length"/> is greater than the size of the vector.</item>
        /// </list></exception>
        /// <exception cref="ArgumentException">If <paramref name="length"/> is not positive.</exception>
        public virtual Vector<T> SubVectorI(int IIndex, int length)
        {
            int index = IIndex - 1; //convert to 0 based indexing and carry on as per 0 based indexing method SubVector.
            return SubVector(index, length);
        }



        /// <summary>
        /// Updates specified elements of the Vector. Uses 1 based indexing.
        /// </summary>
        /// <param name="index">The first element to begin copying from, using 1 based indexing.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <exception cref="ArgumentOutOfRangeException"><list><item>If <paramref name="index"/> is not positive or
        /// greater than or equal to the size of the vector.</item>
        /// <item>If <paramref name="index"/> + <paramref name="length"/> is greater than the size of the vector.</item>
        /// </list></exception>
        /// <exception cref="ArgumentException">If <paramref name="length"/> is not positive.</exception>
        public virtual void SetSubVectorI(int IIndex, int length, Vector<T> subVector)
        {
            SetSubVector(IIndex - 1, length, subVector);
        }

        /// <summary>
        /// Updates specified elements of the Vector with the other vector's elements.
        /// </summary>
        /// <param name="index">The first element to begin copying from, using 1 based indexing.</param>
        /// <param name="vectorToCopyFrom">The vector to copy from .</param>
        /// <exception cref="ArgumentOutOfRangeException"><list><item>If <paramref name="index"/> is not positive or
        /// greater than or equal to the size of the vector.</item>
        /// <item>If <paramref name="index"/> + <paramref name="vectorToCopyFrom"/> 'size  is greater than the size of the vector.</item>
        public virtual void SetSubVectorI(int IIndex, Vector<T> vectorToCopyFrom)
        {
            SetSubVector(IIndex-1, vectorToCopyFrom.Count, vectorToCopyFrom);
        }


        /// <summary>
        /// same as MinimumIndex but using 1 based index.
        /// </summary>
        /// <returns></returns>
        public int MinimumIndexI()
        {
            int zeroBasedIndex= MinimumIndex();
            return zeroBasedIndex+1;
        }

        public int MaximumIndexI()
        {
            int zeroBasedIndex = MaximumIndex();
            return zeroBasedIndex+1;
        }

        public int AbsoluteMinimumIndexI()
        {
            int zeroBasedIndex = AbsoluteMinimumIndex();
            return zeroBasedIndex+1;
        }
        
        public  int AbsoluteMaximumIndexI()
        {
            int zeroBasedIndex = AbsoluteMaximumIndex();
            return zeroBasedIndex+1;
        }

        /// <summary>
        /// returns the element of the vector using 1 based Index. No range checking.
        /// </summary>
        /// <param name="IIndex"> the IIndex using 1 based indexing</param>
        /// <returns> the element at that position.</returns>
        public T AtI(int IIndex)
        {
            return At(IIndex - 1);
        }

        /// <summary>
        /// Sets the element of the vector using 1 based Index. No range checking.
        /// </summary>
        /// <param name="IIndex"> the IIndex using 1 based indexing</param>
        /// <param name="value"> the value to which to se the element to </param>
        public void AtI(int IIndex, T value)
        {
            At(IIndex - 1, value);
        }


        /// <summary>
        /// Tests  all the elements of a given Vector  for a conditon and returns an <see cref="IEnumerable{T}"/> of indeces (1 based) where this conditions is true.  
        /// The condition is given using System.Predicate;
        /// </summary>
        /// <param name="matchCondition">System.Predicate. The condition that the Vector elements are tested for.</param>
        /// <returns> The resultant <see cref="IEnumerable{T}"/> of ints where the Predicate is true , using  1 based indexing.</returns>
        /// <exception cref="ArgumentNullException">If matchCondition is <see langword="null" />.</exception>
        public virtual IEnumerable<int> FindIndicesI(Predicate<T> matchCondition)
        {
            if (matchCondition == null) throw new ArgumentNullException("matchCondition");

            for (var index = 0; index < Count; index++)
            {
                if (matchCondition(At(index)))
                {
                    yield return index+1;
                }
            }

            yield break;
        }


        /// <summary>
        /// For each int in an enumerable of ints set all the elements of the given Vector to a specified value.
        /// Uses 1 based indexing.
        /// </summary>
        /// <param name="indxs">The enumerables of ints, 1 based indexing</param>
        /// <param name="value">The value to set the Elements to.</param>
        /// <exception cref="ArgumentNullException">If indxs is <see langword="null" />.</exception>
        public virtual void SetOnIndicesI(IEnumerable<int> indxs, T value)
        {
            if (indxs == null) throw new ArgumentNullException("indxs");

            foreach (int i in indxs)
            {
                this[i-1] = value;
            }
        }

        /// <summary>
        /// For each int in an enumerable of ints applies a function to the given Vector and sets the elements to the outcome of the function.
        /// Uses 1 based indexing.
        /// </summary>
        /// <param name="indxs">The enumerables of ints, 1 based indexing</param>
        /// <param name="fun"> The function </param>
        /// <exception cref="ArgumentNullException">If indxs is <see langword="null" />.</exception>
        public virtual void ApplyOnIndicesI(IEnumerable<int> indxs, Func<T, T> fun)
        {
            if (indxs == null) throw new ArgumentNullException("indxs");

            foreach (int i in indxs)
            {
                this[i-1] = fun(this[i-1]);
            }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator{int}"/> that enumerates over the vectors  indices using 1 based counting. 1..Count
        /// </summary>
        /// <returns>An <see cref="IEnumerator{int}"/> that enumerates over the vectors indices, using 1 based counting. 1...Count.</returns>
        public IEnumerable<int> IndicesI()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return i+1;
            }
            yield break;
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> that contains the position (using 1 based indexing) and value of the element.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator{T}"/> over this vector that contains the position and value of each
        /// element.
        /// </returns>
        /// <remarks>
        /// The enumerator returns a 
        /// <seealso cref="Tuple{T,K}"/>
        /// with the first value being the element index and the second value 
        /// being the value of the element at that index. For sparse vectors, the enumerator will exclude all elements
        /// with a zero value. 
        /// </remarks>
        public virtual IEnumerable<Tuple<int, T>> GetIndexedEnumeratorI()
        {
            for (var i = 0; i < Count; i++)
            {
                yield return new Tuple<int, T>(i+1, this[i]);
            }
        }



        public virtual Vector<T> SelectElements(IEnumerable<int> keep)
        {
            var keepList = keep.ToList();
            var returnv = CreateVector(keepList.Count);
            
            int target = 0;
            foreach (int c in keepList)
            {
                if (c >= Count || c < 0)
                {
                    throw new IndexOutOfRangeException("Vector.SelectElements index out of range.");
                }
                returnv.At(target,this.At(c));
                target++;
            }
            return returnv;
        }

        public virtual Vector<T> SelectElementsI(IEnumerable<int> keepI)
        {
            return SelectElements(from elem in keepI select elem - 1);
        }


  }
}
