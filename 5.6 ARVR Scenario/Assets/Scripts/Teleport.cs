using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {

	public Transform playerTransform;
    public float delatTimeBeforeTeleport;
    public int deltaBlink;

    float timer;
    int count;
    bool teleportBigger = false;
    Vector3 targetTeleportPosition = new Vector3();

    // Use this for initialization

    // Update is called once per frame
    void Update () {
		Vector3 start = this.gameObject.transform.position;
		Vector3 end = (this.transform.forward * 20) + start;

		RaycastHit hit;
		if (Physics.Raycast(start, end, out hit))
		{
            if (hit.collider.gameObject.name == "TeleportPlatform")
            {
                targetTeleportPosition = hit.collider.gameObject.transform.position;
                if (targetTeleportPosition == hit.collider.gameObject.transform.position && timer >= delatTimeBeforeTeleport)
                {
                    playerTransform.position = hit.collider.gameObject.transform.position;
                    playerTransform.Translate(Vector3.up * 0.5f);
                    Reset();

                    // Need to make sure to change the teleport's scale back
                    hit.collider.gameObject.transform.localScale = new Vector3(1.0f, 0.1f, 1.0f);
                    teleportBigger = false;
                }
                else if (targetTeleportPosition == hit.collider.gameObject.transform.position)
                {
                    timer += Time.deltaTime;

                    // Make the target teleport blink by changing its local scale
                    count++;
                    if (count % deltaBlink == 0)
                    {
                        if (teleportBigger)
                        {
                            hit.collider.gameObject.transform.localScale = new Vector3(1.5f, 0.1f, 1.5f);
                        }
                        else
                        {
                            hit.collider.gameObject.transform.localScale = new Vector3(1.0f, 0.1f, 1.0f);
                        }
                        teleportBigger = !teleportBigger;
                    }
                }
                else
                {
                    Reset();
                }
            }
            else
            {
                Reset();
            }
		}
	}

    void Reset()
    {
        timer = 0f;
        count = 0;
        targetTeleportPosition = new Vector3();
    }
}
