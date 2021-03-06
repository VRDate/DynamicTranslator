﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abp.Dependency;

using DynamicTranslator.Application.Result;
using DynamicTranslator.Constants;
using DynamicTranslator.Domain.Model;

namespace DynamicTranslator.Application.Orchestrators.Organizers
{
    public class ResultOrganizer : IResultOrganizer, ITransientDependency
    {
        private readonly IResultService _resultService;

        public ResultOrganizer(IResultService resultService)
        {
            _resultService = resultService;
        }

        public Task<Maybe<string>> OrganizeResult(ICollection<TranslateResult> findedMeans, string currentString, out Maybe<string> failedResults)
        {
            var succeededResults = Organize(findedMeans, currentString, true);

            failedResults = Organize(findedMeans, currentString, false);

            return Task.FromResult(succeededResults);
        }

        private Maybe<string> Organize(ICollection<TranslateResult> findedMeans, string currentString, bool isSucceeded)
        {
            var mean = new StringBuilder();
            var results = findedMeans.Where(result => result.IsSuccess == isSucceeded);

            foreach (var result in results)
            {
                mean.AppendLine(result.ResultMessage.DefaultIfEmpty(string.Empty).First());
            }

            if (!string.IsNullOrEmpty(mean.ToString()))
            {
                var means = mean.ToString()
                                .Split('\r')
                                .Select(x => x.Trim().ToLower())
                                .Where(s => (s != string.Empty) && (s != currentString.Trim()) && (s != "Translation"))
                                .Distinct()
                                .ToList();

                mean.Clear();
                means.ForEach(m => mean.AppendLine($"{Titles.Asterix} {m.ToLower()}"));

                if (isSucceeded && means.Any())
                {
                    _resultService.SaveOrUpdateAsync(new CompositeTranslateResult(currentString, 1, findedMeans, DateTime.Now));
                }

                return new Maybe<string>(mean.ToString());
            }

            return new Maybe<string>();
        }
    }
}
