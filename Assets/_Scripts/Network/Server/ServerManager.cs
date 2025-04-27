using System.Collections.Generic;

namespace _Scripts.Network.Server
{
    public class ServerManager
    {
        public List<PlayerState> PlayerStates;
        

        public ServerManager()
        {
            PlayerStates = new List<PlayerState>
            {
                new("sosal?"),
                new("da!")
            };
        }
    }
}