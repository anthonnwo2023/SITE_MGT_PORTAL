using System;
using System.Collections.Generic;
using System.Text;

namespace Project.V1.Lib.Helpers.HTML.Table
{
    public static class HTMLTable
    {
        public class Initialize : HtmlBase, IDisposable
        {
            public Initialize(StringBuilder sb, string classAttributes = "", string id = "", Dictionary<string, string> properties = null) : base(sb)
            {
                Append("<table");
                AddOptionalAttributes(classAttributes, id, null, properties);
            }

            public void StartHead(string classAttributes = "", string id = "")
            {
                Append("<thead");
                AddOptionalAttributes(classAttributes, id);
            }

            public void EndHead()
            {
                Append("</thead>");
            }

            public void StartFoot(string classAttributes = "", string id = "")
            {
                Append("<tfoot");
                AddOptionalAttributes(classAttributes, id);
            }

            public void EndFoot()
            {
                Append("</tfoot>");
            }

            public void StartBody(string classAttributes = "", string id = "")
            {
                Append("<tbody");
                AddOptionalAttributes(classAttributes, id);
            }

            public void EndBody()
            {
                Append("</tbody>");
            }

            public void Dispose()
            {
                Append("</table>");
            }

            public Row AddRow(string classAttributes = "", string id = "")
            {
                return new Row(GetBuilder(), classAttributes, id);
            }
        }

        public class Row : HtmlBase, IDisposable
        {
            public Row(StringBuilder sb, string classAttributes = "", string id = "") : base(sb)
            {
                Append("<tr");
                AddOptionalAttributes(classAttributes, id);
            }

            public void Dispose()
            {
                Append("</tr>");
            }

            public void AddCell(string innerText, string classAttributes = "", string id = "", string colSpan = "", Dictionary<string, string> properties = null)
            {
                Append("<td");
                AddOptionalAttributes(classAttributes, id, colSpan, properties);
                Append($"{innerText}");
                Append("</td>");
            }
        }
    }

    public abstract class HtmlBase
    {
        private StringBuilder _sb;

        protected HtmlBase(StringBuilder sb)
        {
            _sb = sb;
        }

        public StringBuilder GetBuilder()
        {
            return _sb;
        }

        protected void Append(string toAppend)
        {
            _sb.Append(toAppend);
        }

        protected void AddOptionalAttributes(string className = "", string id = "", string colSpan = "", Dictionary<string, string> properties = null)
        {

            if (!string.IsNullOrEmpty(id))
            {
                _sb.Append($" id=\"{id}\"");
            }
            if (!string.IsNullOrEmpty(className))
            {
                _sb.Append($" class=\"{className}\"");
            }
            if (!string.IsNullOrEmpty(colSpan))
            {
                _sb.Append($" colspan=\"{colSpan}\"");
            }

            if (properties != null)
            {
                foreach (var property in properties)
                {
                    _sb.Append($" {property.Key} =\"{property.Value}\"");
                }
            }

            _sb.Append('>');
        }
    }
}
