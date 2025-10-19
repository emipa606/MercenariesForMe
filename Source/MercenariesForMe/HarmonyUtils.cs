using System.Linq;
using Verse;

namespace aRandomKiwi.MFM;

internal class HarmonyUtils
{
    private static bool IsSOS2SpaceMap(Map map)
    {
        return map.Biome.defName.Contains("OuterSpace");
    }

    private static bool IsRimNauts2SpaceMap(Map map)
    {
        return map.Biome.defName.StartsWith("RimNauts2_");
    }

    public static bool IsSOS2OrRimNauts2SpaceMap(Map map)
    {
        return IsSOS2SpaceMap(map) || IsRimNauts2SpaceMap(map);
    }

    public static Map GetPlayerMainColonyMap(bool excludeSOS2Rimnauts2SpaceMaps = true, bool requirePlayerHome = true)
    {
        var playerHomes = (from map in Find.Maps
            where (!requirePlayerHome || map.IsPlayerHome)
                  && (!excludeSOS2Rimnauts2SpaceMaps || !IsSOS2OrRimNauts2SpaceMap(map))
            select map).OrderByDescending(map => map.PlayerWealthForStoryteller).ToList();

        return playerHomes.Count > 0 ? playerHomes.First() : null;
    }
}