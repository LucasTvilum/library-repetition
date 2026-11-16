using Library.Core;

namespace Library.Tests;

// Unit tests af IsbnValidator.
// Hver test følger Arrange–Act–Assert og tester præcis én ting.
// Navngivning: Metode_Scenarie_ForventetResultat
[TestClass]
public class IsbnValidatorTests
{
    [TestMethod]
    public void IsValid_GyldigtIsbn13_ReturnererTrue()
    {
        // Arrange
        var isbn = "9780306406157";

        // Act
        var result = IsbnValidator.IsValid(isbn);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsValid_GyldigtIsbnMedBindestreger_ReturnererTrue()
    {
        // Arrange – bindestregerne skal fjernes, før cifrene tjekkes
        var isbn = "978-0-306-40615-7";

        // Act
        var result = IsbnValidator.IsValid(isbn);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsValid_ForkertKontrolciffer_ReturnererFalse()
    {
        // Arrange – sidste ciffer er 8 i stedet for 7, så summen ikke går op i 10
        var isbn = "9780306406158";

        // Act
        var result = IsbnValidator.IsValid(isbn);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsValid_KunTolvCifre_ReturnererFalse()
    {
        // Arrange – ét ciffer for lidt
        var isbn = "978030640615";

        // Act
        var result = IsbnValidator.IsValid(isbn);

        // Assert
        Assert.IsFalse(result);
    }
}
