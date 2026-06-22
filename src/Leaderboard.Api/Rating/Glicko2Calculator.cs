namespace Leaderboard.Api.Rating;

public static class Glicko2Calculator
{
    public static double G(double phi) =>
        1.0 / Math.Sqrt(1.0 + 3.0 * phi * phi / (Math.PI * Math.PI));

    public static double E(double mu, double muJ, double phiJ) =>
        1.0 / (1.0 + Math.Exp(-G(phiJ) * (mu - muJ)));
}
