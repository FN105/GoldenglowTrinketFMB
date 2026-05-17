using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading;
using GoldenglowTrinket.NB.NormalBeaconProjectile;
using GoldenglowTrinket.NB.NormalBeaconTAS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Companions;
using StardewValley.Extensions;
using StardewValley.Monsters;
using StardewValley.Objects.Trinkets;
using StardewValley.Projectiles;
using StardewValley.Tools;
using static System.Net.Mime.MediaTypeNames;
using static StardewValley.Minigames.TargetGame;
using static StardewValley.Monsters.Monster;

namespace GoldenglowTrinket
{
    //饰品随从
    public class NormalBeaconTrinket : TrinketEffect
    {
        private FireballCompanion _companion;//
        private FireballCompanion _companion2;
        private FireballCompanion _companion3;//
        private FireballCompanion _companion4;
        private bool www;

        private float _fireballDelay = 1600f;//攻速
        //private float _specialFireballDelay = 5000f;//攻速
        private int _minDamage = 140;
        private int _maxDamage = 180;
        private HashSet<string> _cachedIgnoreLocations;
        private HashSet<string> _cachedIgnoreMonsters;

        // 实例方法
        public HashSet<string> GetIgnoredLocations()
        {
            if (_cachedIgnoreLocations == null)
            {
                _cachedIgnoreLocations = new HashSet<string>(
                    ArgUtility.SplitQuoteAware(
                        // 通过实例属性 Trinket 访问
                        Trinket?.GetTrinketData()?.CustomFields?.GetValueOrDefault("IgnoreLocations"),
                        '/'
                    ),
                    StringComparer.OrdinalIgnoreCase
                );
            }
            return _cachedIgnoreLocations;
        }

        public HashSet<string> GetIgnoredMonsterNames()
        {
            if (_cachedIgnoreMonsters == null)
            {
                _cachedIgnoreMonsters = new HashSet<string>(
                    ArgUtility.SplitQuoteAware(
                        // 通过实例属性 Trinket 访问
                        Trinket?.GetTrinketData()?.CustomFields?.GetValueOrDefault("IgnoreMonsters"),
                        '/'
                    ),
                    StringComparer.OrdinalIgnoreCase
                );
            }
            return _cachedIgnoreMonsters;
        }
        private bool HasEnhancementUnit(Farmer farmer, string itemId)
        {
            if (farmer == null)
                return false;

            return farmer.Items.Any(item =>
                item != null && item.QualifiedItemId == $"(O){itemId}");
        }
        public NormalBeaconTrinket(Trinket trinket) : base(trinket)//
        {

        }


        public override void Update(Farmer farmer, GameTime time, GameLocation location)
        {
            base.Update(farmer, time, location);
            //如果没随从，那就生成，同时判断是否有特殊物品，否则不生成
            // 检查是否需要重新应用效果（当随从丢失或强化物品状态改变时）
            bool needsReapply = false;//是否生成

            //随从检查只在主机端执行
            if (farmer.IsLocalPlayer)
            {
                

                // 检查前三个随从是否存在
                if (_companion == null || !farmer.companions.Contains(_companion) ||
                    _companion2 == null || !farmer.companions.Contains(_companion2) ||
                    _companion3 == null || !farmer.companions.Contains(_companion3))
                {
                    needsReapply = true;
                }

                // 检查特殊物品在不在
                bool hasEnhancement = HasEnhancementUnit(farmer, "GoldenglowBeaconEnhancementUnit3");
                bool shouldHaveFourthCompanion = hasEnhancement && _companion4 != null;//有特殊物品和有第四个随从
                bool shouldNotHaveFourthCompanion = !hasEnhancement && _companion4 == null;//没特殊物品和没第四个随从

                if (!(shouldHaveFourthCompanion || shouldNotHaveFourthCompanion))//判断当前情况合不合预期，不和就重新生成
                {
                    needsReapply = true;
                }

                // 检查第四个随从状态（如果应该有但没有，或者不应该有但有）
                if (hasEnhancement && (_companion4 == null || !farmer.companions.Contains(_companion4)))
                {
                    needsReapply = true;
                }

                if (needsReapply)
                {
                    Apply(farmer);
                }
            }
        }
        public override void Apply(Farmer farmer)
        {
            Unapply(farmer);
            //_companion = CreateCompanion(farmer, new Vector2(-10f, 45f));
            //_companion2 = CreateCompanion(farmer, new Vector2(25f, 30f));
            //_companion3 = CreateCompanion(farmer, new Vector2(50f, 45f));

                _companion = CreateCompanion(farmer, new Vector2(-25f, 45f));
                _companion2 = CreateCompanion(farmer, new Vector2(0f, 30f));
                _companion3 = CreateCompanion(farmer, new Vector2(25f, 45f));

                if (HasEnhancementUnit(farmer, "GoldenglowBeaconEnhancementUnit3"))
                {
                    _companion4 = CreateCompanion(farmer, new Vector2(50f, 50f));
                    if (!farmer.companions.Contains(_companion4)) farmer.AddCompanion(_companion4);
                }
                else
                {
                    _companion4 = null; // 确保第四个随从为null
                }

                if (!farmer.companions.Contains(_companion)) farmer.AddCompanion(_companion);
                if (!farmer.companions.Contains(_companion2)) farmer.AddCompanion(_companion2);
                if (!farmer.companions.Contains(_companion3)) farmer.AddCompanion(_companion3);
        }

        private FireballCompanion CreateCompanion(Farmer farmer, Vector2 offset)
        {
            var companion = new FireballCompanion(
                parent: this,
                variant: 0,
                fireballDelay: 1300f,
                offset: offset
            );
            companion.InitializeCompanion(farmer);
            companion.Position = farmer.Position + offset;
            return companion;
        }

        public override void Unapply(Farmer farmer)
        {

                if (_companion != null)
                {
                    farmer.RemoveCompanion(_companion);
                    _companion.ResetAttackState(); 
                }
                if (_companion2 != null)
                {
                    farmer.RemoveCompanion(_companion2);
                    _companion2.ResetAttackState();
                }
                if (_companion3 != null)
                {
                    farmer.RemoveCompanion(_companion3);
                    _companion3.ResetAttackState();
                }
                if (_companion4 != null)
                {
                    farmer.RemoveCompanion(_companion4);
                    _companion4.ResetAttackState();
                }   
                
            _companion = _companion2 = _companion3 = _companion4 = null;

        }

