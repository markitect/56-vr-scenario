using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ShootLaser : MonoBehaviour
{

	public Color renderColor;
	public float speed;

	public bool isFiring = false;

	public LayerMask collisionLayers;
	public LayerMask laserLayerMask;

	private LineRenderer lineRenderer;

	private List<Vector3> linePoints = new List<Vector3>();
	
	private Vector3 start;
	private Vector3 end;
	private Vector3 direction;

	private float laserDistance;

	// Use this for initialization
	void Start()
	{
		this.lineRenderer = this.gameObject.GetComponent<LineRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		if (this.isFiring)
		{
			var step = Time.deltaTime * speed;
			this.end += this.direction * step;
			this.laserDistance += (this.direction * step).magnitude;
			this.linePoints[this.linePoints.Count - 1] = end;
			this.direction = Vector3.zero;

			RecalculatePoints();

			if (this.laserDistance < 20f)
			{
				RaycastForMovement();
			}

			this.lineRenderer.numPositions = this.linePoints.Count;
			this.lineRenderer.SetPositions(this.linePoints.ToArray());
		}
		else
		{
			if (Input.GetMouseButtonDown(1))
			{
				this.FireLaser();
			}
		}
	}

	public void FireLaser()
	{
		this.lineRenderer.startWidth = .05f;
		this.lineRenderer.startColor = this.renderColor;
		this.lineRenderer.endColor = this.renderColor;
		this.linePoints.Clear();
		this.start = this.gameObject.transform.position;
		this.linePoints.Add(start);
		this.end = this.start;
		this.linePoints.Add(end);
		this.direction = this.transform.forward;
		this.isFiring = true;
		this.collisionLayers |= this.laserLayerMask;
	}

	private void RecalculatePoints()
	{
		var currentStart = this.linePoints.FirstOrDefault();
		var newDirection = this.transform.forward;
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
			else
			{
				this.linePoints.Add(this.linePoints.Last() + (newDirection * distanceTraveled));
				return;
			}
		}
	}

	private void RaycastForMovement()
	{
		RaycastHit hit;
		if (Physics.Raycast(new Ray(this.start, (this.end - this.start).normalized), out hit, Vector3.Distance(start, end), this.collisionLayers))
		{
			var closestGameObject = hit.collider.gameObject;
			if (closestGameObject.layer == LayerMask.NameToLayer("Reflective"))
			{
				var reflection = Vector3.Reflect(this.end - this.start, hit.normal);
				this.direction = reflection.normalized;
				this.start = this.end;
				this.linePoints.Add(this.end);
				return;
			}
			else
			{
				this.linePoints.Add(hit.point);
				this.direction = Vector3.zero;
				return;
			}
		}
		this.direction = (this.end - this.start).normalized;
	}
}
