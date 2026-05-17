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
            }
            
        }
    }
}
