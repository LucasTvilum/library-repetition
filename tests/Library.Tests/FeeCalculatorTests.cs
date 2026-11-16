using Library.Core;

namespace Library.Tests;

// Del 4 – FeeCalculator udviklet med TDD (Red → Green → Refactor).
// Krav: 5 kr. pr. dag for sent, dog højst 100 kr. Til tiden eller før er gratis.
// MSTest laver en ny instans af klassen pr. test, så felterne deles ikke mellem tests.
[TestClass]
public class FeeCalculatorTests
{
    private readonly FeeCalculator _calculator = new();
    private static readonly DateTime Due = new(2026, 11, 30);

    [TestMethod]
    public void Calculate_AfleveretTilTiden_Returnerer0()
    {
        decimal fee = _calculator.Calculate(Due, Due);

        Assert.AreEqual(0m, fee);
    }

    [TestMethod]
    public void Calculate_AfleveretFoerForfald_Returnerer0()
    {
        var returned = new DateTime(2026, 11, 25);

        decimal fee = _calculator.Calculate(Due, returned);

        Assert.AreEqual(0m, fee);
    }

    [TestMethod]
    public void Calculate_EnDagForSent_Returnerer5()
    {
        var returned = new DateTime(2026, 12, 1);

        decimal fee = _calculator.Calculate(Due, returned);

        Assert.AreEqual(5m, fee);
    }
}
