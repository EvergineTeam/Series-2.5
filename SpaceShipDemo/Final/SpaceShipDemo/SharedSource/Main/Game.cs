#region File Description
//-----------------------------------------------------------------------------
// Game
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace SpaceShipDemo
{
    /// <summary>
    /// The game class
    /// </summary>
    public class Game : WaveEngine.Framework.Game
    {
        /// <summary>
        /// Initializes the specified application.
        /// </summary>
        /// <param name="application">The application.</param>
        public override void Initialize(IApplication application)
        {
            base.Initialize(application);

            ScreenContext screenContext = new ScreenContext(new MyScene());
            WaveServices.ScreenContextManager.To(screenContext);
        }
    }
}
