using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.Events;

public struct UKIStatus
{
	public uint netId;     // netId of client that last changed UKIStatus
	public int mode;
	public float speed;
}

//[System.Serializable]
//public class UKIEventFloat : UnityEvent<float> {}

//[System.Serializable]
//public class UKIEventInt : UnityEvent<int> {}

public class ServerNetworkManager : MonoBehaviour
{
	public static ServerNetworkManager Instance { get; private set; }

	public NetworkManager m_networkManager;

	//public UKIEventFloat OnUKISetSpeed;
	//public UKIEventInt OnUKISetMode;

	private List<Connection> m_connections = new List<Connection>();
	private UKIStatus m_status;


	private void Awake()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		if (Instance == this)
			Instance = null;
	}

	public void AddConnection(Connection connection)
	{
		// a client has connected
		Debug.Log("client " + connection.netId + ": connected");
		m_connections.Add(connection);
	}

	public void RemoveConnection(Connection connection)
	{
		// a client has disconnected
		Debug.Log("client " + connection.netId + ": disconnected");
		m_connections.Remove(connection);
	}

	private void Update()
	{
		// ensure server is running
		if (!NetworkServer.active)
		{
			Debug.Log("starting server on port " + m_networkManager.networkPort);
			m_networkManager.StartServer();
		}
	}

	public void SetUKIMode(Connection connection, int mode)
	{
		Debug.Log("client " + connection.netId + ": UKIMode=" + mode);
		m_status.mode = mode;
		m_status.netId = connection.netId.Value;
		RefreshAllClients();

		//OnUKISetMode.Invoke (mode);
		MasterController.Instance.SetUKIMode(mode);
	}

	public void SetUKISpeed(Connection connection, float speed)
	{
		Debug.Log("client " + connection.netId + ": UKISpeed=" + speed);
		m_status.speed = speed;
		m_status.netId = connection.netId.Value;
		RefreshAllClients();

		//OnUKISetSpeed.Invoke (speed);
		MasterController.Instance.SetUKISpeed(speed);
	}

	private void RefreshAllClients()
	{
		// acknowledge state change by sending UKIStatus back to all connected clients
		foreach (Connection connection in m_connections)
		{
			connection.RefreshStatus(m_status);
		}
	}
}

