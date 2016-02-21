using System;
using System.Linq;
using ICities;
using MoreNetworkStuff.Redirection;

namespace MoreNetworkStuff
{
    public class MoreNetworkStuff : IUserMod
    {

        private static bool _bootstrapped;

        public string Name
        {
            get
            {
                try
                {
                    if (!_bootstrapped)
                    {
                        var moledozerType = Util.FindType("MoledozerMod");
                        if (moledozerType != null)
                        {
                            UnityEngine.Debug.Log("MoreNetworkStuff - Moledozer detected!");
                            RedirectionHelper.RedirectCalls(moledozerType.GetMethod("OnLevelLoaded"),
                                typeof(LoadingExtensionBase).GetMethod("OnLevelLoaded"));
                            RedirectionHelper.RedirectCalls(moledozerType.GetMethod("OnLevelUnloading"),
                                typeof(LoadingExtensionBase).GetMethod("OnLevelUnloading"));
                        }
                        _bootstrapped = true;
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
                return "More Network Stuff";
            }
        }

        public string Description
        {
            get { return "Adds more network stuff to place"; }
        }
    }
}
