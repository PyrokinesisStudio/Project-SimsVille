﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace TSOVille.Code.Debug
{
    public partial class TSOVilleTools : Form
    {
        private TSOVilleUIInspector uiInspetor;
        private TSOSceneInspector sceneInspector;

        public TSOVilleTools()
        {
            InitializeComponent();

            /**
             * UI Inspector
             */
            uiInspetor = new TSOVilleUIInspector();
            uiInspetor.Show();

            sceneInspector = new TSOSceneInspector();
            sceneInspector.Show();


        }

        public void PositionAroundGame(GameWindow gameWindow)
        {

            this.Location = new System.Drawing.Point(gameWindow.ClientBounds.X - this.Width - 10, gameWindow.ClientBounds.Y);
            uiInspetor.Location = new System.Drawing.Point(
                gameWindow.ClientBounds.X - uiInspetor.Width - 10,
                gameWindow.ClientBounds.Y + this.Height + 10
            );
            sceneInspector.Location = new System.Drawing.Point(
                gameWindow.ClientBounds.X + gameWindow.ClientBounds.Width + 10,
                gameWindow.ClientBounds.Y
            );
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var window = new TSOVilleFindAssetSearch();
            window.StartSearch(txtFindAsset.Text);
            window.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var window = new TSOEdith();
            //window.Show();
        }
    }
}
