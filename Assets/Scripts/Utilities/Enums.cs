public enum ItemType
{
    Seed, Commodity, Furniture,///種、商品、家具///
    HoeTool, ChopTool, BreakTool,ReapTool, WaterTool,CollectTool,///鍬、伐採、採掘、草刈り、水やり、収穫
    ReapableScenery///雑草
}


public enum SlotType
{
    Bag, Box, Shop
}

public enum InventoryLocation
{
    Player, Box
}
public enum PartType 
{
    None, Carry, Hoe, Break, Water, Chop, Collect, Reap///なし、持ち上げ、道具
}

public enum PartName
{
    Body, Hair, Arm, Tool
}

public enum Season
{
    春, 夏, 秋, 冬
}

public enum GridType//グリッド種別
{
    Diggable, DropItem, PlaceFurniture, NPCObstacle
}

public enum ParticleEffectType
{
    None, LeavesFalling01, LeavesFalling02, Rock, ReapableScenery
}

public enum GameState
{
    Gameplay, Pause
}

public enum LightShift
{
    Morning, Night
}

public enum SoundName
{
    none, FootStepSoft, FootStepHard,
    Axe, Pickaxe, Hoe, Reap, Water, Basket,
    Pickup, Plant, TreeFalling, Rustle,
    AmbientCountryside1, AmbientCountryside2, MusicCalm1, MusicCalm2, MusicCalm3, MusicCalm4, MusicCalm5, MusicCalm6, AmbientIndoor1
}
