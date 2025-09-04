using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using Microsoft.Xna.Framework.Input;

namespace Game0
{
    /// <summary>
    /// A class representing the hourglass sprite
    /// </summary>
    public class HourglassSprite
    {
        private Texture2D texture;

        private short state;
        private float stateTimer;

        private Vector2 position;

        /// <summary>
        /// Constructs the hourglass sprite based on the Graphics device dimension
        /// </summary>
        /// <param name="graphics">the graphics device on which to draw the sprite</param>
        public HourglassSprite(GraphicsDevice graphics)
        {
            position = new Vector2(graphics.Viewport.Width / 2, 96);
        }

        /// <summary>
        /// loads the content for the hourglass sprite
        /// </summary>
        /// <param name="content">the content manager with which to load the sprite</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("hourglass");
        }

        /// <summary>
        /// Updates the animation of the hourglass
        /// </summary>
        /// <param name="gameTime">The gametime</param>
        /// <param name="boomState">The state of the explosion</param>
        public void Update(GameTime gameTime, BoomState boomState)
        {
            //Update the timer
            stateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //update the animation state of the hourglass
            if (stateTimer > 0.4f)
            {
                if (boomState != BoomState.Undoing)
                    state = (short)((state + 1) % 15);
                else
                {
                    state--;
                    if (state == -1) state = 14;
                }
                stateTimer -= 0.4f;

            }
        }

        /// <summary>
        /// Draws the sprite
        /// </summary>
        /// <param name="gameTime">the gametime</param>
        /// <param name="spriteBatch">the sprite batch to draw the sprite in</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, new Rectangle(state * 42, 0, 42, 42), Color.White, 0f, new Vector2(21, 21), 2f, SpriteEffects.None, 0);
        }
    }
}
