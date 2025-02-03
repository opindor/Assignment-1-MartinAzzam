using Unity.VisualScripting;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{
    private static AudioManagerScript instance;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        // Play the music when the game starts
        GetComponent<AudioSource>().Play();
    }
}
