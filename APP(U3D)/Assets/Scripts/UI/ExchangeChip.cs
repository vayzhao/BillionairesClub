using System;
using UnityEngine;
using UnityEngine.UI;

public class ExchangeChip: MonoBehaviour
{
    public UIManager uiManager;
    public GameObject window;
    public PortalTagManager portals;

    public void PopUp()
    {
        Blackboard.lockMovement = true;
        Blackboard.lockRotation = true;
        Blackboard.LockCursor(false);

        uiManager.CreatePage(window);

        
    }

    public void Close()
    {
        Blackboard.lockMovement = false;
        Blackboard.lockRotation = false;
        Blackboard.LockCursor(true);
        portals.Resume();

    }

}