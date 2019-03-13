using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelGeneration : MonoBehaviour
{
    //Lists for level terrain generation
    private List<Vector3> verts = new List<Vector3>();
    private List<int> tris = new List<int>();
    private List<Vector2> uv = new List<Vector2>();

    //Terrain detail
    private int resolution = 3;
    //Terrain seed
    private int levelrand = 0;

    //Terrain texture
    private List<Color> collist = new List<Color>();
    private float texRedChannel = 0f;
    private float texGreenChannel = 0f;
    private float texBlueChannel = 0f;
    private float texMaxChannel = 0f;
    public Texture2D TerrainTex;

    private float terrainheight = 0f;
    public GameObject astronaut;

    private float GetSeamlessNoise(float scale, float x)
    {
        return Mathf.PerlinNoise(Mathf.Cos(x / (64f * resolution) * 360f * Mathf.Deg2Rad) * scale + levelrand, Mathf.Sin(x / (64f * resolution) * 360f * Mathf.Deg2Rad) * 0.7f + levelrand) * 10;
    }

    void Start()
    {

        //Add astronauts to the ground.
        for (int i = 0; i < 15; i++)
        {
            GameObject newAstronaut = Instantiate(astronaut, new Vector3(Random.Range(-320, 320) * 2, -90, 0), Quaternion.identity);

            //Make the astronaut the child of one of the foreground level terrain parts, so they will be attached to the terrain's movement.
            if (newAstronaut.transform.position.x < 0)
            {
                newAstronaut.transform.SetParent(transform.GetChild(2));
            }
            else
            {
                newAstronaut.transform.SetParent(transform.GetChild(3));
            }
        }

        levelrand = Random.Range(0, 10000);

        //Terrain generation, the terrain is split into two gameobjects so the terrain can be seamlessly looped when the player's ship flies around it.
        for (int i = 0; i < 2; i++)
        {
            //Vertices and UV generation
            for (int y = 0; y < 2; y++)
            {
                if (i == 0)
                {
                    for (int x = 0; x < 64 * resolution / 2; x++)
                    {
                        terrainheight = Mathf.Pow(GetSeamlessNoise(1.5f, x), 1.4f) * 2f + GetSeamlessNoise(3f, x) * 1f + GetSeamlessNoise(6f, x) * 1f + GetSeamlessNoise(15f, x) * 0.8f + GetSeamlessNoise(50f, x) * 0.5f;
                        verts.Add(new Vector3(x * 2 / (float)resolution - 64, Mathf.Pow(y * terrainheight, 1.2f) / 2f, (y - 1) * -1 * Mathf.Pow(GetSeamlessNoise(1.5f, x), 1.4f) * -5f));
                        uv.Add(new Vector2(verts[verts.Count - 1].x / 128f, verts[verts.Count - 1].y / 128f));
                    }
                }
                else
                {
                    for (int x = 64 * resolution / 2 - 1; x < 64 * resolution + 1; x++)
                    {
                        terrainheight = Mathf.Pow(GetSeamlessNoise(1.5f, x), 1.4f) * 2f + GetSeamlessNoise(3f, x) * 1f + GetSeamlessNoise(6f, x) * 1f + GetSeamlessNoise(15f, x) * 0.8f + GetSeamlessNoise(50f, x) * 0.5f;
                        verts.Add(new Vector3(x * 2 / (float)resolution - 64, Mathf.Pow(y * terrainheight, 1.2f) / 2f, (y - 1) * -1 * Mathf.Pow(GetSeamlessNoise(1.5f, x), 1.4f) * -5f));
                        uv.Add(new Vector2(verts[verts.Count - 1].x / 128f, verts[verts.Count - 1].y / 128f));
                    }
                }
            }

            //Triangles generation
            if (i == 0)
            {
                for (int t = 0; t < 64 * resolution / 2; t++)
                {
                    if ((t + 1) % (64 * resolution / 2) != 0)
                    {
                        tris.Add(t);
                        tris.Add(t + 64 / 2 * resolution);
                        tris.Add(t + 1);
                        tris.Add(t + 64 / 2 * resolution);
                        tris.Add(t + 64 / 2 * resolution + 1);
                        tris.Add(t + 1);
                    }
                }
            }
            else
            {
                for (int t = 0; t < 64 * resolution / 2 + 2; t++)
                {
                    if ((t + 1) % (64 * resolution / 2 + 2) != 0)
                    {
                        tris.Add(t);
                        tris.Add(t + 64 / 2 * resolution + 2);
                        tris.Add(t + 1);
                        tris.Add(t + 64 / 2 * resolution + 2);
                        tris.Add(t + 64 / 2 * resolution + 3);
                        tris.Add(t + 1);
                    }
                }
            }

            Mesh newmesh = new Mesh();
            newmesh.vertices = verts.ToArray();
            newmesh.triangles = tris.ToArray();
            newmesh.uv = uv.ToArray();

            transform.GetChild(i).gameObject.GetComponent<MeshFilter>().mesh = newmesh;
            transform.GetChild(i).gameObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
            transform.GetChild(i).gameObject.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            transform.GetChild(i).gameObject.GetComponent<MeshFilter>().mesh.RecalculateTangents();

            tris.Clear();
            verts.Clear();
            uv.Clear();
        }


        //Generating texture for terrain
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                //Generate RGB texture using perlinnoise
                texRedChannel = Mathf.PerlinNoise(x * 0.337f + levelrand + 100, y * 0.337f + levelrand + 100);
                texGreenChannel = Mathf.PerlinNoise(x * 0.337f + levelrand + 500, y * 0.337f + levelrand + 500);
                texBlueChannel = Mathf.PerlinNoise(x * 0.337f + levelrand + 1000, y * 0.337f + levelrand + 1000);

                //Check which color channel is the brightest, remove every other color than that
                texMaxChannel = Mathf.Max(texRedChannel, texGreenChannel, texBlueChannel);

                if (texRedChannel == texMaxChannel)
                {
                    texRedChannel = 1;
                    texGreenChannel = 0;
                    texBlueChannel = 0;
                }
                else if (texGreenChannel == texMaxChannel)
                {
                    texRedChannel = 0;
                    texGreenChannel = 1;
                    texBlueChannel = 0;
                }
                else if (texBlueChannel == texMaxChannel)
                {
                    texRedChannel = 0;
                    texGreenChannel = 0;
                    texBlueChannel = 1;
                }

                collist.Add(new Color(texRedChannel, texGreenChannel, texBlueChannel));

            }

        }
        TerrainTex.SetPixels(collist.ToArray());
        TerrainTex.Apply();
    }

    void Update()
    {
        //Seamless level scrolling, terrain elements are moved according to spaceship horizontal movement to make the world appear continuous

        if (transform.GetChild(0).position.x <= -320)
        {
            transform.GetChild(0).position += new Vector3(1024, 0, 0);
        }
        else if (transform.GetChild(0).position.x >= 840)
        {
            transform.GetChild(0).position += new Vector3(-1024, 0, 0);
        }

        if (transform.GetChild(1).position.x <= -840)
        {
            transform.GetChild(1).position += new Vector3(1024, 0, 0);
        }
        else if (transform.GetChild(1).position.x >= 320)
        {
            transform.GetChild(1).position += new Vector3(-1024, 0, 0);
        }



        if (transform.GetChild(2).position.x <= -750)
        {
            transform.GetChild(2).position += new Vector3(1024 * 1.25f, 0, 0);
        }
        else if (transform.GetChild(2).position.x >= 750)
        {
            transform.GetChild(2).position += new Vector3(-1024 * 1.25f, 0, 0);
        }

        if (transform.GetChild(3).position.x <= -750)
        {
            transform.GetChild(3).position += new Vector3(1024 * 1.25f, 0, 0);
        }
        else if (transform.GetChild(3).position.x >= 750)
        {
            transform.GetChild(3).position += new Vector3(-1024 * 1.25f, 0, 0);
        }

    }
}
