# 如何创建和生成 MCP DLL 文件以供 CallFunc 调用

本文档介绍如何创建一个 MCP DLL 文件，以供 CallFunc 方法调用。
MCP DLL 文件需要实现接口 xml.Revit.MCP.Public.IMCPMethod，
并按照 JSON-RPC 2.0 规范与 Revit 的 MCP 服务进行交互。

## MCP DLL 的结构和实现

创建一个实现类

实现 `IMCPMethod` 接口

以下是一个实现类的示例，用于创建标高并返回新标高`ElementModelRequest`：

```C#
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

```

在 Visual Studio 中编译项目，生成 .dll 文件。
编译后，确保生成的 .dll 文件位于指定的 MCP 文件夹中。
![mcp-dll.png](./img/mcp-dll.png)

通过以上步骤，您可以轻松创建和生成 MCP DLL 文件，并将其集成到 Revit 的 MCP 服务中进行动态调用。

![通过函数名称调用功能.png](./img/%E9%80%9A%E8%BF%87%E5%87%BD%E6%95%B0%E5%90%8D%E7%A7%B0%E8%B0%83%E7%94%A8%E5%8A%9F%E8%83%BD.png)
