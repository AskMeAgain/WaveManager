using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class waveManager : MonoBehaviour {

    //Settings for users
    [Space(10)]
    [Header("Settings")]
    public float waveReductionOverTime = 0.9f;
    public float waveSpeed = 1;
    public float waveFrequency = 1;
    public float wavePropagation = 2;
    public float waveScale = 1;

    public float waveMaxArea = 20;

    public AnimationCurve waveGrowCurve;

    //stores all active Waves
    Dictionary<Wave, Dictionary<GameObject, List<int>>> activeWaves;

    bool deleteFlag = false;

    public static waveManager instance = null;

    //setting up lazy singleton when script starts
    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
        }
    }

    //List<int> contains only the indices of the vertices who needs to get checked.
    public Dictionary<GameObject, List<int>> VerticesInRangeDict;

    //original vertex Points
    Dictionary<GameObject, Vector3[]> originalVertices;

    //normals of each vertex
    Dictionary<GameObject, Vector3[]> normals;

    //dictionary to store the active times for waves
    Dictionary<Wave, float> activeWavesTime;

    //dictionary to store the active distance for each wave
    Dictionary<Wave, float> activeWavesDistance;

    //contains all objects which needs to get checked if they are in range
    GameObject[] allWalls;

    void Start() {

        //Get all Walls in an Array
        allWalls = GameObject.FindGameObjectsWithTag("Walls");

        //instantiate the dictionaries
        originalVertices = new Dictionary<GameObject, Vector3[]>();
        activeWavesTime = new Dictionary<Wave, float>();
        activeWavesDistance = new Dictionary<Wave, float>();
        normals = new Dictionary<GameObject, Vector3[]>();
        offsetDictionary = new Dictionary<GameObject, Dictionary<int, float>>();
        activeWaves = new Dictionary<Wave, Dictionary<GameObject, List<int>>>();

        //get all original vertices in WORLDSPACE stored inside dictionary
        for (int i = 0; i < allWalls.Length; i++) {

            Mesh thisObj = allWalls[i].GetComponent<MeshFilter>().mesh;
            int length = thisObj.vertices.Length;

            Vector3[] verticesCopy = (Vector3[])thisObj.vertices.Clone();
            originalVertices.Add(allWalls[i], verticesCopy);

            Vector3[] normalCopy = (Vector3[])thisObj.normals.Clone();
            normals.Add(allWalls[i], normalCopy);

        }

        initOffsetDictionary();

    }


    //this function provides other scripts with the ability to spawn waves.
    public void makeWave(Vector3 pos, Wave.WaveType type) {

        Wave tempWave = new Wave(type);
        tempWave.pos = pos;

        //load the dictionary with every vertex point in range of the wave
        Dictionary<GameObject, List<int>> temp2 = new Dictionary<GameObject, List<int>>(getVerticesInRange(pos, tempWave.maxDistance));

        //check if the wave exists before
        if (!activeWaves.ContainsKey(tempWave)) {
            activeWaves.Add(tempWave, temp2);
            activeWavesTime.Add(tempWave, 1);
            activeWavesDistance.Add(tempWave, 0);
        } else {
            activeWavesTime[tempWave] = 1;
            //activeWavesDistance[tempWave] = 0;
        }
    }

    void initOffsetDictionary() {

        //init offset values

        offsetDictionary = new Dictionary<GameObject, Dictionary<int, float>>();

        for (int i = 0; i < allWalls.Length; i++) {

            Mesh thisObj = allWalls[i].GetComponent<MeshFilter>().mesh;

            Dictionary<int, float> temp = new Dictionary<int, float>();
            for (int ii = 0; ii < thisObj.vertexCount; ii++) {
                temp.Add(ii, -100);
            }

            offsetDictionary.Add(allWalls[i], temp);

        }
    }

    //executed ~60 times per second, provided by unity
    void FixedUpdate() {

        foreach (Wave w in activeWaves.Keys) {
            activeWavesDistance[w] += wavePropagation;
        }

        if (activeWaves.Count > 0)
            moveVertices();

    }

    Dictionary<GameObject, Dictionary<int, float>> offsetDictionary;

    private void moveVertices() {

        //weird hack to remove dictionary because of using a Construct
        Wave removeWave = null;

        initOffsetDictionary();

        //we iterate over each active wave to move the vertex points accordingly
        foreach (Wave w in activeWaves.Keys) {

            //removing inactive waves
            if (activeWavesTime[w] < 0.01) {
                activeWavesTime[w] = 0;

                //set remove flag and remove the inactive wave
                deleteFlag = true;
                removeWave = w;
            } else {
                activeWavesTime[w] -= waveReductionOverTime;
            }

            foreach (GameObject obj in activeWaves[w].Keys) {

                //Get the current vertices and change only the index ones
                Vector3[] newVertices = obj.GetComponent<MeshFilter>().mesh.vertices;

                //we can skip the loop if the time is lower then 0.01f
                if (activeWavesTime[w] > 0.01f) {

                    Vector3 localPos = obj.transform.InverseTransformPoint(w.pos);

                    foreach (int index in activeWaves[w][obj]) {

                        //formula to calculate the movement
                        float distance = Vector3.Distance(localPos, newVertices[index]);

                        //variable to store the new height of the vertex point
                        float offset = Mathf.Sin(Time.time * waveSpeed + distance * waveFrequency) * waveGrowCurve.Evaluate(activeWavesTime[w]) * w.scale;

                        //each waves should have a propagation
                        if (distance < activeWavesDistance[w]) {

                            //we need to make sure that older waves dont override newer waves
                            if (offset > offsetDictionary[obj][index]) {
                                offsetDictionary[obj][index] = offset;
                                newVertices[index] = originalVertices[obj][index] + normals[obj][index] * offset;
                            }
                        }
                    }

                } else {
                    newVertices = originalVertices[obj];
                }

                //setting the new meshes and calculating everything using Unity methods
                Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
                mesh.vertices = newVertices.ToArray();
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();

            }
        }

        if (deleteFlag) {
            deleteFlag = false;
            activeWaves.Remove(removeWave);
            activeWavesTime.Remove(removeWave);
            activeWavesDistance.Remove(removeWave);

        }
    }


    Dictionary<GameObject, List<int>> getVerticesInRange(Vector3 pos, int maxDistance) {

        //this function calculates every vertex point in range using a position and a max Distance.

        VerticesInRangeDict = new Dictionary<GameObject, List<int>>();

        //iterating over every object in the scene which should spawn waves
        foreach (GameObject obj in originalVertices.Keys) {

            Vector3[] allVerticesOfObject = (Vector3[])originalVertices[obj].Clone();
            List<int> VertexIndex = new List<int>();
            Vector3 localPos = obj.transform.InverseTransformPoint(pos);

            for (int i = 0; i < allVerticesOfObject.Length; i++) {

                //check if vertice is in range of pos
                if (Vector3.Distance(allVerticesOfObject[i], localPos) < maxDistance) {
                    VertexIndex.Add(i);
                }
            }

            //some objects are not included because they are not in range to spawn a wave
            if (VertexIndex.Count > 0) {
                VerticesInRangeDict.Add(obj, VertexIndex);
            }
        }

        return VerticesInRangeDict;

    }
}
