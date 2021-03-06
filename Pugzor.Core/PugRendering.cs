﻿using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Pugzor.Core.Abstractions;
using Pugzor.Core.Helpers;
using Microsoft.Extensions.Options;

namespace Pugzor.Core
{
    public class PugRendering : IPugRendering
    {
        private readonly INodeServices _nodeServices;
        private PugzorViewEngineOptions _options;

        public PugRendering(INodeServices nodeServices, IOptions<PugzorViewEngineOptions> options)
        {
            _nodeServices = nodeServices;
            var tempDirectory = TemporaryDirectoryHelper.CreateTemporaryDirectory();
            EmbeddedFileHelper.ExpandEmbeddedFiles(tempDirectory);
            _options = options.Value;
        }

        public async Task<string> Render(FileInfo pugFile, object model, ViewDataDictionary viewData, ModelStateDictionary modelState)
        {
            var opts = _options != null ? new
            {
                pretty = _options.Pretty ? "\t" : null,
                basedir = _options.BaseDir
            } : new object();

            return await _nodeServices
                .InvokeAsync<string>("pugcompile", pugFile.FullName, model, viewData, modelState, opts)
                .ConfigureAwait(false);
        }
    }
}
