using ColossalFramework;
using MoreNetworkStuff.Redirection;

namespace MoreNetworkStuff.Detours
{

    [TargetType(typeof(RoadsPanel))]
    public class RoadsPanelAssetAndMapEditorDetour : GeneratedScrollPanel
    {
        [RedirectMethod]
        protected override bool IsPlacementRelevant(NetInfo info)
        {
            return PanelsDetours.IsPlacementRelevant(info, isMapEditor, isGame, isAssetEditor);
        }

        [RedirectMethod]
        protected override bool IsPlacementRelevant(BuildingInfo info)
        {
            bool flag = true;
            //begin mod
            flag &= (info.m_paths != null);
            //end mod
            if (ToolsModifierControl.isAssetEditor)
                flag &= info.m_buildingAI.WorksAsNet();
            return base.IsPlacementRelevant(info) && flag;
        }
        
        [RedirectMethod]
        protected override bool IsServiceValid(NetInfo info)
        {
            if (ToolsModifierControl.isAssetEditor)
            {
                if ((UnityEngine.Object) info == (UnityEngine.Object) ToolsModifierControl.toolController.m_editPrefabInfo)
                    return false;
                //begin mod
                return info.GetService() == this.service || info.GetService() == ItemClass.Service.PublicTransport || info.GetService() == ItemClass.Service.Beautification || info.GetService() == ItemClass.Service.Fishing || info.GetService() == ItemClass.Service.Monument;
                //end mod
            }
            return ToolsModifierControl.isMapEditor ? info.GetService() == this.service || info.GetService() == ItemClass.Service.PublicTransport : info.GetService() == this.service && (info.m_vehicleTypes & VehicleInfo.VehicleType.Car) != VehicleInfo.VehicleType.None;
        }
        
        //ignored. Added to satisfy compiler
        public override ItemClass.Service service { get; }
    }
}