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
using System.Web;
using System.Xml;
using erminas.SmartAPI.Utils;

namespace erminas.SmartAPI.CMS.Project.Pages.Elements
{
    public interface IText : IValueElement<string>
    {
        string Description { get; }
    }

    internal abstract class Text : AbstractValueElement<String>, IText
    {
        private string _description;

        protected Text(IProject project, Guid guid, ILanguageVariant languageVariant)
            : base(project, guid, languageVariant)
        {
        }

        protected Text(IProject project, XmlElement xmlElement) : base(project, xmlElement)
        {
            LoadXml();
            IsInitialized = false;
        }

        public override void Commit()
        {
            string htmlEncodedValue = string.IsNullOrEmpty(_value)
                                          ? RQL.SESSIONKEY_PLACEHOLDER
                                          : HttpUtility.HtmlEncode(_value);

            const string SAVE_TEXT_VALUE =
                @"<ELT translationmode=""0"" extendedinfo="""" reddotcacheguid="""" action=""save"" guid=""{0}"" pageid=""{1}"" id="""" index="""" type=""{2}"">{3}</ELT>";
            Project.Select();
            Project.Session.ExecuteRQLRaw(SAVE_TEXT_VALUE.RQLFormat(this, Page.Id, (int) Type, htmlEncodedValue),
                                          RQL.IODataFormat.FormattedText);
        }

        public string Description
        {
            get { return LazyLoad(ref _description); }
        }

        protected override string FromString(string value)
        {
            return value;
        }

        protected override sealed string FromXmlNodeValue(string arg)
        {
            return null;
        }

        protected override sealed void LoadWholeValueElement()
        {
            LoadXml();

            using (new LanguageContext(LanguageVariant))
            {
                const string LOAD_VALUE = @"<ELT action=""load"" guid=""{0}"" extendedinfo=""""/>";
                Project.Select();
                _value = Project.Session.ExecuteRQLRaw(LOAD_VALUE.RQLFormat(this), RQL.IODataFormat.FormattedText);
            }
        }

        private void LoadXml()
        {
            InitIfPresent(ref _description, "reddotdescription", x => x);
        }
    }
}