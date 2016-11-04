using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[InitializeOnLoad]
public class EditorCameraSpeed
	: EditorWindow
{
	[MenuItem("Tools/Camera Speed &S")]
	public static void CameraSpeed()
	{
		var window = GetWindow<EditorCameraSpeed>();

		// TODO: gets cleared each frame?
		SceneView.onSceneGUIDelegate += OnSceneDelegate;
	}

	static void CameraSpeedUpdate()
	{
		var e = Event.current;

		// Tools.s_LockedViewTool is ViewTool.FPS when holding right-click down
		// SceneView.OnGUI() calls SceneViewMotion.DoViewTool(self)
		// SceneViewMotion.DoViewTool(): key down event: process WASD and add to s_Motion vector
		// SceneViewMotion.DoViewTool(): layout event: view.pivot change by s_Motion * internal dt tracking

		// solution 1: try and modify speed values during this update
		// -> doesn't work because the timing of events and this call is different, and it is out of sync

		// solution 2: get a callback in our code somewhere during the gui event handler so we can modify some values
		// -> doesnt work because there seems to be no delegates or callbacks during SceneView.OnGUI that we can hook into

		// solution 3: modify ilcode bytes of SceneViewMotion.GetMovementDirection() to return a modified value
		// -> can modify ?

		// solution 4: replace kFPSPref* input keys and move the main window pivot ourselves
		// -> gotta implement a lot?

		// solution 5: build custom permanent control and listen for OnGUI()
		// -> will get events when not focused?
		// -> will get events that are consumed by scene view?
	}

	static float cameraMoveSpeed = 10.0f;
	static float cameraMoveSpeedCtrl = 1.0f;

	public void OnGUI()
	{
		var event_ = Event.current;
		var controlID = GUIUtility.GetControlID(FocusType.Passive);
		var eventType = event_.GetTypeForControl(controlID);

		cameraMoveSpeed = EditorGUILayout.Slider(cameraMoveSpeed, 0.0f, 10.0f);
		cameraMoveSpeedCtrl = EditorGUILayout.Slider(cameraMoveSpeedCtrl, 0.1f, 1.0f);

		SceneView.onSceneGUIDelegate += OnSceneDelegate;
	}

	public static void OnSceneDelegate(SceneView sceneView)
	{
		if (Event.current.type != EventType.Layout)
			return;

		var tools_type = typeof(UnityEditor.Tools);
		var locked_view_tool_field = (FieldInfo)tools_type.GetField("s_LockedViewTool", BindingFlags.NonPublic | BindingFlags.Static);
		var locked_view_tool = (ViewTool)locked_view_tool_field.GetValue(null);

		if (locked_view_tool != ViewTool.FPS)
			return;

		var scene_view_assembly = Assembly.GetAssembly(typeof(UnityEditor.SceneView));
		var scene_view_motion_type = scene_view_assembly.GetType("UnityEditor.SceneViewMotion");
		var scene_view_flyspeed_type = scene_view_assembly.GetType("UnityEditor.SceneViewMotion");

		var flyspeed_field = (FieldInfo)scene_view_motion_type.GetField("s_FlySpeed", BindingFlags.NonPublic | BindingFlags.Static);
		var flyspeed = flyspeed_field.GetValue(null);
		var flyspeed_modified = (float)flyspeed;

		flyspeed_modified = cameraMoveSpeed;

		if (Event.current.control)
			flyspeed_modified = cameraMoveSpeedCtrl;

		flyspeed_field.SetValue(null, flyspeed_modified);

		// we can stop input with this
		//Event.current.Use();
	}
}