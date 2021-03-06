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
using System.Globalization;
using System.Xml;
using erminas.SmartAPI.CMS.Project.ContentClasses.Elements;
using erminas.SmartAPI.Utils;

namespace erminas.SmartAPI.CMS.Project.Pages.Elements
{
    public interface IStandardFieldDate : IStandardField<DateTime>
    {
    }

    /// <summary>
    ///     Standard field for dates. Takes input for SetValueFromString in the format yyyy-MM-dd.
    /// </summary>
    [PageElementType(ElementType.StandardFieldDate)]
    internal class StandardFieldDate : StandardField<DateTime>, IStandardFieldDate
    {
        private readonly DateTime BASE_DATE = new DateTime(1899, 12, 30);

        internal StandardFieldDate(IProject project, XmlElement xmlElement) : base(project, xmlElement)
        {
        }

        public StandardFieldDate(IProject project, Guid guid, ILanguageVariant languageVariant)
            : base(project, guid, languageVariant)
        {
        }

        public override void Commit()
        {
            using (new LanguageContext(LanguageVariant))
            {
                //TODO testen gegen _value == null und ob das ergebnis mit htmlencode richtig ist
                var value = _value.Date == default(DateTime)
                    ? RQL.SESSIONKEY_PLACEHOLDER
                    : _value.Date.Subtract(BASE_DATE).Days.ToString(CultureInfo.InvariantCulture);
                Project.ExecuteRQL(string.Format(SAVE_VALUE, Guid.ToRQLString(), value,
                                                 (int) ElementType));
            }
            //TODO check guid
            //xml
        }

        protected override DateTime FromString(string value)
        {
            try
            {
                return DateTime.Parse(value, CultureInfo.InvariantCulture);
            } catch (FormatException e)
            {
                throw new ArgumentException(string.Format("Invalid date value: {0}", value), e);
            }
        }

        protected override DateTime FromXmlNodeValue(string value)
        {
            return BASE_DATE.AddDays(int.Parse(value));
        }

        protected override string GetXmlNodeValue()
        {
            return Value == default(DateTime)
                       ? ""
                       : Value.Subtract(BASE_DATE).Days.ToString(CultureInfo.InvariantCulture);
        }

        protected override void LoadWholeStandardField()
        {
        }
    }
}