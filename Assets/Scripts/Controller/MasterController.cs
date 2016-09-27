using UnityEngine;
using System.Collections;
using System.IO;
using UKI;

public class MasterController : MonoBehaviour, IMasterController
{
	public string m_configFile = "Config";
	public UKI.Config m_config;

	public RoboticsController m_roboticsControllers;
	public LightingController m_lightingControllers;

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
		m_roboticsControllers.Startup ();	
		m_lightingControllers.Startup ();
	}

	public void Shutdown ()
	{
		m_roboticsControllers.Shutdown();
		m_lightingControllers.Shutdown();
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
		//StopAllCoroutines();
		m_roboticsControllers.Stop();
	}
		
	public void StopLighting ()
	{
		//StopAllCoroutines();
		m_lightingControllers.Stop();
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

		bool registrationSuccess = m_roboticsControllers.RegisterActuator (a);

		return registrationSuccess;
	}

	public void SetActuatorSpeed (int actuatorID, float speed)
	{
		m_roboticsControllers.SetActuatorSpeed(actuatorID, speed);
	}

	public void StopActuator (int actuatorID)
	{
		m_roboticsControllers.StopActuator(actuatorID);
	}

	public void StopAllActuators ()
	{
		m_roboticsControllers.StopAllActuators();
	}

	public void AllUp()
	{
		m_roboticsControllers.SetAllActuatorSpeeds(1.0f);
	}

	public void AllDown()
	{
		m_roboticsControllers.SetAllActuatorSpeeds(-1.0f);
	}

	public void Wave()
	{
		m_roboticsControllers.SetAllActuatorSpeeds(-1.0f);
		//TODO Implement wait until all down, then commence wave function
	}

	public void Callibrate()
	{
		StartCoroutine(CallibrateCoroutine());
	}

	public void UpdateAllActuatorStates()
	{
		m_roboticsControllers.UpdateAllActuatorStates();
	}

	public void UpdateActuatorState(int actuatorID)
	{
		ActuatorState state = m_roboticsControllers.GetActuatorState (actuatorID);
		Debug.Log ("Actuator State: " + state.ToString());
	}

	public IEnumerator CallibrateCoroutine()
	{
		m_roboticsControllers.SetAllActuatorSpeeds(-1.0f);
		m_roboticsControllers.SetAllActuatorSpeeds(-1.0f);

		bool stillRetracting = true;
		while (stillRetracting) {
			ActuatorState [] states = m_roboticsControllers.GetAllActuatorStates ();
			stillRetracting = false;
			foreach (ActuatorState a in states) {
				if (!a.m_atInnerLimit) {
					stillRetracting = true;
				}
			}
			yield return new WaitForSeconds(1.0f);
		}

		//All should be retracted now

		m_roboticsControllers.SetAllActuatorSpeeds(1.0f);

		bool stillExtending = true;
		while (stillExtending) {
			ActuatorState [] states = m_roboticsControllers.GetAllActuatorStates ();
			stillExtending = false;
			foreach (ActuatorState a in states) {
				if (!a.m_atOuterLimit) {
					stillExtending = true;
				}
			}
			yield return new WaitForSeconds(1.0f);
		}

		//All should be extended now

		m_roboticsControllers.SetAllActuatorSpeeds(-1.0f);

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

	private IEnumerator WaitForAllActuatorsToRetract()
	{
		bool stillRetracting = true;
		while (stillRetracting) 
		{
			ActuatorState [] states = m_roboticsControllers.GetAllActuatorStates ();
			stillRetracting = false;
			foreach (ActuatorState a in states) 
			{
				if (!a.m_atInnerLimit) {
					stillRetracting = true;
					break;
				}
			}
			yield return new WaitForSeconds(1.0f);
		}
	}
}
