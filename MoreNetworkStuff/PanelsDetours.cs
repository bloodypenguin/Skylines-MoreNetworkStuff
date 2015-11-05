using System;
using System.Linq;
using System.Reflection;
using ColossalFramework;

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

        private static bool _deployed;
        private static RedirectCallsState _state1;
        private static MethodInfo _originalInfo1;
        private static MethodInfo _detourInfo1;
        private static RedirectCallsState _state2;
        private static MethodInfo _originalInfo2;
        private static MethodInfo _detourInfo2;
        private static RedirectCallsState _state3;
        private static MethodInfo _originalInfo3;
        private static MethodInfo _detourInfo3;

        private static RedirectCallsState _state4;
        private static MethodInfo _originalInfo4;
        private static MethodInfo _detourInfo4;


        public static void Deploy()
        {
            if (_deployed) return;
            try
            {
                _originalInfo1 = typeof (RoadsPanel).GetMethod("IsPlacementRelevant",
                    BindingFlags.Instance | BindingFlags.NonPublic, null, new[] {typeof (NetInfo)}, null);
                _detourInfo1 = typeof (RoadsPanelDetour).GetMethod("IsPlacementRelevant",
                    BindingFlags.Instance | BindingFlags.NonPublic, null, new[] {typeof (NetInfo)}, null);
                _state1 = RedirectionHelper.RedirectCalls(_originalInfo1, _detourInfo1);

                _originalInfo2 = typeof (RoadsGroupPanel).GetMethod("IsServiceValid",
                    BindingFlags.Instance | BindingFlags.NonPublic, null, new[] {typeof (PrefabInfo)}, null);
                _detourInfo2 = typeof (RoadsGroupPanelDetour).GetMethod("IsServiceValid",
                    BindingFlags.Instance | BindingFlags.NonPublic, null, new[] {typeof (PrefabInfo)}, null);
                _state2 = RedirectionHelper.RedirectCalls(_originalInfo2, _detourInfo2);

                _originalInfo3 = typeof (GeneratedGroupPanel).GetMethod("IsPlacementRelevant",
                    BindingFlags.Instance | BindingFlags.NonPublic, null, new[] {typeof (NetInfo)}, null);
                _detourInfo3 = typeof (GeneratedGroupPanelDetour).GetMethod("IsPlacementRelevant",
                    BindingFlags.Instance | BindingFlags.NonPublic, null, new[] {typeof (NetInfo)}, null);
                _state3 = RedirectionHelper.RedirectCalls(_originalInfo3, _detourInfo3);

                _originalInfo4 = typeof(RoadsPanel).GetMethod("IsPlacementRelevant",
                    BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(BuildingInfo) }, null);
                _detourInfo4 = typeof(RoadsPanelDetour).GetMethod("IsPlacementRelevant",
                    BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(BuildingInfo) }, null);
                _state4 = RedirectionHelper.RedirectCalls(_originalInfo4, _detourInfo4);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);  
                Revert();
            }
            _deployed = true;
        }

        public static void Revert()
        {
            if (!_deployed) return;
            try
            {
                if (_originalInfo1 != null && _detourInfo1 != null)
                {
                    RedirectionHelper.RevertRedirect(_originalInfo1, _state1);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            try
            {
                if (_originalInfo2 != null && _detourInfo2 != null)
                {
                    RedirectionHelper.RevertRedirect(_originalInfo2, _state2);
                }

            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            try
            {
                if (_originalInfo3 != null && _detourInfo3 != null)
                {
                    RedirectionHelper.RevertRedirect(_originalInfo3, _state3);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            try
            {
                if (_originalInfo4 != null && _detourInfo4 != null)
                {
                    RedirectionHelper.RevertRedirect(_originalInfo4, _state4);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            _deployed = false;
        }



        private class RoadsPanelDetour : GeneratedScrollPanel
        {
            protected override bool IsPlacementRelevant(NetInfo info)
            {
                return PanelsDetours.IsPlacementRelevant(info, isMapEditor, isGame, isAssetEditor);
            }

            protected override bool IsPlacementRelevant(BuildingInfo info)
            {
                return base.IsPlacementRelevant(info);
            }

            public override ItemClass.Service service
            {
                get { throw new System.NotImplementedException(); }
            }

        }

        private class RoadsGroupPanelDetour : GeneratedGroupPanel
        {
            protected override bool IsServiceValid(PrefabInfo info)
            {
                if (info.GetService() == this.service || this.isMapEditor &&
                    (info.GetService() == ItemClass.Service.PublicTransport/* || info.GetService() == ItemClass.Service.Beautification*/))
                    return true;
                if (this.isAssetEditor && info.GetService() == ItemClass.Service.PublicTransport)
                    return info.GetSubService() == ItemClass.SubService.PublicTransportTrain || info.GetSubService() == ItemClass.SubService.PublicTransportMetro || info.GetSubService() == ItemClass.SubService.None;
                return false;
            }
        }

        private class GeneratedGroupPanelDetour : GeneratedGroupPanel
        {
            private bool IsPlacementRelevant(NetInfo info)
            {
                return PanelsDetours.IsPlacementRelevant(info, isMapEditor, isGame, isAssetEditor);
            }
        }

        private static bool IsPlacementRelevant(NetInfo info, bool isMapEditor, bool isGame, bool isAssetEditor)
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