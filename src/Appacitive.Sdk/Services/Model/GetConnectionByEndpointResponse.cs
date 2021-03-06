﻿using Appacitive.Sdk.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class GetConnectionByEndpointResponse : ApiResponse
    {
        [JsonProperty("connections")]
        public List<Connection> Connections { get; set; }

        [JsonProperty("paginginfo")]
        public PagingInfo PagingInfo { get; set; }

        public static GetConnectionByEndpointResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<GetConnectionByEndpointResponse>(bytes);
        }
    }
}
