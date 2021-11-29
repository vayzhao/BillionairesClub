using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChipSprite: MonoBehaviour
{
    [Tooltip("Value of this chip")]
    public int value = 5;

    [Tooltip("Sprite to display when the button is disabled")]
    public Sprite disableSprite;
    private Sprite defaultSprite;       // sprite used for the image

    [Tooltip("Gameobject to display when the user is dragging a chip")]
    public GameObject pref_drag;
    private GameObject obj_drag;        // the actual gameobject 

    private bool isOn;                  // determine whether or not this button is functioning
    private Image image;                // image used for this chip 
    private EventTrigger triggers;      // a component that triggers functions based on user's input
    private BeforeBet manager; // a component that handles all the drag and drop functions

    void Start()
    {
        InitializeSprite();
        RegisterEvents();        
    }

    /// <summary>
    /// Method to initialize the chip sprite
    /// </summary>
    void InitializeSprite()
    {
        // find image component for this game object, and define its default sprite
        image = GetComponent<Image>();
        defaultSprite = image.sprite;

        // find the drag and drop manager
        manager = GetComponentInParent<BeforeBet>();

        // turn on the button
        Enable(true);
        Normal(null);
    }

    /// <summary>
    /// Method to add methods to its trigger 
    /// </summary>
    void RegisterEvents()
    {
        // find trigger component on for this game object
        triggers = GetComponent<EventTrigger>();

        // clear all the registered methods
        triggers.triggers.Clear();

        // register functions
        triggers.RegisterEventMethod(EventTriggerType.PointerEnter, Highlight);
        triggers.RegisterEventMethod(EventTriggerType.PointerExit, Normal);
        triggers.RegisterEventMethod(EventTriggerType.BeginDrag, BeginDrag);
        triggers.RegisterEventMethod(EventTriggerType.Drag, Drag);
        triggers.RegisterEventMethod(EventTriggerType.EndDrag, EndDrag);
        triggers.RegisterEventMethod(EventTriggerType.PointerDown, PointerDown);
        triggers.RegisterEventMethod(EventTriggerType.PointerUp, Highlight);
    }


    /// <summary>
    /// Method to activate or deactivate the button
    /// </summary>
    /// <param name="flag"></param>
    public void Enable(bool flag)
    {
        isOn = flag;
        image.sprite = flag ? defaultSprite : disableSprite;
    }

    /// <summary>
    /// Methods for the drag and drop function of the chip
    /// 1. Spawns a clone chip sprite at the mouse position when 
    /// the user starts dragging the chip
    /// 2. Adjust the clone chip sprite position to the pointer position
    /// everyframe 
    /// 3. Remove the clone chip sprite when the user release dragging
    /// </summary>
    void BeginDrag(PointerEventData data)
    {
        // return if the chip is not activated or skip dragging is on
        if (!isOn || manager.isSkipDrag.isOn)
            return;

        // spawn the clone chip sprite and update its image
        obj_drag = Instantiate(pref_drag, transform);
        obj_drag.GetComponent<Image>().sprite = defaultSprite;
    }
    void Drag(PointerEventData data)
    {
        // return if the chip is not activated or skip dragging is on
        if (!isOn || manager.isSkipDrag.isOn)
            return;

        // adjust clone chip sprite's position
        obj_drag.transform.position = data.position;
    }
    void EndDrag(PointerEventData data)
    {
        // return if the chip is not activated or skip dragging is on
        if (!isOn || manager.isSkipDrag.isOn)
            return;

        // destroy the clone chip
        Destroy(obj_drag.gameObject);

        // return if the mouse is hovering on other ui components
        foreach (var obj in data.hovered)
        {
            if (obj.layer.Equals(LayerMask.NameToLayer("UI")))
                return;
        }

        // add chip
        manager.AddChip(value);
    }

    /// <summary>
    /// Method to immediate add chip, this only function when skip dragging
    /// is on
    /// </summary>
    void PointerDown(PointerEventData data)
    {
        // retrn if the chip is not activated or dragging is off
        if (!isOn || !manager.isSkipDrag.isOn)
            return;

        // return if the pointer button is not left click
        if (data.button.ToString() != "Left")
            return;

        // add chip
        manager.AddChip(value);

        // play normal sprite
        Normal(data);
    }

    /// <summary>
    /// Method to highlight / reset the chip sprite color when the 
    /// curosr is entering / exiting the chip image
    /// </summary>
    void Highlight(PointerEventData data) => image.color = new Color(1f, 1f, 1f, 1f);
    void Normal(PointerEventData data) => image.color = new Color(1f, 1f, 1f, 0.75f);

    
}