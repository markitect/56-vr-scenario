using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootLaser : MonoBehaviour {

	public Color laserColor;

	private LineRenderer lineRenderer;

	private List<Vector3> linePoints = new List<Vector3>();

	// Use this for initialization
	void Start () {
		if(!(this.lineRenderer = this.gameObject.GetComponent<LineRenderer>()))
		{
			this.lineRenderer = this.gameObject.AddComponent<LineRenderer>();
		}

		this.laserColor = Color.red;
	}
	
	// Update is called once per frame
	void Update () {
		this.linePoints.Clear();
		var start = this.gameObject.transform.position;
		var end = (this.transform.forward * 20) + start;
		this.linePoints.Add(start);
		this.linePoints.Add(end);
		var reflection = Vector3.zero;

		while(RaycastForReflection(start, end) && this.linePoints.Count < 30)
		{
			start = linePoints[linePoints.Count - 2];
			end = linePoints[linePoints.Count - 1];
		}

		this.lineRenderer.startWidth = .05f;
		this.lineRenderer.startColor = this.laserColor;
		this.lineRenderer.endColor = this.laserColor;
		this.lineRenderer.numPositions = this.linePoints.Count;
		this.lineRenderer.SetPositions(this.linePoints.ToArray());
	}

	private bool RaycastForReflection(Vector3 start, Vector3 direction)
	{
		var hits = Physics.RaycastAll(new Ray(start, direction.normalized));
		if(hits.Length > 0)
		{
			this.linePoints[this.linePoints.Count-1] = hits[0].point;
			if(hits[0].collider.gameObject.name != "Wall")
			{
			 	var colliderNormal = hits[0].normal;

				var reflection = Vector3.Reflect(direction - start, colliderNormal);
				this.linePoints.Add(hits[0].point + (reflection.normalized * 20));
				return true;
			}
			else
			{
				return false;
			}
		}
		this.linePoints.Add(direction);
		return false;
	}
}
