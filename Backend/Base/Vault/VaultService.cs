using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VaultSharp;

namespace VigilantMeerkat.Micro.Base.Vault
{
    public class VaultService
    {
        private readonly IVaultClient vaultClient;

        public VaultService(IVaultClient vaultClient)
        {
            this.vaultClient = vaultClient;
        }

        public async Task<string> GetKey(string path, string key)
        {
            return (await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path, mountPoint: "secret")).Data.Data[key].ToString();
        }
    }
}
