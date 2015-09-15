using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.Particles;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics3D;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Sound;

namespace SpaceShipDemo
{
    /// <summary>
    /// Manager for the asteroids
    /// </summary>
    [DataContract]
    public class AsteroidManager : Behavior
    {
        /// <summary>
        /// The exploding time
        /// </summary>
        private const float GameOverTime = 3;

        /// <summary>
        /// The index
        /// </summary>
        private int asteroidIndex = 0;

        /// <summary>
        /// The spawned
        /// </summary>
        private bool isSpawned = false;

        /// <summary>
        /// The appear distance
        /// </summary>
        private const float appearDistance = 1000;

        /// <summary>
        /// The exploding
        /// </summary>
        private bool isGameOver;

        /// <summary>
        /// The remain time
        /// </summary>
        private float remainingGameOver;

        /// <summary>
        /// The remaining asteroid interval
        /// </summary>
        private float remainingAsteroidInterval;

        /// <summary>
        /// The asteroids
        /// </summary>
        private List<Entity> asteroids;

        /// <summary>
        /// Gets or sets the number of asteroids.
        /// </summary>
        /// <value>
        /// The number of asteroids.
        /// </value>
        [DataMember]
        public int NumberOfAsteroids { get; set; }

        /// <summary>
        /// Gets or sets the asteroid interval.
        /// </summary>
        /// <value>
        /// The asteroid interval.
        /// </value>
        [DataMember]
        public float AsteroidInterval { get; set; }

        /// <summary>
        /// Gets or sets the ship.
        /// </summary>
        /// <value>
        /// The ship.
        /// </value>
        [DataMember]
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Framework.Physics3D.SphereCollider3D" })]
        public string Ship { get; set; }

        /// <summary>
        /// Gets or sets the explosion.
        /// </summary>
        /// <value>
        /// The explosion.
        /// </value>
        [DataMember]
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Components.Particles.ParticleSystem3D" })]
        public string Explosion { get; set; }

        /// <summary>
        /// Gets or sets the asteroid spread.
        /// </summary>
        /// <value>
        /// The asteroid spread.
        /// </value>
        [DataMember]
        public int AsteroidSpread { get; set; }

        /// <summary>
        /// The ship entity
        /// </summary>
        private Entity shipEntity;

        /// <summary>
        /// The explosion entity
        /// </summary>
        private Entity explosionEntity;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsteroidManager"/> class.
        /// </summary>
        public AsteroidManager()
        {
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.remainingAsteroidInterval = this.AsteroidInterval;

            if (this.Ship != null)
            {
                this.shipEntity = this.EntityManager.Find(this.Ship);
            }

            if (this.Explosion != null)
            {
                this.explosionEntity = this.EntityManager.Find(this.Explosion);
            }
        }

        /// <summary>
        /// Creates the asteroids.
        /// </summary>
        private void CreateAsteroids()
        {
            this.asteroids = new List<Entity>();

            for (int i = 0; i < this.NumberOfAsteroids; i++)
            {
                var asteroid = this.CreateAsteroid(i);

                this.EntityManager.Add(asteroid);

                this.asteroids.Add(asteroid);
            }

            this.asteroidIndex = 0;

            this.isSpawned = true;
        }

        /// <summary>
        /// Creates the asteroid.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        private Entity CreateAsteroid(int i)
        {
            string model;

            // Randomize the asteroids
            switch (i % this.NumberOfAsteroids)
            {
                case 0:
                    model = WaveContent.Assets.Models.asteroid_1_0_fbx;
                    break;
                case 1:
                    model = WaveContent.Assets.Models.asteroid_2_0_fbx;
                    break;
                case 2:
                    model = WaveContent.Assets.Models.asteroid_3_0_fbx;
                    break;
                default:
                    model = WaveContent.Assets.Models.asteroid_4_0_fbx;
                    break;
            }

            // Creates the asteroid entity
            var asteroid = new Entity("asteroid-" + i)
            .AddComponent(new Transform3D())
            .AddComponent(new Model(model))
            .AddComponent(new MaterialsMap() { DefaultMaterialPath = WaveContent.Assets.Materials.asteroidMat })
            .AddComponent(new Spinner())
            .AddComponent(new SpawnBehavior())
            .AddComponent(new SphereCollider3D())
            .AddComponent(new ModelRenderer());
            asteroid.IsVisible = false;

            return asteroid;
        }

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            if (!this.isSpawned)
            {
                // Creates the asteroids the first frame
                this.CreateAsteroids();
                return;
            }

