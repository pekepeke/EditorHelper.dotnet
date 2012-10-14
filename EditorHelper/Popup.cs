using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

namespace EditorHelper
{
    [Guid("a4c64e56-aec2-4367-902b-f7085fab31f6")]
    interface IPopup
    {
        void CreateSubMenu(String text);
        void AddSubMenu(String text, int value);
        void AddMenu(String text, int value);
        int TrackMenu();
        void DeleteMenu();
    }

    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("EditorHelper.Popup")]
    [Guid("d880e614-e319-4013-8cae-32cb58bc6fbd")]
    public class Popup : IPopup
    {

        protected ContextMenuStrip contextMenu = null;
        protected ToolStripMenuItem currentSubMenu = null;
        List<String> menuTexts = new List<string>();
        int index = -1;
        bool selected = false;

        protected ContextMenuStrip GetMenu() {
            if (this.contextMenu == null) {
                this.contextMenu = new ContextMenuStrip();
            }
            return this.contextMenu;
        }

        public void CreateSubMenu(String text) {
            this.currentSubMenu = new ToolStripMenuItem(text);
            this.GetMenu().Items.Add(this.currentSubMenu);
        }

        public void AddSubMenu(String text, int value) {
            ToolStripMenuItem menu = new ToolStripMenuItem(text);
            menu.Click += delegate(Object sender, EventArgs e) {
                this.index = value;
                this.selected = true;
            };

            this.currentSubMenu.DropDownItems.AddRange(
                new ToolStripItem[] { menu }
            );
            this.menuTexts.Add(text);
        }

        public void AddMenu(String text, int value) {
            ToolStripMenuItem menu = new ToolStripMenuItem(text);
            this.GetMenu().Items.Add(menu);

            menu.Click += delegate(Object sender, EventArgs e) {
                this.index = value;
                this.selected = true;
            };

            this.menuTexts.Add(text);
        }

        public int TrackMenu() {
            if (this.contextMenu == null) {
                return -1;
            }
            //this.AddMenu("Cancel", -1);

            Application.DoEvents();
            this.GetMenu().Show(new System.Drawing.Point(Cursor.Position.X, Cursor.Position.Y));
            Application.DoEvents();
            this.GetMenu().Focus();
            Application.DoEvents();

            ContextMenuStrip menu = this.GetMenu();
            while (true) {
                Application.DoEvents();
                if (this.selected || !menu.Visible) {
                    break;
                }
            }

            this.contextMenu.Close();
            return this.index;
        }

        public void DeleteMenu() {
            if (this.contextMenu != null) {
                this.contextMenu.Close();
                this.contextMenu.Dispose();
                this.contextMenu = null;
            }
        }
    }
}