        //随从类
        public class FireballCompanion : FlyingCompanion
        {

            private int _BaseDamage = 120;
            private int _ActualDamage = 0;
            private Vector2 _fireballOffset = new Vector2(-50f, -148f);//固定偏移（x-往左，+往右，y-）
            private float _hoverTimer;
            private readonly float _hoverSpeed = 3f;//浮动速度
            private readonly float _hoverAmplitude = 5f;//浮动大小
            private Vector2 _customOffset;
            private Vector2 _originalOffset;
            private Vector2 _attackPosition;  // 记录攻击时的位置
            private Vector2 _leavePosition;  // 记录离开前的位置
            private bool _TingLiu = false;
            private float _TingLiuTime;
            private float _PuGongCD;
            private bool _PuGong = false;
            float _animationTimer = 0f;
            float _animationTimer1 = 0f;
            private float _rotationAngle;
            private float _rotationAngle1;
            private bool _isRetreating = false;  // 是否正在后退
            private float _retreatProgress = 0f; // 缓冲进度
            private const float RetreatDuration = 600f; // 缓冲动画总时长（毫秒）
            private Vector2 _originalAttackPos; // 初始攻击位置
            private Vector2 _retreatOffset = new Vector2(0f, 20f); // 后退偏移量
            //private static int _companionCounter = 0;

            private bool _isSpecialAttacking = false;
            private Vector2 _dashTarget; // 冲刺目标坐标
            private bool BaoZhaCiShu = true;
            private bool XiaoShi = false;
            private float _swingTimer;      // 摇摆计时器
            private float SwingSpeed = 0.007f;  // 摇摆速度（值越大摆动越快）
            private float SwingAmplitude = 0.04f; // 摇摆幅度（弧度值）
            private float MonsterTime = 0f;      // 判断怪物是否在范围内计时器
            //自爆率
            private double TeShuGongJiGaiLv = 0.015f;

            private float _initialTimer;

            private float _dailyParticleTimer;      // 日常粒子计时器
            private Vector2 Offset1 = new Vector2(-5f, -190f);//修正弹道

            private float _targetRotation;    // 目标旋转角度
            private float _currentRotation;   // 当前实际角度
            private bool InEvent = false;
            private Vector2 _lastValidPosition;//随从位置
            private Vector2 _lastOwnerPosition;//玩家位置
            private bool _isRight = false;//随从在右边
            private float SuijiJiaoDu;
            private float SuijiDaXiao;
            private int DianHuZhen;//电弧帧
            private float _lightningAlpha;      // 当前透明度（0.0-1.0）
            private float _PinkAlphaTimer;          // 透明度计时器
            private bool _inSlimeHutch = false;
            private Monster target;
            private readonly NormalBeaconTrinket _parent;
            private float GongSu = 1300f;   // 信标总攻速
            private NBSelfEffect _nbSelfEffect;
            private NBHitEffect _nbHitEffect;
            private Monster _specialTarget; // 自爆攻击的目标怪物
            private float BaseMultiplier=0.2f; // 攻击初始倍率
            private Vector2 _lastPosition;
            private float AllAttackSpeed =0f;   // 信标总攻速
            public FireballCompanion() : base(0) // 无参数构造函数，供网络反序列化使用
            {
                _nbSelfEffect = new NBSelfEffect(this);
                _nbHitEffect = new NBHitEffect();
                _lastOwnerPosition = Vector2.Zero;

                //int index = _companionCounter++ % 3;
                //switch (index)
                //{
                //    case 0: _originalOffset = new Vector2(-25f, 45f); break;
                //    case 1: _originalOffset = new Vector2(0f, 30f); break;
                //    case 2: _originalOffset = new Vector2(25f, 45f); break;
                //}
                //_fireballOffset = _originalOffset;

                // 调用 InitializeCompanion 确保 FlyingCompanion 所有网络字段都初始化，防止联机反序列化时空引用崩溃
                if (Owner != null)
                    InitializeCompanion(Owner);
                    // 确保必要的网络字段有默认值
                    if (lerp < 0f) lerp = -1f;
                    if (direction.Value == 0) direction.Value = 2;
            }
            public FireballCompanion(NormalBeaconTrinket parent, int variant, float fireballDelay, Vector2 offset)
            : base(variant)
            {
                var hasLightField = typeof(FlyingCompanion).GetField("hasLight", BindingFlags.NonPublic | BindingFlags.Instance);
                if (hasLightField != null)
                {
                    hasLightField.SetValue(this, false);
                }
                _originalOffset = offset;
                _fireballOffset = offset;
                _initialTimer = Game1.random.Next(0, 6000);
                _swingTimer = _hoverTimer = _animationTimer = _initialTimer;
                _parent = parent;
                _nbSelfEffect = new NBSelfEffect(this);
                _nbHitEffect = new NBHitEffect();
                _lastOwnerPosition = Vector2.Zero;
            }

