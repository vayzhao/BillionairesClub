using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blackjack
{
    public class GameManager : MonoBehaviour
    {
        [HideInInspector]
        public Player[] players;                 // data for all players
                
        private UIManager uiManager;             // a script that handles all the UI components
        private PlayerAction playerAction;       // a script that handles betting decision in the game
        private TableInformation tableInfo;      // a script that handles information of the players
        private TableController tableController; // a script that handles all the 3D models used on the table
        private LabelController labelController; // a script that handles all the UI-text objects in the scene

        /// <summary>
        /// Method to setup the game manager for blackjack
        /// </summary>
        /// <param name="canvas"></param>
        public void Setup(Transform canvas)
        {
            // find ui manager from the canvas
            uiManager = canvas.GetComponentInChildren<UIManager>();

            // create game object for player action
            playerAction = uiManager.playerAction.GetComponent<PlayerAction>();

            // find table information
            tableInfo = FindObjectOfType<TableInformation>();
            tableController = FindObjectOfType<TableController>();

            // create UI object to display local player's hand
            labelController = uiManager.labelController.GetComponent<LabelController>();
            labelController.Setup();

            // bind relative script to each other
            tableController.gameManager = this;
            tableController.playerAction = playerAction;
            tableController.labelController = labelController;
            playerAction.gameManager = this;
            playerAction.labelController = labelController;
            playerAction.tableController = tableController;

            // register clicking event for ready button
            uiManager.readyBtn.onClick.AddListener(() => GameStart());

            // set player action & ready button as interactable object
            uiManager.interactable.Add(playerAction.gameObject);
            uiManager.interactable.Add(uiManager.readyBtn.gameObject);
        }

        /// <summary>
        /// Method to initialize all components and get ready to start the game
        /// </summary>
        public void FinishedLoading()
        {
            // access player data
            players = tableInfo.players;

            // method to run start method for playerAction & tableController
            playerAction.SetUp();
            tableController.Setup();
        }

        /// <summary>
        /// Method to start the game officially
        /// </summary>
        void GameStart()
        {
            // remove ready button from the interactable object list and destroy it
            uiManager.interactable.Remove(uiManager.readyBtn.gameObject);
            Destroy(uiManager.readyBtn.gameObject);

            // display labels on the table
            labelController.DisplayTableLabel();
        }
    }
}

