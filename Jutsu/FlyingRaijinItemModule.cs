using ThunderRoad;

namespace Jutsu
{
    /**
     * Item module class for Flying Raijin
     * Reference this class in the module section of the Item json to add the component that executes functionality
     */
    public class FlyingRaijinItemModule : ItemModule
    {
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            
            //Adds functionality script to game object
            item.gameObject.AddComponent<FlyingRaijin>();
        }
    }
}