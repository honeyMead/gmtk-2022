using UnityEngine;

public class Finish : MonoBehaviour
{
    public int winValue;
    public AudioSource AudioSrc { get; private set; }

    void Start()
    {
        AudioSrc = GetComponent<AudioSource>();
    }

    public void Win()
    {
        AudioSrc.Play();
    }
}
