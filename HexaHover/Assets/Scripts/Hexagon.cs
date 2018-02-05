using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    public bool Walkable = true;
    public float FallAcceleration = 9.81f;
    private float _fallSpeed = 0f;
    public Vector3 OrigPosition;
    public GameObject SpawnHoloPrefab;
    [HideInInspector]
    private GameObject _spawnHoloObject;

    public bool ShowSpawnIndicator = false;
    private float _showSpawnIndicatorMaxTime = 0f;
    private float _showSpawnIndicatorTimer = 0f;

    public bool ShowDespawnIndicator = false;
    private float _showDespawnIndicatorMaxTime = 0f;
    private float _showDespawnIndicatorTimer = 0f;

    Color _highlightColor;


    private void Start()
    {
        OrigPosition = this.transform.position;
        this.GetComponent<Rigidbody>().isKinematic = true;

        _spawnHoloObject = Instantiate(SpawnHoloPrefab);
        _spawnHoloObject.transform.position = this.transform.position;
        _spawnHoloObject.transform.parent = transform.parent;
        _spawnHoloObject.SetActive(false);

        //save materials
        foreach (Material mat in GetComponent<Renderer>().materials)
        {
            if (mat.HasProperty("_EmissionColor") && mat.GetColor("_EmissionColor") != Color.black)
            {
                _highlightColor = mat.color;
            }
        }
    }

    private void Update()
    {
        //Gravity
        if (!Walkable || ShowSpawnIndicator)
        {
            _fallSpeed += FallAcceleration * Time.deltaTime;
            Vector3 grav = new Vector3(0, -_fallSpeed * Time.deltaTime, 0);
            this.transform.position += grav;
        }

        if (ShowSpawnIndicator)
        {
            _showSpawnIndicatorTimer += Time.deltaTime;
            if (_showSpawnIndicatorTimer >= _showSpawnIndicatorMaxTime)
            {
                ResetMaterials();
                this.transform.position = OrigPosition;
                this.transform.rotation = Quaternion.Euler(-90, 0, 0);
                this.GetComponent<Rigidbody>().velocity = Vector3.zero;
                _spawnHoloObject.SetActive(false);
                Walkable = true;
                ShowSpawnIndicator = false;
                _fallSpeed = 0;
            }
            else if(!_spawnHoloObject.activeSelf)
            {
                _spawnHoloObject.SetActive(true);
            }
        }
        else if (ShowDespawnIndicator)
        {
            _showDespawnIndicatorTimer += Time.deltaTime;
            if (_showDespawnIndicatorTimer >= _showDespawnIndicatorMaxTime)
            {
                Walkable = false;
                ShowDespawnIndicator = false;
            }
        }
    }

    public void SetWalkable(bool walkable, float spawnTimer, float despawnTimer)
    {
        Walkable = walkable;

        if (walkable)
        {
            if (spawnTimer > 0f)
            {
                Walkable = false;
                ShowSpawnIndicator = true;
                _showSpawnIndicatorMaxTime = spawnTimer;
                _showSpawnIndicatorTimer = 0f;
            }
            else
            {
                this.transform.position = OrigPosition;
                this.transform.rotation = Quaternion.Euler(-90, 0, 0);
                this.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
        else
        {
            if (despawnTimer > 0f)
            {
                Walkable = true;
                _showDespawnIndicatorMaxTime = despawnTimer;
                ShowDespawnIndicator = true;
                _showDespawnIndicatorTimer = 0f;
                SetColor(Color.black);
            }
        }
    }
    public void SetColor(Color color)
    {
        foreach (Material mat in GetComponent<Renderer>().materials)
        {
            if (mat.HasProperty("_EmissionColor") && mat.GetColor("_EmissionColor") != Color.black)
            {
                mat.color = color;
            }
        }
    }

    void ResetMaterials()
    {
        foreach (Material mat in GetComponent<Renderer>().materials)
        {
            if (mat.HasProperty("_EmissionColor") && mat.GetColor("_EmissionColor") != Color.black)
            {
                mat.color = _highlightColor;
            }
        }
    }
}
