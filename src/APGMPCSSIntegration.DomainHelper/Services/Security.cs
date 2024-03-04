using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace APGMPCSSIntegration.DomainHelper.Services
{
    public class Security
    {
        private X509Certificate2 certificate, privateCert;
        private string hashAlgorithm;

        public Security(string fileName, string hashAlgorithm)
          : this(fileName, (string)null, hashAlgorithm)
        {
        }

        public Security(string fileName, string password, string hashAlgorithm)
        {
            this.certificate = string.IsNullOrWhiteSpace(password) ? new X509Certificate2(fileName) : new X509Certificate2(fileName, password);
            this.hashAlgorithm = hashAlgorithm;
            X509Store store = new X509Store(StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 cert in store.Certificates)
            {
                //Log.Information("Certificates Subject Name: " + cert.SubjectName.Name);
                this.privateCert = cert;
            }
        }

        public bool Verify(string messageStr, string signatureStr)
        {
            try
            {
                RSA rsa = this.certificate.GetRSAPublicKey();
                byte[] signature = Convert.FromBase64String(signatureStr);
                byte[] message = Encoding.UTF8.GetBytes(messageStr);
                bool isVerified = rsa.VerifyData(message, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                System.Diagnostics.Debug.WriteLine("isVerified: " + isVerified);
                return isVerified;
            }
            catch (Exception ex)
            {
                //Log.Information("ERROR in verify Signature: " + ex);
                return false;
            }
        }

        public string Sign(string message)
        {
            try
            {
                if (this.certificate.HasPrivateKey)
                {
                    RSA privatekey = this.certificate.GetRSAPrivateKey();
                    return Convert.ToBase64String(privatekey.SignData(Encoding.UTF8.GetBytes(message), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
                }
                else
                {
                    Console.WriteLine("No Privatekey found.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                //Log.Information("ERROR in loading certificates: " + ex);
                return null;
            }

        }
    }
}
