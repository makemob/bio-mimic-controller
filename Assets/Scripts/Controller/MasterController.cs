using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UKI;

public class MasterController : MonoBehaviour, IMasterController
{
	enum ActuatorName
	{
		LeftFrontHip,
		LeftFrontKnee,
		LeftFrontAnkle,

		LeftMidHip,
		LeftMidKnee,
		LeftMidAnkle,

		LeftRearHip,
		LeftRearKnee,
		LeftRearAnkle,

		RightFrontHip,
		RightFrontKnee,
		RightFrontAnkle,

		RightMidHip,
		RightMidKnee,
		RightMidAnkle,

		RightRearHip,
		RightRearKnee,
		RightRearAnkle,

		LeftWingRotate,
		LeftWingRaise,
		RightWingRotate,
		RightWingRaise
	}

	public string m_configFile = "Config";
	public UKI.Config m_config;

	public RoboticsController m_roboticsControllers;
	public LightingController m_lightingControllers;

	public static MasterController Instance;

	private List<int> ALL_LEGS;
	private List<int> ALL_WINGS;
	private List<int> ALL_ANKLES;

	//
	// Unity interface
	//

	void Awake() 
	{ 
		Instance = this;
		Application.targetFrameRate = 30;

		LoadConfig();

		List<ActuatorName> legs = new List<ActuatorName> ();
		legs.Add(ActuatorName.LeftFrontHip);
		legs.Add(ActuatorName.LeftFrontKnee);
		legs.Add(ActuatorName.LeftFrontAnkle);

		legs.Add(ActuatorName.LeftMidHip);
		legs.Add(ActuatorName.LeftMidKnee);
		legs.Add(ActuatorName.LeftMidAnkle);

		legs.Add(ActuatorName.LeftRearHip);
		legs.Add(ActuatorName.LeftRearKnee);
		legs.Add(ActuatorName.LeftRearAnkle);

		legs.Add(ActuatorName.RightFrontHip);
		legs.Add(ActuatorName.RightFrontKnee);
		legs.Add(ActuatorName.RightFrontAnkle);

		legs.Add(ActuatorName.RightMidHip);
		legs.Add(ActuatorName.RightMidKnee);
		legs.Add(ActuatorName.RightMidAnkle);

		legs.Add(ActuatorName.RightRearHip);
		legs.Add(ActuatorName.RightRearKnee);
		legs.Add(ActuatorName.RightRearAnkle);

		List<ActuatorName> wings = new List<ActuatorName> ();
		wings.Add (ActuatorName.LeftWingRotate);
		wings.Add (ActuatorName.LeftWingRaise);
		wings.Add (ActuatorName.RightWingRotate);
		wings.Add (ActuatorName.RightWingRaise);


		List<ActuatorName> ankles = new List<ActuatorName>();
		ankles.Add (ActuatorName.LeftFrontAnkle);
		ankles.Add (ActuatorName.LeftMidAnkle);
		ankles.Add (ActuatorName.LeftRearAnkle);
		ankles.Add (ActuatorName.RightFrontAnkle);
		ankles.Add (ActuatorName.RightMidAnkle);
		ankles.Add (ActuatorName.RightRearAnkle);

		ALL_LEGS = new List<int> (legs.Count);
		GetActuatorIDsByName (legs, ref ALL_LEGS);

		ALL_WINGS = new List<int> (wings.Count);
		GetActuatorIDsByName (wings, ref ALL_WINGS);

		ALL_ANKLES = new List<int> (ankles.Count);
		GetActuatorIDsByName (ankles, ref ALL_ANKLES);
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
			a.enabled = false;
			return false;
		}

		bool registrationSuccess = m_roboticsControllers.RegisterActuator (a);

		if (registrationSuccess) 
		{
			if (a.GetID () == GetActuatorIDByName (ActuatorName.LeftWingRaise) ||
				a.GetID () == GetActuatorIDByName (ActuatorName.LeftWingRotate) ||
				a.GetID () == GetActuatorIDByName (ActuatorName.RightWingRaise) ||
				a.GetID () == GetActuatorIDByName (ActuatorName.RightWingRotate)) {
				m_roboticsControllers.SetActuatorAcceleration (a.GetID (), 0.8f);
			}
		}

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

	public void LegsOut()
	{
		m_roboticsControllers.SetActuatorSpeeds(ALL_LEGS, 1.0f);
	}

