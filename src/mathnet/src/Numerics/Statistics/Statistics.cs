﻿// <copyright file="Statistics.cs" company="Math.NET">
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

namespace MathNet.Numerics.Statistics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods to return basic statistics on set of data.
    /// </summary>
    public static class Statistics
    {
        /// <summary>
        /// Returns the minimum value in the sample data.
        /// Returns NaN if data is empty or if any entry is NaN.
        /// </summary>
        /// <param name="data">The sample data.</param>
        /// <returns>The minimum value in the sample data.</returns>
        public static double Minimum(this IEnumerable<double> data)
        {
            var array = data as double[];
            return array != null
                ? ArrayStatistics.Minimum(array)
                : StreamingStatistics.Minimum(data);
        }
        /// <summary>
        /// Returns the minimum value in the sample data.
        /// Returns NaN if data is empty or if any entry is NaN.
        /// Null-entries are ignored.
        /// </summary>
        /// <param name="data">The sample data.</param>
        /// <returns>The minimum value in the sample data.</returns>
        public static double Minimum(this IEnumerable<double?> data)
        {
            return StreamingStatistics.Minimum(data.Where(d => d.HasValue).Select(d => d.Value));
        }

        /// <summary>
        /// Returns the maximum value in the sample data.
        /// Returns NaN if data is empty or if any entry is NaN.
        /// </summary>
        /// <param name="data">The sample data.</param>
        /// <returns>The maximum value in the sample data.</returns>
        public static double Maximum(this IEnumerable<double> data)
        {
            var array = data as double[];
            return array != null
                ? ArrayStatistics.Maximum(array)
                : StreamingStatistics.Maximum(data);
        }

        /// <summary>
        /// Returns the maximum value in the sample data.
        /// Returns NaN if data is empty or if any entry is NaN.
        /// Null-entries are ignored.
        /// </summary>
        /// <param name="data">The sample data.</param>
        /// <returns>The maximum value in the sample data.</returns>
        public static double Maximum(this IEnumerable<double?> data)
        {
            return StreamingStatistics.Maximum(data.Where(d => d.HasValue).Select(d => d.Value));
        }

        /// <summary>
        /// Evaluates the sample mean, an estimate of the population mean.
        /// Returns NaN if data is empty or if any entry is NaN.
        /// </summary>
        /// <param name="data">The data to calculate the mean of.</param>
        /// <returns>The mean of the sample.</returns>
        public static double Mean(this IEnumerable<double> data)
        {
            var array = data as double[];
            return array != null
                ? ArrayStatistics.Mean(array)
                : StreamingStatistics.Mean(data);
        }

        /// <summary>
        /// Evaluates the sample mean, an estimate of the population mean.
        /// Returns NaN if data is empty or if any entry is NaN.
        /// Null-entries are ignored.
        /// </summary>
        /// <param name="data">The data to calculate the mean of.</param>
        /// <returns>The mean of the sample.</returns>
        public static double Mean(this IEnumerable<double?> data)
        {
            return StreamingStatistics.Mean(data.Where(d => d.HasValue).Select(d => d.Value));
        }

        /// <summary>
        /// Estimates the unbiased population variance from the provided samples.
        /// On a dataset of size N will use an N-1 normalizer (Bessel's correction).
        /// Returns NaN if data has less than two entries or if any entry is NaN.
        /// </summary>
        /// <param name="samples">A subset of samples, sampled from the full population.</param>
        public static double Variance(this IEnumerable<double> samples)
        {
            var array = samples as double[];
            return array != null
                ? ArrayStatistics.Variance(array)
                : StreamingStatistics.Variance(samples);
        }

        /// <summary>
        /// Estimates the unbiased population variance from the provided samples.
        /// On a dataset of size N will use an N-1 normalizer (Bessel's correction).
        /// Returns NaN if data has less than two entries or if any entry is NaN.
        /// Null-entries are ignored.
        /// </summary>
        /// <param name="samples">A subset of samples, sampled from the full population.</param>
        public static double Variance(this IEnumerable<double?> samples)
        {
            return StreamingStatistics.Variance(samples.Where(d => d.HasValue).Select(d => d.Value));
        }

        /// <summary>
        /// Evaluates the variance from the provided full population.
        /// On a dataset of size N will use an N normalizer and would thus be biased if applied to a subset.
        /// Returns NaN if data is empty or if any entry is NaN.
        /// </summary>
        /// <param name="population">The full population data.</param>
        public static double PopulationVariance(this IEnumerable<double> population)
        {
            var array = population as double[];
            return array != null
                ? ArrayStatistics.PopulationVariance(array)
                : StreamingStatistics.PopulationVariance(population);
        }

        /// <summary>
        /// Evaluates the variance from the provided full population.
        /// On a dataset of size N will use an N normalize and would thus be biased if applied to a subsetr.
        /// Returns NaN if data is empty or if any entry is NaN.
        /// Null-entries are ignored.
        /// </summary>
        /// <param name="population">The full population data.</param>
        public static double PopulationVariance(this IEnumerable<double?> population)
        {
            return StreamingStatistics.PopulationVariance(population.Where(d => d.HasValue).Select(d => d.Value));
        }

        /// <summary>
        /// Estimates the unbiased population standard deviation from the provided samples.
        /// On a dataset of size N will use an N-1 normalizer (Bessel's correction).
        /// Returns NaN if data has less than two entries or if any entry is NaN.
        /// </summary>
        /// <param name="samples">A subset of samples, sampled from the full population.</param>
        public static double StandardDeviation(this IEnumerable<double> samples)
        {
            var array = samples as double[];
            return array != null
                ? ArrayStatistics.StandardDeviation(array)
                : StreamingStatistics.StandardDeviation(samples);
        }

        /// <summary>
        /// Estimates the unbiased population standard deviation from the provided samples.
        /// On a dataset of size N will use an N-1 normalizer (Bessel's correction).
        /// Returns NaN if data has less than two entries or if any entry is NaN.
        /// Null-entries are ignored.
        /// </summary>
        /// <param name="samples">A subset of samples, sampled from the full population.</param>
        public static double StandardDeviation(this IEnumerable<double?> samples)
        {
            return StreamingStatistics.StandardDeviation(samples.Where(d => d.HasValue).Select(d => d.Value));
        }

        /// <summary>
        /// Evaluates the standard deviation from the provided full population.
        /// On a dataset of size N will use an N normalizer and would thus be biased if applied to a subset.
        /// Returns NaN if data is empty or if any entry is NaN.
        /// </summary>
        /// <param name="population">The full population data.</param>
        public static double PopulationStandardDeviation(this IEnumerable<double> population)
        {
            var array = population as double[];
            return array != null
                ? ArrayStatistics.PopulationStandardDeviation(array)
                : StreamingStatistics.PopulationStandardDeviation(population);
        }

        /// <summary>
        /// Evaluates the standard deviation from the provided full population.
        /// On a dataset of size N will use an N normalizer and would thus be biased if applied to a subset.
        /// Returns NaN if data is empty or if any entry is NaN.
        /// Null-entries are ignored.
        /// </summary>
        /// <param name="population">The full population data.</param>
        public static double PopulationStandardDeviation(this IEnumerable<double?> population)
        {
            return StreamingStatistics.PopulationStandardDeviation(population.Where(d => d.HasValue).Select(d => d.Value));
        }

        /// <summary>
        /// Estimates the unbiased population skewness from the provided samples.
        /// Uses a normalizer (Bessel's correction; type 2).
        /// Returns NaN if data has less than three entries or if any entry is NaN.
        /// </summary>
        /// <param name="samples">A subset of samples, sampled from the full population.</param>
        public static double Skewness(this IEnumerable<double> samples)
        {
            return new RunningStatistics(samples).Skewness;
        }

        /// <summary>
        /// Estimates the unbiased population skewness from the provided samples.
        /// Uses a normalizer (Bessel's correction; type 2).
        /// Returns NaN if data has less than three entries or if any entry is NaN.
        /// Null-entries are ignored.
        /// </summary>
        /// <param name="samples">A subset of samples, sampled from the full population.</param>
        public static double Skewness(this IEnumerable<double?> samples)
        {
            return new RunningStatistics(samples.Where(d => d.HasValue).Select(d => d.Value)).Skewness;
        }

        /// <summary>
        /// Evaluates the skewness from the full population.
        /// Does not use a normalizer and would thus be biased if applied to a subset (type 1).
        /// Returns NaN if data has less than two entries or if any entry is NaN.
        /// </summary>
        /// <param name="population">The full population data.</param>
        public static double PopulationSkewness(this IEnumerable<double> population)
        {
            return new RunningStatistics(population).PopulationSkewness;
        }

        /// <summary>
        /// Evaluates the skewness from the full population.
        /// Does not use a normalizer and would thus be biased if applied to a subset (type 1).
        /// Returns NaN if data has less than two entries or if any entry is NaN.
        /// Null-entries are ignored.
        /// </summary>
        /// <param name="population">The full population data.</param>
        public static double PopulationSkewness(this IEnumerable<double?> population)
        {
            return new RunningStatistics(population.Where(d => d.HasValue).Select(d => d.Value)).PopulationSkewness;
        }

        /// <summary>
        /// Estimates the unbiased population kurtosis from the provided samples.
        /// Uses a normalizer (Bessel's correction; type 2).
        /// Returns NaN if data has less than four entries or if any entry is NaN.
        /// </summary>
        /// <param name="samples">A subset of samples, sampled from the full population.</param>
        public static double Kurtosis(this IEnumerable<double> samples)
        {
            return new RunningStatistics(samples).Kurtosis;
        }

        /// <summary>
        /// Estimates the unbiased population kurtosis from the provided samples.
        /// Uses a normalizer (Bessel's correction; type 2).
        /// Returns NaN if data has less than four entries or if any entry is NaN.
        /// Null-entries are ignored.
        /// </summary>
        /// <param name="samples">A subset of samples, sampled from the full population.</param>
        public static double Kurtosis(this IEnumerable<double?> samples)
        {
            return new RunningStatistics(samples.Where(d => d.HasValue).Select(d => d.Value)).Kurtosis;
        }

        /// <summary>
        /// Evaluates the kurtosis from the full population.
        /// Does not use a normalizer and would thus be biased if applied to a subset (type 1).
        /// Returns NaN if data has less than three entries or if any entry is NaN.
        /// </summary>
        /// <param name="population">The full population data.</param>
        public static double PopulationKurtosis(this IEnumerable<double> population)
        {
            return new RunningStatistics(population).PopulationKurtosis;
        }

        /// <summary>
        /// Evaluates the kurtosis from the full population.
        /// Does not use a normalizer and would thus be biased if applied to a subset (type 1).
        /// Returns NaN if data has less than three entries or if any entry is NaN.
        /// Null-entries are ignored.
        /// </summary>
        /// <param name="population">The full population data.</param>
        public static double PopulationKurtosis(this IEnumerable<double?> population)
        {
            return new RunningStatistics(population.Where(d => d.HasValue).Select(d => d.Value)).PopulationKurtosis;
        }

        /// <summary>
        /// Estimates the sample mean and the unbiased population variance from the provided samples.
        /// On a dataset of size N will use an N-1 normalizer (Bessel's correction).
        /// Returns NaN for mean if data is empty or if any entry is NaN and NaN for variance if data has less than two entries or if any entry is NaN.
        /// </summary>
        /// <param name="samples">The data to calculate the mean of.</param>
        /// <returns>The mean of the sample.</returns>
        public static Tuple<double, double> MeanVariance(this IEnumerable<double> samples)
        {
            var array = samples as double[];
            return array != null
                ? ArrayStatistics.MeanVariance(array)
                : StreamingStatistics.MeanVariance(samples);
        }

        /// <summary>
        /// Estimates the sample mean and the unbiased population standard deviation from the provided samples.
        /// On a dataset of size N will use an N-1 normalizer (Bessel's correction).
        /// Returns NaN for mean if data is empty or if any entry is NaN and NaN for standard deviation if data has less than two entries or if any entry is NaN.
        /// </summary>
        /// <param name="samples">The data to calculate the mean of.</param>
        /// <returns>The mean of the sample.</returns>
        public static Tuple<double, double> MeanStandardDeviation(this IEnumerable<double> samples)
        {
            var array = samples as double[];
            return array != null
                ? ArrayStatistics.MeanStandardDeviation(array)
                : StreamingStatistics.MeanStandardDeviation(samples);
        }

        /// <summary>
        /// Estimates the unbiased population skewness and kurtosis from the provided samples in a single pass.
        /// Uses a normalizer (Bessel's correction; type 2).
        /// </summary>
        /// <param name="samples">A subset of samples, sampled from the full population.</param>
        public static Tuple<double, double> SkewnessKurtosis(this IEnumerable<double> samples)
        {
            var stats = new RunningStatistics(samples);
            return new Tuple<double, double>(stats.Skewness, stats.Kurtosis);
        }

        /// <summary>
        /// Evaluates the skewness and kurtosis from the full population.
        /// Does not use a normalizer and would thus be biased if applied to a subset (type 1).
        /// </summary>
        /// <param name="population">The full population data.</param>
        public static Tuple<double, double> PopulationSkewnessKurtosis(this IEnumerable<double> population)
        {
            var stats = new RunningStatistics(population);
            return new Tuple<double, double>(stats.PopulationSkewness, stats.PopulationKurtosis);
        }

        /// <summary>
        /// Estimates the unbiased population covariance from the provided samples.
        /// On a dataset of size N will use an N-1 normalizer (Bessel's correction).
        /// Returns NaN if data has less than two entries or if any entry is NaN.
        /// </summary>
        /// <param name="samples1">A subset of samples, sampled from the full population.</param>
        /// <param name="samples2">A subset of samples, sampled from the full population.</param>
        public static double Covariance(this IEnumerable<double> samples1, IEnumerable<double> samples2)
        {
            var array1 = samples1 as double[];
            var array2 = samples2 as double[];
            return array1 != null && array2 != null
                ? ArrayStatistics.Covariance(array1, array2)
                : StreamingStatistics.Covariance(samples1, samples2);
        }

        /// <summary>
        /// Estimates the unbiased population covariance from the provided samples.
        /// On a dataset of size N will use an N-1 normalizer (Bessel's correction).
        /// Returns NaN if data has less than two entries or if any entry is NaN.
        /// Null-entries are ignored.
        /// </summary>
        /// <param name="samples1">A subset of samples, sampled from the full population.</param>
        /// <param name="samples2">A subset of samples, sampled from the full population.</param>
        public static double Covariance(this IEnumerable<double?> samples1, IEnumerable<double?> samples2)
        {
            return StreamingStatistics.Covariance(samples1.Where(d => d.HasValue).Select(d => d.Value), samples2.Where(d => d.HasValue).Select(d => d.Value));
        }

        /// <summary>
        /// Evaluates the population covariance from the provided full populations.
        /// On a dataset of size N will use an N normalizer and would thus be biased if applied to a subset.
        /// Returns NaN if data is empty or if any entry is NaN.
        /// </summary>
        /// <param name="population1">The full population data.</param>
        /// <param name="population2">The full population data.</param>
        public static double PopulationCovariance(this IEnumerable<double> population1, IEnumerable<double> population2)
        {
            var array1 = population1 as double[];
            var array2 = population2 as double[];
            return array1 != null && array2 != null
                ? ArrayStatistics.PopulationCovariance(array1, array2)
                : StreamingStatistics.PopulationCovariance(population1, population2);
        }

        /// <summary>
        /// Evaluates the population covariance from the provided full populations.
        /// On a dataset of size N will use an N normalize and would thus be biased if applied to a subset.
        /// Returns NaN if data is empty or if any entry is NaN.
        /// Null-entries are ignored.
        /// </summary>
        /// <param name="population1">The full population data.</param>
        /// <param name="population2">The full population data.</param>
        public static double PopulationCovariance(this IEnumerable<double?> population1, IEnumerable<double?> population2)
        {
            return StreamingStatistics.PopulationCovariance(population1.Where(d => d.HasValue).Select(d => d.Value), population2.Where(d => d.HasValue).Select(d => d.Value));
        }

        /// <summary>
        /// Estimates the sample median from the provided samples (R8).
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        public static double Median(this IEnumerable<double> data)
        {
            var array = data.ToArray();
            return ArrayStatistics.MedianInplace(array);
        }

        /// <summary>
        /// Estimates the sample median from the provided samples (R8).
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        public static double Median(this IEnumerable<double?> data)
        {
            var array = data.Where(d => d.HasValue).Select(d => d.Value).ToArray();
            return ArrayStatistics.MedianInplace(array);
        }

        /// <summary>
        /// Estimates the tau-th quantile from the provided samples.
        /// The tau-th quantile is the data value where the cumulative distribution
        /// function crosses tau.
        /// Approximately median-unbiased regardless of the sample distribution (R8).
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        /// <param name="tau">Quantile selector, between 0.0 and 1.0 (inclusive).</param>
        public static double Quantile(this IEnumerable<double> data, double tau)
        {
            var array = data.ToArray();
            return ArrayStatistics.QuantileInplace(array, tau);
        }

        /// <summary>
        /// Estimates the tau-th quantile from the provided samples.
        /// The tau-th quantile is the data value where the cumulative distribution
        /// function crosses tau.
        /// Approximately median-unbiased regardless of the sample distribution (R8).
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        /// <param name="tau">Quantile selector, between 0.0 and 1.0 (inclusive).</param>
        public static double Quantile(this IEnumerable<double?> data, double tau)
        {
            var array = data.Where(d => d.HasValue).Select(d => d.Value).ToArray();
            return ArrayStatistics.QuantileInplace(array, tau);
        }

        /// <summary>
        /// Estimates the tau-th quantile from the provided samples.
        /// The tau-th quantile is the data value where the cumulative distribution
        /// function crosses tau.
        /// Approximately median-unbiased regardless of the sample distribution (R8).
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        public static Func<double,double> QuantileFunc(this IEnumerable<double> data)
        {
            var array = data.ToArray();
            Array.Sort(array);
            return tau => SortedArrayStatistics.Quantile(array, tau);
        }

        /// <summary>
        /// Estimates the tau-th quantile from the provided samples.
        /// The tau-th quantile is the data value where the cumulative distribution
        /// function crosses tau.
        /// Approximately median-unbiased regardless of the sample distribution (R8).
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        public static Func<double, double> QuantileFunc(this IEnumerable<double?> data)
        {
            var array = data.Where(d => d.HasValue).Select(d => d.Value).ToArray();
            Array.Sort(array);
            return tau => SortedArrayStatistics.Quantile(array, tau);
        }

        /// <summary>
        /// Estimates the tau-th quantile from the provided samples.
        /// The tau-th quantile is the data value where the cumulative distribution
        /// function crosses tau. The quantile definition can be specificed to be compatible
        /// with an existing system.
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        /// <param name="tau">Quantile selector, between 0.0 and 1.0 (inclusive).</param>
        /// <param name="definition">Quantile definition, to choose what product/definition it should be consistent with</param>
        public static double QuantileCustom(this IEnumerable<double> data, double tau, QuantileDefinition definition)
        {
            var array = data.ToArray();
            return ArrayStatistics.QuantileCustomInplace(array, tau, definition);
        }

        /// <summary>
        /// Estimates the tau-th quantile from the provided samples.
        /// The tau-th quantile is the data value where the cumulative distribution
        /// function crosses tau. The quantile definition can be specificed to be compatible
        /// with an existing system.
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        /// <param name="tau">Quantile selector, between 0.0 and 1.0 (inclusive).</param>
        /// <param name="definition">Quantile definition, to choose what product/definition it should be consistent with</param>
        public static double QuantileCustom(this IEnumerable<double?> data, double tau, QuantileDefinition definition)
        {
            var array = data.Where(d => d.HasValue).Select(d => d.Value).ToArray();
            return ArrayStatistics.QuantileCustomInplace(array, tau, definition);
        }

        /// <summary>
        /// Estimates the tau-th quantile from the provided samples.
        /// The tau-th quantile is the data value where the cumulative distribution
        /// function crosses tau. The quantile definition can be specificed to be compatible
        /// with an existing system.
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        /// <param name="definition">Quantile definition, to choose what product/definition it should be consistent with</param>
        public static Func<double, double> QuantileCustomFunc(this IEnumerable<double> data, QuantileDefinition definition)
        {
            var array = data.ToArray();
            Array.Sort(array);
            return tau => SortedArrayStatistics.QuantileCustom(array, tau, definition);
        }

        /// <summary>
        /// Estimates the tau-th quantile from the provided samples.
        /// The tau-th quantile is the data value where the cumulative distribution
        /// function crosses tau. The quantile definition can be specificed to be compatible
        /// with an existing system.
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        /// <param name="definition">Quantile definition, to choose what product/definition it should be consistent with</param>
        public static Func<double, double> QuantileCustomFunc(this IEnumerable<double?> data, QuantileDefinition definition)
        {
            var array = data.Where(d => d.HasValue).Select(d => d.Value).ToArray();
            Array.Sort(array);
            return tau => SortedArrayStatistics.QuantileCustom(array, tau, definition);
        }

        /// <summary>
        /// Estimates the p-Percentile value from the provided samples.
        /// If a non-integer Percentile is needed, use Quantile instead.
        /// Approximately median-unbiased regardless of the sample distribution (R8).
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        /// <param name="p">Percentile selector, between 0 and 100 (inclusive).</param>
        public static double Percentile(this IEnumerable<double> data, int p)
        {
            var array = data.ToArray();
            return ArrayStatistics.PercentileInplace(array, p);
        }

        /// <summary>
        /// Estimates the p-Percentile value from the provided samples.
        /// If a non-integer Percentile is needed, use Quantile instead.
        /// Approximately median-unbiased regardless of the sample distribution (R8).
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        /// <param name="p">Percentile selector, between 0 and 100 (inclusive).</param>
        public static double Percentile(this IEnumerable<double?> data, int p)
        {
            var array = data.Where(d => d.HasValue).Select(d => d.Value).ToArray();
            return ArrayStatistics.PercentileInplace(array, p);
        }

        /// <summary>
        /// Estimates the p-Percentile value from the provided samples.
        /// If a non-integer Percentile is needed, use Quantile instead.
        /// Approximately median-unbiased regardless of the sample distribution (R8).
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        public static Func<int, double> PercentileFunc(this IEnumerable<double> data)
        {
            var array = data.ToArray();
            Array.Sort(array);
            return p => SortedArrayStatistics.Percentile(array, p);
        }

        /// <summary>
        /// Estimates the p-Percentile value from the provided samples.
        /// If a non-integer Percentile is needed, use Quantile instead.
        /// Approximately median-unbiased regardless of the sample distribution (R8).
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        public static Func<int, double> PercentileFunc(this IEnumerable<double?> data)
        {
            var array = data.Where(d => d.HasValue).Select(d => d.Value).ToArray();
            Array.Sort(array);
            return p => SortedArrayStatistics.Percentile(array, p);
        }

        /// <summary>
        /// Estimates the first quartile value from the provided samples.
        /// Approximately median-unbiased regardless of the sample distribution (R8).
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        public static double LowerQuartile(this IEnumerable<double> data)
        {
            var array = data.ToArray();
            return ArrayStatistics.LowerQuartileInplace(array);
        }

        /// <summary>
        /// Estimates the first quartile value from the provided samples.
        /// Approximately median-unbiased regardless of the sample distribution (R8).
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        public static double LowerQuartile(this IEnumerable<double?> data)
        {
            var array = data.Where(d => d.HasValue).Select(d => d.Value).ToArray();
            return ArrayStatistics.LowerQuartileInplace(array);
        }

        /// <summary>
        /// Estimates the third quartile value from the provided samples.
        /// Approximately median-unbiased regardless of the sample distribution (R8).
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        public static double UpperQuartile(this IEnumerable<double> data)
        {
            var array = data.ToArray();
            return ArrayStatistics.UpperQuartileInplace(array);
        }

        /// <summary>
        /// Estimates the third quartile value from the provided samples.
        /// Approximately median-unbiased regardless of the sample distribution (R8).
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        public static double UpperQuartile(this IEnumerable<double?> data)
        {
            var array = data.Where(d => d.HasValue).Select(d => d.Value).ToArray();
            return ArrayStatistics.UpperQuartileInplace(array);
        }

        /// <summary>
        /// Estimates the inter-quartile range from the provided samples.
        /// Approximately median-unbiased regardless of the sample distribution (R8).
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        public static double InterquartileRange(this IEnumerable<double> data)
        {
            var array = data.ToArray();
            return ArrayStatistics.InterquartileRangeInplace(array);
        }

        /// <summary>
        /// Estimates the inter-quartile range from the provided samples.
        /// Approximately median-unbiased regardless of the sample distribution (R8).
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        public static double InterquartileRange(this IEnumerable<double?> data)
        {
            var array = data.Where(d => d.HasValue).Select(d => d.Value).ToArray();
            return ArrayStatistics.InterquartileRangeInplace(array);
        }

        /// <summary>
        /// Estimates {min, lower-quantile, median, upper-quantile, max} from the provided samples.
        /// Approximately median-unbiased regardless of the sample distribution (R8).
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        public static double[] FiveNumberSummary(this IEnumerable<double> data)
        {
            var array = data.ToArray();
            return ArrayStatistics.FiveNumberSummaryInplace(array);
        }

        /// <summary>
        /// Estimates {min, lower-quantile, median, upper-quantile, max} from the provided samples.
        /// Approximately median-unbiased regardless of the sample distribution (R8).
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        public static double[] FiveNumberSummary(this IEnumerable<double?> data)
        {
            var array = data.Where(d => d.HasValue).Select(d => d.Value).ToArray();
            return ArrayStatistics.FiveNumberSummaryInplace(array);
        }

        /// <summary>
        /// Returns the order statistic (order 1..N) from the provided samples.
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        /// <param name="order">One-based order of the statistic, must be between 1 and N (inclusive).</param>
        public static double OrderStatistic(IEnumerable<double> data, int order)
        {
            var array = data.ToArray();
            return ArrayStatistics.OrderStatisticInplace(array, order);
        }

        /// <summary>
        /// Returns the order statistic (order 1..N) from the provided samples.
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        public static Func<int, double> OrderStatisticFunc(IEnumerable<double> data)
        {
            var array = data.ToArray();
            Array.Sort(array);
            return order => SortedArrayStatistics.OrderStatistic(array, order);
        }


        /// <summary>
        /// Evaluates the rank of each entry of the provided samples.
        /// The rank definition can be specificed to be compatible
        /// with an existing system.
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        /// <param name="definition">Rank definition, to choose how ties should be handled and what product/definition it should be consistent with</param>
        public static double[] Ranks(this IEnumerable<double> data, RankDefinition definition = RankDefinition.Default)
        {
            var array = data.ToArray();
            return ArrayStatistics.RanksInplace(array, definition);
        }

        /// <summary>
        /// Evaluates the rank of each entry of the provided samples.
        /// The rank definition can be specificed to be compatible
        /// with an existing system.
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        /// <param name="definition">Rank definition, to choose how ties should be handled and what product/definition it should be consistent with</param>
        public static double[] Ranks(this IEnumerable<double?> data, RankDefinition definition = RankDefinition.Default)
        {
            return Ranks(data.Where(d => d.HasValue).Select(d => d.Value), definition);
        }


        /// <summary>
        /// Estimates the quantile tau from the provided samples.
        /// The tau-th quantile is the data value where the cumulative distribution
        /// function crosses tau. The quantile definition can be specificed to be compatible
        /// with an existing system.
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        /// <param name="x">Quantile value.</param>
        /// <param name="definition">Rank definition, to choose how ties should be handled and what product/definition it should be consistent with</param>
        public static double QuantileRank(this IEnumerable<double> data, double x, RankDefinition definition = RankDefinition.Default)
        {
            var array = data.ToArray();
            Array.Sort(array);
            return SortedArrayStatistics.QuantileRank(array, x, definition);
        }

        /// <summary>
        /// Estimates the quantile tau from the provided samples.
        /// The tau-th quantile is the data value where the cumulative distribution
        /// function crosses tau. The quantile definition can be specificed to be compatible
        /// with an existing system.
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        /// <param name="x">Quantile value.</param>
        /// <param name="definition">Rank definition, to choose how ties should be handled and what product/definition it should be consistent with</param>
        public static double QuantileRank(this IEnumerable<double?> data, double x, RankDefinition definition = RankDefinition.Default)
        {
            return QuantileRank(data.Where(d => d.HasValue).Select(d => d.Value), x, definition);
        }

        /// <summary>
        /// Estimates the quantile tau from the provided samples.
        /// The tau-th quantile is the data value where the cumulative distribution
        /// function crosses tau. The quantile definition can be specificed to be compatible
        /// with an existing system.
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        /// <param name="definition">Rank definition, to choose how ties should be handled and what product/definition it should be consistent with</param>
        public static Func<double, double> QuantileRankFunc(this IEnumerable<double> data, RankDefinition definition = RankDefinition.Default)
        {
            var array = data.ToArray();
            Array.Sort(array);
            return x => SortedArrayStatistics.QuantileRank(array, x, definition);
        }

        /// <summary>
        /// Estimates the quantile tau from the provided samples.
        /// The tau-th quantile is the data value where the cumulative distribution
        /// function crosses tau. The quantile definition can be specificed to be compatible
        /// with an existing system.
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        /// <param name="definition">Rank definition, to choose how ties should be handled and what product/definition it should be consistent with</param>
        public static Func<double, double> QuantileRankFunc(this IEnumerable<double?> data, RankDefinition definition = RankDefinition.Default)
        {
            return QuantileRankFunc(data.Where(d => d.HasValue).Select(d => d.Value), definition);
        }


        /// <summary>
        /// Estimates the empirical cummulative distribution function (CDF) at x from the provided samples.
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        /// <param name="x">The value where to estimate the CDF at.</param>
        public static double EmpiricalCDF(this IEnumerable<double> data, double x)
        {
            var array = data.ToArray();
            Array.Sort(array);
            return SortedArrayStatistics.EmpiricalCDF(array, x);
        }

        /// <summary>
        /// Estimates the empirical cummulative distribution function (CDF) at x from the provided samples.
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        /// <param name="x">The value where to estimate the CDF at.</param>
        public static double EmpiricalCDF(this IEnumerable<double?> data, double x)
        {
            return EmpiricalCDF(data.Where(d => d.HasValue).Select(d => d.Value), x);
        }

        /// <summary>
        /// Estimates the empirical cummulative distribution function (CDF) at x from the provided samples.
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        public static Func<double, double> EmpiricalCDFFunc(this IEnumerable<double> data)
        {
            var array = data.ToArray();
            Array.Sort(array);
            return x => SortedArrayStatistics.EmpiricalCDF(array, x);
        }

        /// <summary>
        /// Estimates the empirical cummulative distribution function (CDF) at x from the provided samples.
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        public static Func<double, double> EmpiricalCDFFunc(this IEnumerable<double?> data)
        {
            return EmpiricalCDFFunc(data.Where(d => d.HasValue).Select(d => d.Value));
        }


        /// <summary>
        /// Estimates the empirical inverse CDF at tau from the provided samples.
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        /// <param name="tau">Quantile selector, between 0.0 and 1.0 (inclusive).</param>
        public static double EmpiricalInvCDF(this IEnumerable<double> data, double tau)
        {
            var array = data.ToArray();
            return ArrayStatistics.QuantileCustomInplace(array, tau, QuantileDefinition.EmpiricalInvCDF);
        }

        /// <summary>
        /// Estimates the empirical inverse CDF at tau from the provided samples.
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        /// <param name="tau">Quantile selector, between 0.0 and 1.0 (inclusive).</param>
        public static double EmpiricalInvCDF(this IEnumerable<double?> data, double tau)
        {
            return EmpiricalInvCDF(data.Where(d => d.HasValue).Select(d => d.Value), tau);
        }

        /// <summary>
        /// Estimates the empirical inverse CDF at tau from the provided samples.
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        public static Func<double, double> EmpiricalInvCDFFunc(this IEnumerable<double> data)
        {
            var array = data.ToArray();
            Array.Sort(array);
            return tau => SortedArrayStatistics.QuantileCustom(array, tau, QuantileDefinition.EmpiricalInvCDF);
        }

        /// <summary>
        /// Estimates the empirical inverse CDF at tau from the provided samples.
        /// </summary>
        /// <param name="data">The data sample sequence.</param>
        public static Func<double, double> EmpiricalInvCDFFunc(this IEnumerable<double?> data)
        {
            return EmpiricalInvCDFFunc(data.Where(d => d.HasValue).Select(d => d.Value));
        }
    }
}