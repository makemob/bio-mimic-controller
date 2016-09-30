using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITextData : MonoBehaviour {

	public Text m_label;
	public Text m_data;

	void Set(string text, Object data) 
	{
		m_label.text = text;
		m_data.text = data.ToString();
	}

}
