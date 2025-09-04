using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;

namespace Game0
{
    public enum BoomState
    {
        Before,
        Active,
        Done,
        Undoing
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private BoomState boomState = BoomState.Before;
        private float minePositionX;
        private float grassPositionX;
        private Vector2 startGrass;

        private Texture2D grass;
        private Texture2D mine;
        private HourglassSprite hourglass;
        private KnightSprite knight;
        private ExplosionSprite boom;
        private SpriteFont fontBlock;
        private SpriteFont fontBasic;

        private KeyboardState currentKeyboard;
        private KeyboardState pastKeyboard;

        /// <summary>
        /// Constructs the game
        /// </summary>
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Loads the content for all the sprites
        /// </summary>
        protected override void LoadContent()
        {
            //Loads the starting position of the mine
            minePositionX = GraphicsDevice.Viewport.Width - 200;

            //Determine position where grass should start being drawn
            startGrass = new Vector2(0, GraphicsDevice.Viewport.Height - 64);

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //Loads all textures used
            grass = Content.Load<Texture2D>("grass");
            mine = Content.Load<Texture2D>("explosive-thekingphoenix");
            hourglass = new HourglassSprite(GraphicsDevice);
            hourglass.LoadContent(Content);
            knight = new KnightSprite(GraphicsDevice);
            knight.LoadContent(Content);
            boom = new ExplosionSprite();
            boom.LoadContent(Content);

            //Loads the fonts used
            fontBlock = Content.Load<SpriteFont>("LibertinusKeyboard");
            fontBasic = Content.Load<SpriteFont>("LibertinusSans");
        }

        /// <summary>
        /// Update the states in the game
        /// </summary>
        /// <param name="gameTime">The gametime</param>
        protected override void Update(GameTime gameTime)
        {
            //initializes the keyboards
            pastKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();

            //exits the game on escape
            if (currentKeyboard.IsKeyDown(Keys.Escape))
                Exit();

            //caches the state of the explosion
            BoomState pastBoom = boomState;

            //determines the factor with which to move the grass
            grassPositionX = ((float)gameTime.TotalGameTime.TotalSeconds * 64) % 16;
            if (pastBoom == BoomState.Undoing) grassPositionX *= -1;

            //update the hourglass animation
            hourglass.Update(gameTime, boomState);

            //determine if explosion should be reversed
            if (pastBoom == BoomState.Done && pastKeyboard.IsKeyUp(Keys.Space) && currentKeyboard.IsKeyDown(Keys.Space))
                boomState = BoomState.Undoing;

            //updates animations/positions based on state of the explosion
            if (pastBoom == BoomState.Before) 
            {
                minePositionX -= (float)gameTime.ElapsedGameTime.TotalSeconds * 64;
                knight.Update(gameTime, pastBoom);
                if (knight.Position.X >= minePositionX) 
                {
                    boomState = BoomState.Active;
                    boom.RegisterLocation(knight.Position.X, knight.Position.Y);
                }
            }
            if (pastBoom == BoomState.Active) 
            {
                knight.Update(gameTime, pastBoom);
                boom.Update(gameTime, pastBoom);
                if (boom.State == 15)
                    boomState = BoomState.Done;
            }
            if (pastBoom == BoomState.Undoing)
            {
                boom.Update(gameTime, pastBoom);
                if (boom.State < 4)
                {
                    knight.Update(gameTime, pastBoom);
                    minePositionX += (float)gameTime.ElapsedGameTime.TotalSeconds * 64;
                }
            }

            //update base game
            base.Update(gameTime);
        }

        /// <summary>
        /// Renders the game
        /// </summary>
        /// <param name="gameTime">the gametime</param>
        protected override void Draw(GameTime gameTime)
        {
            //Clear the background
            GraphicsDevice.Clear(Color.LightSkyBlue);

            //start sprite batch
            _spriteBatch.Begin();

            //Draw the grass with the movement accounted for
            for (int i = 0; i < 4; i++) 
            {
                int x = -16;
                while (x < GraphicsDevice.Viewport.Width) 
                {
                    _spriteBatch.Draw(grass, startGrass + new Vector2(x - grassPositionX, i * 16), Color.LightGreen);
                    x += 16;
                }
            }

            //always draw the hourglass
            hourglass.Draw(gameTime, _spriteBatch);
            
            //only draw knight when explosion has not 'truly' started
            if(boom.State < 4)
                knight.Draw(gameTime, _spriteBatch);

            //only draw explosion when it is appropriate
            if(boomState == BoomState.Active || boomState == BoomState.Undoing)
                boom.Draw(gameTime, _spriteBatch);

            //only draw mine when it is appropriate
            if (boomState == BoomState.Before || (boomState == BoomState.Undoing && boom.State < 4))
            {
                _spriteBatch.Draw(mine, new Vector2(minePositionX, GraphicsDevice.Viewport.Height - 50), new Rectangle(64, 0, 32, 32), Color.White, 0f, new Vector2(16,16), 2f, SpriteEffects.None, 0);
            }

            //Always draw escape instruction
            _spriteBatch.DrawString(fontBasic, "Press \'esc\' to exit", new Vector2(GraphicsDevice.Viewport.Width - 200, 20), Color.Black);
            
            //Animate the title drawing sequence
            if (gameTime.TotalGameTime.TotalSeconds > 2) _spriteBatch.DrawString(fontBlock, "U", new Vector2(GraphicsDevice.Viewport.Width / 2 - 95, 150), Color.Black);
            if (gameTime.TotalGameTime.TotalSeconds > 2.5) _spriteBatch.DrawString(fontBlock, "N", new Vector2(GraphicsDevice.Viewport.Width / 2 - 45, 150), Color.Black);
            if (gameTime.TotalGameTime.TotalSeconds > 3) _spriteBatch.DrawString(fontBlock, "D", new Vector2(GraphicsDevice.Viewport.Width / 2 + 5, 150), Color.Black);
            if (gameTime.TotalGameTime.TotalSeconds > 3.5) _spriteBatch.DrawString(fontBlock, "O", new Vector2(GraphicsDevice.Viewport.Width / 2 + 55, 150), Color.Black);
            
            //only draw instructions for reversal when they work
            if (gameTime.TotalGameTime.TotalSeconds > 5 && boomState == BoomState.Done) 
            {
                _spriteBatch.DrawString(fontBasic, "Press \'space\' to reverse", new Vector2(GraphicsDevice.Viewport.Width / 2 - 110, 220), Color.Black);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
