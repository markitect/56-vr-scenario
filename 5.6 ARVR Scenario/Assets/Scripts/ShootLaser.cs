using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShootLaser : MonoBehaviour
{

	public Color laserColor;

	public float speed = 2f;

	public bool isFiring = false;

	private LineRenderer lineRenderer;

	private List<Vector3> linePoints = new List<Vector3>();
	
	private float segmentLength;

	private Vector3 start;
	private Vector3 end;
	private Vector3 direction;
	private bool ended = false;

	// Use this for initialization
	void Start()
	{
		if (!(this.lineRenderer = this.gameObject.GetComponent<LineRenderer>()))
		{
			this.lineRenderer = this.gameObject.AddComponent<LineRenderer>();
		}

		this.laserColor = Color.red;
	}

	// Update is called once per frame
	void Update()
	{
		if (this.isFiring)
		{
			end += this.direction * Time.deltaTime * speed;
			this.linePoints[this.linePoints.Count-1] = end;

			//if (this.linePoints.Count > 2)
			//{
			//	RecalcuateReflections();
			//}

			RaycastForReflection();

			if (Input.GetMouseButtonDown(1))
			{
				this.linePoints.Clear();
				this.isFiring = false;
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
		var newPoints = new List<Vector3> {this.linePoints.FirstOrDefault()};
		var startPoint = this.linePoints[0];
		var endPoint = this.linePoints[1];

		newPoints.Add(endPoint);

		while (RaycastReflection(startPoint, endPoint, ref newPoints) && newPoints.Count < this.linePoints.Count - 1)
		{
			startPoint = newPoints[newPoints.Count - 2];
			endPoint = newPoints[newPoints.Count - 1];
		}

		for (var i = 0; i < this.linePoints.Count - 1; ++i)
		{
			this.linePoints[i] = newPoints[i];
		}

		this.start = this.linePoints[this.linePoints.Count - 2];
		this.end = this.linePoints[this.linePoints.Count - 1];
	}

	private bool RaycastReflection(Vector3 startPoint, Vector3 endPoint, ref List<Vector3> newPoints)
	{
		var hits = Physics.RaycastAll(new Ray(startPoint, (endPoint - startPoint).normalized));

		if (hits.Length > 0)
		{
			newPoints[newPoints.Count - 1] = hits[0].point;
			if (hits[0].collider.gameObject.name != "Wall")
			{
				var reflection = Vector3.Reflect(endPoint - startPoint, hits[0].normal);
				this.direction = reflection.normalized;
				return true;
			}
		}
		return false;
	}

	private void RaycastForReflection()
	{
		var hits = Physics.RaycastAll(new Ray(this.start, (this.end - this.start).normalized), Vector3.Distance(start, end));

		if (hits.Length > 0)
		{
			if (hits[0].collider.gameObject.name != "Wall")
			{
				var reflection = Vector3.Reflect(end - start, hits[0].normal);
				this.direction = reflection.normalized;
				this.start = this.end;
				this.linePoints.Add(this.end);
			}
			else
			{
				ended = true;
			}
		}
	}
}
