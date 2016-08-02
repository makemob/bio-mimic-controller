using UnityEngine;
using System.Collections;
using UKI;

public class MasterController : MonoBehaviour, IMasterController
{
	public IRoboticsController m_roboticsController;
	public ILightingController m_lightingController;

	//
	// Unity interface
	//

	void Awake() {}
	void Start() {}
	void Update() {}

	//
	// User interface
	//

	public void Startup ()
	{
		if (m_roboticsController != null)
			m_roboticsController.Startup();
		if (m_lightingController != null)
			m_lightingController.Startup ();
	}

	public void Shutdown ()
	{
		if (m_roboticsController != null)
			m_roboticsController.Shutdown();
		if (m_lightingController != null)
			m_lightingController.Shutdown ();
	}

	public void Stop()
	{
		StopRobotics ();
		StopLighting ();
	}
		
	public void TestSequence ()
	{
		Debug.Log ("TODO: Implement a test sequence");
	}
		
	public void StopRobotics ()
	{
		if (m_roboticsController != null)
			m_roboticsController.Stop();
	}
		
	public void StopLighting ()
	{
		if (m_lightingController != null)
			m_lightingController.Stop();
	}
}