            private bool HasEnhancementUnit(string itemId)
            {
                if (Owner == null)
                    return false;
                
                foreach (var item in Owner.Items)
                {
                    // 物品不为空且物品ID匹配时返回true
                    if (item != null && item.QualifiedItemId == $"(O){itemId}")
                    {
                        return true;
                    }
                }

                return false;
            }
            private float GetEnhancedBaseDamage()
            {
                float BaseMultiplier = 0.2f; 

                
                if (HasEnhancementUnit("GoldenglowBeaconEnhancementUnit1")||HasEnhancementUnit("GoldenglowBeaconEnhancementUnit2") || HasEnhancementUnit("GoldenglowBeaconEnhancementUnit3"))
                {
                    BaseMultiplier += 0.15f; 
                }

                return BaseMultiplier;
            }
            private float GetTimeScale()
            {
                // 基础攻速
                float baseGongSu = 1300f;

                // 攻速加成百分比（比如7表示7%）
                AllAttackSpeed = 0f;

                // 最终攻速=基础攻速 / (1 + 加成百分比)
                GongSu = baseGongSu / (1f + AllAttackSpeed / 100f);

                return baseGongSu / GongSu;
            }
            public override void Update(GameTime time, GameLocation location)
            {
                if (Owner == null)
                return;

                if (Game1.eventUp)
                {
                    // 事件期间隐藏随从
                    InEvent = true;
                }
                else
                {
                    InEvent = false;
                }
                //重写
                if (IsLocal&&!_TingLiu && !_PuGong)
                {
                    if (lerp < 0f)
                    {

                        if ((OwnerPosition - Position).Length() > 80f)
                        {
                            startPosition = Position;
                            float radius = 0.33f;
                            endPosition = OwnerPosition + new Vector2(Utility.RandomFloat(-64f, 64f) * radius, Utility.RandomFloat(-64f, 64f) * radius);
                            if (location.isCollidingPosition(new Rectangle((int)endPosition.X - 8, (int)endPosition.Y - 8, 16, 16), Game1.viewport, isFarmer: false, 0, glider: false, null, pathfinding: true, projectile: false, ignoreCharacterRequirement: true))
                            {
                                endPosition = OwnerPosition;
                            }
                            lerp = 0f;
                            hopEvent.Fire(1f);
                            if (Math.Abs(OwnerPosition.X - Position.X) > 8f)
                            {
                                if (OwnerPosition.X > Position.X)
                                {
                                    direction.Value = 1;
                                }
                                else
                                {
                                    direction.Value = 3;
                                }
                            }
                        }
                    }
                    if (lerp >= 0f)
                    {
                        lerp += (float)time.ElapsedGameTime.TotalSeconds / 0.4f;
                        if (lerp > 1f) lerp = 1f;
                        Position = new Vector2(
                            Utility.Lerp(startPosition.X, endPosition.X, lerp),
                            Utility.Lerp(startPosition.Y, endPosition.Y, lerp)
                        );
                        if (lerp == 1f) lerp = -1f;
                    }
                }

                hopEvent.Poll();
                if (gravity != 0f || height != 0f)
                {
                    height += gravity;
                    gravity -= (float)time.ElapsedGameTime.TotalSeconds * 6f;
                    if (height <= 0f)
                    {
                        height = 0f;
                        gravity = 0f;
                    }
                }
                height = 32f;
                //base.Update(time, location);
                _nbSelfEffect.UpdateFromCompanion(_fireballOffset, _isRight);
                _BaseDamage =120;
                BaseMultiplier = GetEnhancedBaseDamage();
                _animationTimer += (float)time.ElapsedGameTime.TotalMilliseconds;
                _animationTimer1 += (float)time.ElapsedGameTime.TotalMilliseconds;
                if (_animationTimer1 >= 110f)
                {
                    SuijiJiaoDu = (float)(-0.3 + random.NextDouble() * 0.6);
                    SuijiDaXiao = (float)(random.NextDouble() + 1);
                    DianHuZhen = random.Next(0, 30) * 32;
                    _animationTimer1 = 0f; 
                }


                //悬浮
                _hoverTimer += (float)time.ElapsedGameTime.TotalSeconds;
                float verticalOffset = (float)Math.Sin(_hoverTimer * _hoverSpeed) * _hoverAmplitude;
                _customOffset = new Vector2(0, verticalOffset);


                //摆动
                _swingTimer += (float)time.ElapsedGameTime.TotalMilliseconds;

                // 周期摆动
                _rotationAngle1 = SwingAmplitude * (float)Math.Sin(_swingTimer * SwingSpeed);//左边弧度，右边速度

                GongSu = 1300f;//总攻速
                if (_ActualDamage == 0) _ActualDamage = (int)(_BaseDamage * BaseMultiplier);
                if (Game1.shouldTimePass())
                {
                    // 通过 _parent 访问实例方法
                    HashSet<string> ignoreLocations = _parent?.GetIgnoredLocations() ?? new HashSet<string>();
                    string locationName = location.NameOrUniqueName;

                    if (ignoreLocations.Contains(locationName) || ignoreLocations.Contains(location.Name))
                    {
                        _inSlimeHutch = true;
                    }

                    HashSet<string> ignoreMonsters = _parent?.GetIgnoredMonsterNames() ?? new HashSet<string>();
                    Monster target1 = Utility.findClosestMonsterWithinRange(
                        location,
                        Owner.Position,
                        1000,
                        ignoreUntargetables: true,
                        (Monster m) => !ignoreMonsters.Contains(m.Name)
                    );
                    target = target1;
                    float timeScale = GetTimeScale();//攻速
                    //Monster target = Utility.findClosestMonsterWithinRange(location, Owner.Position, 1000);
                    if (target != null && !_inSlimeHutch)
                    {

                        MonsterTime = 0f;
                        if (_TingLiu && !_isSpecialAttacking)
                        {
                            //缓冲动画
                            if (_isRetreating)
                            {
                                float deltaTime = (float)time.ElapsedGameTime.TotalSeconds;

                                // 计算总动画时间
                                float totalDuration = 0.6f;
                                float FadeInTime = 0.1f;   // 淡入时间（秒）
                                float StayTime = 0.3f;     // 保持时间（秒）
                                float FadeOutTime = 0.2f;  // 淡出时间（秒）
                                // 循环计时器
                                _PinkAlphaTimer = (_PinkAlphaTimer + deltaTime) % totalDuration;

                                // 分阶段计算透明度
                                if (_PinkAlphaTimer <= FadeInTime)
                                {
                                    // 淡入阶段 
                                    _lightningAlpha = _PinkAlphaTimer / FadeInTime;
                                }
                                else if (_PinkAlphaTimer <= FadeInTime + StayTime)
                                {
                                    // 保持阶段 
                                    _lightningAlpha = 1f;
                                }
                                else
                                {
                                    // 淡出阶段 
                                    float fadeOutProgress = (_PinkAlphaTimer - FadeInTime - StayTime) / FadeOutTime;
                                    _lightningAlpha = 1f - fadeOutProgress;
                                }
                                _retreatProgress += (float)time.ElapsedGameTime.TotalMilliseconds / RetreatDuration;


                                if (_retreatProgress <= 2f)
                                {
                                    // 后退阶段（0-1）：从初始位置向后退
                                    float t = _retreatProgress;
                                    Position = _originalAttackPos + _retreatOffset * (float)Math.Sin(t * Math.PI);
                                }
                            }
                            _TingLiuTime += (float)time.ElapsedGameTime.TotalMilliseconds;
                            //Position = _attackPosition;
                            if (_TingLiuTime > 600f / timeScale)
                            {
                                _TingLiu = false;
                                _TingLiuTime = 0f;
                                _PuGong = true; // 进入等待 0.8s 状态(如果_PuGong= true，那么饰品就会消失0.8s)
                                _nbSelfEffect.AddDisappearEffect(location, _attackPosition);
                                location.playSound("Goldenglow_BeaconFade");
                            }
                        }
                        //特殊自爆攻击
                        else if (_TingLiu && _isSpecialAttacking)
                        {
                            float deltaTime = (float)time.ElapsedGameTime.TotalSeconds;

                            // 计算总动画时间
                            float totalDuration = 0.6f;
                            float FadeInTime = 0.2f;   // 淡入时间（秒）
                            float StayTime = 0.2f;     // 保持时间（秒）
                            float FadeOutTime = 0.2f;  // 淡出时间（秒）
                                                       // 循环计时器
                            _PinkAlphaTimer = (_PinkAlphaTimer + deltaTime) % totalDuration;

                            // 分阶段计算透明度
                            if (_PinkAlphaTimer <= FadeInTime)
                            {
                                // 淡入阶段 
                                _lightningAlpha = _PinkAlphaTimer / FadeInTime;
                            }
                            else if (_PinkAlphaTimer <= FadeInTime + StayTime)
                            {
                                // 保持阶段 
                                _lightningAlpha = 1f;
                            }
                            else
                            {
                                // 淡出阶段 
                                float fadeOutProgress = (_PinkAlphaTimer - FadeInTime - StayTime) / FadeOutTime;
                                _lightningAlpha = 1f - fadeOutProgress;
                            }
                            //缓冲动画
                            _TingLiuTime += (float)time.ElapsedGameTime.TotalMilliseconds;

                            if (_TingLiuTime >= 0f && _TingLiuTime < 300f / timeScale)
                            {
                                if (_isRetreating)
                                {
                                    _retreatProgress += (float)time.ElapsedGameTime.TotalMilliseconds / RetreatDuration;

                                    // if (_retreatProgress <= 1f)
                                    //{
                                    // 后退阶段（0-1）：从初始位置向后退
                                    float t = _retreatProgress;
                                    Position = _originalAttackPos + _retreatOffset * (float)Math.Sin(t * Math.PI);
                                    //}

                                }

                            }
                            //冲刺
                            else if (_TingLiuTime >= 300f / timeScale)
                            {
                                // 获取当前目标位置
                                Vector2 currentTargetPos = (_specialTarget != null && _specialTarget.Health > 0)
                                    ? _specialTarget.Position + new Vector2(10f, 0f)  // 瞄准怪物中心偏上
                                    : _dashTarget + new Vector2(0f, -128f);  // 如果怪物死亡，使用最后记录的位置


                                Vector2 fireballStartPos = Position + _fireballOffset + Offset1;
                                Vector2 motion = Utility.getVelocityTowardPoint(fireballStartPos, currentTargetPos, 10f);

                                _rotationAngle = (float)Math.Atan2(motion.Y, motion.X) + MathHelper.PiOver2 - MathHelper.Pi;

                                //  检查信标和怪物位置是否接近重合
                                float positionDifference = Math.Abs(Position.X - currentTargetPos.X) + Math.Abs(Position.Y - currentTargetPos.Y-150f);
                                //Game1.addHUDMessage(new HUDMessage("信标"+ Position));
                                //Game1.addHUDMessage(new HUDMessage("怪" + currentTargetPos));
                                //Game1.addHUDMessage(new HUDMessage("positionDifference" + positionDifference));
                                // 如果位置接近重合，触发爆炸
                                if (positionDifference < 75f) // 容差范围
                                {
                                    if (BaoZhaCiShu)
                                    {
                                        Vector2 explosionTile = new Vector2(Position.X / 64, Position.Y / 64 - 2);
                                        Vector2 explosionCenter = new Vector2(Position.X + 32, Position.Y - 96);
                                        int damage = (int)(Game1.random.Next(_BaseDamage - (int)(_BaseDamage * 0.2), _BaseDamage + (int)(_BaseDamage * 0.2) + 1) * 3.75);
                                        _nbHitEffect.CreateSpecialExplosion(location, explosionCenter);
                                        location?.explode(
                                            explosionTile,
                                            1,
                                            Owner,
                                            damageFarmers: false,
                                            damage,
                                            !(location is Farm) && !(location is SlimeHutch)
                                        );
                                        location.playSound("Goldenglow_Boom");
                                        BaoZhaCiShu = false;
                                        XiaoShi = true;
                                    }
                                }
                                else
                                {
                                    // 跟踪移动 
                                    if (motion != Vector2.Zero)
                                    {
                                        motion.Normalize();
                                    }
                                    // 移动速度
                                    float moveSpeed = 15f + 15f * -(1-timeScale);
                                    moveSpeed = MathHelper.Clamp(moveSpeed, 7.5f, 30f);
                                    // 应用移动
                                    Position += motion * moveSpeed;
                                }
                                
                            }
                            //Position = _attackPosition;
                            if (_TingLiuTime > 600f / timeScale && !BaoZhaCiShu)
                            {
                                _TingLiu = false;
                                _TingLiuTime = 0f;
                                BaoZhaCiShu = true;
                                _PuGong = true; // 进入等待 0.7s 状态(如果_PuGong= true，那么饰品就会消失0.7s)
                                if (!XiaoShi)
                                {
                                    //_nbSelfEffect.AddDisappearEffect(location, _attackPosition);//消失
                                }

                                location.playSound("Goldenglow_BeaconFade");
                            }

                        }

                        //消失
                        else if (_PuGong)
                        {
                            _PuGongCD += (float)time.ElapsedGameTime.TotalMilliseconds;
                            if (_PuGongCD > 700f / timeScale)
                            {
                                _PuGongCD = 0f;
                                _PuGong = false;
                                XiaoShi = false;
                                _isSpecialAttacking = false;
                                BaoZhaCiShu = true;
                                if (TeShuGongJiGaiLv >= 0.3f)
                                {
                                    //Game1.addHUDMessage(new HUDMessage("保底"));
                                    TeShuGongJiGaiLv = 1f;
                                }
                                // 攻击
                                if (Game1.random.NextBool(TeShuGongJiGaiLv))
                                {
                                    TeShuGongJiGaiLv = 0.015f;
                                    _ActualDamage = (int)(_BaseDamage * BaseMultiplier);
                                    //_minDamage = 24;
                                    //_maxDamage = 32;
                                    //Game1.addHUDMessage(new HUDMessage("重置概率"));
                                    _TingLiu = true;
                                    _isSpecialAttacking = true;
                                    StartSpecialAttack(target, location);

                                }
                                else
                                {// 电流
                                    TeShuGongJiGaiLv += 0.015f;
                                    //Game1.addHUDMessage(new HUDMessage("下次攻击暴击的概率="+ TeShuGongJiGaiLv));
                                    _TingLiu = true;
                                    StartAttackSequence(target, location);
                                    //Game1.addHUDMessage(new HUDMessage("当前伤害：" + _ActualDamage));
                                    if (_ActualDamage < (int)(_BaseDamage * (1.1f+ BaseMultiplier-0.2f)))
                                    {
                                        _ActualDamage += (int)(_BaseDamage * 0.15);
                                        //_minDamage += 18;
                                        //_maxDamage += 24;
                                    }

                                }
                                _TingLiu = true; // 进入停留 0.5s 状态
                            }
                        }
                        //启动
                        else
                        {
                            if (Game1.random.NextBool(TeShuGongJiGaiLv))
                            {
                                TeShuGongJiGaiLv = 0.015f;
                                //Game1.addHUDMessage(new HUDMessage("重置概率"));
                                _TingLiu = true;
                                _isSpecialAttacking = true;
                                StartSpecialAttack(target, location);
                                _isRight = false;
                                _nbSelfEffect.AddDisappearEffect(location, _leavePosition);
                                _ActualDamage = (int)(_BaseDamage * BaseMultiplier);
                                //_minDamage = 24;
                                //_maxDamage = 32;
                            }
                            else
                            {// 电流
                                TeShuGongJiGaiLv += 0.015f;
                                //Game1.addHUDMessage(new HUDMessage("下次攻击暴击概率=" + TeShuGongJiGaiLv));
                                _TingLiu = true;
                                StartAttackSequence(target, location);
                                _isRight = false;
                                _nbSelfEffect.AddDisappearEffect(location, _leavePosition);
                                //Game1.addHUDMessage(new HUDMessage("当前伤害：" + _ActualDamage));
                                _ActualDamage += (int)(_BaseDamage * 0.15);
                                //_minDamage += 18;
                                //_maxDamage += 24;
                            }

                        }
                    }



                    //如果没有怪
                    else
                    { // 复位
                      //Position = Owner.Position + _originalOffset;
                        _inSlimeHutch = false;
                        if (!_TingLiu || !_PuGong)
                        {
                            MonsterTime += (float)time.ElapsedGameTime.TotalMilliseconds;
                            if (MonsterTime >= 3000)
                            {
                                TeShuGongJiGaiLv = 0.015f;
                                _ActualDamage = (int)(_BaseDamage * BaseMultiplier);
                                MonsterTime = 0f;
                                // Game1.addHUDMessage(new HUDMessage("脱战超过三秒，累加效果消失"));
                            }
                            if (!_TingLiu)
                            {
                                _rotationAngle = 0f;
                                //Game1.addHUDMessage(new HUDMessage("这是1"));
                            }
                            if (Owner != null)
                            {
                                //初始化
                                if (_lastOwnerPosition == Vector2.Zero)
                                {
                                    _lastOwnerPosition = Owner.Position;
                                }
   
                                Vector2 movement = Position - _lastPosition;
                                float moveDirection = Math.Sign(movement.X);

                                Vector2 ownerMovement = Owner.Position - _lastOwnerPosition;
                                float verticalMovement = ownerMovement.Y;
                                float horizontalMovement = ownerMovement.X;

                                bool isPlayerVerticalMoving = Math.Abs(verticalMovement) > 1f && Math.Abs(horizontalMovement) < 1f;
                                if (isPlayerVerticalMoving)
                                {

                                }
                                else
                                {
                                    if (moveDirection > 0) // 向右移动
                                    {
                                        _targetRotation = 0.17f; // 向左偏移
                                    }
                                    else if (moveDirection < 0) // 向左移动
                                    {
                                        _targetRotation = -0.17f;  // 向右偏移
                                    }
                                    else 
                                    {
                                        _targetRotation = 0f;
                                    }

                                    // 平滑插值当前角度
                                    _currentRotation = MathHelper.Lerp(_currentRotation, _targetRotation, 0.2f);
                                    // 记录当前位置供下一帧比较
                                    _lastPosition = Position;
                                }
                                // 更新最后记录位置
                                _lastValidPosition = Position;
                                _lastOwnerPosition = Owner.Position;
                            }
                        }
                        if (_TingLiu && !_isSpecialAttacking)
                        {

                            if (_isRetreating)
                            {
                                float deltaTime = (float)time.ElapsedGameTime.TotalSeconds;

                                // 计算总动画时间
                                float totalDuration = 0.6f;
                                float FadeInTime = 0.2f;   // 淡入时间（秒）
                                float StayTime = 0.2f;     // 保持时间（秒）
                                float FadeOutTime = 0.2f;  // 淡出时间（秒）
                                // 循环计时器
                                _PinkAlphaTimer = (_PinkAlphaTimer + deltaTime) % totalDuration;

                                // 分阶段计算透明度
                                if (_PinkAlphaTimer <= FadeInTime)
                                {
                                    // 淡入阶段 
                                    _lightningAlpha = _PinkAlphaTimer / FadeInTime;
                                }
                                else if (_PinkAlphaTimer <= FadeInTime + StayTime)
                                {
                                    // 保持阶段 
                                    _lightningAlpha = 1f;
                                }
                                else
                                {
                                    // 淡出阶段 
                                    float fadeOutProgress = (_PinkAlphaTimer - FadeInTime - StayTime) / FadeOutTime;
                                    _lightningAlpha = 1f - fadeOutProgress;
                                }
                                _retreatProgress += (float)time.ElapsedGameTime.TotalMilliseconds / RetreatDuration;
                                if (_retreatProgress <= 1f)
                                {
                                    // 后退阶段 
                                    float t = _retreatProgress;
                                    Position = _originalAttackPos + _retreatOffset * (float)Math.Sin(t * Math.PI);
                                }

                            }
                            _TingLiuTime += (float)time.ElapsedGameTime.TotalMilliseconds;
                            //Position = _attackPosition;
                            if (_TingLiuTime > 600f / timeScale)
                            {
                                _PuGong = true;
                                _nbSelfEffect.AddDisappearEffect(location, _attackPosition);
                                _TingLiu = false;
                                _TingLiuTime = 0f;
                            }
                        }
                        //特殊自爆攻击
                        else if (_TingLiu && _isSpecialAttacking)
                        {
                            //缓冲动画
                            float deltaTime = (float)time.ElapsedGameTime.TotalSeconds;

                            // 计算总动画时间
                            float totalDuration = 0.6f;
                            float FadeInTime = 0.2f;   // 淡入时间（秒）
                            float StayTime = 0.2f;     // 保持时间（秒）
                            float FadeOutTime = 0.2f;  // 淡出时间（秒）
                                                       // 循环计时器
                            _PinkAlphaTimer = (_PinkAlphaTimer + deltaTime) % totalDuration;

                            // 分阶段计算透明度
                            if (_PinkAlphaTimer <= FadeInTime)
                            {
                                // 淡入阶段 
                                _lightningAlpha = _PinkAlphaTimer / FadeInTime;
                            }
                            else if (_PinkAlphaTimer <= FadeInTime + StayTime)
                            {
                                // 保持阶段 
                                _lightningAlpha = 1f;
                            }
                            else
                            {
                                // 淡出阶段 
                                float fadeOutProgress = (_PinkAlphaTimer - FadeInTime - StayTime) / FadeOutTime;
                                _lightningAlpha = 1f - fadeOutProgress;
                            }
                            _TingLiuTime += (float)time.ElapsedGameTime.TotalMilliseconds;

                            if (_TingLiuTime >= 0f && _TingLiuTime < 300f / timeScale)
                            {
                                if (_isRetreating)
                                {
                                    _retreatProgress += (float)time.ElapsedGameTime.TotalMilliseconds / RetreatDuration;

                                    // if (_retreatProgress <= 1f)
                                    //{
                                    // 后退阶段 
                                    float t = _retreatProgress;
                                    Position = _originalAttackPos + _retreatOffset * (float)Math.Sin(t * Math.PI);
                                    //}

                                }

                            }
                            //冲刺
                            else if (_TingLiuTime >= 300f / timeScale)
                            {
                                // 获取当前目标位置
                                Vector2 currentTargetPos = (_specialTarget != null && _specialTarget.Health > 0)
                                    ? _specialTarget.Position + new Vector2(10f, 0f)  // 瞄准怪物中心偏上
                                    : _dashTarget + new Vector2(0f, -128f);  // 如果怪物死亡，使用最后记录的位置

                                //  
                                Vector2 fireballStartPos = Position + _fireballOffset + Offset1;
                                Vector2 motion = Utility.getVelocityTowardPoint(fireballStartPos, currentTargetPos, 10f);

                                _rotationAngle = (float)Math.Atan2(motion.Y, motion.X) + MathHelper.PiOver2 - MathHelper.Pi;

                                //  检查信标和怪物位置是否接近重合
                                float positionDifference = Math.Abs(Position.X - currentTargetPos.X) + Math.Abs(Position.Y - currentTargetPos.Y - 150f);
                                //Game1.addHUDMessage(new HUDMessage("信标"+ Position));
                                //Game1.addHUDMessage(new HUDMessage("怪" + currentTargetPos));
                                //Game1.addHUDMessage(new HUDMessage("positionDifference" + positionDifference));
                                // 如果位置接近重合，触发爆炸
                                if (positionDifference < 75f) // 容差范围
                                {
                                    if (BaoZhaCiShu)
                                    {
                                        Vector2 explosionTile = new Vector2(Position.X / 64, Position.Y / 64 - 2);
                                        Vector2 explosionCenter = new Vector2(Position.X + 32, Position.Y - 96);
                                        int damage = (int)(Game1.random.Next(_BaseDamage - (int)(_BaseDamage * 0.2), _BaseDamage + (int)(_BaseDamage * 0.2) + 1) * 3.75);
                                        _nbHitEffect.CreateSpecialExplosion(location, explosionCenter);
                                        location?.explode(
                                            explosionTile,
                                            1,
                                            Owner,
                                            damageFarmers: false,
                                            damage,
                                            !(location is Farm) && !(location is SlimeHutch)
                                        );
                                        location.playSound("Goldenglow_Boom");
                                        BaoZhaCiShu = false;
                                        XiaoShi = true;
                                    }
                                }
                                else
                                {
                                    // 跟踪移动 
                                    if (motion != Vector2.Zero)
                                    {
                                        motion.Normalize();
                                    }
                                    // 移动速度
                                    float moveSpeed = 15f + 15f * -(1 - timeScale);
                                    moveSpeed = MathHelper.Clamp(moveSpeed, 7.5f, 30f);
                                    //移动
                                    Position += motion * moveSpeed;
                                }

                            }
                            //Position = _attackPosition;
                            if (_TingLiuTime > 600f / timeScale && !BaoZhaCiShu)
                            {
                                _TingLiu = false;
                                _TingLiuTime = 0f;
                                BaoZhaCiShu = true;
                                _PuGong = true; // 进入等待 0.7s 状态(如果_PuGong= true，那么饰品就会消失0.7s)
                                if (!XiaoShi)
                                {
                                    //_nbSelfEffect.AddDisappearEffect(location, _attackPosition);//消失
                                }

                                location.playSound("Goldenglow_BeaconFade");
                            }
                        }
                        //消失
                        else if (_PuGong)
                        {
                            _PuGongCD += (float)time.ElapsedGameTime.TotalMilliseconds;
                            if (_PuGongCD > 700f / timeScale)
                            {
                                BaoZhaCiShu = true;
                                _PuGong = false;
                                XiaoShi = false;
                                _PuGongCD = 0f;
                                _isSpecialAttacking = false;
                                lerp = -1f;
                                //Position = _leavePosition;
                                if (Owner != null)
                                {
                                    // 计算玩家周围的随机位置
                                    float radius = 30f; // 玩家周围的半径
                                    Vector2 randomOffset = new Vector2(
                                        Utility.RandomFloat(-radius, radius),
                                        Utility.RandomFloat(-radius, radius)
                                    );

                                    // 瞬移到玩家位置 + 随机偏移
                                    Position = Owner.Position + randomOffset;
                                    //Game1.addHUDMessage(new HUDMessage("位置" + Position));
                                     
                                    _nbSelfEffect.AddAppearEffect(location, Position);
                                }
                                else
                                {
                                    // 使用原来的位置
                                    _nbSelfEffect.AddAppearEffect(location, Position);
                                }
                                //Position = Owner.Position + _originalOffset;

                                //_TingLiu = true;
                            }
                        }
                    }
                    if (target == null && !_PuGong && !(_TingLiu && _isSpecialAttacking))
                    {
                        _dailyParticleTimer += (float)time.ElapsedGameTime.TotalMilliseconds;
                        if (_dailyParticleTimer >= 250f)
                        {
                            //Game1.addHUDMessage(new HUDMessage("日常特效"));
                            _nbSelfEffect.AddCasualParticles(location, Position);
                            _dailyParticleTimer = 0f;

                        }
                    }
                }
                else if (_TingLiu)
                {
                    //Position = _attackPosition;
                }
            }

