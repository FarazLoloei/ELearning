// <copyright file="Rating.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Domain.ValueObjects;

using ELearning.SharedKernel;

public class Rating : ValueObject
{
    private const byte RatingMaxValue = 5;

    private const byte RatingMinValue = 0;

    private const int DecimalPrecision = 1;

    public decimal Value { get; }

    public int NumberOfRatings { get; }

    private Rating(decimal value, int numberOfRatings)
    {
        ValidateRating(value);
        ValidateNumberOfRatings(numberOfRatings);

        this.Value = Math.Round(value, DecimalPrecision); // Round to 1 decimal place during creation
        this.NumberOfRatings = numberOfRatings;
    }

    public static Rating Create(decimal value, int numberOfRatings) => new Rating(value, numberOfRatings);

    public static Rating CreateDefault() => new Rating(0, 0);

    public Rating AddRating(decimal newRating) => this.AdjustRating(newRating, 1);

    public Rating RemoveRating(decimal oldRating)
    {
        ValidateRating(oldRating);

        if (this.NumberOfRatings <= 0)
        {
            throw new InvalidOperationException("Cannot remove a rating when there are no ratings.");
        }

        var newNumberOfRatings = this.NumberOfRatings - 1;
        if (newNumberOfRatings == 0)
        {
            return CreateDefault();
        }

        var newTotalValue = (this.Value * this.NumberOfRatings) - oldRating;
        var newAverageValue = newTotalValue / newNumberOfRatings;
        return new Rating(newAverageValue, newNumberOfRatings);
    }

    private Rating AdjustRating(decimal ratingChange, int ratingCountChange)
    {
        ValidateRating(ratingChange);

        var newNumberOfRatings = this.NumberOfRatings + ratingCountChange;
        var newTotalValue = (this.Value * this.NumberOfRatings) + ratingChange;
        var newAverageValue = newTotalValue / newNumberOfRatings;

        return new Rating(Math.Round(newAverageValue, DecimalPrecision), newNumberOfRatings);
    }

    private static void ValidateRating(decimal rating)
    {
        if (rating < RatingMinValue || rating > RatingMaxValue)
        {
            throw new ArgumentException($"Rating must be between {RatingMinValue} and {RatingMaxValue}", nameof(rating));
        }
    }

    private static void ValidateNumberOfRatings(int numberOfRatings)
    {
        if (numberOfRatings < 0)
        {
            throw new ArgumentException("Number of ratings cannot be negative", nameof(numberOfRatings));
        }
    }

    public override string ToString() => $"{this.Value:0.0}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Value;
        yield return this.NumberOfRatings;
    }
}
