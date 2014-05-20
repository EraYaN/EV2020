﻿// <copyright file="Barycentric.cs" company="Math.NET">
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

using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Properties;

namespace MathNet.Numerics.Interpolation
{
    /// <summary>
    /// Barycentric Interpolation Algorithm.
    /// </summary>
    /// <remarks>Supports neither differentiation nor integration.</remarks>
    public class Barycentric : IInterpolation
    {
        readonly double[] _x;
        readonly double[] _y;
        readonly double[] _w;

        /// <param name="x">Sample points (N), no sorting assumed.</param>
        /// <param name="y">Sample values (N).</param>
        /// <param name="w">Barycentric weights (N).</param>
        public Barycentric(double[] x, double[] y, double[] w)
        {
            if (x.Length != y.Length || x.Length != w.Length)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength);
            }

            if (x.Length < 1)
            {
                throw new ArgumentOutOfRangeException("x");
            }

            _x = x;
            _y = y;
            _w = w;
        }

        /// <summary>
        /// Create a barycentric polynomial interpolation from a set of (x,y) value pairs with equidistant x. No sorting is assumed.
        /// </summary>
        /// <remarks>
        /// The value pairs do not have to be sorted, but if they are not sorted ascendingly
        /// and the passed x and y arguments are arrays, they will be sorted inplace and thus modified.
        /// </remarks>
        public static Barycentric InterpolatePolynomialEquidistant(IEnumerable<double> x, IEnumerable<double> y)
        {
            var xx = (x as double[]) ?? x.ToArray();
            var yy = (y as double[]) ?? y.ToArray();

            if (xx.Length != yy.Length)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength);
            }

            if (xx.Length < 1)
            {
                throw new ArgumentOutOfRangeException("x");
            }

            Sorting.Sort(xx, yy);

            var weights = new double[xx.Length];
            weights[0] = 1.0;
            for (int i = 1; i < weights.Length; i++)
            {
                weights[i] = -(weights[i - 1]*(weights.Length - i))/i;
            }

            return new Barycentric(xx, yy, weights);
        }

        /// <summary>
        /// Create a barycentric polynomial interpolation from a set of values related to linearly/equidistant spaced points within an interval.
        /// </summary>
        /// <remarks>
        /// The value pairs do not have to be sorted, but if they are not sorted ascendingly
        /// and the passed x and y arguments are arrays, they will be sorted inplace and thus modified.
        /// </remarks>
        public static Barycentric InterpolatePolynomialEquidistant(double leftBound, double rightBound, IEnumerable<double> y)
        {
            var yy = (y as double[]) ?? y.ToArray();
            var xx = Generate.LinearSpaced(yy.Length, leftBound, rightBound);
            return InterpolatePolynomialEquidistant(xx, yy);
        }

        /// <summary>
        /// Create a barycentric rational interpolation without poles, using Mike Floater and Kai Hormann's Algorithm.
        /// </summary>
        /// <param name="x">Sample points (N), no sorting assumed. Optimized for arrays.</param>
        /// <param name="y">Sample values (N). Optimized for arrays.</param>
        /// <param name="order">
        /// Order of the interpolation scheme, 0 &lt;= order &lt;= N.
        /// In most cases a value between 3 and 8 gives good results.
        /// </param>
        /// <remarks>
        /// The value pairs do not have to be sorted, but if they are not sorted ascendingly
        /// and the passed x and y arguments are arrays, they will be sorted inplace and thus modified.
        /// </remarks>
        public static Barycentric InterpolateRationalFloaterHormann(IEnumerable<double> x, IEnumerable<double> y, int order)
        {
            var xx = (x as double[]) ?? x.ToArray();
            var yy = (y as double[]) ?? y.ToArray();

            if (xx.Length != yy.Length)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength);
            }

            if (0 > order || xx.Length <= order)
            {
                throw new ArgumentOutOfRangeException("order");
            }

            Sorting.Sort(xx, yy);

            var weights = new double[xx.Length];

            // order: odd -> negative, even -> positive
            double sign = ((order & 0x1) == 0x1) ? -1.0 : 1.0;

            // compute barycentric weights
            for (int k = 0; k < xx.Length; k++)
            {
                double s = 0;
                for (int i = Math.Max(k - order, 0); i <= Math.Min(k, weights.Length - 1 - order); i++)
                {
                    double v = 1;
                    for (int j = i; j <= i + order; j++)
                    {
                        if (j != k)
                        {
                            v = v/Math.Abs(xx[k] - xx[j]);
                        }
                    }

                    s = s + v;
                }

                weights[k] = sign*s;
                sign = -sign;
            }

            return new Barycentric(xx, yy, weights);
        }

        /// <summary>
        /// Create a barycentric rational interpolation without poles, using Mike Floater and Kai Hormann's Algorithm.
        /// </summary>
        /// <param name="x">Sample points (N), no sorting assumed. Optimized for arrays.</param>
        /// <param name="y">Sample values (N). Optimized for arrays.</param>
        /// <remarks>
        /// The value pairs do not have to be sorted, but if they are not sorted ascendingly
        /// and the passed x and y arguments are arrays, they will be sorted inplace and thus modified.
        /// </remarks>
        public static Barycentric InterpolateRationalFloaterHormann(IEnumerable<double> x, IEnumerable<double> y)
        {
            var xx = (x as double[]) ?? x.ToArray();
            var order = Math.Min(3, xx.Length - 1);
            return InterpolateRationalFloaterHormann(xx, y, order);
        }

        /// <summary>
        /// Gets a value indicating whether the algorithm supports differentiation (interpolated derivative).
        /// </summary>
        bool IInterpolation.SupportsDifferentiation
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the algorithm supports integration (interpolated quadrature).
        /// </summary>
        bool IInterpolation.SupportsIntegration
        {
            get { return false; }
        }

        public double Interpolate(double t)
        {
            // trivial case: only one sample?
            if (_x.Length == 1)
            {
                return _y[0];
            }

            // evaluate closest point and offset from that point (no sorting assumed)
            int closestPoint = 0;
            double offset = t - _x[0];
            for (int i = 1; i < _x.Length; i++)
            {
                if (Math.Abs(t - _x[i]) < Math.Abs(offset))
                {
                    offset = t - _x[i];
                    closestPoint = i;
                }
            }

            // trivial case: on a known sample point?
            if (offset == 0.0)
            {
                // NOTE (cdrnet, 2009-08) not offset.AlmostZero() by design
                return _y[closestPoint];
            }

            if (Math.Abs(offset) > 1e-150)
            {
                // no need to guard against overflow, so use fast formula
                closestPoint = -1;
                offset = 1.0;
            }

            double s1 = 0.0;
            double s2 = 0.0;
            for (int i = 0; i < _x.Length; i++)
            {
                if (i != closestPoint)
                {
                    double v = offset*_w[i]/(t - _x[i]);
                    s1 = s1 + (v*_y[i]);
                    s2 = s2 + v;
                }
                else
                {
                    double v = _w[i];
                    s1 = s1 + (v*_y[i]);
                    s2 = s2 + v;
                }
            }

            return s1/s2;
        }

        /// <summary>
        /// Differentiate at point t. NOT SUPPORTED.
        /// </summary>
        /// <param name="t">Point t to interpolate at.</param>
        /// <returns>Interpolated first derivative at point t.</returns>
        double IInterpolation.Differentiate(double t)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Differentiate twice at point t. NOT SUPPORTED.
        /// </summary>
        /// <param name="t">Point t to interpolate at.</param>
        /// <returns>Interpolated second derivative at point t.</returns>
        double IInterpolation.Differentiate2(double t)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Indefinite integral at point t. NOT SUPPORTED.
        /// </summary>
        /// <param name="t">Point t to integrate at.</param>
        double IInterpolation.Integrate(double t)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Definite integral between points a and b. NOT SUPPORTED.
        /// </summary>
        /// <param name="a">Left bound of the integration interval [a,b].</param>
        /// <param name="b">Right bound of the integration interval [a,b].</param>
        double IInterpolation.Integrate(double a, double b)
        {
            throw new NotSupportedException();
        }
    }
}
