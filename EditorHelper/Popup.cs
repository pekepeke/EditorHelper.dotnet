using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;

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
        }

        public void AddMenu(String text, int value) {
            ToolStripMenuItem menu = new ToolStripMenuItem(text);
            this.GetMenu().Items.Add(menu);

            menu.Click += delegate(Object sender, EventArgs e) {
                this.index = value;
                this.selected = true;
            };
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

            //this.WaitClose();
            this.WaitCloseByGetMessage();

            this.contextMenu.Close();
            return this.index;
        }

        protected void WaitClose() {
            ContextMenuStrip menu = this.GetMenu();
            while (true) {
                Application.DoEvents();
                if (this.selected || !menu.Visible) {
                    break;
                }
            }
        }

        protected void WaitCloseByGetMessage() {
            ContextMenuStrip menu = this.GetMenu();
            MSG msg;
            while (GetMessage(out msg, 0, 0, 0).ToInt64() > 0) {
                DispatchMessage(out msg);
                //Application.DoEvents();
                if (this.selected || !menu.Visible) {
                    break;
                }
            }
        }

        public void DeleteMenu() {
            if (this.contextMenu != null) {
                this.contextMenu.Close();
                this.contextMenu.Dispose();
                this.contextMenu = null;
            }
        }
        #region P/Invoke

        private const int PM_NOREMOVE = 0;
        private const int PM_REMOVE = 1;

        private const int WM_PAINT = 0xF;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_MOUSELEAVE = 0x2A3;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PeekMessage(
            out MSG lpMsg,
            int hWnd,
            int wMsgFilterMin,
            int wMsgFilterMax,
            int wRemoveMsg
        );

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr DispatchMessage(
             out MSG lpMsg
        );

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetMessage(
            out MSG lpMsg,
            int hWnd,
            int wMsgFilterMin,
            int wMsgFilterMax
        );

        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public IntPtr hWnd;
            public int msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public int time;
            public POINTAPI pt;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTAPI
        {
            public int x;
            public int y;
        }

        #endregion
    }
}
