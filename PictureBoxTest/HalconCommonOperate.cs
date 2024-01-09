using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCoordinator
{
    public static class HalconCommonOperate
    {
        /// <summary>
        /// 将bmp转为
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="image"></param>
        public static void Bitmap2HObjectBpp24(Bitmap bmp, out HObject image)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData srcBmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                HOperatorSet.GenImageInterleaved(out image, srcBmpData.Scan0, "bgr", bmp.Width, bmp.Height, 0, "byte", 0, 0, 0, 0, -1, 0);
                bmp.UnlockBits(srcBmpData);
            }
            catch (Exception)
            {
                image = null;
            }
            finally
            {
                bmp.Dispose();
            }
        }
        /// <summary>
        /// HObject转为彩色图
        /// </summary>
        /// <param name="ho_image"></param>
        /// <param name="res24"></param>
        /// 
        public static void Bitmap2HObjectBpp8(Bitmap bmp, out HObject image)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData srcBmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
                HOperatorSet.GenImage1(out image, "byte", bmp.Width, bmp.Height, srcBmpData.Scan0);
                bmp.UnlockBits(srcBmpData);
            }
            catch (Exception)
            {
                image = null;
            }
            finally
            {
                bmp.Dispose();
            }
        }

        public static void HObject2Bpp24(HObject ho_image, out Bitmap res24)
        {

            //HOperatorSet.WriteImage(ho_image, "bmp", 0, @"E:\Dick\CASE\Type C R1.3&0.9\data\20200520\NG原图\CCD1\ex\xiaok1.bmp");
            HTuple width0, height0, type, width, height;
            //获取图像尺寸
            HOperatorSet.GetImageSize(ho_image, out width0, out height0);
            //创建交错格式图像
            HOperatorSet.InterleaveChannels(ho_image, out HObject InterImage, "argb", "match", 255);  //"rgb", 4 * width0, 0     "argb", "match", 255

            //获取交错格式图像指针
            HOperatorSet.GetImagePointer1(InterImage, out HTuple Pointer, out type, out width, out height);
            IntPtr ptr = Pointer;
            //构建新Bitmap图像
            Bitmap res32 = new Bitmap(width / 4, height, width, PixelFormat.Format32bppArgb, ptr);  // Format32bppArgb     Format24bppRgb


            //32位Bitmap转24位
            res24 = new Bitmap(res32.Width, res32.Height, PixelFormat.Format24bppRgb);
            Graphics graphics = Graphics.FromImage(res24);
            graphics.DrawImage(res32, new Rectangle(0, 0, res32.Width, res32.Height));

            res32.Dispose();
        }

        public static void HObject2Bpp8(HObject image, out Bitmap res)
        {
            HTuple hpoint, type, width, height;

            const int Alpha = 255;

            HOperatorSet.GetImagePointer1(image, out hpoint, out type, out width, out height);

            res = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            ColorPalette pal = res.Palette;
            for (int i = 0; i <= 255; i++)
            {
                pal.Entries[i] = Color.FromArgb(Alpha, i, i, i);
            }
            res.Palette = pal;
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = res.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            int PixelSize = Bitmap.GetPixelFormatSize(bitmapData.PixelFormat) / 8;

            IntPtr ptr1 = bitmapData.Scan0;
            IntPtr ptr2 = hpoint;
            int bytes = width * height;
            byte[] rgbvalues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr2, rgbvalues, 0, bytes);
            System.Runtime.InteropServices.Marshal.Copy(rgbvalues, 0, ptr1, bytes);
            res.UnlockBits(bitmapData);

        }
        /// <summary>
        /// 用2*2的系数对图片进行闭运算,并将结果保存到d:\Closing.bmp
        /// </summary>
        /// <param name="orginImage"></param>
        /// <returns></returns>
        public static Bitmap ImageToClosing(Bitmap orginImage)
        {

            try
            {
                Bitmap2HObjectBpp24(orginImage, out HObject in_image);
                HObject ho_Image, ho_Region, ho_RegionClosing;
                HObject ho_BinImage;
                HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
                // Initialize local and output iconic variables 
                HOperatorSet.GenEmptyObj(out ho_Image);
                HOperatorSet.GenEmptyObj(out ho_Region);
                HOperatorSet.GenEmptyObj(out ho_RegionClosing);
                HOperatorSet.GenEmptyObj(out ho_BinImage);
                ho_Image = in_image;
                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                ho_Region.Dispose();
                HOperatorSet.Threshold(ho_Image, out ho_Region, 128, 255);
                ho_RegionClosing.Dispose();
                HOperatorSet.ClosingRectangle1(ho_Region, out ho_RegionClosing, 2, 2);
                ho_BinImage.Dispose();
                HOperatorSet.RegionToBin(ho_RegionClosing, out ho_BinImage, 255, 0, hv_Width,
                    hv_Height);
                HOperatorSet.WriteImage(ho_BinImage, "bmp", 0, "d:/Closing.bmp");

                ho_Image.Dispose();
                ho_Region.Dispose();
                ho_RegionClosing.Dispose();
                ho_BinImage.Dispose();

                hv_Width.Dispose();
                hv_Height.Dispose();
                Bitmap map = new Bitmap(@"d:\Closing.bmp");
                return map;
            }
            catch (Exception)
            {
                return null;
            }


        }
    }
}
