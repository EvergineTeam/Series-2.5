using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace SpaceShipDemo
{
    [DataContract]
    public class FollowBehavior : Behavior
    {
        [RequiredComponent]
        public Transform3D Transform;

        [DataMember]
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Framework.Graphics.Tranform3D" })]
        public string EntityPath { get; set; }

        [DataMember]
        public bool SmoothFollow { get; set; }

        [DataMember]
        public bool RotatingFollow { get; set; }

        private Transform3D targetTransform;

        protected override void Initialize()
        {
            base.Initialize();

            if (string.IsNullOrEmpty(this.EntityPath))
            {
                return;
            }

            var entity = this.EntityManager.Find(this.EntityPath);
            this.targetTransform = entity.FindComponent<Transform3D>();
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.targetTransform == null)
            {
                return;
            }

            if (this.SmoothFollow)
            {
                // Smooth follow
                var lerp = Math.Min(1, 10 * (float)gameTime.TotalSeconds);

                this.Transform.Position = Vector3.Lerp(this.Transform.Position, this.targetTransform.Position, lerp);

                if (this.RotatingFollow)
                {
                    this.Transform.Rotation = Vector3.Lerp(this.Transform.Rotation, this.targetTransform.Rotation, lerp);
                }
            }
            else
            {
                // Direct follow
                this.Transform.Position = this.targetTransform.Position;

                if (this.RotatingFollow)
                {
                    this.Transform.Rotation = this.targetTransform.Rotation;
                }
            }
        }
    }
}
