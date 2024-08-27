using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class BakeSkybox
{
    public int screenSize = 1024;
    public string directory = "Assets/Skyboxes";
    public string skyboxShader = "Mobile/Skybox";


    public string[] skyBoxImage = new string[] { "front", "right", "back", "left", "up", "down" };
    public string[] skyBoxProps = new string[] { "_FrontTex", "_RightTex", "_BackTex", "_LeftTex", "_UpTex", "_DownTex" };

    public Vector3[] skyDirection = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, -90, 0), new Vector3(0, 180, 0), new Vector3(0, 90, 0), new Vector3(-90, 0, 0), new Vector3(90, 0, 0) };

    [MenuItem("MADFINGER/Bake Skybox", false, 4)]
    void CaptureSkybox()
    {
        if (!System.IO.Directory.Exists(directory))
            System.IO.Directory.CreateDirectory(directory);

        foreach (var t in Selection.transforms)
            captureSkyBox(t);
    }

    void captureSkyBox(Transform t)
    {
        var go = new GameObject("SkyboxCamera");
        go.AddComponent<Camera>();
        go.GetComponent<Camera>().backgroundColor = Color.black;
        go.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
        go.GetComponent<Camera>().fieldOfView = 90;
        go.GetComponent<Camera>().aspect = 1.0f;

        go.transform.position = t.position;
        go.transform.rotation = Quaternion.identity;

        // render skybox        
        for (var orientation = 0; orientation < skyDirection.Length; orientation++)
        {
            var assetPath = System.IO.Path.Combine(directory, t.name + "_" + skyBoxImage[orientation] + ".png");
            captureSkyBoxFace(orientation, go.GetComponent<Camera>(), assetPath);
        }
        GameObject.DestroyImmediate(go);

        // wire skybox material
        AssetDatabase.Refresh();

        var skyboxMaterial = new Material(Shader.Find(skyboxShader));
        for (var orientation = 0; orientation < skyDirection.Length; orientation++)
        {
            var texPath = System.IO.Path.Combine(directory, t.name + "_" + skyBoxImage[orientation] + ".png");
            Texture2D tex = AssetDatabase.LoadAssetAtPath(texPath, typeof(Texture2D)) as Texture2D;
            tex.wrapMode = TextureWrapMode.Clamp;
            skyboxMaterial.SetTexture(skyBoxProps[orientation], tex);
        }

        // save material
        var matPath = System.IO.Path.Combine(directory, t.name + "_skybox" + ".mat");
        AssetDatabase.CreateAsset(skyboxMaterial, matPath);
    }

    void captureSkyBoxFace(int orientation, Camera cam, string assetPath)
    {
        cam.transform.eulerAngles = skyDirection[orientation];
        var rt = new RenderTexture(screenSize, screenSize, 24);
        cam.GetComponent<Camera>().targetTexture = rt;
        cam.GetComponent<Camera>().Render();
        RenderTexture.active = rt;

        var screenShot = new Texture2D(screenSize, screenSize, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, screenSize, screenSize), 0, 0);

        RenderTexture.active = null;
        GameObject.DestroyImmediate(rt);

        var bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(assetPath, bytes);

        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
    }
}