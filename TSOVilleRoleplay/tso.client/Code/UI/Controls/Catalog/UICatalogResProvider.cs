﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSOVille.Code.UI.Controls.Catalog
{
    public interface UICatalogResProvider
    {
        Texture2D GetIcon(ulong id);
    }
}
