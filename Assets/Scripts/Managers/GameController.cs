using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    Board m_gameBoard;
    Spawner m_spawner;

	// Use this for initialization
	void Start () {
        m_gameBoard = GameObject.FindWithTag("Board").GetComponent<Board>();
        m_spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();

        if (!m_gameBoard)
        {
            Debug.LogWarning("Warning: No game board defined");
        }

        if (!m_spawner)
        {
            Debug.LogWarning("Warning: No spawner defined");
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
