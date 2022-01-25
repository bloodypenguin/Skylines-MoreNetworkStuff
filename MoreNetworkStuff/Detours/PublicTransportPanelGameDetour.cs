using System.Collections.Generic;
using MoreNetworkStuff.Redirection;

namespace MoreNetworkStuff.Detours
{
    [TargetType(typeof(PublicTransportPanel))]
    public class PublicTransportPanelGameDetour : GeneratedScrollPanel
    {
      private static Dictionary<string, int> switchMap; //compiler generated in the original
      
        [RedirectMethod]
        private bool IsRoadEligibleToPublicTransport(NetInfo info)
          {
            string category = this.category;
            if (category != null)
            {
              if (switchMap == null)
              {
                switchMap = new Dictionary<string, int>(9)
                {
                  {
                    "PublicTransportTrain",
                    0
                  },
                  {
                    "PublicTransportMetro",
                    1
                  },
                  {
                    "PublicTransportBus",
                    2
                  },
                  {
                    "PublicTransportTram",
                    3
                  },
                  {
                    "PublicTransportShip",
                    4
                  },
                  {
                    "PublicTransportPlane",
                    5
                  },
                  {
                    "PublicTransportMonorail",
                    6
                  },
                  {
                    "PublicTransportCableCar",
                    7
                  },
                  {
                    "PublicTransportTrolleybus",
                    8
                  }
                };
              }
              int num;

              if (switchMap.TryGetValue(category, out num))
              {
                switch (num)
                {
                  case 0:
                    return (info.m_vehicleTypes & VehicleInfo.VehicleType.Train) != VehicleInfo.VehicleType.None && (info.m_dlcRequired & SteamHelper.DLC_BitMask.ModderPack8) == SteamHelper.DLC_BitMask.None;
                  case 1:
                    return (info.m_vehicleTypes & VehicleInfo.VehicleType.Metro) != VehicleInfo.VehicleType.None;
                  case 2:
                    return (info.m_laneTypes & NetInfo.LaneType.TransportVehicle) != NetInfo.LaneType.None;
                  case 3:
                    return (info.m_vehicleTypes & VehicleInfo.VehicleType.Tram) != VehicleInfo.VehicleType.None;
                  case 4:
                    //begin mod
                    return (info.m_vehicleTypes & (VehicleInfo.VehicleType.Ferry | VehicleInfo.VehicleType.Ship)) != VehicleInfo.VehicleType.None;
                    //end mod
                  case 5:
                    //begin mod
                    return (info.m_vehicleTypes & (VehicleInfo.VehicleType.Helicopter | VehicleInfo.VehicleType.Blimp | VehicleInfo.VehicleType.Plane)) != VehicleInfo.VehicleType.None;
                    //end mod
                  case 6:
                    return info.m_dlcRequired == SteamHelper.DLC_BitMask.AirportDLC;
                  case 7:
                    return (info.m_vehicleTypes & VehicleInfo.VehicleType.Monorail) != VehicleInfo.VehicleType.None;
                  case 8:
                    return (info.m_vehicleTypes & VehicleInfo.VehicleType.CableCar) != VehicleInfo.VehicleType.None;
                  case 9:
                    return (info.m_vehicleTypes & VehicleInfo.VehicleType.Trolleybus) != VehicleInfo.VehicleType.None;
                  case 10:
                    return (info.m_vehicleTypes & VehicleInfo.VehicleType.Train) != VehicleInfo.VehicleType.None && (info.m_dlcRequired & SteamHelper.DLC_BitMask.ModderPack8) != SteamHelper.DLC_BitMask.None;
                }
              }
            }
            return false;
          }
        

        //ignored. Added to satisfy compiler
        public override ItemClass.Service service { get; }
    }
}