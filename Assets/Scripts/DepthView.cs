using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

/// <summary>
/// セットされたGameObjectにTextureとして深度画像を描画するクラスです．
/// </summary>
[RequireComponent(typeof(Renderer))]
public class DepthView : MonoBehaviour {

    Texture2D depthTexture;
    public GameObject DepthSourceManager;
    DepthSourceManager depthSourceManager;
    FrameDescription frameDescription;
    byte[] depthImageBuffer;

	void Start () {
        frameDescription = KinectSensor.GetDefault().DepthFrameSource.FrameDescription;
        depthSourceManager = DepthSourceManager.GetComponent<DepthSourceManager>();
        depthTexture = new Texture2D(frameDescription.Width, frameDescription.Height, TextureFormat.RGB24, false);
        depthImageBuffer = new byte[frameDescription.LengthInPixels * 3];
	}
	
	void Update () {
        updateDepthTexture(depthSourceManager.Depth);        
        gameObject.GetComponent<Renderer>().material.mainTexture = depthTexture;
    }

    void updateDepthTexture(ushort[] depth) {
        for (int i = 0; i < depth.Length; ++i) {
            var val = (byte)(depth[i] * 255 / 8000);
            int index = i * 3;
            depthImageBuffer[index] = val;
            depthImageBuffer[index + 1] = val;
            depthImageBuffer[index + 2] = val;
        }
        depthTexture.LoadRawTextureData(depthImageBuffer);
        depthTexture.Apply();
    }


}
