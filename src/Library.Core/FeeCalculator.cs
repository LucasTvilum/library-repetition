namespace Library.Core;

public class FeeCalculator
{
    public decimal Calculate(DateTime due, DateTime returned)
    {
        if (returned.Date > due.Date)
            return 5m;

        return 0m;
    }
}
