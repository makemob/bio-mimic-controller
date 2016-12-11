using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FlashColor : MonoBehaviour {

	public Color m_color = Color.green;
	public float m_time = 0.1f;
	private Image m_image;
	
	public void Flash()
	{
		if (!m_image)
			m_image = GetComponent<Image> ();
		
		if (gameObject.activeInHierarchy) {
			StartCoroutine (FlashCoroutine ());
		}
	}
	public IEnumerator FlashCoroutine() 
	{
		if (m_image)
		{
			Color oldColor = m_image.color;
			m_image.color = m_color;
			yield return new WaitForSeconds (m_time);
			m_image.color = oldColor;
		}
	}
}
