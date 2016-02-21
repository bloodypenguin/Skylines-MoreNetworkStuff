using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using ColossalFramework.UI;
using ICities;
using MoreNetworkStuff.Detours;
using PrefabHook;
using UnityEngine;

namespace MoreNetworkStuff
{
    public class LoadingExtension : LoadingExtensionBase
    {
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
            if (info.name != "Airplane Path" && info.name != "Ship Path")
            {
                return;
            }
            info.m_availableIn = ItemClass.Availability.GameAndMap;
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
            {
                BulldozeToolDetour.Deploy();
                if (IsHooked())
                {
                    return;
                }
                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage(
                    "Missing dependency",
                    "'More Network Stuff' mod requires the 'Prefab Hook' mod to work properly. Please subscribe to the mod and restart the game!",
                    false);
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
                        Util.BulldozePedestrianConnections();
                    };
                    var seButton = MakeButton(tsBar, "Make All Segments Editable");
                    seButton.relativePosition = new Vector3(0, 26);
                    seButton.eventClick += (comp, param) =>
                    {
                        Util.MakeAllSegmentsEditable();
                    };
                }
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
    }
}