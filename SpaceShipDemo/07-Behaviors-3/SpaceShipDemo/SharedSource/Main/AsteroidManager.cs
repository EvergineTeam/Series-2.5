using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics3D;
using WaveEngine.Framework.Services;

namespace SpaceShipDemo
{
    [DataContract]
    public class AsteroidManager : Behavior
    {
        private bool isSpawned;

        [DataMember]
        public int NumberOfAsteroids { get; set; }

        private List<Entity> asteroids;

        private int asteroidIndex;

        [DataMember]
        public float AsteroidInterval { get; set; }

        [DataMember]
        public float SpawnDistance { get; set; }

        [DataMember]
        public int AsteroidSpread { get; set; }

        [DataMember]
        [RenderPropertyAsEntity]
        public string ShipPath { get; set; }

        private float remainingAsteroidTime;

        private Entity shipEntity;

        protected override void Initialize()
        {
            base.Initialize();

            if(!string.IsNullOrEmpty(this.ShipPath))
            {
                this.shipEntity = this.EntityManager.Find(this.ShipPath);
            }
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (!this.isSpawned)
            {
                // First frame
                this.CreateAsteroids();
                this.isSpawned = true;
                return;
            }

            if((this.shipEntity == null) && (this.NumberOfAsteroids == 0))
            {
                return;
            }

            this.remainingAsteroidTime -= (float)gameTime.TotalSeconds;

            if(this.remainingAsteroidTime < 0)
            {
                this.ShowAsteroid();
                this.remainingAsteroidTime += this.AsteroidInterval;
            }
        }

        private void ShowAsteroid()
        {
            var asteroid = this.asteroids[this.asteroidIndex];
            asteroid.IsVisible = true;

            var shipTransform = this.shipEntity.FindComponent<Transform3D>();

            var position = shipTransform.Position + (shipTransform.WorldTransform.Forward * this.SpawnDistance);

            position.X += WaveServices.Random.Next(-this.AsteroidSpread, this.AsteroidSpread);
            position.Y += WaveServices.Random.Next(-this.AsteroidSpread, this.AsteroidSpread);

            var transform = asteroid.FindComponent<Transform3D>();
            
            transform.Position = position;

            transform.Scale = new Vector3(WaveServices.Random.Next(3, 8));

            var spinner = asteroid.FindComponent<Spinner>();
            spinner.IncreaseX = WaveServices.Random.Next(-100, 100) * 0.01f;
            spinner.IncreaseY = WaveServices.Random.Next(-100, 100) * 0.01f;
            spinner.IncreaseZ = WaveServices.Random.Next(-100, 100) * 0.01f;

            this.asteroidIndex = (this.asteroidIndex + 1) % this.NumberOfAsteroids;
        }

        private void CreateAsteroids()
        {
            this.asteroids = new List<Entity>();

            for (int i = 0; i < this.NumberOfAsteroids; i++)
            {
                var asteroid = this.CreateAsteroids(i);
                this.asteroids.Add(asteroid);

                this.EntityManager.Add(asteroid);
            }

            this.asteroidIndex = 0;
        }

        private Entity CreateAsteroids(int i)
        {
            string model;

            switch (i % this.NumberOfAsteroids)
            {       
                case 0:
                    model = WaveContent.Assets.Models.asteroid_1_0_fbx;
                    break;
                case 1:
                    model = WaveContent.Assets.Models.asteroid_2_0_fbx;
                    break;
                case 2:
                    model = WaveContent.Assets.Models.asteroid_2_0_fbx;
                    break;
                default:
                    model = WaveContent.Assets.Models.asteroid_3_0_fbx;
                    break;
            }

            var asteroid = new Entity("asteroid-" + i)
            .AddComponent(new Transform3D())
            .AddComponent(new Model(model))
            .AddComponent(new MaterialsMap() { DefaultMaterialPath = WaveContent.Assets.Materials.asteroidMat })
            .AddComponent(new Spinner())
            .AddComponent(new SphereCollider3D())
            .AddComponent(new ModelRenderer());

            asteroid.IsVisible = false;

            return asteroid;
        }
    }
}
