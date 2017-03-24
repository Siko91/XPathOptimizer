using System;
using System.Linq;
using System.Xml.Linq;

namespace XPathOptimizer.Main
{
	public class XPathPartGenerator
	{
		private Func<XElement, string> replacer;

		public XPathPartGenerator(Func<XElement, string> replacer){
			this.replacer = replacer;
		}

		public string Replace (XElement el){
			return replacer(el);
		}

		public static XPathPartGenerator[] All(params string[] attributes)
		{
			return new [] { TagOnly (), StarWithIndex (), StarOnly () }
				.Concat (attributes.Select (a => TagWithAttribute (a)))
				.Concat (attributes.Select (a => StarWithAttribute (a)))
				.ToArray();
			
		}

		public static XPathPartGenerator TagWithAttribute(string attribute)
		{


			return new XPathPartGenerator ((el) => 
				{
					var attr = el.Attributes(attribute).SingleOrDefault();
					if (attr==null) 
						return null;
					
					return string.Format(@"{0}[@{1}='{2}']", el.Name.LocalName, attribute, attr.Value);
				});
		}

		public static XPathPartGenerator TagOnly()
		{
			return new XPathPartGenerator ((el) => el.Name.LocalName);
		}

		public static XPathPartGenerator StarWithIndex()
		{
			return new XPathPartGenerator ((el) => {
				var index = el.ElementsBeforeSelf ()
					.Where (e => e.Name.LocalName == el.Name.LocalName)
					.Count ();
				return "*[" + (index+1) + "]";
			});
		}


		public static XPathPartGenerator StarWithAttribute(string attribute)
		{
			return new XPathPartGenerator ((el) => 
				{
					var attr = el.Attributes(attribute).SingleOrDefault();
					if (attr==null) 
						return null;

					return string.Format(@"*[@{0}='{1}']", attribute, attr.Value);
				});
		}

		public static XPathPartGenerator StarOnly()
		{
			return new XPathPartGenerator ((el) => "*");
		}
	}
}

