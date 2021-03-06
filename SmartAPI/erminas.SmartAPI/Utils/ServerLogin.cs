﻿// SmartAPI - .Net programmatic access to RedDot servers
//  
// Copyright (C) 2013 erminas GbR
// 
// This program is free software: you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with this program.
// If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Net;

namespace erminas.SmartAPI.Utils
{
    /// <summary>
    ///     Contains alls information needed to connect to a server. The name is not needed for the connection but can used for client side organisation of login information.
    /// </summary>
    public class ServerLogin
    {
        public ServerLogin()
        {
        }

        public ServerLogin(string url, PasswordAuthentication authData)
        {
            Address = new Uri(url);
            AuthData = authData;
        }

        /// <summary>
        ///     Address of the server. If you do not know the version of the RedDot server use
        ///     <see
        ///         cref="WebServiceURLProber.RqlUri" />
        ///     to get the correct address.
        /// </summary>
        public Uri Address { get; set; }

        /// <summary>
        ///     Authentication data (username/password)
        /// </summary>
        public PasswordAuthentication AuthData { get; set; }

        public Version ManualVersionOverride { get; set; }

        /// <summary>
        ///     (Optional) Name of the login.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// (Optional) Windows authentication information, only needed, when the cms server needs windows authentication before the cms login.
        /// </summary>
        public NetworkCredential WindowsAuthentication { get; set; }
    }
}