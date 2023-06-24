using Core.Crypt;
using Core.Interface.IO;

/********************************************************************
Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

Created:	    2018 ~ 2023
Filename: 	    IOTEACrypt.cs
Author:		    DaiLi.Ou
Descriptions:   使用XXTEA进行文件的加密解密.
*********************************************************************/
namespace Core.IO
{
    public class IOTEACrypt : IIOCrypt
    {
        /**
        * 代码 Dll 加解密策略
        *  方案一
        *  1. 资源先打包成 AssetBundle
        *  2. 使用AES256加解密数据生成AssetBundle文件
        *  
        *  方案二
        *  1. 资源无需打包成Asset Bundle
        *  2. 将代码源文件写入一个byte[]
        *  3. byte[] 使用XXTEA的方式加密
        *  4. 将byte[]写入成一个本地文件
        *  
        *  方案三
        *  1. 结合上诉两个方案
        *  2. 对代码源文件进行二次加密
        *  3. 先进行AES256加密，再进行XXTEA加密
        *  4. AES256/XXTEA解密秘钥，可跟随热更版本。请求服务器获取。
        *  
        *  方案四
        *  1. 结合上诉两个方案
        *  2. 对代码资产进行TEA加密
        *  3. 对游戏资产进行AES256加密
        *  4. AES256/XXTEA解密秘钥，可跟随热更版本。请求服务器获取。
        *  
        *  Tips: 
        *  PC端无需考虑解密性能问题。
        *  Mobile端需要测试下解密耗时。
        *  只进行代码源文件的二次加密时，预测解密耗时可忽略不计。
        * */

        private string password = string.Empty;
        public IOTEACrypt(string password)
        {
            this.password = password;
        }

        public byte[] Decrypted(byte[] data)
        {
            return XXTEA.Decrypt(data, password);
        }

        public byte[] Encrypted(byte[] data)
        {
            return XXTEA.Encrypt(data, password);
        }
    }
}