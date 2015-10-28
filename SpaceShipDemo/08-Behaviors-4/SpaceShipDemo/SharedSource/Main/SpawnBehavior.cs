using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace SpaceShipDemo
{
    [DataContract]
    public class SpawnBehavior : Behavior
    {
        [RequiredComponent]
        public Transform3D Transform;

        private Vector3 endScale;

        private bool isSpawning;

        public void Spawn()
        {
            this.endScale = this.Transform.Scale;
            this.Transform.Scale = Vector3.Zero;
            this.isSpawning = true;
        }

        protected override void Update(TimeSpan gameTime)
        {
            if(this.isSpawning)
            {
                var scale = this.Transform.Scale;

                scale = Vector3.Lerp(this.Transform.Scale, this.endScale, 0.005f);

                this.Transform.Scale = scale;

                if(scale.X > this.endScale.X)
                {
                    this.isSpawning = false;
                }
            }
        }
    }
}
