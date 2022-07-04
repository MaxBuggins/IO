using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SurfaceType { concreate, grass, dirt, metal, water, glass, gravel } // Music Generas

[System.Serializable]
public class SurfaceMaterial // Thanks Internet man for idear (may you surf forever)
{
    public SurfaceType surfaceType;

    public TerrainLayer terrainLayer;
    public Material material;

    public AudioClip[] clips;
}




[CreateAssetMenu(fileName = "Audio/SurfaceMaterialData")] //cool
public class SurfaceMaterialData : ScriptableObject
{
    [SerializeField] private SurfaceMaterial[] surfaceMaterials;

    public SurfaceMaterial FindSurfaceMaterial(TerrainLayer layer) //Iterate the list and compare layers 
    {
        foreach(SurfaceMaterial surfaceMaterial in surfaceMaterials)
        {
            if (surfaceMaterial.terrainLayer == layer)
                return (surfaceMaterial);
        }

        return (surfaceMaterials[0]);
    }

    public SurfaceMaterial FindSurfaceMaterial(Material material) //Iterate the list and compare layers 
    {
        foreach (SurfaceMaterial surfaceMaterial in surfaceMaterials)
        {
            if (surfaceMaterial.material == material)
                return (surfaceMaterial);
        }

        return (surfaceMaterials[0]);
    }

    public SurfaceMaterial FindSurfaceMaterial(SurfaceType surfaceType) //Iterate the list and compare identities
    {
        foreach (SurfaceMaterial surfaceMaterial in surfaceMaterials)
        {
            if (surfaceMaterial.surfaceType == surfaceType)
                return (surfaceMaterial);
        }

        return (surfaceMaterials[0]);
    }
}


