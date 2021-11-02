using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Archive : MonoBehaviour
{
    public GameObject empty;
    public GameObject exist;

    public Image protraitImg;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI resourceText;

    [HideInInspector]
    public Image img;

    void Start()
    {
        img = GetComponent<Image>();
    }


}