using System;
using System.Linq;
using ICities;
using MoreNetworkStuff.Redirection;

namespace MoreNetworkStuff
{
    public class MoreNetworkStuff : IUserMod
    {

        private static bool _bootstrapped;

        public string Name => "More Network Stuff";

        public string Description => "Adds more network stuff to place";
    }
}
