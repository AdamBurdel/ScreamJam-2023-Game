using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CandiceAIforGames.Data
{
    public class CandiceTypeCreator
    {
        List<string> header;
        List<string> variables;
        List<string> footer;
        string[] type;
        List<string> properties;
        List<string> constructor;
        string className;
        public CandiceTypeCreator(string className, Dictionary<string, CandiceTypeAttribute> attributes, bool hasConstructor)
        {
            header = new List<string>();
            variables = new List<string>();
            footer = new List<string>();
            properties = new List<string>();
            constructor = new List<string>();
            this.className = className;
            if (hasConstructor)
            {
                CreateConstructor(className, attributes);
            }
            CreateHeader(className);
            CreateVariables(attributes);
        }
        private void CreateConstructor(string className, Dictionary<string, CandiceTypeAttribute> attributes)
        {
            string header = "\tpublic " + className + "(";
            string variables = "";


            List<string> content = new List<string>();

            foreach (KeyValuePair<string, CandiceTypeAttribute> item in attributes)
            {
                CandiceTypeAttribute attribute = item.Value;
                variables += attribute.name + ",";
                header += attribute.type + " " + attribute.name + ",";
                content.Add("\t\tthis." + attribute.name + " = " + attribute.name + ";");
                string data = "\t\t\t" + attribute.type + " " + attribute.name + " = ";
                string node = "node.SelectSingleNode(\"" + attribute.name + "\").InnerText";
                string entry = "sqlDr[\"" + attribute.name + "\"]";


                string convert = "Convert.";
                if (attribute.type.Equals("string"))
                {
                    convert += "ToString(";
                    entry = convert + entry + ")";
                }
                else if (attribute.type.Equals("int"))
                {
                    convert += "ToInt32(";
                    node = convert + node + ")";
                    entry = convert + entry + ")";
                }
                else if (attribute.type.Equals("decimal"))
                {
                    convert += "ToDecimal(";
                    node = convert + node + ")";
                    entry = convert + entry + ")";
                }
                else if (attribute.type.Equals("float"))
                {
                    convert += "ToFloat(";
                    node = convert + node + ")";
                    entry = convert + entry + ")";
                }
                else if (attribute.type.Equals("double"))
                {
                    convert += "ToDouble(";
                    node = convert + node + ")";
                    entry = convert + entry + ")";
                }





            }
            header = header.Remove(header.Length - 1);
            header += ")";

            variables = variables.Remove(variables.Length - 1);

            constructor.Add(header);
            constructor.Add("\t{");
            constructor.AddRange(content);
            constructor.Add("\t}");

        }
        private void CreateHeader(string className)
        {
            header.Add("[Serializable]\npublic class " + className);
            header.Add("{");

            footer.Add("}");
        }
        private void CreateVariables(Dictionary<string, CandiceTypeAttribute> attributes)
        {
            foreach (KeyValuePair<string, CandiceTypeAttribute> item in attributes)
            {
                CandiceTypeAttribute attribute = item.Value;
                variables.Add("\t" + attribute.modifier + " " + attribute.type + " " + attribute.name + ";");
                if (attribute.hasGetter && attribute.hasSetter)
                {
                    CreateGetterAndSetter(attribute);
                }
                else
                {
                    if (attribute.hasGetter)
                    {
                        CreateGetter(attribute);
                    }
                    if (attribute.hasSetter)
                    {
                        CreateSetter(attribute);
                    }
                }

            }
        }
        void CreateGetterAndSetter(CandiceTypeAttribute attribute)
        {
            string name = attribute.name[0].ToString().ToUpper();
            for (int i = 1; i < attribute.name.Length; i++)
            {
                name += attribute.name[i];
            }
            properties.Add("\tpublic " + attribute.type + " " + name);
            properties.Add("\t{");
            properties.Add("\t\tget{return " + attribute.name + ";}");
            properties.Add("\t\tset{" + attribute.name + "=value;}");
            properties.Add("\t}");

        }
        void CreateGetter(CandiceTypeAttribute attribute)
        {
            string name = attribute.name[0].ToString().ToUpper();
            for (int i = 1; i < attribute.name.Length; i++)
            {
                name += attribute.name[i];
            }
            properties.Add("\tpublic " + attribute.type + " " + name);
            properties.Add("\t{");
            properties.Add("\t\tget{return " + attribute.name + ";}");
            properties.Add("\t}");

        }
        void CreateSetter(CandiceTypeAttribute attribute)
        {
            string name = attribute.name[0].ToString().ToUpper();
            for (int i = 1; i < attribute.name.Length; i++)
            {
                name += attribute.name[i];
            }
            properties.Add("\tpublic " + attribute.type + " " + name);
            properties.Add("\t{");
            properties.Add("\t\tset{" + attribute.name + "=value;}");
            properties.Add("\t}");
        }
        public int CreateType(string ds)
        {
            int rc = -1;
            string path = ds + className + ".cs";
            if (!File.Exists(path))
            {
                List<string> content = new List<string>();
                content.AddRange(header);
                content.AddRange(variables);
                content.AddRange(constructor);
                content.AddRange(properties);
                content.AddRange(footer);
                System.IO.File.WriteAllLines(path, content);
                rc = 0;
            }
            else
            {
                rc = 1;
            }
            return rc;
        }

    }

}
