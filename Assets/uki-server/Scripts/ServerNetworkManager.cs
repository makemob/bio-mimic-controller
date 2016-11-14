using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.Events;

public struct UKIStatus
{
	public uint netId;     // netId of client that last changed UKIStatus
	public int legMode;
	public float legSpeed;
	public int wingMode;
	public float wingSpeed;
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
	private float m_timeSinceLastRefresh;


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
		else if ((m_timeSinceLastRefresh += Time.unscaledDeltaTime) >= 2.0f)
		{
			// send refresh at minimum every 2 seconds (to ping all clients)
			RefreshAllClients();
		}
	}

	public void SetUKILegMode(Connection connection, int mode)
	{
		Debug.Log("client " + connection.netId + ": UKILegMode=" + mode);
		m_status.legMode = mode;
		m_status.netId = connection.netId.Value;
		RefreshAllClients();

		//OnUKISetMode.Invoke(mode);
		MasterController.Instance.SetUKIMode(mode);
	}

	public void SetUKILegSpeed(Connection connection, float speed)
	{
		Debug.Log("client " + connection.netId + ": UKILegSpeed=" + speed);
		m_status.legSpeed = speed;
		m_status.netId = connection.netId.Value;
		RefreshAllClients();

		//OnUKISetSpeed.Invoke(speed);
		MasterController.Instance.SetUKISpeed(speed);
	}

	public void SetUKIWingMode(Connection connection, int mode)
	{
		Debug.Log("client " + connection.netId + ": UKIWingMode=" + mode);
		m_status.wingMode = mode;
		m_status.netId = connection.netId.Value;
		RefreshAllClients();
	}

	public void SetUKIWingSpeed(Connection connection, float speed)
	{
		Debug.Log("client " + connection.netId + ": UKIWingSpeed=" + speed);
		m_status.wingSpeed = speed;
		m_status.netId = connection.netId.Value;
		RefreshAllClients();
	}

	private void RefreshAllClients()
	{
		// acknowledge state change by sending UKIStatus back to all connected clients
		foreach (Connection connection in m_connections)
		{
			connection.RefreshStatus(m_status);
		}

		m_timeSinceLastRefresh = 0.0f;
	}
}

