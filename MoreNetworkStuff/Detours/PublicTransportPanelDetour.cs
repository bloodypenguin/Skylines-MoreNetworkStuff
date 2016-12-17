using MoreNetworkStuff.Redirection;

namespace MoreNetworkStuff.Detours
{
    [TargetType(typeof(PublicTransportPanel))]
    public class PublicTransportPanelDetour : GeneratedScrollPanel
    {

        [RedirectMethod]
        private bool IsRoadEligibleToPublicTransport(NetInfo info)
        {
            if (this.category == "PublicTransportTrain" && (info.m_vehicleTypes & VehicleInfo.VehicleType.Train) != VehicleInfo.VehicleType.None || this.category == "PublicTransportMetro" && (info.m_vehicleTypes & VehicleInfo.VehicleType.Metro) != VehicleInfo.VehicleType.None || this.category == "PublicTransportBus" && (info.m_laneTypes & NetInfo.LaneType.TransportVehicle) != NetInfo.LaneType.None)
                return true;
            if (this.category == "PublicTransportTram")
                return (info.m_vehicleTypes & VehicleInfo.VehicleType.Tram) != VehicleInfo.VehicleType.None;
            //begin mod
            if (this.category == "PublicTransportPlane")
                return (info.m_vehicleTypes & VehicleInfo.VehicleType.Plane) != VehicleInfo.VehicleType.None;
            if (this.category == "PublicTransportShip")
                return (info.m_vehicleTypes & VehicleInfo.VehicleType.Ship) != VehicleInfo.VehicleType.None;
            //end mod
            return false;
        }

        //ignored. Added to satisfy compiler
        public override ItemClass.Service service { get; }
    }
}