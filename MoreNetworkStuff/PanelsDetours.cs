
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
        //TODO: add more stuff
        public static readonly string[] AssetEditorProceduralWhitelist =
        {
            "Train Cargo Track",
            "Train Cargo Track Elevated",
            "Cargo Connection",
            "Ship Dock",
            "Ship Dockway",
            "Airplane Runway",
            "Airplane Taxiway",
            "Airplane Stop",
            "Airplane Cargo Stop",
            "Pedestrian Connection",
            "Pedestrian Connection Surface",
            "Pedestrian Connection Underground",
            "Pedestrian Connection Inside",
            "Bus Station Stop",
            "Bus Station Way",
            "Quay",
            "Canal",
            "Canal2",
            "Canal3",
            "Canal Wide",
            "Canal Wide2",
            "Canal Wide3",
            "Flood Wall",
            "Castle Wall 1",
            "Castle Wall 2",
            "Castle Wall 3",
            "Trench Ruins 01",
            "Fishing Dockway",
            "Aviation Club Runway",
            "Trolleybus Depot",
            "Helicopter Depot Path",
            "Helicopter Stop",
            "Ferry Dock",
            "Ferry Dockway",
            "CableCar Stop",
            "Blimp Depot Path",
            "Blimp Stop",
        };

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
                        flag |= (AssetEditorProceduralWhitelist.Contains(info.name) || (info?.name?.Contains("Station Track") ?? false));
                        flag &= (AssetEditorProceduralWhitelist.Contains(info.name) || (info?.name?.Contains("Station Track") ?? false));
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