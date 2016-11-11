using UnityEngine.Networking;


// this is the 'player' NetworkBehaviour which represents a connection from a client device
public class Connection : NetworkBehaviour
{
	[SyncVar]	// this field is synchronised back to remote client
	public int m_fromServer;


	private void Start()
	{
		if (ServerNetworkManager.Instance != null)
			ServerNetworkManager.Instance.AddConnection(this);
	}

	private void OnDestroy()
	{
		if (ServerNetworkManager.Instance != null)
			ServerNetworkManager.Instance.RemoveConnection(this);
	}

	// commands are sent from client to server
	// commands are executed only on the server-side instance of the client
	[Command]
	private void CmdSetUKIMode(int mode)
	{
		ServerNetworkManager.Instance.SetUKIMode(this, mode);
	}

	[Command]
	private void CmdSetUKISpeed(float speed)
	{
		ServerNetworkManager.Instance.SetUKISpeed(this, speed);
	}


	public void RefreshStatus(UKIStatus status)
	{
		// TODO

		m_fromServer++;	// test
	}
}

