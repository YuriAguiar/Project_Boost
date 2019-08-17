using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    private bool isTransitioning = false;
    private Rigidbody _rigidbody;
    private AudioSource _audioSource;
    private Light _thrustLight;
    [SerializeField] private AudioClip _mainEngine;
    [SerializeField] private AudioClip _success;
    [SerializeField] private AudioClip _death;
    [SerializeField] private ParticleSystem _mainEngineParticles;
    [SerializeField] private ParticleSystem _successParticles;
    [SerializeField] private ParticleSystem _deathParticles;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _thrustLight = GetComponentInChildren<Light>();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (!isTransitioning)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }

        if (Debug.isDebugBuild)
        {
            RespondToDebugInput();
        }
    }

    private bool _collisionsDisabled = false;
    private void RespondToDebugInput()
    {
        if (Input.GetKey(KeyCode.L))
        {
            LoadNextLvl();
        }

        if (Input.GetKey(KeyCode.C))
        {
            _collisionsDisabled = !_collisionsDisabled;
        }
    }

    [SerializeField] float _mainThrust = 150f;
    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            ApplyThrust();
        }
        else
        {
            StopThrust();
        }
    }

    private void StopThrust()
    {
        _audioSource.Stop();
        _mainEngineParticles.Stop();
        _thrustLight.enabled = false;
    }

    private void ApplyThrust()
    {
        _rigidbody.AddRelativeForce(Vector3.up * _mainThrust * Time.deltaTime);

        if (!_audioSource.isPlaying)
        {
            _audioSource.PlayOneShot(_mainEngine);
        }

        if (!_mainEngineParticles.isPlaying)
        {
            _mainEngineParticles.Play();
        }

        if (!_thrustLight.isActiveAndEnabled)
        {
            _thrustLight.enabled = true;
        }
    }

    [SerializeField] float _rotationForce = 2f;
    private void RespondToRotateInput()
    {
        float rotationThisFrame = _rotationForce * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            RotateManually(Vector3.forward * _rotationForce);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RotateManually(-Vector3.forward * _rotationForce);
        }
    }

    private void RotateManually(Vector3 rotationVector)
    {
        _rigidbody.freezeRotation = true;
        transform.Rotate(rotationVector);
        _rigidbody.freezeRotation = false;
    }


    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun
    /// touching another rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    void OnCollisionEnter(Collision other)
    {
        if (isTransitioning || _collisionsDisabled) { return; }

        switch (other.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    [SerializeField] private float _lvlLoadDelay = 3f;
    private void StartSuccessSequence()
    {
        isTransitioning = true;
        _audioSource.Stop();
        _audioSource.PlayOneShot(_success);
        _successParticles.Play();
        Invoke("LoadNextLvl", _lvlLoadDelay);
    }

    private void StartDeathSequence()
    {
        isTransitioning = true;
        _audioSource.Stop();
        _audioSource.PlayOneShot(_death);
        _deathParticles.Play();
        Invoke("LoadFirstLvl", _lvlLoadDelay);
    }

    private void LoadNextLvl()
    {
        int currLvl = SceneManager.GetActiveScene().buildIndex;
        if ((currLvl + 1) % SceneManager.sceneCountInBuildSettings == 0)
        {
            LoadFirstLvl();
            return;
        }

        SceneManager.LoadScene(currLvl + 1);
    }

    private void LoadFirstLvl()
    {
        SceneManager.LoadScene(0);
    }
}