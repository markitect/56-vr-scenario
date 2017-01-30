using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {

	public Transform playerTransform;
    public float delatTimeBeforeTeleport;
    public int deltaBlink;
    public GUITexture fadeTexture;
    public float fadeInSpeed;

    float timer;
    int count;
    bool teleportBigger = false;
    Vector3 targetTeleportPosition = new Vector3();

    // Use this for initialization

    // Update is called once per frame
    void Update () {
		Vector3 start = this.gameObject.transform.position;
		Vector3 end = (this.transform.forward * 20) + start;

        // Fade in texture if any
        Color oldColor = fadeTexture.color;
        oldColor.a = Mathf.Lerp(oldColor.a, 0, fadeInSpeed);
        fadeTexture.color = oldColor;

		RaycastHit hit;
		if (Physics.Raycast(start, end, out hit))
		{
            if (hit.collider.gameObject.name == "TeleportPlatform")
            {
                // When hit a teleport platform, update the target teleport
                targetTeleportPosition = hit.collider.gameObject.transform.position;

                if (targetTeleportPosition == hit.collider.gameObject.transform.position && timer >= delatTimeBeforeTeleport)
                {
                    // Black the screen
                    Color currentColor = fadeTexture.color;
                    currentColor.a = 1;
                    fadeTexture.color = currentColor;

                    // Move player position
                    playerTransform.position = hit.collider.gameObject.transform.position;
                    playerTransform.Translate(Vector3.up * 0.5f);

                    Reset();

                    // Need to make sure to change the teleport's scale back
                    hit.collider.gameObject.transform.localScale = new Vector3(1.0f, 0.1f, 1.0f);
                    teleportBigger = false;
                }
                else if (targetTeleportPosition == hit.collider.gameObject.transform.position)
                {
                    // Player still looking at the same teleport but not long enough before moving
                    // Increase timer
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
                    // Player look at some other teleport
                    Reset();
                }
            }
            else
            {
                // Player look at somewhere else
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