            private void StartAttackSequence(Monster target, GameLocation location)
            {
                //if (_PuGong || _TingLiu) return;

                //离开前的位置
                _leavePosition = Position;

                //初始出现位置
                _attackPosition = target.Position + new Vector2(
                    (float)(Math.Cos(Game1.random.NextDouble() * Math.PI) * 100f),
                    (float)(Math.Sin(Game1.random.NextDouble() * Math.PI) * 100f - 90f)
                );
                //Game1.addHUDMessage(new HUDMessage("怪的位置：" + _rotationAngle));
                // 使用存储的位置
                Position = _attackPosition;
                _originalAttackPos = _attackPosition; // 记录初始位置
                _isRetreating = true; // 开始缓冲动画
                _retreatProgress = 0f; // 重置进度

                Vector2 fireballStartPos = Position + _fireballOffset + Offset1;
                Vector2 motion = Utility.getVelocityTowardPoint(fireballStartPos, target.Position, 10f);
                float projectileRotation = (float)Math.Atan2(motion.Y, motion.X) + MathHelper.PiOver2 - MathHelper.Pi;

                // // 计算从随从到怪物的向量
                // Vector2 directionToMonster = target.Position - Position;
                // // 计算向量与正 x 轴的夹角（弧度）
                // float angle = (float)Math.Atan2(-directionToMonster.Y - 128f, -directionToMonster.X - 5f);
                ////Game1.addHUDMessage(new HUDMessage("角度："+ angle));

                //朝向怪物角度
                _rotationAngle = projectileRotation;
                //Game1.addHUDMessage(new HUDMessage("随从的角度：" + _rotationAngle));
                if (_rotationAngle >= 0.08)
                {
                    _isRight = true;
                }
                else
                {
                    _isRight = false;
                }
                //缓冲角度
                float angle1 = _rotationAngle - MathHelper.PiOver2;   
                _retreatOffset = new Vector2(
                    (float)Math.Cos(angle1) * 15f, // 负数表示反向（后退）
                    (float)Math.Sin(angle1) * 15f
                );
                _nbSelfEffect.AddAppearEffect(location, Position);//出现特效
                // 发起攻击
                ShootFireball(location, Owner);

            }
            private void StartSpecialAttack(Monster target, GameLocation location)
            {
                
                //离开前的位置
                _leavePosition = Position;

                //初始出现位置
                _attackPosition = target.Position + new Vector2(
                    (float)(Math.Cos(Game1.random.NextDouble() * Math.PI) * 100f),
                    (float)(Math.Sin(Game1.random.NextDouble() * Math.PI) * 100f - 90f)
                );
                // 使用存储的位置
                Position = _attackPosition;
                _originalAttackPos = _attackPosition; // 记录初始位置
                _isRetreating = true; // 开始缓冲动画
                _retreatProgress = 0f; // 重置进度
                // 计算从随从到怪物的向量
                //Vector2 directionToMonster = target.Position - Position;

                //子弹角度取反变成信标角度
                Vector2 fireballStartPos = Position + _fireballOffset + Offset1;
                Vector2 GuaiWuPianYi = new Vector2(0f, 0f);
                Vector2 motion = Utility.getVelocityTowardPoint(fireballStartPos, target.Position + GuaiWuPianYi, 10f);
                float projectileRotation = (float)Math.Atan2(motion.Y, motion.X) + MathHelper.PiOver2 - MathHelper.Pi;


                //怪的位置+手动偏移
                //Vector2 _dashTarget1 = new Vector2(motion.Y, motion.X);
                Vector2 _dashTarget2 = new Vector2(0f, 128f);
                _dashTarget = target.Position + _dashTarget2;
                _specialTarget= target;
                //Game1.addHUDMessage(new HUDMessage("怪的位置："+ target.Position));

                //朝向怪物角度
                _rotationAngle = projectileRotation;

                //缓冲角度
                float angle1 = _rotationAngle - MathHelper.PiOver2; ////  
                _retreatOffset = new Vector2(
                    (float)Math.Cos(angle1) * 15f, // 负数表示反向（后退）
                    (float)Math.Sin(angle1) * 15f
                );

                _nbSelfEffect.AddAppearEffect(location, Position);//出现特效
                // 发起攻击
                //ShootFireball(location, Owner);

            }

