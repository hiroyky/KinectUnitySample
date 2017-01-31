using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

/// <summary>
/// Kinectから深度情報を取得するクラスです．
/// </summary>
public class DepthSourceManager : MonoBehaviour {

    public ushort[] Depth { get; private set; }
    KinectSensor sensor;
    DepthFrameReader depthReader;    

	void Start () {
        sensor = KinectSensor.GetDefault();
        if (sensor != null) {
            depthReader = sensor.DepthFrameSource.OpenReader();
            var description = sensor.DepthFrameSource.FrameDescription;            
            Depth = new ushort[description.LengthInPixels];                        
            if (!sensor.IsOpen) {
                sensor.Open();
            }
        }
	}
	
	void Update () {
        if (depthReader != null) {
            using (DepthFrame frame = depthReader.AcquireLatestFrame()) {
                if (frame != null) {
                    frame.CopyFrameDataToArray(Depth);
                    frame.Dispose();
                }
            }
        }
	}

    void OnApplicationQuit() {
        if (depthReader != null) {
            depthReader.Dispose();
            depthReader = null;
        }
        if (sensor != null) {
            if (sensor.IsOpen) {
                sensor.Close();
            }            
            sensor = null;
        }
    }
}
