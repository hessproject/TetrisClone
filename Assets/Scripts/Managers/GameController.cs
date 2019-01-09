﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    //Board reference
    Board m_gameBoard;

    //Spawner reference
    Spawner m_spawner;

    //Currently active shape (only one shape is in play at a time)
    Shape m_activeShape;

    //Ghost shape at bottom of board
    Ghost m_ghost;

    //Shape holder
    Holder m_holder;

    public GameObject m_gameOverPanel;

    float m_dropInterval = .9f;
    float m_dropIntervalModded;
    float m_timeToDrop;

    //Key repeat rates
    [Range(0.02f, 1f)]
    public float m_keyRepeatRateLeftRight = .125f;
    float m_timeToNextKeyLeftRight;
    [Range(0.02f, 1f)]
    public float m_keyRepeatRateDown = .01f;
    float m_timeToNextKeyDown;
    [Range(0.02f, 1f)]
    public float m_keyRepeatRateRotate = .20f;
    float m_timeToNextKeyRotate;

    bool m_gameOver = false;

    //Managers
    SoundManager m_soundManager;
    ScoreManager m_scoreManager;

    //Determine the direction that pieces rotate
    public IconToggle m_rotateIconToggle;
    bool m_clockwise = true;

    public bool m_isPaused = false;
    public GameObject m_pausePanel;

    // Use this for initialization
    void Start () {

        m_gameBoard = GameObject.FindWithTag("Board").GetComponent<Board>();
        m_spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();
        m_soundManager = FindObjectOfType<SoundManager>();
        m_scoreManager = FindObjectOfType<ScoreManager>();
        m_ghost = FindObjectOfType<Ghost>();
        m_holder = FindObjectOfType<Holder>();

        m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;
        m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
        m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

        if (!m_gameBoard)
        {
            Debug.LogWarning("Warning: No game board defined");
        }

        if (!m_soundManager)
        {
            Debug.LogWarning("Warning: No SoundManager defined");
        }

        if (!m_scoreManager)
        {
            Debug.LogWarning("Warning: No ScoreManager defined");
        }

        if (!m_spawner)
        {
            Debug.LogWarning("Warning: No spawner defined");
        }
        else
        {
            if (m_activeShape == null)
            {
                m_activeShape = m_spawner.SpawnShape();
            }
            //Make sure spawner spawns on even position to align with grid
            m_spawner.transform.position = Vectorf.Round(m_spawner.transform.position);
        }

        //Make sure gameover and pause panels aren't showing at game start
        if (m_gameOverPanel)
        {
            m_gameOverPanel.SetActive(false);
        }
        if (m_pausePanel)
        {
            m_pausePanel.SetActive(false);
        }

        m_dropIntervalModded = m_dropInterval;
	}

    void Update()
    {
        if (m_gameBoard == null || m_spawner == null || m_activeShape == null || m_soundManager == null || m_scoreManager == null || m_gameOver)
        {
            return;
        }
        PlayerInput();
    }

    void LateUpdate()
    {
        if (m_ghost)
        {
            m_ghost.DrawGhost(m_activeShape, m_gameBoard);
        }
    }

    void PlayerInput()
    {
        if ((Input.GetButton("MoveRight") && (Time.time > m_timeToNextKeyLeftRight)) || Input.GetButtonDown("MoveRight"))
        {
            m_activeShape.MoveRight();
            m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;

            if (!m_gameBoard.isValidPosition(m_activeShape))
            {
                m_activeShape.MoveLeft();
                PlaySound(m_soundManager.m_errorSound);
            }
            else
            {
                PlaySound(m_soundManager.m_moveSound, .5f);
            }
        }
        else if ((Input.GetButton("MoveLeft") && (Time.time > m_timeToNextKeyLeftRight)) || Input.GetButtonDown("MoveLeft"))
        {
            m_activeShape.MoveLeft();
            m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;

            if (!m_gameBoard.isValidPosition(m_activeShape))
            {
                m_activeShape.MoveRight();
                PlaySound(m_soundManager.m_errorSound);
            }
            else
            {
                PlaySound(m_soundManager.m_moveSound, .5f);
            }
        }
        else if (Input.GetButton("Rotate") && Time.time > m_timeToNextKeyRotate)
        {
            m_activeShape.Rotate(m_clockwise);
            m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

            if (!m_gameBoard.isValidPosition(m_activeShape))
            {
                m_activeShape.Rotate(!m_clockwise);
                PlaySound(m_soundManager.m_errorSound);
            } 
            else
            {
                PlaySound(m_soundManager.m_moveSound, .5f);
            }
        }
        else if ((Input.GetButton("MoveDown") && (Time.time > m_timeToNextKeyDown)) || (Time.time > m_timeToDrop))
        {
            m_timeToDrop = Time.time + m_dropIntervalModded;
            m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;

            m_activeShape.MoveDown();

            if (!m_gameBoard.isValidPosition(m_activeShape))
            {
                if (m_gameBoard.IsOverLimit(m_activeShape))
                {
                    GameOver();
                }
                else
                {
                    LandShape();
                }
            }

        }
        else if (Input.GetButtonDown("ToggleRotate"))
        {
            ToggleRotateDirection();
        }
        else if (Input.GetButtonDown("Pause"))
        {
            TogglePause();
        }
        else if (Input.GetButtonDown("Hold"))
        {
            Hold();
        }
    }

    private void GameOver()
    {
        m_activeShape.MoveUp();
        m_gameOver = true;
        if (m_gameOverPanel)
        {
            m_gameOverPanel.SetActive(true);
        }

        PlaySound(m_soundManager.m_gameOverSound, 2);
        PlaySound(m_soundManager.m_gameOverVocal);
    }

    private void LandShape()
    {
        //Reset key counters
        m_timeToNextKeyDown = Time.time;
        m_timeToNextKeyRotate = Time.time;
        m_timeToNextKeyLeftRight = Time.time;

        //Physically land shape
        m_activeShape.MoveUp();
        m_gameBoard.StoreShapeInGrid(m_activeShape);

        PlaySound(m_soundManager.m_dropSound, .5f);

        //Clear rows and drop rest of board
        m_gameBoard.ClearAllRows();

        if(m_gameBoard.m_completedRows > 0)
        {
            m_scoreManager.ScoreLines(m_gameBoard.m_completedRows);
            if (m_scoreManager.m_didLevelUp)
            {
                PlaySound(m_soundManager.m_levelUpVocal);
                m_dropIntervalModded = Mathf.Clamp(m_dropInterval - ((float)m_scoreManager.m_level - 1) * 0.075f, .05f, 1f);
            }
            else
            {
                if (m_gameBoard.m_completedRows > 1)
                {
                    AudioClip randomVocal = m_soundManager.GetRandomClip(m_soundManager.m_vocalSounds);
                    PlaySound(randomVocal);
                }
            }
            PlaySound(m_soundManager.m_clearRowSound);
        }

        if (m_ghost)
        {
            m_ghost.Reset();
        }

        if (m_holder)
        {
            m_holder.m_canRelease = true;
        }

        //Create new shape
        m_activeShape = m_spawner.SpawnShape();

    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void PlaySound(AudioClip clip, float volMultipler = 1)
    {
        if (clip && m_soundManager.m_fxEnabled)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, Mathf.Clamp(m_soundManager.m_fxVolume * volMultipler, 0.05f, 1f));
        }
    }

    public void ToggleRotateDirection()
    {
        m_clockwise = !m_clockwise;
        if (m_rotateIconToggle)
        {
            m_rotateIconToggle.ToggleIcon(m_clockwise);
        }
    }

    public void TogglePause()
    {
        if (m_gameOver)
        {
            return;
        }

        m_isPaused = !m_isPaused;

        if (m_pausePanel)
        {
            m_pausePanel.SetActive(m_isPaused);

            if (m_soundManager)
            {
                m_soundManager.m_musicSource.volume = (m_isPaused) ? m_soundManager.m_musicVolume * 0.25f : m_soundManager.m_musicVolume;
            }

            Time.timeScale = (m_isPaused) ? 0 : 1;
        }
    }

    public void Hold()
    {
        if (!m_holder)
        {
            return;
        }

        if (!m_holder.m_heldShape)
        {
            m_holder.Catch(m_activeShape);
            m_activeShape = m_spawner.SpawnShape();
            PlaySound(m_soundManager.m_holdSound);
        }
        else if (m_holder.m_canRelease)
        {
            Shape shapeToCatch = m_activeShape;
            m_activeShape = m_holder.Release();
            m_activeShape.transform.position = m_spawner.transform.position;
            m_holder.Catch(shapeToCatch);
            PlaySound(m_soundManager.m_holdSound);
        }
        else
        {
            PlaySound(m_soundManager.m_errorSound);
            return;
        }

        if (m_ghost)
        {
            m_ghost.Reset();
        }
        
    }

}
