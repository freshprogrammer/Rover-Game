using System;
using Microsoft.Xna.Framework;
using Archives;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace RoverGame
{
    public class RoverGameGameObjectFactory:GameObjectFactory
    {
        private SoundEffect pistolSound;
        private SoundEffect playerHitSound;
        private SoundEffect ringSound;
        private SoundEffect winSound;
        private SoundEffect ghostHitSound;

        private Texture2D marioSpriteSheet;
        private Texture2D pacmanSpriteSheet;


        //Default Values for game variables
        //collectables
        private int scorePerSilverCoin = 1;
        private int scorePerGoldCoin = 5;
        private int scorePerRing = 10;
        private int scorePerEmerald = 50;
        //player
        private int playerHealth = 10;
        private float playerInvincibleAfterHitTime = 0.5f;
        private int playerMoveSpeed = 350;
        private float playerFireDelay = 0.08f;
        private int playerFireSeperation = 10;
        //player Projectiles
        private int projectileSpeed = 700;
        //ninja
        private int ninjaHealth = 7;
        private float ninjaInvincibleAfterHitTime = 0f;
        private int ninjaMoveSpeed = 150;
        private float ninjaFireDelay = 1f;
        private int ninjaFireSeperation = 15;
        private int ninjaWanderTargetRadius = 20;
        private int ninjaWanderTargetDistance = 64;
        private int ninjaWanderMaxJitter = 5;
        private bool ninjaTargetPlayer = true;
        //zombie
        private int zombieHealth = 7;
        private float zombieInvincibleAfterHitTime = 0f;
        private int zombieMoveSpeed = 50;
        private int zombieWanderTargetRadius = 20;
        private int zombieWanderTargetDistance = 64;
        private int zombieWanderMaxJitter = 5;
        private bool zombieTargetPlayer = true;
        //zombie spawner
        private int zombieSpawnerMaxActive = 5;
        private float zombieSpawnerSpawnDelay = 2.5f;
        //guard
        private int guardHealth = 2;
        private float guardInvincibleAfterHitTime = 0f;
        //shuriken
        private int shurikenSpeed = 500;
        //ghost
        private int ghostHealth = 1;

        public RoverGameGameObjectFactory()
            : base()
        {
        }

        public override void UpdateActivationRadii()
        {
            ContextParameters paramaters = sSystemRegistry.ContextParameters;
            //float halfHeight2 = (paramaters.GameHeight * 0.5f) * (paramaters.GameHeight * 0.5f);
            //float halfWidth2 = (paramaters.GameWidth * 0.5f) * (paramaters.GameWidth * 0.5f);
            //float screenSizeRadius = (float)Math.Sqrt(halfHeight2 + halfWidth2);

            //to londgside
            float screenSizeRadius;
            if (paramaters.GameWidth > paramaters.GameHeight)
                screenSizeRadius = paramaters.GameWidth / 2;
            else
                screenSizeRadius = paramaters.GameHeight / 2;

            mActivationRadiusTight = screenSizeRadius + 128.0f;
            mActivationRadiusNormal = screenSizeRadius * 1.25f;
            mActivationRadiusWide = screenSizeRadius * 2.0f;
            mActivationRadiusExtraWide = screenSizeRadius * 4.0f;
            mActivationRadius_AlwaysActive = -1.0f;
        }
        
        public override void PreloadEffects()
        {
            // These textures appear in every level, so they are long-term.
            
            ContentManager content = sSystemRegistry.Game.Content;
            
            pistolSound = content.Load<SoundEffect>(@"sounds\pistol_shot_02a");
            playerHitSound = content.Load<SoundEffect>(@"sounds\Sonic_Rings_Out");
            ringSound = content.Load<SoundEffect>(@"sounds\SonicRing");
            winSound = content.Load<SoundEffect>(@"sounds\Sonic_Vanish");
            ghostHitSound = content.Load<SoundEffect>(@"sounds\button-3");

            pacmanSpriteSheet = content.Load<Texture2D>(@"pics\enemies\ghosts\pacman-sprites");
            marioSpriteSheet = content.Load<Texture2D>(@"pics\tiles\Mario Sprites");

        }

        protected override void InitializeStaticData()
        {
            //instanciate static data
            int objectTypeCount = Enum.GetValues(typeof(RoverGameGameObjectTypes)).Length;
            mStaticData = new FixedSizeArray<FixedSizeArray<BaseObject>>(objectTypeCount);

            for (int x = 0; x < objectTypeCount; x++)
            {
                mStaticData.Add(null);
            }
        }

        protected override void InitializeComponentPools()
        {
            MaxGameObjects = 10000;
            mGameObjectPool = new GameObjectPool(MaxGameObjects);

            int renderedObjects = MaxGameObjects;//what game objects are inportant but not rendered?

            int particals = 1000;
            int collectables = 1500;
            int projectiles = 200;
            int simplePhysicsEntities = 15;
            int players = 1;
            int enemies = 1500;
            int wanderEnemies = 1500;
            int patrolEnemies = 200;
            int circularEnemies = 200;
            int ninjas = 200;

            int staticSets = 15;

            ComponentClass[] componentTypes = {
                    //new ComponentClass(AnimationComponent.class, 384),
                    //new ComponentClass(AttackAtDistanceComponent.class, 16),
                    new ComponentClass(enemies+players+particals+projectiles, typeof(BackgroundCollisionComponent)),
                    //new ComponentClass(ButtonAnimationComponent.class, 32),
                    //new ComponentClass(CameraBiasComponent.class, 8),
                    new ComponentClass(circularEnemies, typeof(CircularAIComponent)),
                    //new ComponentClass(ChangeComponentsComponent.class, 256),
                    //new ComponentClass(DoorAnimationComponent.class, 256),  //!
                    new ComponentClass(enemies+players+projectiles, typeof(DynamicCollisionComponent)),
                    //new ComponentClass(EnemyAnimationComponent.class, 256),
                    //new ComponentClass(FadeDrawableComponent.class, 32),
                    //new ComponentClass(FixedAnimationComponent.class, 8),
                    //new ComponentClass(FrameRateWatcherComponent.class, 1),
                    //new ComponentClass(GenericAnimationComponent.class, 32),
                    new ComponentClass(staticSets, typeof(GravityComponent)),
                    new ComponentClass(collectables+enemies, typeof(HitPlayerComponent)),
                    new ComponentClass(collectables+enemies+players, typeof(HitReactionComponent)),
                    new ComponentClass(players, typeof(InventoryComponent)),
                    new ComponentClass(players+ninjas, typeof(LaunchProjectileComponent)),
                    new ComponentClass(enemies+players+particals+collectables+projectiles, typeof(LifetimeComponent)),
                    new ComponentClass(staticSets, typeof(MovementComponent)),
                    //new ComponentClass(NPCAnimationComponent.class, 8),
                    //new ComponentClass(NPCComponent.class, 8),
                    //new ComponentClass(OrbitalMagnetComponent.class, 1),
                    new ComponentClass(patrolEnemies,typeof(PatrolAIComponent)),
                    new ComponentClass(staticSets, typeof(PhysicsComponent)),
                    new ComponentClass(players, typeof(PlayerComponent)),
                    new ComponentClass(wanderEnemies,typeof(PursuitAIComponent)),
                    new ComponentClass(renderedObjects, typeof(RenderComponent)),
                    //new ComponentClass(SimpleCollisionComponent.class, 32),
                    new ComponentClass(simplePhysicsEntities, typeof(SimplePhysicsComponent)),
                    new ComponentClass(enemies, typeof(SolidSurfaceComponent)),
                    new ComponentClass(collectables+enemies+players, typeof(SpriteComponent)),
                    new ComponentClass(wanderEnemies,typeof(WanderAIComponent)),
            };

            mComponentPools = new FixedSizeArray<FreshGameComponentPool>(componentTypes.Length, sComponentPoolComparator);
            for (int x = 0; x < componentTypes.Length; x++)
            {
                ComponentClass component = componentTypes[x];
                mComponentPools.Add(new FreshGameComponentPool(component.type, component.poolSize));
            }
            mComponentPools.Sort(true);

            mPoolSearchDummy = new FreshGameComponentPool(typeof(object), 1);
        }
        
        public void LoadSettings(VariableLibrary vars)
        {
            //collectables
            vars.GetVariable("scorePerRing", ref scorePerRing, true);
            vars.GetVariable("scorePerSilverCoin", ref scorePerSilverCoin, true);
            vars.GetVariable("scorePerGoldCoin", ref scorePerGoldCoin, true);
            vars.GetVariable("scorePerEmerald", ref scorePerEmerald, true);
            //player
            vars.GetVariable("playerHealth", ref playerHealth, true);
            vars.GetVariable("playerInvincibleAfterHitTime", ref playerInvincibleAfterHitTime, true);
            vars.GetVariable("playerMoveSpeed", ref playerMoveSpeed, true);
            vars.GetVariable("playerFireDelay", ref playerFireDelay, true);
            vars.GetVariable("playerFireSeperation", ref playerFireSeperation, true);
            //player projectiles
            vars.GetVariable("projectileSpeed", ref projectileSpeed, true);
            //ninja
            vars.GetVariable("ninjaHealth", ref ninjaHealth, true);
            vars.GetVariable("ninjaInvincibleAfterHitTime", ref ninjaInvincibleAfterHitTime, true);
            vars.GetVariable("ninjaMoveSpeed", ref ninjaMoveSpeed, true);
            vars.GetVariable("ninjaFireDelay", ref ninjaFireDelay, true);
            vars.GetVariable("ninjaFireSeperation", ref ninjaFireSeperation, true);
            vars.GetVariable("ninjaWanderTargetRadius", ref ninjaWanderTargetRadius, true);
            vars.GetVariable("ninjaWanderTargetDistance", ref ninjaWanderTargetDistance, true);
            vars.GetVariable("ninjaWanderMaxJitter", ref ninjaWanderMaxJitter, true);
            vars.GetVariable("ninjaTargetPlayer", ref ninjaTargetPlayer, true);
            //zombie
            vars.GetVariable("zombieHealth", ref zombieHealth, true);
            vars.GetVariable("zombieInvincibleAfterHitTime", ref zombieInvincibleAfterHitTime, true);
            vars.GetVariable("zombieMoveSpeed", ref zombieMoveSpeed, true);
            vars.GetVariable("zombieWanderTargetRadius", ref zombieWanderTargetRadius, true);
            vars.GetVariable("zombieWanderTargetDistance", ref zombieWanderTargetDistance, true);
            vars.GetVariable("zombieWanderMaxJitter", ref zombieWanderMaxJitter, true);
            vars.GetVariable("zombieTargetPlayer", ref zombieTargetPlayer, true);
            //zombie spawner
            vars.GetVariable("zombieSpawnerMaxActive", ref zombieSpawnerMaxActive, true);
            vars.GetVariable("zombieSpawnerSpawnDelay", ref zombieSpawnerSpawnDelay, true);
            //guard
            vars.GetVariable("guardHealth", ref guardHealth, true);
            vars.GetVariable("guardInvincibleAfterHitTime", ref guardInvincibleAfterHitTime, true);

            //shuriken
            vars.GetVariable("shurikenSpeed", ref shurikenSpeed, true);

            //ghost
            vars.GetVariable("ghostHealth", ref ghostHealth, true);
        }

        public override GameObject Spawn(int objectType, float x, float y, Vector2 facingDir, int priority) 
        {
            GameObject newObject = null;
            switch(objectType) 
            {
                default:
                case (int)RoverGameGameObjectTypes.Invalid:
                    break;
                case (int)RoverGameGameObjectTypes.Player:
                    newObject = SpawnPlayer(x, y);
                    break;
                case (int)RoverGameGameObjectTypes.Ghost_Random:
                    int ghostType = FreshArchives.Random.Next(4);
                    switch(ghostType)
                    {
                        case 0:
                            ghostType = (int)RoverGameGameObjectTypes.Ghost_Blue;
                            break;
                        case 1:
                            ghostType = (int)RoverGameGameObjectTypes.Ghost_Red;
                            break;
                        case 2:
                            ghostType = (int)RoverGameGameObjectTypes.Ghost_Orange;
                            break;
                        case 3:
                            ghostType = (int)RoverGameGameObjectTypes.Ghost_Pink;
                            break;
                    }
                    newObject = SpawnGhost(x, y, ghostType);
                    break;
                case (int)RoverGameGameObjectTypes.Ghost_Eyes:
                    newObject = SpawnGhostEyes(x, y);
                    break;
                case (int)RoverGameGameObjectTypes.Ghost_Blue:
                case (int)RoverGameGameObjectTypes.Ghost_Red:
                case (int)RoverGameGameObjectTypes.Ghost_Orange:
                case (int)RoverGameGameObjectTypes.Ghost_Pink:
                    newObject = SpawnGhost(x, y, objectType);
                    break;
                case (int)RoverGameGameObjectTypes.Guard:
                    newObject = SpawnGuard(x, y);
                    break;
                case (int)RoverGameGameObjectTypes.Zombie:
                    newObject = SpawnZombie(x, y);
                    break;
                case (int)RoverGameGameObjectTypes.ZombieSpawner:
                    newObject = SpawnZombieSpawner(x, y);
                    break;
                case (int)RoverGameGameObjectTypes.Ninja:
                    newObject = SpawnNinja(x, y);
                    break;
                case (int)RoverGameGameObjectTypes.Shuriken:
                    newObject = SpawnShuriken(x, y);
                    break;
                case (int)RoverGameGameObjectTypes.Breackable_Block_Piece:
                    newObject = SpawnBreakableBlockPiece(x, y);
                    break;
                case (int)RoverGameGameObjectTypes.Collectable:
                    newObject = SpawnRing(x, y);
                    break;
                case (int)RoverGameGameObjectTypes.Coin_Gold:
                    newObject = SpawnCoin_Gold(x, y);
                    break;
                case (int)RoverGameGameObjectTypes.Coin_Silver:
                    newObject = SpawnCoin_Silver(x, y);
                    break;
                case (int)RoverGameGameObjectTypes.Gem_Emerald:
                    newObject = SpawnGem_Emerald(x, y);
                    break;
                case (int)RoverGameGameObjectTypes.WinObject:
                    newObject = SpawnWinObject(x, y);
                    break;
                case (int)RoverGameGameObjectTypes.Background_Plate:
                    newObject = SpawnBackgroundPlate(x, y);
                    break;
                case (int)RoverGameGameObjectTypes.Gun_Projectile:
                    newObject = SpawnGunProjectile(x, y);
                    break;
                case (int)RoverGameGameObjectTypes.Particle_Burst:
                    newObject = SpawnParticleBurst(x, y);
                    break;
                case (int)RoverGameGameObjectTypes.Particle:
                    newObject = SpawnParticle(x, y);
                    break;
                case (int)RoverGameGameObjectTypes.Tile_Blocked:
                case (int)RoverGameGameObjectTypes.Tile_Blank:

                case (int)RoverGameGameObjectTypes.Tile_BlockedTopLeft:
                case (int)RoverGameGameObjectTypes.Tile_BlockedTopRight:
                case (int)RoverGameGameObjectTypes.Tile_BlockedBottomLeft:
                case (int)RoverGameGameObjectTypes.Tile_BlockedBottomRight:

                case (int)RoverGameGameObjectTypes.Mario_Tile_Brick:
                case (int)RoverGameGameObjectTypes.Mario_Tile_Brick_Ground:
                case (int)RoverGameGameObjectTypes.Mario_Tile_Block:
                case (int)RoverGameGameObjectTypes.Mario_Tile_Mystery_Block:
                case (int)RoverGameGameObjectTypes.Mario_Tile_Mystery_Block_Empty:
                case (int)RoverGameGameObjectTypes.Mario_Tile_Flagpole:
                case (int)RoverGameGameObjectTypes.Mario_Tile_Flagpole_Top:
                case (int)RoverGameGameObjectTypes.Mario_Tile_Sky:
                case (int)RoverGameGameObjectTypes.Mario_Tile_Pipe_BottomLeft:
                case (int)RoverGameGameObjectTypes.Mario_Tile_Pipe_BottomRight:
                case (int)RoverGameGameObjectTypes.Mario_Tile_Pipe_TopLeft:
                case (int)RoverGameGameObjectTypes.Mario_Tile_Pipe_TopRight:
                    newObject = SpawnTile(x, y, objectType, priority);
                    break;
            }
            return newObject;
        }

        public GameObject SpawnBackgroundPlate(float positionX, float positionY)
        {
            const int width = 1280;
            const int height = 720;
            int type = (int)RoverGameGameObjectTypes.Background_Plate;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusExtraWide;
            result.width = width;
            result.height = height;

            ContentManager content = sSystemRegistry.Game.Content;

            FixedSizeArray<BaseObject> staticData = GetStaticData(type);
            if (staticData == null)
            {
                const int staticObjectCount = 1;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                //InventoryRecord addWin = new InventoryRecord();
                //addWin.winCount = 1;

                //staticData.Add(addWin);

                SetStaticData(type, staticData);
            }

            Rectangle crop = new Rectangle(0, 0, width, height);
            DrawableTexture2D textureDrawable = new DrawableTexture2D(content.Load<Texture2D>(@"pics\background\dirt_big"), (int)result.width, (int)result.height);
            textureDrawable.SetCrop(crop);

            RenderComponent render = (RenderComponent)AllocateComponent(typeof(RenderComponent));
            render.Priority = SortConstants.BACKGROUND_START;
            render.setDrawable(textureDrawable);


            result.Add(render);

            //AddStaticData(type, result, null);

            return result;
        }

        public GameObject SpawnTree(float positionX, float positionY)
        {
            const int width = 128;
            const int height = 128;
            int type = (int)RoverGameGameObjectTypes.Tree;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusTight;
            result.width = width;
            result.height = height;

            ContentManager content = sSystemRegistry.Game.Content;

            FixedSizeArray<BaseObject> staticData = GetStaticData(type);
            if (staticData == null)
            {
                const int staticObjectCount = 1;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                //InventoryRecord addWin = new InventoryRecord();
                //addWin.winCount = 1;

                //staticData.Add(addWin);

                SetStaticData(type, staticData);
            }

            Rectangle crop = new Rectangle(0, 0, width, height);
            DrawableTexture2D textureDrawable = new DrawableTexture2D(content.Load<Texture2D>(@"pics\world\tree_128x128"), (int)result.width, (int)result.height);
            textureDrawable.SetCrop(crop);

            RenderComponent render = (RenderComponent)AllocateComponent(typeof(RenderComponent));
            render.Priority = SortConstants.FOREGROUND_OBJECT;
            render.setDrawable(textureDrawable);

            const int collisionWidth = 46;
            const int collisionHeight = 16;

            SolidSurfaceComponent collision = GenerateRectangleSolidSurfaceComponent(collisionWidth, collisionHeight, false);
            collision.SetOffset((width - collisionWidth) / 2, height - collisionHeight);//center collision at bottom center

            result.Add(render);
            result.Add(collision);

            //AddStaticData(type, result, null);

            return result;
        }

        public GameObject SpawnResourceEmerald(float positionX, float positionY)
        {
            const int width = 64;
            const int height = 64;
            int type = (int)RoverGameGameObjectTypes.Resource_Emerald;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusTight;
            result.width = width;
            result.height = height;

            ContentManager content = sSystemRegistry.Game.Content;

            FixedSizeArray<BaseObject> staticData = GetStaticData(type);
            if (staticData == null)
            {
                const int staticObjectCount = 1;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                //InventoryRecord addWin = new InventoryRecord();
                //addWin.winCount = 1;

                //staticData.Add(addWin);

                SetStaticData(type, staticData);
            }

            Rectangle crop = new Rectangle(0, 0, width, height);
            DrawableTexture2D textureDrawable = new DrawableTexture2D(content.Load<Texture2D>(@"pics\world\emerald_small"), (int)result.width, (int)result.height);
            textureDrawable.SetCrop(crop);

            RenderComponent render = (RenderComponent)AllocateComponent(typeof(RenderComponent));
            render.Priority = SortConstants.GENERAL_OBJECT;
            render.setDrawable(textureDrawable);

            const int collisionWidth = 64;
            const int collisionHeight = 64;

            SolidSurfaceComponent collision = GenerateRectangleSolidSurfaceComponent(collisionWidth, collisionHeight, false);

            result.Add(render);
            result.Add(collision);

            //AddStaticData(type, result, null);

            return result;
        }

        public GameObject SpawnStructure_RepairBay(float positionX, float positionY)
        {
            const int width = 128;
            const int height = 128;
            int type = (int)RoverGameGameObjectTypes.Resource_Emerald;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusTight;
            result.width = width;
            result.height = height;

            ContentManager content = sSystemRegistry.Game.Content;

            FixedSizeArray<BaseObject> staticData = GetStaticData(type);
            if (staticData == null)
            {
                const int staticObjectCount = 1;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                //InventoryRecord addWin = new InventoryRecord();
                //addWin.winCount = 1;

                //staticData.Add(addWin);

                SetStaticData(type, staticData);
            }

            Rectangle crop = new Rectangle(0, 0, width, height);
            DrawableTexture2D textureDrawable = new DrawableTexture2D(content.Load<Texture2D>(@"pics\structures\repair_bay\repair"), (int)result.width, (int)result.height);
            textureDrawable.SetCrop(crop);

            RenderComponent render = (RenderComponent)AllocateComponent(typeof(RenderComponent));
            render.Priority = SortConstants.GENERAL_OBJECT;
            render.setDrawable(textureDrawable);

            //bool oneCollisionSurface = true;
            //if (oneCollisionSurface)
            //{
            //    SolidSurfaceComponent colision = GenerateRectangleSolidSurfaceComponent(width, height, false);
            //    result.Add(colision);
            //}
            //else
            //{
            //    const int collisionWidth = 39;
            //    const int collisionHeight = 39;

            //    SolidSurfaceComponent tlColision = GenerateRectangleSolidSurfaceComponent(collisionWidth, collisionHeight, false);
            //    tlColision.SetOffset(0, 0);
            //    SolidSurfaceComponent trColision = GenerateRectangleSolidSurfaceComponent(collisionWidth, collisionHeight, false);
            //    trColision.SetOffset(width - collisionWidth, 0);
            //    SolidSurfaceComponent blColision = GenerateRectangleSolidSurfaceComponent(collisionWidth, collisionHeight, false);
            //    blColision.SetOffset(0, height - collisionHeight);
            //    SolidSurfaceComponent brColision = GenerateRectangleSolidSurfaceComponent(collisionWidth, collisionHeight, false);
            //    brColision.SetOffset(width - collisionWidth, height - collisionHeight);

            //    result.Add(tlColision);
            //    result.Add(trColision);
            //    result.Add(blColision);
            //    result.Add(brColision);
            //}

            result.Add(render);

            //AddStaticData(type, result, null);

            return result;
        }

        public GameObject SpawnPlayer(float positionX, float positionY)
        {
            int thisGameObjectType = (int)RoverGameGameObjectTypes.Player;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadius_AlwaysActive;
            result.width = 32;
            result.height = 32;

            result.life = playerHealth;
            result.team = GameObject.Team.PLAYER;


            FixedSizeArray<BaseObject> staticData = GetStaticData(thisGameObjectType);

            if (staticData == null)
            {
                ContentManager content = sSystemRegistry.Game.Content;
                int staticObjectCount = 3;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                //GravityComponent gravity = (GravityComponent)AllocateComponent(typeof(GravityComponent));
                //FreshGameComponent movement = (MovementComponent)AllocateComponent(typeof(MovementComponent));

                //PhysicsComponent physics = (PhysicsComponent)AllocateComponent(typeof(PhysicsComponent));
                //physics.Mass = 9.1f;   // ~90kg w/ earth gravity
                //physics.DynamicFrictionCoeffecient = 0.2f;
                //physics.Bounciness = 0.7f;
                //physics.StaticFrictionCoeffecient = 0.01f;

                // Animation Data
                //Idle
                SpriteAnimation idle = new SpriteAnimation((int)Animations.Idle, 1);
                idle.Loop = false;
                float animationDelay = 0.16f;

                idle.AddFrame(new AnimationFrame(content.Load<Texture2D>(@"pics\player\001_idleNN_01"), animationDelay));

                //Attack
                SpriteAnimation attack = new SpriteAnimation((int)Animations.Attack, 3);
                attack.Loop = true;
                attack.AddFrame(new AnimationFrame(content.Load<Texture2D>(@"pics\player\001_attackNN_01"), animationDelay));
                attack.AddFrame(new AnimationFrame(content.Load<Texture2D>(@"pics\player\001_attackNN_02"), animationDelay));
                attack.AddFrame(new AnimationFrame(content.Load<Texture2D>(@"pics\player\001_attackNN_03"), animationDelay));

                //Attack
                SpriteAnimation move = new SpriteAnimation((int)Animations.Move, 2);
                move.Loop = true;
                move.AddFrame(new AnimationFrame(content.Load<Texture2D>(@"pics\player\001_moveNN_01"), animationDelay));
                move.AddFrame(new AnimationFrame(content.Load<Texture2D>(@"pics\player\001_moveNN_02"), animationDelay));

                // Save static data
                //staticData.Add(movement);
                //staticData.Add(gravity);
                //staticData.Add(physics);

                //animations
                staticData.Add(idle);
                staticData.Add(attack);
                staticData.Add(move);

                SetStaticData(thisGameObjectType, staticData);
            }

            RenderComponent render = (RenderComponent)AllocateComponent(typeof(RenderComponent));
            render.Priority = SortConstants.PLAYER;
            render.CameraRelative = true;

            HitReactionComponent hitReact = AllocateComponent<HitReactionComponent>();
            hitReact.setTakeHitSound(HitType.HIT, playerHitSound);
            hitReact.setInvincibleAfterHitTime(playerInvincibleAfterHitTime);

            float collisionRadius = result.width / 2;
            float collisionOffsetX = collisionRadius;
            float collisionOffsetY = collisionRadius;

            FixedSizeArray<CollisionVolume> vulnerableVollumes = new FixedSizeArray<CollisionVolume>(1);
            vulnerableVollumes.Add(new SphereCollisionVolume(collisionRadius, collisionOffsetX, collisionOffsetY, HitType.HIT));

            DynamicCollisionComponent dynamicCollision = AllocateComponent<DynamicCollisionComponent>();
            dynamicCollision.SetHitReactionComponent(hitReact);
            dynamicCollision.SetCollisionVolumes(null, vulnerableVollumes);

            SpriteComponent sprite = (SpriteComponent)AllocateComponent(typeof(SpriteComponent));
            sprite.SetSize((int)result.width, (int)result.height);
            sprite.SetRenderComponent(render);
            //sprite.SetCollisionComponent(dynamicCollision);
            sprite.SetRenderMode(SpriteComponent.RenderMode.RotateToFacingDirection);

            BackgroundCollisionComponent bgcollision = (BackgroundCollisionComponent)AllocateComponent(typeof(BackgroundCollisionComponent));
            bgcollision.SetSize((int)result.width, (int)result.height);
            bgcollision.SetOffset(0, 0);
            bgcollision.CollideWithLevelBounds = true;

            InventoryComponent inventory = AllocateComponent<InventoryComponent>();
            inventory.SetUpdateRecord(new InventoryRecord());

            PlayerComponent player = (PlayerComponent)AllocateComponent(typeof(PlayerComponent));
            player.SetInventory(inventory);
            player.SetSpriteComponent(sprite);
            player.SetMoveSpeed(playerMoveSpeed);

            //AnimationComponent animation = (AnimationComponent)allocateComponent(AnimationComponent.class);
            //animation.setSprite(sprite);

            //animation.setPlayer(player);
            //SoundSystem soundSys = sSystemRegistry.SoundSystem;
            //if (soundSys != null) {
            //    animation.setDeathSound(soundSys.load(R.raw.gem1));
            //}

            //LaunchProjectileComponent gunTest = AllocateComponent<LaunchProjectileComponent>();
            //gunTest.SetProjectileSpeed(40);
            //SoundEffect sound = sSystemRegistry.Game.Content.Load<SoundEffect>(@"sounds\button-3");
            //gunTest.SetShootSound(sound);
            //gunTest.SetShotsPerSet(100);
            //gunTest.SetDelayBetweenShots(0.01f);
            //gunTest.SetOffsetX(result.width / 2);
            //gunTest.SetOffsetY(result.height / 2);
            //gunTest.SetSetsPerActivation(1);
            //gunTest.SetDelayBetweenSets(1.5f);
            //gunTest.SetThetaError(90);
            //gunTest.SetLanuchAngle(5);
            //gunTest.SetLaunchSpeed(500);
            //gunTest.SetRequiredAction(GameObject.ActionType.ATTACK);
            //gunTest.SetObjectTypeToSpawn((int)GeoManGameObjectTypes.Breackable_Block_Piece);

            LaunchProjectileComponent geometryWarsGun0 = AllocateComponent<LaunchProjectileComponent>();
            geometryWarsGun0.SetShootSound(pistolSound);
            //geometryWarsGun0.SetSetsPerActivation(1);
            geometryWarsGun0.SetShotsPerSet(1);
            geometryWarsGun0.SetDelayBetweenSets(playerFireDelay);
            geometryWarsGun0.SetOffsetX(result.width / 2);
            geometryWarsGun0.SetOffsetY(result.height / 2);
            geometryWarsGun0.SetThetaError(playerFireSeperation);
            geometryWarsGun0.SetLaunchStrategy(LaunchProjectileComponent.LaunchStrategy.UsingFacingDirection);
            geometryWarsGun0.SetLaunchSpeed(projectileSpeed);
            geometryWarsGun0.SetRequiredAction(GameObject.ActionType.Attack);
            geometryWarsGun0.SetObjectTypeToSpawn((int)RoverGameGameObjectTypes.Gun_Projectile);

            //player.setHitReactionComponent(hitReact);

            //animation.setInventory(inventory);

            LifetimeComponent lifetime = AllocateComponent<LifetimeComponent>();

            // Very very basic DDA.  Make the game easier if we've died on this level too much.
            //        LevelSystem level = sSystemRegistry.levelSystem;
            //        if (level != null) { 
            //        	player.adjustDifficulty(object, level.getAttemptsCount());
            //        }

            result.Add(geometryWarsGun0);

            //result.Add(ai);
            result.Add(player);
            result.Add(inventory);
            result.Add(bgcollision);
            result.Add(render);
            result.Add(lifetime);
            //result.add(animation);
            result.Add(sprite);
            result.Add(dynamicCollision);
            result.Add(hitReact);
            //result.add(damageSwap);
            //result.add(invincibleSwap);

            AddStaticData(thisGameObjectType, result, sprite);

            sprite.PlayAnimation((int)Animations.Idle);

            return result;
        }

        public GameObject SpawnTile(float positionX, float positionY, int tileType, int priority)
        {
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusTight;
            result.width = 32;
            result.height = 32;
            result.PositionLocked = true;
            result.DestroyOnDeactivation = false;


            FixedSizeArray<BaseObject> staticData = GetStaticData(tileType);
            if (staticData == null)
            {
                ContentManager content = sSystemRegistry.Game.Content;
                GraphicsDevice device = sSystemRegistry.Game.GraphicsDevice;

                int staticObjectCount = 1;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                Texture2D texture;
                int fileImageSize = 64;
                Rectangle crop = new Rectangle(0, 0, fileImageSize, fileImageSize);
                switch (tileType)
                {
                    default:
                    case (int)RoverGameGameObjectTypes.Tile_Blank:
                        texture = content.Load<Texture2D>(@"pics\tiles\dirt_01");
                        break;
                    case (int)RoverGameGameObjectTypes.Tile_Blocked:
                        texture = content.Load<Texture2D>(@"pics\tiles\dirt_01_blocked");
                        break;

                    case (int)RoverGameGameObjectTypes.Tile_BlockedTopLeft:
                        texture = content.Load<Texture2D>(@"pics\tiles\blocked_topLeftCorner");
                        break;

                    case (int)RoverGameGameObjectTypes.Tile_BlockedTopRight:
                        texture = content.Load<Texture2D>(@"pics\tiles\blocked_topRightCorner");
                        break;

                    case (int)RoverGameGameObjectTypes.Tile_BlockedBottomLeft:
                        texture = content.Load<Texture2D>(@"pics\tiles\blocked_bottomLeftCorner");
                        break;

                    case (int)RoverGameGameObjectTypes.Tile_BlockedBottomRight:
                        texture = content.Load<Texture2D>(@"pics\tiles\blocked_bottomRightCorner");
                        break;

                    case (int)RoverGameGameObjectTypes.Mario_Tile_Brick:
                        texture = marioSpriteSheet;
                        crop = new Rectangle(373, 102, 16, 16);
                        break;
                    case (int)RoverGameGameObjectTypes.Mario_Tile_Brick_Ground:
                        texture = marioSpriteSheet;
                        crop = new Rectangle(373, 124, 16, 16);
                        break;
                    case (int)RoverGameGameObjectTypes.Mario_Tile_Block:
                        texture = marioSpriteSheet;
                        crop = new Rectangle(373, 142, 16, 16);
                        break;
                    case (int)RoverGameGameObjectTypes.Mario_Tile_Mystery_Block:
                        texture = marioSpriteSheet;
                        crop = new Rectangle(372, 160, 16, 16);
                        break;
                    case (int)RoverGameGameObjectTypes.Mario_Tile_Mystery_Block_Empty:
                        texture = marioSpriteSheet;
                        crop = new Rectangle(373, 65, 16, 16);
                        break;
                    case (int)RoverGameGameObjectTypes.Mario_Tile_Flagpole:
                        texture = marioSpriteSheet;
                        crop = new Rectangle(268, 181, 16, 16);
                        break;
                    case (int)RoverGameGameObjectTypes.Mario_Tile_Flagpole_Top:
                        texture = marioSpriteSheet;
                        crop = new Rectangle(268, 44, 16, 16);
                        break;
                    case (int)RoverGameGameObjectTypes.Mario_Tile_Sky:
                        texture = marioSpriteSheet;
                        crop = new Rectangle(276, 343, 16, 16);
                        break;
                    case (int)RoverGameGameObjectTypes.Mario_Tile_Pipe_BottomLeft:
                        texture = marioSpriteSheet;
                        crop = new Rectangle(614, 63, 16, 16);
                        break;
                    case (int)RoverGameGameObjectTypes.Mario_Tile_Pipe_BottomRight:
                        texture = marioSpriteSheet;
                        crop = new Rectangle(630, 63, 16, 16);
                        break;
                    case (int)RoverGameGameObjectTypes.Mario_Tile_Pipe_TopLeft:
                        texture = marioSpriteSheet;
                        crop = new Rectangle(614, 46, 16, 16);
                        break;
                    case (int)RoverGameGameObjectTypes.Mario_Tile_Pipe_TopRight:
                        texture = marioSpriteSheet;
                        crop = new Rectangle(630, 46, 16, 16);
                        break;
                }

                DrawableTexture2D textureDrawable = new DrawableTexture2D(texture, (int)result.width, (int)result.height);
                textureDrawable.SetCrop(crop);

                RenderComponent render = (RenderComponent)AllocateComponent(typeof(RenderComponent));
                render.Priority = priority;
                render.setDrawable(textureDrawable);

                switch (tileType)
                {
                    default:
                    case (int)RoverGameGameObjectTypes.Tile_Blank:
                        //add anything else specific to this type type
                        break;
                }

                staticData.Add(render);
                SetStaticData(tileType, staticData);
            }

            switch (tileType)
            {
                default:
                    break;
                //case (int)GeoManGameObjectTypes.Tile_Blocked:
                //    //add anything else specific to this type type
                //    result.Add(GenerateRectangleSolidSurfaceComponent(result.width,result.height,false));
                //    break;
            }



            AddStaticData(tileType, result, null);

            return result;
        }

        public GameObject SpawnGuard(float positionX, float positionY)
        {
            int thisGameObjectType = (int)RoverGameGameObjectTypes.Guard;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusWide;
            result.width = 32;
            result.height = 32;

            result.life = guardHealth;
            result.team = GameObject.Team.AI;

            const int collisionWidth = 26;
            const int collisionHeight = 36;
            const int collisionOffsetX = 2;
            const int collisionOffsetY = 0;

            FixedSizeArray<BaseObject> staticData = GetStaticData(thisGameObjectType);

            if (staticData == null)
            {
                ContentManager content = sSystemRegistry.Game.Content;
                int staticObjectCount = 2;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                FreshGameComponent movement = (MovementComponent)AllocateComponent(typeof(MovementComponent));

                // Animation Data
                float animationDelay = 0.5f;
                Rectangle crop = new Rectangle(17, 28, 36, 36);
                //Idle
                SpriteAnimation idle = new SpriteAnimation((int)Animations.Idle, 1);
                idle.Loop = false;

                idle.AddFrame(new AnimationFrame(content.Load<Texture2D>(@"pics\enemies\guard\unit_guard_red_stand"), animationDelay, crop));

                // Save static data
                staticData.Add(movement);

                //animations
                staticData.Add(idle);

                SetStaticData(thisGameObjectType, staticData);
            }

            RenderComponent render = (RenderComponent)AllocateComponent(typeof(RenderComponent));
            render.Priority = SortConstants.GENERAL_ENEMY;
            render.CameraRelative = true;

            HitReactionComponent hitReact = AllocateComponent<HitReactionComponent>();
            hitReact.setTakeHitSound(HitType.HIT, ghostHitSound);
            hitReact.setInvincibleAfterHitTime(guardInvincibleAfterHitTime);

            FixedSizeArray<CollisionVolume> vollumes = new FixedSizeArray<CollisionVolume>(1);
            vollumes.Add(new AABoxCollisionVolume(collisionOffsetX, collisionOffsetY, collisionWidth, collisionHeight, HitType.HIT));

            DynamicCollisionComponent dynamicCollision = AllocateComponent<DynamicCollisionComponent>();
            dynamicCollision.SetHitReactionComponent(hitReact);
            dynamicCollision.SetCollisionVolumes(vollumes, vollumes);

            SpriteComponent sprite = (SpriteComponent)AllocateComponent(typeof(SpriteComponent));
            sprite.SetSize((int)result.width, (int)result.height);
            sprite.SetRenderComponent(render);
            //sprite.SetCollisionComponent(dynamicCollision);
            sprite.SetRenderMode(SpriteComponent.RenderMode.FlipAlongXAxis);

            BackgroundCollisionComponent bgcollision = (BackgroundCollisionComponent)AllocateComponent(typeof(BackgroundCollisionComponent));
            bgcollision.SetSize(collisionWidth, collisionHeight);
            bgcollision.SetOffset(collisionOffsetX, collisionOffsetY);

            //CircularAIComponent ai = AllocateComponent<CircularAIComponent>();
            //ai.Setup(500, 500, 500, 60);
            //ai.Active = true;

            LifetimeComponent lifetime = AllocateComponent<LifetimeComponent>();
            lifetime.SetObjectToSpawnOnDeath((int)RoverGameGameObjectTypes.Coin_Gold, true);

            //result.Add(ai);
            result.Add(bgcollision);
            result.Add(render);
            result.Add(lifetime);
            //result.add(animation);
            result.Add(sprite);
            result.Add(dynamicCollision);
            result.Add(hitReact);
            //result.add(damageSwap);
            //result.add(invincibleSwap);

            AddStaticData(thisGameObjectType, result, sprite);

            sprite.PlayAnimation((int)Animations.Idle);

            return result;
        }

        public GameObject SpawnNinja(float positionX, float positionY)
        {
            int thisGameObjectType = (int)RoverGameGameObjectTypes.Ninja;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusWide;
            result.width = 32;
            result.height = 32;

            result.life = ninjaHealth;
            result.team = GameObject.Team.AI;

            const int collisionWidth = 20;
            const int collisionHeight = 32;
            const int collisionOffsetX = 5;
            const int collisionOffsetY = 4;

            FixedSizeArray<BaseObject> staticData = GetStaticData(thisGameObjectType);

            if (staticData == null)
            {
                ContentManager content = sSystemRegistry.Game.Content;
                int staticObjectCount = 1;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                // Animation Data
                const float animationDelay = 0.5f;
                Rectangle crop = new Rectangle(17, 28, 36, 36);
                //Idle
                SpriteAnimation idle = new SpriteAnimation((int)Animations.Idle, 1);
                idle.Loop = false;

                idle.AddFrame(new AnimationFrame(content.Load<Texture2D>(@"pics\enemies\ninja\unit_ninja_red_stand"), animationDelay, crop));

                // Save static data
                //animations
                staticData.Add(idle);

                SetStaticData(thisGameObjectType, staticData);
            }

            RenderComponent render = (RenderComponent)AllocateComponent(typeof(RenderComponent));
            render.Priority = SortConstants.GENERAL_ENEMY;
            render.CameraRelative = true;

            HitReactionComponent hitReact = AllocateComponent<HitReactionComponent>();
            hitReact.setTakeHitSound(HitType.HIT, ghostHitSound);
            hitReact.setInvincibleAfterHitTime(ninjaInvincibleAfterHitTime);

            FixedSizeArray<CollisionVolume> vollumes = new FixedSizeArray<CollisionVolume>(1);
            vollumes.Add(new AABoxCollisionVolume(collisionOffsetX, collisionOffsetY, collisionWidth, collisionHeight, HitType.HIT));

            DynamicCollisionComponent dynamicCollision = AllocateComponent<DynamicCollisionComponent>();
            dynamicCollision.SetHitReactionComponent(hitReact);
            dynamicCollision.SetCollisionVolumes(vollumes, vollumes);

            SpriteComponent sprite = (SpriteComponent)AllocateComponent(typeof(SpriteComponent));
            sprite.SetSize((int)result.width, (int)result.height);
            sprite.SetRenderComponent(render);
            //sprite.SetCollisionComponent(dynamicCollision);
            sprite.SetRenderMode(SpriteComponent.RenderMode.FlipAlongXAxis);

            BackgroundCollisionComponent bgcollision = (BackgroundCollisionComponent)AllocateComponent(typeof(BackgroundCollisionComponent));
            bgcollision.SetSize(collisionWidth, collisionHeight);
            bgcollision.SetOffset(collisionOffsetX, collisionOffsetY);

            LaunchProjectileComponent shurikenThrower = AllocateComponent<LaunchProjectileComponent>();
            //geometryWarsGun0.SetShootSound(pistolSound);
            //geometryWarsGun0.SetSetsPerActivation(1);
            shurikenThrower.SetShotsPerSet(1);
            shurikenThrower.SetDelayBetweenSets(ninjaFireDelay);
            shurikenThrower.SetOffsetX(result.width / 2);
            shurikenThrower.SetOffsetY(result.height / 2);
            shurikenThrower.SetThetaError(ninjaFireSeperation);
            shurikenThrower.SetLaunchStrategy(LaunchProjectileComponent.LaunchStrategy.UsingFacingDirection);
            shurikenThrower.SetLaunchSpeed(shurikenSpeed);
            shurikenThrower.SetRequiredAction(GameObject.ActionType.Attack);
            shurikenThrower.SetObjectTypeToSpawn((int)RoverGameGameObjectTypes.Shuriken);

            PursuitAIComponent ai = AllocateComponent<PursuitAIComponent>();
            ai.SetSpeed(ninjaMoveSpeed);
            ai.PlayerIsTarget = ninjaTargetPlayer;
            ai.FaceMovementDir = true;
            ai.SetWanderConfig(ninjaWanderTargetRadius, ninjaWanderTargetDistance, ninjaWanderMaxJitter);

            LifetimeComponent lifetime = AllocateComponent<LifetimeComponent>();
            lifetime.SetObjectToSpawnOnDeath((int)RoverGameGameObjectTypes.Gem_Emerald, true);
            
            result.Add(ai);
            result.Add(bgcollision);
            result.Add(render);
            result.Add(lifetime);
            result.Add(sprite);
            result.Add(dynamicCollision);
            result.Add(hitReact);
            result.Add(shurikenThrower);
            
            AddStaticData(thisGameObjectType, result, sprite);

            sprite.PlayAnimation((int)Animations.Idle);

            return result;
        }

        public GameObject SpawnShuriken(float positionX, float positionY)
        {
            int thisGameObjectType = (int)RoverGameGameObjectTypes.Shuriken;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusTight;
            result.width = 16;
            result.height = 16;
            result.DestroyOnDeactivation = true;

            result.life = 1;

            int collisionRadius = (int)(result.width / 2);
            const int collisionOffsetX = 0;
            const int collisionOffsetY = 0;

            FixedSizeArray<BaseObject> staticData = GetStaticData(thisGameObjectType);

            if (staticData == null)
            {
                ContentManager content = sSystemRegistry.Game.Content;
                int staticObjectCount = 2;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                FreshGameComponent movement = (MovementComponent)AllocateComponent(typeof(MovementComponent));

                // Animation Data
                float animationDelay = 0.1f;
                Rectangle crop = new Rectangle(0, 0, 16, 16);
                //Idle
                SpriteAnimation idle = new SpriteAnimation((int)Animations.Idle, 2);
                idle.Loop = true;

                idle.AddFrame(new AnimationFrame(content.Load<Texture2D>(@"pics\enemies\ninja\projectile_shuriken"), animationDelay, crop));
                idle.AddFrame(new AnimationFrame(content.Load<Texture2D>(@"pics\enemies\ninja\projectile_shuriken_1"), animationDelay, crop));

                // Save static data
                staticData.Add(movement);

                //animations
                staticData.Add(idle);

                SetStaticData(thisGameObjectType, staticData);
            }

            RenderComponent render = (RenderComponent)AllocateComponent(typeof(RenderComponent));
            render.Priority = SortConstants.PROJECTILE;
            render.CameraRelative = true;

            HitReactionComponent hitReact = AllocateComponent<HitReactionComponent>();
            hitReact.setDieOnAttack(true);

            FixedSizeArray<CollisionVolume> vollumes = new FixedSizeArray<CollisionVolume>(1);
            vollumes.Add(new SphereCollisionVolume(collisionRadius, collisionRadius, collisionRadius, HitType.HIT));

            DynamicCollisionComponent dynamicCollision = AllocateComponent<DynamicCollisionComponent>();
            dynamicCollision.SetHitReactionComponent(hitReact);
            dynamicCollision.SetCollisionVolumes(vollumes, vollumes);

            SpriteComponent sprite = (SpriteComponent)AllocateComponent(typeof(SpriteComponent));
            sprite.SetSize((int)result.width, (int)result.height);
            sprite.SetRenderComponent(render);
            //sprite.SetCollisionComponent(dynamicCollision);
            //sprite.SetRenderMode(SpriteComponent.RenderMode.RotateToFacingDirection);

            BackgroundCollisionComponent bgcollision = (BackgroundCollisionComponent)AllocateComponent(typeof(BackgroundCollisionComponent));
            bgcollision.SetSize((int)result.width, (int)result.height);
            bgcollision.SetOffset(collisionOffsetX, collisionOffsetY);

            LifetimeComponent lifetime = AllocateComponent<LifetimeComponent>();
            lifetime.SetDieOnHitBackground(true);
            //lifetime.setObjectToSpawnOnDeath((int)GeoManGameObjectTypes.Particle_Burst);

            result.Add(bgcollision);
            result.Add(render);
            result.Add(lifetime);
            result.Add(sprite);
            result.Add(dynamicCollision);
            result.Add(hitReact);

            AddStaticData(thisGameObjectType, result, sprite);

            sprite.PlayAnimation((int)Animations.Idle);

            return result;
        }

        public GameObject SpawnZombie(float positionX, float positionY)
        {
            int thisGameObjectType = (int)RoverGameGameObjectTypes.Zombie;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusWide;
            result.width = 32;
            result.height = 32;

            result.life = zombieHealth;
            result.team = GameObject.Team.AI;

            const int collisionWidth = 20;
            const int collisionHeight = 32;
            const int collisionOffsetX = 5;
            const int collisionOffsetY = 4;

            FixedSizeArray<BaseObject> staticData = GetStaticData(thisGameObjectType);

            if (staticData == null)
            {
                ContentManager content = sSystemRegistry.Game.Content;
                int staticObjectCount = 2;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                FreshGameComponent movement = (MovementComponent)AllocateComponent(typeof(MovementComponent));

                // Animation Data
                float animationDelay = 0.5f;
                Rectangle crop = new Rectangle(17, 28, 36, 36);
                //Idle
                SpriteAnimation idle = new SpriteAnimation((int)Animations.Idle, 1);
                idle.Loop = false;

                idle.AddFrame(new AnimationFrame(content.Load<Texture2D>(@"pics\enemies\zombie\unit_zombie_red_stand"), animationDelay, crop));

                // Save static data
                staticData.Add(movement);

                //animations
                staticData.Add(idle);

                SetStaticData(thisGameObjectType, staticData);
            }

            RenderComponent render = (RenderComponent)AllocateComponent(typeof(RenderComponent));
            render.Priority = SortConstants.GENERAL_ENEMY;
            render.CameraRelative = true;

            HitReactionComponent hitReact = AllocateComponent<HitReactionComponent>();
            hitReact.setTakeHitSound(HitType.HIT, ghostHitSound);

            FixedSizeArray<CollisionVolume> vollumes = new FixedSizeArray<CollisionVolume>(1);
            vollumes.Add(new AABoxCollisionVolume(collisionOffsetX, collisionOffsetY, collisionWidth, collisionHeight, HitType.HIT));

            DynamicCollisionComponent dynamicCollision = AllocateComponent<DynamicCollisionComponent>();
            dynamicCollision.SetHitReactionComponent(hitReact);
            dynamicCollision.SetCollisionVolumes(vollumes, vollumes);

            SpriteComponent sprite = (SpriteComponent)AllocateComponent(typeof(SpriteComponent));
            sprite.SetSize((int)result.width, (int)result.height);
            sprite.SetRenderComponent(render);
            //sprite.SetCollisionComponent(dynamicCollision);
            sprite.SetRenderMode(SpriteComponent.RenderMode.FlipAlongXAxis);

            BackgroundCollisionComponent bgcollision = (BackgroundCollisionComponent)AllocateComponent(typeof(BackgroundCollisionComponent));
            bgcollision.SetSize(collisionWidth, collisionHeight);
            bgcollision.SetOffset(collisionOffsetX, collisionOffsetY);

            PursuitAIComponent ai = AllocateComponent<PursuitAIComponent>();
            ai.SetSpeed(zombieMoveSpeed);
            ai.PlayerIsTarget = zombieTargetPlayer;
            ai.FaceMovementDir = true;
            ai.SetHeading(FreshArchives.Random.Next(0, 360));
            ai.SetWanderConfig(zombieWanderTargetRadius, zombieWanderTargetDistance, zombieWanderMaxJitter);

            LifetimeComponent lifetime = AllocateComponent<LifetimeComponent>();
            lifetime.SetObjectToSpawnOnDeath((int)RoverGameGameObjectTypes.Coin_Silver, true);


            result.Add(ai);
            result.Add(bgcollision);
            result.Add(render);
            result.Add(lifetime);
            //result.add(animation);
            result.Add(sprite);
            result.Add(dynamicCollision);
            result.Add(hitReact);
            //result.add(damageSwap);
            //result.add(invincibleSwap);

            AddStaticData(thisGameObjectType, result, sprite);

            sprite.PlayAnimation((int)Animations.Idle);

            return result;
        }

        public GameObject SpawnZombieSpawner(float positionX, float positionY)
        {
            int thisGameObjectType = (int)RoverGameGameObjectTypes.ZombieSpawner;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusWide;
            result.width = 32;
            result.height = 32;

            result.team = GameObject.Team.AI;


            FixedSizeArray<BaseObject> staticData = GetStaticData(thisGameObjectType);

            if (staticData == null)
            {
                ContentManager content = sSystemRegistry.Game.Content;
                int staticObjectCount = 1;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                DrawableTexture2D textureDrawable = new DrawableTexture2D(content.Load<Texture2D>(@"pics\enemies\zombie\zombieSpawnPoint"), (int)result.width, (int)result.height);
                textureDrawable.SetCrop(new Rectangle(0,0,64,64));

                RenderComponent render = (RenderComponent)AllocateComponent(typeof(RenderComponent));
                render.Priority = SortConstants.GENERAL_OBJECT;
                render.setDrawable(textureDrawable);

                staticData.Add(render);

                SetStaticData(thisGameObjectType, staticData);
            }

            LaunchProjectileComponent spawner = AllocateComponent<LaunchProjectileComponent>();
            spawner.SetObjectTypeToSpawn((int)RoverGameGameObjectTypes.Zombie);
            spawner.SetLaunchSpeed(0);
            spawner.SetDelayBetweenSets(zombieSpawnerSpawnDelay);
            spawner.SetShotsPerSet(1);
            spawner.SetThetaError(360);
            spawner.SetOffsetX(result.width / 2);
            spawner.SetOffsetY(result.height / 2);
            spawner.EnableProjectileTracking(zombieSpawnerMaxActive);

            result.Add(spawner);

            AddStaticData(thisGameObjectType, result, null);

            return result;
        }

        public FixedSizeArray<BaseObject> GetStaticData_GenericGhost()
        {

            FixedSizeArray<BaseObject> staticData = GetStaticData((int)RoverGameGameObjectTypes.Ghost_Generic);

            if (staticData == null)
            {
                ContentManager content = sSystemRegistry.Game.Content;
                int staticObjectCount = 2;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                const int spacing = 20;
                const int XOffset = 7;
                const int YOffset = 87;
                const int clearBorder = 1;
                const int tileSize = 14;
                const int spriteSize = tileSize + clearBorder * 2;
                const int fearCropY = 4;
                const int eyesCropY = 6;
                
                const int upTileIndex = 0;
                const int downTileIndex = 1;
                const int leftTileIndex = 2;
                const int rightTileIndex = 3;

                Rectangle cropWorkspace;

                // Animation Data
                float animationDelay = 0.5f;
                //Fear - omni directional
                SpriteAnimation fear = new SpriteAnimation((int)Animations.Fear, 4);
                fear.Loop = true;
                cropWorkspace = new Rectangle(XOffset + spacing * 0 - clearBorder, YOffset + spacing * fearCropY - clearBorder, spriteSize, spriteSize);
                fear.AddFrame(new AnimationFrame(pacmanSpriteSheet, animationDelay, cropWorkspace));
                cropWorkspace = new Rectangle(XOffset + spacing * 1 - clearBorder, YOffset + spacing * fearCropY - clearBorder, spriteSize, spriteSize);
                fear.AddFrame(new AnimationFrame(pacmanSpriteSheet, animationDelay, cropWorkspace));
                cropWorkspace = new Rectangle(XOffset + spacing * 2 - clearBorder, YOffset + spacing * fearCropY - clearBorder, spriteSize, spriteSize);
                fear.AddFrame(new AnimationFrame(pacmanSpriteSheet, animationDelay, cropWorkspace));
                cropWorkspace = new Rectangle(XOffset + spacing * 3 - clearBorder, YOffset + spacing * fearCropY - clearBorder, spriteSize, spriteSize);
                fear.AddFrame(new AnimationFrame(pacmanSpriteSheet, animationDelay, cropWorkspace));

                //eyes - 4 directions
                SpriteAnimation eyesDown = new SpriteAnimation((int)Animations.Dead_Move, 1);
                cropWorkspace = new Rectangle(XOffset + spacing * downTileIndex - clearBorder, YOffset + spacing * eyesCropY - clearBorder, spriteSize, spriteSize);
                eyesDown.AddFrame(new AnimationFrame(pacmanSpriteSheet, animationDelay, cropWorkspace));
                SpriteAnimation eyesLeft = new SpriteAnimation((int)Animations.Dead_Move, 1);
                cropWorkspace = new Rectangle(XOffset + spacing * leftTileIndex - clearBorder, YOffset + spacing * eyesCropY - clearBorder, spriteSize, spriteSize);
                eyesLeft.AddFrame(new AnimationFrame(pacmanSpriteSheet, animationDelay, cropWorkspace));
                SpriteAnimation eyesRight = new SpriteAnimation((int)Animations.Dead_Move, 1);
                cropWorkspace = new Rectangle(XOffset + spacing * rightTileIndex - clearBorder, YOffset + spacing * eyesCropY - clearBorder, spriteSize, spriteSize);
                eyesRight.AddFrame(new AnimationFrame(pacmanSpriteSheet, animationDelay, cropWorkspace));
                SpriteAnimation eyesUp = new SpriteAnimation((int)Animations.Dead_Move, 1);
                cropWorkspace = new Rectangle(XOffset + spacing * upTileIndex - clearBorder, YOffset + spacing * eyesCropY - clearBorder, spriteSize, spriteSize);
                eyesUp.AddFrame(new AnimationFrame(pacmanSpriteSheet, animationDelay, cropWorkspace));

                SpriteAnimation_8Way eyes = new SpriteAnimation_8Way((int)Animations.Dead_Move, eyesUp, eyesRight, eyesDown, eyesLeft);
                eyes.defaultDir = Dir.Down;

                //animations
                staticData.Add(fear);
                staticData.Add(eyes);

                SetStaticData((int)RoverGameGameObjectTypes.Ghost_Generic, staticData);
            }
            return staticData;
        }

        public GameObject SpawnGhost(float positionX, float positionY, int type)
        {
            int thisGameObjectType = type;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusWide;
            result.width = 32;
            result.height = 32;

            result.life = ghostHealth;
            result.team = GameObject.Team.AI;


            FixedSizeArray<BaseObject> staticData = GetStaticData(thisGameObjectType);

            if (staticData == null)
            {
                ContentManager content = sSystemRegistry.Game.Content;
                int staticObjectCount = 2;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                const int spacing = 20;
                const int XOffset = 7;
                const int YOffset = 87;
                const int clearBorder = 1;
                const int tileSize = 14;
                const int spriteSize = tileSize + clearBorder * 2;

                int colorCropY;

                switch (type)
                {
                    case (int)RoverGameGameObjectTypes.Ghost_Red:
                    default:
                        colorCropY = 0;
                        break;
                    case (int)RoverGameGameObjectTypes.Ghost_Pink:
                        colorCropY = 1;
                        break;
                    case (int)RoverGameGameObjectTypes.Ghost_Blue:
                        colorCropY = 2;
                        break;
                    case (int)RoverGameGameObjectTypes.Ghost_Orange:
                        colorCropY = 3;
                        break;
                }

                ///this should change with animation and eyes
                const int upTileIndex = 0;
                const int downTileIndex = 1;
                const int leftTileIndex = 2;
                const int rightTileIndex = 3;

                Rectangle cropWorkspace;

                // Animation Data
                float animationDelay = 0.4f;

                //Move
                SpriteAnimation moveUp, moveRight, moveDown, moveLeft;
                moveUp = null;
                moveRight = null;
                moveDown = null;
                moveLeft = null;
                for (int dirNo = 0; dirNo < 4; dirNo++)
                {
                    SpriteAnimation moveAnim = new SpriteAnimation((int)Animations.Move, 2);
                    moveAnim.Loop = true;
                    cropWorkspace = new Rectangle(XOffset + spacing * (dirNo * 2) - clearBorder, YOffset + spacing * colorCropY - clearBorder, spriteSize, spriteSize);
                    moveAnim.AddFrame(new AnimationFrame(pacmanSpriteSheet, animationDelay, cropWorkspace));
                    cropWorkspace = new Rectangle(XOffset + spacing * (dirNo * 2 + 1) - clearBorder, YOffset + spacing * colorCropY - clearBorder, spriteSize, spriteSize);
                    moveAnim.AddFrame(new AnimationFrame(pacmanSpriteSheet, animationDelay, cropWorkspace));


                    switch (dirNo)
                    {
                        case upTileIndex:
                            moveUp = moveAnim;
                            break;
                        case downTileIndex:
                            moveDown = moveAnim;
                            break;
                        case rightTileIndex:
                            moveRight = moveAnim;
                            break;
                        case leftTileIndex:
                            moveLeft = moveAnim;
                            break;
                    }
                }
                SpriteAnimation_8Way move = new SpriteAnimation_8Way((int)Animations.Move, moveUp, moveRight, moveDown, moveLeft);

                //animations
                staticData.Add(move);

                staticData = FixedSizeArray<BaseObject>.Merge(staticData, GetStaticData_GenericGhost());

                SetStaticData(thisGameObjectType, staticData);
            }

            RenderComponent render = (RenderComponent)AllocateComponent(typeof(RenderComponent));
            render.Priority = SortConstants.GENERAL_ENEMY;
            render.CameraRelative = true;

            HitReactionComponent hitReact = AllocateComponent<HitReactionComponent>();
            hitReact.setTakeHitSound(HitType.HIT, ghostHitSound);

            float collisionOffsetX = 0;
            float collisionOffsetY = 0;
            float collisionWidth = result.width;
            float collisionHeight = result.height;

            FixedSizeArray<CollisionVolume> attackVollumes = new FixedSizeArray<CollisionVolume>(1);
            attackVollumes.Add(new AABoxCollisionVolume(collisionOffsetX, collisionOffsetY, collisionWidth, collisionHeight, HitType.HIT));

            //FixedSizeArray<CollisionVolume> vulnerableVollumes = new FixedSizeArray<CollisionVolume>(1);
            //vulnerableVollumes.Add(new AABoxCollisionVolume(collisionOffsetX, collisionOffsetY, collisionWidth, collisionHeight, HitType.HIT));

            DynamicCollisionComponent dynamicCollision = AllocateComponent<DynamicCollisionComponent>();
            dynamicCollision.SetHitReactionComponent(hitReact);
            dynamicCollision.SetCollisionVolumes(attackVollumes, attackVollumes);

            BackgroundCollisionComponent bgcollision = AllocateComponent<BackgroundCollisionComponent>();
            bgcollision.SetSize((int)collisionWidth, (int)collisionHeight);
            bgcollision.SetOffset((int)collisionOffsetX, (int)collisionOffsetY);
            bgcollision.CollideWithLevelBounds = true;

            SpriteComponent sprite = (SpriteComponent)AllocateComponent(typeof(SpriteComponent));
            sprite.SetSize((int)result.width, (int)result.height);
            sprite.SetRenderComponent(render);


            LevelSystem levelSys = sSystemRegistry.LevelSystem;

            int startAngle = FreshArchives.Random.Next(0, 360);
            int speed = 150;
            int targetRadius = 50;
            int targetDistance = 10;
            int maxJitter = 2;
            switch (type)
            {
                default:
                case (int)RoverGameGameObjectTypes.Ghost_Red:
                    //chase
                    targetDistance = 20;
                    break;
                case (int)RoverGameGameObjectTypes.Ghost_Orange:
                    //random
                    targetDistance = 6;
                    maxJitter = 10;
                    break;
                case (int)RoverGameGameObjectTypes.Ghost_Pink:
                    //speed
                    speed = 200;
                    break;
                case (int)RoverGameGameObjectTypes.Ghost_Blue:
                    //slow
                    speed = 100;
                    break;
            }

            WanderAIComponent ai = (WanderAIComponent)AllocateComponent(typeof(WanderAIComponent));
            ai.SetSpeed(speed);
            ai.SetHeading(startAngle);
            ai.SetWanderConfig(targetRadius, targetDistance, maxJitter);
            ai.FaceMovementDir = true;

            LifetimeComponent life = AllocateComponent<LifetimeComponent>();
            life.SetObjectToSpawnOnDeath((int)RoverGameGameObjectTypes.Ghost_Eyes, true);

            //const int inset = 1;
            //SolidSurfaceComponent solid = GenerateRectangleSolidSurfaceComponent(result.width - inset * 2, result.height - inset * 2, false);
            //solid.SetOffset(inset, inset);
            //result.Add(solid);

            result.Add(bgcollision);
            result.Add(render);
            result.Add(sprite);
            result.Add(ai);
            result.Add(hitReact);
            result.Add(life);
            result.Add(dynamicCollision);

            AddStaticData(thisGameObjectType, result, sprite);

            sprite.PlayAnimation((int)Animations.Move);

            return result;
        }

        public GameObject SpawnGhostEyes(float positionX, float positionY)
        {
            int thisGameObjectType = (int)RoverGameGameObjectTypes.Ghost_Eyes;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusTight;
            result.width = 32;
            result.height = 32;
            result.DestroyOnDeactivation = true;

            result.life = 1;
            result.team = GameObject.Team.AI;


            FixedSizeArray<BaseObject> staticData = GetStaticData(thisGameObjectType);

            if (staticData == null)
            {
                ContentManager content = sSystemRegistry.Game.Content;
                int staticObjectCount = 2;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                staticData = GetStaticData_GenericGhost();

                SetStaticData(thisGameObjectType, staticData);
            }

            RenderComponent render = (RenderComponent)AllocateComponent(typeof(RenderComponent));
            render.Priority = SortConstants.GENERAL_ENEMY;
            render.CameraRelative = true;

            SpriteComponent sprite = (SpriteComponent)AllocateComponent(typeof(SpriteComponent));
            sprite.SetSize((int)result.width, (int)result.height);
            sprite.SetRenderComponent(render);

            int startAngle = FreshArchives.Random.Next(0, 360);
            int speed = 450;
            int targetRadius = 50;
            int targetDistance = 10;
            int maxJitter = 2;

            WanderAIComponent ai = (WanderAIComponent)AllocateComponent(typeof(WanderAIComponent));
            ai.SetSpeed(speed);
            ai.SetHeading(startAngle);
            ai.SetWanderConfig(targetRadius, targetDistance, maxJitter);
            ai.FaceMovementDir = true;

            LifetimeComponent life = AllocateComponent<LifetimeComponent>();
            life.SetDieWhenInvisible(true);

            result.Add(render);
            result.Add(sprite);
            result.Add(ai);
            result.Add(life);

            AddStaticData(thisGameObjectType, result, sprite);

            sprite.PlayAnimation((int)Animations.Dead_Move);

            return result;
        }

        public GameObject SpawnRing(float positionX, float positionY)
        {
            int type = (int)RoverGameGameObjectTypes.Collectable;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusTight;
            result.width = 32;
            result.height = 32;
            result.life = 1;

            FixedSizeArray<BaseObject> staticData = GetStaticData(type);
            if (staticData == null)
            {
                ContentManager content = sSystemRegistry.Game.Content;
                const int staticObjectCount = 3;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                SpriteAnimation idle = new SpriteAnimation(0, 4);
                idle.Loop = true;
                float animationDelay = FreshArchives.FramesToTime(24, 3);
                Rectangle crop = new Rectangle(0, 0, 128, 128);
                idle.AddFrame(new AnimationFrame(content.Load<Texture2D>(@"pics\collectables\Sonic_Ring_Frame_00"), animationDelay, crop));
                idle.AddFrame(new AnimationFrame(content.Load<Texture2D>(@"pics\collectables\Sonic_Ring_Frame_01"), animationDelay, crop));
                idle.AddFrame(new AnimationFrame(content.Load<Texture2D>(@"pics\collectables\Sonic_Ring_Frame_02"), animationDelay, crop));
                idle.AddFrame(new AnimationFrame(content.Load<Texture2D>(@"pics\collectables\Sonic_Ring_Frame_03"), animationDelay, crop));

                InventoryRecord addCoin = new InventoryRecord();
                addCoin.itemCount = scorePerRing;


                RenderComponent render = AllocateComponent<RenderComponent>();
                render.Priority = SortConstants.GENERAL_OBJECT;

                SpriteComponent sprite = AllocateComponent<SpriteComponent>();
                sprite.SetSize((int)result.width, (int)result.height);
                sprite.SetRenderComponent(render);
                sprite.BacedOnGlobalTime = true;

                sprite.AddAnimation(idle);
                sprite.PlayAnimation(0);

                staticData.Add(render);
                staticData.Add(sprite);

                //staticData.Add(idle); - set up above
                staticData.Add(addCoin);

                SetStaticData(type, staticData);
            }

            HitReactionComponent hitReact = AllocateComponent<HitReactionComponent>();
            hitReact.setDieWhenCollected(true);
            //hitReact.setInvincible(true);

            hitReact.setTakeHitSound(HitType.COLLECT, ringSound);

            HitPlayerComponent hitPlayer = AllocateComponent<HitPlayerComponent>();
            //this distance should be half the player size
            hitPlayer.Setup(result.width * 0.75f, hitReact, HitType.COLLECT, false);


            // TODO: this is pretty dumb.  The static data binding needs to be made generic.
            int staticDataSize = staticData.GetCount();
            for (int x = 0; x < staticDataSize; x++)
            {
                BaseObject entry = staticData.Get(x);
                if (entry is InventoryComponent.UpdateRecord)
                {
                    hitReact.setInventoryUpdate((InventoryComponent.UpdateRecord)entry);
                    break;
                }
            }

            LifetimeComponent life = AllocateComponent<LifetimeComponent>();

            result.Add(hitPlayer);
            result.Add(hitReact);
            result.Add(life);

            AddStaticData(type, result, null);

            return result;
        }

        public GameObject SpawnCoin_Gold(float positionX, float positionY)
        {
            int type = (int)RoverGameGameObjectTypes.Coin_Gold;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusTight;
            result.width = 16;
            result.height = 16;
            result.life = 1;

            FixedSizeArray<BaseObject> staticData = GetStaticData(type);
            if (staticData == null)
            {
                ContentManager content = sSystemRegistry.Game.Content;
                const int staticObjectCount = 3;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                SpriteAnimation idle = new SpriteAnimation(0, 7);
                idle.Loop = true;
                float animationDelay = FreshArchives.FramesToTime(24, 3);
                const int size = 16;

                Texture2D texture = content.Load<Texture2D>(@"pics\collectables\pickup_coin_gold_16");

                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 0, 0, size, size)));
                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 1, 0, size, size)));
                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 2, 0, size, size)));
                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 3, 0, size, size)));
                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 4, 0, size, size)));
                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 5, 0, size, size)));
                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 6, 0, size, size)));

                InventoryRecord addCoin = new InventoryRecord();
                addCoin.itemCount = scorePerGoldCoin;


                RenderComponent render = AllocateComponent<RenderComponent>();
                render.Priority = SortConstants.GENERAL_OBJECT;

                SpriteComponent sprite = AllocateComponent<SpriteComponent>();
                sprite.SetSize((int)result.width, (int)result.height);
                sprite.SetRenderComponent(render);
                sprite.BacedOnGlobalTime = true;

                sprite.AddAnimation(idle);
                sprite.PlayAnimation(0);

                staticData.Add(render);
                staticData.Add(sprite);

                //staticData.Add(idle); - set up above
                staticData.Add(addCoin);

                SetStaticData(type, staticData);
            }

            HitReactionComponent hitReact = AllocateComponent<HitReactionComponent>();
            hitReact.setDieWhenCollected(true);
            //hitReact.setInvincible(true);

            hitReact.setTakeHitSound(HitType.COLLECT, ringSound);

            HitPlayerComponent hitPlayer = AllocateComponent<HitPlayerComponent>();
            //this distance should be half the player size
            hitPlayer.Setup(result.width, hitReact, HitType.COLLECT, false);


            // TODO: this is pretty dumb.  The static data binding needs to be made generic.
            int staticDataSize = staticData.GetCount();
            for (int x = 0; x < staticDataSize; x++)
            {
                BaseObject entry = staticData.Get(x);
                if (entry is InventoryComponent.UpdateRecord)
                {
                    hitReact.setInventoryUpdate((InventoryComponent.UpdateRecord)entry);
                    break;
                }
            }

            LifetimeComponent life = AllocateComponent<LifetimeComponent>();

            result.Add(hitPlayer);
            result.Add(hitReact);
            result.Add(life);

            AddStaticData(type, result, null);

            return result;
        }

        public GameObject SpawnCoin_Silver(float positionX, float positionY)
        {
            int type = (int)RoverGameGameObjectTypes.Coin_Silver;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusTight;
            result.width = 16;
            result.height = 16;
            result.life = 1;

            FixedSizeArray<BaseObject> staticData = GetStaticData(type);
            if (staticData == null)
            {
                ContentManager content = sSystemRegistry.Game.Content;
                const int staticObjectCount = 3;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                SpriteAnimation idle = new SpriteAnimation(0, 7);
                idle.Loop = true;
                float animationDelay = FreshArchives.FramesToTime(24, 3);
                Rectangle crop = new Rectangle(0, 0, 128, 128);
                const int size = 8;

                Texture2D texture = content.Load<Texture2D>(@"pics\collectables\pickup_coin_silver_small_8");

                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 0, 0, size, size)));
                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 1, 0, size, size)));
                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 2, 0, size, size)));
                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 3, 0, size, size)));
                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 4, 0, size, size)));
                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 5, 0, size, size)));
                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 6, 0, size, size)));

                InventoryRecord addCoin = new InventoryRecord();
                addCoin.itemCount = scorePerSilverCoin;


                RenderComponent render = AllocateComponent<RenderComponent>();
                render.Priority = SortConstants.GENERAL_OBJECT;

                SpriteComponent sprite = AllocateComponent<SpriteComponent>();
                sprite.SetSize((int)result.width, (int)result.height);
                sprite.SetRenderComponent(render);
                sprite.BacedOnGlobalTime = true;

                sprite.AddAnimation(idle);
                sprite.PlayAnimation(0);

                staticData.Add(render);
                staticData.Add(sprite);

                //staticData.Add(idle); - set up above
                staticData.Add(addCoin);

                SetStaticData(type, staticData);
            }

            HitReactionComponent hitReact = AllocateComponent<HitReactionComponent>();
            hitReact.setDieWhenCollected(true);
            //hitReact.setInvincible(true);

            hitReact.setTakeHitSound(HitType.COLLECT, ringSound);

            HitPlayerComponent hitPlayer = AllocateComponent<HitPlayerComponent>();
            //this distance should be half the player size
            hitPlayer.Setup(result.width, hitReact, HitType.COLLECT, false);


            // TODO: this is pretty dumb.  The static data binding needs to be made generic.
            int staticDataSize = staticData.GetCount();
            for (int x = 0; x < staticDataSize; x++)
            {
                BaseObject entry = staticData.Get(x);
                if (entry is InventoryComponent.UpdateRecord)
                {
                    hitReact.setInventoryUpdate((InventoryComponent.UpdateRecord)entry);
                    break;
                }
            }

            LifetimeComponent life = AllocateComponent<LifetimeComponent>();

            result.Add(hitPlayer);
            result.Add(hitReact);
            result.Add(life);

            AddStaticData(type, result, null);

            return result;
        }

        public GameObject SpawnGem_Emerald(float positionX, float positionY)
        {
            int type = (int)RoverGameGameObjectTypes.Gem_Emerald;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusTight;
            result.width = 16;
            result.height = 16;
            result.life = 1;

            FixedSizeArray<BaseObject> staticData = GetStaticData(type);
            if (staticData == null)
            {
                ContentManager content = sSystemRegistry.Game.Content;
                const int staticObjectCount = 3;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                SpriteAnimation idle = new SpriteAnimation(0, 7);
                idle.Loop = true;
                float animationDelay = FreshArchives.FramesToTime(24, 3);
                const int size = 16;

                Texture2D texture = content.Load<Texture2D>(@"pics\collectables\pickup_gem_emerald_12");

                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 0, 0, size, size)));
                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 1, 0, size, size)));
                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 2, 0, size, size)));
                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 3, 0, size, size)));
                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 4, 0, size, size)));
                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 5, 0, size, size)));
                idle.AddFrame(new AnimationFrame(texture, animationDelay, new Rectangle(size * 6, 0, size, size)));

                InventoryRecord addCoin = new InventoryRecord();
                addCoin.itemCount = scorePerEmerald;


                RenderComponent render = AllocateComponent<RenderComponent>();
                render.Priority = SortConstants.GENERAL_OBJECT;

                SpriteComponent sprite = AllocateComponent<SpriteComponent>();
                sprite.SetSize((int)result.width, (int)result.height);
                sprite.SetRenderComponent(render);
                sprite.BacedOnGlobalTime = true;

                sprite.AddAnimation(idle);
                sprite.PlayAnimation(0);

                staticData.Add(render);
                staticData.Add(sprite);

                //staticData.Add(idle); - set up above
                staticData.Add(addCoin);

                SetStaticData(type, staticData);
            }

            HitReactionComponent hitReact = AllocateComponent<HitReactionComponent>();
            hitReact.setDieWhenCollected(true);
            //hitReact.setInvincible(true);

            hitReact.setTakeHitSound(HitType.COLLECT, ringSound);

            HitPlayerComponent hitPlayer = AllocateComponent<HitPlayerComponent>();
            //this distance should be half the player size
            hitPlayer.Setup(result.width, hitReact, HitType.COLLECT, false);


            // TODO: this is pretty dumb.  The static data binding needs to be made generic.
            int staticDataSize = staticData.GetCount();
            for (int x = 0; x < staticDataSize; x++)
            {
                BaseObject entry = staticData.Get(x);
                if (entry is InventoryComponent.UpdateRecord)
                {
                    hitReact.setInventoryUpdate((InventoryComponent.UpdateRecord)entry);
                    break;
                }
            }

            LifetimeComponent life = AllocateComponent<LifetimeComponent>();

            result.Add(hitPlayer);
            result.Add(hitReact);
            result.Add(life);

            AddStaticData(type, result, null);

            return result;
        }

        public GameObject SpawnWinObject(float positionX, float positionY)
        {
            int type = (int)RoverGameGameObjectTypes.WinObject;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusTight;
            result.width = 32;
            result.height = 32;
            result.life = 1;

            ContentManager content = sSystemRegistry.Game.Content;

            FixedSizeArray<BaseObject> staticData = GetStaticData(type);
            if (staticData == null)
            {
                const int staticObjectCount = 1;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);
                
                InventoryRecord addWin = new InventoryRecord();
                addWin.winCount = 1;

                staticData.Add(addWin);

                SetStaticData(type, staticData);
            }

            Rectangle crop = new Rectangle(0, 0, 64, 64);
            DrawableTexture2D textureDrawable = new DrawableTexture2D(content.Load<Texture2D>(@"pics\misc\winItem"), (int)result.width, (int)result.height);
            textureDrawable.SetCrop(crop);

            RenderComponent render = (RenderComponent)AllocateComponent(typeof(RenderComponent));
            render.Priority = SortConstants.GENERAL_OBJECT;
            render.setDrawable(textureDrawable);

            HitReactionComponent hitReact = AllocateComponent<HitReactionComponent>();
            hitReact.setDieWhenCollected(true);
            hitReact.setInvincible(true);

            hitReact.setTakeHitSound(HitType.COLLECT, winSound);

            HitPlayerComponent hitPlayer = AllocateComponent<HitPlayerComponent>();
            //this distance should be half the player size
            hitPlayer.Setup(result.width * 0.75f, hitReact, HitType.COLLECT, false);


            // TODO: this is pretty dumb.  The static data binding needs to be made generic.
            int staticDataSize = staticData.GetCount();
            for (int x = 0; x < staticDataSize; x++)
            {
                BaseObject entry = staticData.Get(x);
                if (entry is InventoryComponent.UpdateRecord)
                {
                    hitReact.setInventoryUpdate((InventoryComponent.UpdateRecord)entry);
                    break;
                }
            }

            LifetimeComponent life = AllocateComponent<LifetimeComponent>();

            result.Add(hitPlayer);
            result.Add(hitReact);
            result.Add(life);
            result.Add(render);

            //AddStaticData(type, result, null);

            return result;
        }

        public GameObject SpawnBreakableBlockPiece(float positionX, float positionY)
        {
            int type = (int)RoverGameGameObjectTypes.Breackable_Block_Piece;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusTight;
            result.width = 16;
            result.height = 16;
            result.DestroyOnDeactivation = true;

            FixedSizeArray<BaseObject> staticData = GetStaticData(type);
            if (staticData == null)
            {
                int staticObjectCount = 4;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                FreshGameComponent gravity = AllocateComponent<GravityComponent>();
                FreshGameComponent movement = AllocateComponent<MovementComponent>();

                SimplePhysicsComponent physics = AllocateComponent<SimplePhysicsComponent>();
                physics.setBounciness(0.5f);

                DrawableTexture2D textureDrawable = new DrawableTexture2D(
                        FreshArchives.GetWhiteSquare(sSystemRegistry.Game.GraphicsDevice),
                        (int)result.width,
                        (int)result.height);


                RenderComponent render = AllocateComponent<RenderComponent>();
                render.Priority = SortConstants.GENERAL_OBJECT;
                render.setDrawable(textureDrawable);

                staticData.Add(render);
                staticData.Add(movement);
                staticData.Add(gravity);
                staticData.Add(physics);
                SetStaticData(type, staticData);
            }

            LifetimeComponent lifetime = AllocateComponent<LifetimeComponent>();
            //lifetime.setTimeUntilDeath((float)(FreshArchives.Random.NextDouble()*5+3));
            lifetime.SetTimeUntilDeath(2f);

            lifetime.SetDeathSound(playerHitSound);

            BackgroundCollisionComponent bgcollision = AllocateComponent<BackgroundCollisionComponent>();
            bgcollision.SetSize(12, 12);
            bgcollision.SetOffset(2, 2);


            result.Add(lifetime);
            result.Add(bgcollision);

            AddStaticData(type, result, null);

            return result;
        }

        public GameObject SpawnGunProjectile(float positionX, float positionY)
        {
            int type = (int)RoverGameGameObjectTypes.Gun_Projectile;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusTight;
            result.width = 16;
            result.height = 16;
            result.DestroyOnDeactivation = true;

            const int collisionTollerance = 0;

            FixedSizeArray<BaseObject> staticData = GetStaticData(type);
            if (staticData == null)
            {
                int staticObjectCount = 2;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                FreshGameComponent movement = AllocateComponent<MovementComponent>();

                DrawableTexture2D textureDrawable = new DrawableTexture2D(
                        FreshArchives.GetWhiteSquare(sSystemRegistry.Game.GraphicsDevice),
                        (int)result.width,
                        (int)result.height);


                RenderComponent render = AllocateComponent<RenderComponent>();
                render.Priority = SortConstants.PROJECTILE;
                render.setDrawable(textureDrawable);

                staticData.Add(render);
                staticData.Add(movement);
                SetStaticData(type, staticData);
            }

            LifetimeComponent lifetime = AllocateComponent<LifetimeComponent>();
            lifetime.SetDieOnHitBackground(true);
            lifetime.SetObjectToSpawnOnDeath((int)RoverGameGameObjectTypes.Particle_Burst, true);

            //SoundEffect sound = sSystemRegistry.Game.Content.Load<SoundEffect>(@"sounds\Sonic_Vanish");
            //lifetime.SetDeathSound(sound);

            HitReactionComponent hitReact = AllocateComponent<HitReactionComponent>();
            hitReact.setDieOnAttack(true);
            
            float collisionWidth = result.width - collisionTollerance * 2;
            float collisionHeight = result.height - collisionTollerance * 2;
            
            FixedSizeArray<CollisionVolume> attackVollumes = new FixedSizeArray<CollisionVolume>(1);
            attackVollumes.Add(new AABoxCollisionVolume(collisionTollerance, collisionTollerance, collisionWidth, collisionHeight, HitType.HIT));

            DynamicCollisionComponent dynamicCollision = AllocateComponent<DynamicCollisionComponent>();
            dynamicCollision.SetHitReactionComponent(hitReact);
            dynamicCollision.SetCollisionVolumes(attackVollumes, null);

            BackgroundCollisionComponent bgcollision = AllocateComponent<BackgroundCollisionComponent>();
            bgcollision.SetSize((int)collisionWidth, (int)collisionHeight);
            bgcollision.SetOffset(collisionTollerance, collisionTollerance);


            result.Add(bgcollision);
            result.Add(hitReact);
            result.Add(dynamicCollision);
            result.Add(lifetime);
            
            AddStaticData(type, result, null);

            return result;
        }

        public GameObject SpawnParticleBurst(float positionX, float positionY)
        {
            //int type = (int)GeoManGameObjectTypes.Particle_Burst;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusTight;
            result.width = 16;
            result.height = 16;
            result.DestroyOnDeactivation = true;
            
            //FixedSizeArray<BaseObject> staticData = GetStaticData(type);
            //if (staticData == null)
            //{
            //    int staticObjectCount = 1;
            //    staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                
            //    staticData.Add(burst);
            //    SetStaticData(type, staticData);
            //}

            LaunchProjectileComponent burst = AllocateComponent<LaunchProjectileComponent>();
            burst.SetObjectTypeToSpawn((int)RoverGameGameObjectTypes.Particle);
            burst.SetLaunchSpeed(125);
            burst.SetDelayBetweenShots(0);//shoot all at once
            burst.SetShotsPerSet(FreshArchives.Random.Next(5,13));
            burst.SetThetaError(360);
            burst.SetSetsPerActivation(1);
            burst.SetOffsetX(result.width / 2);
            burst.SetOffsetY(result.height / 2);
            //burst.EnableProjectileTracking(100);

            LifetimeComponent lifetime = AllocateComponent<LifetimeComponent>();
            lifetime.SetTimeUntilDeath(0.1f);

            result.Add(burst);
            result.Add(lifetime);

            //AddStaticData(type, result, null);

            return result;
        }

        public GameObject SpawnParticle(float positionX, float positionY)
        {
            int type = (int)RoverGameGameObjectTypes.Particle;
            GameObject result = mGameObjectPool.Allocate();
            result.SetPosition(positionX, positionY);
            result.ActivationRadius = mActivationRadiusTight;
            result.width = 2;
            result.height = 2;
            result.DestroyOnDeactivation = true;

            FixedSizeArray<BaseObject> staticData = GetStaticData(type);
            if (staticData == null)
            {
                int staticObjectCount = 4;
                staticData = new FixedSizeArray<BaseObject>(staticObjectCount);

                //FreshGameComponent gravity = AllocateComponent<GravityComponent>();
                FreshGameComponent movement = AllocateComponent<MovementComponent>();

                //SimplePhysicsComponent physics = AllocateComponent<SimplePhysicsComponent>();
                //physics.setBounciness(0.5f);

                PhysicsComponent physics = AllocateComponent<PhysicsComponent>();
                physics.Bounciness = 0.5f;

                DrawableTexture2D textureDrawable = new DrawableTexture2D(
                        FreshArchives.GetWhiteSquare(sSystemRegistry.Game.GraphicsDevice),
                        (int)result.width,
                        (int)result.height);


                RenderComponent render = AllocateComponent<RenderComponent>();
                render.Priority = SortConstants.FOREGROUND_EFFECT;
                render.setDrawable(textureDrawable);

                staticData.Add(render);
                staticData.Add(movement);
                //staticData.Add(gravity);
                staticData.Add(physics);
                SetStaticData(type, staticData);
            }

            LifetimeComponent lifetime = AllocateComponent<LifetimeComponent>();
            //lifetime.setTimeUntilDeath((float)(FreshArchives.Random.NextDouble()*5+3));
            lifetime.SetTimeUntilDeath(0.25f + 0.75f*(float)FreshArchives.Random.NextDouble());

            BackgroundCollisionComponent bgcollision = AllocateComponent<BackgroundCollisionComponent>();
            bgcollision.SetSize((int)result.width, (int)result.height);


            result.Add(lifetime);
            result.Add(bgcollision);

            AddStaticData(type, result, null);

            return result;
        }

        public SolidSurfaceComponent GenerateRectangleSolidSurfaceComponent(float width, float height, bool container)
        {
            SolidSurfaceComponent solidSurface = AllocateComponent <SolidSurfaceComponent>();
            solidSurface.Inititalize(4);
        
            int normalFlip;
            if(container) normalFlip = 1;
            else normalFlip = -1;
        
            // box shape:
            // ___       ___1
            // | |      2| |3
            // ---       ---4
            Vector2 surface1Start = new Vector2(0, height);
            Vector2 surface1End = new Vector2(width, height);
            Vector2 surface1Normal = new Vector2(0.0f, normalFlip*-1.0f);

            Vector2 surface2Start = new Vector2(0, height);
            Vector2 surface2End = new Vector2(0, 0);
            Vector2 surface2Normal = new Vector2(normalFlip*1.0f, 0.0f);

            Vector2 surface3Start = new Vector2(width, height);
            Vector2 surface3End = new Vector2(width, 0);
            Vector2 surface3Normal = new Vector2(normalFlip*-1.0f, 0);

            Vector2 surface4Start = new Vector2(0, 0);
            Vector2 surface4End = new Vector2(width, 0);
            Vector2 surface4Normal = new Vector2(0, normalFlip*1.0f);

            solidSurface.AddSurface(surface1Start, surface1End, surface1Normal);
            solidSurface.AddSurface(surface2Start, surface2End, surface2Normal);
            solidSurface.AddSurface(surface3Start, surface3End, surface3Normal);
            solidSurface.AddSurface(surface4Start, surface4End, surface4Normal);
         
            return solidSurface;
        }
    }
    public enum RoverGameGameObjectTypes
    {
        //these first values must match those in the generic GameObjectTypes
        Invalid = -1,
        Tile_Blank = 0,
        Tile_Blocked = 1,
        Player = 2,
        
        Tile_BlockedTopLeft,
        Tile_BlockedTopRight,
        Tile_BlockedBottomLeft,
        Tile_BlockedBottomRight,

        Background_Plate,
        Structure_RepairBay,
        Tree,
        Resource_Emerald,

        Collectable,
        Gem_Emerald,
        Coin_Gold,
        Coin_Silver,
        WinObject,

        Breackable_Block_Piece,

        Ghost_Generic,
        Ghost_Eyes,
        Ghost_Red,
        Ghost_Pink,
        Ghost_Blue,
        Ghost_Orange,
        Ghost_Random,

        Guard,
        Ninja,
        Shuriken,
        Zombie,
        ZombieSpawner,

        Gun_Projectile,
        Particle,
        Particle_Burst,
        
        Mario_Tile_Brick,
        Mario_Tile_Brick_Ground,
        Mario_Tile_Block,
        Mario_Tile_Mystery_Block,
        Mario_Tile_Mystery_Block_Empty,
        Mario_Tile_Flagpole,
        Mario_Tile_Flagpole_Top,
        Mario_Tile_Sky,
        Mario_Tile_Pipe_BottomLeft,
        Mario_Tile_Pipe_BottomRight,
        Mario_Tile_Pipe_TopLeft,
        Mario_Tile_Pipe_TopRight,
    }
    public enum Animations
    {
        Attack,
        Idle,
        Fear,
        Move,
        Dead_Move,
    }
    public class RoverGameTile : Tile
    {
        /*Ideas
         * 
         * spikes - instant death
         * pressure button
         * push button
         * doors
         * teleport
         * keys
         * locks
         * visible timer
         * 
         * 
         * hidden blocks
         * fake blocks - dissapear on press
         * water & flippers
         * fire & fire boots
         * ice & skates
         * suction cub shoes - treadmill tiles
         * theif
         * 
         * */
        new public enum Types
        {
            //first set in mandatory to match the core tiles
            Invalid,
            Blank,
            Blocked,
            PlayerSpawn,

            Collectable,
            WinObject,
            BreakableBlock,
            RandomGhost,

            Guard,
            Ninja,
            Zombie,
            ZombieSpawnPoint,

            BlockedTopLeft,
            BlockedTopRight,
            BlockedBottomLeft,
            BlockedBottomRight,

            Mario_Tile_Brick,
            Mario_Tile_Brick_Ground,
            Mario_Tile_Block,
            Mario_Tile_Mystery_Block,
            Mario_Tile_Mystery_Block_Empty,
            Mario_Tile_Flagpole,
            Mario_Tile_Flagpole_Top,
            Mario_Tile_Sky,
            Mario_Tile_Pipe_BottomLeft,
            Mario_Tile_Pipe_BottomRight,
            Mario_Tile_Pipe_TopLeft,
            Mario_Tile_Pipe_TopRight,
        }

        static RoverGameTile()
        {
            string[] names = Enum.GetNames(typeof(Types));
            BuildLookUpArrays(names);


            //Define corralation between enum -> type idex -> SaveString/Collision Index/Image/ object

            int index = (int)Types.Invalid;
            saveStringLookup[index] = " ";//shouldn't match anthing
            collisionLookup[index] = (int)CollisionTileIndex.None;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Invalid;

            //blank Tile
            index = (int)Types.Blank;
            saveStringLookup[index] = "!";
            collisionLookup[index] = (int)CollisionTileIndex.None;//TODO update these images and collision implications
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Tile_Blank;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Invalid;

            index = (int)Types.Blocked;
            saveStringLookup[index] = "W";
            collisionLookup[index] = (int)CollisionTileIndex.Square;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Tile_Blocked;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Invalid;

            //player(s)
            index = (int)Types.PlayerSpawn;
            saveStringLookup[index] = "P";
            collisionLookup[index] = (int)CollisionTileIndex.None;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Player;

            //Collectable(s)
            index = (int)Types.Collectable;
            saveStringLookup[index] = "C";
            collisionLookup[index] = (int)CollisionTileIndex.None;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Collectable;
            index = (int)Types.WinObject;
            saveStringLookup[index] = "Q";
            collisionLookup[index] = (int)CollisionTileIndex.None;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            objectLookup[index] = (int)RoverGameGameObjectTypes.WinObject;

            //Block
            index = (int)Types.BreakableBlock;
            saveStringLookup[index] = "B";
            collisionLookup[index] = (int)CollisionTileIndex.None;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Breackable_Block_Piece;

            //Enemy(s)
            index = (int)Types.RandomGhost;
            saveStringLookup[index] = "E";
            collisionLookup[index] = (int)CollisionTileIndex.None;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Ghost_Random;
            //Guard
            index = (int)Types.Guard;
            saveStringLookup[index] = "`";
            collisionLookup[index] = (int)CollisionTileIndex.None;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Guard;
            //Ninja
            index = (int)Types.Ninja;
            saveStringLookup[index] = "T";
            collisionLookup[index] = (int)CollisionTileIndex.None;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Ninja;
            //Zombie
            index = (int)Types.Zombie;
            saveStringLookup[index] = "R";
            collisionLookup[index] = (int)CollisionTileIndex.None;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Zombie;
            //Zombie Spawn
            index = (int)Types.ZombieSpawnPoint;
            saveStringLookup[index] = "U";
            collisionLookup[index] = (int)CollisionTileIndex.None;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            objectLookup[index] = (int)RoverGameGameObjectTypes.ZombieSpawner;

            //BlockedCorners(s)
            index = (int)Types.BlockedTopLeft;
            saveStringLookup[index] = "@";
            collisionLookup[index] = (int)CollisionTileIndex.Triangle_TopLeft;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Tile_BlockedTopLeft;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            index = (int)Types.BlockedTopRight;
            saveStringLookup[index] = "#";
            collisionLookup[index] = (int)CollisionTileIndex.Triangle_TopRight;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Tile_BlockedTopRight;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            index = (int)Types.BlockedBottomLeft;
            saveStringLookup[index] = "$";
            collisionLookup[index] = (int)CollisionTileIndex.Triangle_BottomLeft;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Tile_BlockedBottomLeft;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            index = (int)Types.BlockedBottomRight;
            saveStringLookup[index] = "%";
            collisionLookup[index] = (int)CollisionTileIndex.Triangle_BottomRight;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Tile_BlockedBottomRight;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Invalid;




            //Mario
            index = (int)Types.Mario_Tile_Brick_Ground;
            saveStringLookup[index] = "G";
            collisionLookup[index] = (int)CollisionTileIndex.Square;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Mario_Tile_Brick_Ground;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            index = (int)Types.Mario_Tile_Block;
            saveStringLookup[index] = "0";
            collisionLookup[index] = (int)CollisionTileIndex.Square;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Mario_Tile_Block;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            index = (int)Types.Mario_Tile_Brick;
            saveStringLookup[index] = "1";
            collisionLookup[index] = (int)CollisionTileIndex.Square;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Mario_Tile_Brick;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            index = (int)Types.Mario_Tile_Mystery_Block;
            saveStringLookup[index] = "2";
            collisionLookup[index] = (int)CollisionTileIndex.Square;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Mario_Tile_Mystery_Block;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            index = (int)Types.Mario_Tile_Mystery_Block_Empty;
            saveStringLookup[index] = "3";
            collisionLookup[index] = (int)CollisionTileIndex.Square;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Mario_Tile_Mystery_Block_Empty;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            index = (int)Types.Mario_Tile_Flagpole;
            saveStringLookup[index] = "4";
            collisionLookup[index] = (int)CollisionTileIndex.None;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Mario_Tile_Flagpole;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            index = (int)Types.Mario_Tile_Flagpole_Top;
            saveStringLookup[index] = "5";
            collisionLookup[index] = (int)CollisionTileIndex.None;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Mario_Tile_Flagpole_Top;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            index = (int)Types.Mario_Tile_Sky;
            saveStringLookup[index] = "6";
            collisionLookup[index] = (int)CollisionTileIndex.None;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Mario_Tile_Sky;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            index = (int)Types.Mario_Tile_Pipe_BottomLeft;
            saveStringLookup[index] = "7";
            collisionLookup[index] = (int)CollisionTileIndex.Square;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Mario_Tile_Pipe_BottomLeft;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            index = (int)Types.Mario_Tile_Pipe_BottomRight;
            saveStringLookup[index] = "8";
            collisionLookup[index] = (int)CollisionTileIndex.Square;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Mario_Tile_Pipe_BottomRight;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            index = (int)Types.Mario_Tile_Pipe_TopLeft;
            saveStringLookup[index] = "9";
            collisionLookup[index] = (int)CollisionTileIndex.Square;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Mario_Tile_Pipe_TopLeft;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Invalid;
            index = (int)Types.Mario_Tile_Pipe_TopRight;
            saveStringLookup[index] = "-";
            collisionLookup[index] = (int)CollisionTileIndex.Square;
            tileIndexLookup[index] = (int)RoverGameGameObjectTypes.Mario_Tile_Pipe_TopRight;
            objectLookup[index] = (int)RoverGameGameObjectTypes.Invalid;

            SanityCheck();
        }

        public RoverGameTile(int type)
            : base(type)
        {

        }

        public override Tile SaveStringToType(string input)
        {
            return new RoverGameTile(SaveStringToTypeIndex(input));
        }
    }
}
