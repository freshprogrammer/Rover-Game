using Archives;

namespace RoverGame
{
    public class RoverGameHudSystem : HudSystem
    {
        private StringRenderObject life;
        private StringRenderObject score;
        private StringRenderObject shots;
        private StringRenderObject distance;


        public override void Setup()
        {
            Reset();

            life = new StringRenderObject(new KromskyFontSpriteSheet(), "Health:00X");
            life.Priority = SortConstants.HUD;
            life.SetPosition(0, 700);

            score = new StringRenderObject(new KromskyFontSpriteSheet(), "Score:0000");
            score.Priority = SortConstants.HUD;
            score.SetPosition(0, 700 - ((16 + 1) * 1));

            shots = new StringRenderObject(new KromskyFontSpriteSheet(), "Shots:00000");
            shots.Priority = SortConstants.HUD;
            shots.SetPosition(0, 700 - ((16 + 1) * 2));

            distance = new StringRenderObject(new KromskyFontSpriteSheet(), "Distance Traveled:000000");
            distance.Priority = SortConstants.HUD;
            distance.SetPosition(0, 700 - ((16 + 1) * 3));
        }

        public override void Reset()
        {
            Enabled = true;
        }

        public override void Update(float secondsDelta, BaseObject parent)
        {
            if (Enabled)
            {
                GameObjectManager manager = sSystemRegistry.GameObjectManager;
                if (manager != null)
                {
                    GameObject playerObject = (GameObject)manager.GetPlayer();
                    if (playerObject != null)
                    {
                        int coinCount = 0;
                        InventoryComponent inventory = playerObject.FindByType<InventoryComponent>();
                        if (inventory != null)
                        {
                            coinCount = ((InventoryRecord)inventory.GetRecord()).itemCount;
                        }

                        int shotCount = 0;
                        LaunchProjectileComponent gun = playerObject.FindByType<LaunchProjectileComponent>();
                        if (gun != null)
                        {
                            shotCount = gun.TotalProjectilesFired;
                        }

                        double dist = 0;
                        PlayerComponent player = playerObject.FindByType<PlayerComponent>();
                        if (player != null)
                        {
                            dist = player.DistanceTraveled;
                        }

                        life.SetText("Health:" + playerObject.life);
                        life.Update(secondsDelta, this);

                        score.SetText("Score:" + coinCount);
                        score.Update(secondsDelta, this);

                        shots.SetText("Shots:" + shotCount);
                        shots.Update(secondsDelta, this);

                        distance.SetText("Distance Traveled:" + dist);
                        distance.Update(secondsDelta, this);
                    }
                }
            }
        }

        public override void UpdateInventory(InventoryComponent.UpdateRecord inv)
        {
            //TODO write this stub HUD inventory stub
        }
    }
}
