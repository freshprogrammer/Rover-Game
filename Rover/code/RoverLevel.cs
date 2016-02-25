using Archives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoverGame
{
    class RoverLevel : Level
    {
        //would be stored as a list of object types (int type ID) at a location - maybe be addition info like resource count or team ID
        //List<Ro

        public override void LoadFromFile()
        {
            //scrap load from file
            //load fixed level - normaly these would be loaded from a XML like list file

            //create blank level
            //add player at 0,128
            //add base recharge station at 0,0
            //    size 128,128
            //add res at 200,200
            //    size 64,64
            //    resCount 100
            //Add Rock at 500,0
            //    size 64,128

            width = 1280*2;
            height = 720*2;
            
        }

        public override void BuildEmpty()
        {
            width = 500;
            height = 500;
        }

        public override void SetupCollision()
        {
            //dynamic collision - pass blank colision map
            const int tileSize = 0;
            sSystemRegistry.CollisionSystem.Initialize(new TiledCollisionWorld(0,0), tileSize, tileSize);
        }

        public override void SpawnObjects()
        {
            RoverGameGameObjectFactory factory = (RoverGameGameObjectFactory)sSystemRegistry.GameObjectFactory;
            GameObjectManager manager = sSystemRegistry.GameObjectManager;

            manager.Add(factory.SpawnBackgroundPlate(0, 0));
            manager.Add(factory.SpawnBackgroundPlate(0, 720));
            manager.Add(factory.SpawnBackgroundPlate(1280, 0));
            manager.Add(factory.SpawnBackgroundPlate(1280, 720));
            
            //trees
            manager.Add(factory.SpawnTree(512, 128));
            manager.Add(factory.SpawnTree(64, 150));
            manager.Add(factory.SpawnTree(850, 450));

            //emerald field
            //  X
            //  XX (half space centered)
            // XXX
            //  XX
            //  X
            const int emeraldFieldOriginX = 700;
            const int emeraldFieldOriginY = 100;
            const int emeraldSpacing = 32;
            manager.Add(factory.SpawnResourceEmerald(emeraldFieldOriginX + 0.0f * emeraldSpacing, emeraldFieldOriginY + 0.0f * emeraldSpacing));//mid
            manager.Add(factory.SpawnResourceEmerald(emeraldFieldOriginX + 1.0f * emeraldSpacing, emeraldFieldOriginY + 0.0f * emeraldSpacing));
            manager.Add(factory.SpawnResourceEmerald(emeraldFieldOriginX + 2.0f * emeraldSpacing, emeraldFieldOriginY + 0.0f * emeraldSpacing));
            manager.Add(factory.SpawnResourceEmerald(emeraldFieldOriginX + 0.5f * emeraldSpacing, emeraldFieldOriginY + 1.0f * emeraldSpacing));//lower
            manager.Add(factory.SpawnResourceEmerald(emeraldFieldOriginX + 1.5f * emeraldSpacing, emeraldFieldOriginY + 1.0f * emeraldSpacing));
            manager.Add(factory.SpawnResourceEmerald(emeraldFieldOriginX + 1.0f * emeraldSpacing, emeraldFieldOriginY + 2.0f * emeraldSpacing));
            manager.Add(factory.SpawnResourceEmerald(emeraldFieldOriginX + 0.5f * emeraldSpacing, emeraldFieldOriginY + -1.0f * emeraldSpacing));//upper
            manager.Add(factory.SpawnResourceEmerald(emeraldFieldOriginX + 1.5f * emeraldSpacing, emeraldFieldOriginY + -1.0f * emeraldSpacing));
            manager.Add(factory.SpawnResourceEmerald(emeraldFieldOriginX + 1.0f * emeraldSpacing, emeraldFieldOriginY + -2.0f * emeraldSpacing));

            
            manager.Add(factory.SpawnResourceEmerald(600,600));

            manager.Add(factory.SpawnStructure_RepairBay(300, 300));

            GameObject player = factory.SpawnPlayer(0, 128);
            manager.Add(player);
            manager.SetPlayer(player);
            sSystemRegistry.CameraSystem.SetTarget(player);

            manager.Add(factory.SpawnZombie(500, 0));
            //manager.Add(factory.SpawnZombieSpawner(0, 600));
        }
    }
}
