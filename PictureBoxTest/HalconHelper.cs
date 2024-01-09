using Emgu.CV.Structure;
using Emgu.CV;
using System.Text;
using HalconDotNet;
using AnomalyDetectByHalcon;
using Microsoft.VisualBasic.Logging;

namespace TestCoordinator
{
    public static class HalconHelper
    {
        public static object obj = new object();

        private static AnomalyAndClassDetectByHalcon AnomalyByHalconOperate = new AnomalyAndClassDetectByHalcon();

        public static uint TryMatch(Image<Gray, byte> imageTmp, int iX, int iY, int iEdgeLength, float fMatchedThreshold, int iBinarizeThreshold, string strTemplateURI, int iMode
            , out Point pHalfWidthHeight, out Point pMatchedCenter
            , ref Image<Gray, Byte> imTmp, bool bCopyImg = false
            )
        {
            pMatchedCenter = new Point();
            pHalfWidthHeight = new Point();
            HTuple x = new HTuple(), y = new HTuple();
            HTuple templateId = new HTuple();
            HObject hoImage = new HObject();
            HOperatorSet.GenEmptyObj(out hoImage);
            try
            {
                if (iX == 0 || iY == 0)
                {
                    // Log.WriteLogByError("选区不能为边缘，无法匹配");
                    return 3;
                }
                else
                {

                    lock (obj)
                    {

                        using (var imageTemplate = new Image<Gray, byte>(strTemplateURI))
                        {
                            pHalfWidthHeight = new Point(imageTemplate.Size.Width / 2, imageTemplate.Size.Height / 2);
                        }
                        if (bCopyImg)
                        {
                            imTmp = imageTmp.Clone();
                        }
                        var halconTemplatePath = $"{Path.GetDirectoryName(strTemplateURI)}\\{Path.GetFileNameWithoutExtension(strTemplateURI)}.shm";
                        if (File.Exists(halconTemplatePath))
                        {
                            HOperatorSet.ReadShapeModel(halconTemplatePath, out templateId); // 读取模板
                        }
                        else
                        {
                            HOperatorSet.ReadImage(out var hObject, strTemplateURI);
                            using (hObject)
                            {
                                AnomalyByHalconOperate.create_shape_mode(hObject, out templateId);
                                HOperatorSet.WriteShapeModel(templateId, halconTemplatePath);  // 保存模版
                                hObject.Dispose();

                            }
                        }
                        StringBuilder sb = new StringBuilder();
                        using (templateId)
                        {
                            using (var bitmap = imageTmp.ToBitmap())
                            {
                                HalconCommonOperate.Bitmap2HObjectBpp8(bitmap, out hoImage);
                                AnomalyByHalconOperate.find_image_shape_mode(hoImage, out _, templateId, new HTuple(fMatchedThreshold),
                                    new HTuple(iY), new HTuple(iX), new HTuple(iEdgeLength), new HTuple(iEdgeLength), out x, out y);
                                if (x.Length <= 0 || y.Length <= 0)
                                {
                                    // Log.WriteLogBySystem("本区域匹配失败");
                                    return 2;
                                }
                                //var xArr = x.ToDArr();
                                //var yArr = y.ToDArr();
                                if (x[0].D > 0 && y[0] > 0)
                                {
                                    pMatchedCenter.X = (int)x[0].D;
                                    pMatchedCenter.Y = (int)y[0].D;
                                    sb.Append(pMatchedCenter.X + "," + pMatchedCenter.Y);
                                    sb.Append(Environment.NewLine);

                                    //MessageBox.Show("匹配成功,像素坐标:\r\n" + sb.ToString());
                                    // Log.WriteLogBySystem(" 匹配成功，像素坐标:" + sb.ToString());
                                    // hoImage.Dispose();
                                    return 0;
                                }
                                else
                                {
                                    // Log.WriteLogBySystem("本区域匹配失败");
                                    return 2;
                                }

                            }
                            //
                        }
                    }

                }
            }

            catch (Exception ex)
            {
                // Log.WriteLogByError(ex.Message.Replace("\\", "\\\\"));
                return 1;
            }
            finally
            {
                x.Dispose();
                y.Dispose();
                HOperatorSet.ClearShapeModel(templateId);
                templateId.Dispose();
                hoImage.Dispose();


            }
        }
    }
}

