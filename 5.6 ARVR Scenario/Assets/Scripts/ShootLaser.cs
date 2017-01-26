using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class ShootLaser : MonoBehaviour
{
	public Color laserColor;

	public float speed = 2f;

	public bool isFiring = false;

	public LayerMask collisionLayers;

	private LineRenderer lineRenderer;

	private List<Vector3> linePoints = new List<Vector3>();
	private List<Vector3> newPoints = new List<Vector3>();

	private float segmentLength;

	private Vector3 start;
	private Vector3 end;
	private Vector3 direction;
	private bool ended = false;

	private List<GameObject> newPointSpheres = new List<GameObject>();
	private List<GameObject> linePointSpheres = new List<GameObject>();

	// Use this for initialization
	void Start()
	{
		if (!(this.lineRenderer = this.gameObject.GetComponent<LineRenderer>()))
		{
			this.lineRenderer = this.gameObject.AddComponent<LineRenderer>();
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (this.isFiring)
		{
			if (!this.ended)
			{
				end += this.direction * Time.deltaTime * speed;
				this.linePoints[this.linePoints.Count - 1] = end;
			}

			if (this.linePoints.Count > 2 || this.ended)
			{
				RecalcuateReflections();
			}

			if (!ended)
			{
				RaycastForMovement();
			}

			if (Input.GetMouseButtonDown(1))
			{
				this.newPoints.Clear();
				this.linePoints.Clear();
				this.isFiring = false;
				this.ended = false;
			}

			this.lineRenderer.startWidth = .05f;
			this.lineRenderer.startColor = this.laserColor;
			this.lineRenderer.endColor = this.laserColor;
			this.lineRenderer.numPositions = this.linePoints.Count;
			this.lineRenderer.SetPositions(this.linePoints.ToArray());
		}
		else
		{
			if (Input.GetMouseButtonDown(0))
			{
				this.FireLaser();
			}
		}
	}

	public void FireLaser()
	{
		this.linePoints.Clear();
		this.start = this.gameObject.transform.position;
		this.linePoints.Add(start);
		this.end = this.start;
		this.linePoints.Add(end);
		this.direction = this.transform.forward;
		this.isFiring = true;
		this.ended = false;
	}

	private void RecalcuateReflections()
	{
		var currentEnd = this.end;
		this.start = this.linePoints[0];
		this.newPoints = new List<Vector3> { start };
		this.direction = this.transform.forward;

		if (!this.ended)
		{
			while (this.RaycastReflections() && this.newPoints.Count < this.linePoints.Count - 1)
			{
				start = newPoints[newPoints.Count - 1];
			}
			
			this.newPoints.Add(currentEnd);
		}
		else
		{
			while (this.RaycastReflections())
			{
				start = newPoints[newPoints.Count - 1];
			}
		}

		this.linePoints = this.newPoints;

		// reset start and end tot he last two points in the list
		this.start = this.linePoints[this.linePoints.Count - 2];
		this.end = this.linePoints[this.linePoints.Count - 1];
	}

	private bool RaycastReflections()
	{
		var hits = Physics.RaycastAll(new Ray(start, this.direction), this.collisionLayers);

		if (hits.Length > 0)
		{
			var closestHit = FindClosestHit(hits);

			this.newPoints.Add(closestHit.point);
			this.end = closestHit.point;
			if (closestHit.collider.gameObject.layer != LayerMask.NameToLayer("Wall"))
			{
				var reflection = Vector3.Reflect(this.end - this.start, closestHit.normal);
				this.direction = reflection.normalized;
				return true;
			}
			else
			{
				return false;
			}
		}
		return false;
	}

	private void RaycastForMovement()
	{
		var hits = Physics.RaycastAll(new Ray(this.start, (this.end - this.start).normalized), Vector3.Distance(start, end), this.collisionLayers);

		if (hits.Length > 0)
		{
			var closestHit = FindClosestHit(hits);

			if (closestHit.collider.gameObject.layer != LayerMask.NameToLayer("Wall") && closestHit.collider.gameObject.layer != LayerMask.NameToLayer("Prism"))
			{
                var reflection = Vector3.Reflect(this.end - this.start, closestHit.normal);
				this.direction = reflection.normalized;
				this.start = this.end;
				this.linePoints.Add(this.end);
			}
            else if (closestHit.collider.gameObject.layer == LayerMask.NameToLayer("Prism"))
            {
                ended = true;
                GameObject faceHit = closestHit.collider.gameObject;
                GameObject parentPrism = faceHit.transform.parent.gameObject;
                parentPrism.GetComponent<LaserSplitter>().SplitLaser(faceHit, laserColor, speed);   
            }
			else
			{
				this.ended = true;
			}
		}
	}

	private RaycastHit FindClosestHit(RaycastHit[] hits)
	{
		var closestDistance = Vector3.Distance(start, hits[0].point);
		var closestHit = hits[0];

		for (var i = 1; i < hits.Length; ++i)
		{
			if (closestDistance > Vector3.Distance(start, hits[i].point))
			{
				closestHit = hits[i];
			}
		}

		return closestHit;
	}
}
