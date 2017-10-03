﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Commands.Common.Authentication;
using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using Microsoft.Azure.Commands.Profile.Properties;
using Microsoft.Azure.Commands.ResourceManager.Common;
using Microsoft.Azure.Management.Internal.Resources;
using Microsoft.Azure.Management.Internal.Resources.Models;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.Profile.Default
{
    /// <summary>
    /// Cmdlet to get default options. 
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureRmDefault", DefaultParameterSetName = ResourceGroupParameterSet)]
    [OutputType(typeof(ResourceGroup))]
    public class GetAzureRMDefaultCommand : AzureRMCmdlet
    {
        private const string ResourceGroupParameterSet = "ResourceGroup";
        private bool resourceGroup;

        [Parameter(ParameterSetName = ResourceGroupParameterSet, Mandatory = false, HelpMessage = "Display Default Resource Group", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public SwitchParameter ResourceGroup
        {
            get { return resourceGroup; }
            set { resourceGroup = value; }
        }

        public override void ExecuteCmdlet()
        {
            IAzureContext context = AzureRmProfileProvider.Instance.Profile.DefaultContext;
            IResourceManagementClient client = AzureSession.Instance.ClientFactory.CreateCustomArmClient<ResourceManagementClient>(
                                    context.Environment.GetEndpointAsUri(AzureEnvironment.Endpoint.ResourceManager),
                                    AzureSession.Instance.AuthenticationFactory.GetServiceClientCredentials(context, AzureEnvironment.Endpoint.ResourceManager),
                                    AzureSession.Instance.ClientFactory.GetCustomHandlers());
            client.SubscriptionId = context.Subscription.Id;

            // If no parameters are specified, show all defaults
            if (!ResourceGroup)
            {
                if (context.ExtendedProperties.ContainsKey(Resources.DefaultResourceGroupKey))
                {
                    var defaultResourceGroup = client.ResourceGroups.Get(context.ExtendedProperties[Resources.DefaultResourceGroupKey]);
                    WriteObject(defaultResourceGroup);
                }
            }

            // If any parameters are specified, show only defaults with switch parameters set to true
            if (ResourceGroup)
            {
                if (context.ExtendedProperties.ContainsKey(Resources.DefaultResourceGroupKey))
                {
                    var defaultResourceGroup = client.ResourceGroups.Get(context.ExtendedProperties[Resources.DefaultResourceGroupKey]);
                    WriteObject(defaultResourceGroup);
                }
            }
        }
    }
}