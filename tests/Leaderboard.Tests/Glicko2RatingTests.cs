using Leaderboard.Api.Rating;
using Xunit;

namespace Leaderboard.Tests;

public class Glicko2RatingTests
{
    [Fact]
    public void DefaultPlayer_HasMuZero()
    {
        var player = Glicko2Rating.NewPlayer();
        Assert.Equal(0.0, player.Mu, precision: 10);
    }

    [Fact]
    public void DefaultPlayer_PhiMatchesExpected()
    {
        var player = Glicko2Rating.NewPlayer();
        Assert.Equal(350.0 / 173.7178, player.Phi, precision: 10);
    }

    [Fact]
    public void RoundTrip_ReturnsOriginalValues()
    {
        var original = new Glicko2Rating(1842.0, 200.0, 0.06);
        var restored = Glicko2Rating.FromInternal(original.Mu, original.Phi, original.Volatility);

        Assert.Equal(original.Rating, restored.Rating, precision: 9);
        Assert.Equal(original.RatingDeviation, restored.RatingDeviation, precision: 9);
        Assert.Equal(original.Volatility, restored.Volatility, precision: 9);
    }
}
