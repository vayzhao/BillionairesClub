using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public static class ExtensionMethods
{
    #region Random
    /// <summary>
    /// Method to choose a non-duplicate element from a list,
    /// and reset the list when it is empty
    /// </summary>
    /// <param name="list">the list of elements</param>
    /// <param name="min">minimum value</param>
    /// <param name="max">maximum value</param>
    /// <returns></returns>
    public static int GetNonDuplicateInt(this List<int> list, int min, int max)
    {
        // reset the list when running out of elements
        if (list.Count == 0)
            for (int i = min; i < max; i++)
                list.Add(i);

        // pop a random result from the list 
        var result = list[UnityEngine.Random.Range(0, list.Count)];
        list.Remove(result);

        return result;
    }
    #endregion

    #region Cards
    public static void SetCard(this GameObject cardGameObj, Card cardInfo)
    {
        cardGameObj.SetActive(true);

        var mesh = cardGameObj.GetComponent<MeshFilter>();
        mesh.mesh = Blackboard.GetCardMesh(cardInfo.GetCardIndex());
    }
    public static void HideCard(this GameObject cardGameObj)
    {
        for (int i = 0; i < cardGameObj.transform.childCount; i++) 
        {
            cardGameObj.transform.GetChild(i).localEulerAngles = Vector3.zero;
        }
        cardGameObj.SetActive(false);
    }
    public static void ShowCard(this GameObject cardGameObj)
    {
        for (int i = 0; i < cardGameObj.transform.childCount; i++)
        {
            cardGameObj.transform.GetChild(i).localEulerAngles = Vector3.forward * 180f;
        }
        cardGameObj.SetActive(true);
    }

    public static string GetName(this Value value, bool isShorten)
    {
        switch (value)
        {
            case Value.JACK:
                return isShorten ? "J" : "Jack";
            case Value.QUEEN:
                return isShorten ? "Q" : "Queen";
            case Value.KING:
                return isShorten ? "K" : "King"; 
            case Value.ACE:
                return isShorten ? "A" : "Ace"; 
            default:
                return $"{(int)value + 2}";
        }
    }
    public static string GetName(this Value value) { return value.GetName(false); }
    public static string GetName(this Rank rank)
    {
        switch (rank)
        {
            case Rank.OnePair:
                return $"OnePair";
            case Rank.TwoPairs:
                return $"TwoPair";
            case Rank.ThreeOfAKind:
                return $"Trips";
            case Rank.Straight:
                return $"Straight";
            case Rank.Flush:
                return $"Flush";
            case Rank.FullHouse:
                return $"FullHouse";
            case Rank.FourOfAKind:
                return $"Quads";
            case Rank.StraightFlush:
                return $"StraightFlush";
            case Rank.RoyalFlush:
                return $"Royal StraightFlush";
            default:
                return $"High-hand";
        }
    }
    #endregion

    #region UnityEngine.UI
    public static void Switch(this Button btn, bool flag)
    {
        // enable / disable the button
        btn.enabled = flag;

        // switch button's image sprite
        btn.image.sprite = flag ?
            btn.spriteState.pressedSprite :
            btn.spriteState.disabledSprite;
    }
    public static void RegisterEventMethod(this EventTrigger trg, EventTriggerType type, Action<PointerEventData> method)
    {
        // create an entry that invokes the event method
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(data => method.Invoke((PointerEventData)data));

        // add the entry into the trigger
        trg.triggers.Add(entry);
    }
    #endregion

}