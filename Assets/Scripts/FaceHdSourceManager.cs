using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Windows.Kinect;
using Microsoft.Kinect.Face;

/// <summary>
/// Kinectで顔を検出し，その特徴を保持するクラスです．
/// </summary>
public class FaceHdSourceManager : MonoBehaviour {

    KinectSensor sensor = null;
    BodyFrameSource bodySource = null;
    BodyFrameReader bodyReader = null;
    HighDefinitionFaceFrameSource hdFaceFrameSource = null;
    HighDefinitionFaceFrameReader hdFaceFrameReader = null;
    FaceAlignment faceAlignment = null;
    FaceModel faceModel = null;
    FaceModelBuilder faceModelBuilder = null;

    public string FaceCaptureStatus { get; private set; }
    public bool IsFaceModelCollectCompleted { get; private set; }
    public IList<CameraSpacePoint> FaceVertices { get; private set; }

    void Start () {
        initialize();	
	}
	
	void Update () {
        updateCollectionStatus();
        updateBodyDetection();
        updateFaceVertices();
    }

    void OnApplicationQuit() {
        if (bodyReader != null) {
            bodyReader.Dispose();
            bodyReader = null;
        }
        if (hdFaceFrameReader != null) {
            hdFaceFrameReader.Dispose();
            hdFaceFrameReader = null;
        }
        if (sensor == null) {
            if (sensor.IsOpen) {
                sensor.Close();
            }
            sensor = null;
        }
    }

    void initialize() {
        IsFaceModelCollectCompleted = false;
        FaceCaptureStatus = "";
        FaceVertices = new List<CameraSpacePoint>();

        sensor = KinectSensor.GetDefault();
        if (sensor == null) {
            return;
        }
        sensor.Open();

        bodySource = sensor.BodyFrameSource;
        bodyReader = bodySource.OpenReader();

        hdFaceFrameSource = HighDefinitionFaceFrameSource.Create(sensor);
        hdFaceFrameReader = hdFaceFrameSource.OpenReader();

        faceModel = FaceModel.Create();
        faceAlignment = FaceAlignment.Create();
        FaceModelBuilderAttributes attributes = FaceModelBuilderAttributes.None;
        faceModelBuilder = hdFaceFrameSource.OpenModelBuilder(attributes);
        faceModelBuilder.CollectFaceDataAsync(collectFaceModelCompleted, collectFaceModelFailed);
    }

    void updateCollectionStatus() {
        if (faceModelBuilder == null) {
            return;
        }
        FaceCaptureStatus = faceModelBuilder.CollectionStatus.ToString();
    }

    void collectFaceModelCompleted(FaceModelData faceModelData) {
        print("Model created!");
        faceModel = faceModelData.ProduceFaceModel();
        faceModelBuilder.Dispose();
        faceModelBuilder = null;
        IsFaceModelCollectCompleted = true;
    }

    void collectFaceModelFailed(int err) {
        print("error");
    }

    void updateBodyDetection() {
        using (var frame = bodyReader.AcquireLatestFrame()) {
            if (frame == null) {
                return;
            }

            Body[] bodies = new Body[frame.BodyCount];
            frame.GetAndRefreshBodyData(bodies);

            Body body = bodies.Where(b => b.IsTracked).FirstOrDefault();
            if (!hdFaceFrameSource.IsTrackingIdValid && body != null) {
                hdFaceFrameSource.TrackingId = body.TrackingId;
            }
        }
    }

    void updateFaceVertices() {
        using (var frame = hdFaceFrameReader.AcquireLatestFrame()) {
            if (frame == null || !frame.IsFaceTracked) {
                return;
            }
            frame.GetAndRefreshFaceAlignmentResult(faceAlignment);
        }

        FaceVertices = faceModel.CalculateVerticesForAlignment(faceAlignment);
    }
}