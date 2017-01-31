using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class FaceHdView : MonoBehaviour {

    public GameObject FaceHdSourceManager;
    public Material Material;
    KinectSensor sensor = null;
    FaceHdSourceManager faceHdSourceManager = null;
    Mesh faceMesh = null;

    void Start () {
        var renderer = gameObject.GetComponent<Renderer>();
        renderer.material = Material;

        sensor = KinectSensor.GetDefault();
        if (sensor != null) {
            faceHdSourceManager = FaceHdSourceManager.GetComponent<FaceHdSourceManager>();
            var description = sensor.DepthFrameSource.FrameDescription;
            faceMesh = createMesh(1347);
            gameObject.GetComponent<MeshFilter>().mesh = faceMesh;
        }
	}
	
	void Update () {
        var faceVertices = faceHdSourceManager.FaceVertices;
        if (faceVertices != null && faceVertices.Count >= 3) {
            faceMesh.vertices = convertToVector3From(faceVertices);
            faceMesh.triangles = FaceIndex.FaceTriangles;
            faceMesh.RecalculateNormals();
        }
	}

    Mesh createMesh(int num) {
        Mesh mesh = new Mesh();
        var vertices = new Vector3[num];
        var triangles = new int[num];

        for (int i = 0; i < num; ++i) {
            vertices[i] = new Vector3();
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }

    Vector3[] convertToVector3From(IList<CameraSpacePoint> points) {
        Vector3[] retval = new Vector3[points.Count];
        for (int i = 0; i < points.Count; ++i) {
            retval[i] = new Vector3(points[i].X, points[i].Y, points[i].Z);
        }
        return retval;
    }
}
