using System.Text.RegularExpressions;
using CommunityToolkit.Diagnostics;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public class ResolutionHelper
    {
        public static Resolution Parse(string formattedString)
        {
            Guard.IsNotNullOrWhiteSpace(formattedString, nameof(formattedString));

            var matches = Regex.Matches(formattedString, @"(\d) x (\d) @ (\d)");

            var res = new Resolution();

            if (matches.Count < 2)
                throw new System.InvalidOperationException(@$"String: {formattedString} has invalid format");

            res.width = int.Parse(matches[0].Value);
            res.height = int.Parse(matches[1].Value);

            if (matches.Count > 2)
            {
                res.refreshRateRatio = new RefreshRate()
                {
                    numerator = uint.Parse(matches[3].Value),
                    denominator = 1u
                };
            }

            return res;
        }
    }
}
