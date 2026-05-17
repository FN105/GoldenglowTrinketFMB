using GoldenglowTrinket.NB.NormalBeaconTAS;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenglowTrinket.NB.NormalBeaconTAS
{
    public class NBHitEffect
    {
        private static readonly Lazy<Random> _lazyRandom = new Lazy<Random>(() => new Random());
        private static Random random => _lazyRandom.Value; // 通过属性访问
        //电流粒子///////////////////////////
        public void CreateExplosion(GameLocation location, Vector2 epicenter)
        {
            Vector2 SOffset = new Vector2(-32, -32);
            Random rand = new Random();
            //float drop = 0.03f;
            //float drop = 0.03f;
            // 生成间断电弧
            int particleCount = 0; // 已生成次数
            Action generateParticle = null; // 递归调用自身
            generateParticle = () =>
            {
                Vector2 SOffset1 = new Vector2(-40, -32);
                if (particleCount < 6) // 总共生成6次
                {
                    // 生成随机贴图
                    int randomNumber = random.Next(1, 15);
                    string textureName = $"Mods/Goldenglow_Arc{randomNumber}";

                    //随机角度
                    float initialRotation = (float)(random.NextDouble() * Math.PI * 2);

                    var particle = new TemporaryAnimatedSprite(
                        textureName: textureName,
                        sourceRect: new Rectangle(0, 0, 64, 64),
                        animationInterval: 90,
                        animationLength: 1,
                        numberOfLoops: 1,
                        position: epicenter + SOffset1,
                        flicker: false,
                        flipped: false,
                        layerDepth: 0.9f,
                        alphaFade: 0.025f,
                        color: Color.White,
                        scale: 1.5f,
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

            // 首次触发生成
            Game1.delayedActions.Add(new DelayedAction(0, generateParticle));
            for (int i = 0; i < 4; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 2 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.05);//角度()
                float scale1 = (float)(0.2 + rand.NextDouble() * 0.6);//大小()
                                                                      // 蓝色光球
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/Goldenglow_BlueLightBall",
                    sourceRect: new Rectangle(0, 0, 32, 32),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 1000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter,//位置：爆炸中心
                    flicker: false,//闪烁：否
                    flipped: false,//翻转：否
                    layerDepth: 0.9f,//层深度：0.9f
                    alphaFade: 0.015f,// （透明度渐变） 
                    color: Color.White, 
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
            for (int i = 0; i < 4; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);
                float speed = (float)(rand.NextDouble() * 2 + 0.3);
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.05);//角度()
                float scale1 = (float)(0.2 + rand.NextDouble() * 0.6);//大小()
                                                                      // f粉色光球
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/Goldenglow_PinkLightBall",
                    sourceRect: new Rectangle(0, 0, 32, 32),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 1000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter,//位置：爆炸中心
                    flicker: false,//闪烁：否
                    flipped: false,//翻转：否
                    layerDepth: 0.9f,//层深度：0.9f
                    alphaFade: 0.01f,// （透明度渐变） 
                    color: Color.White, 
                    scale: scale1,//缩放：3.5f
                    scaleChange: 0f,//缩放变化：0f
                    rotation: rotationChange1,//旋转：0f
                    rotationChange: rotationChange1//旋转变化：0.05f

                );

                // 手动设置运动参数
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                //particle.acceleration = new Vector2(0, 0.3f);

                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < 1; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 2 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.05);//角度()
                float scale1 = (float)(0.2 + rand.NextDouble() * 1.1);//大小()
                Vector2 SOffset1 = new Vector2(-27, -16);

                // 五角星
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/Goldenglow_PinkStar",
                    sourceRect: new Rectangle(0, 0, 32, 32),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 500,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter + SOffset1,//位置：爆炸中心
                    flicker: false,//闪烁：否
                    flipped: false,//翻转：否
                    layerDepth: 0.9f,//层深度：0.9f
                    alphaFade: 0.025f,// （透明度渐变） 
                    color: Color.White, 
                    scale: 1.5f,//缩放：3.5f
                    scaleChange: 0f,//缩放变化：0f
                    rotation: rotationChange1,//旋转：0f
                    rotationChange: 0f//旋转变化：0.05f

                );
                var particle1 = new TemporaryAnimatedSprite(
                    textureName: "Mods/Goldenglow_WhiteStar",
                    sourceRect: new Rectangle(0, 0, 32, 32),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 500,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter + SOffset1,//位置：爆炸中心
                    flicker: false,//闪烁：否
                    flipped: false,//翻转：否
                    layerDepth: 0.9f,//层深度：0.9f
                    alphaFade: 0.05f,// （透明度渐变） 
                    color: Color.White, 
                    scale: 1.5f,//缩放：3.5f
                    scaleChange: 0f,//缩放变化：0f
                    rotation: rotationChange1,//旋转：0f
                    rotationChange: 0f//旋转变化：0.05f

                );
                location.temporarySprites.Add(particle1);
                location.temporarySprites.Add(particle);

            }

            for (int i = 0; i < 1; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);
                float speed = (float)(rand.NextDouble() * 2 + 0.3);
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float rotationChange1 = (float)(0.02 + rand.NextDouble() * 0.1);//角度()
                float scale1 = (float)(1.2 + rand.NextDouble() * 1.5);//大小()

                // 蓝色三角Goldenglow_BlueTriangle

                var particle = new NBTASMgmt(
                     "蓝三角",//id
                     700f,//多少毫秒后开始动态变化
                    "Mods/Goldenglow_BlueTriangle",
                    sourceRect: new Rectangle(0, 0, 32, 32),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 1000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter + SOffset,//位置：爆炸中心
                    layerDepth: 0.9f,
                    alphaFade: 0.01f,// （透明度渐变） 
                    color: Color.White,
                    scale: scale1,//缩放：3.5f
                    scaleChange: 0f,//缩放变化：0f
                    rotation: 0f,//旋转：0f
                    rotationChange: rotationChange1//旋转变化：0.05f     
                    );

                // 手动设置运动参数
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                //particle.acceleration = new Vector2(0, 0.3f);

                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < 1; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);
                float speed = (float)(rand.NextDouble() * 2 + 0.3);
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float rotationChange1 = (float)(0.02 + rand.NextDouble() * 0.1);//角度()
                float scale1 = (float)(1.2 + rand.NextDouble() * 1.5);//大小()

                var particle = new NBTASMgmt(
                     "粉三角",
                     700f,//时长
                    "Mods/Goldenglow_PinkTriangle",
                    sourceRect: new Rectangle(0, 0, 32, 32),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 1000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter + SOffset,//位置：爆炸中心
                    layerDepth: 0.9f,
                    alphaFade: 0.01f,// （透明度渐变） 
                    color: Color.White,
                    scale: scale1,//缩放：3.5f
                    scaleChange: 0f,//缩放变化：0f
                    rotation: 0f,//旋转：0f
                    rotationChange: rotationChange1//旋转变化：0.05f     
                    );

                // 手动设置运动参数
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                //particle.acceleration = new Vector2(0, 0.3f);

                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < 2; i++)
            {
                // 1. 随机生成一个角度（0到360度）
                float angle = (float)(Game1.random.NextDouble() * Math.PI * 2);

                // 2. 根据角度计算运动方向（速度向量）
                float speed = 4f; // 粒子移动速度
                Vector2 motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );

                // 3. 计算粒子旋转角度（使头部对准运动方向）
                float rotation = (float)Math.Atan2(motion.Y, motion.X) + MathHelper.PiOver2;

                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.05);//角度()
                float scale1 = (float)(0.2 + rand.NextDouble() * 2.5);//大小()

                var particle = new NBTASMgmt(
                     "直线蓝色电",//id
                     250f,//时长
                    "Mods/Goldenglow_LineLight",
                    sourceRect: new Rectangle(0, 0, 32, 32),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 500f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter + SOffset,//位置：爆炸中心
                    layerDepth: 0.9f,
                    alphaFade: 0.01f,// （透明度渐变） 
                    color: Color.White,
                    scale: scale1,//缩放：3.5f
                    scaleChange: 0f,//缩放变化：0f
                    rotation: rotation,//旋转：0f
                    rotationChange: 0f//旋转变化：0.05f   
                    );
                // 5. 设置粒子运动方向和加速度
                particle.motion = motion; // 向外运动
                                          //particle.acceleration = new Vector2(0, 0.1f);  

                location.temporarySprites.Add(particle);
            }
            location.playSound("Goldenglow_NormalAttack");

        }



        //爆炸粒子///////////////////////////
        public void CreateSpecialExplosion(GameLocation location, Vector2 epicenter)
        {
            Vector2 Offset16 = new Vector2(-16, -16);
            Vector2 SOffset = new Vector2(-32, -32);
            Vector2 Offset48 = new Vector2(-48, -48);
            Vector2 Offset64 = new Vector2(-64, -64);
            Vector2 newSOffset = new Vector2(-96, -96);
            Vector2 newShine7fOffset = new Vector2(-96, -96);//64
            epicenter = epicenter + Offset16;
            Random rand = new Random();
            //float drop = 0.03f;
            //float drop = 0.03f;
            // 生成间断电弧
            int particleCount = 0; // 已生成次数
            Action generateParticle = null; // 递归调用自身
            generateParticle = () =>
            {
                if (particleCount < 6) // 总共生成6次
                {
                    // 生成随机贴图
                    int randomNumber = random.Next(1, 15);
                    string textureName = $"Mods/Goldenglow_Arc{randomNumber}";

                    //随机角度
                    float initialRotation = (float)(random.NextDouble() * Math.PI * 2);

                    var particle = new TemporaryAnimatedSprite(
                        textureName: textureName,
                        sourceRect: new Rectangle(0, 0, 64, 64),
                        animationInterval: 90,
                        animationLength: 1,
                        numberOfLoops: 1,
                        position: epicenter + Offset16 + newSOffset,
                        flicker: false,
                        flipped: false,
                        layerDepth: 0.9f,
                        alphaFade: 0.025f,
                        color: Color.White,
                        scale: 4f,
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
            // 首次触发生成
            Game1.delayedActions.Add(new DelayedAction(0, generateParticle));


            for (int i = 0; i < 8; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 2 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.05);//角度()
                float scale1 = (float)(0.2 + rand.NextDouble() * 1.3);//大小()
                                                                      // 蓝色光球
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/Goldenglow_BlueLightBall",
                    sourceRect: new Rectangle(0, 0, 32, 32),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 1000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter,//位置：爆炸中心
                    flicker: false,//闪烁：否
                    flipped: false,//翻转：否
                    layerDepth: 0.9f,//层深度：0.9f
                    alphaFade: 0.015f,// （透明度渐变） 
                    color: Color.White, 
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
            for (int i = 0; i < 8; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);
                float speed = (float)(rand.NextDouble() * 2 + 0.3);
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.05);//角度()
                float scale1 = (float)(0.2 + rand.NextDouble() * 1.3);//大小()
                                                                      // f粉色光球
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/Goldenglow_PinkLightBall",
                    sourceRect: new Rectangle(0, 0, 32, 32),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 1000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter,//位置：爆炸中心
                    flicker: false,//闪烁：否
                    flipped: false,//翻转：否
                    layerDepth: 0.9f,//层深度：0.9f
                    alphaFade: 0.01f,// （透明度渐变） 
                    color: Color.White, 
                    scale: scale1,//缩放：3.5f
                    scaleChange: 0f,//缩放变化：0f
                    rotation: rotationChange1,//旋转：0f
                    rotationChange: rotationChange1//旋转变化：0.05f

                );

                // 手动设置运动参数
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                //particle.acceleration = new Vector2(0, 0.3f);

                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < 1; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);//角度
                float speed = (float)(rand.NextDouble() * 2 + 0.3);//（2,4）的速度（NextDouble()是[0，1））
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;//转速
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.05);//角度()
                float scale1 = (float)(0.2 + rand.NextDouble() * 1.1);//大小()


                // 五角星
                var particle = new TemporaryAnimatedSprite(
                    textureName: "Mods/Goldenglow_PinkStar",
                    sourceRect: new Rectangle(0, 0, 32, 32),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 600,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter + Offset64,//位置：爆炸中心
                    flicker: false,//闪烁：否
                    flipped: false,//翻转：否
                    layerDepth: 1f,//层深度：0.9f
                    alphaFade: 0.03f,// （透明度渐变） 
                    color: Color.White, 
                    scale: 5f,//缩放：3.5f
                    scaleChange: 0f,//缩放变化：0f
                    rotation: rotationChange1,//旋转：0f
                    rotationChange: 0f//旋转变化：0.05f

                );
                var particle1 = new TemporaryAnimatedSprite(
                    textureName: "Mods/Goldenglow_WhiteStar",
                    sourceRect: new Rectangle(0, 0, 32, 32),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 300,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter + Offset64,//位置：爆炸中心
                    flicker: false,//闪烁：否
                    flipped: false,//翻转：否
                    layerDepth: 1.1f,//层深度：0.9f
                    alphaFade: 0.06f,// （透明度渐变） 
                    color: Color.White, 
                    scale: 5f,//缩放：3.5f
                    scaleChange: 0f,//缩放变化：0f
                    rotation: rotationChange1,//旋转：0f
                    rotationChange: 0f//旋转变化：0.05f

                );
                //烟雾
                var particle3 = new TemporaryAnimatedSprite(
                       textureName: "Mods/Goldenglow_ShineGroup",
                       sourceRect: new Rectangle(0, 0, 64, 64),
                       animationInterval: 50f,
                       animationLength: 8,
                       numberOfLoops: 1,
                       position: epicenter + SOffset + newSOffset + newShine7fOffset + SOffset,
                       flicker: false,
                       flipped: false,
                       layerDepth: 0.9f,
                       alphaFade: 0.02f,
                       color: Color.White,
                       scale: 8f,
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
                       position: epicenter + SOffset + newSOffset + newShine7fOffset + SOffset,
                       flicker: false,
                       flipped: false,
                       layerDepth: 0.9f,
                       alphaFade: 0.02f,
                       color: Color.White,
                       scale: 8f,
                       scaleChange: 0f,
                       rotation: 0f,
                       rotationChange: 0f
                   );
                location.temporarySprites.Add(particle1);
                location.temporarySprites.Add(particle);
                location.temporarySprites.Add(particle3);
                location.temporarySprites.Add(particle4);

            }

            for (int i = 0; i < 6; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);
                float speed = (float)(rand.NextDouble() * 2 + 0.3);
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float rotationChange1 = (float)(0.02 + rand.NextDouble() * 0.1);//角度()
                float scale1 = (float)(1.6 + rand.NextDouble() * 3.2);//大小()

                // 蓝色三角Goldenglow_BlueTriangle

                var particle = new NBTASMgmt(
                     "蓝三角",//id
                     700f,//时长
                    "Mods/Goldenglow_BlueTriangle",
                    sourceRect: new Rectangle(0, 0, 32, 32),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 1000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter + Offset48,//位置：爆炸中心
                    layerDepth: 0.9f,
                    alphaFade: 0.01f,// （透明度渐变） 
                    color: Color.White,
                    scale: scale1,//缩放：3.5f
                    scaleChange: 0f,//缩放变化：0f
                    rotation: 0f,//旋转：0f
                    rotationChange: rotationChange1//旋转变化：0.05f     
                    );

                // 手动设置运动参数
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                //particle.acceleration = new Vector2(0, 0.3f);

                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < 6; i++)
            {
                float angle = (float)(rand.NextDouble() * Math.PI * 2);
                float speed = (float)(rand.NextDouble() * 2 + 0.3);
                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float rotationChange1 = (float)(0.02 + rand.NextDouble() * 0.1);//角度()
                float scale1 = (float)(1.6 + rand.NextDouble() * 3.2);//大小()

                var particle = new NBTASMgmt(
                     "粉三角",
                     700f,//时长
                    "Mods/Goldenglow_PinkTriangle",
                    sourceRect: new Rectangle(0, 0, 32, 32),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 1000f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter + Offset48,//位置：爆炸中心
                    layerDepth: 0.9f,
                    alphaFade: 0.01f,// （透明度渐变） 
                    color: Color.White,
                    scale: scale1,//缩放：3.5f
                    scaleChange: 0f,//缩放变化：0f
                    rotation: 0f,//旋转：0f
                    rotationChange: rotationChange1//旋转变化：0.05f     
                    );

                // 手动设置运动参数
                particle.motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                //particle.acceleration = new Vector2(0, 0.3f);

                location.temporarySprites.Add(particle);
            }
            for (int i = 0; i < 8; i++)
            {
                // 0到360度
                float angle = (float)(Game1.random.NextDouble() * Math.PI * 2);

                //根据角度计算运动方向（速度向量）
                float speed = 6f; // 粒子移动速度
                Vector2 motion = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );

                // 计算粒子旋转角度（使头部对准运动方向）
                float rotation = (float)Math.Atan2(motion.Y, motion.X) + MathHelper.PiOver2;

                float rotationSpeed = (float)(rand.NextDouble() - 0.5) * 0.1f;
                Color particleColor = Color.Lerp(Color.Yellow, Color.Red, (float)rand.NextDouble());
                float rotationChange1 = (float)(0.03 + rand.NextDouble() * 0.05);//角度()
                float scale1 = (float)(0.4 + rand.NextDouble() * 3.5);//大小()

                var particle = new NBTASMgmt(
                     "直线蓝色电",//id
                     250f,//时长
                    "Mods/Goldenglow_LineLight",
                    sourceRect: new Rectangle(0, 0, 32, 32),//源矩形：新的矩形(0, 0, 7, 7)
                    animationInterval: 500f,//动画间隔：40f
                    animationLength: 1,//动画长度：15
                    numberOfLoops: 1,//循环次数：1
                    position: epicenter + SOffset,//位置：爆炸中心
                    layerDepth: 0.9f,
                    alphaFade: 0.01f,// （透明度渐变） 
                    color: Color.White,
                    scale: scale1,//缩放
                    scaleChange: 0f,//缩放变化
                    rotation: rotation,//旋转
                    rotationChange: 0f//旋转变化
                    );
                //运动方向和加速度
                particle.motion = motion;

                location.temporarySprites.Add(particle);
            }
            location.playSound("Goldenglow_NormalAttack");

        }
    }
}
