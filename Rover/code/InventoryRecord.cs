
using Archives;
namespace RoverGame
{
    public class InventoryRecord : Archives.InventoryComponent.UpdateRecord
    {
        public int itemCount;
        public int winCount;

        public InventoryRecord()
            : base()
        {

        }

        public override void Reset()
        {
            itemCount = 0;
            winCount = 0;
        }

        public override void Add(InventoryComponent.UpdateRecord otherRecord)
        {
            if(otherRecord is InventoryRecord)
            {
                InventoryRecord other = (InventoryRecord)otherRecord;
                itemCount += other.itemCount;
                winCount += other.winCount;
            }
        }
    }
}
