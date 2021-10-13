using System;
using System.Collections.Generic;
using UnityEngine;

public class CardModel : MonoBehaviour
{
    public MeshFilter meshFilter;
    public void Start()
    {
        meshFilter.mesh = Blackboard.GetCardMesh(2);
    }
}