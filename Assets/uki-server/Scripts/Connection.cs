using UnityEngine.Networking;


// This is the player gameobject representing a connection from a client.
// This class is instantiated on both the client and server.
// [Command] methods execute code on the server-side instance only.
// [ClientRpc] methods execute code on the client-side instance only.


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

	// commands (client -> server) - code executed on server-side only
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


	// client rpcs (server -> client) - code executed on client-side only
	[ClientRpc]
	private void RpcRefreshStatus(UKIStatus status) { }


	public void RefreshStatus(UKIStatus status)
	{
		RpcRefreshStatus(status);

		m_fromServer++;	// test
	}
}

