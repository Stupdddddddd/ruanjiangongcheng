using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private void Start()
    {
        covered = selected = Used = false;
        slotImg = GetComponent<Image>();
    }
    [Serializable]
    public struct Cover
    {
        public FloatRange Size;
        public float Velocity;
        private float state;
        public float State
        {
            get => state;
            set => state = Mathf.Clamp(value, 0f, 1f);
        }
    }
    private bool covered;
    [SerializeField] private Cover cover;
    [Serializable]
    public struct Select
    {
        public Color From;
        public Color To;
        public float Velocity;
        private float state;
        public float State
        {
            get => state;
            set => state = Mathf.Clamp(value, 0f, 1f);
        }
    }
    private bool selected;
    [SerializeField] private Select select;
    [SerializeField] private Image img;
    public GameObject Item { get; set; }
    public bool Used { get; set; }
    public Image Img => img;
    private Image slotImg;
    private void FixedUpdate()
    {
        cover.State += cover.Velocity * Time.fixedDeltaTime * (Used || covered ? 1 : -1);
        img.transform.localScale = (cover.Size.Max * cover.State + cover.Size.Min * (1 - cover.State)) * Vector3.one;
        select.State += select.Velocity * Time.fixedDeltaTime * (Used || selected ? 1 : -1);
        slotImg.color = select.To * select.State + select.From * (1 - select.State);
    }
    public void OnPointerEnter(PointerEventData eventData) => covered = true;
    public void OnPointerExit(PointerEventData eventData) => covered = false;
    public void OnPointerDown(PointerEventData eventData) => selected = true;
    public void OnPointerUp(PointerEventData eventData)
    {
        selected = false;
        if (EditManager.Instance.CurItem)
        {
            EditManager.Instance.ItemRecord[EditManager.Instance.CurItem].Used = false;
            Destroy(EditManager.Instance.CurItem);
        }
        if (Used || UIManager.Instance.Edit.Ready) return;
        Used = true;
        EditManager.Instance.CurItem = Instantiate(Item,
            GameSystem.Input.InEdit.PointerPos.ReadValue<Vector2>(), Quaternion.identity, null);
        EditManager.Instance.ItemRecord.Add(EditManager.Instance.CurItem, this);
    }
}
