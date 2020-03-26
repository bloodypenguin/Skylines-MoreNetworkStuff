using ColossalFramework;
using MoreNetworkStuff.Redirection;

namespace MoreNetworkStuff.Detours
{
    [TargetType(typeof(GeneratedGroupPanel))]
    public class GeneratedGroupPanelAssetAndMapEditorDetour : GeneratedGroupPanel
    {
        [RedirectMethod]
        private bool IsPlacementRelevant(NetInfo info)
        {
            //begin mod
            return PanelsDetours.IsPlacementRelevant(info, isMapEditor, isGame, isAssetEditor);
            //end mod
        }
    }
}