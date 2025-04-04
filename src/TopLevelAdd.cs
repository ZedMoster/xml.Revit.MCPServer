using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using xml.Revit.MCP.Models;
using xml.Revit.MCP.Public;
using xml.Revit.Toolkit.Attributes;
using xml.Revit.Toolkit.Extensions;
using xml.Revit.Toolkit.Utils;

namespace xml.Revit.MCPServer
{
    /// <summary>
    /// 实现新增标高功能的类，继承自 IMCPMethod 接口。
    /// </summary>
    public sealed class TopLevelAdd : IMCPMethod
    {
        /// <summary>
        /// 构造函数，初始化方法名称。
        /// </summary>
        public TopLevelAdd()
        {
            MethodName = "新增标高";
        }

        /// <summary>
        /// 方法名称，用于标识当前功能。
        /// </summary>
        public string MethodName { get; private set; }

        /// <summary>
        /// 执行新增标高的核心逻辑。
        /// </summary>
        /// <param name="request">包含请求参数的 JsonRPCRequest 对象。</param>
        /// <param name="uidoc">当前 Revit 的 UIDocument 对象。</param>
        /// <returns>返回一个 JsonRPCResponse 对象，包含执行结果。</returns>
        public JsonRPCResponse Execute(JsonRPCRequest request, UIDocument uidoc)
        {
            // 初始化响应对象
            var jsonResponse = new JsonRPCResponse();

            // 获取当前文档
            var doc = uidoc.Document;

            // 默认标高偏移量，单位为毫米
            double elevationOffset = 3000d;

            // 从请求参数中解析标高偏移量
            var obj = request.Params as JObject;
            if (obj != null)
            {
                elevationOffset = obj["offset"].Value<double>();
            }

            // 获取文档中的最高标高
            var highestLevel = doc.OfClass<Level>().OrderBy(l => l.Elevation).LastOrDefault();
            if (highestLevel == null)
            {
                throw new InvalidOperationException("未找到有效的标高。");
            }

            // 开启事务并创建新标高
            doc.Transaction(t =>
            {
                var newLevel = Level.Create(
                    doc,
                    highestLevel.Elevation + elevationOffset.MMToFeet() // 偏移量转换为英尺
                );

                // 将新标高信息封装到响应结果中
                jsonResponse.Result = new ElementModelRequest(newLevel);
            });

            return jsonResponse; // 返回响应对象
        }
    }
}