            // 新增方法：更新攻击位置
            public void ResetAttackState()
            {
                _fireballOffset = _originalOffset;
            }

            public override void Draw(SpriteBatch b)
            {
                // 为空时不绘制
                if (Owner == null)
                 return;
                // 冷却期间不绘制
                if (_PuGong || XiaoShi || InEvent) return;
                // 不是节日活动期间
                if (Owner?.currentLocation == null ||
                    Owner.currentLocation.DisplayName == "Temp" && !Game1.isFestival())
                {
                    return;
                }
                const int frameWidth = 32;
                const int frameHeight = 32;
                const int totalFrames = 8;
                const float frameTime = 120f; // 每帧显示时间（毫秒）

                // 计算当前帧
                //_animationTimer += (float)Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
                int currentFrame = (int)(_animationTimer / frameTime) % totalFrames;//信标

                int BeaconCurrentFrame = (int)(_animationTimer / frameTime) % 8;//电流
                Texture2D texture = Game1.content.Load<Texture2D>("Mods/Goldenglow_BeaconGroup");
                Texture2D Pinktexture = Game1.content.Load<Texture2D>("Mods/Goldenglow_PinkCurrentBeaconGroup");
                Texture2D BeaconGroupCurrent = Game1.content.Load<Texture2D>("Mods/Goldenglow_BeaconGroupCurrent");
                Texture2D BeaconGroupCurrent1 = Game1.content.Load<Texture2D>("Mods/Goldenglow_OtherBeaconGroupCurrent");
                // Vector2 _fireballOffset1 = new Vector2(-50f, -148f);
                Vector2 _CurrentOffset1 = new Vector2(0f, 10f);
                float combinedRotation = _rotationAngle1 + _currentRotation; // 组合浮动和移动旋转
                float combinedRotation1 = _rotationAngle1 + _currentRotation + SuijiJiaoDu; // 组合浮动和移动旋转
                //这是打怪的时候
                if (_TingLiu)
                {
                    Vector2 finalPosition = Position + _fireballOffset;
                    b.Draw(
                          texture,
                          Game1.GlobalToLocal(finalPosition + Owner.drawOffset + new Vector2(0f, (0f - height) * 4f) + new Vector2(0f, 0f - height)),
                           new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight),
                          Color.White,
                          _rotationAngle,
                          new Vector2(8f, 8f),
                          1.7f,
                          SpriteEffects.None,
                          0.9f
                      );
                    //电流
                    b.Draw(
                          BeaconGroupCurrent,
                          Game1.GlobalToLocal(finalPosition + Owner.drawOffset + new Vector2(0f, (0f - height) * 4f) + new Vector2(0f, 0f - height)),//位置
                           new Rectangle(DianHuZhen, 0, frameWidth, frameHeight),//帧
                          Color.White,
                          _rotationAngle,
                          new Vector2(8f, 8f),
                          1.7f,
                          SpriteEffects.None,
                          _position.Y / 10000f
                      );
                    //粉色充能闪电
                    b.Draw(
                          Pinktexture,
                          Game1.GlobalToLocal(finalPosition + Owner.drawOffset + new Vector2(0f, (0f - height) * 4f) + new Vector2(0f, 0f - height)),
                           new Rectangle(0, 0, 32, 32),
                          Color.White * _lightningAlpha,
                          _rotationAngle,
                          new Vector2(8f, 8f),
                          1.7f,
                          SpriteEffects.None,
                          1f
                      );
                }
                else//这是日常
                {
                    Vector2 finalPosition = Position + _customOffset + _fireballOffset;
                    b.Draw(
                         texture,
                         Game1.GlobalToLocal(finalPosition + Owner.drawOffset + new Vector2(0f, (0f - height) * 4f) + new Vector2(0f, 0f - height)),
                          new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight),
                         Color.White,
                         combinedRotation, // 使用组合后的旋转角度,
                         new Vector2(8f, 8f),
                         1.7f,
                         SpriteEffects.None,
                         _position.Y / 10000f
                     );
                    //电流
                    b.Draw(
                          BeaconGroupCurrent1,
                          Game1.GlobalToLocal(finalPosition + _CurrentOffset1 + Owner.drawOffset + new Vector2(0f, (0f - height) * 4f) + new Vector2(0f, 0f - height)),
                           new Rectangle(DianHuZhen, 0, frameWidth, frameHeight),
                          Color.White,
                          combinedRotation1, // 使用组合后的旋转角度
                          new Vector2(8f, 8f),
                          SuijiDaXiao,
                          SpriteEffects.None,
                          _position.Y / 10000f
                      );
                }
            }

