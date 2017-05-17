using MoreNetworkStuff.Redirection;

namespace MoreNetworkStuff.Detours
{
    [TargetType(typeof(RoadsGroupPanel))]
    public class RoadsGroupPanelDetour : GeneratedGroupPanel
    {
        [RedirectMethod]
        protected override bool IsServiceValid(PrefabInfo info)
        {
            if (info.GetService() == ItemClass.Service.Residential)
            {
                return true;
            }

            if (info.GetService() == this.service || this.isMapEditor &&
                (info.GetService() == ItemClass.Service.PublicTransport/* || info.GetService() == ItemClass.Service.Beautification*/))
                return true;
            if (this.isAssetEditor && info.GetService() == ItemClass.Service.PublicTransport)
                return info.GetSubService() == ItemClass.SubService.PublicTransportTrain || info.GetSubService() == ItemClass.SubService.PublicTransportMetro || info.GetSubService() == ItemClass.SubService.PublicTransportMonorail || info.GetSubService() == ItemClass.SubService.None;
            return false;
        }
    }
}