namespace Leaderboard.Api.Rating;

public readonly record struct MatchResult(double OpponentMu, double OpponentPhi, double Score);

public static class Glicko2Calculator
{
    public static double G(double phi) =>
        1.0 / Math.Sqrt(1.0 + 3.0 * phi * phi / (Math.PI * Math.PI));

    public static double E(double mu, double muJ, double phiJ) =>
        1.0 / (1.0 + Math.Exp(-G(phiJ) * (mu - muJ)));

    public static double V(double mu, IReadOnlyList<MatchResult> results)
    {
        double sum = 0.0;
        foreach (var r in results)
        {
            double e = E(mu, r.OpponentMu, r.OpponentPhi);
            double g = G(r.OpponentPhi);
            sum += g * g * e * (1.0 - e);
        }
        return 1.0 / sum;
    }

    public static double Delta(double mu, IReadOnlyList<MatchResult> results)
    {
        double v = V(mu, results);
        double sum = 0.0;
        foreach (var r in results)
        {
            double e = E(mu, r.OpponentMu, r.OpponentPhi);
            double g = G(r.OpponentPhi);
            sum += g * (r.Score - e);
        }
        return v * sum;
    }
}
