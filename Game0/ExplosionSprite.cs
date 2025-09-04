using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game0
{
    public class ExplosionSprite
    {
        private Texture2D texture;

        private Vector2 position = new Vector2(-1, -1);

        private float stateTimer;

        /// <summary>
        /// Stores the animation state/frame of the explosion
        /// </summary>
        public short State { get; private set; }

        /// <summary>
        /// A method for defining the position of the explosion
        /// </summary>
        /// <param name="posX">the x position of the explosion</param>
        /// <param name="posY">the y position of the explosion</param>
        public void RegisterLocation(float posX, float posY) 
        {
            position = new Vector2(posX, posY);
        }

        /// <summary>
        /// Loads the content for the sprite
        /// </summary>
        /// <param name="content">the content manager with which to load the sprite</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("explosion");
        }

        /// <summary>
        /// Updates the state/frame of the explosion
        /// </summary>
        /// <param name="gameTime">the gametime</param>
        /// <param name="boomState">the state of the explosion as determined by the main game</param>
        public void Update(GameTime gameTime, BoomState boomState)
        {
            //If the explosion is going off and has not completed, progress the explosion
            if (boomState == BoomState.Active && State < 15)
            {
                stateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (stateTimer > 0.1f)
                {
                    stateTimer -= 0.1f;
                    State = (short)((State + 1) % 16);
                }
            }

            //If the explosion is reversing and has not completed, reverse the explosion
            else if (boomState == BoomState.Undoing && State >= 0) 
            {
                stateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (stateTimer > 0.1f)
                {
                    stateTimer -= 0.1f;
                    State = (short)((State - 1));
                }
            }
        }

        /// <summary>
        /// Draws the explosion
        /// </summary>
        /// <param name="gameTime">the gametime</param>
        /// <param name="spriteBatch">the sprite batch in which to draw the sprite</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Only draw if the state is valid
            if (position.X != -1 && State < 15 && State >= 0) 
            {
                spriteBatch.Draw(texture, position, new Rectangle((State % 4) * 400, (State / 4) * 400, 400, 400), Color.White, 0f, new Vector2(200, 200), 0.5f, SpriteEffects.None, 0);
            }
        }
    }
}
