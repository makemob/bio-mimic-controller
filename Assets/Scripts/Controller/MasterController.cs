using UnityEngine;
using System.Collections;
using UKI;

public class MasterController : MonoBehaviour, IMasterController, ILightingController
{
	public RoboticsController [] m_roboticsControllers;
	public ILightingController [] m_lightingControllers;

	public static MasterController Instance;

	//
	// Unity interface
	//

	void Awake() 
	{ 
		Instance = this; 
	}

	void Start() {}
	void Update() {}

	//
	// User interface
	//

	public void Startup ()
	{
		foreach (RoboticsController r in m_roboticsControllers)
			r.Startup ();
		
		foreach (ILightingController l in m_lightingControllers)
			l.Startup ();
	}

	public void Shutdown ()
	{
		foreach (RoboticsController r in m_roboticsControllers)
			r.Shutdown();
		foreach (ILightingController l in m_lightingControllers)
			l.Shutdown ();
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
		foreach (RoboticsController r in m_roboticsControllers)
			r.Stop();
	}
		
	public void StopLighting ()
	{
		foreach (ILightingController l in m_lightingControllers)
			l.Stop();
	}

	public void RegisterActuator(Actuator a)
	{
		foreach (RoboticsController r in m_roboticsControllers)
			r.RegisterActuator(a);
	}

	public void SetActuatorSpeed (int actuatorID, float speed)
	{
		foreach (RoboticsController r in m_roboticsControllers)
			r.SetActuatorSpeed(actuatorID, speed);
	}

	public void StopActuator (int actuatorID)
	{
		foreach (RoboticsController r in m_roboticsControllers)
			r.StopActuator(actuatorID);
	}

	public void StopAllActuators ()
	{
		foreach (RoboticsController r in m_roboticsControllers)
			r.StopAllActuators();
	}


}
