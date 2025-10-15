using Bingyan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : BaseItem
{
    [Title("±¬Õ¨ÌØÐ§")][SerializeField] GameObject explosion;
    protected override void ResetState() { }
    protected override void Execute() { }
    public override bool Droppable
    {
        get
        {
            switch (MapManager.Instance.DisabledQuadrant)
            {
                case 1:
                    if (transform.position.x > 0 && transform.position.y > 0) return false;
                    break;
                case 2:
                    if (transform.position.x < 0 && transform.position.y > 0) return false;
                    break;
                case 3:
                    if (transform.position.x < 0 && transform.position.y < 0) return false;
                    break;
                case 4:
                    if (transform.position.x > 0 && transform.position.y < 0) return false;
                    break;
            }
            return !EditManager.Instance.Outside(CurCell);
        }
    }
    public override void OnDrop()
    {
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (MapManager.Instance.Map[MapManager.Instance.CurCell.x + 1 - i,
                    MapManager.Instance.CurCell.y + 1 - j] &&
                    MapManager.Instance.Map[MapManager.Instance.CurCell.x + 1 - i,
                    MapManager.Instance.CurCell.y + 1 - j].TryGetComponent(out BaseItem item))
                    Destroy(item.gameObject);
        AudioManager.Instance.Play("Bomb", AudioManager.Instance.gameObject);
        Instantiate(explosion, transform.position, Quaternion.identity, null);
        Destroy(gameObject);
    }
    public override void OnDestroy() { }
}
