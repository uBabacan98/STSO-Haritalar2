using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTileMapGenerator : MonoBehaviour
{
    public GameObject hexTilePrelab;

    [SerializeField] int MapWidth = 25;
    [SerializeField] int MapHeight = 12;

    void Start()
    {
        CreateHexTileMap();
    }

    void CreateHexTileMap()
    {
        for (float x = 0; x < MapWidth; x++)
        {
            for(float z = 0; z < MapHeight; z++)
            {
                GameObject TempGO = Instantiate(hexTilePrelab);
                Vector3 pos;

                if(z % 2 == 0)
                {
                    pos = new Vector3(x * HexCoordinates.xOffset, 0, z * HexCoordinates.zOffset);
                }
                else
                {
                   pos = new Vector3(x * HexCoordinates.xOffset + HexCoordinates.xOffset / 2, 0, z * HexCoordinates.zOffset);
                }
                StartCoroutine(SetTileInfo(TempGO, x, z, pos));
            }
        }
    }

    IEnumerator SetTileInfo(GameObject GO, float x, float z, Vector3 pos)
    {
        yield return new WaitForSeconds(0.00001f);
        // GO.transform.parent = transform;
        GO.name = x.ToString() + ", " + z.ToString();
        GO.transform.position = pos;
    }

    void OnTriggerExit(Collider other)
    {
        Destroy(other.gameObject);
    }
}
