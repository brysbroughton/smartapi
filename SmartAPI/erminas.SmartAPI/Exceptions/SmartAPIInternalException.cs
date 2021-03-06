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
using System.Runtime.Serialization;

namespace erminas.SmartAPI.Exceptions
{
    /// Class for internal errors. If this exception occurs, it probably is related to a bug in the SmartAPI
    /// and we would like to hear about it at https://github.com/erminas/smartapi or info@erminas.de.
    [Serializable]
    public class SmartAPIInternalException : Exception
    {
        public SmartAPIInternalException()
        {
        }

        public SmartAPIInternalException(string message) : base(message)
        {
        }

        public SmartAPIInternalException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SmartAPIInternalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}