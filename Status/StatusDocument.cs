using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Tavis.Status
{
    public enum State {
                    Waiting, // Resource is waiting on some kind of external input
                    Error, // In some kind of failure state, see message
                    Warning, //  Ok, but some problem condition detected
                    Busy, // Something is happening, comeback and check later
                    Ok //  All is good
        }

    public class StatusDocument
    {
        private string _progress;
        public const string StatusXmlMediaType = "application/status+xml";
        public const string StatusJsonMediaType = "application/status+json";

        public string Message { get; set; }  // human readable representation of current state

        public int MaxProgress { get; private set; }
        public int CurrentProgress { get; private set; }
        public decimal PercentComplete { get; private set; }

        // represented as a fraction  2/45, used for busy or waiting  
        public State State { get; set; }  // [required]

        public string Progress
        {
            get { return _progress; }
            set { _progress = value;
                ParseProgress();
            }
        }

        private void ParseProgress()
        {
            if (!String.IsNullOrEmpty(_progress))
            {
                var parts = _progress.Split('/');
                CurrentProgress = int.Parse(parts[0]);
                MaxProgress = int.Parse(parts[1]);
                PercentComplete = CurrentProgress/((decimal) MaxProgress);
            }
            else
            {
                CurrentProgress = 0;
                MaxProgress = 0;
                PercentComplete = 0;
            }


        }

        public static StatusDocument LoadXml(Stream stream)
        {
            var doc = XDocument.Load(stream);

            
            var statusElement = doc.Element("status");
            
            if (statusElement == null) throw new FormatException("Root element must be status");

            var statusDoc = new StatusDocument();

            foreach (var attribute in statusElement.Attributes())
            {
                if (attribute.Name == "state")
                {
                    var value = attribute.Value.ToLower();
                    switch (value)
                    {
                        case "waiting": statusDoc.State = State.Waiting;
                            break;
                        case "error": statusDoc.State = State.Error;
                            break;
                        case "warning": statusDoc.State = State.Warning;
                            break;
                        case "busy": statusDoc.State = State.Busy;
                            break;
                            
                    }
          
                } else if (attribute.Name == "progress")
                {
                    statusDoc.Progress = attribute.Value;
                }
                else if (attribute.Name == "message")
                {
                    statusDoc.Message = attribute.Value;
                }
                
            }

            return statusDoc;
        }

        public void SerializeXmlToStream(Stream stream)
        {
            var writer = XmlWriter.Create(stream);
            writer.WriteStartDocument();
            writer.WriteStartElement("status");
            writer.WriteAttributeString("state",Enum.GetName(typeof(State), State).ToLower());
            writer.WriteAttributeString("progress", Progress);
            writer.WriteAttributeString("message", Message);

            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Flush();

        }
        public StatusDocument()
        {
            Message = string.Empty;
            Progress = string.Empty;
            State = State.Ok;
        }


    }

    
}
