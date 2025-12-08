/*
 * Copyright (c) 2025 Piotr Francug - HotCode
 * Copyright (c) 2024 Rick (rick 'at' gibbed 'dot' us)
 *
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 *
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 *
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SAM.Picker
{
    internal static class DarkMode
    {
        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(
            IntPtr hwnd,
            int attr,
            ref int attrValue,
            int attrSize
        );

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        public static bool UseImmersiveDarkMode(IntPtr handle, bool enabled)
        {
            if (IsWindows10OrGreater(17763))
            {
                var attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
                if (IsWindows10OrGreater(18985))
                {
                    attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;
                }
                int useImmersiveDarkMode = enabled ? 1 : 0;
                return DwmSetWindowAttribute(
                        handle,
                        attribute,
                        ref useImmersiveDarkMode,
                        sizeof(int)
                    ) == 0;
            }
            return false;
        }

        private static bool IsWindows10OrGreater(int build = -1)
        {
            return Environment.OSVersion.Version.Major >= 10
                && Environment.OSVersion.Version.Build >= build;
        }

        public static void ApplyDarkTheme(Form form)
        {
            UseImmersiveDarkMode(form.Handle, true);
            form.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            form.ForeColor = System.Drawing.Color.White;
            ApplyDarkThemeToControls(form.Controls);
        }

        public static void ApplyLightTheme(Form form)
        {
            UseImmersiveDarkMode(form.Handle, false);
            form.BackColor = System.Drawing.SystemColors.Control;
            form.ForeColor = System.Drawing.SystemColors.ControlText;
            ApplyLightThemeToControls(form.Controls);
        }

        private static void ApplyDarkThemeToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is ToolStrip toolStrip)
                {
                    toolStrip.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
                    toolStrip.ForeColor = System.Drawing.Color.White;
                    toolStrip.Renderer = new DarkToolStripRenderer();
                }
                else if (control is StatusStrip statusStrip)
                {
                    statusStrip.BackColor = System.Drawing.Color.FromArgb(0, 122, 204);
                    statusStrip.ForeColor = System.Drawing.Color.White;
                    statusStrip.Renderer = new DarkToolStripRenderer();
                }
                else if (control is ListView listView)
                {
                    listView.BackColor = System.Drawing.Color.FromArgb(37, 37, 38);
                    listView.ForeColor = System.Drawing.Color.White;
                }
                else if (control is TextBox textBox)
                {
                    textBox.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
                    textBox.ForeColor = System.Drawing.Color.White;
                }
                else
                {
                    control.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
                    control.ForeColor = System.Drawing.Color.White;
                }
                if (control.HasChildren)
                {
                    ApplyDarkThemeToControls(control.Controls);
                }
            }
        }

        private static void ApplyLightThemeToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is ToolStrip toolStrip)
                {
                    toolStrip.BackColor = System.Drawing.SystemColors.Control;
                    toolStrip.ForeColor = System.Drawing.SystemColors.ControlText;
                    toolStrip.Renderer = new ToolStripProfessionalRenderer();
                }
                else if (control is StatusStrip statusStrip)
                {
                    statusStrip.BackColor = System.Drawing.SystemColors.Control;
                    statusStrip.ForeColor = System.Drawing.SystemColors.ControlText;
                    statusStrip.Renderer = new ToolStripProfessionalRenderer();
                }
                else if (control is ListView listView)
                {
                    listView.BackColor = System.Drawing.SystemColors.Window;
                    listView.ForeColor = System.Drawing.SystemColors.WindowText;
                }
                else if (control is TextBox textBox)
                {
                    textBox.BackColor = System.Drawing.SystemColors.Window;
                    textBox.ForeColor = System.Drawing.SystemColors.WindowText;
                }
                else
                {
                    control.BackColor = System.Drawing.SystemColors.Control;
                    control.ForeColor = System.Drawing.SystemColors.ControlText;
                }
                if (control.HasChildren)
                {
                    ApplyLightThemeToControls(control.Controls);
                }
            }
        }
    }

    internal class DarkToolStripRenderer : ToolStripProfessionalRenderer
    {
        public DarkToolStripRenderer()
            : base(new DarkColorTable())
        {
            RoundedEdges = false;
        }
    }

    internal class DarkColorTable : ProfessionalColorTable
    {
        public override System.Drawing.Color ToolStripDropDownBackground =>
            System.Drawing.Color.FromArgb(45, 45, 48);
        public override System.Drawing.Color ImageMarginGradientBegin =>
            System.Drawing.Color.FromArgb(45, 45, 48);
        public override System.Drawing.Color ImageMarginGradientMiddle =>
            System.Drawing.Color.FromArgb(45, 45, 48);
        public override System.Drawing.Color ImageMarginGradientEnd =>
            System.Drawing.Color.FromArgb(45, 45, 48);
        public override System.Drawing.Color MenuBorder =>
            System.Drawing.Color.FromArgb(60, 60, 60);
        public override System.Drawing.Color MenuItemBorder =>
            System.Drawing.Color.FromArgb(60, 60, 60);
        public override System.Drawing.Color MenuItemSelected =>
            System.Drawing.Color.FromArgb(62, 62, 64);
        public override System.Drawing.Color MenuItemSelectedGradientBegin =>
            System.Drawing.Color.FromArgb(62, 62, 64);
        public override System.Drawing.Color MenuItemSelectedGradientEnd =>
            System.Drawing.Color.FromArgb(62, 62, 64);
        public override System.Drawing.Color MenuStripGradientBegin =>
            System.Drawing.Color.FromArgb(45, 45, 48);
        public override System.Drawing.Color MenuStripGradientEnd =>
            System.Drawing.Color.FromArgb(45, 45, 48);
        public override System.Drawing.Color ToolStripBorder =>
            System.Drawing.Color.FromArgb(45, 45, 48);
        public override System.Drawing.Color ToolStripGradientBegin =>
            System.Drawing.Color.FromArgb(45, 45, 48);
        public override System.Drawing.Color ToolStripGradientMiddle =>
            System.Drawing.Color.FromArgb(45, 45, 48);
        public override System.Drawing.Color ToolStripGradientEnd =>
            System.Drawing.Color.FromArgb(45, 45, 48);
        public override System.Drawing.Color ButtonSelectedBorder =>
            System.Drawing.Color.FromArgb(0, 122, 204);
        public override System.Drawing.Color ButtonSelectedGradientBegin =>
            System.Drawing.Color.FromArgb(62, 62, 64);
        public override System.Drawing.Color ButtonSelectedGradientMiddle =>
            System.Drawing.Color.FromArgb(62, 62, 64);
        public override System.Drawing.Color ButtonSelectedGradientEnd =>
            System.Drawing.Color.FromArgb(62, 62, 64);
        public override System.Drawing.Color ButtonPressedBorder =>
            System.Drawing.Color.FromArgb(0, 122, 204);
        public override System.Drawing.Color ButtonPressedGradientBegin =>
            System.Drawing.Color.FromArgb(0, 122, 204);
        public override System.Drawing.Color ButtonPressedGradientMiddle =>
            System.Drawing.Color.FromArgb(0, 122, 204);
        public override System.Drawing.Color ButtonPressedGradientEnd =>
            System.Drawing.Color.FromArgb(0, 122, 204);
        public override System.Drawing.Color ButtonCheckedGradientBegin =>
            System.Drawing.Color.FromArgb(0, 122, 204);
        public override System.Drawing.Color ButtonCheckedGradientMiddle =>
            System.Drawing.Color.FromArgb(0, 122, 204);
        public override System.Drawing.Color ButtonCheckedGradientEnd =>
            System.Drawing.Color.FromArgb(0, 122, 204);
        public override System.Drawing.Color SeparatorDark =>
            System.Drawing.Color.FromArgb(60, 60, 60);
        public override System.Drawing.Color SeparatorLight =>
            System.Drawing.Color.FromArgb(60, 60, 60);
    }
}
