using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tavis.Status;
using Xunit;

namespace StatusTests
{
    public class SimpleTests
    {

        [Fact]
        public void CreateEmptyStatusDocument()
        {
            var statusDoc = new StatusDocument();

            Assert.NotNull(statusDoc);
        }

        [Fact]
        public void CreateStatusDocument()
        {
            var statusDoc = new StatusDocument();
            statusDoc.Message = "Hello World";
            statusDoc.State = State.Ok;
            
            Assert.NotNull(statusDoc);
        }

        [Fact]
        public void AttributeWithNewLine()
        {
            var xDoc = new XDocument(new XElement("root",new XAttribute("foo","This is one line and " + Environment.NewLine+ "this is another")));
            var ms = new MemoryStream();
            xDoc.Save(ms);
            ms.Position = 0;
            var xDoc2 = XDocument.Load(ms);
            var value = xDoc2.Element("root").Attribute("foo").Value;
            Assert.Equal("This is one line and " + Environment.NewLine + "this is another", value);
        }

        [Fact]
        public void LoadStatusDocument()
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.Write("<status state='waiting' progress='1/10' message='hello world' />");
            sw.Flush();
            ms.Position = 0;

            var statusDoc = StatusDocument.LoadXml(ms);
            
            Assert.Equal(State.Waiting,statusDoc.State);
            Assert.Equal("hello world", statusDoc.Message);
            Assert.Equal("1/10", statusDoc.Progress);
        }

        [Fact]
        public void RoundTripDocument()
        {
            var doc = new StatusDocument();
            doc.State = State.Busy;
            doc.Message = "I'm working!!!";
            doc.Progress = "1/999999";

            var ms = new MemoryStream();
            doc.SerializeXmlToStream(ms);

            ms.Position = 0;

            var newDoc = StatusDocument.LoadXml(ms);

            Assert.Equal(doc.Message, newDoc.Message);
            Assert.Equal(doc.State, newDoc.State);
            Assert.Equal(doc.Progress, newDoc.Progress);
        }


    }
}
