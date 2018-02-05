using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject PlayerPrefab;

    public float PlayerSpawnRadius = 3.0f;

    [Header("Game win state")]
    public float PlayerOrbitSpeed = 1.0f;
    public float PlayerOrbitRadius = 4.0f;

    private List<PlayerInfo> _playerRoundDeathOrder = new List<PlayerInfo>();
    private List<PlayerInfo> _playerRoundWinners = new List<PlayerInfo>();
    private TimeManager _timeManager;
    private Arena _arena;

    private Camera _mainCamera;

    [Header("Round End")]
    public float RoundEndYKill = -10;
    public float RoundEndDrawTimeDifference = 2f;
    private float _roundEndDrawTimer = 0f;

    public float RoundEndResetTimerMax = 10f;
    public static bool RoundEndResetStart = false;
    private float _roundEndResetTimer = 0f;
    private static int _roundIndex = 0;

    [Header("Match End")]
    public static int MatchEndScore = 5;
    private bool _endMatch = false;

    public bool RotateSpawnsEveryRound = false;

    private int _winningPlayerIndex = -1;

    public struct PlayerInfo
    {
        public int Number;
        public Color Color;
        public float TimeOfDeath;
        public int Score;
    }

    public static List<PlayerInfo> Players = new List<PlayerInfo>();

	void Start ()
	{
	    _timeManager = FindObjectOfType<TimeManager>();
	    _arena = FindObjectOfType<Arena>();
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        int spawnOffset = (RotateSpawnsEveryRound ? _roundIndex : 0);
        PlaceHovercraft.PlaceAllPlayers(PlayerPrefab, PlayerSpawnRadius, spawnOffset);
	}
	
	void Update ()
	{
        if (!_endMatch)
        {
            RoundEnd();
        }

        if (RoundEndResetStart)
        {
            if (_winningPlayerIndex != -1)
            {
                GameObject player = GetPlayer(_winningPlayerIndex);
                OrbitCamera.OrbitPlayer(_mainCamera, player, PlayerOrbitRadius, PlayerOrbitSpeed);
            }
        }
    }

    private void OnGUI()
    {
        if (Debug.isDebugBuild)
        {
            if (GUILayout.Button("Reset"))
            {
                Reset();
            }
        }
    }

    private void RoundEnd()
    {
        if (Players.Count > 1)
        {
            if (_playerRoundDeathOrder.Count > Players.Count - 2)
            {
                if (_roundEndDrawTimer >= RoundEndDrawTimeDifference)
                {
                    if (RoundEndResetStart)
                    {
                        //Add points to the winner
                        if (_playerRoundWinners.Count == 1)
                        {
                            PlayerInfo winner = _playerRoundWinners[0];
                            _arena.SetArenaColor(winner.Color);
                            winner.Score++;
                            FindObjectOfType<Score_UI>().UpdateScore(winner.Number, winner.Score);

                            for (int i = 0; i < Players.Count; i++)
                            {
                                if (Players[i].Number == winner.Number)
                                {
                                    Players[i] = winner;
                                }
                            }

                            if (winner.Score >= MatchEndScore)
                            {
                                MatchEnd(winner.Number);
                                return;
                            }
                        }
                        else
                        {
                            _arena.SetArenaColor(Color.black);
                        }


                        if (_roundEndResetTimer >= RoundEndResetTimerMax)
                        {
                            Reset();
                        }
                        else
                        {
                            _roundEndResetTimer += Time.deltaTime;
                        }
                    }
                    else
                    {
                        if (_playerRoundDeathOrder.Count == Players.Count)
                        {
                            float lastDeathTime = _playerRoundDeathOrder[_playerRoundDeathOrder.Count - 1].TimeOfDeath;
                            //Draw logic: Once the draw timer is done, the game will check all the dead players.
                            //It will check their time of death, and compare it to the last death's time of death.
                            //If it is within a certain timeframe, both players will be winners
                            for (int deathIndex = _playerRoundDeathOrder.Count - 1; deathIndex >= 0; --deathIndex)
                            {
                                float timeDifference = lastDeathTime - _playerRoundDeathOrder[deathIndex].TimeOfDeath;
                                if (timeDifference <= RoundEndDrawTimeDifference)
                                {
                                    _playerRoundWinners.Add(_playerRoundDeathOrder[deathIndex]);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (PlayerInfo player in Players)
                            {
                                if (player.TimeOfDeath < 0f)
                                {
                                    _playerRoundWinners.Add(player);
                                    break;
                                }
                            }
                        }
                        RoundEndResetStart = true;
                    }
                }
                else
                {
                    _roundEndDrawTimer += Time.deltaTime;
                }
            }
        }
    }

    public void Reset()
    {
        if (Application.isPlaying)
        {
            for (int p = 0; p < Players.Count; p++)
            {
                PlayerInfo player = Players[p];
                player.TimeOfDeath = -1f;
                Players[p] = player;
            }
            RoundEndResetStart = false;

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            ++_roundIndex;
        }
    }

    void MatchEnd(int winnerId)
    {
        _endMatch = true;
        _winningPlayerIndex = winnerId;
        FindObjectOfType<EndScreen_UI>().SpawnEndcreen();
        //_arena.DropArena = true;
    }

    public void PlayerDie(int playerId)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].Number == playerId)
            {
                PlayerInfo p = Players[i];
                p.TimeOfDeath = _timeManager.ElapsedSeconds;
                Players[i] = p;
                _playerRoundDeathOrder.Add(p);
            }
        }
    }

    public GameObject GetPlayer(int playerNumber)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (player.GetComponent<Hovercraft>().PlayerNumber == playerNumber)
            {
                return player;
            }
        }

        return null;
    }

    public void StaticReset()
    {
        Players.Clear();
        RoundEndResetStart = false;
        _roundIndex = 0;
        Destroy(GameObject.FindObjectOfType<AudioManager>().gameObject);

    }
}
