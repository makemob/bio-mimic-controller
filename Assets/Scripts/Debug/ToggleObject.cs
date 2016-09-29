using UnityEngine;
using System.Collections;

public class ToggleObject : MonoBehaviour {

	public GameObject m_object;

	public void Toggle () {
		if (m_object)
			m_object.SetActive(!m_object.activeSelf);
	}
}
