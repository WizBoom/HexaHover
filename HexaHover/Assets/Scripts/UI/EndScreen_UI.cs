using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen_UI : MonoBehaviour
{
    public GameObject Prefab;
    public Vector2 Offset;

    private List<Score_UI.PanelInfo> _panels = new List<Score_UI.PanelInfo>();

    void Start()
    {
        GetComponent<Canvas>().enabled = false;
    }

    public static int Compare(GameManager.PlayerInfo a, GameManager.PlayerInfo b)
    {
        return a.Score - b.Score;
    }

    public void SpawnEndcreen()
    {
        //GameManager.Players.Sort(Compare);
        var players = GameManager.Players;
        players.Sort(Compare);
        players.Reverse();

        GetComponent<Canvas>().enabled = true;
        FindObjectOfType<Score_UI>().gameObject.GetComponent<Canvas>().enabled = false;

        float panelHeight = (Prefab.GetComponent<RectTransform>().rect.height);
        float panelWidth = (Prefab.GetComponent<RectTransform>().rect.width / 2);

        foreach (GameManager.PlayerInfo p in players)
        {
            GameObject panel = Instantiate(Prefab);
            panel.transform.parent = this.transform;
            panel.GetComponent<Image>().color = p.Color;
            Score_UI.PanelInfo pi;
            pi.PlayerNumber = p.Number;
            pi.Panel = panel;
            _panels.Add(pi);
            UpdateScore(p.Number, p.Score);
        }

        for (int i = 0; i < _panels.Count; i++)
        {
            _panels[i].Panel.GetComponent<RectTransform>().anchoredPosition = Vector3.zero +
                                                                              new Vector3(0,
                                                                                  -(panelHeight * i) -
                                                                                  (panelHeight / 2f), 0) +
                                                                              new Vector3(Offset.x, Offset.y, 0);
        }
    }

    public void UpdateScore(int playerNumber, int score)
    {
        foreach (Score_UI.PanelInfo p in _panels)
        {
            if (playerNumber == p.PlayerNumber)
            {
                if (score <= 9)
                {
                    p.Panel.GetComponentInChildren<Text>().text = "0" + score.ToString();
                }
                else
                {
                    p.Panel.GetComponentInChildren<Text>().text = score.ToString();
                }
            }
        }
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (GetComponent<Canvas>().enabled)
            {
                SceneManager.LoadScene(0);
                FindObjectOfType<GameManager>().StaticReset();
            }
        }
    }
}
