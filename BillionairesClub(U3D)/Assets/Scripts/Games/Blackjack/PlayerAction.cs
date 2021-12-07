using System;
using UnityEngine;

namespace Blackjack
{
    public class PlayerAction : BeforeBet
    {
        [HideInInspector]
        public GameManager gameManager;
        [HideInInspector]
        public LabelController labelController;
        [HideInInspector]
        public TableController tableController;


        public void SetUp()
        {

        }
    }
}