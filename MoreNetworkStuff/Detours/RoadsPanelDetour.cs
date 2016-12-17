using MoreNetworkStuff.Redirection;

namespace MoreNetworkStuff.Detours
{

    [TargetType(typeof(RoadsPanel))]
    public class RoadsPanelDetour : GeneratedScrollPanel
    {
        [RedirectMethod]
        protected override bool IsPlacementRelevant(NetInfo info)
        {
            return PanelsDetours.IsPlacementRelevant(info, isMapEditor, isGame, isAssetEditor);
        }

        [RedirectMethod]
        protected override bool IsPlacementRelevant(BuildingInfo info)
        {
            if (info.GetService() == ItemClass.Service.Residential)
            {
                return true;
            }
            return base.IsPlacementRelevant(info);
        }

        //ignored. Added to satisfy compiler
        public override ItemClass.Service service { get; }
    }
}