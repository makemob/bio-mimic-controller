using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
	public float m_orbitSpeed = 360.0f;
	public float m_zoomSpeed = 10.0f;
	public float m_minZoom = 0.5f;
	public float m_maxZoom = 10.0f;

	private Vector3 m_velocity;	//x = left right orbit, y = up down orbit, z = zoom
	private Vector3 m_lastMousePosition;

	private Vector3 Target
	{
		get { return transform.parent ? transform.parent.position : Vector3.zero; }
	}

	void Update()
	{
		//Update mouse position
		Vector3 mousePosition = Input.mousePosition;
		Vector3 mouseDelta = mousePosition - m_lastMousePosition;
		m_lastMousePosition = mousePosition;

		//Update keyboard movement
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
			m_velocity.x += Time.deltaTime * m_orbitSpeed;
		if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
			m_velocity.x += Time.deltaTime * -m_orbitSpeed;
		if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey(KeyCode.E))
			m_velocity.y += Time.deltaTime * m_orbitSpeed;
		if (Input.GetKey (KeyCode.DownArrow) || Input.GetKey(KeyCode.Q))
			m_velocity.y += Time.deltaTime * -m_orbitSpeed;
		if (Input.GetKey (KeyCode.W))
			m_velocity.z += Time.deltaTime * -m_zoomSpeed;
		if (Input.GetKey (KeyCode.S))
			m_velocity.z += Time.deltaTime * m_zoomSpeed;
		
		//Alter xy velocity if clicking
		if (Input.GetMouseButton (0)) 
		{
			m_velocity.x += mouseDelta.x * Time.deltaTime * m_orbitSpeed;
			m_velocity.y += mouseDelta.y * Time.deltaTime * -m_orbitSpeed;	//y-inverted
		}

		//Alter zoom if double touching
		if (Input.GetMouseButton (1) || Input.touchCount > 1) 
		{
			m_velocity.z += mouseDelta.y * Time.deltaTime * m_zoomSpeed;
		}

		//Translate zoom
		Vector3 toTarget = Target - transform.position;
		Vector3 directionToParent = toTarget.normalized;
		float moveThisFrame = m_velocity.z * Time.deltaTime;
		float distanceToParent = Mathf.Clamp(toTarget.magnitude + moveThisFrame, m_minZoom, m_maxZoom);
		transform.position = Target - directionToParent * distanceToParent;

		//Rotate around
		transform.LookAt(Target);
		transform.RotateAround(Target, Vector3.up, m_velocity.x * Time.deltaTime);
		transform.RotateAround(Target, transform.right, m_velocity.y * Time.deltaTime);

		//Reduce the velocity
		m_velocity = m_velocity - m_velocity * 0.1f;
		if (m_velocity.sqrMagnitude < 0.1f)
			m_velocity = Vector3.zero;
	}
		
}