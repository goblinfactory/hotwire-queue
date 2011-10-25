﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.Security
{
    public class NoSecurityRequestAuthenticator : IAuthenticateRequest 
    {

        public void AuthenticateRequest(ParsedBody parsedBody,NameValueCollection headers,  string httpMethod, Modules.EndpointMatch endpointMatch)
        {
            return;
        }
    }
}
