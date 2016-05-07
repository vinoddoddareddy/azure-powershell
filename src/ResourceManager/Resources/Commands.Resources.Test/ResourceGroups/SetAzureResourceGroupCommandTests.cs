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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Azure.Commands.Resources.Models;
using Microsoft.WindowsAzure.Commands.ScenarioTest;
using Microsoft.WindowsAzure.Commands.Test.Utilities.Common;
using Moq;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Azure.ServiceManagemenet.Common.Models;

namespace Microsoft.Azure.Commands.Resources.Test
{
    public class SetAzureResourceGroupCommandTests : RMTestBase
    {
        private SetAzureResourceGroupCommand cmdlet;

        private Mock<ResourcesClient> resourcesClientMock;

        private Mock<ICommandRuntime> commandRuntimeMock;

        private string resourceGroupName = "myResourceGroup";
        private string resourceGroupId = "/subscriptions/subId/resourceGroups/myResourceGroup";

        private List<Hashtable> tags;

        public SetAzureResourceGroupCommandTests(ITestOutputHelper output)
        {
            XunitTracingInterceptor.AddToContext(new XunitTracingInterceptor(output));
            resourcesClientMock = new Mock<ResourcesClient>();
            commandRuntimeMock = new Mock<ICommandRuntime>();
            cmdlet = new SetAzureResourceGroupCommand()
            {
                CommandRuntime = commandRuntimeMock.Object,
                ResourcesClient = resourcesClientMock.Object
            };

            tags = new [] {new Hashtable
                {
                    {"Name", "value1"},
                    {"Value", ""}
                }}.ToList();
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void UpdatesSetPSResourceGroupWithTag()
        {
            UpdatePSResourceGroupParameters expectedParameters = new UpdatePSResourceGroupParameters()
            {
                ResourceGroupName = resourceGroupName,
                Tag = tags.ToArray()
            };
            UpdatePSResourceGroupParameters actualParameters = new UpdatePSResourceGroupParameters();
            PSResourceGroup expected = new PSResourceGroup()
            {
                ResourceGroupName = expectedParameters.ResourceGroupName,
                Resources = new List<PSResource>() { new PSResource() { Name = "resource1" } },
                Tags = expectedParameters.Tag
            };
            resourcesClientMock.Setup(f => f.UpdatePSResourceGroup(It.IsAny<UpdatePSResourceGroupParameters>()))
                .Returns(expected)
                .Callback((UpdatePSResourceGroupParameters p) => { actualParameters = p; });

            cmdlet.Name = expectedParameters.ResourceGroupName;
            cmdlet.Tag = expectedParameters.Tag;

            cmdlet.ExecuteCmdlet();

            Assert.Equal(expectedParameters.ResourceGroupName, actualParameters.ResourceGroupName);
            Assert.Equal(expectedParameters.Tag, actualParameters.Tag);

            commandRuntimeMock.Verify(f => f.WriteObject(expected), Times.Once());
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void UpdatesSetPSResourceGroupWithTagFromId()
        {
            UpdatePSResourceGroupParameters expectedParameters = new UpdatePSResourceGroupParameters()
            {
                ResourceGroupName = resourceGroupName,
                Tag = tags.ToArray()
            };
            UpdatePSResourceGroupParameters actualParameters = new UpdatePSResourceGroupParameters();
            PSResourceGroup expected = new PSResourceGroup()
            {
                ResourceGroupName = expectedParameters.ResourceGroupName,
                Resources = new List<PSResource>() { new PSResource() { Name = "resource1" } },
                Tags = expectedParameters.Tag
            };
            resourcesClientMock.Setup(f => f.UpdatePSResourceGroup(It.IsAny<UpdatePSResourceGroupParameters>()))
                .Returns(expected)
                .Callback((UpdatePSResourceGroupParameters p) => { actualParameters = p; });

            cmdlet.Id = resourceGroupId;
            cmdlet.Tag = expectedParameters.Tag;

            cmdlet.ExecuteCmdlet();

            Assert.Equal(expectedParameters.ResourceGroupName, actualParameters.ResourceGroupName);
            Assert.Equal(expectedParameters.Tag, actualParameters.Tag);

            commandRuntimeMock.Verify(f => f.WriteObject(expected), Times.Once());
        }
    }
}
