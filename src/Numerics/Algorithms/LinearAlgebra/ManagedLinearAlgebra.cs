﻿// <copyright file="ManagedLinearAlgebra.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
// Copyright (c) 2009 Math.NET
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
using System;
using MathNet.Numerics.Threading;

namespace MathNet.Numerics.Algorithms.LinearAlgebra
{
    /// <summary>
    /// The managed linear algebra provider.
    /// </summary>
    internal class ManagedLinearAlgebra : ILinearAlgebra
    {
        /// <summary>
        /// Adds a scaled vector to another: <c>y += alpha*x</c>.
        /// </summary>
        /// <param name="y">The vector to update.</param>
        /// <param name="alpha">The value to scale <param name="x"/> by.</param>
        /// <param name="x">The vector to add to <paramref name="y"/>.</param>
        /// <remarks>This equivalent to the AXPY BLAS routine.</remarks>
        public void AddVectorToScaledVector(double[] y, double alpha, double[] x)
        {
            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (y.Length != x.Length)
            {
                throw new ArgumentException(Properties.Resources.ArgumentVectorsSameLength);
            }

            if (alpha.AlmostZero())
            {
                return;
            }

            if (alpha.AlmostEqual(1.0))
            {
                Parallel.For(0, y.Length, i => y[i] += x[i]);
            }
            else
            {
                Parallel.For(0, y.Length, i => y[i] += alpha * x[i]);
            }
        }

        public void ScaleArray(double alpha, double[] x)
        {
            if (alpha.AlmostEqual(1.0))
            {
                return;
            }
            Parallel.For(0, x.Length, i => x[i] = alpha * x[i]);
        }
    }
}