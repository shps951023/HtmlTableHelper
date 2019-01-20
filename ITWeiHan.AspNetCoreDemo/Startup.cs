using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using HtmlTableHelper;

namespace ITWeiHan.AspNetCoreDemo
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Run(async (context) =>
            {
                var sourceData = new[] {
                    new Dictionary<string, object> (){
                        {"Name" , "ITWeiHan" },{"Age",25},{"Country","Taiwan"}
                    }
                };
                var tablehtml = sourceData.ToHtmlTable();
                await context.Response.WriteAsync(tablehtml);
            });
        }
    }
}
