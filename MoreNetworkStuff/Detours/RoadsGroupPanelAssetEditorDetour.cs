using System;
using MoreNetworkStuff.Redirection;

namespace MoreNetworkStuff.Detours
{
    [TargetType(typeof(RoadsGroupPanel))]
    public class RoadsGroupPanelAssetEditorDetour : GeneratedGroupPanel
    {
        [RedirectMethod]
        protected override bool IsServiceValid(PrefabInfo info)
        {
            if (ToolsModifierControl.isMapEditor)
            {
                if (info.GetService() == this.service)
                    return true;
                return info.GetService() == ItemClass.Service.PublicTransport && info.GetSubService() != ItemClass.SubService.PublicTransportTours;
            }
            if (ToolsModifierControl.isAssetEditor)
            {
                BuildingInfo buildingInfo = info as BuildingInfo;
                //begin mod
                if (info.GetService() == this.service &&
                    ((Object) buildingInfo == (Object) null ||
                     buildingInfo.m_buildingAI.WorksAsNet()) || (info is NetInfo && (info.GetService() == ItemClass.Service.PublicTransport
                                                             || info.GetService() == ItemClass.Service.Fishing
                                                             || info.GetService() == ItemClass.Service.Monument)))
                {
                    return true;  
                }
                //end mod

                return info.GetService() == ItemClass.Service.Beautification && info is NetInfo && ToolsModifierControl.toolController.m_editPrefabInfo is NetInfo;
            }
            NetInfo netInfo = info as NetInfo;
            if (info.GetService() != this.service)
                return false;
            return (Object) netInfo == (Object) null || (netInfo.m_vehicleTypes & VehicleInfo.VehicleType.Car) != VehicleInfo.VehicleType.None;
        }
    }
}