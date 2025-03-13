using ELearning.SharedKernel;

namespace ELearning.Domain.ValueObjects;

public class Rating : ValueObject
{
    public decimal Value { get; private set; }

    public int NumberOfRatings { get; private set; }

    private Rating()
    { }

    private Rating(decimal value, int numberOfRatings)
    {
        if (value < 0 || value > 5)
            throw new ArgumentException("Rating must be between 0 and 5", nameof(value));

        if (numberOfRatings < 0)
            throw new ArgumentException("Number of ratings cannot be negative", nameof(numberOfRatings));

        Value = Math.Round(value, 1);
        NumberOfRatings = numberOfRatings;
    }

    public static Rating Create(decimal value, int numberOfRatings)
    {
        return new Rating(value, numberOfRatings);
    }

    public static Rating CreateDefault()
    {
        return new Rating(0, 0);
    }

    public Rating AddRating(decimal newRating)
    {
        if (newRating < 0 || newRating > 5)
            throw new ArgumentException("Rating must be between 0 and 5", nameof(newRating));

        var totalValue = Value * NumberOfRatings;
        var newNumberOfRatings = NumberOfRatings + 1;
        var newAverageValue = (totalValue + newRating) / newNumberOfRatings;

        return new Rating(newAverageValue, newNumberOfRatings);
    }

    public Rating RemoveRating(decimal oldRating)
    {
        if (NumberOfRatings <= 1)
            return CreateDefault();

        var totalValue = Value * NumberOfRatings;
        var newNumberOfRatings = NumberOfRatings - 1;
        var newAverageValue = (totalValue - oldRating) / newNumberOfRatings;

        return new Rating(newAverageValue, newNumberOfRatings);
    }

    public override string ToString() => $"{Value:0.0}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return NumberOfRatings;
    }
}