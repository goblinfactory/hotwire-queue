﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Configuration;

namespace Icodeon.Hotwire.Framework.DTOs
{
    public class ModuleConfigurationDTO : IModuleConfiguration
    {
        private List<IModuleEndpoint> _endpoints;

        public ModuleConfigurationDTO()
        {
            _endpoints = new List<IModuleEndpoint>();
        }

        public bool Debug { get; set; }
        public bool Active { get; set; }
        public bool ExclusiveUse { get; set; }
        public string RootServiceName { get; set; }
        public MethodValidation MethodValidation { get; set; }
        public List<IModuleEndpoint> Endpoints {
            get { return _endpoints; }
            set { _endpoints = value;  }
        }


        public void AddEndpoint(EndpointConfiguration endpoint)
        {
            _endpoints.Add(endpoint);
        }

        public string GetConfigurationSectionName()
        {
            return ConfigurationSectionName;
        }

        public string ConfigurationSectionName { get; set; }
    }
}
