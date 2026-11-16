namespace Library.Core;

// Beregner gebyr for for sen aflevering af en bog.
// 5 kr. pr. dag for sent, dog højst 100 kr. Til tiden eller før er gratis.
public class FeeCalculator
{
    public const decimal FeePerDay = 5m;
    public const decimal MaxFee = 100m;

    public decimal Calculate(DateTime due, DateTime returned)
    {
        // .Date ignorerer klokkeslæt, så kun hele kalenderdage tæller
        int daysLate = (returned.Date - due.Date).Days;
        if (daysLate <= 0)
            return 0m;

        return Math.Min(daysLate * FeePerDay, MaxFee);
    }
}
