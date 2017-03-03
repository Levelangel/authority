using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Management;
using System.Net.Mime;
using System.Runtime.Serialization.Formatters.Binary;


namespace RegLib
{
    /// <summary>
    /// 加密解密类
    /// </summary>
    public class EncryptDecrypt
    {
        /// <summary>
        /// 获得CUPID
        /// </summary>
        private static string GetCPUID()
        {
            try
            {
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();

                String strCpuID = null;
                foreach (ManagementObject mo in moc)
                {
                    strCpuID = mo.Properties["ProcessorId"].Value.ToString();
                    break;
                }
                return strCpuID;
            }
            catch
            {
                return "unknown";
            }
        }

        /// <summary>
        /// 获取主板ID
        /// </summary>
        private static string GetMotherBoardID()
        {
            try
            {
                ManagementClass mc = new ManagementClass("Win32_BaseBoard");
                ManagementObjectCollection moc = mc.GetInstances();
                string strID = null;
                foreach (ManagementObject mo in moc)
                {
                    strID = mo.Properties["SerialNumber"].Value.ToString();
                    break;
                }
                return strID;
            }
            catch
            {
                return "unknown";
            }
        }

        /// <summary>
        /// 获取网卡ID
        /// </summary>
        private static string GetMacID()
        {
            try
            {
                ManagementClass mc = new ManagementClass("win32_networkadapterconfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                string str = "";
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["ipenabled"] == true)
                    {
                        str = mo["macaddress"].ToString();
                    }
                }
                str = str.Replace(":", "");
                return str;
            }
            catch (Exception)
            {
                return "unknown";
            }
        }

        /// <summary>
        /// 获得本机ID
        /// </summary>
        public static string GetID()
        {
            string strCPUID = GetCPUID();
            string strMotherBoardID = GetMotherBoardID();
            string strMacID = GetMacID();
            string Temp = strCPUID + strMotherBoardID + strMacID;
            Temp = MD5(Temp);
            return Temp;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="pToEncrypt">需要加密的字符串</param>
        public static string MD5(string pToEncrypt)
        {
            byte[] result = Encoding.Default.GetBytes(pToEncrypt.Trim());
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "").ToUpper();
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="pToEncrypt">需要加密的字符串</param>
        /// <param name="pKey">密匙，长度为8</param>
        public static string DESEncrypt(string pToEncrypt, string pKey)
        {
            if (pKey.Length != 8)
            {
                return "Error";
            }
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);
                des.Key = Encoding.UTF8.GetBytes(pKey);
                des.IV = Encoding.UTF8.GetBytes(pKey);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();
                foreach (byte b in ms.ToArray())
                {
                    ret.AppendFormat("{0:X2}", b);
                }
                return ret.ToString();
            }
            catch (Exception)
            {
                return "Error";
            }
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="pToDecrypt">需要DES解密的字符串</param>
        /// <param name="pKey">DES密匙</param>
        public static string DESDecrypt(string pToDecrypt, string pKey)
        {
            if (pKey.Length != 8)
            {
                return "Error";
            }
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
                for (int x = 0; x < pToDecrypt.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }
                des.Key = ASCIIEncoding.UTF8.GetBytes(pKey);
                des.IV = ASCIIEncoding.UTF8.GetBytes(pKey);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Encoding.Default.GetString(ms.ToArray());  
            }
            catch (Exception)
            {
                return "Error";
            }
        }

        /// <summary>
        /// 生成注册文件
        /// </summary>
        /// <param name="RegInfo">注册信息</param>
        /// <param name="path">导出路径</param>
        public static bool ExportRegFile(clsRegInfo RegInfo, string path)
        {
            try
            {
                Stream fStream = new FileStream(Environment.GetEnvironmentVariable("TEMP",EnvironmentVariableTarget.Machine) + "\\tmpRegInfo.tmp", FileMode.Create, FileAccess.ReadWrite);
                BinaryFormatter binFormat = new BinaryFormatter();//创建二进制序列化器

                binFormat.Serialize(fStream, RegInfo);
                fStream.Seek(0, SeekOrigin.Begin);
                StringBuilder ret = new StringBuilder();
                while (fStream.Position != fStream.Length)
                {
                    ret.AppendFormat("{0:X2}", fStream.ReadByte());
                }
                string regInfo = ret.ToString();
                fStream.Close();
                File.Delete(Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.Machine) + "\\tmpRegInfo.tmp");
                string ID_8 = RegInfo.ID.Substring(0,8);
                regInfo = ID_8 + regInfo;
                regInfo = DESEncrypt(regInfo, ID_8);
                regInfo = ID_8 + regInfo;
                StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8);
                sw.Write(regInfo);
                sw.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 读取注册文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public static clsRegInfo LoadRegFile(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            string regInfo = sr.ReadToEnd();
            sr.Close();
            string ID_8 = regInfo.Substring(0, 8);
            regInfo = regInfo.Substring(8, regInfo.Length - 8);
            regInfo = DESDecrypt(regInfo, ID_8);
            if (regInfo.Substring(0, 8) != ID_8)
            {
                return null;
            }
            regInfo = regInfo.Substring(8, regInfo.Length - 8);
            byte[] reg = new byte[regInfo.Length/2];
            for (int i = 0; i < regInfo.Length/2; i++)
            {
                reg[i] = Convert.ToByte(regInfo.Substring(2*i, 2), 16);
            }
            BinaryFormatter binFormat = new BinaryFormatter();
            Stream sm = new MemoryStream(reg);
            clsRegInfo ret = (clsRegInfo)binFormat.Deserialize(sm);
            sm.Close();
            return ret;
        }

    }

}
