using System;
using System.Collections.Generic;
using System.Text;

namespace ORM
{
    public class TagNode
    {
        #region Constructors
        public TagNode(string Name, string Value)
        {
            this._strName = Name;
            this._strValue = Value;
        }

        public TagNode(string Name)
            : this(Name, String.Empty)
        {
        }
        #endregion

        #region Private variables
        private string _strName = String.Empty;
        private string _strValue = String.Empty;
        private List<TagNode> _children = new List<TagNode>();
        private TagAttributesCollection _attributes = new TagAttributesCollection();
        #endregion

        #region Public methods
        public void AddChildNode(TagNode child)
        {
            _children.Add(child);
        }

        public void AddAttribute(TagAttribute attribute)
        {
            _attributes.Add(attribute);
        }

        public void AddAttribute(string Name, string Value)
        {
            _attributes.Add(new TagAttribute(Name, Value));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<" + _strName + _attributes.ToString() + ">");
            foreach (TagNode child in _children)
            {
                sb.Append(child.ToString());
            }
            sb.Append(_strValue);
            sb.Append("</" + _strName + ">");
            return sb.ToString();
        }
        #endregion

        #region Properties
        public int ChildNodeCount
        {
            get { return _children.Count; }
        }
        public bool HasChildNodes
        {
            get { return ChildNodeCount > 0; }
        }

        public List<TagNode> ChildNodes
        {
            get
            {
                return _children;
            }
        }
        public string Name
        {
            get
            {
                return _strName;
            }
        }
        public string Value
        {
            get
            {
                return _strValue;
            }
        }
        #endregion
    }

    public class TagAttribute
    {
        #region Constructors
        public TagAttribute(string Name, string Value)
        {
            this._strName = Name;
            this._strValue = Value;
        }
        #endregion

        #region Private variables
        private string _strName = String.Empty;
        private string _strValue = String.Empty;
        #endregion

        #region Properties
        public string Name { get { return _strName; } }
        public string Value { get { return _strValue; } }
        #endregion
    }

    public class TagAttributesCollection
    {
        #region Private variables
        private List<TagAttribute> _coll = new List<TagAttribute>();
        private string _attributeTemplate = " {0}=\"{1}\"";
        #endregion

        #region Public methods
        public void Add(TagAttribute attribute)
        {
            _coll.Add(attribute);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (TagAttribute att in _coll)
            {
                sb.Append(String.Format(_attributeTemplate, att.Name, att.Value.Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;")));
            }
            return sb.ToString();
        }
        #endregion
    }
}
