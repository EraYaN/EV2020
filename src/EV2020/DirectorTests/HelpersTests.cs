using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EV2020.Director;
using MathNet.Numerics.LinearAlgebra.Double;

namespace DirectorTests
{
	[TestClass]
	public class HelpersTests
	{
		// unit test code
		[TestMethod]
		public void TestDeadzoneNoEffect()
		{
			double value = 10;
			double upperBound = 5;
			double lowerBound = -5;
			double expected = value;
			double actual = value.Deadzone(lowerBound, upperBound);
			
			Assert.AreEqual(expected, actual, 0.001, "It alters the value incorrectly.");
		}
		[TestMethod]
		public void TestDeadzoneLowerEffect()
		{
			double value = -4;
			double upperBound = 5;
			double lowerBound = -5;
			double expected = 0;
			double actual = value.Deadzone(lowerBound, upperBound);

			Assert.AreEqual(expected, actual, 0.001, "It does not alter the value correctly (lower).");
		}
		[TestMethod]
		public void TestDeadzoneUpperEffect()
		{
			double value = 4;
			double upperBound = 5;
			double lowerBound = -5;
			double expected = 0;
			double actual = value.Deadzone(lowerBound, upperBound);

			Assert.AreEqual(expected, actual, 0.001, "It does not alter the value correctly (upper).");
		}
		[TestMethod]
		public void TestAngleQ1()
		{
			double expected;
			double actual;
			//Q1
			actual = DenseVector.OfArray(new double[] { 1, 1 }).Angle();
			expected = Math.PI / 4;
			Assert.AreEqual(expected, actual, 0.001, "First Quadrant fail");
		}
		[TestMethod]
		public void TestAngleQ2()
		{
			double expected;
			double actual;			
			//Q2
			actual = DenseVector.OfArray(new double[] { -1, 1 }).Angle();
			expected = Math.PI / 4 + Math.PI / 2;
			Assert.AreEqual(expected, actual, 0.001, "Second Quadrant fail");			
		}
		[TestMethod]
		public void TestAngleQ3()
		{
			double expected;
			double actual;			
			//Q3
			actual = DenseVector.OfArray(new double[] { -1, -1 }).Angle();
			expected = Math.PI / 4 + Math.PI;
			Assert.AreEqual(expected, actual, 0.001, "Third Quadrant fail");
		}
		[TestMethod]
		public void TestAngleQ4()
		{
			double expected;
			double actual;			
			//Q4
			actual = DenseVector.OfArray(new double[] { 1, -1 }).Angle();
			expected = Math.PI / 4 - (Math.PI / 2);
			Assert.AreEqual(expected, actual, 0.001, "Forth Quadrant fail");
		}
		
	}
}
