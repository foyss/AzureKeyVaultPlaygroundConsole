﻿using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;

namespace AzureKeyVaultPlayground
{
    class Program
    {
        // This is the ID which can be found as "Application (client) ID" when selecting the registered app under "Azure Active Directory" -> "App registrations".
        const string APP_CLIENT_ID = "";

        // This is the client secret from the app registration process.
        const string APP_CLIENT_SECRET = "";

        // This is available as "DNS Name" from the overview page of the Key Vault.
        const string KEYVAULT_BASE_URI = "";
        
        // Secret name set on Azure
        const string KEYVAULT_SECRET_NAME = "";

        async static Task Main(string[] args)
        {
            Console.WriteLine("Azure KeyVault retrieve secret console");

            // Use the client SDK to get access to the key vault. To authenticate we use the identity app we registered and
            // use the client ID and the client secret to make our claim.
            var kvc = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(
                async (string authority, string resource, string scope) => {
                    var authContext = new AuthenticationContext(authority);
                    var credential = new ClientCredential(APP_CLIENT_ID, APP_CLIENT_SECRET);
                    AuthenticationResult result = await authContext.AcquireTokenAsync(resource, credential);

                    if (result == null)
                        throw new InvalidOperationException("Failed to retrieve JWT token");
                    
                    return result.AccessToken;
                }
            ));

            // Calling GetSecretAsync will trigger the authentication code above and eventually
            // retrieve the secret which we can then read.
            var secretBundle = await kvc.GetSecretAsync(KEYVAULT_BASE_URI, KEYVAULT_SECRET_NAME);
            
            Console.WriteLine("Secret:" + secretBundle.Value);
            Console.ReadKey();
        }
    }
}
