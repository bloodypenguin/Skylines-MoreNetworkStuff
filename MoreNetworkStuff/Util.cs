using System;
using System.Linq;
using UnityEngine;

namespace MoreNetworkStuff
{
    public static class Util
    {
        public static Type FindType(string className)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types.Where(type => type.Name == className))
                    {
                        return type;
                    }
                }
                catch
                {
                    // ignored
                }
            }
            return null;
        }


        public static void MakeAllSegmentsEditable()
        {
            // change this to true to make the bulldozer work for train station tracks and other networks
            bool makeAllNetworksEditable = true;

            var mgr = NetManager.instance;
            for (var i = 0; i < mgr.m_segments.m_size; i++)
            {
                if (mgr.m_segments.m_buffer[i].m_flags == NetSegment.Flags.None) continue;

                Debug.Log("Segment " + i + " -  Type: " + mgr.m_segments.m_buffer[i].Info.name + ", Length: " + mgr.m_segments.m_buffer[i].m_averageLength);
                if (makeAllNetworksEditable) mgr.m_segments.m_buffer[i].m_flags &= ~NetSegment.Flags.Untouchable;
            }
        }

        public static void BulldozePedestrianConnections()
        {
            var mgr = NetManager.instance;
            for (ushort i = 0; i < mgr.m_segments.m_size; i++)
            {
                var netSegment = mgr.m_segments.m_buffer[i];
                if (netSegment.m_flags == NetSegment.Flags.None) continue;

                var name = netSegment.Info.name;
                if (name.Contains("Pedestrian Connection")|| name == "Cargo Connection" ||
                    name == "Ship Dock" || name == "Ship Dockway" /*|| name == "Bus Station Stop" || name == "Bus Station Way"*/)
                {
                    mgr.ReleaseSegment(i, false);
                }
            }
        }
    }
}