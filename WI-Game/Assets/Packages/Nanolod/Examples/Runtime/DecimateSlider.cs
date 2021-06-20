using System;
using System.Collections.Generic;
using System.Linq;
using Nanolod;
using Nanomesh;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class DecimateSlider : MonoBehaviour
{
    public GameObject original;

    public Text polycountLabel;
    
    private Slider _slider;
    private List<Mesh> _originalMeshes;
    private List<Mesh> _meshes;
    private int _polycount;

    private void Start()
    {
        _originalMeshes = new List<Mesh>();
        _meshes = new List<Mesh>();

        _slider = GetComponent<Slider>();
        _slider.onValueChanged.AddListener(OnValueChanged);

        Renderer[] renderers = original.GetComponentsInChildren<Renderer>();

        HashSet<Mesh> uniqueMeshes = new HashSet<Mesh>();

        foreach (Renderer renderer in renderers)
        {
            if (renderer is MeshRenderer meshRenderer)
            {
                var meshFilter = renderer.gameObject.GetComponent<MeshFilter>();
                if (!meshFilter)
                    continue;
                
                var mesh = meshFilter.sharedMesh;

                _polycount += mesh.triangles.Length;
                
                if (!uniqueMeshes.Add(mesh))
                    continue;
                
                _originalMeshes.Add(mesh);
                
                // Clone mesh
                mesh = meshFilter.sharedMesh = UnityConverter.ToSharedMesh(mesh).ToUnityMesh();
                _meshes.Add(mesh);
            }
            else if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
            {
                var mesh = skinnedMeshRenderer.sharedMesh;
                
                _polycount += mesh.triangles.Length;
                
                if (!uniqueMeshes.Add(mesh))
                    continue;
                
                _originalMeshes.Add(mesh);
                
                // Clone mesh
                mesh = skinnedMeshRenderer.sharedMesh = mesh.ToSharedMesh().ToUnityMesh();
                _meshes.Add(mesh);
            }
        }

        OnValueChanged(1);
    }

    private void OnValueChanged(float value)
    {
        polycountLabel.text = $"{Math.Round(100 * value)}% ({Math.Round(value * _polycount)}/{_polycount} triangles)";
        
        Profiling.Start("Convert");

        var connectedMeshes = _originalMeshes.Select(x => UnityConverter.ToSharedMesh(x).ToConnectedMesh()).ToArray();

        Debug.Log(Profiling.End("Convert"));
        Profiling.Start("Clean");

        foreach (var connectedMesh in connectedMeshes)
        {
            // Important step :
            // We merge positions to increase chances of having correct topology information
            // We merge attributes in order to make interpolation properly operate on every face
            connectedMesh.MergePositions(0.0001f);
            connectedMesh.MergeAttributes();
            connectedMesh.Compact();
        }

        Debug.Log(Profiling.End("Clean"));
        Profiling.Start("Decimate");

        SceneDecimator sceneDecimator = new SceneDecimator();
        sceneDecimator.Initialize(connectedMeshes);

        sceneDecimator.DecimateToRatio(value);

        Debug.Log(Profiling.End("Decimate"));
        Profiling.Start("Convert back");

        for (int i = 0; i < connectedMeshes.Length; i++)
        {
            _meshes[i].Clear();
            connectedMeshes[i].ToSharedMesh().ToUnityMesh(_meshes[i]);
            _meshes[i].bindposes = _originalMeshes[i].bindposes;
        }

        Debug.Log(Profiling.End("Convert back"));
    }
}