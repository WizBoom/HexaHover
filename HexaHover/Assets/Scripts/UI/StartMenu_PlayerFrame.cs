using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class StartMenu_PlayerFrame : MonoBehaviour {
    public bool IsActivated = false;
    [SerializeField]
    private Texture[] _faces;
    private float _faceTime = 0;
    private float _faceTimer = 0.75f;

    [SerializeField]
    Text _joinText;
    [SerializeField]
    Image _Image;
    [SerializeField]
    RawImage _rawImage;
    [SerializeField]
    Text _ColorText;

    // Use this for initialization
    void Start () {
        TogglePlayerActivated();
        TogglePlayerActivated();
    }

    // Update is called once per frame
    void Update () {
        _faceTime += Time.deltaTime;
        if (_faceTime>_faceTimer)
        {
            _faceTime = 0;
            _faceTimer = Random.Range(0.5f, 0.8f);
            SetFace(Random.Range(0, _faces.Length));
        }


	}

    public void SetColor(Color color)
    {
        _Image.GetComponent<Image>().color = color;
        _rawImage.GetComponent<Outline>().effectColor = color;

    }

    public void TogglePlayerActivated()
    {
        if (IsActivated)
        {
            IsActivated = false;
            GetComponentInChildren<Text>().text = GetComponentInChildren<Text>().text.Substring(0,11) + "join";
            GetComponentInChildren<Text>().GetComponentInChildren<Outline>().effectColor = Color.green;
            _Image.enabled = false;
            _rawImage.enabled = false;
            _ColorText.enabled = false;
        }
        else
        {
            IsActivated = true;
            GetComponentInChildren<Text>().text = GetComponentInChildren<Text>().text.Substring(0, 11) + "leave";
            GetComponentInChildren<Text>().GetComponentInChildren<Outline>().effectColor = Color.red;
            _Image.enabled = true;
            _rawImage.enabled = true;
            _ColorText.enabled = true;
        }
    }

    private void SetFace(int index)
    {
        GetComponentInChildren<RawImage>().texture = _faces[index];
    }

}
