#region File Description
//-----------------------------------------------------------------------------
// ShipBehavior
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Sound;

namespace SpaceShipDemo
{
    /// <summary>
    /// The behavior of the ship player
    /// </summary>
    [DataContract]
    public class ShipBehavior : Behavior
    {
        /// <summary>
        /// The transform required component
        /// </summary>
        [RequiredComponent]
        public Transform3D Transform;

        /// <summary>
        /// The initialize position
        /// </summary>
        private Vector3 initPosition;

        /// <summary>
        /// The initialize rotation
        /// </summary>
        private Vector3 initRotation;

        /// <summary>
        /// The initialize speed
        /// </summary>
        private float currentSpeed;

        /// <summary>
        /// Gets or sets the speed of the sheep.
        /// </summary>
        /// <value>
        /// The speed.
        /// </value>
        [DataMember]
        public float Speed { get; set; }

        /// <summary>
        /// Gets or sets the acceleration of the sheep
        /// </summary>
        /// <value>
        /// The acceleration.
        /// </value>
        [DataMember]
        public float Acceleration { get; set; }

        /// <summary>
        /// Gets or sets the maneuverability of the ship
        /// </summary>
        /// <value>
        /// The maneuverability.
        /// </value>
        [DataMember]
        public float Maneuverability { get; set; }

        protected override void Initialize()
        {
            base.Initialize();

            // Sets the initial values
            this.initPosition = this.Transform.Position;
            this.currentSpeed = this.Speed;
            this.initRotation = this.Transform.Rotation;
        }

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            Vector3 rotation = Vector3.Zero;

            // Gets the keyboard input and updates the rotation
            var input = WaveServices.Input.KeyboardState;

            if (input.W == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                rotation.Y -= this.Maneuverability * (float)gameTime.TotalSeconds;
            }

            if (input.S == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                rotation.Y += this.Maneuverability * (float)gameTime.TotalSeconds;
            }

            if (input.A == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                rotation.X += this.Maneuverability * (float)gameTime.TotalSeconds;
            }

            if (input.D == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                rotation.X -= this.Maneuverability * (float)gameTime.TotalSeconds;
            }

            // Creates a quaternion and aplies to the current orientation
            this.Transform.LocalOrientation *= Quaternion.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z);

            // Updates the position
            this.Transform.LocalPosition += ((float)gameTime.TotalSeconds) * this.currentSpeed * this.Transform.WorldTransform.Forward;

            // Updates the speed
            this.currentSpeed += (float)gameTime.TotalSeconds * this.Acceleration;
        }

        /// <summary>
        /// Games the over.
        /// </summary>
        public void GameOver()
        {
            // Sets the game over state, hiding the ship
            this.Owner.IsVisible = false;
            this.currentSpeed = 0;
            this.Acceleration = -this.Acceleration;

            this.Owner.FindChild("engineSound").FindComponent<SoundEmitter3D>().Volume = 0;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            // Sets the ship to its initial position.
            this.Owner.IsVisible = true;
            this.Transform.Rotation = this.initRotation;
            this.Transform.Position = this.initPosition;
            this.currentSpeed = this.Speed;
            this.Acceleration = -this.Acceleration;
            this.Owner.FindChild("engineSound").FindComponent<SoundEmitter3D>().Volume = 1;
        }
    }
}
