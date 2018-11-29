using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public Shape[] m_allShapes;

    Shape GetRandomShape()
    {
        return m_allShapes[Random.Range(0, m_allShapes.Length)];
    }

    public Shape SpawnShape()
    {
        Shape shape = Instantiate(GetRandomShape(), transform.position, Quaternion.identity) as Shape;
        if (shape)
        {
            return shape;
        }
        else
        {
            Debug.LogWarning("Warning: Unable to spawn shape");
            return null;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
