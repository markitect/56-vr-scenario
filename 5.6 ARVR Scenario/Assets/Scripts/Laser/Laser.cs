using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Networking;

//using UnityEngine.VR.WSA.Persistence;

[RequireComponent(typeof(LineRenderer))]
public class Laser : NetworkBehaviour
{
	private Color laserColor;

	[SyncVar]
	public LaserType laserType;
	
	[SyncVar]
	public float speed;
	
	[SyncVar]
	public float length;
	
	[SyncVar]
	public bool isFiring = false;

	[SyncVar]
	public int layerMaskValue;

	public ScoreKeeper scorer { get; private set; }
	
	public LayerMask collisionLayers;
	public LayerMask laserLayerMask;

	private LineRenderer lineRenderer;

	private List<Vector3> linePoints = new List<Vector3>();

	private Vector3 start;
	private Vector3 end;

	private Vector3 direction;
	private bool isScoring;
	private float scoreTimer;

	private bool isDissolving;

	private float distanceTraveled;
	
	private float timeToExist = 5f;

    private GameObject prismBeingHit;
	
	void Start()
	{
		this.lineRenderer = GetComponent<LineRenderer>();
		this.linePoints.Clear();
		this.start = this.gameObject.transform.position;
		this.lineRenderer.startWidth = .05f;
		this.laserColor = LaserData.Lasers[this.laserType].LaserColor;
		this.lineRenderer.startColor = laserColor;
		this.lineRenderer.endColor =  laserColor;
		this.linePoints.Add(start);
		this.end = this.start;
		this.linePoints.Add(end);
		this.direction = this.transform.forward;
		this.laserLayerMask |= 1 << layerMaskValue;
	}

	// Use this for initialization

	// Update is called once per frame
	void Update()
	{
		if (this.isFiring)
		{
			var step = Time.deltaTime * speed;
			this.end += this.direction * step;
			this.distanceTraveled += (this.direction * step).magnitude;
			this.linePoints[this.linePoints.Count - 1] = end;
			this.direction = Vector3.zero;
			
			this.lineRenderer.positionCount = this.linePoints.Count;
			this.lineRenderer.SetPositions(this.linePoints.ToArray());

			RecalculatePoints();

			if (this.distanceTraveled <= this.length)
			{
				RaycastForMovement();
			}
			else
			{
				this.isDissolving = true;
			}

			if (isScoring)
			{
				scoreTimer += Time.deltaTime;

				if (scorer != null && scoreTimer >= scorer.Rate)
				{
					scorer.Score();
				}
			}

			if (isDissolving)
			{
				// move the first point in the line to the 2nd point in the line by speed divided by the distance between the lines.

				if (linePoints.Count > 1)
				{
					var dissolveStart = linePoints[0];
					var lineFraction = speed * Time.deltaTime / Vector3.Distance(dissolveStart, linePoints[1]);

					// if the line fraction is 1 then we have reached point 2.  Delete the first point to shorten the line.
					if (lineFraction < 1)
					{
						linePoints[0] = Vector3.Lerp(dissolveStart, linePoints[1], lineFraction);
					}
					else
					{
						linePoints.RemoveAt(0);
					}
				}

				// this objects job is done as there is only 1 point left on the line.
				// this won't interfere with creation since it is regulated by the isDissolving flag.
				if (linePoints.Count <= 1)
				{
					this.isFiring = false;
					this.linePoints.Clear();
                    if(this.prismBeingHit != null)
                    {
                        this.prismBeingHit.GetComponent<LaserSplitter>().IsInUse = false;
                    }
					Destroy(this.gameObject);
				}
			}
		}
	}
	
	public void FireLaser(ScoreKeeper owner, float speed, float distance, LaserType laserType)
	{
        scorer = owner;
		this.speed = speed;
		this.length = distance;
		this.isFiring = true;
        this.laserType = laserType;
		this.laserColor = LaserData.Lasers[this.laserType].LaserColor;
		this.laserLayerMask = LaserData.Lasers[this.laserType].LaserLayer;
		Invoke("StartDisolve", this.timeToExist);
	}