            //发射电流方法 


            private void ShootFireball(GameLocation location, Farmer farmer)
            {
                //Monster target = Utility.findClosestMonsterWithinRange(location, Owner.Position, 1000);
                if (farmer == null) farmer = Game1.player;
                if (target == null) return;

                Vector2 fireballStartPos = Position + _fireballOffset + Offset1;
                Vector2 direction = target.Position - fireballStartPos;
                direction.Normalize();

                // 计算旋转角度
                Vector2 motion = Utility.getVelocityTowardPoint(fireballStartPos, target.Position, 10f);
                float projectileRotation = (float)Math.Atan2(motion.Y, motion.X) + MathHelper.PiOver2;
                //Game1.addHUDMessage(new HUDMessage("子弹的角度：" + projectileRotation));
                Vector2 velocity = Utility.getVelocityTowardPoint(Position + _fireballOffset + Offset1, target.Position, 10f);
                NBProjectile fireball = new NBProjectile(
                    actualDamage: Game1.random.Next(_ActualDamage - (int)(_ActualDamage * 0.2), _ActualDamage + (int)(_ActualDamage * 0.2) + 1),
                spriteIndex: 47,
                bouncesTillDestruct: 0,
                tailLength: 2,
                rotationVelocity: 0f,
                xVelocity: motion.X,
                yVelocity: motion.Y,
                startingPosition: Position + _fireballOffset + Offset1,
                collisionSound: null,
                location: location,
                firer: farmer,
                target: target, // 传递目标怪物
                trackStrength: 0.4f, // 跟踪强度d
                maxTrackSpeed: 12f,    // 最大跟踪速度 
                collisionBehavior: OnFireballCollision
                );

                // 弹射物属性设置
                fireball.maxVelocity.Value = 20f;
                fireball.acceleration.Value = velocity * 0.1f;
                fireball.light.Value = true;
                fireball.IgnoreLocationCollision = true;
                fireball.ignoreObjectCollisions.Value = true;
                fireball.startingRotation.Value = projectileRotation;

                if (Game1.IsMasterGame)
                    location.projectiles.Add(fireball);
                else
                {
                    // 客机端：添加子弹到场景用于视觉显示（导致子弹来回横条）（禁用）
                    location.projectiles.Add(fireball);
                    _nbSelfEffect.AddAppearEffect(location, Position + _fireballOffset + Offset1);
                }
            }


            private static readonly Lazy<Random> _lazyRandom = new Lazy<Random>(() => new Random());
            private static Random random => _lazyRandom.Value; // 通过属性访问
            private void OnFireballCollision(GameLocation location, int x, int y, Character who)
            {
                // 调用饰品效果中的爆炸方法
                Vector2 explosionCenter = new Vector2(x, y);
                
                _nbHitEffect.CreateExplosion(location, explosionCenter);
                if (this.Owner.IsLocalPlayer)
                {
                    ModEntry.StaticHelper.Multiplayer.SendMessage(
                        message: new ModEntry.HitEffectMessage
                        {
                            EffectType = "explosion",
                            X = explosionCenter.X,
                            Y = explosionCenter.Y,
                            LocationName = location.Name
                        },
                        messageType: "GoldenglowTrinket/PlayHitEffect"
                    );
                }

            }

        }
    }
}