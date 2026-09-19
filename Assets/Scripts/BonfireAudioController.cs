using UnityEngine;

public class BonfireAudioController : MonoBehaviour
{
    public AudioSource smallBonfireAudio;
    public AudioSource mediumBonfireAudio;
    public AudioSource largeBonfireAudio;

    private BonfireSize _size;
    public BonfireSize size
    {
        get { return _size; }
        set
        {
            _size = value;
            updateSound();
        }
    }

    private void updateSound()
    {
        switch (size)
        {
            case BonfireSize.Small:
                smallBonfireAudio.enabled = true;
                largeBonfireAudio.enabled = false;
                mediumBonfireAudio.enabled = false;
                break;
            case BonfireSize.Medium:
                smallBonfireAudio.enabled = false;
                mediumBonfireAudio.enabled = true;
                largeBonfireAudio.enabled = false;
                break;
            case BonfireSize.Large:
                smallBonfireAudio.enabled = false;
                mediumBonfireAudio.enabled = false;
                largeBonfireAudio.enabled = true;
                break;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }
}
