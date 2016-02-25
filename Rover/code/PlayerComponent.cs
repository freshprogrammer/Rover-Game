using Archives;
using Microsoft.Xna.Framework;
using System;

namespace RoverGame
{
    public class PlayerComponent : FreshGameComponent
    {
        private float playerMoveSpeed = 350f;

        private double distanceTraveled;
        private Vector2 lastLocation;

        public double DistanceTraveled { get { return distanceTraveled/32; } }//XXX- change this to reference the level tile size

        private InventoryComponent inventory;
        private SpriteComponent sprite;

        public PlayerComponent()
        {
            Reset();
        }

        public override void Reset()
        {
            inventory = null;
            distanceTraveled = 0;
            lastLocation = Vector2.Zero;
        }

        public override void Update(float delta, BaseObject parent)
        {
            GameObject parentObject = (GameObject)parent;
            Vector2 currentLoc = parentObject.GetPosition();

            if (!lastLocation.Equals(Vector2.Zero))
            {
                Vector2 playerLocationDelta = currentLoc - lastLocation;
                distanceTraveled += playerLocationDelta.Length();
            }
            lastLocation = currentLoc;

            //Point tileLoc = BaseObject.sSystemRegistry.LevelSystem.CurrentLevel.GetTileLoc(parentObject.GetCenteredPositionX(),parentObject.GetCenteredPositionY());

            if (parentObject.lastReceivedHitType != CollisionParameters.HitType.Invalid)
            {
                BaseObject.sSystemRegistry.VibrationSystem.Vibrate(PlayerIndex.One, 0.4f, 0.4f, 0.5f);
            }

            if (parentObject.life <= 0)
            {
                GotoDead();
            }
            if (inventory != null)
            {
                //TODO make non generic versions of Player Component and Inventory component for game specific code
                InventoryRecord inv = (InventoryRecord)inventory.GetRecord();
                if (inv.winCount > 0)
                {
                    //reset to prevent multiple calls - should porbably handle this with a object state or something
                    inv.winCount = 0;
                    //won
                    GotoWin();
                }
            }

            if (PlayerController.Moving)
            {
                parentObject.SetCurrentAction(GameObject.ActionType.Move);
                Vector2 inputOffset = Vector2.Zero;
                inputOffset = PlayerController.MovementDir;
                //inputOffset.Normalize();
                inputOffset *= (playerMoveSpeed * delta);

                Vector2 newLoc = inputOffset + currentLoc;

                LevelSystem level = sSystemRegistry.LevelSystem;

                parentObject.SetPosition(newLoc);
            }
            else
            {
                parentObject.SetCurrentAction(GameObject.ActionType.Idle);
            }

            if (PlayerController.LookDir != Vector2.Zero)
            {
                if (PlayerController.MouseLook)
                {
                    //calc mouse location relative to player and adjust facing dir baced on that
                    //look dir should be the mouse location on the screen - not normalized
                    Vector2 mouseGameLocation = sSystemRegistry.CameraSystem.GetGameLocation(PlayerController.LookDir.X, PlayerController.LookDir.Y);
                    
                    parentObject.facingDirection.X = mouseGameLocation.X - parentObject.GetCenteredPositionX();
                    parentObject.facingDirection.Y = mouseGameLocation.Y - parentObject.GetCenteredPositionY();

                    FreshArchives.NormalizeDown(parentObject.facingDirection, 1);
                }
                else
                    parentObject.facingDirection = PlayerController.LookDir;
            }
            else if (PlayerController.Moving)
            {
                parentObject.facingDirection = PlayerController.MovementDir;
            }

            if (PlayerController.FirePressed)
            {
                parentObject.SetCurrentAction(GameObject.ActionType.Attack);
            }

            if(sprite!=null)
                Animate(parentObject);
        }

        private void Animate(GameObject parentObject)
        {
            switch (parentObject.GetCurrentAction())
            {
                default:
                case GameObject.ActionType.Idle:
                    sprite.PlayAnimation((int)Animations.Idle);
                    break;
                case GameObject.ActionType.Move:
                    sprite.PlayAnimation((int)Animations.Move);
                    break;
                case GameObject.ActionType.Attack:
                    sprite.PlayAnimation((int)Animations.Attack);
                    break;
            }
        }

        protected void GotoDead()
        {
            sSystemRegistry.LevelSystem.SendGameEvent(LevelSystem.GameEvent.Death);
            BaseObject.sSystemRegistry.VibrationSystem.Vibrate(PlayerIndex.One, 0.5f, 0.5f, 0.75f);
        }

        protected void GotoWin()
        {
            sSystemRegistry.LevelSystem.SendGameEvent(LevelSystem.GameEvent.Win);
            
            ////mState = State.WIN;
            //TimeSystem timeSystem = sSystemRegistry.TimeSystem;
            //timeSystem.AppyScale(0.1f, 8.0f, true);
        }

        public void SetInventory(InventoryComponent inv)
        {
            inventory = inv;
        }

        public void SetSpriteComponent(SpriteComponent spr)
        {
            sprite = spr;
        }

        public void SetMoveSpeed(int s)
        {
            playerMoveSpeed = s;
        }
    }
}
