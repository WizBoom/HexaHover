using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score_UI : MonoBehaviour
{
    public GameObject PlayerPanelPrefab;
    public Vector2 Offset;

    public struct PanelInfo
    {
        public int PlayerNumber;
        public GameObject Panel;
    }

    private List<PanelInfo> _panels = new List<PanelInfo>();


    void Start()
    {
        float panelHeight = (PlayerPanelPrefab.GetComponent<RectTransform>().rect.height);
        float panelWidth = (PlayerPanelPrefab.GetComponent<RectTransform>().rect.width/2);

        foreach (GameManager.PlayerInfo p in GameManager.Players)
        {
            GameObject panel = Instantiate(PlayerPanelPrefab);
            panel.transform.parent = this.transform;
            panel.GetComponent<Image>().color = p.Color;
            PanelInfo pi;
            pi.PlayerNumber = p.Number;
            pi.Panel = panel;
            _panels.Add(pi);
            UpdateScore(p.Number, p.Score);
        }

        for (int i = 0; i < _panels.Count; i++)
        {
            _panels[i].Panel.GetComponent<RectTransform>().anchoredPosition = Vector3.zero +
                                                                    new Vector3(Offset.x, -Offset.y, 0) +
                                                                    new Vector3(panelWidth, -(panelHeight*i) - (panelHeight / 2f), 0);
        }
    }

    public void UpdateScore(int playerNumber, int score)
    {
        foreach (PanelInfo p in _panels)
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
}
