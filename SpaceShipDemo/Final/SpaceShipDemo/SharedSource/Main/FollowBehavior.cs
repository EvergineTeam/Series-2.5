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
    /// <summary>
    /// Follow behavior
    /// </summary>
    [DataContract]
    public class FollowBehavior : Behavior
    {
        /// <summary>
        /// The transform component
        /// </summary>
        [RequiredComponent]
        public Transform3D Transform;

        /// <summary>
        /// The target transform
        /// </summary>
        private Transform3D targetTransform;

        /// <summary>
        /// If the follow is smooth
        /// </summary>
        /// <value>
        ///   <c>true</c> if smooth; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool Smooth { get; set; }

        /// <summary>
        /// Gets or sets the entity path.
        /// </summary>
        /// <value>
        /// The entity path.
        /// </value>
        [DataMember]
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Framework.Graphics.Transform3D" })]
        public string EntityPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity must rotate along the target.
        /// </summary>
        /// <value>
        ///   <c>true</c> if it follows with rotation; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool FollowRotation { get; set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            try
            {
                // Obtains the target transform
                var entity = this.EntityManager.Find(this.EntityPath);
                this.targetTransform = entity.FindComponent<Transform3D>();
            }
            catch (Exception)
            {
                this.targetTransform = null;
            }
        }

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            if (this.targetTransform == null)
            {
                return;
            }

            if (this.Smooth)
            {
                // Smooth follow uses lerp position.
                var lerp = Math.Min(1, 10 * (float)gameTime.TotalSeconds);

                this.Transform.Position = Vector3.Lerp(this.Transform.Position, this.targetTransform.Position, lerp);

                if (this.FollowRotation)
                {
                    this.Transform.Rotation = Vector3.Lerp(this.Transform.Rotation, this.targetTransform.Rotation, lerp);
                }
            }
            else
            {
                // Sets directly the position and (if applicabe) the rotation
                this.Transform.Position = this.targetTransform.Position;

                if (this.FollowRotation)
                {
                    this.Transform.Rotation = this.targetTransform.Rotation;
                }
            }
        }
    }
}
