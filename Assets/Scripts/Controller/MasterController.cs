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
		//m_lightingControllers.Startup ();
	}

	public void Shutdown ()
	{
		m_roboticsControllers.Shutdown();
		//m_lightingControllers.Shutdown();
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
		//m_lightingControllers.Stop();
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

	public void SetActuatorCallibration(int actuatorID, CallibrationResults results)
	{
		m_roboticsControllers.SetActuatorCallibration (actuatorID, results);
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
		StartCoroutine(CallibrateAllCoroutine());
	}

	public void CallibrateActuator(int id)
	{
		//HACK: Let's test for now.
		TestActuator (id);
		//StartCoroutine(CallibrateActuatorCoroutine(id));
	}

	public void TestActuator(int id)
	{
		StartCoroutine (ActuatorAccuracyTestCoroutine (id));
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

	private IEnumerator CallibrateAllCoroutine()
	{
		CallibrationResults Results = new CallibrationResults ();

		Debug.Log ("===============================================");
		Debug.Log ("                 CALLIBRATING                  ");
		Debug.Log ("===============================================");

		Debug.Log ("CALLIBRATION: RETRACTING...");
		m_roboticsControllers.SetAllActuatorSpeeds(-1.0f);
		yield return new WaitUntil (AllActuatorsAtInnerLimit);
		yield return new WaitForSeconds (1.0f);

		Debug.Log ("CALLIBRATION: RETRACTION COMPLETE. EXTENDING...");
		m_roboticsControllers.SetAllActuatorSpeeds(1.0f);
		float startExtensionTime = Time.realtimeSinceStartup;
		yield return new WaitUntil (AllActuatorsAtOuterLimit);
		float endExtensionTime = Time.realtimeSinceStartup;
		yield return new WaitForSeconds (1.0f);

		Debug.Log ("CALLIBRATION: EXTENSION COMPLETE. RETRACTING...");
		m_roboticsControllers.SetAllActuatorSpeeds(-1.0f);
		float startRetractionTime = Time.realtimeSinceStartup;
		yield return new WaitUntil (AllActuatorsAtInnerLimit);
		float endRetractionTime = Time.realtimeSinceStartup;

		//Collate results
		Results.m_extensionTime = endExtensionTime - startExtensionTime;
		Results.m_retractionTime = endRetractionTime - startRetractionTime;

		Debug.Log ("===============================================");
		Debug.Log ("           CALLIBRATION RESULTS                ");
		Debug.Log ("");
		Debug.Log (Results.ToString());
		Debug.Log ("");
		Debug.Log ("===============================================");

		//m_roboticsControllers.SetActuatorSpeed

	}

	private IEnumerator CallibrateActuatorCoroutine(int id)
	{
		CallibrationResults Results = new CallibrationResults ();

		Debug.Log ("===============================================");
		Debug.Log ("           CALLIBRATING ACTUATOR " + id);
		Debug.Log ("===============================================");

		Debug.Log ("CALLIBRATION: RETRACTING...");
		m_roboticsControllers.SetActuatorSpeed(id, -1.0f);
		yield return new WaitUntil (() => ActuatorAtInnerLimit(id));
		yield return new WaitForSeconds (1.0f);

		Debug.Log ("CALLIBRATION: RETRACTION COMPLETE. EXTENDING...");
		m_roboticsControllers.SetActuatorSpeed(id, 1.0f);
		float startExtensionTime = Time.realtimeSinceStartup;
		yield return new WaitUntil (() => ActuatorAtOuterLimit(id));
		float endExtensionTime = Time.realtimeSinceStartup;
		yield return new WaitForSeconds (1.0f);

		Debug.Log ("CALLIBRATION: EXTENSION COMPLETE. RETRACTING...");
		m_roboticsControllers.SetActuatorSpeed(id, -1.0f);
		float startRetractionTime = Time.realtimeSinceStartup;
		yield return new WaitUntil (() => ActuatorAtInnerLimit(id));
		float endRetractionTime = Time.realtimeSinceStartup;

		//Collate results
		Results.m_extensionTime = endExtensionTime - startExtensionTime;
		Results.m_retractionTime = endRetractionTime - startRetractionTime;

		Debug.Log ("===============================================");
		Debug.Log ("           CALLIBRATION RESULTS                ");
		Debug.Log ("");
		Debug.Log (Results.ToString());
		Debug.Log ("");
		Debug.Log ("===============================================");

	}

	private IEnumerator ActuatorAccuracyTestCoroutine(int id)
	{
		Debug.Log ("===============================================");
		Debug.Log ("           TESTING ACTUATOR " + id);
		Debug.Log ("===============================================");

		Debug.Log ("TESTING: RETRACTING...");
		m_roboticsControllers.SetActuatorSpeed(id, -1.0f);
		yield return new WaitUntil (() => ActuatorAtInnerLimit(id));
		yield return new WaitForSeconds (5.0f);

		Debug.Log ("TESTING: EXTENDING FOR 15sec...");
		m_roboticsControllers.SetActuatorSpeed(id, 1.0f);
		yield return new WaitForSeconds (15.0f);
		m_roboticsControllers.SetActuatorSpeed(id, 0.0f);
		Debug.Log ("Expected extension: " + m_roboticsControllers.GetActuatorState(id).m_predictedExtension);
		//yield return new WaitForSeconds (60.0f);

		/*
		Debug.Log ("TESTING: EXTENSION COMPLETE. RETRACTING...");
		m_roboticsControllers.SetActuatorSpeed(id, -1.0f);
		yield return new WaitUntil (() => ActuatorAtInnerLimit(id));

		Debug.Log ("TESTING: EXTENDING FOR 10sec...");
		m_roboticsControllers.SetActuatorSpeed(id, 1.0f);
		yield return new WaitForSeconds (10.0f);
		m_roboticsControllers.SetActuatorSpeed(id, 0.0f);
		Debug.Log ("Expected extension: " + m_roboticsControllers.GetActuatorState(id).m_predictedExtension);
		yield return new WaitForSeconds (60.0f);

		Debug.Log ("TESTING: EXTENSION COMPLETE. RETRACTING...");
		m_roboticsControllers.SetActuatorSpeed(id, -1.0f);
		yield return new WaitUntil (() => ActuatorAtInnerLimit(id));

		Debug.Log ("TESTING: EXTENDING FOR 20sec...");
		m_roboticsControllers.SetActuatorSpeed(id, 1.0f);
		yield return new WaitForSeconds (20.0f);
		m_roboticsControllers.SetActuatorSpeed(id, 0.0f);
		Debug.Log ("Expected extension: " + m_roboticsControllers.GetActuatorState(id).m_predictedExtension);
		yield return new WaitForSeconds (60.0f);

		Debug.Log ("TESTING: EXTENSION COMPLETE. RETRACTING...");
		m_roboticsControllers.SetActuatorSpeed(id, -1.0f);
		yield return new WaitUntil (() => ActuatorAtInnerLimit(id));
		*/
	}

	private bool ActuatorAtInnerLimit(int id)
	{
		return m_roboticsControllers.GetActuatorState(id).m_atInnerLimit;
	}

	private bool ActuatorAtOuterLimit(int id)
	{
		return m_roboticsControllers.GetActuatorState(id).m_atInnerLimit;
	}

	private bool AllActuatorsAtInnerLimit()
	{
		ActuatorState [] states = m_roboticsControllers.GetAllActuatorStates ();
		foreach (ActuatorState a in states) 
		{
			if (!a.m_atInnerLimit)
				return false;
		}

		return true;
	}

	private bool AllActuatorsAtOuterLimit()
	{
		ActuatorState [] states = m_roboticsControllers.GetAllActuatorStates ();
		foreach (ActuatorState a in states) 
		{
			if (!a.m_atOuterLimit)
				return false;
		}

		return true;
	}

	public void ResetEmergencyStopForAll()
	{
		m_roboticsControllers.ResetEmergencyStopForAll ();
	}
}
