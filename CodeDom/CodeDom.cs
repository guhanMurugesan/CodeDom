using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeDom
{
    class CodeDom
    {
        private CodeCompileUnit codeComUnit;
        private CodeTypeDeclaration codeType;
        public CodeDom()
        {
            CodeNamespace codeNamespace = new CodeNamespace("AutoCodeGen");
            codeType = new CodeTypeDeclaration("AutoClass");

            codeType.IsClass = true;
            codeType.TypeAttributes = System.Reflection.TypeAttributes.Public | System.Reflection.TypeAttributes.Sealed;
            codeComUnit = new CodeCompileUnit();
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            codeNamespace.Types.Add(codeType);
            codeComUnit.Namespaces.Add(codeNamespace);
            
            AddFields();
            AddProperties();
            AddMethod();
            AddConstructor();
            AddEntryPoint();
        }

        private void AddFields()
        {
            CodeMemberField nameField = new CodeMemberField() { Attributes = MemberAttributes.Private, Name = "_Name", Type = new CodeTypeReference(typeof(System.String)) };
            nameField.Comments.Add(new CodeCommentStatement("name of the class"));
            codeType.Members.Add(nameField);
        }

        private void AddProperties()
        {
            CodeMemberProperty propertyField = new CodeMemberProperty() { Name = "Name", Type = new CodeTypeReference(typeof(System.String)), Attributes = MemberAttributes.Public | MemberAttributes.Final, HasGet = true };
            propertyField.Comments.Add(new CodeCommentStatement("expose name of the class to the world"));
            propertyField.GetStatements.Add( new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_Name")));

            codeType.Members.Add(propertyField);
        }

        private void AddMethod()
        {
            CodeMemberMethod codeMethod = new CodeMemberMethod() { Name="Print",Attributes=MemberAttributes.Public|MemberAttributes.Final };
            codeMethod.Comments.Add(new CodeCommentStatement("Get the formatted name"));
            codeMethod.ReturnType = new CodeTypeReference("System.String");

            CodeFieldReferenceExpression nameField = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Name");

            CodeMethodReturnStatement returnStatement = new CodeMethodReturnStatement();
            returnStatement.Expression = new CodeMethodInvokeExpression( new CodeTypeReferenceExpression("System.String"),"Format",new CodePrimitiveExpression("Name:{0}"),nameField);

            codeMethod.Statements.Add(returnStatement);

            codeType.Members.Add(codeMethod);
        }

        private void AddConstructor()
        {
            CodeConstructor codeConstructor = new CodeConstructor() { Attributes = MemberAttributes.Public};

            CodeFieldReferenceExpression field = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_Name");
            codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(System.String),"Name"));
            codeConstructor.Statements.Add(new CodeAssignStatement(field,new CodeArgumentReferenceExpression("Name")));

            codeType.Members.Add(codeConstructor);
        }

        private void AddEntryPoint()
        {
            CodeEntryPointMethod entryPointMethod = new CodeEntryPointMethod();

            CodeObjectCreateExpression objExp = new CodeObjectCreateExpression(new CodeTypeReference("AutoClass"), new CodePrimitiveExpression("HelloWorld"));

            entryPointMethod.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference("AutoClass"), "obj", objExp));

            CodeMethodInvokeExpression methExp1 = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("obj"), "Print");

            entryPointMethod.Statements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("System.Console"),"Write",methExp1));

            codeType.Members.Add(entryPointMethod);
        }

        public void GenerateCSharpCode(string filename)
        {
            CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions codeOptions = new CodeGeneratorOptions();
            codeOptions.BracingStyle = "C";
            using (StreamWriter writer = new StreamWriter(filename))
            {
                codeProvider.GenerateCodeFromCompileUnit(codeComUnit, writer, codeOptions);
            }
        }

        public bool CompileAutoGenCode()
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();

            CompilerParameters cp = new CompilerParameters();

            cp.GenerateExecutable = true;
            cp.GenerateInMemory = false;
            cp.OutputAssembly = "AutoCodeDom";
            cp.ReferencedAssemblies.Add("System.dll");

            CompilerResults cr = provider.CompileAssemblyFromFile(cp,"SampleCode.cs");

            Console.WriteLine("Error count:{0}",cr.Errors.Count);

            if (cr.Errors.Count == 0)
            {
                Console.WriteLine("Built successfully!");
                return true;
            }
            else
                foreach (var item in cr.Errors)
                {
                    Console.WriteLine(item);
                }
            return false;
        }

    }
}
