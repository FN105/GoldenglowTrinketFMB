using StardewValley.Monsters;
using StardewValley;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewValley.BellsAndWhistles;
using StardewValley.TokenizableStrings;
using StardewValley.TerrainFeatures;
using static StardewValley.Minigames.TargetGame;
using Netcode;

namespace GoldenglowTrinket.NB.NormalBeaconProjectile
{
    public class NBProjectile : BasicProjectile
    {
        private Character _firer;
        private Monster _target;
        private float trackStrength = 0.5f;//插值比例
        private float maxTrackSpeed = 10f;//最大速度
        private int _actualDamage;
        private Farmer _ownerFarmer;
        Random random = new Random();
        private float TrackParticlesTimer;
        private Vector2 lastBulletPos;
        private float stayTimer;
        private readonly float stayRange = 16f;      // 判定"没动"的范围（像素）
        private bool isFreeFlying = false;

        public NBProjectile() : base() //无参数构造函数，供网络反序列化使用
        {
        }

        public NBProjectile(
            int actualDamage,
            int spriteIndex,
            int bouncesTillDestruct,
            int tailLength,
            float rotationVelocity,
            float xVelocity,
            float yVelocity,
            Vector2 startingPosition,
            string collisionSound,
            GameLocation location,
            Character firer,
            Monster target = null,
            float trackStrength = 0.1f,
            float maxTrackSpeed = 10f,
            onCollisionBehavior collisionBehavior = null
            )
            : base(
                damageToFarmer: 0,
                spriteIndex: spriteIndex,
                bouncesTillDestruct: bouncesTillDestruct,
                tailLength: tailLength,
                rotationVelocity: rotationVelocity,
                xVelocity: xVelocity,
                yVelocity: yVelocity,
                startingPosition: startingPosition,
                collisionSound: collisionSound,
                bounceSound: null,
                firingSound: null,
                explode: false,
                damagesMonsters: true, //
                location: location,
                firer: firer,
                collisionBehavior: collisionBehavior) 
        {
            this._actualDamage = actualDamage;
            this._firer = firer;
            this._target = target;
            this.trackStrength = trackStrength;
            this.maxTrackSpeed = maxTrackSpeed;
            this.lastBulletPos = startingPosition;
            if (firer is Farmer farmer)
            {
                this._ownerFarmer = farmer;
            }

        }
        private bool HasEnhancementUnit(string itemId)
        {
            if (_ownerFarmer == null)
                return false;

            foreach (var item in _ownerFarmer.Items)
            {
                // 物品不为空且物品ID匹配时返回true
                if (item != null && item.QualifiedItemId == $"(O){itemId}")
                {
                    return true;
                }
            }

            return false;
        }
        private (int addedPrecision, float critChance) GetEnhancedAC()
        {
            int addedPrecision = 0;
            float critChance = 0f;

            if (HasEnhancementUnit("GoldenglowBeaconEnhancementUnit2")|| HasEnhancementUnit("GoldenglowBeaconEnhancementUnit3"))
            {
                addedPrecision += 100;
                critChance += 0.1f;
            }

            return (addedPrecision, critChance); 
        }
        public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
        {
            if (Game1.IsMasterGame)
            {
                if (collisionBehavior != null)
                    collisionBehavior(location, (int)position.Value.X, (int)position.Value.Y, null);
                //return;
                if (collisionBehavior != null) //客机端命中特效
                    collisionBehavior(location, (int)position.Value.X, (int)position.Value.Y, null);
            //    location.explode(
            //    new Vector2(position.Value.X / 64, position.Value.Y / 64),
            //    1,
            //    Game1.player,
            //    false,
            //    _actualDamage,
            //    true
            //);
            //    destroyMe = true;
            //    piercesLeft.Value = 0;

                //return;
            }

            if (!damagesMonsters.Value)
            {
                return;
            }
            if (n is Monster monster)
            {
                if (collisionBehavior != null)
                {
                    // 传递子弹当前位置作为碰撞坐标
                    collisionBehavior?.Invoke(location, getBoundingBox().Center.X, getBoundingBox().Center.Y, GetPlayerWhoFiredMe(location));
                }
                var enhancements = GetEnhancedAC();
                int addedPrecision = enhancements.addedPrecision;
                float critChance = 0;
                critChance += enhancements.critChance;
                // 直接造成伤害，不传递击退
                Farmer who = _ownerFarmer ?? Game1.player;
                location.damageMonster(
                monster.GetBoundingBox(),
                _actualDamage,
                _actualDamage + 1,
                isBomb: false,
                knockBackModifier: 1f,//击退
                addedPrecision: addedPrecision,
                critChance: critChance,//暴击率
                critMultiplier: 1.5f,//暴击倍率
                triggerMonsterInvincibleTimer: false,
                who,
                isProjectile: true
                );
                    piercesLeft.Value = 0;
                    this.destroyMe = true;
                //Game1.addHUDMessage(new HUDMessage("111"+ _actualDamage));
                return;
            }

        }
        public override void behaviorOnCollisionWithTerrainFeature(TerrainFeature t, Vector2 tileLocation, GameLocation location)
        {
            return;
        }

