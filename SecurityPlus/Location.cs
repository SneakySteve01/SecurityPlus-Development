using Rage;

namespace SecurityPlus
{
    /*
     * This class never ended up being used, it's actually
     * a leftover from the old SecurityCallouts version
     * of the plugin. Feel free to revive it for yourself
     * if you'd like, or ignore it like me lol
     */
    public class Location
    {
        private Vector3 position;

        public enum locationType
        {
            STORES,
            GAS,
            AIR,
            FACILITY
        }

        public Location(locationType locationType)
        {
            
        }

        public Vector3 getPosition()
        {
            return position;
        }
    }
}