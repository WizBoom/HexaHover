using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class StartMenu_Players : MonoBehaviour
{
    [SerializeField]
    private Color[] PlayerColors;
    private int[] _playerColorIndex = new int[6];

    [SerializeField]
    private GameObject[] _playerFrames;
    private bool[] _triggersAvailable = new bool[6];

    [SerializeField]
    private GameObject _roundsBox;
    private int _rounds = 5;


    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < _playerColorIndex.Length; i++)
        {
            _playerColorIndex[i] = Random.Range(0, PlayerColors.Length);
            _playerFrames[i].GetComponent<StartMenu_PlayerFrame>().SetColor(PlayerColors[_playerColorIndex[i]]);
        }

        AddRounds(0);
    }

    // Update is called once per frame
    void Update()
    {

        for (int playerNr = 0; playerNr < _playerFrames.Length; playerNr++)
        {
            //Triggers
            if ((playerNr >= 2 && Input.GetAxis("Controller" + playerNr + "_Thrust") < 0.1))
            {
                _triggersAvailable[playerNr] = true;
                
            }

            //Activate
            if (
                (playerNr < 2 && Input.GetButtonDown("Keyboard" + playerNr + "_Thrust"))
                || (playerNr >= 2 && Input.GetButtonDown("Controller" + playerNr + "_Boost"))
                )
            {
                _playerFrames[playerNr].GetComponent<StartMenu_PlayerFrame>().TogglePlayerActivated();
            }
            //Colors
            if (
                (playerNr == 0 && Input.GetKeyDown(KeyCode.Q))
                || (playerNr == 1 && Input.GetKeyDown(KeyCode.U))
                || (playerNr >= 2 && Input.GetAxis("Controller" + playerNr + "_Thrust") > 0.7 && _triggersAvailable[playerNr])
               )
            {
                _playerColorIndex[playerNr] += 1;
                if (_playerColorIndex[playerNr] > PlayerColors.Length-1)
                    _playerColorIndex[playerNr] = 0;
                _playerFrames[playerNr].GetComponent<StartMenu_PlayerFrame>().SetColor(PlayerColors[_playerColorIndex[playerNr]]);
                _triggersAvailable[playerNr] = false;

            }
        }

    }


    private void SaveManagerValues()
    {
        for (int i = 0; i < _playerFrames.Length; i++)
        {
            if (_playerFrames[i].GetComponent<StartMenu_PlayerFrame>().IsActivated)
            {
                GameManager.PlayerInfo player = new GameManager.PlayerInfo();
                player.Number = i;
                player.Color = PlayerColors[_playerColorIndex[i]];
                player.Score = 0;
                player.TimeOfDeath = -1f;
                GameManager.Players.Add(player);
            }
        }

        //rounds
        GameManager.MatchEndScore = _rounds;
    }

    public void LoadLevel(int index)
    {
        int readyToPlay = 0;

        for (int i = 0; i < _playerFrames.Length; i++)
        {
            if (_playerFrames[i].GetComponent<StartMenu_PlayerFrame>().IsActivated)
            {
                readyToPlay++;
            }
        }
        if (readyToPlay>1)
        {
            SaveManagerValues();
            SceneManager.LoadScene(index);
        }
    }

    public void AddRounds(int value)
    {
        _rounds += value;
        _rounds = Mathf.Clamp(_rounds, 1, 99);
        if (_rounds == 1)
        {
            _roundsBox.GetComponentInChildren<Text>().text = _rounds + " Round";
        }
        _roundsBox.GetComponentInChildren<Text>().text = _rounds + " Rounds";
    }
}