using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Archive
{
    public class Panel : MonoBehaviour
    {
        [Header("Panels")]
        [Tooltip("Panel to display when this slot does not have any archive record")]
        public GameObject empty;
        [Tooltip("Panel to display when this slot has an archive record")]
        public GameObject exist;

        [Header("UI elements")]
        [Tooltip("Image to display player's protrait sprite")]
        public Image protraitImg;
        [Tooltip("Image to display the panel border")]
        public Image backGroundImg;
        [Tooltip("Text to display player's name")]
        public TextMeshProUGUI nameText;
        [Tooltip("Text to display archive saved date")]
        public TextMeshProUGUI dateText;
        [Tooltip("Text to display player's chip & gem")]
        public TextMeshProUGUI resourceText;

        /// <summary>
        /// Method to determine whether or not this panel has archive record on it
        /// </summary>
        /// <returns></returns>
        public bool HasRecord() => exist.activeSelf;

        /// <summary>
        /// Method to hide the exist panel and display the empty panel
        /// </summary>
        public void ClearPanel()
        {
            empty.SetActive(true);
            exist.SetActive(false);            
        }

        /// <summary>
        /// Method to display exist panel and paste data onto the panel
        /// </summary>
        /// <param name="data">archive data</param>
        public void UpdatePanel(Data data)
        {
            // hide the empty panel and display the exist panel
            exist.SetActive(true);
            empty.SetActive(false);

            // collect archive data and paste them on the UI elements
            nameText.text = data.name;
            dateText.text = data.dateString;
            resourceText.text = $"  <sprite name=\"Chip\">{data.chip} /  <sprite name=\"Gem\"> {data.gem}";
            protraitImg.sprite = Blackboard.GetPortraitPrefab(data.modelIndex);
        }
    }
}