            if ((this.shipEntity == null) || (this.NumberOfAsteroids == 0))
            {
                return;
            }

            if (this.isGameOver)
            {
                // If the ship is exploding, updates the countdown.
                this.remainingGameOver -= (float)gameTime.TotalSeconds;

                if (remainingGameOver <= 0)
                {
                    this.isGameOver = false;
                    this.Reset();
                }
            }
            else
            {
                // Checks the collider of the ship
                var shipBounding = this.shipEntity.FindComponent<SphereCollider3D>().BoundingSphere;

                // Check collision
                foreach (var asteroid in this.asteroids)
                {
                    if (!asteroid.IsVisible)
                    {
                        continue;
                    }

                    var collider = asteroid.FindComponent<SphereCollider3D>();

                    if (collider.Intersects(ref shipBounding))
                    {
                        // If the ship collides with the asteroid, game over!
                        this.GameOver();
                    }
                }

                // Updates the remaining time for the next asteroid spawn.
                this.remainingAsteroidInterval -= (float)gameTime.TotalSeconds;
                if (this.remainingAsteroidInterval < 0)
                {
                    this.ShowAsteroid();

                    this.remainingAsteroidInterval += this.AsteroidInterval;
                }
            }
        }

        /// <summary>
        /// Shows the next asteroid.
        /// </summary>
        private void ShowAsteroid()
        {
            // Gets the current asteroid
            var asteroid = this.asteroids[this.asteroidIndex];

            asteroid.IsVisible = true;

            var shipTransform = this.shipEntity.FindComponent<Transform3D>();

            // Places it in front of the ship with a random displacement.
            var position = shipTransform.Position + (shipTransform.WorldTransform.Forward * appearDistance);

            position.X += WaveServices.Random.Next(-this.AsteroidSpread, this.AsteroidSpread);
            position.Y += WaveServices.Random.Next(-this.AsteroidSpread, this.AsteroidSpread);

            var transform = asteroid.FindComponent<Transform3D>();

            transform.Position = position;

            // Sets a random scale for the asteroid.
            transform.Scale = new Vector3(WaveServices.Random.Next(30, 80));

            // Sets a random spinning
            var spinner = asteroid.FindComponent<Spinner>();
            spinner.IncreaseX = WaveServices.Random.Next(-100, 100) * 0.01f;
            spinner.IncreaseY = WaveServices.Random.Next(-100, 100) * 0.01f;
            spinner.IncreaseZ = WaveServices.Random.Next(-100, 100) * 0.01f;

            // Start spawning animatoin
            asteroid.FindComponent<SpawnBehavior>().Spawn();

            // Updates the index
            this.asteroidIndex = (this.asteroidIndex + 1) % this.NumberOfAsteroids;
        }

        /// <summary>
        /// Game over event.
        /// </summary>
        private void GameOver()
        {
            // Sets game over state
            this.isGameOver = true;
            this.remainingGameOver = GameOverTime;

            if (this.shipEntity != null)
            {
                // Sets ship in game over state.
                this.shipEntity.FindComponent<ShipBehavior>().GameOver();
            }

            if (this.explosionEntity != null)
            {
                // Places a (big) explosion on the ship.
                this.explosionEntity.FindComponent<Transform3D>().Position = this.shipEntity.FindComponent<Transform3D>().Position;
                this.explosionEntity.FindComponent<ParticleSystem3D>().Emit = true;
                this.explosionEntity.FindChild("explosionSound").FindComponent<SoundEmitter3D>().Play();
            }
        }

        /// <summary>
        /// Resets the asteroid field.
        /// </summary>
        public void Reset()
        {
            this.isGameOver = false;

            // Hides all the asteroids
            foreach (var asteroid in this.asteroids)
            {
                asteroid.IsVisible = false;
            }

            // Resets the ship
            if (this.shipEntity != null)
            {
                this.shipEntity.FindComponent<ShipBehavior>().Reset();
            }

            // Hides the explosion
            if (this.explosionEntity != null)
            {
                this.explosionEntity.FindComponent<ParticleSystem3D>().Emit = false;
            }
        }
    }
}
