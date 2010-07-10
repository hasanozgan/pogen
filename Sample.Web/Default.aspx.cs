using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Collections.Generic;

public partial class _Default : BasePage 
{
    public class MyDataItem
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }    
        }

        public MyDataItem(int id)
        {
            Id = id;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            List<MyDataItem> items = new List<MyDataItem>();
            items.Add(new MyDataItem(1));
            items.Add(new MyDataItem(2));
            items.Add(new MyDataItem(3));

            //GridView1.DataSource = items;
            //GridView1.DataBind();
        }
    }

    protected override void OnLoadComplete(EventArgs e)
    {
        Page.Title = _("Hello World");

        base.OnLoadComplete(e);
    }

    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        CurrentLanguage = DropDownList1.SelectedItem.Value;

        Response.Write(@"Verbatim String Orneki");
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DropDownList1.Items.Clear();
            DropDownList1.Items.Add(new ListItem(_("English"), _("en-US")));
            DropDownList1.Items.Add(new ListItem(_("Türkçe"), _("tr-TR")));

            DropDownList1.SelectedValue = _("en-US");
        }
    }
    
}
