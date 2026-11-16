using Library.Core;

namespace Library.Tests;

// Del 4 – FeeCalculator udviklet med TDD (Red → Green → Refactor).
// Krav: 5 kr. pr. dag for sent, dog højst 100 kr. Til tiden eller før er gratis.
[TestClass]
public class FeeCalculatorTests
{
    [TestMethod]
    public void Calculate_AfleveretTilTiden_Returnerer0()
    {
        var calculator = new FeeCalculator();
        var due = new DateTime(2026, 11, 30);

        decimal fee = calculator.Calculate(due, due);

        Assert.AreEqual(0m, fee);
    }

    [TestMethod]
    public void Calculate_AfleveretFoerForfald_Returnerer0()
    {
        var calculator = new FeeCalculator();
        var due = new DateTime(2026, 11, 30);
        var returned = new DateTime(2026, 11, 25);

        decimal fee = calculator.Calculate(due, returned);

        Assert.AreEqual(0m, fee);
    }
}
