using Leaderboard.Api.Rating;
using Xunit;

namespace Leaderboard.Tests;

public class Glicko2CalculatorTests
{
    private static double Mu(double rating) => (rating - 1500.0) / 173.7178;
    private static double Phi(double rd) => rd / 173.7178;

    [Theory]
    [InlineData(30.0, 0.9955)]
    [InlineData(100.0, 0.9531)]
    [InlineData(300.0, 0.7242)]
    public void G_MatchesGlickmanExample(double rd, double expected)
    {
        var result = Glicko2Calculator.G(Phi(rd));
        Assert.Equal(expected, result, precision: 4);
    }

    [Theory]
    [InlineData(1400.0, 30.0, 0.639)]
    [InlineData(1550.0, 100.0, 0.432)]
    [InlineData(1700.0, 300.0, 0.303)]
    public void E_MatchesGlickmanExample(double opponentRating, double opponentRd, double expected)
    {
        var playerMu = Mu(1500.0);
        var result = Glicko2Calculator.E(playerMu, Mu(opponentRating), Phi(opponentRd));
        Assert.Equal(expected, result, precision: 3);
    }
}
