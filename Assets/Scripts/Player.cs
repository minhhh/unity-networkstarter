using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

	public int moveX = 0;
	public int moveY = 0;
	public float moveSpeed = 0.2f;

	[SyncVar]
	public Color myColor;

    [SyncVar]
    public int myInt = 0;

	void Start()
	{
 		DontDestroyOnLoad(gameObject);
	}

	public override void OnStartClient()
	{
		Debug.Log("Player OnStartClient netId:" + netId + " color:" + this.myColor);
		GetComponent<MeshRenderer>().material.color = myColor;
	}
		
	[ClientRpc]
	void RpcPoke(int value)
	{
		Debug.Log("value:"+value);
	}

	void Update () 
	{
		if (!isLocalPlayer) {
			return;
		}
		
		// input handling for local player only
		int oldMoveX = moveX;
		int oldMoveY = moveY;
		
		moveX = 0;
		moveY = 0;

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			CmdLobby();
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			moveX -= 1;
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			moveX += 1;
		}
		if (Input.GetKey(KeyCode.UpArrow))
		{
            CmdUpdateMyInt(1);
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
            CmdUpdateMyInt(-1);
		}
		if (moveX != oldMoveX || moveY != oldMoveY)
		{
			CmdMove(moveX, moveY);
		}
	}
	
    [Command]
    public void CmdUpdateMyInt (int x)
    {
        myInt += x;

        RpcPoke (myInt);
    }

	[Command]
	public void CmdLobby()
	{
		var lobby = NetworkManager.singleton as NetworkLobbyManager;
		NetworkManager.singleton.ServerChangeScene(lobby.lobbyScene);
	}

	[Command]
	public void CmdMove(int x, int y)
	{
		moveX = x;
		moveY = y;
		transform.Translate(moveX * moveSpeed, moveY * moveSpeed, 0);
		base.SetDirtyBit(1);
	}
	
	[ServerCallback]
	public void FixedUpdate()
	{
		transform.Translate(moveX * moveSpeed, moveY * moveSpeed, 0);
	}
}
