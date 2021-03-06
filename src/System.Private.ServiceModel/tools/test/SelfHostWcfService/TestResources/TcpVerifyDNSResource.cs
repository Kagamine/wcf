﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Security.Cryptography.X509Certificates;
using WcfTestBridgeCommon;

namespace WcfService.TestResources
{
    internal class TcpVerifyDNSResource : TcpResource
    {
        protected override string Address { get { return "tcp-VerifyDNS"; } }

        protected override string GetHost(ResourceRequestContext context)
        {
            return Environment.MachineName;
        }

        protected override Binding GetBinding()
        {
            NetTcpBinding binding = new NetTcpBinding() { PortSharingEnabled = false };
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

            return binding;
        }

        protected override void ModifyHost(ServiceHost serviceHost, ResourceRequestContext context)
        {
            // Ensure the https certificate is installed before this endpoint resource is used
            CertificateManager.InstallMyCertificate(context.BridgeConfiguration,
                                                    context.BridgeConfiguration.BridgeHttpsCertificate);

            string certThumbprint = HttpsResource.EnsureHttpsCertificateInstalled(context.BridgeConfiguration);

            serviceHost.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine,
                                                      StoreName.My,
                                                      X509FindType.FindByThumbprint,
                                                      certThumbprint);
        }
    }
}
