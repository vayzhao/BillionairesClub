using System;
using UnityEngine;

namespace Blackjack
{
    public class TableController : WagerBehaviour
    {
        [HideInInspector]
        public GameManager gameManager;
        [HideInInspector]
        public PlayerAction playerAction;
        [HideInInspector]
        public LabelController labelController;

        public void Setup()
        {

        }

        
    }
}