	public void LegsIn()
	{
		m_roboticsControllers.SetActuatorSpeeds(ALL_LEGS, -1.0f);
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

//	private bool AllActuatorsStopped(int[] ids)
//	{
//		return m_roboticsControllers.GetActuatorState(id).m_atInnerLimit;
//	}

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

	private bool AllActuatorsAtInnerLimitOrRetracted()
	{
		ActuatorState [] states = m_roboticsControllers.GetAllActuatorStates ();
		foreach (ActuatorState a in states) 
		{
			if (!(a.m_atInnerLimit || a.m_predictedExtension < 5.0f))
				return false;
		}

		return true;
	}

	private bool AllLegsAtInnerLimit()
	{
		return ActuatorsAtInnerLimit(ALL_LEGS);
	}
		
	private bool AllLegsAtInnerLimitOrRetracted()
	{
		return ActuatorsAtInnerLimitOrRetracted (ALL_LEGS);
	}

	private bool ActuatorsAtInnerLimit(List<int> ids)
	{
		ActuatorState [] states = m_roboticsControllers.GetActuatorStates (ids);
		foreach (ActuatorState a in states) 
		{
			if (!(a.m_atInnerLimit))
				return false;
		}

		return true;
	}

	private bool ActuatorsAtInnerLimitOrRetracted(List<int> ids)
	{
		ActuatorState [] states = m_roboticsControllers.GetActuatorStates (ids);
		foreach (ActuatorState a in states) 
		{
			if (!(a.m_atInnerLimit || a.m_predictedExtension < 5.0f))
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

	private bool m_looping = false;
	public void StartLoopTest()
	{
		m_looping = true;
		StopAllCoroutines ();
		ResetEmergencyStopForAll ();
		StartCoroutine ("LoopTestCoroutine");
	}

	public void StopLoopTest()
	{
		m_looping = false;
		StopCoroutine ("LoopTestCoroutine");
		StopAllActuators ();
	}

	public void BoardingPose()
	{
		ResetEmergencyStopForAll ();

		StopLegs ();

		int hip = GetActuatorIDByName (ActuatorName.LeftFrontHip);
		int knee = GetActuatorIDByName (ActuatorName.LeftFrontKnee);
		int ankle = GetActuatorIDByName (ActuatorName.LeftFrontAnkle);

		int[] actuators = { hip, knee, ankle };
		float[] positions = { 100.0f, 0.0f, 50.0f };

		MoveToPose (actuators, positions);
	}

	bool IsAnkle(int actuatorID)
	{
		return ALL_ANKLES.Contains (actuatorID);
	}
		
	public void DrivingPose()
	{
		//Debug.LogError ("Driving pose not implemented!");
		StopLegs();

		ResetEmergencyStopForAll ();


		int size = ALL_LEGS.Count;
		List<float> positions = new List<float>(size);
		for (int i = 0; i < size; i++) 
		{
			if (ALL_LEGS[i] == GetActuatorIDByName(ActuatorName.LeftFrontAnkle) || 
				ALL_LEGS[i] == GetActuatorIDByName(ActuatorName.RightFrontAnkle))
				positions.Add(80.0f);	//front ankles 80mm out. everything else 0
			else
				positions.Add (0.0f);
		}
		
		StartCoroutine (MoveToPose_Coroutine (ALL_LEGS.ToArray(), positions.ToArray()));
	}

	public void TestPose()
	{
		Debug.LogError ("Test pose not implemented!");
	}

	public void WingsPose1()
	{
		int leftWingRotate = GetActuatorIDByName (ActuatorName.LeftWingRotate);
		int leftWingRaise = GetActuatorIDByName (ActuatorName.LeftWingRaise);
		int rightWingRotate = GetActuatorIDByName (ActuatorName.RightWingRotate);
		int rightWingRaise = GetActuatorIDByName (ActuatorName.RightWingRaise);

		int[] actuators = { leftWingRotate, leftWingRaise, rightWingRotate, rightWingRaise };
		float[] positions = { 100.0f, 100.0f, 100.0f, 100.0f };

		MoveToPose (actuators, positions);
	}

	public void WingsPose2()
	{
		int leftWingRotate = GetActuatorIDByName (ActuatorName.LeftWingRotate);
		int leftWingRaise = GetActuatorIDByName (ActuatorName.LeftWingRaise);
		int rightWingRotate = GetActuatorIDByName (ActuatorName.RightWingRotate);
		int rightWingRaise = GetActuatorIDByName (ActuatorName.RightWingRaise);

		int[] actuators = { leftWingRotate, leftWingRaise, rightWingRotate, rightWingRaise };
		float[] positions = { 0.0f, 100.0f, 0.0f, 100.0f };

		MoveToPose (actuators, positions);
	}

	public void WingsPose3()
	{
		int leftWingRotate = GetActuatorIDByName (ActuatorName.LeftWingRotate);
		int leftWingRaise = GetActuatorIDByName (ActuatorName.LeftWingRaise);
		int rightWingRotate = GetActuatorIDByName (ActuatorName.RightWingRotate);
		int rightWingRaise = GetActuatorIDByName (ActuatorName.RightWingRaise);

		int[] actuators = { leftWingRotate, leftWingRaise, rightWingRotate, rightWingRaise };
		float[] positions = { 100.0f, 0.0f, 100.0f, 0.0f };

		MoveToPose (actuators, positions);
	}


	private IEnumerator LoopTestCoroutine()
	{
		m_roboticsControllers.SetActuatorSpeeds(ALL_LEGS, -1.0f);
		yield return new WaitUntil (AllLegsAtInnerLimitOrRetracted);	//Need to ensure switches set on already retracted actuators for this to work
		yield return new WaitForSeconds(2.0f);

		while (m_looping) 
		{
			m_roboticsControllers.SetActuatorSpeeds(ALL_LEGS, 1.0f);
			yield return new WaitForSeconds (5.0f);
			m_roboticsControllers.SetActuatorSpeeds(ALL_LEGS, -1.0f);
			yield return new WaitUntil (AllLegsAtInnerLimitOrRetracted);//WaitForSeconds (5.0f);
		}
	}

	private int GetActuatorIDByName(ActuatorName name)
	{
		return m_config.GetActuatorID(name.ToString());
	}

	private void GetActuatorIDsByName(List<ActuatorName> names, ref List<int> ids)
	{
		foreach(var name in names)
		{
			ActuatorConfig a = new ActuatorConfig ();
			if (m_config.FindActuator (name.ToString(), ref a)) 
			{
				if (ids.Contains (a.id)) {
					Debug.LogError ("Multiple actuators named " + name.ToString());
					continue;
				}
				ids.Add (a.id);
			}
		}
	}

	private void MoveToPose(int [] actuatorIDs, float [] extensions)
	{
		StartCoroutine(MoveToPose_Coroutine(actuatorIDs, extensions));
	}

	private IEnumerator MoveToPose_Coroutine(int [] ids, float [] positions)
	{
		int completed = 0;
		for(int i=0; i < ids.Length; i++)
		{
			StartCoroutine(MoveActuatorToPosition_Coroutine (ids[i], positions[i], () => { 
				completed = completed + 1;
				//Debug.Log("Completed: " + completed);
			}));
		}

		yield return new WaitUntil (() => { return completed == ids.Length; });
		//Debug.Log("All Completed: " + completed);
	}

	private IEnumerator MoveActuatorToPosition_Coroutine(int id, float desiredExtension, Action onFinished)
	{
		m_roboticsControllers.MoveActuatorTowardsPosition (id, desiredExtension);
		yield return new WaitUntil (() => m_roboticsControllers.CloseEnoughToPosition (id, desiredExtension));
		m_roboticsControllers.StopActuator (id);

		onFinished ();
	}

	public void RotateWings(float direction)
	{
		int leftWingRotate = GetActuatorIDByName (ActuatorName.LeftWingRotate);
		int rightWingRotate = GetActuatorIDByName (ActuatorName.RightWingRotate);
		m_roboticsControllers.SetActuatorSpeed (leftWingRotate, direction);
		m_roboticsControllers.SetActuatorSpeed (rightWingRotate, direction);

	}

	public void RaiseWings(float direction)
	{
		int leftWingRaise = GetActuatorIDByName (ActuatorName.LeftWingRaise);
		int rightWingRaise = GetActuatorIDByName (ActuatorName.RightWingRaise);
		m_roboticsControllers.SetActuatorSpeed (leftWingRaise, direction);
		m_roboticsControllers.SetActuatorSpeed (rightWingRaise, direction);

	}

	public void StopWings()
	{
		foreach (int id in ALL_WINGS) 
		{
			m_roboticsControllers.StopActuator (id);
		}
	}

	public void StopLegs()
	{
		StopCoroutine ("LoopTestCoroutine");
		StopCoroutine ("MoveToPose_Coroutine");

		foreach (int id in ALL_LEGS) 
		{
			m_roboticsControllers.StopActuator (id);
		}
	}

	//
	// External networking interface
	//

	public void SetUKIWingMode(int mode)
	{
		Debug.Log ("Wing mode set to: " + mode);
		Stop ();

		switch (mode) 
		{
		case 0:
			StopWings ();
			break;
		case 1:
			RotateWings (1.0f);
			break;
		case 2:
			RotateWings (-1.0f);
			break;
		case 3:
			RaiseWings (1.0f);
			break;
		case 4:
			RaiseWings (-1.0f);
			break;
		default:
			Debug.LogWarning ("Unhandled mode: " + mode);
			break;
		}
	}

	public void SetUKILegMode(int mode)
	{
		Debug.Log ("Leg mode set to: " + mode);
		Stop ();

		switch (mode) 
		{
		case 0:
			DrivingPose ();
			break;
		case 1:
			BoardingPose ();
			break;
		case 2:
			StartLoopTest ();
			break;
		case 3:
			StopLegs ();
			break;
		default:
			Debug.LogWarning ("Unhandled mode: " + mode);
			break;
		}
	}

	public void SetUKISpeed(float speed)
	{
		Debug.Log ("Speed change not yet implemented. Speed: " + speed);
	}
}
