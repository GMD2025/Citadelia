using System.Collections.Generic;
using _Scripts.UI;

namespace _Scripts.Server
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