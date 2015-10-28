using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace SpaceShipDemo
{
    [DataContract]
    public class ShipBehavior : Behavior
    {
        [RequiredComponent]
        public Transform3D Tranform;

        [DataMember]
        public float Speed { get; set; }

        private float currentSpeed;

        protected override void Initialize()
        {
            base.Initialize();

            this.currentSpeed = this.Speed;
        }

        protected override void Update(TimeSpan gameTime)
        {
            var rotation = Vector3.Zero;

            var input = WaveServices.Input.KeyboardState;

            if (input.W == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                rotation.Y -= (float)gameTime.TotalSeconds;
            }

            if (input.S == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                rotation.Y += (float)gameTime.TotalSeconds;
            }

            if (input.A == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                rotation.X -= (float)gameTime.TotalSeconds;
            }

            if (input.D == WaveEngine.Common.Input.ButtonState.Pressed)
            {
                rotation.X += (float)gameTime.TotalSeconds;
            }

            this.Tranform.LocalOrientation *= Quaternion.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z);

            var localPosition = this.Tranform.LocalPosition;

            this.Tranform.LocalPosition += ((float)gameTime.TotalSeconds) * this.currentSpeed * this.Tranform.WorldTransform.Forward;

            ////this.Tranform.LocalPosition = localPosition;
        }

        public void Reset()
        {
            this.Owner.IsVisible = true;
            this.Tranform.Position = Vector3.Zero;
            this.Tranform.Rotation = Vector3.Zero;
            this.currentSpeed = this.Speed;
        }

        public void GameOver()
        {
            this.Owner.IsVisible = false;
            this.currentSpeed = 0;
        }
    }
}
