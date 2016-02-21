using System;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Plugins;
using ICities;
using MoreNetworkStuff.Redirection;

namespace MoreNetworkStuff.Detours
{
    //here we do some manual detours
    public class BulldozeToolDetour
    {
        private static bool _deployed;
        private static RedirectCallsState _state;
        private static MethodInfo _originalInfo;
        private static MethodInfo _detourInfo;

        public static void Deploy()
        {
            if (_deployed) return;
            try
            {
                if (_originalInfo == null)
                {
                    _originalInfo = typeof(BulldozeTool).GetMethod("GetService");
                }
                var methodName = IsMoledozerActive() ? "GetServiceMoledozer" : "GetServiceVanilla";
                _detourInfo = typeof(BulldozeToolDetour).GetMethod(methodName);
                _state = RedirectionHelper.RedirectCalls(_originalInfo, _detourInfo);
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
                if (_originalInfo != null && _detourInfo != null)
                {
                    RedirectionHelper.RevertRedirect(_originalInfo, _state);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            _deployed = false;
        }

        public ToolBase.RaycastService GetServiceVanilla()
        {
            if ((Singleton<ToolManager>.instance.m_properties.m_mode & ItemClass.Availability.Editors) != ItemClass.Availability.None)
            {
                switch (Singleton<InfoManager>.instance.CurrentMode)
                {
                    case InfoManager.InfoMode.Transport:
                        return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels | ItemClass.Layer.AirplanePaths | ItemClass.Layer.ShipPaths | ItemClass.Layer.Markers);
                    case InfoManager.InfoMode.Traffic:
                        return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels | ItemClass.Layer.Markers);
                    default:
                        return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.Default | ItemClass.Layer.Markers);
                }
            }
            else
            {
                switch (Singleton<InfoManager>.instance.CurrentMode)
                {
                    case InfoManager.InfoMode.Water:
                    case InfoManager.InfoMode.Heating:
                        return new ToolBase.RaycastService(ItemClass.Service.Water, ItemClass.SubService.None, ItemClass.Layer.Default | ItemClass.Layer.WaterPipes);
                    case InfoManager.InfoMode.Transport:
                        //added: | ItemClass.Layer.AirplanePaths | ItemClass.Layer.ShipPaths
                        return new ToolBase.RaycastService(ItemClass.Service.PublicTransport, ItemClass.SubService.None, ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels | ItemClass.Layer.AirplanePaths | ItemClass.Layer.ShipPaths);
                    case InfoManager.InfoMode.Traffic:
                        return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels);
                    default:
                        return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.Default);
                }
            }
        }


        public ToolBase.RaycastService GetServiceMoledozer()
        {
            ItemClass.Layer trafficLayer = (this.GetType() != typeof(BulldozeTool)) ?
                ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels | ItemClass.Layer.Markers :
                ItemClass.Layer.MetroTunnels;

            ItemClass.Availability mode = Singleton<ToolManager>.instance.m_properties.m_mode;

            if ((mode & ItemClass.Availability.Editors) != ItemClass.Availability.None)
            {
                InfoManager.InfoMode currentMode = Singleton<InfoManager>.instance.CurrentMode;
                if (currentMode == InfoManager.InfoMode.Transport)
                {
                    return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None,
                        ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels | ItemClass.Layer.AirplanePaths | ItemClass.Layer.ShipPaths | ItemClass.Layer.Markers);
                }
                if (currentMode != InfoManager.InfoMode.Traffic)
                {
                    return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.Default | ItemClass.Layer.Markers);
                }
                return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, trafficLayer | ItemClass.Layer.Markers);
            }
            else
            {
                InfoManager.InfoMode currentMode = Singleton<InfoManager>.instance.CurrentMode;
                if (currentMode == InfoManager.InfoMode.Water || currentMode == InfoManager.InfoMode.Heating)
                {
                    return new ToolBase.RaycastService(ItemClass.Service.Water, ItemClass.SubService.None, ItemClass.Layer.Default | ItemClass.Layer.WaterPipes);
                }
                if (currentMode == InfoManager.InfoMode.Transport)
                {
                    //added: | ItemClass.Layer.AirplanePaths | ItemClass.Layer.ShipPaths
                    return new ToolBase.RaycastService(ItemClass.Service.PublicTransport, ItemClass.SubService.None, ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels | ItemClass.Layer.AirplanePaths | ItemClass.Layer.ShipPaths);
                }
                if (currentMode != InfoManager.InfoMode.Traffic)
                {
                    return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.Default);
                }
                return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, trafficLayer);
            }
        }

        private static bool IsMoledozerActive()
        {
            try
            {
                var plugins = PluginManager.instance.GetPluginsInfo();
                foreach (var name in from plugin in plugins.Where(p => p.isEnabled)
                                     select plugin.GetInstances<IUserMod>()
                                         into instances
                                     where instances.Any()
                                     select instances[0].Name into name
                                     where name == "Moledozer"
                                     select name)
                {
                    UnityEngine.Debug.Log($"MoreNetworkStuff: {name} is active");
                    return true;
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"MoreNetworkStuff: error while looking for moledozer: {e.Message}");
            }
            return false;
        }

    }
}