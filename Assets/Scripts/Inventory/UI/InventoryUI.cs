using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HarvestShore.Inventory
{
public class InventoryUI : MonoBehaviour
 {
    public ItemTooltip itemTooltip;
    
    [Header("ドラッグ画像")]
    public Image dragItem;
    [Header("プレイヤーバッグUI")]
    [SerializeField] private GameObject bagUI;
    private bool bagOpened;
    
    [Header("共通バッグ")]        
    [SerializeField] private GameObject baseBag;        
    public GameObject shopSlotPrefab;
    public GameObject boxSlotPrefab;
    
    [Header("取引UI")]
    public TradeUI tradeUI;
    public TextMeshProUGUI playerMoneyText;
    
    [SerializeField] private SlotUI [] playerSlots;
    [SerializeField] private List<SlotUI> baseBagSlots;

    private void OnEnable() 
    {
      EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
      EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadedEvent;
      EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
      EventHandler.BaseBagCloseEvent += OnBaseBagCloseEvent;
      EventHandler.ShowTradeUI += OnShowTradeUI;
    }

    private void OnDisable()
    {
      EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
      EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadedEvent;
      EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
      EventHandler.BaseBagCloseEvent -= OnBaseBagCloseEvent;
      EventHandler.ShowTradeUI -= OnShowTradeUI;
    }

   
    private void Start()
    {
        if (baseBagSlots == null)
            baseBagSlots = new List<SlotUI>();

        ///给每一个格子序号
        if (playerSlots != null)
        {
            for (int i =0; i< playerSlots.Length; i++)
            {
                if (playerSlots[i] != null)
                    playerSlots[i].slotIndex = i;
            }
        }
        bagOpened = bagUI != null && bagUI.activeInHierarchy;
        if (playerMoneyText != null)
            playerMoneyText.text = InventoryManager.Instance.playerMoney.ToString();
    }

    ///键盘按下b键打开背包
    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            OpenBagUI();
        }
    }
    
    private void OnShowTradeUI(ItemDetails item, bool isSell)
    {
        if (tradeUI == null)
            return;

        tradeUI.gameObject.SetActive(true);
        tradeUI.SetupTradeUI(item, isSell);
    }

    /// <summary>        
    /// 打开通用包裹UI事件        
    /// </summary>        
    /// <param name="slotType"></param>        
    /// <param name="bagData"></param>
     private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bagData)        
     {
        if (bagData == null || bagData.itemList == null || baseBag == null || baseBag.transform.childCount == 0)
            return;

        GameObject prefab = slotType switch
        {                
            SlotType.Shop => shopSlotPrefab,                
            SlotType.Box => boxSlotPrefab,               
             _ => null,            
        };
        if (prefab == null)
            return;

        //生成背包UI            
        baseBag.SetActive(true);

        ClearBaseBagSlots();
        baseBagSlots = new List<SlotUI>();

        for (int i = 0; i < bagData.itemList.Count; i++)            
        {                
            var slot = Instantiate(prefab, baseBag.transform.GetChild(0)).GetComponent<SlotUI>();                
            slot.slotIndex = i;                
            baseBagSlots.Add(slot);            
        }

        if (baseBag.TryGetComponent(out RectTransform baseBagRect))
            LayoutRebuilder.ForceRebuildLayoutImmediate(baseBagRect);
        
        if (slotType == SlotType.Shop)
        {
            if (bagUI != null && bagUI.TryGetComponent(out RectTransform bagRect))
            {
                bagRect.pivot = new Vector2(-1, 0.5f);
                bagUI.SetActive(true);
                bagOpened = true;
            }
        }
         //更新UI显示            
        OnUpdateInventoryUI(InventoryLocation.Box, bagData.itemList);
     }
    
    /// <summary>
    /// 关闭通用包裹UI事件
    /// </summary>
    /// <param name="slotType"></param>
    /// <param name="bagData"></param>
    private void OnBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bagData)
    {
        if (baseBag == null)
            return;

        baseBag.SetActive(false);
        if (itemTooltip != null)
            itemTooltip.gameObject.SetActive(false);
        UpdateSlotHightlight(-1);

        ClearBaseBagSlots();

        if (slotType == SlotType.Shop)
        {
            if (bagUI != null && bagUI.TryGetComponent(out RectTransform bagRect))
            {
                bagRect.pivot = new Vector2(0.5f, 0.5f);
                bagUI.SetActive(false);
                bagOpened = false;
            }
        }
    }

    
     private void OnBeforeSceneUnloadedEvent()//取消高亮动画
    {
        UpdateSlotHightlight(-1);
    }

    /// <summary>        
    /// 更新指定位置的Slot事件函数        
    /// </summary>        
    /// <param name="location">库存位置</param>        
    /// <param name="list">数据列表</param>
    private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
    {
        if (list == null)
            return;

        switch (location)
        {
            case InventoryLocation.Player:
                if (playerSlots == null)
                    break;

                for (int i = 0; i < playerSlots.Length; i++)
                {
                    if (playerSlots[i] == null)
                        continue;

                    if (i >= list.Count || list[i].itemAmount <= 0)
                    {
                        playerSlots[i].UpdateEmptySlot();
                    }
                    else
                    {
                        var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                        if (item != null)
                            playerSlots[i].UpdateSlot(item, list[i].itemAmount);
                        else
                            playerSlots[i].UpdateEmptySlot();
                    }
                }
                break;
            case InventoryLocation.Box:                    
            if (baseBagSlots == null)
                break;

            for (int i = 0; i < baseBagSlots.Count; i++)                    
            {
                if (baseBagSlots[i] == null)
                    continue;

                if (i >= list.Count || list[i].itemAmount <= 0)                        
                {
                    baseBagSlots[i].UpdateEmptySlot();
                }
                else                        
                {                            
                    var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                    if (item != null)
                        baseBagSlots[i].UpdateSlot(item, list[i].itemAmount);
                    else
                        baseBagSlots[i].UpdateEmptySlot();
                }                    
            }                    
            break;    
        }
        if (playerMoneyText != null)
            playerMoneyText.text = InventoryManager.Instance.playerMoney.ToString();
    }

    /// <summary>
    ///打开关闭背包UI,Button调用事件
    /// </summary>
    public void OpenBagUI()
    {
        if (bagUI == null)
            return;

        bagOpened = !bagOpened;

        bagUI.SetActive(bagOpened);
    }


    /// <summary>
    /// 更新Slot高亮显示
    /// </summary>
    /// <param name="index">序号</param>
    public void UpdateSlotHightlight(int index)
    {
        if (playerSlots == null)
            return;

        foreach (var slot in playerSlots)
        {
            if (slot == null || slot.slotHightlight == null)
                continue;

            if (slot.isSelected && slot.slotIndex == index)
            {
                slot.slotHightlight.gameObject.SetActive(true);
            }
            else
            {
                slot.isSelected = false;///其他格子取消
                slot.slotHightlight.gameObject.SetActive(false);
            }
        }
    }

    private void ClearBaseBagSlots()
    {
        if (baseBagSlots == null)
            return;

        foreach (var slot in baseBagSlots)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }
        baseBagSlots.Clear();
    }
  }
}
