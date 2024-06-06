using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;

namespace aRandomKiwi.MFM
{
    internal class HarmonyUtils
    {
        public static bool IsSOS2SpaceMap(Map map)
        {
            var traverse = Traverse.Create(map);
            var isSpaceMethod = traverse.Method("IsSpace");
            if (isSpaceMethod.MethodExists() && (bool)isSpaceMethod.GetValue())
            {
                return true;
            }
            return false;
        }

        public static Map GetPlayerMainColonyMapSOS2Excluded()
        {
            var allPlayerHomes = (from x in Find.Maps
                                  where x.IsPlayerHome
                                  select x).ToList();

            var allNonSpaceMaps = new List<Map>();
            foreach (var map in allPlayerHomes)
            {
                if (IsSOS2SpaceMap(map) == false)
                {
                    allNonSpaceMaps.Add(map);
                }
            }

            if (!allNonSpaceMaps.Any())
            {
                return null;
            }

            try {
                return allNonSpaceMaps.OrderByDescending(map => map.PlayerWealthForStoryteller).First();
            } 
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
