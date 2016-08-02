using UnityEngine;


public class Actuator : Debuggable 
{
	public Rigidbody m_base;
	public Rigidbody m_movingPart;

	public float m_minPosition = 0.0f;
	public float m_maxPosition = 1.0f;

	public bool m_autoMove = false;
	public float m_autoMovePeriod = 1.0f;

	[Range(0.0f, 360.0f)]
	public float m_currentAngle = 0.0f;

	[Range(0.0f, 1.0f)]
	public float m_currentNormalisedPosition = 0.0f;

	private ConfigurableJoint m_baseJoint;
	private float m_previousNormalisedPosition = 0.0f;

	void Start () 
	{
		if (m_movingPart)
		{
			//Need to loop through to find the correct joint as there may be multiple attached to this game object.
			ConfigurableJoint [] allJoints = m_movingPart.GetComponents<ConfigurableJoint>();
			for(int i=0; i < allJoints.Length; i++)
			{
				if (allJoints[i].connectedBody == m_base)
					m_baseJoint = allJoints[i];
			}
		}
	}

	void Update () 
	{
		if (m_autoMove)
		{
			//m_currentNormalisedPosition = 0.5f * (Mathf.Sin(Time.realtimeSinceStartup / m_autoMovePeriod) + 1.0f);
			m_currentAngle += Time.deltaTime * (360.0f/m_autoMovePeriod);
			if (m_currentAngle >= 360.0f)
				m_currentAngle -= 360.0f;
			m_currentNormalisedPosition = 0.5f * (Mathf.Sin(m_currentAngle * Mathf.Deg2Rad) + 1.0f);
		}

		if (m_baseJoint && m_previousNormalisedPosition != m_currentNormalisedPosition)
		{
			m_baseJoint.GetComponent<Rigidbody>().WakeUp();
			m_baseJoint.targetPosition = new Vector3(0.0f, Mathf.Lerp(m_minPosition, m_maxPosition, m_currentNormalisedPosition), 0.0f);
		}

		m_previousNormalisedPosition = m_currentNormalisedPosition;
	}

	public float GetNormalisedPosition() 
	{
		return m_currentNormalisedPosition;
	}
}
