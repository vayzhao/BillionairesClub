using System;

namespace Archive
{
    [Serializable]
    public class Data
    {
        public int chip;            // amount of poker chip the player had
        public int gem;             // amount of gem the player had
        public int modelIndex;      // model index for player's character

        public string name;         // name of the player
        public string description;  // description of the player
        public string dateString;   // date when this data is stored
       
        public Data(Player player)
        {
            gem = player.gem;
            chip = player.chip;
            name = player.name;
            modelIndex = player.modelIndex;
            description = player.description;
            dateString = DateTime.Now.ToString("MM/dd/yyyy");
        }
    }
}

