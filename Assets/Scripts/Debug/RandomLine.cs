using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI.Extensions;

public class RandomLine : MonoBehaviour 
{
	public float m_speed = 1.0f;
	public int m_maxSize = 64;
	private Queue<Vector2> m_queue = new Queue<Vector2>();

	void Start () 
	{

	}

	void Update () 
	{
		float CurrentTime = Time.time * m_speed;

		float x = Screen.width * Mathf.PerlinNoise (CurrentTime + 3.0f, CurrentTime + 0.11f);
		float y = Screen.height * Mathf.PerlinNoise (CurrentTime + 31.0f, CurrentTime + 11.0f);

		m_queue.Enqueue(new Vector2(x, y));
		if (m_queue.Count > m_maxSize)
			m_queue.Dequeue();

		GetComponent<UILineRenderer> ().Points = m_queue.ToArray ();
	}
}
