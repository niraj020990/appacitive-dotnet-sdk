﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class UserFixture
    {

        [TestMethod]
        public async Task CreateUserAsyncTest()
        {
            var user = new User()
            {
                Username = "john.doe_" + Unique.String,                  // ensure unique user name
                Email = "john.doe@" + Unique.String + ".com",           // unique but useless email address
                Password = "p@ssw0rd",
                DateOfBirth = DateTime.Today.AddYears(-25),
                FirstName = "John",
                LastName = "Doe",
                Phone = "987-654-3210",
                Location = new Geocode(18, 19)
            };
            user.SetAttribute("attr1", "value1");
            await user.SaveAsync();
        }


        [TestMethod]
        public async Task FindAllUsersAsyncTest()
        {
            // Create a new user
            var newUser = await UserHelper.CreateNewUserAsync();
            // Get list of users
            var users = await User.FindAllAsync();
            users.ForEach(x => Console.WriteLine("id: {0} username: {1}",
                x.Id,
                x.Username));
        }

        [TestMethod]
        public async Task FindAllUsersWithQueryAsyncTest()
        {
            // Create a new user
            var newUser = await UserHelper.CreateNewUserAsync();
            // Get list of users
            var users = await User.FindAllAsync( Query.Property("username").IsEqualTo(newUser.Username).AsString() );
            Assert.IsTrue(users != null && users.Count == 1);
            Assert.IsTrue(users[0].Id == newUser.Id);
            users.ForEach(x => Console.WriteLine("id: {0} username: {1}",
                x.Id,
                x.Username));
        }
    }
}