	private void StartDisolve()
	{
		this.isDissolving = true;
	}

	private void RecalculatePoints()
	{
		// Grab the current start and current end points of the line
		var currentStart = this.linePoints.FirstOrDefault();
		var currentEnd = this.linePoints.LastOrDefault();

		// Get the direction of the first current line segment
		var newDirection = (this.linePoints[1] - this.linePoints[0]).normalized;

		// Clear the points and add the current start point back in
		this.linePoints.Clear();
		this.linePoints.Add(currentStart);

		// Keep track of how far the laser has traveled so far
		var distanceTraveled = this.distanceTraveled;

		// Iterate through the points of the line renderer and re-raycast to get new reflections if objects have moved
		for (var i = 0; i < this.lineRenderer.positionCount; ++i)
		{
			RaycastHit hit;
			// Raycast from the current point in the new direction the line should be traveling to see it collides with anything new
			if (Physics.Raycast(new Ray(this.linePoints[i], newDirection), out hit, 1000f, this.collisionLayers) &&
				(hit.point - this.linePoints[i]).magnitude < distanceTraveled)
			{
				var hitObject = hit.transform.gameObject;
				
				// Add the point where we collided to the list of line points
				this.linePoints.Add(hit.point);

				// Deduct the length of the line segment from the current distance traveled of the laser
				distanceTraveled -= (hit.point - this.linePoints[i]).magnitude;

				// Handle logic for the different types of object we can collide with
				switch (LayerMask.LayerToName(hitObject.layer))
				{
					case "Reflective":
						{
							// Reflect the direction traveled off of the collision point and set a new reflected direction to travel in
							newDirection = Vector3.Reflect((hit.point - this.linePoints[i]).normalized, hit.normal);
							break;
						}
					case "ScoreTarget":
						{
							// Start scoring and return since the laser has ended.
							this.isScoring = true;
							return;
						}
					case "Prism":
						{
							this.direction = Vector3.zero;
							this.prismBeingHit = hitObject.transform.parent.gameObject;
                            if (this.isFiring && !prismBeingHit.GetComponent<LaserSplitter>().IsInUse)
                            {
                                prismBeingHit.GetComponent<LaserSplitter>().SplitLaser(hitObject, this);
                            }
                            return;
                        }
					default:
						//If we hit anything else end the laser
						return;
				}
			}
			// if we still haven't reached the laser's total yet, keep it moving in the current set direction
			else if (this.distanceTraveled <= this.length)
			{
				this.linePoints.Add(this.linePoints.Last() + (newDirection*distanceTraveled));
				return;
			}
			//If the laser has traveled it's total distance just add the last end to it and return;
			else
			{ 
				this.linePoints.Add(currentEnd);
				return;
			}
		}
	}

	private void RaycastForMovement()
	{
		// Raycast to see if the end of the laser has collided with anything
		RaycastHit hit;
		this.direction = (this.end - this.start).normalized;
		if (Physics.Raycast(new Ray(this.start, this.direction), out hit, Vector3.Distance(start, end), this.collisionLayers))
		{
			// Handle logic for what it collided with
			var hitObject = hit.collider.gameObject;
			switch (LayerMask.LayerToName(hitObject.layer))
			{
				case "Reflective":
					var reflection = Vector3.Reflect(hit.point - this.start, hit.normal);
					this.direction = reflection.normalized;
					this.start = this.end;
					this.linePoints.Add(this.end);
					return;

				case "ScoreTarget":
					isScoring = true;
					return;
				case "Prism":
					this.direction = Vector3.zero;
					this.prismBeingHit = hitObject.transform.parent.gameObject;
                    if (this.isFiring && !this.prismBeingHit.GetComponent<LaserSplitter>().IsInUse)
                    {
                        this.prismBeingHit.GetComponent<LaserSplitter>().SplitLaser(hitObject, this);
                    }
					return;
				default:
					this.linePoints.Add(hit.point);
					this.direction = Vector3.zero;
					return;
			}
		}
	}
}
