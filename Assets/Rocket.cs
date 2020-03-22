using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;

    [SerializeField] float rcsThrust = 125f;
    [SerializeField] float mainThrust = 50f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip sucess;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem sucessParticles;
    [SerializeField] ParticleSystem deathParticles;

    enum State { Alive, Dying, Transcending};
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>(); // this function that can get components that is only Rigidbody, a reference
        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        } 
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; } // ignore collisions when dead

        switch (collision.gameObject.tag)  // reading when collision occurs, gameObject hit and then the tag
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

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(sucess);
        sucessParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay); // load the method after 1 second
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop(); // need to stop the sound and then start a new one
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadLevelOne", levelLoadDelay);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1); // allow for more than 2 screens
    }

    private void LoadLevelOne()
    {
        SceneManager.LoadScene(0);
    }

    void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying) // so sound doesn't layer - could also do keyup/down 
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; // take maual control of rotation
        float rotateThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) // this can either rotate right or left
        {
            transform.Rotate(Vector3.forward * rotateThisFrame); // Vector 3 is the coordinate system - forward is Z axis - multipled to add more than the typical 1 unit that would be put if ig was solo
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotateThisFrame);
        }

        rigidBody.freezeRotation = false; //resume physics of rotation

    }
}
