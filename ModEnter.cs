using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley.GameData;
using StardewValley.Objects.Trinkets;
using StardewValley;
using HarmonyLib;
using Microsoft.Xna.Framework;
using GoldenglowTrinket.NB.NormalBeaconTAS;
using GoldenglowTrinket.NB.NormalBeaconProjectile;
using StardewValley.Monsters;

namespace GoldenglowTrinket
{
    public class ModEntry : Mod
    {
        public static IModHelper StaticHelper;

        public class HitEffectMessage
        {
            public string EffectType { get; set; } // "explosion" 或 "special"
            public float X { get; set; }           // 特效X坐标
            public float Y { get; set; }           // 特效Y坐标
            public string LocationName { get; set; }
        }
        public class SelfEffectMessage
        {
            public string EffectType { get; set; } // "explosion" 或 "special"
            public float X { get; set; }           // 特效X坐标
            public float Y { get; set; }           // 特效Y坐标
            public string LocationName { get; set; }
        }
        public class FireballSyncMessage
        {
            public string LocationName { get; set; }
            public float PosX { get; set; }
            public float PosY { get; set; }
            public float VelX { get; set; }
            public float VelY { get; set; }
            public float TargetX { get; set; }
            public float TargetY { get; set; }
            public int Damage { get; set; }
        }

        public override void Entry(IModHelper helper)
        {
            StaticHelper = helper;
            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
        }
        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.Type == "GoldenglowTrinket/PlayHitEffect")
            {
                // 你原来的特效同步代码，不用动
                HitEffectMessage msg = e.ReadAs<HitEffectMessage>();
                Vector2 position = new Vector2(msg.X, msg.Y);
                GameLocation location = Game1.currentLocation;
                if (location == null || location.Name != msg.LocationName)
                    return;

                var hitEffect = new NBHitEffect();
                if (msg.EffectType == "explosion")
                {
                    hitEffect.CreateExplosion(location, position);
                }
                else if (msg.EffectType == "special")
                {
                    hitEffect.CreateSpecialExplosion(location, position);
                }
            }else if(e.Type == "GoldenglowTrinket/PlaySelfEffect")
            {
                SelfEffectMessage msg = e.ReadAs<SelfEffectMessage>();
                Vector2 position = new Vector2(msg.X, msg.Y);
                GameLocation location = Game1.currentLocation;
                if (location == null || location.Name != msg.LocationName)
                    return;

                var SelfEffect = new NBSelfEffect();
                if (msg.EffectType == "AddDisappearEffect")
                {
                    SelfEffect.AddDisappearEffect(location, position);
                }
                else if (msg.EffectType == "AddAppearEffect")//AddCasualParticles
                {
                    SelfEffect.AddAppearEffect(location, position);
                }
                else if (msg.EffectType == "AddCasualParticles")//AddCasualParticles
                {
                    SelfEffect.AddCasualParticles(location, position);
                }
            }
            else if (e.Type == "GoldenglowTrinket/SyncFireball")
            {
                // 只有主机才处理，别人不管
                //if (!Game1.IsMasterGame)
                //    return;

                // 读消息，和你原来的特效读消息的写法完全一样
                FireballSyncMessage msg = e.ReadAs<FireballSyncMessage>();

                // 找地图，和你原来的特效找地图的写法完全一样
                GameLocation location = Game1.currentLocation;
                if (location == null || location.Name != msg.LocationName)
                    return;

                // 找发射的玩家
                Farmer fromPlayer = Game1.getFarmer(e.FromPlayerID);
                if (fromPlayer == null)
                    return;

                // 找目标怪物，用位置找，和你原来的逻辑一样
                Monster target = null;
                float minDist = 9999;
                foreach (var npc in location.characters)
                {
                    if (npc is Monster m)
                    {
                        float dist = Vector2.Distance(m.Position, new Vector2(msg.TargetX, msg.TargetY));
                        if (dist < minDist)
                        {
                            minDist = dist;
                            target = m;
                        }
                    }
                }

                // 主机创建子弹，同步给所有人
                NBProjectile fireball = new NBProjectile(
                    actualDamage: msg.Damage,
                    spriteIndex: 47,
                    bouncesTillDestruct: 0,
                    tailLength: 2,
                    rotationVelocity: 0f,
                    xVelocity: msg.VelX,
                    yVelocity: msg.VelY,
                    startingPosition: new Vector2(msg.PosX, msg.PosY),
                    collisionSound: null,
                    location: location,
                    firer: fromPlayer,
                    target: target,
                    trackStrength: 0.4f,
                    maxTrackSpeed: 12f,
                    collisionBehavior: null
                );

                // 主机加进去，自动同步给所有人
                location.projectiles.Add(fireball);
            }

        }
        
    }
}
