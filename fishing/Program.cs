
using fishing;
using SlimDX.Direct3D9;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

public class Program
{
    public enum FishingState
    {
        NoFishing,
        WaitingForFishing,
        Fishing,
        TimeToCatch
    }

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);


    const UInt32 WM_KEYDOWN = 0x0100;
    const UInt32 WM_KEYUP = 0x0101;
    const UInt32 WM_LBUTTONDOWN = 0x201;
    const UInt32 WM_LBUTTONUP = 0x202;

    public struct Rect
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
    }

    private static int MAKELPARAM(int p, int p_2)
    {
        return ((p_2 << 16) | (p & 0xFFFF));
    }

    public static IntPtr GetApplicationPtr() { return Process.GetProcessesByName("eso64").First().MainWindowHandle; }


    public static void Delay(int milliseconds)
    {
        var random = new Random(DateTime.Now.Millisecond);
        Thread.Sleep(milliseconds + random.Next(-500, 500));
    }
    private static string lastStatus = string.Empty;

    public static void LogStatus(FishingState state)
    {
        LogStatus($"status - {state}");
    }
    public static void LogStatus(string message)
    {
        if (message != lastStatus)
        {
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: {message}");
            lastStatus = message;
        }
    }

    public static FishingState GetState()
    {
        Rect rect = new Rect();
        GetWindowRect(GetApplicationPtr(), ref rect);
        FishingState result = FishingState.NoFishing;

        using (Bitmap bmpScreenCapture = new Bitmap(1, 1))
        {
            using (Graphics g = Graphics.FromImage(bmpScreenCapture))
            {
                g.CopyFromScreen(rect.Left + 10,
                    rect.Bottom - 20,
                    0,
                    0,
                    bmpScreenCapture.Size,
                    CopyPixelOperation.SourceCopy);
            }

            if (bmpScreenCapture.GetPixel(0, 0).B == 255)
            {
                result = FishingState.WaitingForFishing;
            }
            if (bmpScreenCapture.GetPixel(0, 0).R == 255)
            {
                result = FishingState.Fishing;
            }
            if (bmpScreenCapture.GetPixel(0, 0).G == 255)
            {
                result = FishingState.TimeToCatch;
            }
        }

        return result;
    }
    static int noFishingCounter = 0;
    
 
    public static void Main(string[] args)
    {
        while (true)
        {
            Delay(1000);
            FishingState state = GetState();
            LogStatus(state);
            switch (state)
            {
                case FishingState.WaitingForFishing:
                    WaitingCycle();
                    break;
                case FishingState.Fishing:
                    noFishingCounter = 1;
                    FishingCycle();
                    break;
                case FishingState.NoFishing:
                    if (noFishingCounter > 5)
                    {
                        System.Media.SystemSounds.Hand.Play();
                        noFishingCounter = 0;
                    }
                    else if(noFishingCounter != 0)
                    {
                        noFishingCounter++;
                    }
                    break;
            }
        }
    }

    public static void PressKey(Keys key)
    {
        IntPtr ptr = GetApplicationPtr();

        SendMessage(ptr, WM_KEYDOWN, (IntPtr) key, IntPtr.Zero);
        SendMessage(GetApplicationPtr(), WM_KEYUP, (IntPtr)key, IntPtr.Zero);
        LogStatus($"{key} pressed");
    }
    public static void WaitingCycle()
    {
        LogStatus("WaitingCycle  started");
        while (true)
        {
            PressKey(Keys.E);
            Delay(1000);
            FishingState state = GetState();
            LogStatus(state);
            if (state == FishingState.Fishing || state == FishingState.NoFishing)
            {
                return;
            }
            
        }
    }

    public static void FishingCycle()
    {
        LogStatus("FishingCycle started");
        while (true)
        {
            Thread.Sleep(500);
            FishingState state = GetState();
            LogStatus(state);
            if (state == FishingState.TimeToCatch)
            {
                PressKey(Keys.E);
                Delay(2000);
                PressKey(Keys.R);
                Delay(2000);
                state = GetState();
            }
            if (state == FishingState.WaitingForFishing)
            {
                return;
            }

        }
    }

}