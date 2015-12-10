using SharpEXR;
using SharpEXR.ColorSpace;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EXRViewer {
    class Program {
        static void Main() {
            var args = CommandLineArguments.Parse();

            var img = EXRFile.FromFile(args[0]);
            var part = img.Parts[0];
            part.Open(args[0]);

            var bmp = new Bitmap(part.DataWindow.Width, part.DataWindow.Height);
            var data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var destBytes = part.GetBytes(ImageDestFormat.BGR8, GammaEncoding.sRGB, data.Stride);
            Marshal.Copy(destBytes, 0, data.Scan0, destBytes.Length);
            bmp.UnlockBits(data);

            var floats = part.GetFloats(ChannelConfiguration.RGB, false, GammaEncoding.Linear, false);
            var alpha = part.FloatChannels["A"];
            part.Close();

            var frm = new Form();
            frm.BackgroundImage = bmp;
            frm.FormBorderStyle = FormBorderStyle.FixedDialog;
            frm.MaximizeBox = false;
            frm.ClientSize = new System.Drawing.Size(bmp.Width, bmp.Height);
            frm.ShowDialog();
        }
    }
}
