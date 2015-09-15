using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        /// <summary>
        /// The end scale
        /// </summary>
        private Vector3 endScale;
        private bool spawning = false;

        public void Spawn()
        {
            this.endScale = this.Transform.Scale;
            this.Transform.Scale = Vector3.Zero;
            this.spawning = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.spawning)
            {
                var scale = this.Transform.Scale;

                this.Transform.Scale = Vector3.Lerp(this.Transform.Scale, endScale, 0.005f);

                if (scale.X >= this.endScale.X)
                {
                    this.spawning = false;
                }
            }
        }
    }
}