        public override bool update(GameTime time, GameLocation location)
        {
            //if (destroyMe)
            //return base.update(time, location);

            float deltaMs = (float)time.ElapsedGameTime.TotalMilliseconds;
            

            if (Game1.IsMasterGame) // 只有主机改速度/目标
            //if()
            {
                float posDis = Vector2.Distance(position.Value, lastBulletPos);
                if (posDis < stayRange)
                {
                    stayTimer += deltaMs;

                    // 停留超过一定时间后飞出去
                    if (stayTimer >= 50f)
                    {
                        //清空旧目标，不再追尸体
                        _target = null;

                        //Monster newTarget = Utility.findClosestMonsterWithinRange(
                        //    location,
                        //    this.position.Value,
                        //    200, // 搜索范围
                        //    ignoreUntargetables: true
                        //);
                        //_target.Value = newTarget;

                        //顺着当前子弹的朝向，给一个向前的速度
                        float angle = rotation - MathHelper.PiOver2; // 还原飞行角度
                        //xVelocity.Value = MathHelper.Lerp(xVelocity.Value, maxTrackSpeed, trackStrength);
                        //yVelocity.Value = MathHelper.Lerp(yVelocity.Value, maxTrackSpeed, trackStrength);
                        xVelocity.Value = (float)Math.Cos(angle) * maxTrackSpeed;
                        yVelocity.Value = (float)Math.Sin(angle) * maxTrackSpeed;

                        //重置计时，防止重复触发
                        stayTimer = 0f;
                        lastBulletPos = position.Value;
                        isFreeFlying = true;
                    }
                    else
                    {
                        isFreeFlying = false;
                    }
                }
                else
                {
                    // 正常移动，重置计时
                    stayTimer = 0f;
                    lastBulletPos = position.Value;
                }
            }

            TrackParticlesTimer += time.ElapsedGameTime.Milliseconds;
            if (TrackParticlesTimer >= 30f)
            {
                TrackParticlesTimer = 0;
                TrackParticles(location);
            }

            if (Game1.IsMasterGame)
            {
                // 跟踪逻辑
                if (_target != null
                    && _target.currentLocation == location
                    && _target.Health > 0)
                {
                    // 计算朝向目标的方向
                    Vector2 direction = _target.Position - this.position.Value;
                    if (direction != Vector2.Zero)
                    {
                        direction.Normalize();

                        // 应用跟踪力(插值)
                        xVelocity.Value = MathHelper.Lerp(xVelocity.Value, direction.X * maxTrackSpeed, trackStrength);//1当前速度 2目标速度，3插值比例
                        yVelocity.Value = MathHelper.Lerp(yVelocity.Value, direction.Y * maxTrackSpeed, trackStrength);

                        // 限制最大速度
                        float currentSpeed = (float)Math.Sqrt(xVelocity.Value * xVelocity.Value + yVelocity.Value * yVelocity.Value);
                        if (currentSpeed > maxTrackSpeed)
                        {
                            xVelocity.Value = xVelocity.Value / currentSpeed * maxTrackSpeed;
                            yVelocity.Value = yVelocity.Value / currentSpeed * maxTrackSpeed;
                        }
                        //限最小速
                        if (currentSpeed < 1f)
                        {
                            xVelocity.Value = direction.X * 1f;
                            yVelocity.Value = direction.Y * 1f;
                        }
                        // 更新旋转角度，使子弹朝向目标
                        rotation = (float)Math.Atan2(yVelocity.Value, xVelocity.Value) + MathHelper.PiOver2;
                        //Game1.addHUDMessage(new HUDMessage("怪" + _target));
                        //Game1.addHUDMessage(new HUDMessage("怪" + _target.Health));
                    }
                }
                else/* if (_target == null || _target.Health <= 0)*/
                {
                   //Game1.addHUDMessage(new HUDMessage("没怪"));
                    //Monster oldTarget = _target;
                    // 寻找新目标
                    FindNewTarget(location);
                    //if (_target == oldTarget)
                    //{
                    //    // 目标没变，跳过重复的 rotation 更新
                    //}
                    // 确保旋转角度与当前运动方向一致

                    rotation = (float)Math.Atan2(yVelocity.Value, xVelocity.Value) + MathHelper.PiOver2;

                }
            }
            else
            {
                rotation = (float)Math.Atan2(yVelocity.Value, xVelocity.Value) + MathHelper.PiOver2;
            }

            return base.update(time, location);
        }

