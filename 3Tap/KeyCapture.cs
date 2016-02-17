using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;

namespace ThreeTap
{
    class KeyCapture
    {
        private const int WM_KEYDOWN = 0x0100;
        private const int KF_REPEAT = 0x4000;
        private static TimeSpan ts1 = DateTime.Now.TimeOfDay;
        private static Queue<int> buffer = new Queue<int>(5);
        private static Queue<Keys> keys = new Queue<Keys>(3);
        private const int keyTrapMax = 32;
        private static int keyTrap = keyTrapMax;
        public static event EventHandler YubikeyDetected;
        public static event EventHandler<bool> YubikeyDetectionToggle;
        private static bool yubikeyDetectionToggle = true;

        public KeyCapture()
        {
            WinAPI.KeyboardTap += OnKeyboardTap;
            Start();
        }

        public bool Started { get; set; }

        protected static void OnYubikeyDetected(EventArgs e)
        {
            EventHandler handler = YubikeyDetected;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        protected static void OnYubikeyDetectionToggle(bool e)
        {
            EventHandler<bool> handler = YubikeyDetectionToggle;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        public void Start()
        {
            WinAPI.KeyboardCaptureStart();
            Started = true;
        }

        public void Stop()
        {
            Started = false;
            WinAPI.KeyboardCaptureStop();
        }

        private static IntPtr OnKeyboardTap(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                Keys key = (Keys)Marshal.ReadInt32(lParam);

                TimeSpan dt = DateTime.Now.TimeOfDay;
                bool yubi = false;

                if (keyTrap < keyTrapMax)
                {
                    keyTrap++;
                    //Return a dummy value to trap the keystroke
                    return (System.IntPtr)1;
                }

                TimeSpan ts = dt.Subtract(ts1);
                ts1 = dt;
                buffer.Enqueue(ts.Milliseconds);
                keys.Enqueue(key);

                int prtscr = 0;
                Keys prevKey = Keys.PrintScreen;
                foreach (Keys k in keys)
                {
                    if (k == prevKey && k == Keys.PrintScreen)
                    {
                        prtscr++;
                    }
                    prevKey = k;
                }
                if (prtscr > 2)
                {
                    yubikeyDetectionToggle = !yubikeyDetectionToggle;
                    OnYubikeyDetectionToggle(yubikeyDetectionToggle);
                    keys.Clear();
                }

                if (keys.Count >= 3)
                {
                    keys.Dequeue();
                }


                if (buffer.Count >= 5)
                {
                    yubi = true;
                    foreach (int freq in buffer)
                    {
                        if (freq > 20)
                        {
                            yubi = false;
                            break;
                        }
                    }
                    buffer.Dequeue();
                }
                if (yubi && yubikeyDetectionToggle)
                {
                    keyTrap = 6;
                    buffer.Clear();
                    keys.Clear();
                    OnYubikeyDetected(EventArgs.Empty);
                }
            }
            return WinAPI.NextEvent(nCode, wParam, lParam);
        }
    }
}