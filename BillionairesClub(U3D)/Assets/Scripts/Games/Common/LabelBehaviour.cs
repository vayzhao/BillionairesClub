using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LabelBehaviour : MonoBehaviour
{
    [Header("Sprite Asset")]
    [Tooltip("A panel background sprite with red color")]
    public Sprite labelSpriteRed;
    [Tooltip("A panel background sprite with green color")]
    public Sprite labelSpriteGreen;
    [Tooltip("A panel background sprite with purple color")]
    public Sprite labelSpritePurple;
    [Tooltip("Default sprite for the card back")]
    public Sprite defaultTexture;

    /// <summary>
    /// A method to spawn a text mesh pro object displaying a message 
    /// to players, and then constantly moving upward and fading out
    /// </summary>
    /// <param name="str">text display to players</param>
    /// <param name="pos">where the text is spawned</param>
    /// <param name="size">size of the text</param>
    /// <param name="duration">display time</param>
    /// <param name="speed">speed for upward movement</param>
    public void FloatText(string str, Vector3 pos, float size, float duration, float speed)
    {
        // first of all, create an empty game object in canvans
        // and then add text mesh pro component onto it
        var obj = Instantiate(new GameObject(), Blackboard.canvas);
        var tmp = obj.AddComponent<TextMeshProUGUI>();

        // modify the text
        tmp.text = str;
        tmp.fontSize = size;
        tmp.enableWordWrapping = false;

        // setup text's initial position
        obj.transform.position = pos;

        // start the movement coroutine
        StartCoroutine(FloatTextMovement(tmp, duration, speed));
    }
    IEnumerator FloatTextMovement(TextMeshProUGUI tmp, float duration, float speed)
    {
        // set up a timer
        var timer = 0f;

        // playing text's movement
        while (duration > 0f)
        {
            // update timer and reduce duration
            timer += Time.deltaTime;
            duration -= Time.deltaTime;

            // update position & color
            tmp.transform.position += Vector3.up * speed;
            tmp.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, timer / duration));

            yield return new WaitForSeconds(Time.deltaTime);
        }

        // destroy the text
        Destroy(tmp.gameObject);
    }
}