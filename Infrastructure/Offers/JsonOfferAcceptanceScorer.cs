using System.Text.Json;
using CIF.Application.CustomerProfiles.Offers;

namespace CIF.Infrastructure.Offers;

// Immutable model; safe for singleton concurrent inference. Training stays offline.
public sealed class JsonOfferAcceptanceScorer : IOfferAcceptanceScorer
{
    private readonly JsonElement model;

    public JsonOfferAcceptanceScorer(string path)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        model = document.RootElement.Clone();
        if (model.GetProperty("schemaVersion").GetInt32() != 1 ||
            !model.GetProperty("features").EnumerateArray().Select(x => x.GetString())
                .SequenceEqual(new[] { "budget", "planPrice", "addOnPrice", "priceToBudgetRatio" }))
            throw new InvalidDataException("Unsupported offer model schema.");
        var kind = model.GetProperty("kind").GetString();
        if (kind is not ("logistic" or "tree"))
            throw new InvalidDataException("Unsupported offer model kind.");
    }

    public double Score(OfferAcceptanceFeatures features)
    {
        var x = features.ToVector();
        if (model.GetProperty("kind").GetString() == "tree")
        {
            var node = model.GetProperty("root");
            while (!node.TryGetProperty("probability", out _))
                node = node.GetProperty((float)x[node.GetProperty("feature").GetInt32()] <=
                    node.GetProperty("threshold").GetDouble() ? "left" : "right");
            return node.GetProperty("probability").GetDouble();
        }
        var z = model.GetProperty("intercept").GetDouble();
        for (var i = 0; i < x.Length; i++)
            z += (x[i] - model.GetProperty("mean")[i].GetDouble()) /
                model.GetProperty("scale")[i].GetDouble() * model.GetProperty("weights")[i].GetDouble();
        return z >= 0 ? 1 / (1 + Math.Exp(-z)) : Math.Exp(z) / (1 + Math.Exp(z));
    }
}
