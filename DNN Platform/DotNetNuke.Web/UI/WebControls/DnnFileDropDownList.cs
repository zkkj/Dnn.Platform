﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.UI.WebControls.Extensions;

namespace DotNetNuke.Web.UI.WebControls
{
    [ToolboxData("<{0}:DnnFileDropDownList runat='server'></{0}:DnnFileDropDownList>")]
    public class DnnFileDropDownList : DnnDropDownList
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SelectItemDefaultText = Localization.GetString("DropDownList.SelectFileDefaultText", Localization.SharedResourceFile);
            Services.GetTreeMethod = "ItemListService/GetFiles";
            Services.SearchTreeMethod = "ItemListService/SearchFiles";
            Services.SortTreeMethod = "ItemListService/SortFiles";
            Services.ServiceRoot = "InternalServices";
            Services.Parameters.Add("parentId", Null.NullInteger.ToString());
            Options.ItemList.DisableUnspecifiedOrder = true;
        }

        protected override void OnPreRender(EventArgs e)
        {
            this.AddCssClass("file");
            base.OnPreRender(e);
        }

        /// <summary>
        /// Gets the selected Folder in the control, or selects the Folder in the control.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IFileInfo SelectedFile
        {
            get
            {
                var fileId = SelectedItemValueAsInt;
                return (fileId == Null.NullInteger) ? null : FileManager.Instance.GetFile(fileId);
            }
            set
            {
                SelectedItem = (value != null) ? new ListItem() { Text = value.FileName, Value = value.FileId.ToString(CultureInfo.InvariantCulture) } : null;
            }
        }

    }
}