        private void FindNewTarget(GameLocation location)
        {
            //if (!Game1.IsMasterGame)
            //    return;

            Monster newTarget = Utility.findClosestMonsterWithinRange(
                location,
                this.position.Value,
                200, // 搜索范围
                ignoreUntargetables: true
            );

            if (newTarget != null)
            {
                _target = newTarget;
                
            }
            else
            {
                // 只在主机端减少穿透次数，客机跟随主机同步
                piercesLeft.Value = 0;
                destroyMe = true;

            }
        }
        private void TrackParticles(GameLocation location)
        {
            Vector2 Dianhu = new Vector2(0, 0);
            
            Vector2 _bulletFlyDir = new Vector2(0, -1); //修改左右飞行偏移
            Vector2 smallOffset = _bulletFlyDir * -20f; // 

            Vector2 _bulletFlyDir1 = new Vector2(-1, 0); // 修改上下飞行偏移
            Vector2 smallOffset1 = _bulletFlyDir1 * -30f; // 乘倍数
            Vector2 particlePosition = this.position.Value+ Dianhu+ smallOffset+ smallOffset1;
            Random rand = new Random();
            int randomNumber = rand.Next(0, 3);

            string textureName = randomNumber switch
            {
                0 => "Mods/Goldenglow_PinkLightBall",
                1 => "Mods/Goldenglow_BlueLightBall",
                _ => "Mods/Goldenglow_SmallGreenShine"
            };

            for (int i = 0; i < rand.Next(1, 3); i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);
                float speed = (float)(rand.NextDouble() * 0.9 + 0.3);
                float rotationChange = (float)(0.03 + rand.NextDouble() * 0.05);
                float scale = (float)(0.2 + rand.NextDouble() * 0.3);

                var particle = new TemporaryAnimatedSprite(
                    textureName: textureName,
                    sourceRect: new Rectangle(0, 0, 32, 32),
                    animationInterval: 1000f,
                    animationLength: 1,
                    numberOfLoops: 1,
                    position: particlePosition,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.015f,
                    color: Color.White,
                    scale: scale,
                    scaleChange: 0f,
                    rotation: 0f,
                    rotationChange: rotationChange
                );

                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );

                location.temporarySprites.Add(particle);
            }



        }
    }
}
