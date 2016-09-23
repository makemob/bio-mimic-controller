using UnityEngine;
using System.Collections;
using System.IO;
using UKI;

public class MasterController : MonoBehaviour, IMasterController
{
	public string m_configFile = "Config";
	public UKI.Config m_config;

	public RoboticsController [] m_roboticsControllers;
	public LightingController [] m_lightingControllers;

	public static MasterController Instance;

	//
	// Unity interface
	//

	void Awake() 
	{ 
		Instance = this; 
		LoadConfig();
	}

	void Start() 
	{
		Startup ();
	}

	void Update() 
	{
		if (Input.GetKeyUp (KeyCode.Escape))
			Application.Quit();
	}

	//
	// User interface
	//

	public void Startup ()
	{
		foreach (RoboticsController r in m_roboticsControllers)
			r.Startup ();
		
		foreach (LightingController l in m_lightingControllers)
			l.Startup ();
	}

	public void Shutdown ()
	{
		foreach (RoboticsController r in m_roboticsControllers)
			r.Shutdown();
		foreach (LightingController l in m_lightingControllers)
			l.Shutdown ();
	}

	public void Stop()
	{
		StopAllCoroutines();
		StopRobotics ();
		StopLighting ();
	}
		
	public void TestSequence ()
	{
		Debug.Log ("TODO: Implement a test sequence");
	}
		
	public void StopRobotics ()
	{
		StopAllCoroutines();
		foreach (RoboticsController r in m_roboticsControllers)
			r.Stop();
	}
		
	public void StopLighting ()
	{
		StopAllCoroutines();
		foreach (LightingController l in m_lightingControllers)
			l.Stop();
	}

	public bool RegisterActuator(Actuator a)
	{
		//It's currently CRITICAL that the config is applied before this actuator is registered with the robotics controller.
		bool found = m_config.FindActuator(a.name, ref a.m_config);	
		if (!found) 
		{
			Debug.Log ("Actuator (" + a.name + ") not found. Disabling.");
			a.gameObject.SetActive(false);
			return false;
		}

		bool registrationSuccess = true;
		foreach (RoboticsController r in m_roboticsControllers)
			registrationSuccess = registrationSuccess && r.RegisterActuator(a);

		return registrationSuccess;
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

	public void AllUp()
	{
		foreach (RoboticsController r in m_roboticsControllers)
			r.SetAllActuatorSpeeds(1.0f);
	}

	public void AllDown()
	{
		foreach (RoboticsController r in m_roboticsControllers)
			r.SetAllActuatorSpeeds(-1.0f);
	}

	public void Wave()
	{
		foreach (RoboticsController r in m_roboticsControllers)
			r.SetAllActuatorSpeeds(-1.0f);

		//TODO Implement wait until all down, then commence wave function
	}

	public void Callibrate()
	{
		StartCoroutine(CallibrateCoroutine());
	}

	public void UpdateAllActuatorStates()
	{
		foreach (RoboticsController r in m_roboticsControllers) {
			r.UpdateAllActuatorStates();
		}
	}

	public void UpdateActuatorState(int actuatorID)
	{
		foreach (RoboticsController r in m_roboticsControllers) {
			ActuatorState state = r.GetActuatorState (actuatorID);
			Debug.Log ("Actuator State: " + state.ToString());
		}
	}

	public IEnumerator CallibrateCoroutine()
	{
		foreach (RoboticsController r in m_roboticsControllers)
				r.SetAllActuatorSpeeds(-1.0f);
		foreach (RoboticsController r in m_roboticsControllers)
			r.SetAllActuatorSpeeds(-1.0f);

		yield return new WaitForSeconds(4.0f);

		foreach (RoboticsController r in m_roboticsControllers)
			r.SetAllActuatorSpeeds(1.0f);

		yield return new WaitForSeconds(4.0f);

		foreach (RoboticsController r in m_roboticsControllers)
			r.SetAllActuatorSpeeds(-1.0f);

		//TODO Implement wait until all downlo then commence wave function
	}

	public void LoadConfig()
	{
		//TODO: Store in game directory or somewhere easy to find.
		TextAsset t = Resources.Load (m_configFile) as TextAsset;
		m_config = JsonUtility.FromJson<Config>(t.text);
	}

	public void SaveConfig()
	{
		string json = JsonUtility.ToJson(m_config, true);
		File.WriteAllText("Assets/Resources/" + m_configFile, json);
	}
}
