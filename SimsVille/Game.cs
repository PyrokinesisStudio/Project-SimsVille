/*
This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.
*/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using LogThis;
using FSO.Common.Rendering.Framework;
using FSO.LotView;
using FSO.Client.Network;
using FSO.Client.UI;
using FSO.Client.GameContent;
using FSO.Common;
using Microsoft.Xna.Framework.Audio;
using TSO.HIT;

namespace FSO.Client
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TSOGame : FSO.Common.Rendering.Framework.Game
    {
        public UILayer uiLayer;
        public _3DLayer SceneMgr;
        public SpriteFont Font, BigFont;
        public bool WindowChange = false;

		public TSOGame() : base()
        {
            GameFacade.Game = this;
            
            Content.RootDirectory = FSOEnvironment.GFXContentDir;
            WorldContent.Init(this.Services, Content.RootDirectory);
            Graphics.SynchronizeWithVerticalRetrace = true;
            
            Graphics.PreferredBackBufferWidth = GlobalSettings.Default.GraphicsWidth;
            Graphics.PreferredBackBufferHeight = GlobalSettings.Default.GraphicsHeight;
            
           // Graphics.HardwareModeSwitch = false;
            Graphics.ApplyChanges();

			Console.WriteLine(IsActive);

            //disabled for now. It's a hilarious mess and is causing linux to freak out.
            //Log.UseSensibleDefaults();
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            if (WindowChange || !GlobalSettings.Default.Windowed) return;
            if (Window.ClientBounds.Width == 0 || Window.ClientBounds.Height == 0) return;
            WindowChange = true;
            var width = Math.Max(1, Window.ClientBounds.Width);
            var height = Math.Max(1, Window.ClientBounds.Height);
            Graphics.PreferredBackBufferWidth = width;
            Graphics.PreferredBackBufferHeight = height;
            Graphics.ApplyChanges();

            GlobalSettings.Default.GraphicsWidth = width;
            GlobalSettings.Default.GraphicsHeight = height;

            WindowChange = false;
            if (uiLayer.CurrentUIScreen == null) return;

            uiLayer.SpriteBatch.ResizeBuffer(GlobalSettings.Default.GraphicsWidth, GlobalSettings.Default.GraphicsHeight);
            uiLayer.CurrentUIScreen.GameResized();
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            if (FSOEnvironment.DPIScaleFactor != 1 || FSOEnvironment.SoftwareDepth)
            {
                GlobalSettings.Default.GraphicsWidth = GraphicsDevice.Viewport.Width / FSOEnvironment.DPIScaleFactor;
                GlobalSettings.Default.GraphicsHeight = GraphicsDevice.Viewport.Height / FSOEnvironment.DPIScaleFactor;
            }

            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;
            GameFacade.Linux = (pid == PlatformID.MacOSX || pid == PlatformID.Unix);

            FSO.Content.Content.Init(GlobalSettings.Default.StartupPath, GraphicsDevice);
            base.Initialize();

            GameFacade.GameThread = Thread.CurrentThread;

            SceneMgr = new _3DLayer();
            SceneMgr.Initialize(GraphicsDevice);

            uiLayer = new UILayer(this, Font, BigFont);
            GameFacade.Controller = new GameController();
            GameFacade.Screens = uiLayer;
            GameFacade.Scenes = SceneMgr;
            GameFacade.GraphicsDevice = GraphicsDevice;
            GameFacade.GraphicsDeviceManager = Graphics;
            GameFacade.Cursor = new CursorManager(this.Window);
            if (!GameFacade.Linux) GameFacade.Cursor.Init(FSO.Content.Content.Get().GetPath(""));

            /** Init any computed values **/
            GameFacade.Init();

            //init audio now
            HITVM.Init();

            GameFacade.Strings = new ContentStrings();
            GameFacade.Controller.StartLoading();

            GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.None };

            try {
                var audioTest = new SoundEffect(new byte[2], 44100, AudioChannels.Mono); //initialises XAudio.
                audioTest.CreateInstance().Play();
            } catch (Exception e)
            {
                e = new Exception();
            }

            this.IsMouseVisible = true;
            this.IsFixedTimeStep = true;
            this.Window.AllowUserResizing = true;

            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);

           
            if (!FSOEnvironment.SoftwareKeyboard) AddTextInput();
            base.Screen.Layers.Add(SceneMgr);
            base.Screen.Layers.Add(uiLayer);
            GameFacade.LastUpdateState = base.Screen.State;

            //this.Window.Title = "FreeSO";

            if (!GlobalSettings.Default.Windowed && !GameFacade.GraphicsDeviceManager.IsFullScreen)
            {
                GameFacade.GraphicsDeviceManager.ToggleFullScreen();
            }
        }

        /// <summary>
        /// Only used on desktop targets. Use extensive reflection to AVOID linking on iOS!
        /// </summary>
        void AddTextInput()
        {
           this.Window.GetType().GetEvent("TextInput").AddEventHandler(this.Window, (EventHandler<TextInputEventArgs>)GameScreen.TextInput);
        }

        void RegainFocus(object sender, EventArgs e)
        {
            GameFacade.Focus = true;
        }

        void LostFocus(object sender, EventArgs e)
        {
            GameFacade.Focus = false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            
            Effect vitaboyEffect = null;

            Font = Content.Load<SpriteFont>("Fonts/SimsFont");
            BigFont = Content.Load<SpriteFont>("Fonts/SimsFontBig");


            GameFacade.MainFont = new FSO.Client.UI.Framework.Font();
            GameFacade.MainFont.AddSize(10, Font);
            GameFacade.MainFont.AddSize(12, Font);
            GameFacade.MainFont.AddSize(14, BigFont);
            GameFacade.MainFont.AddSize(16, BigFont);

            vitaboyEffect = Content.Load<Effect>("Effects/Vitaboy");
                
            

            FSO.Vitaboy.Avatar.setVitaboyEffect(vitaboyEffect);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
       
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            NetworkFacade.Client.ProcessPackets();

            if (HITVM.Get() != null) HITVM.Get().Tick();

            base.Update(gameTime);
        }
    }
}
