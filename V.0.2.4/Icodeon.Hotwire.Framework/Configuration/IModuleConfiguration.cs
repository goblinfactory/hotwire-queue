﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.MediaTypes;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public interface IModuleConfiguration : IConfigurationSection
    {
        bool Debug { get; set; }
        bool Active { get; set; }
        bool ExclusiveUse { get; set; }
        string RootServiceName { get; set; }
        MethodValidation MethodValidation { get; set; }
        List<IModuleEndpoint> Endpoints { get; }
        void AddEndpoint(EndpointConfiguration endpoint);
    }

}
