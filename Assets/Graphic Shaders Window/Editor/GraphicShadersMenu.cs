using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LuviKunG.GraphicShaders
{
    public static class GraphicShadersMenu
    {
        [MenuItem("Window/LuviKunG/Graphic Shaders Management", false)]
        public static void OpenWindow()
        {
            _ = GraphicShadersWindow.OpenWindow();
        }
    }
}