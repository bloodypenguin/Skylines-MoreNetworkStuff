
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using MoreNetworkStuff.Redirection;

namespace MoreNetworkStuff
{
    public class PanelsDetours
    {
        private static readonly string[] AssetEditorProceduralWhitelist =
        {
            "Train Station Track",
            "Train Cargo Track",
            "Train Cargo Track Elevated",
            "Cargo Connection",
            "Ship Dock",
            "Ship Dockway",
            "Airplane Runway",
            "Airplane Taxiway",
            "Airplane Stop",
            "Pedestrian Connection",
            "Pedestrian Connection Surface",
            "Pedestrian Connection Underground",
            "Pedestrian Connection Inside",
            "Metro Station Track",
            "Bus Station Stop",
            "Bus Station Way",
            "Train Station Track (C)",
            "Train Station Track (NP)",
            "Train Station Track (CNP)",
            "Station Track Eleva",
            "Station Track Elevated (C)",
            "Station Track Elevated (NP)",
            "Station Track Elevated (CNP)",
            "Station Track Elevated Narrow",
            "Station Track Elevated Narrow (C)",
            "Station Track Elevated Narrow (NP)",
            "Station Track Elevated Narrow (CNP)",
            "Station Track Sunken",
            "Station Track Sunken (NP)",
            "Station Track Tunnel"
        };

        private static Dictionary<MethodInfo, RedirectCallsState> redirects;
        private static bool _deployed;

        public static void Deploy()
        {
            if (_deployed) return;
            redirects = new Dictionary<MethodInfo, RedirectCallsState>();
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                redirects.AddRange(RedirectionUtil.RedirectType(type));
            }
            _deployed = true;
        }

        public static void Revert()
        {
            if (!_deployed) return;
            if (redirects == null)
            {
                return;
            }
            foreach (var kvp in redirects)
            {
                RedirectionHelper.RevertRedirect(kvp.Key, kvp.Value);
            }
            redirects.Clear();
            _deployed = false;
        }

        public static bool IsPlacementRelevant(NetInfo info, bool isMapEditor, bool isGame, bool isAssetEditor)
        {
            bool flag = true;
            if (Singleton<ToolManager>.exists)
            {
                flag &= info.m_availableIn.IsFlagSet(Singleton<ToolManager>.instance.m_properties.m_mode);
                flag |= isMapEditor && info.m_availableIn.IsFlagSet(ItemClass.Availability.Game);
                if (isAssetEditor)
                {
                    if (info.m_placementStyle == ItemClass.Placement.Procedural)
                    {
                        flag |= AssetEditorProceduralWhitelist.Contains(info.name);
                        flag &= AssetEditorProceduralWhitelist.Contains(info.name);
                    }
                    else if (info.m_placementStyle == ItemClass.Placement.Manual)
                    {
                        flag = true;
                    }
                }
                flag |= isGame && info.m_availableIn.IsFlagSet(ItemClass.Availability.MapEditor);
            }
            return flag & (info.m_placementStyle == ItemClass.Placement.Manual || (isAssetEditor && (info.m_placementStyle == ItemClass.Placement.Procedural)));
        }

    }
}