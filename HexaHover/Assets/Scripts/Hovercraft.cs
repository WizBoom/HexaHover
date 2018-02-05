using UnityEngine;

public class Hovercraft : MonoBehaviour
{
    public int PlayerNumber;
    private bool isDead = false;

    private GameManager _gameManager;

    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (!isDead)
        {
            if (transform.position.y <= _gameManager.RoundEndYKill)
            {
                _gameManager.PlayerDie(PlayerNumber);
                isDead = true;
            }
        }
    }
}
