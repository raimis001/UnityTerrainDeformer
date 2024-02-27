using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class TerrainManager : MonoBehaviour
{

    [SerializeField]
    InputAction clickAction;
    [SerializeField]
    InputAction actonPosition;


    TerrainData data;

    float[,] defaultHeights;
    float[,,] defaultAlphas;

    const float seaLevel = 0.125f;
    const float hDelta = 0.01f;
    const int hSize = 10;

    private void Start()
    {
        data = GetComponent<Terrain>().terrainData;

        defaultHeights = data.GetHeights(0, 0, data.heightmapResolution, data.heightmapResolution);
        defaultAlphas = data.GetAlphamaps(0, 0, data.alphamapResolution, data.alphamapResolution);

        //TerrainModify(new Vector2Int(250, 250), 0.01f, 10);
    }

    private void OnEnable()
    {
        clickAction.Enable();
        actonPosition.Enable();
    }

    private void OnDisable()
    {
        data.SetHeights(0, 0, defaultHeights);
        data.SetAlphamaps(0, 0, defaultAlphas);

    }

    private void Update()
    {
        //if (clickAction.triggered)
        //{
        //    Vector2 mouse = actonPosition.ReadValue<Vector2>();
        //    Ray ray = Camera.main.ScreenPointToRay(mouse);

        //    if (Physics.Raycast(ray, out RaycastHit hit))
        //    {
        //        Vector2Int pos = GetTerrainPosition(hit.point);
        //        TerrainModify(pos, hDelta, 10);
        //    }
        //}
    }


    void TerrainModify(Vector2Int position, float heightDelta, int size)
    {
        Vector2Int pos = new Vector2Int(position.x - size / 2, position.y - size / 2);

        float[,] heights = data.GetHeights(pos.x, pos.y, size, size);
        float[,,] alphas = data.GetAlphamaps(pos.x, pos.y, size, size);

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                if (Vector2.Distance(new Vector2(size / 2f, size / 2f), new Vector2(x, y)) < (size / 2f))
                {
                    heights[x, y] -= heightDelta;
                    alphas[x, y, 0] = 0;
                    alphas[x, y, 1] = 1;
                }

            }

        data.SetHeights(pos.x, pos.y, heights);
        data.SetAlphamaps(pos.x, pos.y, alphas);

    }

    Vector2Int GetTerrainPosition(Vector3 position) 
    {
        Vector2Int result = Vector2Int.zero;

        result.x = Mathf.FloorToInt(((position.x - transform.position.x) / data.size.x) * data.heightmapResolution);
        result.y = Mathf.FloorToInt(((position.z - transform.position.z) / data.size.z) * data.heightmapResolution);

        return result;
    }


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.impulse.magnitude);

        if (!collision.collider.CompareTag("Projectile"))
            return;

        collision.gameObject.SetActive(false);
        if (collision.impulse.magnitude < 10)
            return;


        Vector2Int pos = GetTerrainPosition(collision.contacts[0].point);
        TerrainModify(pos, hDelta, hSize);

        
    }
}
