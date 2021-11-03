using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Archive
{
    public class PanelManager : MonoBehaviour
    {
        public SceneType sceneType;

        public Sprite normalSprite;
        public Sprite selectedSprite;

        public Panel[] archives;

        public Button deleteButton;
        public Button saveButton;
        public Button loadButton;

        private int selectIndex;

        public UIManager uiManager;

        [Header("Notification Panel")]
        public GameObject notification;
        public CharacterSelection characterSelection;

        private void Start()
        {
            ResetSelection();
            InitializeArchivePanels();
        }

        /// <summary>
        /// Method to pre-load all the archive and initialize the archive panels
        /// </summary>
        void InitializeArchivePanels()
        {
            // get the max quantity of archive slot
            var maxSlot = archives.Length;
            for (int i = 0; i < maxSlot; i++)
            {
                // try to obtain the 'n' archive file
                var data = Formatter.Load(i);

                // check to see if the data exists
                if (data != null)
                    // if the data exists, copy values from the archive data and 
                    // paste them onto UI elements in the archive panel
                    archives[i].UpdatePanel(data);
                else
                    // otherwise, clear the archive panel
                    archives[i].ClearPanel();
            }
        }

        /// <summary>
        /// Method to call when the close button of the panel manager
        /// is clicked ** only function when the user is in the Casino Scene
        /// </summary>
        public void OnESC()
        {
            // return if the notification panel is displayed
            if (notification.activeSelf)
                return;

            // otherwise close a page
            uiManager.ClosePage();
        }

        /// <summary>
        /// Method to cancel selection and switch off all buttons,
        /// it is called at the start of the game
        /// </summary>
        public void ResetSelection()
        {
            // cancel selection
            selectIndex = -1;

            // switch off buttons
            deleteButton.Switch(false);
            saveButton.Switch(false);
            loadButton.Switch(false);

            // cancel highlight for all panels
            for (int i = 0; i < archives.Length; i++)
                archives[i].backGroundImg.sprite = normalSprite;
        }

        /// <summary>
        /// Method to be called when the user select an archive panel from the UI
        /// </summary>
        /// <param name="selectIndex">index of the selected panel</param>
        public void SelectAnArchive(int selectIndex)
        {
            // deselect the previous selection if it exists
            if (this.selectIndex != -1)
                archives[this.selectIndex].backGroundImg.sprite = normalSprite; 

            // update selection index
            this.selectIndex = selectIndex;

            // determine whether or not the selected panel has record
            var hasRecord = archives[selectIndex].HasRecord();

            // change sprite for the selected archive
            archives[selectIndex].backGroundImg.sprite = selectedSprite;

            // switch availability of the delete button
            deleteButton.Switch(hasRecord);

            // switch avilability of the save button
            saveButton.Switch(true);

            // if the user is currently in homepage scene, modify the 
            // availability of the load button, based on the archive panel state
            if (sceneType == SceneType.Homepage)
                loadButton.Switch(hasRecord);
        }

        /// <summary>
        /// Method to remove a saved archive file
        /// </summary>
        public void DeleteArchive()
        {
            // remove the selected archive file
            Formatter.Remove(selectIndex);

            // switch off the delete button
            deleteButton.Switch(false);

            // refresh the selected archive panel
            archives[selectIndex].ClearPanel();
        }

        /// <summary>
        /// Method to save an archive file
        /// </summary>
        public void SaveArchive()
        {
            // save the player's information into disk
            Formatter.Save(Blackboard.localPlayer, selectIndex);

            // update the archive panel
            archives[selectIndex].UpdatePanel(Formatter.Load(selectIndex));

            // pop up the notification
            uiManager.CreatePageAdditive(notification);
        }

        /// <summary>
        /// Method to load an archive file
        /// </summary>
        public void LoadArchive()
        {
            Data data = Formatter.Load(selectIndex);


            characterSelection.ReadyFromLoad(data);

        }
    }
}

