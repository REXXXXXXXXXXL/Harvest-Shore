using HarvestShore.CropPlant;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using HarvestShore.Map;
using HarvestShore.Inventory;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed, item;

    private Sprite currentSprite;   //存储当前鼠标图片
    private Image cursorImage;
    private RectTransform cursorCanvas;
    //建造图标跟随
    private Image buildImage;

    //鼠标检测
    private Camera mainCamera;
    private Grid currentGrid;

    private Vector3 mouseWorldPos;//世界坐标
    private Vector3Int mouseGridPos;//网格坐标

    private bool cursorEnable;
    private bool cursorPositionValid;//鼠标当前位置是否可用

    private ItemDetails currentItem;//当前物品
    private Transform playerTransform;

    private Transform PlayerTransform
    {
        get
        {
            if (playerTransform == null)
                playerTransform = SceneTools.FindPlayerTransform();
            return playerTransform;
        }
    }

     private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void Start()
    {
        SceneTools.TryFindComponentWithTag("CursorCanvas", out cursorCanvas);
        if (cursorCanvas == null)
            return;

        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        //拿到建造图标
        buildImage = cursorCanvas.GetChild(1).GetComponent<Image>();
        buildImage.gameObject.SetActive(false);
        
        currentSprite = normal;
        SetCursorImage(normal);

        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (cursorCanvas == null) return;

        cursorImage.transform.position = Input.mousePosition;//跟随鼠标移动

        if (!InteractWithUI() && cursorEnable)
        {
            SetCursorImage(currentSprite);
            CheckCursorValid();
            CheckPlayerInput();
        }
        else
        {
            SetCursorImage(normal);
            buildImage.gameObject.SetActive(false);
        }
    }

    private void CheckPlayerInput()//点按鼠标方法
    {
        if (Input.GetMouseButtonDown(0) && cursorPositionValid)
        {
            //执行方法
            EventHandler.CallMouseClickedEvent(mouseWorldPos, currentItem);
        }
    }

      private void OnBeforeSceneUnloadEvent()
      {
        cursorEnable = false;
      }

     private void OnAfterSceneLoadedEvent()
    {
        currentGrid = SceneTools.FindGrid();
        playerTransform = SceneTools.FindPlayerTransform();
    }
    
    /// <summary>
    /// 设置鼠标图片
    /// </summary>
    /// <param name="sprite"></param>
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }

    #region 设置鼠标样式
    /// <summary>
    /// 设置鼠标可用
    /// </summary>
    private void SetCursorValid()
    {
        cursorPositionValid = true;
        cursorImage.color = new Color(1, 1, 1, 1);
        buildImage.color = new Color(1, 1, 1, 0.5f);
        
    }
    /// <summary>
    /// 设置鼠标不可用
    /// </summary>
    private void SetCursorInValid()
    {
        cursorPositionValid = false;
        cursorImage.color = new Color(1, 0, 0, 0.4f);
        buildImage.color = new Color(1, 0, 0, 0.5f);
        
    }
    #endregion 

    /// <summary>
    /// 物品选择事件函数
    /// </summary>
    /// <param name="itemDetails"></param>
    /// <param name="isSelected"></param>
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        
         if (!isSelected)
        {
            currentItem = null;
            cursorEnable = false;
            currentSprite = normal;
            buildImage.gameObject.SetActive(false);
        }
        else    //物品被选中才切换图片
        {
            currentItem = itemDetails;
            
             //WORKFLOW:添加所有类型对应图片
            currentSprite = itemDetails.itemType switch
            {
                ItemType.Seed => seed,
                ItemType.Commodity => item,
                ItemType.ChopTool => tool,
                ItemType.HoeTool => tool,
                ItemType.WaterTool => tool,
                ItemType.BreakTool => tool,
                ItemType.ReapTool => tool,
                ItemType.Furniture => tool,
                ItemType.CollectTool => tool,
                _ => normal,
            };
            cursorEnable = true;
            
            //显示建造物品图片
            if (itemDetails.itemType == ItemType.Furniture)
            {
                buildImage.gameObject.SetActive(true);
                buildImage.sprite = itemDetails.itemOnWorldSprite;
                buildImage.SetNativeSize();
            }
        }
    }

    private void CheckCursorValid()
    {
      if (mainCamera == null || currentGrid == null || currentItem == null || PlayerTransform == null)
      {
          SetCursorInValid();
          return;
      }

      mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
      mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

      var playerGridPos = currentGrid.WorldToCell(PlayerTransform.position);//人物网格坐标
      
      //建造图片跟随移动
      buildImage.rectTransform.position = Input.mousePosition;

      //判断在使用范围内
      if (Mathf.Abs(mouseGridPos.x - playerGridPos.x) > currentItem.itemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInValid();
            return;
        }

      TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);

        if (currentTile != null)
        {
            CropDetails currentCrop = CropManager.Instance.GetCropDetails(currentTile.seedItemID);
            Crop crop = GridMapManager.Instance.GetCropObject(mouseWorldPos);


           //WORKLOW:补充所有物品类型判断
           switch (currentItem.itemType)
            {
                case ItemType.Seed:
                    if (currentTile.daysSinceDug > -1 && currentTile.seedItemID == -1) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.Commodity:
                    if (currentTile.canDropItem && currentItem.canDropped ) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.HoeTool:
                    if (currentTile.canDig) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.WaterTool:
                    if (currentTile.daysSinceDug > -1 && currentTile.daysSinceWatered == -1) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.BreakTool:    
                case ItemType.ChopTool:
                    if (crop != null)
                    {
                        if (crop.CanHarvest && crop.cropDetails.CheckToolAvailable(currentItem.itemID)) SetCursorValid(); else SetCursorInValid();
                    }
                    else SetCursorInValid();
                    break;          
                case ItemType.CollectTool:
                     if (currentCrop != null)
                    {
                        if (currentCrop.CheckToolAvailable(currentItem.itemID))
                            if (currentTile.growthDays >= currentCrop.TotalGrowthDays) SetCursorValid(); else SetCursorInValid();
                    }
                    else
                        SetCursorInValid();
                    break;
                case ItemType.ReapTool:
                    if (GridMapManager.Instance.HaveReapableItemsInRadius(mouseWorldPos, currentItem)) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.Furniture:
                    buildImage.gameObject.SetActive(true);
                    var inventory = InventoryManager.Instance;
                    var bluePrintDetails = inventory != null && inventory.bluePrintData != null
                        ? inventory.bluePrintData.GetBluePrintDetails(currentItem.itemID)
                        : null;

                    if (bluePrintDetails != null && currentTile.canPlaceFurniture && inventory.CheckStock(currentItem.itemID) && !HaveFurnitureInRadius(bluePrintDetails))
                        SetCursorValid();
                    else
                        SetCursorInValid();
                    break;
                         
            }
        }
        else
        {
            SetCursorInValid();
        }
    }


    private bool HaveFurnitureInRadius(BluePrintDetails bluePrintDetails)
    {
        var buildItem = bluePrintDetails.buildPrefab;
        if (buildItem == null || !buildItem.TryGetComponent(out BoxCollider2D boxCollider))
            return true;

        Vector2 point = mouseWorldPos;
        var size = boxCollider.size;

        var otherColl = Physics2D.OverlapBox(point, size, 0);
        return otherColl != null && otherColl.TryGetComponent(out Furniture _);
    }

    /// <summary>
    /// 是否与UI互动
    /// </summary>
    /// <returns></returns>
    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }
}
