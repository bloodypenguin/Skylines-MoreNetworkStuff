using MoreNetworkStuff.Redirection;

namespace MoreNetworkStuff.Detours
{
    [TargetType(typeof(GeneratedGroupPanel))]
    public class GeneratedGroupPanelDetour : GeneratedGroupPanel
    {
        [RedirectMethod]
        private bool IsPlacementRelevant(NetInfo info)
        {
            return PanelsDetours.IsPlacementRelevant(info, isMapEditor, isGame, isAssetEditor);
        }
    }
}