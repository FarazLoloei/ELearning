using ELearning.SharedKernel;

namespace ELearning.Domain.ValueObjects;

public class Duration : ValueObject
{
    public int Hours { get; private set; }
    public int Minutes { get; private set; }
    public int TotalMinutes => Hours * 60 + Minutes;

    private Duration()
    { }

    private Duration(int hours, int minutes)
    {
        if (hours < 0)
            throw new ArgumentException("Hours cannot be negative", nameof(hours));

        if (minutes < 0 || minutes >= 60)
            throw new ArgumentException("Minutes must be between 0 and 59", nameof(minutes));

        Hours = hours;
        Minutes = minutes;
    }

    public static Duration Create(int hours, int minutes)
    {
        return new Duration(hours, minutes);
    }

    public static Duration FromMinutes(int totalMinutes)
    {
        if (totalMinutes < 0)
            throw new ArgumentException("Duration cannot be negative", nameof(totalMinutes));

        var hours = totalMinutes / 60;
        var minutes = totalMinutes % 60;

        return new Duration(hours, minutes);
    }

    public static Duration CreateDefault()
    {
        return new Duration(0, 0);
    }

    public override string ToString()
    {
        if (Hours == 0)
            return $"{Minutes} min";

        if (Minutes == 0)
            return $"{Hours} hr";

        return $"{Hours} hr {Minutes} min";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Hours;
        yield return Minutes;
    }
}