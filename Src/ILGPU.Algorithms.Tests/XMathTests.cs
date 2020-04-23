﻿using ILGPU.Runtime;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace ILGPU.Algorithms.Tests
{
    public abstract partial class XMathTests : TestBase
    {
        protected XMathTests(ITestOutputHelper output, ContextProvider contextProvider)
            : base(output, contextProvider)
        { }

        internal readonly struct XMathTuple<T> where T : struct
        {
            public XMathTuple(T x, T y)
            {
                X = x;
                Y = y;
            }

            public T X { get; }
            public T Y { get; }
        }

        /// <summary>
        /// Compares two numbers for equality, within a defined tolerance.
        /// </summary>
        private class FloatPrecisionComparer
            : EqualityComparer<float>
        {
            public readonly float Margin;

            public FloatPrecisionComparer(uint decimalPlaces) =>
                Margin = MathF.Pow(10, -decimalPlaces);

            public override bool Equals(float x, float y) =>
                Math.Abs(x - y) < Margin;

            public override int GetHashCode(float obj) =>
                obj.GetHashCode();
        }

        /// <summary>
        /// Compares two numbers for equality, within a defined tolerance.
        /// </summary>
        private class DoublePrecisionComparer
            : EqualityComparer<double>
        {
            public readonly double Margin;

            public DoublePrecisionComparer(uint decimalPlaces) =>
                Margin = Math.Pow(10, -decimalPlaces);

            public override bool Equals(double x, double y) =>
                Math.Abs(x - y) < Margin;

            public override int GetHashCode(double obj) =>
                obj.GetHashCode();
        }

        /// <summary>
        /// Compares two numbers for equality, within a defined tolerance.
        /// </summary>
        private class FloatRelativeErrorComparer
            : EqualityComparer<float>
        {
            public readonly float RelativeError;

            public FloatRelativeErrorComparer(float relativeError) =>
                RelativeError = relativeError;

            public override bool Equals(float x, float y)
            {
                var diff = Math.Abs(x - y);

                if (diff == 0)
                    return true;

                if (x != 0)
                    return diff / x < RelativeError;

                return false;
            }

            public override int GetHashCode(float obj) =>
                obj.GetHashCode();
        }

        /// <summary>
        /// Compares two numbers for equality, within a defined tolerance.
        /// </summary>
        private class DoubleRelativeErrorComparer
            : EqualityComparer<double>
        {
            public readonly double RelativeError;

            public DoubleRelativeErrorComparer(double relativeError) =>
                RelativeError = relativeError;

            public override bool Equals(double x, double y)
            {
                var diff = Math.Abs(x - y);

                if (diff == 0)
                    return true;

                if (x != 0)
                    return diff / x < RelativeError;

                return false;
            }

            public override int GetHashCode(double obj) =>
                obj.GetHashCode();
        }

        /// <summary>
        /// Verifies the contents of the given memory buffer.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="buffer">The target buffer.</param>
        /// <param name="expected">The expected values.</param>
        /// <param name="decimalPlaces">The acceptable error margin.</param>
        public void VerifyWithinPrecision(
            MemoryBuffer<float> buffer,
            float[] expected,
            uint decimalPlaces)
        {
            var data = buffer.GetAsArray(Accelerator.DefaultStream);
            Assert.Equal(data.Length, expected.Length);

            var comparer = new FloatPrecisionComparer(decimalPlaces);
            for (int i = 0, e = data.Length; i < e; ++i)
                Assert.Equal(expected[i], data[i], comparer);
        }

        /// <summary>
        /// Verifies the contents of the given memory buffer.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="buffer">The target buffer.</param>
        /// <param name="expected">The expected values.</param>
        /// <param name="decimalPlaces">The acceptable error margin.</param>
        public void VerifyWithinPrecision(
            MemoryBuffer<double> buffer,
            double[] expected,
            uint decimalPlaces)
        {
            var data = buffer.GetAsArray(Accelerator.DefaultStream);
            Assert.Equal(data.Length, expected.Length);

            var comparer = new DoublePrecisionComparer(decimalPlaces);
            for (int i = 0, e = data.Length; i < e; ++i)
                Assert.Equal(expected[i], data[i], comparer);
        }

        /// <summary>
        /// Verifies the contents of the given memory buffer.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="buffer">The target buffer.</param>
        /// <param name="expected">The expected values.</param>
        /// <param name="relativeError">The acceptable error margin.</param>
        public void VerifyWithinRelativeError(
            MemoryBuffer<float> buffer,
            float[] expected,
            double relativeError)
        {
            var data = buffer.GetAsArray(Accelerator.DefaultStream);
            Assert.Equal(data.Length, expected.Length);

            var comparer = new FloatRelativeErrorComparer((float)relativeError);
            for (int i = 0, e = data.Length; i < e; ++i)
                Assert.Equal(expected[i], data[i], comparer);
        }

        /// <summary>
        /// Verifies the contents of the given memory buffer.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="buffer">The target buffer.</param>
        /// <param name="expected">The expected values.</param>
        /// <param name="relativeError">The acceptable error margin.</param>
        public void VerifyWithinRelativeError(
            MemoryBuffer<double> buffer,
            double[] expected,
            double relativeError)
        {
            var data = buffer.GetAsArray(Accelerator.DefaultStream);
            Assert.Equal(data.Length, expected.Length);

            var comparer = new DoubleRelativeErrorComparer(relativeError);
            for (int i = 0, e = data.Length; i < e; ++i)
                Assert.Equal(expected[i], data[i], comparer);
        }
    }
}
