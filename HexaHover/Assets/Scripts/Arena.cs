using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    public enum ArenaType
    {
        CUSTOM,
        SQUARE,
        HEXAGON,
    }

    [Header("General")]
    public ArenaType Type;
    public float HexagonRadius = 1f;
    public float HexagonHeight = 2f;
    public GameObject HexagonPrefab;

    [Header("ArenaType: Square")]
    public int SquareRows = 10;
    public int SquareColumn = 5;

    [Header("ArenaType: Hexagon")]
    public int ArenaRadius = 10;

    [Header("Dynamic Arena")]
    public float BlockDespawnMaxTimer = 1f;
    public float BlockDespawnIndicatorTimer = 1f;
    private float _blockDespawnTimer = 4f;

    public float BlockSpawnMaxTimer;
    public float BlockSpawnIndicatorTimer = 1f;
    private float _blockSpawnTimer = 0f;

    public float DynamicArenaBeginTimer = 1f;

    private TimeManager _timeManager;
    private List<GameObject> _hexagons = new List<GameObject>();
    [HideInInspector]
    public bool DropArena = false;

	void Start ()
	{
	    _timeManager = FindObjectOfType<TimeManager>();

        GenerateArena(Type);
        
	    foreach (Transform t in this.transform)
	    {
	        if (t.CompareTag("Arena_Dropable"))
	        {
	            _hexagons.Add(t.gameObject);
            }
        }
	}
	
	void Update ()
    {
        if (Debug.isDebugBuild) KeyboardArenaAdapt();
        if (DropArena)
        {
            if (CanDropHexagon())
            {
                // Repeat until hexagon was successfully dropped
                while (!DropHexagon(Random.Range(0, _hexagons.Count), 0,0)) { }
            }
        }
        else if(!GameManager.RoundEndResetStart)
        {
            AdaptArena();
        }
    }

    public bool DropHexagon(int index, float spawnTimer, float despawnTimer)
    {
        Hexagon hex = _hexagons[index].GetComponent<Hexagon>();
        if (hex.Walkable && !hex.ShowSpawnIndicator && !hex.ShowDespawnIndicator)
        {
            hex.SetWalkable(false, spawnTimer, despawnTimer);
            return true;
        }
        return false;
    }

    public bool AddHexagon(int index)
    {
        Hexagon hex = _hexagons[index].GetComponent<Hexagon>();
        if (!hex.Walkable && !hex.ShowSpawnIndicator && !hex.ShowDespawnIndicator)
        {
            hex.SetWalkable(true, BlockSpawnIndicatorTimer, BlockDespawnIndicatorTimer);
            return true;
        }
        return false;
    }

    public bool CanDropHexagon()
    {
        foreach (GameObject h in _hexagons)
        {
            if (h.GetComponent<Hexagon>().Walkable && !h.GetComponent<Hexagon>().ShowDespawnIndicator)
            {
                return true;
            }
        }

        return false;
    }

    public bool CanAddHexagon()
    {
        foreach (GameObject h in _hexagons)
        {           
            if (!h.GetComponent<Hexagon>().Walkable && !h.GetComponent<Hexagon>().ShowSpawnIndicator)
            {
                return true;
            }
        }

        return false;
    }

    public void GenerateArena(ArenaType type)
    {
        switch (type)
        {
            case ArenaType.SQUARE:
                GenerateSquare(SquareRows, SquareColumn);
                break;
            case ArenaType.HEXAGON:
                GenerateHexagon(ArenaRadius);
                break;
        }
    }

    private void GenerateSquare(int rowAmount, int columnAmount)
    {
        Quaternion rot = Quaternion.Euler(-90, 0, 0);
        for (int row = 0; row < rowAmount; row++)
        {
            for (int column = 0; column < columnAmount; column++)
            {
                float xOffset = (row * ((HexagonRadius * Mathf.Sqrt(3)) / 2)) - ((rowAmount * ((HexagonRadius * Mathf.Sqrt(3)) / 2))/2f);
                int mod = row % 2;
                float zOffset = (column * HexagonRadius) + (mod * (HexagonRadius/2)) - ((columnAmount*HexagonRadius)/2f);
                Vector3 pos = new Vector3(xOffset, -HexagonHeight, zOffset);
                GameObject t = Instantiate(HexagonPrefab, pos, rot);
                t.transform.parent = this.transform;
            }
        }
    }

    private void GenerateHexagon(int radius)
    {
        Quaternion rot = Quaternion.Euler(-90, 0, 0);

        //Generate middle bar
        int middleBar = (radius * 2) - 1;
        float middleBarXOffset = middleBar; 
        for (int column = 0; column < middleBar; column++)
        { 
            float zOffset = (column * HexagonRadius) - middleBarXOffset + HexagonRadius/2f;
            Vector3 pos = new Vector3(0, -HexagonHeight, zOffset);
            GameObject t = Instantiate(HexagonPrefab, pos, rot);
            t.transform.parent = this.transform;
        }

        //Generate bottom bar
        for (int row = 1; row < radius; row++)
        {
            bool newRow = false;
            while (!newRow)
            {
                int radiusBar = ((radius * 2) - 1) - row;
                for (int column = 0; column < radiusBar; column++)
                {
                    float zOffset = (column * HexagonRadius) - middleBarXOffset + ((HexagonRadius/2f) * row) + HexagonRadius / 2f;
                    float xOffset = (row * ((HexagonRadius * Mathf.Sqrt(3)) / 2));
                    Vector3 pos = new Vector3(xOffset, -HexagonHeight, zOffset);
                    GameObject t = Instantiate(HexagonPrefab, pos, rot);
                    t.transform.parent = this.transform;
                    if (column >= radiusBar -1)
                    {
                        newRow = true;
                    }
                }
            }
        }

        //Generate top bar
        for (int row = 1; row < radius; row++)
        {
            bool newRow = false;
            while (!newRow)
            {
                int radiusBar = ((radius * 2) - 1) - row;
                for (int column = 0; column < radiusBar; column++)
                {
                    float zOffset = (column * HexagonRadius) - middleBarXOffset + ((HexagonRadius / 2f) * row) + HexagonRadius / 2f;
                    float xOffset = (-row * ((HexagonRadius * Mathf.Sqrt(3)) / 2));
                    Vector3 pos = new Vector3(xOffset, -HexagonHeight, zOffset);
                    GameObject t = Instantiate(HexagonPrefab, pos, rot);
                    t.transform.parent = this.transform;
                    if (column >= radiusBar - 1)
                    {
                        newRow = true;
                    }
                }
            }
        }
    }

    private void KeyboardArenaAdapt()
    {
        if (Input.GetKey(KeyCode.T))
        {
            if (CanDropHexagon())
            {
                // Repeat until hexagon was successfully dropped
                while (!DropHexagon(Random.Range(0, _hexagons.Count), 0,BlockDespawnMaxTimer)){}
            }
        }
        if (Input.GetKey(KeyCode.Y))
        {
            DropArena = false;
            if (CanAddHexagon())
            {
                // Repeat until hexagon was successfully added
                while (!AddHexagon(Random.Range(0, _hexagons.Count))){}
            }
        }
        if (Input.GetKey(KeyCode.U))
        {
            DropArena = true;
        }
    }

    private void AdaptArena()
    {
        if (_timeManager.ElapsedSeconds >= DynamicArenaBeginTimer)
        {
            _blockDespawnTimer += Time.deltaTime;
            _blockSpawnTimer += Time.deltaTime;

            if (_blockDespawnTimer >= BlockDespawnMaxTimer)
            {
                _blockDespawnTimer = 0f;
                if (CanDropHexagon())
                {
                    int rnd = Random.Range(0, _hexagons.Count);

                    bool newNumber = DropHexagon(rnd, BlockSpawnIndicatorTimer, BlockDespawnIndicatorTimer);

                    while (newNumber == false)
                    {
                        rnd = Random.Range(0, _hexagons.Count);
                        newNumber = DropHexagon(rnd, BlockSpawnIndicatorTimer, BlockDespawnIndicatorTimer);
                    }
                }
            }

            if (_blockSpawnTimer >= BlockSpawnMaxTimer)
            {
                _blockSpawnTimer = 0f;
                if (CanAddHexagon())
                {
                    int rnd = Random.Range(0, _hexagons.Count);

                    bool newNumber = AddHexagon(rnd);

                    while (newNumber == false)
                    {
                        rnd = Random.Range(0, _hexagons.Count);
                        newNumber = AddHexagon(rnd);
                    }
                }
            }
        }
    }

    public void SetArenaColor(Color color)
    {
        foreach (GameObject h in _hexagons)
        {
            Hexagon hex = h.GetComponent<Hexagon>();
            hex.SetColor(color);
        }
    }
}
