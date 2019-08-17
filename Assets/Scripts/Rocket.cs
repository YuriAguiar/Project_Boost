using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    enum State
    {
        Alive,
        Dying,
        Transition
    }

    private State state = State.Alive;
    private Rigidbody _rigidbody;
    private AudioSource _audioSource;
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
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    [SerializeField] float _mainThrust = 100f;
    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            ApplyThrust();
        }
        else
        {
            _audioSource.Stop();
            _mainEngineParticles.Stop();
        }
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
    }

    [SerializeField] float _thrustForce = 1f;
    private void RespondToRotateInput()
    {
        float rotationThisFrame = _thrustForce * Time.deltaTime;
        _rigidbody.freezeRotation = true;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * _thrustForce);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * _thrustForce);
        }

        _rigidbody.freezeRotation = false;
    }

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun
    /// touching another rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    void OnCollisionEnter(Collision other)
    {
        if (state != State.Alive) { return; }

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
        state = State.Transition;
        _audioSource.Stop();
        _audioSource.PlayOneShot(_success);
        _successParticles.Play();
        Invoke("LoadNextLvl", _lvlLoadDelay);
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        _audioSource.Stop();
        _audioSource.PlayOneShot(_death);
        _deathParticles.Play();
        Invoke("LoadFirstLvl", _lvlLoadDelay);
    }

    private void LoadNextLvl()
    {
        SceneManager.LoadScene(1);
    }

    private void LoadFirstLvl()
    {
        SceneManager.LoadScene(0);
    }
}