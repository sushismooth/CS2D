using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour {
	
	public GameObject gun;
	Vector3 playerHead;
	Vector3 playerHeadOffset;
	Vector3 target;

	int playerLayer = 8;
	int obstacleLayer = 9;

	public float RotationInDegrees;
	public float meshResolution;
	public float viewRadius;
	public float viewAngle;
	public float maskCutawayDst;
	public LayerMask obstacleMask;

	public MeshFilter viewMeshFilter;
	Mesh viewMesh;

	struct ViewCastInfo {
		public bool hit;
		public Vector3 hitPoint;
		public float distance;
		public float angle;

		public ViewCastInfo(bool h, Vector3 hp, float dst, float ang){
			hit = h;
			hitPoint = hp;
			distance = dst;
			angle = ang;
		}
	}

	void Start () {
		viewMesh = new Mesh ();
		viewMesh.name = "View Mesh";
		viewMeshFilter.mesh = viewMesh;
		playerHeadOffset = new Vector3 (0, -0.1f, 0);

	}

	void Update () {
		FindCurrentAngle ();
		DrawFieldOfView ();
	}

	void FindCurrentAngle(){
		playerHead = transform.position + playerHeadOffset;
		target = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		target.z = 0;
		RotationInDegrees = Mathf.Atan2 (target.y - playerHead.y, target.x - playerHead.x) * Mathf.Rad2Deg;
	}

	void DrawFieldOfView (){
		HideNearbyObjects ();

		int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
		float stepAngleSize = viewAngle / stepCount;
		List<Vector3> viewPoints = new List<Vector3> ();

		for (int i = 0; i <= stepCount; i++) {
			float angle = RotationInDegrees - (viewAngle / 2) + (stepAngleSize * i);
			ViewCastInfo newViewCast = ViewCast (angle);
			viewPoints.Add (newViewCast.hitPoint);
		}

		int vertexCount = viewPoints.Count + 1;
		Vector3[] vertices = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount - 2) * 3];

		vertices [0] = playerHeadOffset;
		for (int i = 0; i < vertexCount - 1; i++) {
			vertices [i + 1] = transform.InverseTransformPoint(viewPoints [i]);

			if (i < vertexCount - 2) {
				triangles [i * 3] = 0;
				triangles [(i * 3) + 1] = i + 1;
				triangles [(i * 3) + 2] = i + 2;
			}
		}
		viewMesh.Clear ();
		viewMesh.vertices = vertices;
		viewMesh.triangles = triangles;
		viewMesh.RecalculateNormals ();
	}

	ViewCastInfo ViewCast (float angle) {
		Vector2 playerHeadVector2 = new Vector2 (playerHead.x, playerHead.y);
		Vector2 vector2FromAngle = new Vector2 (vector3FromAngle (angle).x, vector3FromAngle (angle).y);
		Vector2 minRaycastVector2 = playerHeadVector2 + (vector2FromAngle * 0);
		RaycastHit2D hit = Physics2D.Raycast (minRaycastVector2, vector2FromAngle, viewRadius, obstacleMask);
		if (hit.collider != null) {
			if (hit.collider.gameObject.layer == obstacleLayer) {
				Color hitColor = hit.collider.gameObject.GetComponent<SpriteRenderer> ().color;
				hit.collider.gameObject.GetComponent<SpriteRenderer>().color = new Color (hitColor.r,hitColor.g,hitColor.b,1);
			}
			return new ViewCastInfo (true, hit.point + vector2FromAngle * maskCutawayDst, hit.distance, angle);
		} else {
			return new ViewCastInfo (false, playerHeadVector2 + vector2FromAngle * viewRadius, viewRadius, angle);
		}
	}

	void HideNearbyObjects (){
		Vector2 playerHeadVector2 = new Vector2 (playerHead.x, playerHead.y);
		RaycastHit2D[] nearbyObjects = Physics2D.CircleCastAll(playerHeadVector2,viewRadius+1,new Vector2(0,0),1);
		foreach (RaycastHit2D hit in nearbyObjects) {
			Color itemColor = hit.collider.gameObject.GetComponent<SpriteRenderer> ().color;
			if (hit.collider.gameObject.layer == LayerMask.NameToLayer ("Obstacles")) {
				float distanceAlphaRatio = 1 - Vector2.Distance (hit.collider.gameObject.transform.position,playerHeadVector2) / viewRadius;
				if (distanceAlphaRatio < 0) {
					distanceAlphaRatio = 0;
				}
				hit.collider.gameObject.GetComponent<SpriteRenderer> ().color = new Color (itemColor.r, itemColor.g, itemColor.b, (0.3f * distanceAlphaRatio) + 0.02f);
			}
		}
	}

	Vector3 vector3FromAngle(float angleInDegrees){
		float angleInRad = angleInDegrees * Mathf.Deg2Rad;
		return new Vector3 (Mathf.Cos(angleInRad), Mathf.Sin(angleInRad),0);
	}
}
