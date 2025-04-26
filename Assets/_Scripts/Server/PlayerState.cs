using _Scripts.ResourceSystem;
using _Scripts.UI;

namespace _Scripts.Server.Data
{
    public class PlayerState
    {
        private string name;

        private GridState gridState;
        // other meta-data like avatar, rank and other state could be stored here
        
        public PlayerState(string name)
        {
            this.name = name;
            this.gridState = new GridState();
        }
    }
}