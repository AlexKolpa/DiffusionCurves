using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DiffusionCurves.Diffusion;
using DiffusionCurves.Model;
using DiffusionCurves.OpenGL;
using DiffusionCurves.Views;
using OpenTK.Graphics.OpenGL;

namespace DiffusionCurves.Export
{
    public class DiffusionExport
    {
        RenderState renderState;
        DiffusionBuffers buffers;
        RenderStateControl renderControl;
        IDiffusionRenderer renderer;
        DiffusionPathControl pathControl;
        FramesContainer framesContainer;

        ImageFormat[] formats = new ImageFormat[] { ImageFormat.Bmp, ImageFormat.Jpeg, ImageFormat.Gif, ImageFormat.Tiff, ImageFormat.Png };
        
        public DiffusionExport(IDiffusionRenderer renderer, RenderState renderState, RenderStateControl renderControl,
            DiffusionPathControl pathControl, DiffusionBuffers buffers, FramesContainer framesContainer)
        {
            this.renderState = renderState;
            this.buffers = buffers;
            this.renderControl = renderControl;
            this.renderer = renderer;
            this.pathControl = pathControl;
            this.framesContainer = framesContainer;         
        }

        public static String GetImageTypeFilter()
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();

            String filter = "";

            for (int i = 0; i < encoders.Length; i++)
            {
                filter += encoders[i].FormatDescription.ToLower() +
                    " image (" + encoders[i].FilenameExtension.ToLower().Replace(";", ", ") + ")|" +
                    encoders[i].FilenameExtension.ToLower() + "|";
            }

            return filter.Remove(filter.Length - 1);
        }

        public void Export(String fileFolder, int fileTypeIndex)
        {
            String extension = System.IO.Path.GetExtension(fileFolder);
            String fileName = fileFolder.Substring(0, fileFolder.Length - extension.Length);            

            GL.Viewport(renderState.ViewSize);
            GL.Enable(EnableCap.DepthTest);

            DiffusionBuffers originalBuffers = renderer.Buffers;
            RenderState originalState = renderer.RenderState;
            renderer.RenderState = renderState;

            int index = 1;
            foreach (Frame frame in framesContainer.FramesList)
            {
                pathControl.SetContainer(frame.Curves);

                renderer.Buffers = buffers;

                int texture = renderer.DrawDiffusion();

                if (!GL.IsTexture(texture))
                    continue;

                Bitmap frameBitmap = GetTexture(texture);

                String fullname = fileName + " (" + index++ + ")" + extension;
                frameBitmap.Save(fullname, formats[fileTypeIndex]);

                frameBitmap.Dispose();
            }
            renderer.Buffers = originalBuffers;
            renderer.RenderState = originalState;

            GL.Disable(EnableCap.DepthTest);
            GL.Viewport(originalState.ViewSize);
        }

        String GetExtension(ImageCodecInfo codecInfo)
        {
            String firstExtension = codecInfo.FilenameExtension.Remove(codecInfo.FilenameExtension.IndexOf(";"));

            return firstExtension.Substring(1);
        }

        public void SetFrameSize(Size frameSize)
        {
            renderState.ViewSize = frameSize;
            renderControl.SetFrameSize(frameSize);
        }

        /// <summary>
        /// Retrieves the currently bound OpenGL texture as a bitmap
        /// </summary>
        /// <returns></returns>
        private Bitmap GetTexture(int texture)
        {
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.BindBuffer(BufferTarget.PixelPackBuffer, 0);

            int pixelByteCount = 4;

            byte[] pixels = new byte[renderState.ViewSize.Width * renderState.ViewSize.Height * pixelByteCount];
            GL.GetTexImage<byte>(TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, pixels);
            IntPtr pixelPtr = Marshal.UnsafeAddrOfPinnedArrayElement(pixels, 0);

            Bitmap bitmap = new Bitmap(renderState.ViewSize.Width,
                renderState.ViewSize.Height,
                pixelByteCount * renderState.ViewSize.Width, 
                System.Drawing.Imaging.PixelFormat.Format32bppArgb, 
                pixelPtr);

            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bitmap;
        }
    }
}
