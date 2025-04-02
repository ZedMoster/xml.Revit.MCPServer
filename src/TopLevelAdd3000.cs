using Autodesk.Revit.UI;
using xml.Revit.MCP.Models;
using xml.Revit.MCP.Public;
using xml.Revit.Toolkit.Extensions;

namespace xml.Revit.MCPServer
{
    public sealed class TopLevelAdd3000 : IMCPMethod
    {
        public TopLevelAdd3000()
        {
            MethodName = "新增标高3000";
        }

        public string MethodName { get; private set; }

        public JsonRPCResponse Execute(JsonRPCRequest request, UIDocument uidoc)
        {
            var jsonResponse = new JsonRPCResponse();
            var doc = uidoc.Document;

            double elevationOffset = 3000d;
            var highestLevel = doc.OfClass<Level>().OrderBy(l => l.Elevation).LastOrDefault();
            if (highestLevel == null)
            {
                throw new InvalidOperationException("未找到有效的标高。");
            }

            doc.Transaction(t =>
            {
                var newLevel = Level.Create(
                    doc,
                    highestLevel.Elevation + elevationOffset.MMToFeet()
                );
                jsonResponse.Result = new ElementModelRequest(newLevel);
            });

            return jsonResponse;
        }
    }
}
