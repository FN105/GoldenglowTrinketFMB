using StardewValley;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenglowTrinket.NB.NormalBeaconTAS
{
    public class NBSelfEffect
    {

        public Vector2 _fireballOffset { get; private set; }
        public bool _isRight { get; private set; }
        //private dynamic _companion; 
        private static readonly Lazy<Random> _lazyRandom = new Lazy<Random>(() => new Random());
        private static Random random => _lazyRandom.Value; 

        public string CompanionId { get; private set; }

        public NBSelfEffect(string companionId = null)
        {
            CompanionId = companionId ?? Guid.NewGuid().ToString();
        }
        public NBSelfEffect(NormalBeaconTrinket.FireballCompanion fireballCompanion)
        {
            // 初始化默认值
            _fireballOffset = Vector2.Zero;
            _isRight = false;

        }

        public void UpdateFromCompanion(Vector2 fireballOffset, bool isRight)
        {
            _fireballOffset = fireballOffset;
            _isRight = isRight;
        }

        //出现特效
        public void AddAppearEffect(GameLocation location, Vector2 position)
        {
            //1是从小到大
            Vector2 SOffset = new Vector2(-32, -32);
            Vector2 Offset36 = new Vector2(-40, -40);
            Vector2 ZOffset18 = new Vector2(8, 8);
            Vector2 Offset48 = new Vector2(-48, -48);
            Vector2 Offset64 = new Vector2(-64, -64);
            Vector2 newSOffset = new Vector2(-96, -96);
            Vector2 JiaoZheng = new Vector2(-45, -210);
            Vector2 effectPosition = position + _fireballOffset + JiaoZheng; // 叠加偏移
            Random rand = new Random();
            //float drop = 0.03f;
            //float drop = 0.03f;


            var particle1 = new TemporaryAnimatedSprite(
                    textureName: "Mods/Goldenglow_PinkAnnularLightGroup1",
                    sourceRect: new Rectangle(0, 0, 64, 64),
                    animationInterval: 30,
                    animationLength: 10,
                    numberOfLoops: 1,
                    position: effectPosition + ZOffset18,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.1f,
                    alphaFade: 0.035f,
                    color: Color.White,
                    scale: 1.6f,//缩放
                    scaleChange: 0f,//缩放变化
                    rotation: 0f,
                    rotationChange: 0f
                );
            var particle2 = new TemporaryAnimatedSprite(
                    textureName: "Mods/Goldenglow_WhiteAnnularLightGroup1",
                    sourceRect: new Rectangle(0, 0, 64, 64),
                    animationInterval: 30,
                    animationLength: 10,
                    numberOfLoops: 1,
                    position: effectPosition + ZOffset18,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.1f,
                    alphaFade: 0.055f,
                    color: Color.White,
                    scale: 1.6f,//缩放
                    scaleChange: 0f,//缩放变化
                    rotation: 0f,
                    rotationChange: 0f
                );
            var particle3 = new TemporaryAnimatedSprite(
                       textureName: "Mods/Goldenglow_ShineGroup",
                       sourceRect: new Rectangle(0, 0, 64, 64),
                       animationInterval: 500f,
                       animationLength: 8,
                       numberOfLoops: 1,
                       position: effectPosition + ZOffset18 + Offset36,
                       flicker: false,
                       flipped: false,
                       layerDepth: 0.9f,
                       alphaFade: 0.025f,
                       color: Color.White,
                       scale: 3f,
                       scaleChange: 0f,
                       rotation: 0f,
                       rotationChange: 0f
                   );
            var particle4 = new TemporaryAnimatedSprite(
                   textureName: "Mods/Goldenglow_Shine",
                   sourceRect: new Rectangle(0, 0, 64, 64),
                   animationInterval: 500,
                   animationLength: 1,
                   numberOfLoops: 1,
                   position: effectPosition + ZOffset18 + Offset36,
                   flicker: false,
                   flipped: false,
                   layerDepth: 0.9f,
                   alphaFade: 0.058f,
                   color: Color.White,
                   scale: 3f,
                   scaleChange: 0f,
                   rotation: 0f,
                   rotationChange: 0f
               );
            particle1.motion = new Vector2(
            0, 0);
            location.temporarySprites.Add(particle1);
            particle2.motion = new Vector2(
            0, 0);
            location.temporarySprites.Add(particle2);
            location.temporarySprites.Add(particle3);
            location.temporarySprites.Add(particle4);
            for (int i = 0; i < 4; i++)
            {
                Vector2 ZOffset20 = new Vector2(30, 30);
                float angle = (float)(rand.NextDouble() * Math.PI * 2);
                float speed = (float)(rand.NextDouble() * 2 + 0.3);
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.05);//角度()
                float scale1 = (float)(0.2 + rand.NextDouble() * 0.5);//大小()
                                                                      // f粉色光球
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/Goldenglow_PinkLightBall",
                    sourceRect: new Rectangle(0, 0, 32, 32),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 1000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: effectPosition + ZOffset20,//位置：爆炸中心
                    flicker: false,//闪烁：否
                    flipped: false,//翻转：否
                    layerDepth: 0.9f,//层深度：0.9f
                    alphaFade: 0.01f,//阿尔法渐变（透明度渐变）：0.001f
                    color: Color.White,//
                    scale: scale1,//缩放：3.5f
                    scaleChange: 0f,//缩放变化：0f
                    rotation: 0f,//旋转：0f
                    rotationChange: rotationChange1//旋转变化：0.05f

                );
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                location.temporarySprites.Add(particle);
            }
        }

        //消失特效
        public void AddDisappearEffect(GameLocation location, Vector2 position)
        {


            Vector2 ZOffset18 = new Vector2(8, 8);
            Vector2 SOffset = new Vector2(-32, -32);
            Vector2 Offset36 = new Vector2(-36, -36);
            Vector2 Offset48 = new Vector2(-48, -48);
            Vector2 Offset64 = new Vector2(-64, -64);
            Vector2 newSOffset = new Vector2(-96, -96);
            Vector2 newShine7fOffset = new Vector2(-96, -96);//64
            Vector2 JiaoZheng = new Vector2(-45, -210);
            Vector2 Dianhu = new Vector2(30, 30);
            Vector2 effectPosition = position + _fireballOffset + JiaoZheng; // 叠加偏移
            if (_isRight)
            {
                Vector2 RightOffset = new Vector2(-12, 16);
                effectPosition += RightOffset;
            }
            Random rand = new Random();
            int particleCount = 0; // 已生成次数
            Action generateParticle = null; // 递归调用自身
            generateParticle = () =>
            {
                if (particleCount < 4) // 总共生成4次
                {
                    // 生成随机贴图
                    int randomNumber = random.Next(8, 15);
                    string textureName = $"Mods/Goldenglow_Arc{randomNumber}";

                    //随机角度
                    float initialRotation = (float)(random.NextDouble() * Math.PI * 2);

                    var particle = new TemporaryAnimatedSprite(
                        textureName: textureName,
                        sourceRect: new Rectangle(0, 0, 64, 64),
                        animationInterval: 60,
                        animationLength: 1,
                        numberOfLoops: 1,
                        position: effectPosition + Dianhu,
                        flicker: false,
                        flipped: false,
                        layerDepth: 0.9f,
                        alphaFade: 0.055f,
                        color: Color.White,
                        scale: 0.8f,
                        scaleChange: 0f,
                        rotation: initialRotation,
                        rotationChange: 0f
                    );

                    location.temporarySprites.Add(particle);

                    // 递归调用，间隔100ms
                    particleCount++;
                    Game1.delayedActions.Add(new DelayedAction(130, generateParticle));
                }
            };
            Game1.delayedActions.Add(new DelayedAction(0, generateParticle));
            var particle1 = new TemporaryAnimatedSprite(
                    textureName: "Mods/Goldenglow_PinkAnnularLightGroup2",
                    sourceRect: new Rectangle(0, 0, 64, 64),
                    animationInterval: 30,
                    animationLength: 10,
                    numberOfLoops: 1,
                    position: effectPosition + ZOffset18,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.1f,
                    alphaFade: 0.035f,
                    color: Color.White,
                    scale: 1.6f,//缩放
                    scaleChange: 0f,//缩放变化
                    rotation: 0f,
                    rotationChange: 0f
                );
            var particle2 = new TemporaryAnimatedSprite(
                    textureName: "Mods/Goldenglow_WhiteAnnularLightGroup2",
                    sourceRect: new Rectangle(0, 0, 64, 64),
                    animationInterval: 30,
                    animationLength: 10,
                    numberOfLoops: 1,
                    position: effectPosition + ZOffset18,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.1f,
                    alphaFade: 0.055f,
                    color: Color.White,
                    scale: 1.6f,//缩放
                    scaleChange: 0f,//缩放变化
                    rotation: 0f,
                    rotationChange: 0f
                );
            var particle3 = new TemporaryAnimatedSprite(
                       textureName: "Mods/Goldenglow_ShineGroup",
                       sourceRect: new Rectangle(0, 0, 64, 64),
                       animationInterval: 500f,
                       animationLength: 8,
                       numberOfLoops: 1,
                       position: effectPosition + Offset36,
                       flicker: false,
                       flipped: false,
                       layerDepth: 0.9f,
                       alphaFade: 0.025f,
                       color: Color.White,
                       scale: 3f,
                       scaleChange: 0f,
                       rotation: 0f,
                       rotationChange: 0f
                   );
            var particle4 = new TemporaryAnimatedSprite(
                   textureName: "Mods/Goldenglow_Shine",
                   sourceRect: new Rectangle(0, 0, 64, 64),
                   animationInterval: 500,
                   animationLength: 1,
                   numberOfLoops: 1,
                   position: effectPosition + Offset36,
                   flicker: false,
                   flipped: false,
                   layerDepth: 0.9f,
                   alphaFade: 0.058f,
                   color: Color.White,
                   scale: 3f,
                   scaleChange: 0f,
                   rotation: 0f,
                   rotationChange: 0f
               );
            particle1.motion = new Vector2(
            0, 0);
            location.temporarySprites.Add(particle1);
            particle2.motion = new Vector2(
            0, 0);
            location.temporarySprites.Add(particle2);
            location.temporarySprites.Add(particle3);
            location.temporarySprites.Add(particle4);
            var particle5 = new TemporaryAnimatedSprite(
                    textureName: "Mods/Goldenglow_CrossLight",
                    sourceRect: new Rectangle(0, 0, 64, 64),
                    animationInterval: 27f,
                    animationLength: 12,
                    numberOfLoops: 1,
                    position: effectPosition + Offset36,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.9f,
                    alphaFade: 0.04f,
                    color: Color.White,
                    scale: 3f,//缩放
                    scaleChange: 0f,//缩放变化
                    rotation: 0f,
                    rotationChange: 0f
                );
            var particle6 = new TemporaryAnimatedSprite(
                    textureName: "Mods/Goldenglow_CrossLight",
                    sourceRect: new Rectangle(0, 0, 64, 64),
                    animationInterval: 27f,
                    animationLength: 12,
                    numberOfLoops: 1,
                    position: effectPosition + Offset36,
                    flicker: false,
                    flipped: false,
                    layerDepth: 0.8f,
                    alphaFade: 0.015f,
                    color: Color.White,
                    scale: 3f,//缩放
                    scaleChange: 0f,//缩放变化
                    rotation: 0f,
                    rotationChange: 0f
                );

            particle1.motion = new Vector2(
                0, 0);

            location.temporarySprites.Add(particle5);

        }
        //日常特效
        public void AddCasualParticles(GameLocation location, Vector2 position)
        {
            Vector2 ZOffset18 = new Vector2(8, 8);
            Vector2 SOffset = new Vector2(-32, -32);
            Vector2 Offset36 = new Vector2(-36, -36);
            Vector2 Offset48 = new Vector2(-48, -48);
            Vector2 Offset64 = new Vector2(-64, -64);
            Vector2 newSOffset = new Vector2(-96, -96);
            Vector2 newShine7fOffset = new Vector2(-96, -96);//64
            Vector2 JiaoZheng = new Vector2(-45, -210);
            Vector2 Dianhu = new Vector2(50, 50);
            Vector2 effectPosition = position + _fireballOffset + JiaoZheng; // 叠加偏移
            Random rand = new Random();
            int randomNumber = random.Next(0, 3);
            int randomNumber1 = random.Next(0, 2);
            if (randomNumber == 1)
            {
                for (int i = 0; i < randomNumber1; i++)
                {
                    float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                    float speed = (float)(rand.NextDouble() * 0.9 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                    float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                    Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                    float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.05);//角度()
                    float scale1 = (float)(0.2 + rand.NextDouble() * 0.3);//大小()
                                                                          // 蓝色光球
                    var particle = new TemporaryAnimatedSprite(
                        textureName: "Mods/Goldenglow_PinkLightBall",
                        sourceRect: new Rectangle(0, 0, 32, 32),//源矩形：新的矩形(0, 0, 7, 7)
                        animationInterval: 1000f,//动画间隔：40f
                        animationLength: 1,//动画长度：15
                        numberOfLoops: 1,//循环次数：1
                        position: effectPosition + Dianhu,//位置：爆炸中心
                        flicker: false,//闪烁：否
                        flipped: false,//翻转：否
                        layerDepth: 0.9f,//层深度：0.9f
                        alphaFade: 0.015f,//阿尔法渐变（透明度渐变）：0.001f
                        color: Color.White,//
                        scale: scale1,//缩放：3.5f
                        scaleChange: 0f,//缩放变化：0f
                        rotation: 0f,//旋转：0f
                        rotationChange: rotationChange1//旋转变化：0.05f

                    );

                    // 手动设置运动参数
                    particle.motion = new Vector2(
                        (float)Math.Cos(angle) * speed,// 
                        (float)Math.Sin(angle) * speed // 
                    );
                    //particle.acceleration = new Vector2(0, 0.3f);// (+粒子向下飘,-粒子向上飘)

                    location.temporarySprites.Add(particle);
                }
            }
            else if (randomNumber == 2)
            {
                for (int i = 0; i < randomNumber1; i++)
                {
                    float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                    float speed = (float)(rand.NextDouble() * 0.9 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                    float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                    Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                    float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.05);//角度()
                    float scale1 = (float)(0.2 + rand.NextDouble() * 0.3);//大小()
                                                                          // 蓝色光球
                    var particle = new TemporaryAnimatedSprite(
                        textureName: "Mods/Goldenglow_BlueLightBall",
                        sourceRect: new Rectangle(0, 0, 32, 32),//源矩形：新的矩形(0, 0, 7, 7)
                        animationInterval: 1000f,//动画间隔：40f
                        animationLength: 1,//动画长度：15
                        numberOfLoops: 1,//循环次数：1
                        position: effectPosition + Dianhu,//位置：爆炸中心
                        flicker: false,//闪烁：否
                        flipped: false,//翻转：否
                        layerDepth: 0.9f,//层深度：0.9f
                        alphaFade: 0.015f,//阿尔法渐变（透明度渐变）：0.001f
                        color: Color.White,//
                        scale: scale1,//缩放：3.5f
                        scaleChange: 0f,//缩放变化：0f
                        rotation: 0f,//旋转：0f
                        rotationChange: rotationChange1//旋转变化：0.05f

                    );

                    // 手动设置运动参数
                    particle.motion = new Vector2(
                        (float)Math.Cos(angle) * speed,// 
                        (float)Math.Sin(angle) * speed // 
                    );
                    //particle.acceleration = new Vector2(0, 0.3f);// (+粒子向下飘,-粒子向上飘)

                    location.temporarySprites.Add(particle);
                }
            }
            else
            {
                for (int i = 0; i < randomNumber1; i++)
                {
                    float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                    float speed = (float)(rand.NextDouble() * 0.9 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                    float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                    Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                    float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.05);//角度()
                    float scale1 = (float)(0.2 + rand.NextDouble() * 0.3);//大小()
                                                                          // 蓝色光球
                    var particle = new TemporaryAnimatedSprite(
                        textureName: "Mods/Goldenglow_SmallGreenShine",
                        sourceRect: new Rectangle(0, 0, 32, 32),//源矩形：新的矩形(0, 0, 7, 7)
                        animationInterval: 1000f,//动画间隔：40f
                        animationLength: 1,//动画长度：15
                        numberOfLoops: 1,//循环次数：1
                        position: effectPosition + Dianhu,//位置：爆炸中心
                        flicker: false,//闪烁：否
                        flipped: false,//翻转：否
                        layerDepth: 0.9f,//层深度：0.9f
                        alphaFade: 0.015f,//阿尔法渐变（透明度渐变）：0.001f
                        color: Color.White,//
                        scale: scale1,//缩放：3.5f
                        scaleChange: 0f,//缩放变化：0f
                        rotation: 0f,//旋转：0f
                        rotationChange: rotationChange1//旋转变化：0.05f

                    );

                    // 手动设置运动参数
                    particle.motion = new Vector2(
                        (float)Math.Cos(angle) * speed,// 
                        (float)Math.Sin(angle) * speed // 
                    );
                    //particle.acceleration = new Vector2(0, 0.3f);// (+粒子向下飘,-粒子向上飘)

                    location.temporarySprites.Add(particle);
                }
            }


        }
    }
}
