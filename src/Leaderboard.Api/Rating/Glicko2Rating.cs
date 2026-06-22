namespace Leaderboard.Api.Rating;

public readonly record struct Glicko2Rating(double Rating, double RatingDeviation, double Volatility)
{
    public const double DefaultRating = 1500.0;
    public const double DefaultRd = 350.0;
    public const double DefaultVolatility = 0.06;
    private const double Scale = 173.7178;

    public static Glicko2Rating NewPlayer() =>
        new(DefaultRating, DefaultRd, DefaultVolatility);

    public double Mu => (Rating - DefaultRating) / Scale;

    public double Phi => RatingDeviation / Scale;

    public static Glicko2Rating FromInternal(double mu, double phi, double volatility) =>
        new(mu * Scale + DefaultRating, phi * Scale, volatility);
}
