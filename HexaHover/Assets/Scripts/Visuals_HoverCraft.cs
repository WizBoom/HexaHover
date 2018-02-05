using UnityEngine;

//[ExecuteInEditMode]

public class Visuals_HoverCraft : MonoBehaviour
{
    private float _propRotateMultiplier = 1;

    [SerializeField]
    public Color GlowColor;

    [SerializeField]
    private GameObject _propLeft;
    [SerializeField]
    private GameObject _propRight;

    [SerializeField]
    private Texture[] _faces;

    private float _faceTime = 1;
    private float _faceTimer = 0;

    Material[] _startMats;
    Material[] _propStartMats;

    private PlayerAudioManager _audioManager;

    // Use this for initialization
    void Start()
    {
        //save materials
        _startMats = GetComponent<Renderer>().materials;
        _propStartMats = GetComponentInChildren<Renderer>().materials;
        //audio
        _audioManager = GetComponent<PlayerAudioManager>();

    }

    // Update is called once per frame
    void Update()
    {
        _faceTimer += Time.deltaTime;
        if (_faceTimer>_faceTime)
        {
            SetFace(Random.Range(0, 3));
            _faceTimer = 0;
            _faceTime = Random.Range(0.5f, 1.0f);
        }

        float thrustInput = GetComponent<HovercraftMovement>().GetThrustInput();
        if (thrustInput > 0)
            _propRotateMultiplier += Time.deltaTime*5 * thrustInput;
        else
            _propRotateMultiplier -= Time.deltaTime;

        _propRotateMultiplier = Mathf.Clamp(_propRotateMultiplier, 0, 1);
        
        _propLeft.transform.Rotate(0, 0, Time.deltaTime * 500 * _propRotateMultiplier);
        _propRight.transform.Rotate(0, 0, Time.deltaTime * 500 * -_propRotateMultiplier);

        foreach (Renderer rend in GetComponentsInChildren<Renderer>())
        {
            foreach (Material mat in rend.materials)
            {
                if (mat.HasProperty("_EmissionColor") && mat.GetColor("_EmissionColor") != Color.black)
                {
                    mat.SetColor("_EmissionColor", GlowColor);
                }
            }
        }
        foreach (Light light in GetComponentsInChildren<Light>())
        {
            light.color = GlowColor;
        }

        //particle system
        foreach (ParticleSystem particle in GetComponentsInChildren<ParticleSystem>())
        {
            particle.startColor = new Color(GlowColor.r, GlowColor.g, GlowColor.b, 0.6f) ;

            particle.emissionRate = GetComponent<HovercraftMovement>().GetThrustInput() * 10 +1;
        }

    }

    void SetFace(int index)
    {
        GetComponent<Renderer>().materials[3].SetTexture("_EmissionMap", _faces[index]);
    }

    void OnApplicationQuit()
    {
        ResetMaterials();
    }

    void ResetMaterials()
    {
        GetComponent<Renderer>().materials = _startMats;
        _propLeft.GetComponent<Renderer>().materials = _propStartMats;
        _propRight.GetComponent<Renderer>().materials = _propStartMats;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            _audioManager.Play(_audioManager.Bounce);
        }
    }
    public void PlayBoostSound()
    {
        _audioManager.Play(_audioManager.Boost);
    }
}