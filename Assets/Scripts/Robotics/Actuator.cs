using UnityEngine;
using UKI;


public class Actuator : Debuggable
{
	public enum MoveType
	{
		Angular,
		Clamped
	}

	public ActuatorState m_state;

	public int m_id = 0;

	public Rigidbody m_base;
	public Rigidbody m_movingPart;

	public float m_minPosition = 0.0f;
	public float m_maxPosition = 1.0f;

	public MoveType m_moveType;
	public bool m_autoMoveAngle = false;
	public bool m_autoMoveClamped = false;
	public float m_autoMovePeriod = 1.0f;

	[Range(0.0f, 360.0f)]
	public float m_currentAngle = 0.0f;

	[Range(0.0f, 1.0f)]
	public float m_currentNormalisedPosition = 0.0f;

	public float m_moveSpeed = 0.0f;
	public float m_moveSpeedScale = 0.5f;	//Change this to tune for now

	private ConfigurableJoint m_baseJoint;
	private float m_previousNormalisedPosition = 0.0f;

	//private static int s_numberActuators = 0;	//TODO: Generalise id, possibly add ActuatorState struct.

	void Awake()
	{
		//m_id = s_numberActuators++;
		gameObject.name = m_id.ToString();
		CreateDebugObject();
	}

	void Start () 
	{
		if (MasterController.Instance)
        	MasterController.Instance.RegisterActuator(this);

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

		UpdateTargetPosition();
	}

	void Update () 
	{
		switch (m_moveType) 
		{
		case MoveType.Angular:
			m_currentAngle += Time.deltaTime * (360.0f/m_autoMovePeriod) * m_moveSpeed;
			if (m_currentAngle >= 360.0f)
				m_currentAngle -= 360.0f;
			m_currentNormalisedPosition = 0.5f * (Mathf.Sin(m_currentAngle * Mathf.Deg2Rad) + 1.0f);
			break;
		case MoveType.Clamped:
			m_currentNormalisedPosition += Time.deltaTime * m_moveSpeed * m_moveSpeedScale;
			m_currentNormalisedPosition = Mathf.Clamp (m_currentNormalisedPosition, 0.0f, 1.0f);
			break;
		}


		if (m_previousNormalisedPosition != m_currentNormalisedPosition)
		{
			UpdateTargetPosition ();
		}

		m_previousNormalisedPosition = m_currentNormalisedPosition;

		if (m_state.m_innerTrips > 0 || m_state.m_outerTrips > 0) 
		{
			SetActuatorSpeed (0.0f);
			Debug.Log ("Stopping actuator due to current trip.");
		}
		
	}

	public float GetNormalisedPosition() 
	{
		return m_currentNormalisedPosition;
	}

	public int GetID()
	{
		return m_id;	
	}

	public void SetActuatorSpeed(float normalisedSpeed)
	{
		m_moveSpeed = normalisedSpeed;
	}

	private void UpdateTargetPosition()
	{
		if (m_baseJoint) 
		{
			m_baseJoint.GetComponent<Rigidbody> ().WakeUp ();
			m_baseJoint.targetPosition = new Vector3 (0.0f, Mathf.Lerp (m_minPosition, m_maxPosition, m_currentNormalisedPosition), 0.0f);
		}
	}
}
