using BlowFishCS;
using Library.Interface.IO;

namespace Library.IO {
    public class IOBlowFishCrypt : IIOCrypt {

        /// <summary>
        /// 加密密钥
        /// </summary>
        public string key = string.Empty;

        private static string IVBase64 = @"uf/3KW26Kl5th6EwMUZrW6zpjwSduIhebRT21Yt/E5p9GZsYMQ3srTNqhcJbuTOfAabOxmn3PXk5hiiLqi6kmxgFPveF+7gOdfejdLFj6NgL8ti7rNMbEn/Ee2GCH+aZ2FW+NC4us520R9yi5ROakgfK+5VYH6p0Ap87IUhVnprAnp32uKtxyj7Cys4Saqc25a9Qq/UnWkEC9+d+C5K4rrFWspk8Dt3ffokWt2cd4kvNqJbA77lfYvXtowIiTTSZPuYIWeZ6syXqAcBuhGmyyDQjrY7Zt8/0OvPXac+5ZIWcGRbWPPVDHQmj/JGWlPWsZN3SPq/oxkPevj/PBk5WXA==";

        private BlowFish blowFish = null;

        public IOBlowFishCrypt(string key) {
            this.key = key;
            blowFish = new BlowFish(key);
            blowFish.SetRandomIV();
            //blowFish.IV = System.Convert.FromBase64String(IVBase64);
            blowFish.NonStandard = true;
        }

        public byte[] Decrypted(byte[] data) {
            return blowFish.Decrypt_ECB(data);
        }

        public byte[] Encrypted(byte[] data) {
            return blowFish.Encrypt_ECB(data);
        }

    }
}
