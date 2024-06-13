using ThunderRoad;

namespace Jutsu
{
    public class FlyingRaijinItemModule : ItemModule
    {
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<FlyingRaijin>();
        }
    }
}