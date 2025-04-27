namespace _Scripts.Network.Server
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