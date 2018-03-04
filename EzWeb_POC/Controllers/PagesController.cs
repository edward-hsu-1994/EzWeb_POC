using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RazorLight;
using RazorLight.Razor;

namespace EzWeb_POC.Controllers {

    public class MemoryTemplateProject : RazorLightProject {
        public static string[] templates = new string[] {
            "<div id=\"A\">@{ await IncludeAsync(\"1\", Model); }</div>",
            @"@for (var i = 0; i < Model.people.Length; i++)
                {
                    var person = Model.people[i];
                    <p>No.@(i+1) Name: @person</p>
                }
            "
        };
        public override Task<IEnumerable<RazorLightProjectItem>> GetImportsAsync(string templateKey) {
            return Task.FromResult(Enumerable.Empty<RazorLightProjectItem>());
        }

        /// <summary>
        /// 取得指定樣板
        /// </summary>
        /// <param name="templateKey">樣板唯一識別號</param>
        /// <returns></returns>
        public override async Task<RazorLightProjectItem> GetItemAsync(string templateKey) {
            return new TextSourceRazorProjectItem(templateKey, templates[int.Parse(templateKey)]);
        }
    }

    [Route("[controller]")]
    public class PagesController : Controller {
        public static RazorLightEngine Engine { get; private set; }

        public PagesController() {
            if (Engine == null) {
                var project = new MemoryTemplateProject();
                Engine = new RazorLightEngineBuilder()
                  .UseProject(project)
                  .Build();
            }
        }


        [HttpGet]
        public async Task<ContentResult> Get() {
            var html = await Engine.CompileRenderAsync(0.ToString(), new {
                people = new string[] {
                    "A","B","C","D"
                }
            });

            return new ContentResult() { Content = html, ContentType = "text/html" };
        }

        public override void OnActionExecuted(ActionExecutedContext context) {
            GC.Collect();
            base.OnActionExecuted(context);
        }
    }
}
