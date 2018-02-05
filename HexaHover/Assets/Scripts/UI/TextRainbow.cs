using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TextRainbow : MonoBehaviour
{

    private Color[] Colors = { Color.red, Color.green, Color.yellow, Color.blue, Color.white, Color.cyan, Color.magenta };
    private int index = 0;
    private int previousIndex = 0;
    private float tColor = 0;
    [SerializeField]
    private float fadeTime = 2;
    // Use this for initialization
    void Start()
    {
        index = Random.Range(0, Colors.Length - 1);
        previousIndex = Random.Range(0, Colors.Length - 1);

    }

    // Update is called once per frame
    void Update()
    {
        if (tColor <= 1 && previousIndex!=index)
        {
            GetComponent<Outline>().effectColor = Color.Lerp(Colors[previousIndex], Colors[index], tColor);

            tColor += Time.deltaTime / fadeTime;
        }
        else
        {
            tColor = 0;
            previousIndex = index;
            index = Random.Range(0, Colors.Length - 1);
        }
    }
}