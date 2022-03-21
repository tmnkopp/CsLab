using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace CyberScope.CS.Lab
{
    public class CBForm : System.Web.UI.HtmlControls.HtmlForm
    {
        protected bool _render; 
        public bool RenderFormTag
        {
            get { return _render; }
            set { _render = value; }
        } 
        public CBForm()
        { 
            _render = true;
        } 
        protected override void RenderBeginTag(HtmlTextWriter writer)
        { 
            if (_render)
                base.RenderBeginTag(writer);
        } 
        protected override void RenderEndTag(HtmlTextWriter writer)
        { 
            if (_render)
                base.RenderEndTag(writer);
        }
    }
}
