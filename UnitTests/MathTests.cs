using CryptoKeeper.Domain.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptoKeeper.UnitTests
{
    [TestClass]
    public class MathTests
    {
        [TestMethod]
        public void PercentDiff_ShouldBeAccurate_IfValueAIsHigher()
        {
            Assert.AreEqual(new MathService().PercentDiff(6, 5), -0.1666666666666666666666666667m);
        }

        [TestMethod]
        public void PercentDiff_ShouldBeAccurate_IfValueAIsLower()
        {
            Assert.AreEqual(new MathService().PercentDiff(5, 6), 0.1666666666666666666666666667m);
        }

        [TestMethod]
        public void PercentDiff_ShouldReturnNegative_IfValueAIsHigher()
        {
            Assert.IsTrue(new MathService().PercentDiff(6, 5) < 0);
        }

        [TestMethod]
        public void PercentDiff_ShouldReturnPositive_IfValueAIsLower()
        {
            Assert.IsTrue(new MathService().PercentDiff(5, 6) > 0);
        }

        [TestMethod]
        public void FormatPercent_ShouldReturnCorrectly_IfProvidedPositiveDecimal()
        {
            Assert.AreEqual(new MathService().FormatPercent(0.2m), "20.0%");
        }

        [TestMethod]
        public void FormatPercent_ShouldReturnCorrectly_IfProvidedNegativeDecimal()
        {
            Assert.AreEqual(new MathService().FormatPercent(-0.2m), "-20.0%");
        }
    }
}
