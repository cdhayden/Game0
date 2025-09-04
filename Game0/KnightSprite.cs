using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game0
{
    /// <summary>
    /// class representing a moving knight sprite
    /// </summary>
    public class KnightSprite
    {
        private Texture2D texture;

        private float positionTimer;

        private short state;
        private float stateTimer;

        /// <summary>
        /// The position of the knight on screen
        /// </summary>
        public Vector2 Position { get; private set; }


        /// <summary>
        /// Constructs a knight sprite
        /// </summary>
        /// <param name="graphics">graphics device on which to display the sprite</param>
        public KnightSprite(GraphicsDevice graphics)
        {
            Position = new Vector2(-40, graphics.Viewport.Height - 90);
        }

        /// <summary>
        /// loads content of the knight sprite
        /// </summary>
        /// <param name="content">the content manager with which to load the content</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("RUN");
        }

        /// <summary>
        /// Animates and moves the knight sprite based on gameTime and state of explosion
        /// </summary>
        /// <param name="gameTime">the gametime</param>
        /// <param name="boomState">state of the explosion</param>
        public void Update(GameTime gameTime, BoomState boomState)
        {
            //update animation state based on state of explosion
            stateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (stateTimer > 0.1f)
            {
                if (boomState == BoomState.Undoing)
                {
                    state--;
                    if (state == -1) state = 7;
                }
                else 
                {
                    state = (short)((state + 1) % 8);
                }
                stateTimer -= 0.1f;
            }

            //update position timer during appropriate explosion states
            if(boomState == BoomState.Before || boomState == BoomState.Undoing)
                positionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //update position based on position timer and explosion state
            if (positionTimer > 0.05f && boomState == BoomState.Before)
            {
                positionTimer -= 0.05f;
                Position += new Vector2(400 * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
            }
            else if (positionTimer > 0.05f && boomState == BoomState.Undoing) 
            {
                positionTimer -= 0.05f;
                Position -= new Vector2(400 * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
            }
        }

        /// <summary>
        /// Draws the sprite based on the gametime
        /// </summary>
        /// <param name="gameTime">the gametime</param>
        /// <param name="spriteBatch">the sprite batch in which to draw the knight</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, new Rectangle(state * 96, 0, 96, 84), Color.White, 0f, new Vector2(48, 42), 3f, SpriteEffects.None, 0);
        }
    }
}
