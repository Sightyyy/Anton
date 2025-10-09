using UnityEngine;

public class InfiniteTiles : MonoBehaviour
{
    public Transform player;
    public Transform[] defaultTiles = new Transform[9];
    public float tileWidth = 12f;
    public float tileHeight = 6f;


    private Transform[,] currentTilePos = new Transform[3, 3];
    private Vector2Int playerTileIndex;

    void Start()
    {
        int i = 0;
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                currentTilePos[x, y] = defaultTiles[i];
                i++;
            }
        }

        playerTileIndex = WorldToGrid(player.position);
    }

    void Update()
    {
        UpdateTilePositions();
    }

    Vector2Int WorldToGrid(Vector3 pos)
    {
        return new Vector2Int(
            Mathf.RoundToInt(pos.x / tileWidth),
            Mathf.RoundToInt(pos.y / tileHeight)
        );
    }

    void UpdateTilePositions()
    {
        Vector2Int newIndex = WorldToGrid(player.position);

        if (newIndex != playerTileIndex)
        {
            Vector2Int dir = newIndex - playerTileIndex;

            if (dir.x > 0) ShiftLeftToRight();
            else if (dir.x < 0) ShiftRightToLeft();

            if (dir.y > 0) ShiftBottomToTop();
            else if (dir.y < 0) ShiftTopToBottom();

            playerTileIndex = newIndex;
        }
    }

    private void ShiftLeftToRight()
    {
        for (int y = 0; y < 3; y++)
        {
            Transform left = currentTilePos[0, y];
            for (int x = 0; x < 2; x++)
            {
                currentTilePos[x, y] = currentTilePos[x + 1, y];
            }
            left.position += Vector3.right * tileWidth * 3;
            currentTilePos[2, y] = left;
        }
    }

    private void ShiftRightToLeft()
    {
        for (int y = 0; y < 3; y++)
        {
            Transform right = currentTilePos[2, y];
            for (int x = 2; x > 0; x--)
            {
                currentTilePos[x, y] = currentTilePos[x - 1, y];
            }
            right.position += Vector3.left * tileWidth * 3;
            currentTilePos[0, y] = right;
        }
    }

    private void ShiftBottomToTop()
    {
        Transform[] topRow = new Transform[3];
        for (int x = 0; x < 3; x++)
        {
            topRow[x] = currentTilePos[x, 2];
        }

        for (int y = 2; y > 0; y--)
        {
            for (int x = 0; x < 3; x++)
            {
                currentTilePos[x, y] = currentTilePos[x, y - 1];
            }
        }

        for (int x = 0; x < 3; x++)
        {
            topRow[x].position += Vector3.up * tileHeight * 3;
            currentTilePos[x, 0] = topRow[x];
        }
    }


    private void ShiftTopToBottom()
    {
        Transform[] topRow = new Transform[3];
        for (int x = 0; x < 3; x++)
        {
            topRow[x] = currentTilePos[x, 0];
        }

        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                currentTilePos[x, y] = currentTilePos[x, y + 1];
            }
        }

        for (int x = 0; x < 3; x++)
        {
            topRow[x].position += Vector3.down * tileHeight * 3;
            currentTilePos[x, 2] = topRow[x];
        }
    }
}
