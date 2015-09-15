#region File Description
//-----------------------------------------------------------------------------
// SpawnBehavior
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

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
    /// <summary>
    /// Spawning behavior
    /// </summary>
    [DataContract]
    public class SpawnBehavior : Behavior
    {
        /// <summary>
        /// The transform
        /// </summary>
        [RequiredComponent]
        public Transform3D Transform;

        /// <summary>
        /// The end scale
        /// </summary>
        private Vector3 endScale;

        /// <summary>
        /// If the entity is spawning.
        /// </summary>
        private bool isSpawning = false;

        /// <summary>
        /// Spawns this instance.
        /// </summary>
        public void Spawn()
        {
            this.endScale = this.Transform.Scale;
            this.Transform.Scale = Vector3.Zero;
            this.isSpawning = true;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            if (this.isSpawning)
            {
                // Updates the scaling of the entity
                var scale = this.Transform.Scale;

                this.Transform.Scale = Vector3.Lerp(this.Transform.Scale, endScale, 0.005f);

                if (scale.X >= this.endScale.X)
                {
                    this.isSpawning = false;
                }
            }
        }
    }
}
