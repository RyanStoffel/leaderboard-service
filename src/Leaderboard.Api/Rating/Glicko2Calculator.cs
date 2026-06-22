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

    public static double Volatility(
        double phi,
        double v,
        double delta,
        double sigma,
        double tau)
    {
        double a = Math.Log(sigma * sigma);

        double F(double x)
        {
            double ex = Math.Exp(x);
            double num = ex * (delta * delta - phi * phi - v - ex);
            double den = 2.0 * Math.Pow(phi * phi + v + ex, 2);
            return (num / den) - ((x - a) / (tau * tau));
        }

        double A = a;
        double B;

        double delta2 = delta * delta;
        double phi2v = phi * phi + v;

        if (delta2 > phi2v)
        {
            B = Math.Log(delta2 - phi2v);
        }
        else
        {
            double k = 1.0;
            while (F(a - k * tau) < 0.0)
            {
                k += 1.0;
            }
            B = a - k * tau;
        }

        double fA = F(A);
        double fB = F(B);

        const double epsilon = 0.000001;

        while (Math.Abs(B - A) > epsilon)
        {
            double C = A + (A - B) * fA / (fB - fA);
            double fC = F(C);

            if (fC * fB <= 0.0)
            {
                A = B;
                fA = fB;
            }
            else
            {
                fA = fA / 2.0;
            }

            B = C;
            fB = fC;
        }

        return Math.Exp(A / 2.0);
    }

    public static Glicko2Rating Update(
        Glicko2Rating player,
        IReadOnlyList<MatchResult> results,
        double tau = 0.5)
    {
        if (results.Count == 0)
        {
            double phiStarOnly = Math.Sqrt(player.Phi * player.Phi + player.Volatility * player.Volatility);
            return Glicko2Rating.FromInternal(player.Mu, phiStarOnly, player.Volatility);
        }

        double mu = player.Mu;
        double phi = player.Phi;
        double sigma = player.Volatility;

        double v = V(mu, results);
        double delta = Delta(mu, results);
        double sigmaPrime = Volatility(phi, v, delta, sigma, tau);

        double phiStar = Math.Sqrt(phi * phi + sigmaPrime * sigmaPrime);
        double phiPrime = 1.0 / Math.Sqrt(1.0 / (phiStar * phiStar) + 1.0 / v);

        double sum = 0.0;
        foreach (var r in results)
        {
            sum += G(r.OpponentPhi) * (r.Score - E(mu, r.OpponentMu, r.OpponentPhi));
        }
        double muPrime = mu + phiPrime * phiPrime * sum;

        return Glicko2Rating.FromInternal(muPrime, phiPrime, sigmaPrime);
    }
}
