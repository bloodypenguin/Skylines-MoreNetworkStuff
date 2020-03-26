using System;
using System.Reflection;
using ColossalFramework;
using MoreNetworkStuff.Redirection;

namespace MoreNetworkStuff.Detours
{
    [TargetType(typeof(DefaultTool))]
    public class DefaultToolGameDetour
    {
       
          [RedirectMethod]
          public ToolBase.RaycastService GetService()
          {
            if ((Singleton<ToolManager>.instance.m_properties.m_mode & ItemClass.Availability.MapAndAsset) != ItemClass.Availability.None)
            {
              switch (Singleton<InfoManager>.instance.CurrentMode)
              {
                case InfoManager.InfoMode.Transport:
                  return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels | ItemClass.Layer.AirplanePaths | ItemClass.Layer.ShipPaths | ItemClass.Layer.Markers);
                case InfoManager.InfoMode.Traffic:
                case InfoManager.InfoMode.Tours:
                  return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels | ItemClass.Layer.Markers);
                case InfoManager.InfoMode.Underground:
                  return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.MetroTunnels);
                default:
                  return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.Default | ItemClass.Layer.Markers);
              }
            }
            else
            {
              InfoManager.InfoMode currentMode = Singleton<InfoManager>.instance.CurrentMode;
              switch (currentMode)
              {
                case InfoManager.InfoMode.TrafficRoutes:
                case InfoManager.InfoMode.Tours:
                  return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels);
                case InfoManager.InfoMode.Underground:
                  return Singleton<InfoManager>.instance.CurrentSubMode == InfoManager.SubInfoMode.Default ? new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.MetroTunnels) : new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.WaterPipes);
                case InfoManager.InfoMode.Fishing:
                  return new ToolBase.RaycastService(ItemClass.Service.Fishing, ItemClass.SubService.None, ItemClass.Layer.Default | ItemClass.Layer.FishingPaths);
                default:
                  if (currentMode != InfoManager.InfoMode.Water)
                  { 
                    //added: | ItemClass.Layer.AirplanePaths | ItemClass.Layer.ShipPaths
                    if (currentMode == InfoManager.InfoMode.Transport)
                      return new ToolBase.RaycastService(ItemClass.Service.PublicTransport, ItemClass.SubService.None, ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels | ItemClass.Layer.BlimpPaths | ItemClass.Layer.HelicopterPaths | ItemClass.Layer.FerryPaths | ItemClass.Layer.AirplanePaths | ItemClass.Layer.ShipPaths);
                    if (currentMode != InfoManager.InfoMode.Traffic)
                    {
                      if (currentMode != InfoManager.InfoMode.Heating)
                        return new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.Default);
                    }
                    else
                      goto case InfoManager.InfoMode.TrafficRoutes;
                  }
                  return new ToolBase.RaycastService(ItemClass.Service.Water, ItemClass.SubService.None, ItemClass.Layer.Default | ItemClass.Layer.WaterPipes);
              }
            }
          }
    }
}