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
            Thrust();
            Rotate();
        }
    }

    [SerializeField] float _mainThrust = 100f;
    private void Thrust()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _rigidbody.AddRelativeForce(Vector3.up * _mainThrust * Time.deltaTime);

            if (!_audioSource.isPlaying)
            {
                _audioSource.Play();
            }
        }
        else
        {
            _audioSource.Stop();
        }
    }

    [SerializeField] float _thrustForce = 1f;
    private void Rotate()
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
                state = State.Transition;
                Invoke("LoadNextLvl", 1f);
                break;
            default:
                state = State.Dying;
                Invoke("LoadFirstLvl", 1f);
                break;
        }
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