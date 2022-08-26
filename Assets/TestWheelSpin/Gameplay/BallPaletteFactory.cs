using System.Collections.Generic;
using System.Linq;
using TestWheelSpin.Gameplay.Settings;
using UnityEngine;

namespace TestWheelSpin.Gameplay
{
    public static class BallPaletteFactory
    {
        public static Color GetColor(List<BallsPalette> palettes, ColorId colorId)
        {
            BallsPalette palette = palettes.FirstOrDefault(bp=>bp.ColorId==colorId);
            if (palette == null)
            {
                Debug.Log($"Attention! Color {colorId} in palettes not found.");
                return Color.white;
            }

            return palette.Color;
        }
    }
}
