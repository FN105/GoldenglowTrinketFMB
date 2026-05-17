using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenglowTrinket.NB.NormalBeaconTAS
{

    public class NBTASMgmt : TemporaryAnimatedSprite
    {
        private float growDuration;
        private float shrinkDuration;
        private bool isGrowing = true;
        private float totalTimer;
        private string textureName1;
        public string ParticleId { get; }
        private Dictionary<string, bool> canTrigger = new Dictionary<string, bool>();
        //没有闪烁，反转
        public NBTASMgmt(string LiZiId, float growDur, string textureName, Rectangle sourceRect, float animationInterval, int animationLength, int numberOfLoops, Vector2 position, float layerDepth, float alphaFade, Color color, float scale, float scaleChange, float rotation, float rotationChange)
            : base(textureName, sourceRect, animationInterval, animationLength, numberOfLoops, position, false, false, layerDepth, alphaFade, color, scale, scaleChange, rotation, rotationChange)
        {
            ParticleId = LiZiId;
            growDuration = growDur; // 多少毫秒开始变
            canTrigger[ParticleId] = true;
            if (ParticleId == "垂直叶子" || ParticleId == "垂直星星" || ParticleId == "闪烁星星1" || ParticleId == "白色闪烁光团")
            {
                //scaleChange1 = this.scaleChange; // 反转传入的 scaleChange
            }
        }

        public override bool update(GameTime time)
        {
            totalTimer += (float)time.ElapsedGameTime.TotalMilliseconds;
            if (isGrowing && totalTimer >= growDuration)
            {
                isGrowing = false;

                //蓝三角是1，粉三角是2
                if (ParticleId == "蓝三角")
                {

                    scaleChange = 0.001f;
                    alphaFade = 0.05f;
                }
                if (ParticleId == "粉三角")
                {

                    scaleChange = 0.001f;
                    alphaFade = 0.05f;
                }
                if (ParticleId == "直线蓝色电")//直线蓝色电
                {

                    //this.scaleChange = 0.001f;
                    alphaFade = 0.1f;
                }
                if (ParticleId == "粉色十字")//粉色十字
                {

                    //this.scaleChange = 0.001f;
                    alphaFade = 0.1f;
                }
            }
            return base.update(time);
        }
    }
}
