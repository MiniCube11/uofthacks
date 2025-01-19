using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceMultipleObjectsOnPlane : PressInputBase
{
    float snapGridSize = 0.1f;

    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject placedPrefab;

    [SerializeField]
    LayerMask blockLayer;

    GameObject spawnedObject;

    ARRaycastManager aRRaycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    List<GameObject> placedBlocks = new List<GameObject>();

    public Color blockColour = new Color(223f/255f, 67f/255f, 67f/255f);

    protected override void Awake()
    {
        base.Awake();
        aRRaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.C))
        {
            clearBlocks();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetColour1();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetColour2();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetColour3();
        }
    }

    public void SetColour1()
    {
        blockColour = new Color(67f/255f, 156f/255f, 223f/255f);
    }

    public void SetColour2()
    {
        blockColour = new Color(223f/255f, 67f/255f, 67f/255f);
    }

    public void SetColour3()
    {
        blockColour = new Color(67f/255f, 223f/255f, 67f/255f);
    }

    protected override void OnPress(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, blockLayer))
        {
            // Debug.Log("Hit block: " + hit.collider.gameObject.name + " " + hit.collider.gameObject.layer);
            Vector3 snappedPosition = SnapToGrid(hit.point);
            Debug.Log(hit.collider.transform.rotation);
            PlaceBlock(snappedPosition, hit.collider.transform.rotation);
        }
        else if (aRRaycastManager.Raycast(position, hits, TrackableType.PlaneWithinPolygon))
        {
            // Raycast hits are sorted by distance, so the first hit means the closest.
            var hitPose = hits[0].pose;
            // Instantiated the prefab.
            PlaceBlock(SnapToGrid(hitPose.position), hitPose.rotation);
        }
    }

    void PlaceBlock(Vector3 position, Quaternion rotation)
    {
        spawnedObject = Instantiate(placedPrefab, position, rotation);
        spawnedObject.layer = LayerMask.NameToLayer("Blocks");

        Transform cube = spawnedObject.transform.Find("Cube");
        Transform cylinder = spawnedObject.transform.Find("Cylinder");

        if (cube != null && cube.TryGetComponent(out Renderer cubeRenderer))
        {
            cubeRenderer.material = new Material(cubeRenderer.material);
            cubeRenderer.material.color = blockColour;
        }

        if (cylinder != null && cylinder.TryGetComponent(out Renderer cylinderRenderer))
        {
            cylinderRenderer.material = new Material(cylinderRenderer.material);
            cylinderRenderer.material.color = blockColour;
        }

        placedBlocks.Add(spawnedObject);
    }

    Vector3 SnapToGrid(Vector3 position)
    {
        float x = Mathf.Round(position.x / snapGridSize) * snapGridSize;
        float y = Mathf.Round(position.y / snapGridSize) * snapGridSize;
        float z = Mathf.Round(position.z / snapGridSize) * snapGridSize;
        return new Vector3(x, y, z);
    }

    public void clearBlocks()
    {
        foreach (var block in placedBlocks)
        {
            Destroy(block);
        }

        placedBlocks.Clear();
    }
}

