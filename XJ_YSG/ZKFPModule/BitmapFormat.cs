using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace  ZWSB
{
    public class BitmapFormat
    {
        public struct BITMAPFILEHEADER
        {
            public ushort bfType;
            public int bfSize;
            public ushort bfReserved1;
            public ushort bfReserved2;
            public int bfOffBits;
        }

        public struct MASK
        {
            public byte redmask;
            public byte greenmask;
            public byte bluemask;
            public byte rgbReserved;
        }

        public struct BITMAPINFOHEADER
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }

        /*******************************************
        * 函数名称：RotatePic       
        * 函数功能：旋转图片，目的是保存和显示的图片与按的指纹方向不同     
        * 函数入参：BmpBuf---旋转前的指纹字符串
        * 函数出参：ResBuf---旋转后的指纹字符串
        * 函数返回：无
        *********************************************/
        public static void RotatePic(byte[] BmpBuf, int width, int height, ref byte[] ResBuf)
        {
            int RowLoop = 0;
            int ColLoop = 0;
            int BmpBuflen = width * height;

            try
            {
                for (RowLoop = 0; RowLoop < BmpBuflen; )
                {
                    for (ColLoop = 0; ColLoop < width; ColLoop++)
                    {
                        ResBuf[RowLoop + ColLoop] = BmpBuf[BmpBuflen - RowLoop - width + ColLoop];
                    }

                    RowLoop = RowLoop + width;
                }
            }
            catch (Exception ex)
            {
                //ZKCE.SysException.ZKCELogger logger = new ZKCE.SysException.ZKCELogger(ex);
                //logger.Append();
            }
        }

        /*******************************************
        * 函数名称：StructToBytes       
        * 函数功能：将结构体转化成无符号字符串数组     
        * 函数入参：StructObj---被转化的结构体
        *           Size---被转化的结构体的大小
        * 函数出参：无
        * 函数返回：结构体转化后的数组
        *********************************************/
        public static byte[] StructToBytes(object StructObj, int Size)
        {
            int StructSize = Marshal.SizeOf(StructObj);
            byte[] GetBytes = new byte[StructSize];

            try
            {
                IntPtr StructPtr = Marshal.AllocHGlobal(StructSize);
                Marshal.StructureToPtr(StructObj, StructPtr, false);
                Marshal.Copy(StructPtr, GetBytes, 0, StructSize);
                Marshal.FreeHGlobal(StructPtr);

                if (Size == 14)
                {
                    byte[] NewBytes = new byte[Size];
                    int Count = 0;
                    int Loop = 0;

                    for (Loop = 0; Loop < StructSize; Loop++)
                    {
                        if (Loop != 2 && Loop != 3)
                        {
                            NewBytes[Count] = GetBytes[Loop];
                            Count++;
                        }
                    }

                    return NewBytes;
                }
                else
                {
                    return GetBytes;
                }
            }
            catch (Exception ex)
            {
                //ZKCE.SysException.ZKCELogger logger = new ZKCE.SysException.ZKCELogger(ex);
                //logger.Append();

                return GetBytes;
            }
        }

        /*******************************************
        * 函数名称：GetBitmap       
        * 函数功能：将传进来的数据保存为图片     
        * 函数入参：buffer---图片数据
        *           nWidth---图片的宽度
        *           nHeight---图片的高度
        * 函数出参：无
        * 函数返回：无
        *********************************************/
        public static void GetBitmap(byte[] buffer, int nWidth, int nHeight, ref MemoryStream ms)
        {
            int ColorIndex = 0;
            ushort m_nBitCount = 8;
            int m_nColorTableEntries = 256;
            byte[] ResBuf = new byte[nWidth * nHeight * 2];
            try
            {
                BITMAPFILEHEADER BmpHeader = new BITMAPFILEHEADER();
                BITMAPINFOHEADER BmpInfoHeader = new BITMAPINFOHEADER();
                MASK[] ColorMask = new MASK[m_nColorTableEntries];

                int w = (((nWidth + 3) / 4) * 4);
                //图片头信息
                BmpInfoHeader.biSize = Marshal.SizeOf(BmpInfoHeader);
                BmpInfoHeader.biWidth = nWidth;
                BmpInfoHeader.biHeight = nHeight;
                BmpInfoHeader.biPlanes = 1;
                BmpInfoHeader.biBitCount = m_nBitCount;
                BmpInfoHeader.biCompression = 0;
                BmpInfoHeader.biSizeImage = 0;
                BmpInfoHeader.biXPelsPerMeter = 0;
                BmpInfoHeader.biYPelsPerMeter = 0;
                BmpInfoHeader.biClrUsed = m_nColorTableEntries;
                BmpInfoHeader.biClrImportant = m_nColorTableEntries;
                //文件头信息
                BmpHeader.bfType = 0x4D42;
                BmpHeader.bfOffBits = 14 + Marshal.SizeOf(BmpInfoHeader) + BmpInfoHeader.biClrUsed * 4;
                BmpHeader.bfSize = BmpHeader.bfOffBits + ((((w * BmpInfoHeader.biBitCount + 31) / 32) * 4) * BmpInfoHeader.biHeight);
                BmpHeader.bfReserved1 = 0;
                BmpHeader.bfReserved2 = 0;

                ms.Write(StructToBytes(BmpHeader, 14), 0, 14);
                ms.Write(StructToBytes(BmpInfoHeader, Marshal.SizeOf(BmpInfoHeader)), 0, Marshal.SizeOf(BmpInfoHeader));

                //调试板信息
                for (ColorIndex = 0; ColorIndex < m_nColorTableEntries; ColorIndex++)
                {
                    ColorMask[ColorIndex].redmask = (byte)ColorIndex;
                    ColorMask[ColorIndex].greenmask = (byte)ColorIndex;
                    ColorMask[ColorIndex].bluemask = (byte)ColorIndex;
                    ColorMask[ColorIndex].rgbReserved = 0;

                    ms.Write(StructToBytes(ColorMask[ColorIndex], Marshal.SizeOf(ColorMask[ColorIndex])), 0, Marshal.SizeOf(ColorMask[ColorIndex]));
                }

                //图片旋转，解决指纹图片倒立的问题
                RotatePic(buffer, nWidth, nHeight, ref ResBuf);

                byte[] filter = null;
                if (w - nWidth > 0)
                {
                    filter = new byte[w - nWidth];
                }
                for (int i = 0; i < nHeight; i++)
                {
                    ms.Write(ResBuf, i * nWidth, nWidth);
                    if (w - nWidth > 0)
                    {
                        ms.Write(ResBuf, 0, w - nWidth);
                    }
                }
            }
            catch (Exception ex)
            {
                // ZKCE.SysException.ZKCELogger logger = new ZKCE.SysException.ZKCELogger(ex);
                // logger.Append();
            }
        
        }


        /*******************************************
       * 函数名称：WriteBitmap       
       * 函数功能：将传进来的图片进行处理并返回
       * 函数入参：buffer---图片数据
       *           nWidth---图片的宽度
       *           nHeight---图片的高度
       * 函数出参：无
       * 函数返回：无
       *********************************************/
        public static byte[] GetImgData(string filePath, ref int size1)
        {
            FileStream file = new FileStream(filePath, FileMode.Open);
            long len = file.Seek(0, SeekOrigin.End);
            byte[] bWidth = new byte[5];
            byte[] bHeight = new byte[5];
            file.Seek(18, SeekOrigin.Begin);
            file.Read(bWidth, 0, 4);
            file.Read(bHeight, 0, 4);
            int width = (int)(bWidth[0] | bWidth[1] << 8 | bWidth[2] << 16 | bWidth[3] << 24);
            int height = (int)(bHeight[0] | bHeight[1] << 8 | bHeight[2] << 16 | bHeight[3] << 24);
            int w = (((width + 3) / 4) * 4);
            len -= 1078;
            byte[] byImgData = new byte[len];
            file.Seek(1078, SeekOrigin.Begin);
            file.Read(byImgData, 0, (int)len);
            file.Close();
            size1 = (int)len;
            byte[] byImgDataNew = new byte[len];
            //修改
            for (int i = height - 1; i >= 0; i--)
            {
                for (int j = 0; j < w; j++)
                {
                    byImgDataNew[(height - i - 1) * w + j] = byImgData[i * w + j];
                }
            }

            return byImgDataNew;
        }

        /*******************************************
        * 函数名称：WriteBitmap       
        * 函数功能：将传进来的数据保存为图片     
        * 函数入参：buffer---图片数据
        *           nWidth---图片的宽度
        *           nHeight---图片的高度
        * 函数出参：无
        * 函数返回：无
        *********************************************/
        public static void WriteBitmap(byte[] buffer, int nWidth, int nHeight)
        {
            int ColorIndex = 0;
            ushort m_nBitCount = 8;
            int m_nColorTableEntries = 256;
            byte[] ResBuf = new byte[nWidth * nHeight];

            try
            {

                BITMAPFILEHEADER BmpHeader = new BITMAPFILEHEADER();
                BITMAPINFOHEADER BmpInfoHeader = new BITMAPINFOHEADER();
                MASK[] ColorMask = new MASK[m_nColorTableEntries];
                int w = (((nWidth + 3) / 4) * 4);
                //图片头信息
                BmpInfoHeader.biSize = Marshal.SizeOf(BmpInfoHeader);
                BmpInfoHeader.biWidth = nWidth;
                BmpInfoHeader.biHeight = nHeight;
                BmpInfoHeader.biPlanes = 1;
                BmpInfoHeader.biBitCount = m_nBitCount;
                BmpInfoHeader.biCompression = 0;
                BmpInfoHeader.biSizeImage = 0;
                BmpInfoHeader.biXPelsPerMeter = 0;
                BmpInfoHeader.biYPelsPerMeter = 0;
                BmpInfoHeader.biClrUsed = m_nColorTableEntries;
                BmpInfoHeader.biClrImportant = m_nColorTableEntries;

                //文件头信息
                BmpHeader.bfType = 0x4D42;
                BmpHeader.bfOffBits = 14 + Marshal.SizeOf(BmpInfoHeader) + BmpInfoHeader.biClrUsed * 4;
                BmpHeader.bfSize = BmpHeader.bfOffBits + ((((w * BmpInfoHeader.biBitCount + 31) / 32) * 4) * BmpInfoHeader.biHeight);
                BmpHeader.bfReserved1 = 0;
                BmpHeader.bfReserved2 = 0;

                Stream FileStream = File.Open("finger.bmp", FileMode.Create, FileAccess.Write);
                BinaryWriter TmpBinaryWriter = new BinaryWriter(FileStream);

                TmpBinaryWriter.Write(StructToBytes(BmpHeader, 14));
                TmpBinaryWriter.Write(StructToBytes(BmpInfoHeader, Marshal.SizeOf(BmpInfoHeader)));

                //调试板信息
                for (ColorIndex = 0; ColorIndex < m_nColorTableEntries; ColorIndex++)
                {
                    ColorMask[ColorIndex].redmask = (byte)ColorIndex;
                    ColorMask[ColorIndex].greenmask = (byte)ColorIndex;
                    ColorMask[ColorIndex].bluemask = (byte)ColorIndex;
                    ColorMask[ColorIndex].rgbReserved = 0;

                    TmpBinaryWriter.Write(StructToBytes(ColorMask[ColorIndex], Marshal.SizeOf(ColorMask[ColorIndex])));
                }

                //图片旋转，解决指纹图片倒立的问题
                RotatePic(buffer, nWidth, nHeight, ref ResBuf);

                //写图片
                //TmpBinaryWriter.Write(ResBuf);
                byte[] filter = null;
                if (w - nWidth > 0)
                {
                    filter = new byte[w - nWidth];
                }
                for (int i = 0; i < nHeight; i++)
                {
                    TmpBinaryWriter.Write(ResBuf, i * nWidth, nWidth);
                    if (w - nWidth > 0)
                    {
                        TmpBinaryWriter.Write(ResBuf, 0, w - nWidth);
                    }
                }

                FileStream.Close();
                TmpBinaryWriter.Close();
            }
            catch (Exception ex)
            {
                //ZKCE.SysException.ZKCELogger logger = new ZKCE.SysException.ZKCELogger(ex);
                //logger.Append();
            }
        }
    }
}
