using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    //Volume toggles
    public bool m_musicEnabled = true;
    public bool m_fxEnabled = true;

    //Volume sliders
    [Range(0, 1)]
    public float m_musicVolume = 1.0f;
    [Range(0, 1)]
    public float m_fxVolume = 1.0f;

    //Sound clips
    public AudioClip m_clearRowSound;
    public AudioClip m_moveSound;
    public AudioClip m_errorSound;
    public AudioClip m_dropSound;
    public AudioClip m_gameOverSound;
    public AudioClip m_gameOverVocal;
    public AudioClip m_levelUpVocal;

    //Sound arrays for random selection
    public AudioClip[] m_backgroundMusic;
    public AudioClip[] m_vocalSounds;

    //Selected clip from array
    AudioClip m_selectedBackgroundMusic;

    public AudioSource m_musicSource;

    //For switching the UI sprites depending on if sound is enabled
    public IconToggle m_musicIconToggle;
    public IconToggle m_fxIconToggle;

	// Use this for initialization
	void Start () {
        m_selectedBackgroundMusic = GetRandomClip(m_backgroundMusic);
        PlayBackgroundMusic(m_selectedBackgroundMusic);
	}
	
	// Update is called once per frame
	void Update () {

	}

    public AudioClip GetRandomClip(AudioClip[] clips)
    {
        return clips[Random.Range(0, clips.Length)];
    }

    public void PlayBackgroundMusic(AudioClip musicClip)
    {
        if (!m_musicEnabled || !musicClip || !m_musicSource)
        {
            return;
        }

        //If music is already running, stop
        m_musicSource.Stop();

        //Get music data from SoundManager object
        m_musicSource.clip = musicClip;
        m_musicSource.volume = m_musicVolume;
        m_musicSource.loop = true;
        m_musicSource.Play();
    }

    void UpdateMusic()
    {
        if (m_musicSource.isPlaying != m_musicEnabled)
        {
            if (m_musicEnabled)
            {
                PlayBackgroundMusic(m_selectedBackgroundMusic);
            }
            else
            {
                m_musicSource.Stop();
            }
        }
    }

    public void ToggleMusic()
    {
        m_musicEnabled = !m_musicEnabled;
        UpdateMusic();
        if (m_musicIconToggle)
        {
            m_musicIconToggle.ToggleIcon(m_musicEnabled);
        }
    }

    public void ToggleFX()
    {
        m_fxEnabled = !m_fxEnabled;
        if (m_fxIconToggle)
        {
            m_fxIconToggle.ToggleIcon(m_fxEnabled);
        }
    }

}
