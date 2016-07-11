using System.Linq;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using MoreNetworkStuff.Detours;
using MoreNetworkStuff.Redirection;
using PrefabHook;
using UnityEngine;

namespace MoreNetworkStuff
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private static FieldInfo _uiCategoryfield = typeof(PrefabInfo).GetField("m_UICategory", BindingFlags.NonPublic | BindingFlags.Instance);

        public static readonly string[] ConnectionNetworks =
        {
            "Cargo Connection",
            "Ship Dock",
            "Ship Dockway",
            "Pedestrian Connection",
            "Pedestrian Connection Surface",
            "Pedestrian Connection Underground",
            "Pedestrian Connection Inside",
            "Bus Station Stop",
            "Bus Station Way",
        };


        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            PanelsDetours.Deploy();
            if (!IsHooked())
            {
                return;
            }
            TransportInfoHook.OnPreInitialization += OnPreInitializationTI;
            TransportInfoHook.Deploy();
            NetInfoHook.OnPreInitialization += OnPreInitializationNI;
            NetInfoHook.Deploy();
        }

        private static void OnPreInitializationTI(TransportInfo info)
        {
            if (info.name != "Airplane")
            {
                return;
            }
            info.m_pathVisibility = ItemClass.Availability.GameAndMap;
        }

        private static void OnPreInitializationNI(NetInfo info)
        {
            if (info.name == "Airplane Path" || info.name == "Ship Path")
            {
                info.m_availableIn = ItemClass.Availability.GameAndMap;
            }
            if (info.name == "Pedestrian Connection")
            {
                //info.m_placementStyle = ItemClass.Placement.Manual;
                info.m_availableIn = ItemClass.Availability.All;
            }
            if (info.name?.Contains("Pedestrian Connection") ?? false)
            {
                info.m_class.m_service = ItemClass.Service.Beautification;
                _uiCategoryfield.SetValue(info, "BeautificationPaths");
                var ai = info.GetComponent<PedestrianPathAI>();
                ai.m_tunnelInfo = info;
                ai.m_bridgeInfo = info;
                ai.m_elevatedInfo = info;
                ai.m_slopeInfo = info;
            }
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (!IsHooked())
            {
                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage(
                    "Missing dependency",
                    "'More Network Stuff' mod requires the 'Prefab Hook' mod to work properly. Please subscribe to the mod and restart the game!",
                    false);
                return;
            }
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
            {
                BulldozeToolDetour.Deploy();
            }
            else if (mode == LoadMode.LoadAsset || mode == LoadMode.NewAsset)
            {
                var tsBar = UIView.Find("TSBar");
                if (tsBar != null)
                {
                    var bcButton = MakeButton(tsBar, "Bulldoze Ped. Connections");
                    bcButton.relativePosition = new Vector3(0, 0);
                    bcButton.eventClick +=
                    (comp, param) =>
                    {
                        Scripts.BulldozePedestrianConnections();
                    };
                    var seButton = MakeButton(tsBar, "Make All Segments Editable");
                    seButton.relativePosition = new Vector3(0, 26);
                    seButton.eventClick += (comp, param) =>
                    {
                        Scripts.MakeAllSegmentsEditable();
                    };
                }
                var pedestrianConnection = PrefabCollection<NetInfo>.FindLoaded("Pedestrian Connection");
                pedestrianConnection.m_class.m_layer = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
            }
            var locale = (Locale)typeof(LocaleManager).GetField("m_Locale", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(SingletonLite<LocaleManager>.instance);
            var field = typeof(PrefabInfo).GetField("m_UICategory", BindingFlags.Instance | BindingFlags.NonPublic);
            for (uint i = 0; i < PrefabCollection<NetInfo>.PrefabCount(); i++)
            {
                var info = PrefabCollection<NetInfo>.GetPrefab(i);
                if (info == null)
                {
                    continue;
                }
                var key = new Locale.Key { m_Identifier = "NET_TITLE", m_Key = info.name };
                if (!locale.Exists(key))
                {
                    locale.AddLocalizedString(key, info.name);
                }
                key = new Locale.Key { m_Identifier = "NET_DESC", m_Key = info.name };
                if (!locale.Exists(key))
                {
                    locale.AddLocalizedString(key, info.name);
                }
                var thumb = Util.LoadTextureFromAssembly($"{typeof(MoreNetworkStuff).Name}.resource.thumb.png", false);
                var tooltip = Util.LoadTextureFromAssembly($"{typeof(MoreNetworkStuff).Name}.resource.tooltip.png",
                    false);
                var atlas = Util.CreateAtlas(new[] { thumb, tooltip });
                if (ConnectionNetworks.Contains(info.name))
                {
                    info.m_Atlas = atlas;
                    info.m_Thumbnail = thumb.name;
                    info.m_InfoTooltipAtlas = atlas;
                    info.m_InfoTooltipThumbnail = tooltip.name;
                    info.m_maxHeight = 5;
                    info.m_minHeight = -5;
                }
                else if (mode == LoadMode.LoadAsset || mode == LoadMode.NewAsset)
                {
                    var category = (string)field.GetValue(info);
                    if (category == "LandscapingWaterStructures")
                    {
                        field.SetValue(info, "BeautificationPaths");
                    }
                }
            }
            switch (mode)
            {
                case LoadMode.NewGame:
                case LoadMode.LoadGame:
                    RefreshPanelInGame();
                    break;
            }

        }

        private static UIButton MakeButton(UIComponent component, string t)
        {
            UIButton b = (UIButton)component.AddUIComponent(typeof(UIButton));
            b.text = t;
            b.width = 200;
            b.height = 24;
            b.normalBgSprite = "ButtonMenu";
            b.disabledBgSprite = "ButtonMenuDisabled";
            b.hoveredBgSprite = "ButtonMenuHovered";
            b.focusedBgSprite = "ButtonMenuFocused";
            b.pressedBgSprite = "ButtonMenuPressed";
            b.textColor = new Color32(255, 255, 255, 255);
            b.disabledTextColor = new Color32(7, 7, 7, 255);
            b.hoveredTextColor = new Color32(7, 132, 255, 255);
            b.focusedTextColor = new Color32(255, 255, 255, 255);
            b.pressedTextColor = new Color32(30, 30, 44, 255);
            b.playAudioEvents = true;
            b.isTooltipLocalized = false;
            return b;
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            BulldozeToolDetour.Revert();
            var initializer = GameObject.Find("MoreNetworkStuffInitializer");
            if (initializer != null)
            {
                Object.Destroy(initializer);
            }
        }

        public override void OnReleased()
        {
            base.OnReleased();
            PanelsDetours.Revert();
            if (!IsHooked())
            {
                return;
            }
            TransportInfoHook.Revert();
            NetInfoHook.Revert();
        }

        private static bool IsHooked()
        {
            return Util.IsModActive("Prefab Hook");
        }

        private static void RefreshPanelInGame()
        {
            new GameObject("MoreNetworkStuffInitializer").AddComponent<Initializer>();
        }

        private class Initializer : MonoBehaviour
        {
            private void Update()
            {
                var pathsPanel = GameObject.Find("LandscapingPathsPanel");
                var beautificationPanel = pathsPanel?.GetComponent<BeautificationPanel>();
                beautificationPanel?.RefreshPanel();
                Destroy(this);
            }
        }
    }
}