using UnityEngine;
using UnityEngine.Events;
using UKI;


public class Actuator : Debuggable
{
	public enum MoveType
	{
		Angular,
		Clamped
	}

	public ActuatorConfig m_config;
	public ActuatorState m_state;

	public Rigidbody m_base;
	public Rigidbody m_movingPart;

	public float m_minPosition = 0.0f;
	public float m_maxPosition = 1.0f;
	public float m_minLimit = 0.0f;
	public float m_maxLimit = 0.29f;

	public MoveType m_moveType;
	public bool m_autoMoveAngle = false;
	public bool m_autoMoveClamped = false;
	public float m_autoMovePeriod = 1.0f;

	public UnityEvent m_onStateUpdate;
	public UnityEvent m_onMaxLimitReached;

	[Range(0.0f, 360.0f)]
	public float m_currentAngle = 0.0f;

	[Range(0.0f, 1.0f)]
	public float m_currentNormalisedPosition = 0.0f;

	public float m_moveSpeed = 0.0f;
	public float m_moveSpeedScale = 0.5f;	//Change this to tune for now

	ConfigurableJoint m_baseJoint;
	float m_previousNormalisedPosition = 0.0f;

	static readonly float ARM_INITIAL_EXTENT = -0.038f;	//Arm extension when fully retracted.
	static readonly float ARM_FULL_EXTENT = -0.328f;	//Arm extension when fully retracted.
	static readonly float ARM_LENGTH = 0.29f; 

	void Start () 
	{
		if (MasterController.Instance) 
		{
			bool success = MasterController.Instance.RegisterActuator (this);
			if (!success)
				return;	//Failed to register, prevent startup of this actuator
		}

		ApplyConfig ();

		CreateDebugObject();

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

		GetExtensionMillimetres ();
	}

	public float GetNormalisedPosition() 
	{
		return m_currentNormalisedPosition;
	}

	public int GetID()
	{
		return m_config.id;	
	}

	public void SetActuatorSpeed(float normalisedSpeed)
	{
		if (GetID () == 6 && normalisedSpeed == 0.0f) {
			normalisedSpeed = 0.0f;
		}
		m_moveSpeed = normalisedSpeed;
	}

	public void SetCallibration(CallibrationResults results)
	{
		float averageTime = (results.m_extensionTime + results.m_retractionTime) * 0.5f;
		if (averageTime != 0.0f)
			m_moveSpeed = 1.0f/averageTime;
	}

	public void SetState(ActuatorState newState)
	{
		if (newState.m_innerCurrentTrips > m_state.m_innerCurrentTrips)
			newState.m_innerCurrentTripped = true;
		if (newState.m_outerCurrentTrips > m_state.m_outerCurrentTrips)
			newState.m_outerCurrentTripped = true;

		m_state = newState;
		m_state.m_predictedExtension = GetExtensionMillimetres ();


		m_onStateUpdate.Invoke ();

	}

	void UpdateTargetPosition()
	{
		if (m_baseJoint) 
		{
			m_baseJoint.GetComponent<Rigidbody> ().WakeUp ();
			m_baseJoint.targetPosition = new Vector3 (0.0f, Mathf.Lerp (m_minPosition, m_maxPosition, m_currentNormalisedPosition), 0.0f);

			m_state.m_predictedExtension = GetExtensionMillimetres ();

			if (m_state.m_predictedExtension >= m_maxLimit)
			{
				Debug.LogWarning ("Max limit of " + 1000.0f * m_maxLimit + "mm reached.");
				m_onMaxLimitReached.Invoke ();
				SetActuatorSpeed (0.0f);	//simulation only
			}

			m_onStateUpdate.Invoke ();
		}
	}

	void ApplyConfig()
	{
		gameObject.name = m_config.name;

		m_minPosition = ARM_INITIAL_EXTENT;
		m_maxPosition = ARM_FULL_EXTENT;

		//m_minLimit = ARM_INITIAL_EXTENT - m_config.minExtent;
		//m_maxLimit = ARM_INITIAL_EXTENT - m_config.maxExtent;
		m_minLimit = m_config.minExtent;
		m_maxLimit = m_config.maxExtent;
	}

	public int GetExtensionMillimetres()
	{
		return (int)(-1000.0f * m_currentNormalisedPosition * (m_maxPosition - m_minPosition));
	}
}
