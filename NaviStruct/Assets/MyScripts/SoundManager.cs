using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource scrollSource;
    public AudioSource clickSource;
    public static SoundManager instance = null;

    public float lowPitchRange = .95f;
    public float HighPitchRange = 1.05f;

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySingle(AudioClip clip)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
