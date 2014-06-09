using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EV2020.Director;

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
	}
}
