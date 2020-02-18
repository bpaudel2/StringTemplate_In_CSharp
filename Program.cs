using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.StringTemplate;

namespace StringTemplateExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            Program App = new Program();
            //Hello World :String Template 
            Template hello = new Template("Hello, <name>");
            hello.Add("name", "World");
            Console.WriteLine(hello.Render());

            /*
             The primary classes of interest are ST, STGroupDir, STGroupFile. 
             You can directly create a template in code, you can load templates 
             from a directory, and you can load a file containing a collection of 
             templates (a template group file). Group files behave like zips or jars 
             of template directories.
            */
            TemplateGroup group = new TemplateGroupDirectory(@"C:\Users\bpaudel\source\repos\StringTemplate\StringTemplate\Template");
            Template st = group.GetInstanceOf("decl");
            st.Add("type", "int");
            st.Add("name", "x");
            st.Add("value", 0);
            Console.WriteLine(st.Render());

            /*
             If you would like to keep just the template text and not the formal template 
             definition around the template text, you can use STRawGroupDir. Then, decl.st 
             would hold just the following. 
             <type> <name><init(value)>;
             Templates are dynamic scoping, that means that a template can reference the attributes of 
             any invoking template. 
             Sometimes it's more convenient to collect templates together into a single unit called 
             group file. For example, we can collect the definitions in the separate .st files into an 
             equivalent .stg group
            */

            TemplateGroup tgroup = new TemplateGroupFile(@"C:\Users\bpaudel\source\repos\StringTemplate\StringTemplate\Template\test.stg");
            Template t = group.GetInstanceOf("decl");
            t.Add("type", "int");
            t.Add("name", "x");
            t.Add("value", 0);
            Console.WriteLine(t.Render());

            App.AccessingPropertiesOfModelObject();
            App.AggregateAttributes();
            App.FormattingWithStringTemplate();

            Console.ReadLine();

        }



        /*
         Template expression can access the properties of objects injected from the model. 
        */
        public void AccessingPropertiesOfModelObject()
        {
            //We can inject instances of User just like predefined objects like strings 
            //can refer to properties using .
            /*
             u.id evaluates to the field of the injected User object whereas 
             u.name evaluates the getter for the field name.
            */
             
            Template st = new Template("<b>$u.id$</b>: $u.name$", '$', '$');
            st.Add("u", new User(999, "parrt"));
            Console.WriteLine(st.Render());

        }

        /*
         StringTemplate makes it easy to group data during Add() calls.
         For exp: a.{p1,p2,p3} describes an attribut called a that has three properties p1, p2 and p3.
        */
        public void AggregateAttributes()
        {
            Template st = new Template("<items:{it|<it.id>: <it.lastName>, <it.firstName>\n}>");
            st.AddMany("items.{ firstName ,lastName, id }", "Ter", "Parr", 99); // add() uses varargs
            st.AddMany("items.{firstName, lastName ,id}", "Tom", "Burns", 34);
            Console.WriteLine(st.Render());
        }


        /*
         Applying Templates to Attributes:
         StringTemplate does not distinguished between single and multivalued attributes. For exp: 
         if we add attribute name with value "parrt" to template <name>, it renders to parrt. if we call add() 
         twice, adding values "parrt" and "tombu" to name, it renders "parrttombu".
         In other words, multi-valued attributes render to the concatenation of the string values of the elements. 
         To insert a separator, we can use the separator option: <name; separator=", ">.
         To alter the output emitted for each element, we need to iterate across them. 

         StringTemplate has no foreach statement. Instead, we apply templates to attributes. For exp, 
         to surround each name with square brackets, we can define a bracket template and apply it to the names

        ##############################
        test(name) ::= "<name:bracket()>"  //apply bracket template to each name
        bracket(x) ::= "[<x>]"             //surround parameter with square brackets
        ############################## 
        test(name) ::= "<name:bracket(); separator=\", \">"
        bracket(x) ::= "[<x>]"

        StringTemplate is dynamically typed in the sense that it does not care about the types of the elements except when 
        we access properties. 
        StringTemplate use the "to string" evaluation function appropriate for the implementation language to evaluate objects.

        InLine Templates (Anonymous Template):
        Sometimes creating a separate template definition is too much effort for a one-off template or a really small one. In those 
        cases, we can use anonymous templates (or subtemplates). Anonymous templates are templates without a name enclosed in curly 
        braces. They can have arguments. 

        #####################################
        test(name) ::= "<name:{x | [<x>]}; separator=\", \">"
        #####################################
        Anonymous template {x | [<x>]} is the in-lined version of bracket(). Argument names are separated from the template with the 
        | pipe character.         
        */


        /*
         StringTemplate will iterate across any object that it can reasonably interpret as a collection of elements such as arrays, lists, 
         dictionaries and, in statically typed ports, objects satisfying iterable enumeration interfaces. 

        Use of StringTemplate for formatting: 
        */

        public void FormattingWithStringTemplate()
        {
            int[] num = new int[] {3,9,20,2,1,4,6,32,5,6,77,888,2,1,6,32,5,6,77,
                                    4,9,20,2,1,4,63,9,20,2,1,4,6,32,5,6,77,6,32,5,6,77,
                                    3,9,20,2,1,4,6,32,5,6,77,888,1,6,32,5};
            string t = Template.Format(30, "int <1>[] = { <2; wrap, anchor, separator =\", \"> };", "a", num);
            Console.WriteLine(t);
        }



        /*
         ######## TEMPLATE #################
        text
        <expr>
        <! comment !>
        ####################################
        Escape delimeters with backslash character. \< or \>
        <...> is default delimit expressions, but you can use any single start and stop character. 
        The four canonical attribute expression operations are:
        1. attribute reference : <name>
        2. template include : <supportcode()>
        3. conditional include: <if(trace)>print("enter function"):<endif>
        4. template application (ie map operation) : <vars:decl()>


        ###################### Expression Literals #################################
        true = boolean true value 
        false = boolean false value 
        char = char -> space |\n|\r|\t|\uXXXX
        \\ = Ignore the immediately following newline char. Allows you to put a newline in the template to better format it without actually 
             inserting a newline into the output.
        "string" = a string of output characters 
        {template} = an anonymous subtemplate 
        { args | template } = an anonymous subtemplate with arguments
        [] = an an empty list 
        [expr1, expr2, ... , exprN] = a list with N values, it behaves like an array or list injected from controller code

        ##################### Attribute Expression ##################################
        1. a 
        look up a and convert it to string. Looks up the enclosing tempalte chain searching for a. looks at the template that invoked the 
        current template. If does not exist, it is empty string. 

        2. ( expr )
        evaluaes expr to a string. 

        3. expr.p 
        get property p of expr 

        4. expr1.(expr2) 
        evaluate expr2 to a string and use that as the name of property


        ################## TEMPLATE INCLUDE #############################


        */





    }
}
