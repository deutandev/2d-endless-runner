using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneratorController : MonoBehaviour
{
    [Header("Templates")]
    public List<TerrainTemplateController> terrainTemplates;
    public float terrainTemplateWidth;

    [Header("Generator Area")]
    public Camera gameCamera;
    public float areaStartOffset;
    public float areaEndOffset;

    private const float debugLineHeight = 10.0f;

    private List<GameObject> spawnedTerrain;
    private float lastGeneratedPositionX;

    // Added in "Generate & delete object"
    private float lastRemovedPositionX;

    // Added in "Generate & delete object"
    [Header("Force Early Template")]
    public List<TerrainTemplateController> earlyTerrainTemplates;

    private float GetHorizontalPositionStart()
    {
        return gameCamera.ViewportToWorldPoint(new Vector2(0f, 0f)).x + areaStartOffset;
    }

    private float GetHorizontalPositionEnd()
    {
        return gameCamera.ViewportToWorldPoint(new Vector2(1f, 0f)).x + areaEndOffset;
    }

    // debug
    private void OnDrawGizmos()
    {
        Vector3 areaStartPosition = transform.position;
        Vector3 areaEndPosition = transform.position;

        areaStartPosition.x = GetHorizontalPositionStart();
        areaEndPosition.x = GetHorizontalPositionEnd();

        Debug.DrawLine(areaStartPosition + Vector3.up * debugLineHeight / 2, areaStartPosition + Vector3.down * debugLineHeight / 2, Color.red);
        Debug.DrawLine(areaEndPosition + Vector3.up * debugLineHeight / 2, areaEndPosition + Vector3.down * debugLineHeight / 2, Color.red);
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnedTerrain = new List<GameObject>();
        lastGeneratedPositionX = GetHorizontalPositionStart();
        // added in "Generate & remove object"
        lastRemovedPositionX = lastGeneratedPositionX - terrainTemplateWidth;
        
        foreach (TerrainTemplateController terrain in earlyTerrainTemplates)
        {
            GenerateTerrain(lastGeneratedPositionX, terrain);
            lastGeneratedPositionX += terrainTemplateWidth;
        }

        while (lastGeneratedPositionX < GetHorizontalPositionEnd())
        {
            GenerateTerrain(lastGeneratedPositionX);
            lastGeneratedPositionX += terrainTemplateWidth;
        }
    }

    // Update is called once per frame
    void Update()
    {
        while (lastGeneratedPositionX < GetHorizontalPositionEnd())
        {
            GenerateTerrain(lastGeneratedPositionX);
            lastGeneratedPositionX += terrainTemplateWidth;
        } 

        // Remove used terrain
        while (lastRemovedPositionX + terrainTemplateWidth < GetHorizontalPositionStart())
        {
            lastRemovedPositionX += terrainTemplateWidth;
            RemoveTerrain(lastRemovedPositionX);
        }
    }

    private void GenerateTerrain(float posX, TerrainTemplateController forceterrain = null)
    {
        GameObject newTerrain = Instantiate(terrainTemplates[Random.Range(0, terrainTemplates.Count)].gameObject, transform);
        newTerrain.transform.position = new Vector2(posX, 0f);
        spawnedTerrain.Add(newTerrain);
    }

    // function to remove terrain
    private void RemoveTerrain(float posX)
    {
        GameObject terrainToRemove = null;

        // find terrain at posX
        foreach (GameObject item in spawnedTerrain)
        {
            if (item.transform.position.x == posX)
            {
                terrainToRemove = item;
                break;
            }
        }
    
        // after found;
        if (terrainToRemove != null)
        {
            spawnedTerrain.Remove(terrainToRemove);
            Destroy(terrainToRemove);
        }
    }
}
