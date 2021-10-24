using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StandUp : MonoBehaviour
{
    private void Start()
    {
        RegisterButton();
    }


    public void RegisterButton()
    {
        var btn = this.gameObject.GetComponent<Button>();

        btn.onClick.AddListener(() => Leave());
    }

    private void Leave()
    {
        FindObjectOfType<Loading>().LoadBackToCasino();
    }
}