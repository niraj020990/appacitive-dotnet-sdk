﻿using Appacitive.Sdk.Interfaces;
using Appacitive.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    internal static class AppacitiveContext
    {
        public static string ApiKey { get; internal set; }

        public static Environment Environment { get; internal set; }

        private static object _syncRoot = new object();
        private static string _sessionToken;

        public static string SessionToken
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_sessionToken) == true)
                {
                    lock (_syncroot)
                    {
                        if (string.IsNullOrWhiteSpace(_sessionToken) == true)
                        {
                            CreateSessionAsync().Wait();
                        }
                    }
                }
                return _sessionToken;
            }
        }

        public static string UserToken
        {
            get
            {
                var userContext = ObjectFactory.Build<IUserContext>();
                return userContext.GetUserToken();
            }
        }

        internal static string GlobalUserToken { get; set; }

        public static Geocode UserLocation { get; internal set; }

        public static IDependencyContainer ObjectFactory { get; internal set; }

        internal static void MarkSessionInvalid()
        {
            _sessionToken = null;
        }

        private static object _syncroot = new object();

        private static async Task CreateSessionAsync()
        {
            //TODO: Add validation and failure handling
            var request = new CreateSessionRequest() { ApiKey = AppacitiveContext.ApiKey };
            var service = ObjectFactory.Build<ISessionService>();
            var response = await service.CreateSessionAsync(request);
            _sessionToken = response.Session.SessionKey;
        }

        public static Verbosity Verbosity { get; set; }

        public static bool EnableDebugging { get; set; }
    }
}
