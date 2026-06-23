namespace CourierMax.UseCases;

public static class BusinessDayCalculator
{
    private static readonly HashSet<DateTime> Festivos2026 = new(new[]
    {
        new DateTime(2026,1,1), new DateTime(2026,1,26), new DateTime(2026,1,30),
        new DateTime(2026,3,24), new DateTime(2026,5,1), new DateTime(2026,6,1),
        new DateTime(2026,6,29), new DateTime(2026,7,20), new DateTime(2026,8,17),
        new DateTime(2026,10,20), new DateTime(2026,11,9), new DateTime(2026,12,8),
    });

    public static bool EsHabil(DateTime dia) =>
        dia.DayOfWeek != DayOfWeek.Saturday &&
        dia.DayOfWeek != DayOfWeek.Sunday &&
        !Festivos2026.Contains(dia.Date);

    public static DateTime AddBusinessDays(DateTime start, int days)
    {
        var d = start.Date;
        int restantes = days;
        while (restantes > 0)
        {
            d = d.AddDays(1);
            if (EsHabil(d)) restantes--;
        }
        return d;
    }

    public static int BusinessDaysBetween(DateTime start, DateTime end)
    {
        if (end.Date <= start.Date) return 0;
        int count = 0;
        for (var d = start.Date.AddDays(1); d <= end.Date; d = d.AddDays(1))
            if (EsHabil(d)) count++;
        return count;
    }
}
