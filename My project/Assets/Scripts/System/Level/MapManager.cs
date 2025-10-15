using Bingyan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapManager : AbstractManager<MapManager>
{
    public override void Init()
    {
        base.Init();

        offset = .5f * Vector2.one;
        Origin = -(Vector2)cellCnt * .5f;
        Detect();
    }
    public void Detect()
    {
        Map = new GameObject[cellCnt.x, cellCnt.y];
        Collider2D[] cover = new Collider2D[1];
        for (int x = 0; x < Map.GetLength(0); x++)
            for (int y = 0; y < Map.GetLength(1); y++)
                if (Physics2D.OverlapAreaNonAlloc(
                    new Vector2(x, y) + Origin + offset * .5f,
                    new Vector2(x, y) + Origin + offset * 1.5f, cover) > 0)
                    Map[x, y] = cover[0].TryGetComponent(out BaseItem item) ? item.gameObject : new GameObject();
    }
    [SerializeField] private Vector2Int cellCnt;
    public Vector2Int CellCnt => cellCnt;
    public GameObject[,] Map { get; set; }
    public Vector2 Origin { get; private set; }
    private Vector2 offset;
    public Vector2Int CurCell => new(
        (int)((Camera.main.ScreenToWorldPoint
        (GameSystem.Input.InEdit.PointerPos.ReadValue<Vector2>()).x - Origin.x)),
        (int)((Camera.main.ScreenToWorldPoint
        (GameSystem.Input.InEdit.PointerPos.ReadValue<Vector2>()).y - Origin.y)));
    public Vector2 CurPos => (Vector2)CurCell + Origin + offset;
    private void OnDrawGizmos()
    {
        Vector2 origin = -(Vector2)cellCnt / 2;
        for (int i = 0; i <= cellCnt.x; i++)
            Gizmos.DrawLine(origin + i * Vector2.right,
                origin + i * Vector2.right + cellCnt.y * Vector2.up);
        for (int i = 0; i <= cellCnt.y; i++)
            Gizmos.DrawLine(origin + i * Vector2.up,
                origin + i * Vector2.up + cellCnt.x * Vector2.right);
    }
    public int DisabledQuadrant { get; private set; }
    public void RandomizeQuadrant() => DisabledQuadrant = Random.Range(1, 5);
}
