using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemInteraction : NetworkBehaviour {
    public Transform itemHolder;

    Camera fpsCamera;

    void Start () {
        fpsCamera = GetComponentInChildren<Camera>();
	}
	
	void Update () {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            CmdInteractionItem();
        }
    }

    [Command]
    void CmdInteractionItem()
    {
        if (itemHolder.childCount > 0)
        {
            RpcThrowItem();
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position,
                            fpsCamera.transform.forward,
                            out hit,
                            4f,
                            1 << LayerMask.NameToLayer("Floor")))
        {
            print(hit.transform.name);
            DoorButton door = hit.transform.gameObject.GetComponent<DoorButton>();
            if (door != null)
                door.ToggleDoor();
            Item item = hit.transform.gameObject.GetComponent<Item>();
            if (item != null)
                RpcPickup(hit.transform.GetComponentInParent<NetworkIdentity>().netId);
        }
    }

    [ClientRpc]
    void RpcPickup(NetworkInstanceId netId)
    {
        if (itemHolder.childCount == 0)
        {
            Transform item = ClientScene.FindLocalObject(netId).transform;
            item.GetComponent<Rigidbody>().isKinematic = true;
            item.GetComponent<NetworkTransform>().enabled = false;
            item.SetParent(itemHolder);
            item.transform.position = itemHolder.transform.position;
        }
    }

    [ClientRpc]
    private void RpcThrowItem()
    {
        if (itemHolder.childCount > 0)
        {
            Transform item = itemHolder.GetChild(0);
            item.SetParent(null);
            item.GetComponent<Rigidbody>().isKinematic = false;
            item.GetComponent<NetworkTransform>().enabled = true;
            item.GetComponent<Rigidbody>().AddForce(fpsCamera.transform.forward * 700f);
        }
    }


}
