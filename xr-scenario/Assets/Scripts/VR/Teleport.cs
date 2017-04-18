using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {

	public Transform playerTransform;
	public GameObject gazeCursor;

    public float deltaTimeBeforeTeleport;
    public int deltaBlink;
    public GUITexture fadeTexture;
    public float fadeInSpeed;

    float timer;
    int count;
    bool teleportBigger = false;
	GameObject targetTeleportPlatform;

	RaycastHit hit;
	Vector3 raycastStart; 
	Vector3 raycastDirection;
	int teleportPlatformLayerMask = 1 << 10;
	float maxDistance = 20.0f;

    // Update is called once per frame
    void Update () {
		raycastStart = this.gameObject.transform.position;
		raycastDirection = this.gameObject.transform.forward;

        // Fade in texture if any
        Color oldColor = fadeTexture.color;
        oldColor.a = Mathf.Lerp(oldColor.a, 0, fadeInSpeed);
        fadeTexture.color = oldColor;

		gazeCursor.SetActive( false );

		if (Physics.Raycast(raycastStart, raycastDirection, out hit, maxDistance) && hit.collider.gameObject.name == "TeleportPlatform")
		{
            // When hit a teleport platform, update the target teleport
			targetTeleportPlatform = hit.collider.gameObject;
			gazeCursor.transform.position = hit.point;
			gazeCursor.SetActive( true );

            if (timer >= deltaTimeBeforeTeleport)
            {
                // Black the screen
                Color currentColor = fadeTexture.color;
                currentColor.a = 1;
                fadeTexture.color = currentColor;

                // Move player position
				playerTransform.position = targetTeleportPlatform.transform.position;
                playerTransform.Translate(Vector3.up * 0.5f);

                Reset();
            }
            else
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
						targetTeleportPlatform.transform.localScale = new Vector3(1.0f, 0.1f, 1.0f);
                    }
                    else
                    {
						targetTeleportPlatform.transform.localScale = new Vector3(1.5f, 0.1f, 1.5f);
                    }
                    teleportBigger = !teleportBigger;
                }
            }
        }
        else
        {
            // Player look at somewhere else
            Reset();
        }
	}

    void Reset()
    {
        timer = 0f;
        count = 0;

		if (teleportBigger)
		{
			// Need to make sure to change the teleport's scale back
			targetTeleportPlatform.transform.localScale = new Vector3(1.0f, 0.1f, 1.0f);
			teleportBigger = false;
		}
    }
}
