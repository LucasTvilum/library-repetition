namespace Library.Core;

// Beregner gebyr for for sen aflevering af en bog.
public class FeeCalculator
{
    public const decimal FeePerDay = 5m;

    public decimal Calculate(DateTime due, DateTime returned)
    {
        int daysLate = (returned.Date - due.Date).Days;
        if (daysLate <= 0)
            return 0m;

        return Math.Min(daysLate * FeePerDay, 100m);
    }
}
