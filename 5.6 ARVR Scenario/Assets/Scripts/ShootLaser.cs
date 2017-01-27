using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.VR.WSA.Persistence;

[RequireComponent(typeof(LineRenderer))]
public class ShootLaser : MonoBehaviour
{
	public Color laserColor { get; set; }
	public float speed { get; set; }
	public float length { get; set; }
	public bool isFiring { get; private set; }

	public LayerMask collisionLayers;
	public LayerMask laserLayerMask;

	private LineRenderer lineRenderer;

	private List<Vector3> linePoints = new List<Vector3>();

	private Vector3 start;
	private Vector3 end;
	private Vector3 direction;
	private bool isScoring;
	private ScoreKeeper scorer;
	private float scoreTimer;

	private bool isDissolving;

	private float laserDistance;

	private float startTime;


    // Use this for initialization
    void Start()
	{
		this.lineRenderer = this.gameObject.GetComponent<LineRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.anyKey)
		{
			this.speed = 5f;
			this.length = 10f;
			this.laserLayerMask = LayerMask.NameToLayer("Red");
			this.laserColor = Color.red;
			this.FireLaser(this.gameObject);
		}

		if (this.isFiring)
		{
			var step = Time.deltaTime * speed;
			this.end += this.direction * step;
			this.laserDistance += (this.direction * step).magnitude;
			this.linePoints[this.linePoints.Count - 1] = end;
			this.direction = Vector3.zero;

			RecalculatePoints();

			if (laserDistance <= 30f)
			{
				RaycastForMovement();
			}

			if (this.laserDistance >= this.length)
			{
				isDissolving = true;
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
					GameObject.Destroy(this.gameObject);
				}
			}

			this.lineRenderer.numPositions = this.linePoints.Count;
			this.lineRenderer.SetPositions(this.linePoints.ToArray());
		}
	}

	public void FireLaser(GameObject owner)
	{
		this.linePoints.Clear();
		this.start = this.gameObject.transform.position;
		this.linePoints.Add(start);
		this.end = this.start;
		this.linePoints.Add(end);
		this.direction = this.transform.forward;
		this.lineRenderer.startWidth = .05f;
		this.lineRenderer.startColor = this.laserColor;
		this.lineRenderer.endColor = this.laserColor;
		scorer = owner.GetComponent<ScoreKeeper>();
		startTime = Time.time;
		this.isFiring = true;
		this.collisionLayers |= 1 << this.laserLayerMask.value;
	}

	private void RecalculatePoints()
	{
		var currentStart = this.linePoints.FirstOrDefault();
		var currentEnd = this.linePoints.LastOrDefault();
		var newDirection = (this.linePoints[1] - this.linePoints[0]).normalized;
		this.linePoints.Clear();
		this.linePoints.Add(currentStart);
		var distanceTraveled = this.laserDistance;

		for (var i = 0; i < this.lineRenderer.numPositions; ++i)
		{
			RaycastHit hit;
			if (Physics.Raycast(new Ray(this.linePoints[i], newDirection), out hit, 1000f, this.collisionLayers) &&
				(hit.point - this.linePoints[i]).magnitude < distanceTraveled)
			{
				var hitObject = hit.transform.gameObject;
				this.linePoints.Add(hit.point);
				distanceTraveled -= (hit.point - this.linePoints[i]).magnitude;
				if (hitObject.layer == LayerMask.NameToLayer("Reflective"))
				{
					newDirection = Vector3.Reflect((hit.point - this.linePoints[i]).normalized, hit.normal);
				}
				else
				{
					return;
				}
			}
			else if (this.laserDistance <= this.length)
			{
				this.linePoints.Add(this.linePoints.Last() + (newDirection*distanceTraveled));
				return;
			}
            else
			{
				this.linePoints.Add(currentEnd);
				return;
			}
		}
	}

	private void RaycastForMovement()
	{
		RaycastHit hit;
		if (Physics.Raycast(new Ray(this.start, (this.end - this.start).normalized), out hit, Vector3.Distance(start, end), this.collisionLayers))
		{
			switch (LayerMask.LayerToName(hit.collider.gameObject.layer))
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
				default:
					this.linePoints.Add(hit.point);
					this.direction = Vector3.zero;
					return;
			}
		}
		this.direction = (this.end - this.start).normalized;
	}
}
