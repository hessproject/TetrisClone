using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holder : MonoBehaviour {

    public Transform m_holderXform;
    public Shape m_heldShape = null;
    float m_scale = 0.5f;
    public bool m_canRelease = false;
    
    public void Catch(Shape shape)
    {
        if (m_heldShape)
        {
            Debug.LogWarning("Warning: Release shape before catching another");
            return;
        }

        if (!shape)
        {
            Debug.LogWarning("Warning: Attempted to catch invalid shape");
        }

        if (m_holderXform)
        {
            shape.transform.position = m_holderXform.position + shape.m_queueOffset;
            shape.transform.localScale = new Vector3(m_scale, m_scale, m_scale);
            m_heldShape = shape;
        }
        else
        {
            Debug.LogWarning("Warning: Holder has no transform assigned");
        }
    }

    public Shape Release()
    {
        m_heldShape.transform.localScale = Vector3.one;
        Shape shapeToReturn = m_heldShape;
        m_heldShape = null;

        m_canRelease = false;
        return shapeToReturn;
    }

}
