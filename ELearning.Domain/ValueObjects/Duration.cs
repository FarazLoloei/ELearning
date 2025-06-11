using ELearning.SharedKernel;

namespace ELearning.Domain.ValueObjects;

public class Duration : ValueObject
{
    private const int MinutesPerHour = 60;

    public int Hours { get; }

    public int Minutes { get; }

    public int TotalMinutes => Hours * MinutesPerHour + Minutes;

    private static readonly Duration DefaultInstance = new(0, 0);

    private Duration(int hours, int minutes)
    {
        if (hours < 0)
            throw new ArgumentOutOfRangeException(nameof(hours), "Hours cannot be negative.");

        if (minutes is < 0 or >= MinutesPerHour)
            throw new ArgumentOutOfRangeException(nameof(minutes), "Minutes must be between 0 and 59.");

        Hours = hours;
        Minutes = minutes;
    }

    public static Duration Create(int hours, int minutes) => new(hours, minutes);

    public static Duration FromMinutes(int totalMinutes)
    {
        if (totalMinutes < 0)
            throw new ArgumentOutOfRangeException(nameof(totalMinutes), "Duration cannot be negative.");

        return new Duration(totalMinutes / MinutesPerHour, totalMinutes % MinutesPerHour);
    }

    public static Duration CreateDefault() => DefaultInstance;

    public override string ToString() =>
        (Hours, Minutes) switch
        {
            (0, var min) => $"{min} min",
            (var hr, 0) => $"{hr} hr",
            _ => $"{Hours} hr {Minutes} min"
        };

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Hours;
        yield return Minutes;
    }
}